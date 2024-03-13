using System;
using System.Collections.Generic;
using System.Text;
using Sys = Cosmos.System;

namespace ConvergeOS
{
    /// <summary>
    /// Low level processes. Main class that calls all others, but has some public methods and fields which can be called by other classes.
    /// </summary>
    public class Kernel : Sys.Kernel
    {
        /// <summary>
        /// Contains debug status.
        /// </summary>
        public bool is_debug = true;

        /// <summary>
        /// Returns current revision.
        /// </summary>
        /// <returns>Current revision as an int</returns>
        protected static int GetRevision()
        {
            return 11;
        }

        /// <summary>
        /// Displays an error. Doesn't actually raise any errors, however.
        /// </summary>
        /// <param name="msg">Error message</param>
        protected static void DisplayError(string msg)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(msg);
            Console.ForegroundColor = ConsoleColor.White;

            return;
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

            // Split it and process it
            string[] cmd_split = cmd.Split(' ');
            if (cmd_split.Length > 0)
            {
                if (cmd_split[0] == "echo")
                {
                    // Collect all non-command words and output them
                    for (int i = 1; i < cmd_split.Length; i++)
                    {
                        Console.Write(cmd_split[i]);
                        Console.Write(" ");
                    }

                    Console.Write("\n");
                }
                else if (cmd_split[0] == "debug")
                {
                    if (cmd_split.Length > 2)
                    {
                        DisplayError("Too many arguments for 'debug'!");
                    }
                    else
                    {
                        if (cmd_split[1] == "1")
                        {
                            is_debug = true;
                        }
                        else if (cmd_split[1] == "0") { is_debug = false; }
                        else
                        {
                            DisplayError("Incorrect arguments for 'debug'!");
                        }
                    }
                }
                else if (cmd_split[0] == "exit")
                {
                    System.Environment.Exit(0);
                }
                else
                {
                    DisplayError("Command '" + cmd_split[0] + "' does not exist");
                }

                // debug the command
                if (is_debug == true)
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.Write("\nCommand debug info:\n");
                    for (int i = 0; i < cmd_split.Length; i++)
                    {
                        Console.Write("cmd_split[" + i.ToString() + "] = '");
                        Console.Write(cmd_split[i]);
                        Console.Write("'\n");
                    }

                    Console.Write("\n");

                    // clear color
                    Console.ForegroundColor = ConsoleColor.White;
                }
            }
        } 
    }
}

