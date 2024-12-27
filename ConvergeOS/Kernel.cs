using System;
using System.Collections.Generic;
using System.Text;
using Sys = Cosmos.System;

using CommandProc;

namespace Kernel
{
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
        /// Determines if apps run by the user can access the user's media files (Documents). Commands can always access these areas. Can be edited without suser.
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
        INTERNET = 0x80,

        /// <summary>
        /// Bit flag: 0xFF.
        /// 
        /// All flags, for convenience.
        /// </summary>
        ALL = 0xFF
    }

    /// <summary>
    /// A type for adding information about users, such as their name or permissions.
    /// </summary>
    public class User
    {
        private string username;
        private string password;
        private string user_file_path; // As yet unused
        private bool is_user_fs_ready;
        private int permissions;

        /// <summary>
        /// Sets up a blank, basic user
        /// </summary>
        public User()
        {
            username = "Guest";
            password = "";
            user_file_path = "";
            is_user_fs_ready = false;
            permissions = 0; // No permissions
        }

        /// <summary>
        /// Sets up a user with a name and permissions.
        /// </summary>
        /// <param name="username">The username.</param>
        /// <param name="password">The password.</param>
        /// <param name="permissions">Any permissions the user has.</param>
        public User(string username, string password, int permissions)
        {
            this.username = username;
            this.password = password;
            this.user_file_path = "";
            this.is_user_fs_ready = false;
            this.permissions = permissions;
        }

        /// <summary>
        /// Get the username of the user.
        /// </summary>
        /// <returns>The username</returns>
        public string GetUsername()
        {
            return username;
        }

        /// <summary>
        /// Sets the user's username.
        /// </summary>
        /// <param name="new_username">The new username.</param>
        public void SetUsername(string new_username)
        {
            username = new_username;

            // Set the name for the user's media files
            if (user_file_path == "")
            {
                user_file_path = new_username;
                is_user_fs_ready = true;
            }
        }

        /// <summary>
        /// Checks  to see if the entered password matches the user's password.
        /// </summary>
        /// <param name="attempted_password">The password to check</param>
        /// <returns>Whether the password matches</returns>
        public bool MatchesPassword(string attempted_password)
        {
            if (attempted_password == password || password == "")
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Sets a new password for the user.
        /// </summary>
        /// <param name="new_password">The password to set to.</param>
        public void SetPassword(string new_password)
        {
            password = new_password;
        }

        /// <summary>
        /// Get the user's permissions.
        /// </summary>
        /// <returns>The user's permissions.</returns>
        public int GetPermissions()
        {
            return permissions;
        }

        /// <summary>
        /// Gets only one permission's value
        /// </summary>
        /// <param name="permission">The permission to get.</param>
        /// <returns>The state of that permission.</returns>
        public bool GetOnePermission(int permission)
        {
            int is_perm = permission & permissions;
            return is_perm != 0;
        }

        /// <summary>
        /// Set a user's permissions.
        /// </summary>
        /// <param name="permissions_to_change">The permissions as bit flags to change.</param>
        /// <param name="new_value">Whether to set the permissions to true or false.</param>
        public void SetPermissions(int permissions_to_change, bool new_value)
        {
            // Hope this works!
            if (new_value)
            {
                permissions |= permissions_to_change;
            }
            else
            {
                permissions &= ~permissions_to_change;
            }
        }
    }


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
        /// Reports whether there is an AspectOS Converge filesystem available to use. As we don't have FS support as of r13, we set this to false and don't change it later.
        /// </summary>
        public static bool is_aos_fs_installed = false;

        /// <summary>
        /// Stores the password of the disk user at a low level where it is hard to find. The implementation should change this before compiling. Disabled when a FS is detected.
        /// </summary>
        public static string disk_user_password = "DiskUser";

        /// <summary>
        /// Returns current revision.
        /// </summary>
        /// <returns>Current revision as an int</returns>
        public static int GetRevision()
        {
            return 14;
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
        /// Overridden COSMOS function. DO NOT CALL! Sets up the OS before we enter the CLI.
        /// </summary>
        protected override void BeforeRun()
        {
            try
            { 
                // Setup basic users
                List<User> users = new List<User>();
                users.Add(new User());
                users.Add(new User("Disk", disk_user_password, (int)UserPermissions.ALL));

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

                // Setup users
                if (!is_aos_fs_installed)
                {
                    CLIDispLine("There is no AspectOS hard drive installed! Would you like to boot as Disk User? (y/n)");
                    string result = CLIRead();
                    if (result == "y" || result == "Y")
                    {

                    }
                }

                // Tell the user we have debug on
                if (is_debug == true)
                {
                    Console.Write("Debug is ");
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("ON");
                    Console.ForegroundColor = ConsoleColor.White;
                }
            }
            catch (Exception ex)
            {
                // Big thank-you to Andrew hare for this code from https://stackoverflow.com/a/4272601, which has been modified for this namespace
                var stringBuilder = new StringBuilder();

                while (ex != null)
                {
                    stringBuilder.AppendLine(ex.Message);
                    stringBuilder.AppendLine(ex.StackTrace);

                    ex = ex.InnerException;
                }

                var msg = stringBuilder.ToString();
                // End Andrew's code

                DisplayError("\nC# Error:\n\n" + msg);
            }
        }

        /// <summary>
        /// Overriden COSMOS function. DO NOT CALL!
        /// </summary>
        protected override void Run()
        {
            try
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
                    }
                    else
                    {
                        DisplayWarningOrDebug("\nCommand finished with " + code.ToString() + " errors!");
                    }
                }
            }
            catch (Exception ex)
            {
                // Big thank-you to Andrew hare for this code from https://stackoverflow.com/a/4272601, which has been modified for this namespace
                var stringBuilder = new StringBuilder();

                while (ex != null)
                {
                    stringBuilder.AppendLine(ex.Message);
                    stringBuilder.AppendLine(ex.StackTrace);

                    ex = ex.InnerException;
                }

                var msg = stringBuilder.ToString();
                // End Andrew's code


                DisplayError("\nC# Error:\n\n" + msg);
            }
        } 
    }
}

