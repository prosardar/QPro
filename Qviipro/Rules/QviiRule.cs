using System;

namespace Qviipro.Rules {
    public enum QviiBehavior {
        Skip,
        Redirect,
        Block
    }

    public abstract class QviiRule {
        protected QviiRule() {
            Guid = Guid.NewGuid();
        }

        public Guid Guid { get; set; }

        public QviiBehavior Behavior { get; set; }

        public string Pattern { get; set; }

        public string RedirectPattern { get; set; }

        public string Exceptions { get; set; }

        public bool IsStoreResponse { get; set; }

        public bool IsAllStoreResponse { get; set; }

        public abstract bool IsAccept(string url);
        
        public abstract string Redirect(string url);
    }
}
