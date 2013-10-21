﻿using System;
using System.Net;
using System.Net.Sockets;

namespace Qviipro {
    /// <summary>
    /// </summary>
    public class BaseProxy {
        private readonly ProxyChanger proxyChanger;
        private readonly QviiServer qviiServer;

        /// <summary>
        /// </summary>
        public BaseProxy() {
            proxyChanger = new ProxyChanger();

            qviiServer = new QviiServer(useIPv6: false) {
                OnReceiveRequest = OnReceiveRequest,
                OnReceiveResponse = OnReceiveResponse
            };
        }

        protected bool IsShuttingDown { get; private set; }

        /// <summary>
        /// </summary>
        protected virtual void OnReceiveResponse(TransferItem item) {
            throw new NotImplementedException();
        }

        /// <summary>
        /// </summary>
        protected virtual void OnReceiveRequest(TransferItem item) {
            throw new NotImplementedException();
        }

        /// <summary>
        ///     Запускает прослушивание
        /// </summary>
        public void Start() {
            var hostEndPoint = qviiServer.Start();
            proxyChanger.SetNewProxy(hostEndPoint);
        }

        /// <summary>
        ///     Останавливает прослушивание
        /// </summary>
        public void Stop() {
            proxyChanger.ResetProxy();
            qviiServer.Stop();
        }
    }
}