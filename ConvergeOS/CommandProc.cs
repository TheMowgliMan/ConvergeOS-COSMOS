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
                        code += 1;
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
                else if (cmd_split[0] == "list")
                {
                    Kernel.Kernel.CLIDisp("\ndebug\necho\nexit\nlist\nlogin\nlogout");
                }
                else if (cmd_split[0] == "logout")
                {
                    if (cmd_split.Length > 1)
                    {
                        Kernel.Kernel.DisplayError("Too many arguments for 'logout'!");
                        code += 1;
                    }

                    if (Kernel.Kernel.user_index == 0)
                    {
                        Kernel.Kernel.DisplayError("Can't logout from guest user!");
                        code += 1;
                    }

                    Kernel.Kernel.CLIDisp("Password? > ");
                    if (Kernel.Kernel.users[Kernel.Kernel.user_index].MatchesPassword(Kernel.Kernel.CLIRead()))
                    {
                        Kernel.Kernel.user_index = 0;
                    }
                }
                else if (cmd_split[0] == "login")
                {
                    if (cmd_split.Length != 2)
                    {
                        Kernel.Kernel.DisplayError("Wrong number of arguments for 'login'!");
                        code += 1;
                    }

                    var matched = false;
                    for (int i = 0; i < Kernel.Kernel.users.Count; i++)
                    {
                        if (Kernel.Kernel.users[i].GetUsername() == cmd_split[1])
                        {
                            Kernel.Kernel.CLIDisp("Password? > ");
                            if (Kernel.Kernel.users[i].MatchesPassword(Kernel.Kernel.CLIRead()))
                            {
                                Kernel.Kernel.user_index = i;
                            }
                        }
                    }

                    if (matched == false)
                    {
                        Kernel.Kernel.DisplayError("User " + cmd_split[1] + " does not exist!");
                    }
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