﻿using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.Caching;

namespace Qviipro {
    public struct QviiItemCache {
        public string Key { get; set; }
        public bool IsStore { get; set; }
        public string RequestLine { get; set; }
        public object Response { get; set; }
        public Guid ResponseId { get; set; }
        public Guid RuleGuid { get; set; }
    }

    public class QviiCache {
        private readonly MemoryCache cache;

        private readonly ConcurrentDictionary<string, Guid> ruleGuidLinkIndex = new ConcurrentDictionary<string, Guid>();
        private readonly ConcurrentDictionary<Guid, string> responseGuidLinkIndex = new ConcurrentDictionary<Guid, string>();
        private readonly ConcurrentDictionary<string, string> urlLinkIndex = new ConcurrentDictionary<string, string>();

        private readonly object sync = new object();

        public QviiCache() {
            cache = new MemoryCache("QviiCache");
        }

        ~QviiCache() {
            cache.Dispose();
        }

        public void Add(Guid ruleGuid, TransferItem transferItem, bool storeIsAll) {
            var requestLine = transferItem.HttpRequestLine.URL;
            string index;
            if (storeIsAll == false) {
                if (ContainsItem(ruleGuid, requestLine, out index)) {
                    lock (sync) {
                        RemoveItemFromCache(index, requestLine);
                    }
                }
            }
            index = Guid.NewGuid().ToString();
            var item = new QviiItemCache() {
                RuleGuid = ruleGuid,
                ResponseId = transferItem.BrowserSocket.guid,
                RequestLine = requestLine,
                Response = null,
                IsStore = storeIsAll,
                Key = index
            };

            var policy = new CacheItemPolicy();
            cache.Add(index, item, policy);
            responseGuidLinkIndex.TryAdd(item.ResponseId, index);
            urlLinkIndex.TryAdd(index, item.RequestLine);
            ruleGuidLinkIndex.TryAdd(index, item.RuleGuid);
        }

        public void SetValue(Guid responseId, object response) {
            lock (sync) {
                string index;
                responseGuidLinkIndex.TryGetValue(responseId, out index);
                if (index == null) {
                    Debug.WriteLine("Имеются пересекающиеся шаблоны правил");
                    return;
                }
                var item = (QviiItemCache)cache[index];
                item.Response = response;
                cache[index] = item;
            }
        }

        public List<QviiItemCache> GetItems(Guid ruleId) {
            var items = GetItems<string, Guid>(ruleGuidLinkIndex, ruleId);
            return items;
        }

        public List<QviiItemCache> GetItems(string url) {
            var items = GetItems<string, string>(urlLinkIndex, url);
            return items;
        }

        public void Remove(string key) {
            RemoveItemFromCache(key);
        }

        private bool ContainsItem(Guid ruleGuid, string url, out string index) {
            index = "";
            var indexes = new List<string>();
            foreach (var ruleKey in ruleGuidLinkIndex) {
                if (ruleKey.Value == ruleGuid) {
                    string url1;
                    if (urlLinkIndex.TryGetValue(ruleKey.Key, out url1)) {
                        if (url1 == url) {
                            indexes.Add(ruleKey.Key);
                        }
                    }
                }
            }
            int countIndexes = indexes.Count;
            if (countIndexes == 0) {
                return false;
            }
            if (countIndexes == 1) {
                index = indexes[0];
                return true;
            }
            var item = (QviiItemCache)cache[indexes[0]];
            if (item.IsStore == false) {
                throw new Exception("В кеш содержится много подобных элементов, в то время когда они не для хранения.");
            }
            return true;
        }

        private void RemoveItemFromCache(string index, string requestLine = null) {
            cache.Remove(index);
            Guid g;
            ruleGuidLinkIndex.TryRemove(index, out g);
            var oldResponseId = Guid.Empty;
            foreach (var responseKey in responseGuidLinkIndex) {
                if (responseKey.Value == index) {
                    oldResponseId = responseKey.Key;
                    break;
                }
            }
            string oldReI;
            responseGuidLinkIndex.TryRemove(oldResponseId, out oldReI);
            if (oldReI != index) {
                throw new Exception("Ошибка в индексе при удалении элемента responseId из кеша");
            }
            string u;
            urlLinkIndex.TryRemove(index, out u);

            if (requestLine != null && u != requestLine) {
                throw new Exception("Ошибка в индексе при удалении элемента url из кеша");
            }
        }

        private List<QviiItemCache> GetItems<TKey, TValue>(IEnumerable array, TValue compareValue) {
            var items = new List<QviiItemCache>();
            foreach (KeyValuePair<TKey, TValue> ruleGuidPair in array) {
                if (ruleGuidPair.Value.Equals(compareValue) == false) {
                    continue;
                }
                var cacheItem = (QviiItemCache)cache[ruleGuidPair.Key.ToString()];
                items.Add(cacheItem);
            }
            return items;
        }
    }
}