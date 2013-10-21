namespace Qviipro.Rules {
   public class QviiConcreteRule : QviiRule {

        public override bool IsAccept(string url) {
            return url == Pattern;
        }

        public override string Redirect(string url) {
            if (Behavior != QviiBehavior.Redirect) {
                return url;
            }
            return RedirectPattern;
        }
    }
}
