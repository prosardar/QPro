namespace Qviipro {
    public struct TransferItem {
        public RequestProcessingState State;

        public HttpHeaders Headers;

        public HttpRequestLine HttpRequestLine;

        public HttpStatusLine ResponseStatusLine;

        public SocketState BrowserSocket;

        public SocketState RemoteSocket;

        public QviiTransfer Transfer;
    }
}
