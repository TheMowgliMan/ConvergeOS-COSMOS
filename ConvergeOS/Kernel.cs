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
        protected static int GetRevision()
        {
            return 7;
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
                } else if (cmd_split[0] == "debug") {
                    if (cmd_split.Length > 2){
                        Console.WriteLine("Too many arguments for 'debug'!");
                    } else
                    {
                        if (cmd_split[1] == "1")
                        {
                            is_debug = true;
                        } else if (cmd_split[1] == "0") { is_debug = false; } else
                        {
                            Console.WriteLine("Incorrect arguments for 'debug'!");
                        }
                    }
                } else
                {
                    Console.WriteLine("Command '" + cmd_split[0] + "' does not exist");

                    // debug the command
                    if (is_debug == true)
                    {
                        for (int i = 0; i < cmd_split.Length; i++)
                        {
                            Console.Write(cmd_split[i]);
                            Console.Write(" ");
                        }
                    }

                    Console.Write("\n");
                }
            } 
        }
    }
}
