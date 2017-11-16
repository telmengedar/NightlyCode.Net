using System;
using System.Linq;
using System.Reflection;

namespace NightlyCode.Net {
    internal class Logger {
        static readonly Action<object, string, string> info = (s, m, d) => { };

        static readonly Action<object, string, string> warning = (s, m, d) => { };

        static readonly Action<object, string, Exception> error = (s, m, d) => { };

        static Logger() {
            if (Assembly.GetEntryAssembly()?.GetReferencedAssemblies().Any(a => a.Name == "NightlyCode.Core") ?? false)
            {
                Type loggertype = Type.GetType("NightlyCode.Core.Logs.Logger");
                if (loggertype == null)
                    loggertype = Assembly.Load(Assembly.GetEntryAssembly()?.GetReferencedAssemblies().First(a => a.Name == "NightlyCode.Core"))?.GetType("NightlyCode.Core.Logs.Logger");

                if (loggertype != null)
                {
                    MethodInfo infomethod = loggertype.GetMethod("Info", new[] { typeof(object), typeof(string), typeof(string) });
                    if (infomethod != null)
                        info = (sender, message, details) => infomethod.Invoke(null, new[] { sender, message, details });

                    MethodInfo warningmethod = loggertype.GetMethod("Warning", new[] { typeof(object), typeof(string), typeof(string) });
                    if (warningmethod != null)
                        warning = (sender, message, details) => warningmethod.Invoke(null, new[] { sender, message, details });

                    MethodInfo errormethod = loggertype.GetMethod("Error", new[] { typeof(object), typeof(string), typeof(Exception) });
                    if (errormethod != null)
                        error = (sender, message, details) => errormethod.Invoke(null, new[] { sender, message, details });
                }
            }
        }

        public static void Info(object sender, string message, string details = null) {
            info(sender, message, details);
        }

        public static void Warning(object sender, string message, string details = null) {
            warning(sender, message, details);
        }

        public static void Error(object sender, string message, Exception details = null) {
            error(sender, message, details);
        }
    }
}