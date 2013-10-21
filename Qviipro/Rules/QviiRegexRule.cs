using System.Text.RegularExpressions;

namespace Qviipro.Rules {
    public class QviiRegexRule : QviiRule {
      
        public override bool IsAccept(string url) {
            var regex = new Regex(Pattern);
            var matches = regex.Matches(url);
            return matches.Count > 0;
        }

        public override string Redirect(string url) {
            if (Behavior != QviiBehavior.Redirect) {
                return url;
            }
            var regex = new Regex(Pattern);
            return regex.Replace(url, RedirectPattern);
        }
    }
}
