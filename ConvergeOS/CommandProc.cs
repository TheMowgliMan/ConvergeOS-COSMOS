using System;
using System.IO;
using Kernel;
using static Cosmos.HAL.BlockDevice.ATA_PIO;

namespace CommandProc
{
    /// <summary>
    /// Processes system commands, such as "echo" or "exit."
    /// </summary>
    public static class CommandProc
    {
        /// <summary>
        /// Processes a system command.
        /// </summary>
        /// <param name="command">The command to process</param>
        /// <returns>
        ///     Special return codes which the kernel uses to determine outcomes:
        ///     -1: Exit system;
        ///     0: Success
        ///     >1: Finished with errors
        /// </returns>
        public static int Process(string command)
        {
            // Code to return
            int code = 0;

            // Split the command and process it
            string[] cmd_split = command.Split(' ');
            if (cmd_split.Length > 0)
            {
                if (cmd_split[0] == "echo")
                {
                    // Collect all non-command words and output them
                    for (int i = 1; i < cmd_split.Length; i++)
                    {
                        Kernel.Kernel.CLIDisp(cmd_split[i]);
                        Kernel.Kernel.CLIDisp(" ");
                    }

                    Kernel.Kernel.CLIDisp("\n");
                }
                else if (cmd_split[0] == "debug")
                {
                    if (cmd_split.Length > 2)
                    {
                        Kernel.Kernel.DisplayError("Too many arguments for 'debug'!");
                        code += 1;
                    }
                    else if (cmd_split.Length == 0)
                    {
                        Kernel.Kernel.DisplayError("Too few arguments for 'debug'!");
                    }
                    else
                    {
                        if (cmd_split[1] == "1")
                        {
                            Kernel.Kernel.is_debug = true;
                        }
                        else if (cmd_split[1] == "0") { Kernel.Kernel.is_debug = false; }
                        else
                        {
                            Kernel.Kernel.DisplayError("Incorrect arguments for 'debug'!");
                            code += 1;
                        }
                    }
                }
                else if (cmd_split[0] == "exit")
                {
                    return -1;
                }
                else
                {
                    Kernel.Kernel.DisplayError("Command '" + cmd_split[0] + "' does not exist");
                    code += 1;
                }

                // debug the command
                if (Kernel.Kernel.is_debug == true)
                {
                    Kernel.Kernel.DisplayWarningOrDebug("\nCommand debug info:");
                    for (int i = 0; i < cmd_split.Length; i++)
                    {
                        Kernel.Kernel.DisplayWarningOrDebugPartial("cmd_split[" + i.ToString() + "] = '");
                        Kernel.Kernel.DisplayWarningOrDebugPartial(cmd_split[i]);
                        Kernel.Kernel.DisplayWarningOrDebugPartial("'\n");
                    }

                    Kernel.Kernel.DisplayWarningOrDebugPartial("\n");
                }
            }

            return code;
        }
    }
}