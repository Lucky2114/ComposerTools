using System.Diagnostics;

namespace ComposerTools.Classes
{
    class Logger
    {
        public static void Log(string message)
        {
#if DEBUG

            Debug.Print(message);
#endif
        }
    }
}
