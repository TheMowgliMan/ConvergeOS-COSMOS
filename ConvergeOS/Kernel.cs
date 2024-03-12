using System;
using System.Collections.Generic;
using System.Text;
using Sys = Cosmos.System;

namespace ConvergeOS
{
    public class Kernel : Sys.Kernel
    {
        // Is debug on?
        public bool is_debug = true;

        // get revision
        protected static int GetRevision()
        {
            return 8;
        }

        // output error
        protected static void Error(string msg)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(msg);
            Console.ForegroundColor = ConsoleColor.White;

            return;
        }

        protected override void BeforeRun()
        {
            // Get our startup text
            Console.Write("ConvergeOS r");
            Console.Write(GetRevision());
            Console.WriteLine(" pre-alpha copyright 2024 Nikolai Kopec");
        }

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
                        Error("Too many arguments for 'debug'!");
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
                            Error("Incorrect arguments for 'debug'!");
                        }
                    }
                }
                else
                {
                    Error("Command '" + cmd_split[0] + "' does not exist");
                }

                // debug the command
                if (is_debug == true)
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.Write("\nCommand debug info:");
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

