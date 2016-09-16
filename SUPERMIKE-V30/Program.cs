using System;
using ExploitFilter.NetEngine;
using System.Threading;
using System.IO;
using System.Data;
using System.Data.SqlClient;

namespace ExploitFilter
{
    class Program
    {
        #region Something about a pool thread shit no idea
        static void ConsolePoolThread()
        {
            while (true)
            {
                // NO MATTER IF U TYPE BIC OQE?
                string cmd = Console.ReadLine().ToLower();

                if (cmd == "/close")
                {
                    sqlCon.con.Close();
                }

                // Logging
                if (cmd == "/logging")
                {
                    if (FilterMain.logging == true)
                    {
                        FilterMain.logging = false;
                        Console.WriteLine(FilterMain.FILTER + "Log-save system disabled");
                    }
                    else
                    {
                        FilterMain.logging = true;
                        Console.WriteLine(FilterMain.FILTER + "Log-save system enable");
                    }
                }

                // Ban list
                if (cmd == "/banlist")
                {
                    Console.WriteLine("");
                    int n = 0;
                    foreach (string ban_list in FilterMain.ban_list)
                    {
                        Console.WriteLine(ban_list);
                        n++;
                    }
                    Console.WriteLine("Total blocked ip count: {0}", n);
                }

                // Reload bans
                if (cmd == "/banreload")
                {
                    // Reload bans
                    Program.Reloadbans();
                }

                if (cmd == "/help")
                {
                    Console.Clear();
                    Console.WriteLine("Active commands");
                    Console.WriteLine("-----------------------------------------------------");
                    Console.WriteLine("/help - Print all the existing commands.");
                    Console.WriteLine("/clear - Resets the console.");
                    Console.WriteLine("/logging - Toggles the log-save of unknown packets (on/off)");
                    Console.WriteLine("/reload - Reloads config.cfg");
                    Console.WriteLine("/reloadip - Clears IP Limit");
                    Console.WriteLine("/reopcodes - Reload opcodes");
                    Console.WriteLine("/banreload - Reload bans");
                    Console.WriteLine("/banlist - Show all active IP bans");
                    Console.WriteLine("/hwid - Gets your computer Hardware ID");
                    Console.WriteLine("/hclear - Clears you prestored hwid list");
                    Console.WriteLine("/nospam - Will disable the message showing that new filter is out!");
                    Console.WriteLine("-----------------------------------------------------");

                    Console.WriteLine();
                    Console.WriteLine();

                    Console.WriteLine("Admin commands");
                    Console.WriteLine("-----------------------------------------------------");
                    Console.WriteLine("/notice Message - Will send a notice to everyone online.");
                    Console.WriteLine("/ban Username/Charname - Will IP ban the target.");
                    Console.WriteLine("/disconnect Username/Charname - Will disconnect the target.");
                    Console.WriteLine("-----------------------------------------------------");
                }

                if (cmd == "/clear")
                {
                    Console.Clear();
                }

                if (cmd == "/reload")
                {
                    // Clear console
                    Console.Clear();

                    // Load settings
                    Config.LoadEverything(false);
                    Config.LoadEverything(true);

                    // Load regions
                    Program.LoadRegions();

                    // Load skills
                    Program.BlockedSkills();

                    // Load insult list
                    Program.Insult();

                    // Load opcodes
                    Program.LoadOpcodes();
                }

                if (cmd == "/reloadip")
                {
                    // Clear console
                    Console.Clear();

                    // Clean IP list
                    FilterMain.ip_list.Clear();

                    // Inform?
                    Console.ForegroundColor = ConsoleColor.DarkCyan;
                    Console.WriteLine(FilterMain.FILTER + "IP limit resetted.");
                    Console.ResetColor();
                }

                if (cmd == "/nospam")
                {
                    // Inform?
                    Console.ForegroundColor = ConsoleColor.DarkCyan;
                    Console.WriteLine(FilterMain.FILTER + "I have now stopped the spam about updates, oqe?");
                    Console.ResetColor();

                    // Ignore the spam message.
                    FilterMain.Ignore = true;
                }

                // Send notice
                if (cmd.Contains("/notice"))
                {
                    // Check if there is a message
                    if (cmd.Contains(" "))
                    {
                        // Get the message
                        string message = cmd.Substring(cmd.IndexOf(' ') + 1);

                        // Agent 1
                        AgentContext.SendNoticeOnline(message);

                        // If agent 2?
                        if ((!FilterMain.AGENT_IP2.Equals("0")) && (FilterMain.AGENT_IP2 != String.Empty))
                        {
                            AgentContext2.SendNoticeOnline(message);
                        }

                        // Inform?
                        Console.ForegroundColor = ConsoleColor.DarkCyan;
                        Console.WriteLine(FilterMain.FILTER + "Notice sent to all online players!");
                        Console.ResetColor();

                    }
                }

                // Disconnect user
                if (cmd.Contains("/disconnect"))
                {
                    // Check if contains space
                    if (cmd.Contains(" "))
                    {
                        string username = cmd.Substring(cmd.IndexOf(' ') + 1);

                        // Agent 1
                        AgentContext.DisconnectUser(username);

                        // Agent 2
                        if ((!FilterMain.AGENT_IP2.Equals("0")) && (FilterMain.AGENT_IP2 != String.Empty))
                        {
                            AgentContext2.DisconnectUser(username);
                        }

                        // Inform
                        Console.ForegroundColor = ConsoleColor.DarkCyan;
                        Console.WriteLine(FilterMain.FILTER + "User has been disconnected from the server!");
                        Console.ResetColor();
                    }
                }

                // Ban user
                if (cmd.Contains("/ban"))
                {
                    // Check if contains space
                    if (cmd.Contains(" "))
                    {
                        string username = cmd.Substring(cmd.IndexOf(' ') + 1);

                        // Agent 1
                        AgentContext.BanUser(username);

                        // Agent 2
                        if ((!FilterMain.AGENT_IP2.Equals("0")) && (FilterMain.AGENT_IP2 != String.Empty))
                        {
                            AgentContext2.BanUser(username);
                        }

                        // Inform
                        Console.ForegroundColor = ConsoleColor.DarkCyan;
                        Console.WriteLine(FilterMain.FILTER + "User has been banned from the server!");
                        Console.ResetColor();
                    }
                }

                // Debug
                if (cmd.Contains("/whos"))
                {
                    AgentContext.WhosOnline();
                }

                // Load opcodes
                if (cmd.Contains("/reopcodes"))
                {
                    // Load opcodes
                    Program.LoadOpcodes();

                    // Inform
                    Console.ForegroundColor = ConsoleColor.DarkCyan;
                    Console.WriteLine(FilterMain.FILTER + "Opcodes was reloaded!");
                    Console.ResetColor();
                }
                Thread.Sleep(1);
            }
        }
        #endregion

        #region LoadRegions
        public static void LoadRegions()
        {
            // Remove trade regions
            FilterMain.tradeRegions.Clear();

            // Remove fortress regions
            FilterMain.fortressRegions.Clear();

            // Remove fortress teleports
            FilterMain.fortressTele.Clear();

            // Remove jobcave teleports
            FilterMain.Jobcavetele.Clear();

            // Check if the file exist
            if (!File.Exists("regions/trade.txt"))
            {
                // Inform user
                Console.ForegroundColor = ConsoleColor.DarkYellow;
                Console.WriteLine(FilterMain.FILTER + "regions/trade.txt doesn't exist!");
                Console.ResetColor();
            }
            else
            {
                foreach (string line in File.ReadAllLines("regions/trade.txt"))
                {
                    if (line != String.Empty && line != null)
                    {
                        // Add region
                        FilterMain.tradeRegions.Add(short.Parse(line));
                    }
                }

                // Inform?
                Console.ForegroundColor = ConsoleColor.DarkCyan;
                Console.WriteLine(FilterMain.FILTER + "regions/trade.txt was loaded!");
                Console.ResetColor();
            }

            // Check if the file exist
            if (!File.Exists("regions/fortress.txt"))
            {
                // Inform user
                Console.ForegroundColor = ConsoleColor.DarkYellow;
                Console.WriteLine(FilterMain.FILTER + "regions/fortress.txt doesn't exist!");
                Console.ResetColor();
            }
            else
            {
                foreach (string line in File.ReadAllLines("regions/fortress.txt"))
                {
                    if (line != String.Empty && line != null)
                    {
                        // Add region
                        FilterMain.fortressRegions.Add(short.Parse(line));
                    }
                }

                // Inform?
                Console.ForegroundColor = ConsoleColor.DarkCyan;
                Console.WriteLine(FilterMain.FILTER + "regions/fortress.txt was loaded!");
                Console.ResetColor();
            }

            // Check if the file exist
            if (!File.Exists("regions/fortress_teleports.txt"))
            {
                // Inform user
                Console.ForegroundColor = ConsoleColor.DarkYellow;
                Console.WriteLine(FilterMain.FILTER + "regions/fortress_teleports.txt doesn't exist!");
                Console.ResetColor();
            }
            else
            {
                foreach (string line in File.ReadAllLines("regions/fortress_teleports.txt"))
                {
                    if (line != String.Empty && line != null)
                    {
                        // Add region
                        FilterMain.fortressTele.Add(uint.Parse(line));
                    }
                }

                // Inform?
                Console.ForegroundColor = ConsoleColor.DarkCyan;
                Console.WriteLine(FilterMain.FILTER + "regions/fortress_teleports.txt was loaded!");
                Console.ResetColor();
            }
            // Check if the file exist
            if (!File.Exists("regions/jobcave_teleports.txt"))
            {
                // Inform user
                Console.ForegroundColor = ConsoleColor.DarkYellow;
                Console.WriteLine(FilterMain.FILTER + "regions/jobcave_teleports.txt doesn't exist!");
                Console.ResetColor();
            }
            else
            {
                foreach (string line in File.ReadAllLines("regions/jobcave_teleports.txt"))
                {
                    if (line != String.Empty && line != null)
                    {
                        // Add region
                        FilterMain.Jobcavetele.Add(uint.Parse(line));
                    }
                }

                // Inform?
                Console.ForegroundColor = ConsoleColor.DarkCyan;
                Console.WriteLine(FilterMain.FILTER + "regions/jobcave_teleports.txt was loaded!");
                Console.ResetColor();
            }
        }
        #endregion

        #region BlockedSkills
        public static void BlockedSkills()
        {
            // Delete old inputs
            FilterMain.FortressSkills.Clear();
            FilterMain.BlockedSkills.Clear();

            #region Fortress skills
            // Check if file exist
            if (!File.Exists("skills/FortressSkills.txt"))
            {
                // Inform user
                Console.ForegroundColor = ConsoleColor.DarkYellow;
                Console.WriteLine(FilterMain.FILTER + "skills/FortressSkills.txt doesn't exist!");
                Console.ResetColor();
            }
            else
            {
                foreach (string line in File.ReadAllLines("skills/FortressSkills.txt"))
                {
                    if (line != String.Empty && line != null)
                    {
                        // Add blocked skill
                        FilterMain.FortressSkills.Add(Convert.ToInt32(line));
                    }
                }

                // Inform?
                Console.ForegroundColor = ConsoleColor.DarkCyan;
                Console.WriteLine(FilterMain.FILTER + "skills/FortressSkills.txt was loaded!");
                Console.ResetColor();
            }
            #endregion

            #region Blocked skills
            // Check if file exist
            if (!File.Exists("skills/BlockedSkills.txt"))
            {
                // Inform user
                Console.ForegroundColor = ConsoleColor.DarkYellow;
                Console.WriteLine(FilterMain.FILTER + "skills/BlockedSkills.txt doesn't exist!");
                Console.ResetColor();
            }
            else
            {
                foreach (string line in File.ReadAllLines("skills/BlockedSkills.txt"))
                {
                    if (line != String.Empty && line != null)
                    {
                        // Add blocked skill
                        FilterMain.BlockedSkills.Add(Convert.ToInt32(line));
                    }
                }

                // Inform?
                Console.ForegroundColor = ConsoleColor.DarkCyan;
                Console.WriteLine(FilterMain.FILTER + "skills/BlockedSkills.txt was loaded!");
                Console.ResetColor();
            }
            #endregion
        }
        #endregion

        #region Insultwords
        // Insult words
        public static void Insult()
        {
            // Remove all current blocked shits
            FilterMain.badwords.Clear();

            // Check if the file exist
            if (!File.Exists("config/badwords.txt"))
            {
                // Inform user
                Console.ForegroundColor = ConsoleColor.DarkYellow;
                Console.WriteLine(FilterMain.FILTER + "config/badwords.txt doesn't exist!");
                Console.ResetColor();
            }
            else
            {
                foreach (string line in File.ReadAllLines("config/badwords.txt"))
                {
                    if (line != String.Empty && line != null)
                    {
                        // Add region
                        FilterMain.badwords.Add(line.ToLower());
                    }
                }

                // Inform?
                Console.ForegroundColor = ConsoleColor.DarkCyan;
                Console.WriteLine(FilterMain.FILTER + "config/badwords.txt was loaded!");
                Console.ResetColor();
            }
        }
        #endregion

        #region Reloadbans
        // Reload bans
        public static void Reloadbans()
        {
            // Remove all bans
            FilterMain.ban_list.Clear();

            // Inform
            Console.ForegroundColor = ConsoleColor.DarkCyan;
            Console.WriteLine(FilterMain.FILTER + "Blacklist loaded!");
            Console.ResetColor();

            // New shit
            if (!File.Exists("config/blacklist.txt"))
            {
                // Create file
                var creation = File.Create("config/blacklist.txt");
                creation.Close();

                // Inform user
                Console.ForegroundColor = ConsoleColor.DarkYellow;
                Console.WriteLine(FilterMain.FILTER + "missing file blacklist.txt was created");
                Console.ResetColor();
            }
            else
            {
                foreach (string line in File.ReadAllLines("config/blacklist.txt"))
                {
                    if (line != String.Empty && line != null)
                    {
                        if (!FilterMain.ban_list.Contains(line))
                        {
                            // Add ban
                            FilterMain.ban_list.Add(line);

                            // Inform?
                            Console.ForegroundColor = ConsoleColor.DarkCyan;
                            Console.WriteLine(FilterMain.FILTER + "New blacklist added " + line);
                            Console.ResetColor();
                        }
                        else
                        {
                            // Inform?
                            Console.ForegroundColor = ConsoleColor.DarkCyan;
                            Console.WriteLine(FilterMain.FILTER + "Dublicate row detected on : " + line);
                            Console.ResetColor();
                        }
                    }
                }
            }
        }
        #endregion

        #region Anti-SQL inject
        // Anti SQL Inject
        public static string Plis(string Value)
        {
            // Replace shit
            Value = Value.Replace("'", string.Empty);
            Value = Value.Replace("\"", string.Empty);
            Value = Value.Replace(" ", string.Empty);

            // Return clean shits \\
            return Value;
        }
        #endregion

        #region Shard checker
        public static void ShardCheck(object e)
        {
            try
            {
                // Shard reader
                if(FilterMain.ENABLED && (FilterMain.DB) && (sqlCon.con.State == ConnectionState.Open))
                {
                    // OPEN
                    FilterMain.shard_players = sqlCon.ReadInt("SELECT TOP(1) nUserCount FROM [" + FilterMain.ACC_DB + "].[dbo].[_ShardCurrentuser] order by nID desc");
                }
            }
            catch { }
        }
        #endregion

        #region NoticeSender
        public static void NoticeSend(object e)
        {
            try
            {
                // Notice shits
                if (FilterMain.DB)
                {
                    // Read the notice message
                    SqlDataReader reader = sqlCon.Return("[" + FilterMain.SUP_DB + "].[dbo].[_GetNotice]");
                    reader.Read();

                    // Register message
                    string message = reader.GetString(0);

                    // Check message length
                    if (message.Length > 0)
                    {
                        // Send to first agent
                        AgentContext.SendNoticeOnline(message);

                        // Send to second agent
                        if ((!FilterMain.AGENT_IP2.Equals("0")) && (FilterMain.AGENT_IP2 != String.Empty))
                        {
                            AgentContext2.SendNoticeOnline(message);
                        }
                    }

                    // Close reader
                    reader.Close();
                }
            }
            catch { }
        }
        #endregion

        #region SendNoticeToPlayer
        public static void SendToPlayer(object e)
        {
            try
            {
                // Notice shits
                if (FilterMain.DB)
                {
                    // Read the notice message
                    SqlDataReader reader = sqlCon.Return("[" + FilterMain.SUP_DB + "].[dbo].[_GetPlayerNotice]");
                    reader.Read();

                    // Register message
                    string message = reader.GetString(0);

                    // Check message length
                    if (message.Length > 0)
                    {
                        string[] notices = message.Split('|');

                        // Send to first agent
                        AgentContext.NoticeToPlayer(notices[0], notices[1]);

                        // Send to second agent
                        if ((!FilterMain.AGENT_IP2.Equals("0")) && (FilterMain.AGENT_IP2 != String.Empty))
                        {
                            //AgentContext2.NoticeToPlayer(message, )
                        }
                    }

                    // Close reader
                    reader.Close();
                }
            }
            catch { }
        }
        #endregion

        #region Check memory leaking
        public static void FixMemoryLeak(object e)
        {
            try
            {
                if ((FilterMain.DB) && (sqlCon.con.State == ConnectionState.Open))
                {
                    // Inform?
                    Console.ForegroundColor = ConsoleColor.DarkCyan;
                    Console.WriteLine("[TIMER] Attempting to fix SQL memory leak.");
                    Console.ResetColor();

                    // Reset connections
                    sqlCon.con.Close();

                    // Inform?
                    Console.ForegroundColor = ConsoleColor.DarkCyan;
                    Console.WriteLine("[TIMER] SQL memory leak is now patched.");
                    Console.ResetColor();
                }
            }
            catch { }
        }
        #endregion

        #region Clear event chars
        public static void EventChars(object e)
        {
            try
            {
                // Register timer.
                if (!FilterMain.timer)
                {
                    FilterMain.timer = true;
                }
                else
                {
                    // Remove event chars
                    FilterMain.event_chars.Clear();

                    // Disable CTF timer
                    if (FilterMain.CTF_TIMER != null)
                    {
                        FilterMain.CTF_TIMER.Dispose();
                        FilterMain.CTF_TIMER = null;
                        FilterMain.ctf_called = false;
                    }

                    // Disable ARENA timer
                    if (FilterMain.ARENA_TIMER != null)
                    {
                        FilterMain.ARENA_TIMER.Dispose();
                        FilterMain.ARENA_TIMER = null;
                        FilterMain.arena_called = false;
                    }

                    // Register timer.
                    FilterMain.timer = false;
                }
            }
            catch {
                Console.WriteLine("Error with timers, lel top snek");
            }
        }
        #endregion

        #region Read OPCODES
        public static void LoadOpcodes()
        {
            try
            {
                // Check if exists.
                if (File.Exists("config/legit.txt"))
                {
                    foreach (string line in File.ReadAllLines("config/legit.txt"))
                    {
                        // Read opcode
                        string[] split = line.Split(',');
                        ushort opcode = Convert.ToUInt16(split[0], 16);
                        if (!FilterMain.Opcodes.ContainsKey(opcode))
                        {
                            FilterMain.Opcodes.Add(opcode, split[1]);
                        }
                    }
                }
                else
                {
                    Console.WriteLine("Error loading legit opcodes, you should close program!");
                }

                // Check if badopcodes exists.
                if (File.Exists("config/exploit.txt"))
                {
                    foreach (string line in File.ReadAllLines("config/exploit.txt"))
                    {
                        // Read opcode
                        string[] split = line.Split(',');
                        ushort opcode = Convert.ToUInt16(split[0], 16);
                        if (!FilterMain.BAD_Opcodes.ContainsKey(opcode))
                        {
                            FilterMain.BAD_Opcodes.Add(opcode, split[1]);
                        }
                    }
                }
                else
                {
                    Console.WriteLine("Error loading exploit opcodes, you should close program!");
                }
            }
            catch
            {
                Console.WriteLine("Error loading opcodes, you should close program.");
            }
        }
        #endregion

        #region Main shits
        static void Main(string[] args)
        {
            // FILTER NAME
            Console.Title = FilterMain.FILTER + "- BY Goofie (" + FilterMain.UPDATED + ")";

            // LOAD OPCODES
            Program.LoadOpcodes();

            // Load settings
            Config.LoadEverything(false);
            Config.LoadEverything(true);

            // Load bans
            Program.Reloadbans();

            // Load regions
            Program.LoadRegions();

            // Load skills
            Program.BlockedSkills();

            // Load badwords
            Program.Insult();

            // GATEWAY
            AsyncServer GatewayServer = new AsyncServer();
            GatewayServer.Start(FilterMain.GATEWAY_IP, FilterMain.FAKE_GATEWAY, AsyncServer.E_ServerType.GatewayServer);

            // Agent
            AsyncServer AgentServer = new AsyncServer();
            AgentServer.Start(FilterMain.AGENT_IP, FilterMain.FAKE_AGENT, AsyncServer.E_ServerType.AgentServer);

            // Small checker, sql fak.
            if (FilterMain.DB)
            {
                try
                {
                    if (sqlCon.con.State == ConnectionState.Closed)
                    {
                        // Refresh/open.
                        sqlCon.con.Open();
                    }
                }
                catch (Exception ex)
                {
                    // Exception handler
                    if (FilterMain.EXCEPTION_LOG)
                    {
                        Console.WriteLine(ex.ToString());
                    }
                }
            }

            // Pioneer special
            if (FilterMain.PIONEER)
            {
                // Agent 2
                if ((!FilterMain.AGENT_IP2.Equals("0")) && (FilterMain.AGENT_IP2 != String.Empty))
                {
                    AsyncServer AgentServer2 = new AsyncServer();
                    AgentServer2.Start(FilterMain.AGENT_IP2, FilterMain.FAKE_AGENT2, AsyncServer.E_ServerType.AgentServer2);
                }
            }
            else
            {
                // Agent 2
                if ((!FilterMain.AGENT_IP2.Equals("0")) && (FilterMain.AGENT_IP2 != String.Empty))
                {
                    AsyncServer AgentServer2 = new AsyncServer();
                    AgentServer2.Start(FilterMain.AGENT_IP, FilterMain.FAKE_AGENT2, AsyncServer.E_ServerType.AgentServer2);
                }
            }

            // START NEW THREAD
            new Thread(ConsolePoolThread).Start();

            // Reset log_players stuff
            if((FilterMain.DB) && (FilterMain.log_players) && (sqlCon.con.State == ConnectionState.Open))
            {
                sqlCon.exec("UPDATE ["+ FilterMain.SUP_DB + "].[dbo].[_Players] SET [cur_status] = 0 WHERE [cur_status] = 1");
            }

            // Start timers
            if((FilterMain.DB) && (sqlCon.con.State == ConnectionState.Open))
            {
                // Timer(s)
                FilterMain.t5 = new Timer(new TimerCallback(Program.ShardCheck), null, 0, 30000);
                FilterMain.t6 = new Timer(new TimerCallback(Program.NoticeSend), null, 0, 2500);
                FilterMain.ToPlayer = new Timer(new TimerCallback(Program.SendToPlayer), null, 0, 3000);
                FilterMain.MemoryLeak = new Timer(new TimerCallback(Program.FixMemoryLeak), null, 0, 3600000);
            }
        }
        #endregion
    }
}