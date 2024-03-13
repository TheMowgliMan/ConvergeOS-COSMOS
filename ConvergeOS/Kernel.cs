using System;
using System.Collections.Generic;
using System.Text;
using Sys = Cosmos.System;

using CommandProc;

namespace Kernel
{
    /// <summary>
    /// Low level processes, such as Convergence loading or command-line. Main class that calls all others, but has some public methods and fields which can be called by other classes.
    /// </summary>
    public class Kernel : Sys.Kernel
    {
        /// <summary>
        /// Contains debug status.
        /// </summary>
        public static bool is_debug = true;

        /// <summary>
        /// Returns current revision.
        /// </summary>
        /// <returns>Current revision as an int</returns>
        public static int GetRevision()
        {
            return 11;
        }

        /// <summary>
        /// Displays an error. Doesn't actually raise any errors, however.
        /// </summary>
        /// <param name="msg">Error message</param>
        public static void DisplayError(string msg)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(msg);
            Console.ForegroundColor = ConsoleColor.White;

            return;
        }

        /// <summary>
        /// Display a line of yellow text.
        /// </summary>
        /// <param name="msg">Warning or debug message</param>
        public static void DisplayWarningOrDebug(string msg)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(msg);
            Console.ForegroundColor = ConsoleColor.White;

            return;
        }

        /// <summary>
        /// Display yellow text, without a newline.
        /// </summary>
        /// <param name="msg">Warning or debug message</param>
        public static void DisplayWarningOrDebugPartial(string msg)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write(msg);
            Console.ForegroundColor = ConsoleColor.White;
        }

        /// <summary>
        /// Displays text on the CLI. Doesn't send a newline at the end.
        /// </summary>
        /// <param name="msg">Message to write</param>
        public static void CLIDisp(string msg)
        {
            Console.Write(msg);
        }

        /// <summary>
        /// Displays text on the CLI and sends a newline.
        /// </summary>
        /// <param name="msg">Message to write</param>
        public static void CLIDispLine(string msg)
        {
            Console.WriteLine(msg);
        }

        /// <summary>
        /// Reads a line on the CLI.
        /// </summary>
        /// <returns>User input from CLI</returns>
        public static string CLIRead()
        {
            return Console.ReadLine();
        }

        /// <summary>
        /// Overridden COSMOS function. DO NOT CALL!
        /// </summary>
        protected override void BeforeRun()
        {
            // Get our startup text
            Console.Write("AspectOS Converge r");
            Console.Write(GetRevision());
            Console.WriteLine(" pre-alpha");
        }

        /// <summary>
        /// Overriden COSMOS function. DO NOT CALL!
        /// </summary>
        protected override void Run()
        {
            // Get input
            Console.Write("?>");
            string cmd = Console.ReadLine();

            int code = CommandProc.CommandProc.Process(cmd);
            if (code != 0)
            {
                if (code == -1)
                {
                    System.Environment.Exit(0);
                } else
                {
                    DisplayWarningOrDebug("\nCommand finished with " + code.ToString() + " errors!");
                }
            }
        } 
    }
}

