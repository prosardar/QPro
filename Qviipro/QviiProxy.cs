using System;
using System.Collections.Generic;
using Qviipro.Rules;

namespace Qviipro {
    public class QviiProxy : BaseProxy {
        private readonly QviiCache cache;

        public readonly List<QviiRule> Rules;
        public readonly List<QviiRule> BlackList;

        public QviiProxy() {
            cache = new QviiCache();

            Rules = new List<QviiRule>();
            BlackList = new List<QviiRule>();
        }

        public void AddRule(QviiRule rule) {
            switch (rule.Behavior) {
                case QviiBehavior.Skip:
                    Rules.Add(rule);
                    break;
                case QviiBehavior.Redirect:
                    Rules.Add(rule);
                    break;
                case QviiBehavior.Block:
                    BlackList.Add(rule);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public void RemoveRule(QviiRule rule) {
            switch (rule.Behavior) {
                case QviiBehavior.Skip:
                    Rules.Remove(rule);
                    break;
                case QviiBehavior.Redirect:
                    Rules.Remove(rule);
                    break;
                case QviiBehavior.Block:
                    BlackList.Remove(rule);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public void AddBlackItem() {

        }

        public void RemoveBlackItem() {

        }

        public List<QviiItemCache> GetItemsFromCacheByRule(QviiRule rule) {
            var cacheItems = new List<QviiItemCache>();
            cacheItems = cache.GetItems(rule.Guid);
            return cacheItems;
        }

        public List<QviiItemCache> GetItemsFromCacheByUrl(string url) {
            var cacheItems = new List<QviiItemCache>();
            cacheItems = cache.GetItems(url);
            return cacheItems;
        }

        public void RemoveItemFromCache(string key) {
            cache.Remove(key);
        }

        protected override void OnReceiveRequest(TransferItem item) {
            if (CheckOnBlackList(item) == false) {
                return;
            }
            CheckOnRules(item);
        }

        private bool CheckOnBlackList(TransferItem item) {
            // In BlackList must be rule with QviiBehavior = Block
            foreach (var rule in BlackList) {
                if (rule.IsAccept(item.HttpRequestLine.URI)) {
                    item.State.NextStep = null;
                    return false;
                }
            }
            return true;
        }

        private void CheckOnRules(TransferItem item) {
            foreach (var rule in Rules) {
                if (rule.IsAccept(item.HttpRequestLine.URL)) {
                    if (rule.IsStoreResponse) {
                        cache.Add(rule.Guid, item, rule.IsAllStoreResponse);
                    }
                    item.HttpRequestLine.URL = rule.Redirect(item.HttpRequestLine.URL);
                    break;
                }
            }
        }

        protected override void OnReceiveResponse(TransferItem item) {
            foreach (var rule in Rules) {
                if (rule.IsAccept(item.HttpRequestLine.URL)) {
                    if (rule.IsStoreResponse) {
                        var str = item.Transfer.GetContent();
                        if (string.IsNullOrEmpty(str) == false && item.ResponseStatusLine.StatusCode != 304) {
                            cache.SetValue(item.BrowserSocket.guid, str);
                        }
                        break;
                    }
                }
            }
        }
    }
}
