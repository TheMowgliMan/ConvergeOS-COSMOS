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
            return 12;
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
            // Clear console
            Console.Clear();

            // Get our startup text, add pretty colors
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.Write("AspectOS");

            Console.ForegroundColor = ConsoleColor.Blue;
            Console.Write(" Converge");

            Console.ForegroundColor = ConsoleColor.White;
            Console.Write(" r");

            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write(GetRevision());

            // We leave it color white
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(" pre-alpha");

            // Startup beep time!
            Console.Beep();

            // Tell the user we have debug on
            if (is_debug == true)
            {
                Console.Write("Debug is ");
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("ON");
                Console.ForegroundColor = ConsoleColor.White;
            }
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
                    Sys.Power.Shutdown();
                } else
                {
                    DisplayWarningOrDebug("\nCommand finished with " + code.ToString() + " errors!");
                }
            }
        } 
    }

    /// <summary>
    /// The bit flags for a user's permissions.
    /// </summary>
    [Flags]
    public enum UserPermissions
    {
        /// <summary>
        /// Bit flag: 0x1.
        /// 
        /// Determines if the user can use suser; if on, all other permissions are essentially available.
        /// </summary>
        SUSER = 0x1,

        /// <summary>
        /// Bit flag: 0x2.
        /// 
        /// Determines if the user can shut down the machine.
        /// </summary>
        EXIT = 0x2,

        /// <summary>
        /// Bit flag: 0x4.
        /// 
        /// Determines if apps or commands run by the user can create and access their own local files.
        /// </summary>
        FILE_SANDBOX = 0x4,

        /// <summary>
        /// Bit flag: 0x8.
        /// 
        /// Determines if apps run by the user can access the user's media files (Documents). Commands can always access this. Can be edited without suser.
        /// </summary>
        FILE_USER = 0x8,       // Should be 1 except for guest

        /// <summary>
        /// Bit flag: 0x10.
        /// 
        /// Determines if apps and commands run by the user can access general files.
        /// </summary>
        FILE_GENERAL = 0x10,

        /// <summary>
        /// Bit flag: 0x20.
        /// 
        /// Determines if apps and commands run by the user can access sensitive core files stored on the hard drive, such as the user permissions file.
        /// </summary>
        FILE_SENSITIVE = 0x20,

        /// <summary>
        /// Bit flag: 0x40.
        /// 
        /// Determines if apps run by the user can run suser commands irregardless of the user's actual permissions
        /// </summary>
        APP_ROOT = 0x40,       // Defines if apps run by this user can run as root, nomatter the other permissions

        /// <summary>
        /// Bit flag: 0x80.
        /// 
        /// Determines if apps and commands run by the user can access the internet. Some commands can access the internet, if implemented, nomatter the state of this flag.
        /// </summary>
        INTERNET = 0x80
    }

    /// <summary>
    /// A type for adding information abuot users, such as their name or permissions.
    /// </summary>
    public class User
    {
        
    }
}

