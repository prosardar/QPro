namespace Qviipro {
    public struct TransferItem {
        public SocketState BrowserSocket;
        public HttpHeaders Headers;

        public HttpRequestLine HttpRequestLine;

        public SocketState RemoteSocket;
        public HttpStatusLine ResponseStatusLine;
        public RequestProcessingState State;

        public QviiTransfer Transfer;
    }
}