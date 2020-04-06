using IPALogger = IPA.Logging.Logger;

namespace LevelTweaks
{
    internal static class Logger
    {
        internal static IPALogger log { get; set; }

        internal static void Log(this object obj) => log.Info(obj.ToString());
    }
}
