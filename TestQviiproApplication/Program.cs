using System;
using Qviipro;
using Qviipro.Rules;

namespace TestQviiproApplication {
    class Program {
        private static bool aborted;
        private static ConsoleUtils.ConsoleHandlerRoutine consoleHandlerRoutine;

        private static QviiProxy proxy;

        static void Main(string[] args) {
            InitializeConsoleHandlerRoutine();

            InitializeProxy();

            Console.WriteLine("Server started");
            Console.WriteLine("To exit press CTRL+C");
            proxy.Start();

            while (aborted == false) {
                var keyInfo = Console.ReadKey(true);
                if (keyInfo.Key == ConsoleKey.G) {
                    try {
                        var rule = proxy.Rules[0];
                        var items = proxy.GetItemsFromCacheByRule(rule);
                        foreach (var qviiItemCach in items) {
                            if (qviiItemCach.Response != null) {
                                Console.WriteLine("{0}", qviiItemCach.Response);
                                Console.WriteLine("\n");
                                proxy.RemoveItemFromCache(qviiItemCach.Key);
                            }
                        }
                    }
                    catch (Exception e) {
                        Console.WriteLine(e.Message);
                    }
                }
            }
        }

        private static void InitializeConsoleHandlerRoutine() {
            consoleHandlerRoutine = ConsoleHandlerRoutine;
            ConsoleUtils.SetConsoleCtrlHandler(consoleHandlerRoutine, true);
        }

        private static void InitializeProxy() {
            proxy = new QviiProxy();

            var rule = new QviiRegexRule();
            rule.Behavior = QviiBehavior.Redirect;
            rule.Pattern = @"http://www.yandex.ru+";
            rule.RedirectPattern = @"http://lenta.ru/";
            proxy.AddRule(rule);

            var rule2 = new QviiRegexRule();
            rule2.Behavior = QviiBehavior.Block;
            rule2.Pattern = @"http://images.yandex.ru+";
            proxy.AddRule(rule2);

            var rule3 = new QviiRegexRule();
            rule3.Behavior = QviiBehavior.Skip;
            rule3.IsStoreResponse = true;
            rule3.IsAllStoreResponse = true;
            rule3.Pattern = @"http://www.ntipay.ru+";
            proxy.AddRule(rule3);

            var rule4 = new QviiRegexRule();
            rule4.Behavior = QviiBehavior.Redirect;
            rule4.Pattern = @"(http://w.+dwar.+)&t=2";
            rule4.RedirectPattern = @"$1&t=1";
            proxy.AddRule(rule4);

            var rule5 = new QviiRegexRule();
            rule5.Behavior = QviiBehavior.Redirect;
            rule5.Pattern = @"(http://w.+dwar.+)user_social.php";
            rule5.RedirectPattern = @"$1action_run.phpp";
            proxy.AddRule(rule5);
        }

        private static bool ConsoleHandlerRoutine(CtrlTypes type) {
            switch (type) {
                case CtrlTypes.CTRL_C_EVENT:
                case CtrlTypes.CTRL_BREAK_EVENT:
                case CtrlTypes.CTRL_CLOSE_EVENT:
                case CtrlTypes.CTRL_LOGOFF_EVENT:
                case CtrlTypes.CTRL_SHUTDOWN_EVENT:
                    StoppingProxy();
                    break;
            }
            return true;
        }

        private static void StoppingProxy() {
            proxy.Stop();

            ConsoleUtils.SetConsoleCtrlHandler(consoleHandlerRoutine, false);
            Console.WriteLine("Server stopped");

            Console.WriteLine("Press any key to continue exit");

            aborted = true;
        }
    }
}
