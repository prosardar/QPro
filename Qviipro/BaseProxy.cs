using System;
using System.Net;

namespace Qviipro {
    /// <summary>
    /// </summary>
    public class BaseProxy {
        private readonly ProxyChanger proxyChanger;
        private readonly QviiServer qviiServer;
        private IPEndPoint hostEndPoint;

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
            proxyChanger.SetNewProxy(hostEndPoint);
            qviiServer.Start(hostEndPoint);
        }

        /// <summary>
        ///     Останавливает прослушивание
        /// </summary>
        public void Stop() {
            proxyChanger.ResetProxy();
            qviiServer.Stop();
        }

        #region Initializing
        /// <summary>
        ///     Инициализирует прокси-сервер с заданным портом для прослушивания
        /// </summary>
        /// <param name="port">Номер порта для прослушивания</param>
        public void Initialize(int port) {
            hostEndPoint = new IPEndPoint(IPAddress.Loopback, port);
        }

        /// <summary>
        ///     Инициализирует прокси-сервер с заданным адресом и портом для прослушивания
        /// </summary>
        /// <param name="ip">IP адрес</param>
        /// <param name="port">Номер порта для прослушивания</param>
        public void Initialize(string ip, int port) {
            var ipAddress = IPAddress.Parse(ip);
            hostEndPoint = new IPEndPoint(ipAddress, port);
        }

        /// <summary>
        ///     Инициализирует прокси-сервер с заданным адресом и портом для прослушивания
        /// </summary>
        /// <param name="ip">IP адрес</param>
        /// <param name="port">Номер порта для прослушивания</param>
        public void Initialize(IPAddress ip, int port) {
            hostEndPoint = new IPEndPoint(ip, port);
        }

        /// <summary>
        ///     Инициализирует прокси-сервер с заданной конечной точкой
        /// </summary>
        /// <param name="endPoint">Конечная точка для прослушивания</param>
        public void Initialize(IPEndPoint endPoint) {
            hostEndPoint = endPoint;
        }

        /// <summary>
        /// </summary>
        public void Uninitialize() {}
        #endregion
    }
}