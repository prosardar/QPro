using System;

namespace Qviipro {
    public class ItemBase {

        private readonly string source = String.Empty;

        public string Source {
            get {
                return source;
            }
        }

        public ItemBase(string source) {
            this.source = source;
        }
    }
}