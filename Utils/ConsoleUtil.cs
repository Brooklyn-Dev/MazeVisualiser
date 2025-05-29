using System.Runtime.InteropServices;

namespace MazeVisualiser.Utils
{
    public static class ConsoleUtil
    {
        // Import AttachConsole WinAPI to attach to parent process console
        [DllImport("kernel32.dll")]
        private static extern bool AttachConsole(int dwProcessId);

        // Import FreeConsole WinAPI to detach from current console
        [DllImport("kernel32.dll")]
        private static extern bool FreeConsole();

        private const int ATTACH_PARENT_PROCESS = -1;

        private static bool attached = false;

        // Attach to the parent console
        public static void BindConsole()
        {
            if (attached) return;  // Avoid attaching multiple times

            if (AttachConsole(ATTACH_PARENT_PROCESS))
            {
                // Redirect standard output and error streams to the attached console
                var writer = new StreamWriter(Console.OpenStandardOutput())
                {
                    AutoFlush = true  // Auto flush so output appears immediately
                };
                Console.SetOut(writer);
                Console.SetError(writer);
                attached = true;
            }
        }

        public static void CleanupConsole()
        {
            if (attached)
            {
                Console.Out.Flush();
                FreeConsole();
                attached = false;
            }
        }
    }
}