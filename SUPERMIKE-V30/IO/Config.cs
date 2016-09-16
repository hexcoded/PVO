using System;
using System.IO;

namespace ExploitFilter
{
    class Config
    {
        public static iniFile cfg = new iniFile("config/settings.ini");
        public static void LoadEverything(bool derp)
        {
            try {
                // Config missing, rip.
                if (!File.Exists("config/settings.ini"))
                {
                    Console.Clear();
                }

                if (derp)
                {
                    // Fancy message
                    Console.ForegroundColor = ConsoleColor.DarkYellow;
                    Console.WriteLine("------------------------------ SUPERMIKE coded by Goofie. ------------------------------");
                    Console.ResetColor();

                    // Version
                    Console.ForegroundColor = ConsoleColor.Gray;
                    Console.WriteLine("Version: " + FilterMain.UPDATED + ", Remember to use the latest .exe as frequently as possible!");
                    Console.WriteLine("");
                    Console.WriteLine("");


                    // Special thanks
                    Console.WriteLine("Special thanks to: Elitepvpers.com for supporting SUPERMIKE!");
                    Console.WriteLine("Don't forget to visit Team24-7.net for DDoS protection and much more, highly recommended for your private server!");
                    Console.WriteLine("");
                    Console.WriteLine("");

                    // Information
                    Console.WriteLine("Thanks for using SUPERMIKE. If you have any questions do not hesitate to contact me at skype (missing.input).");
                    Console.ResetColor();


                    Console.ForegroundColor = ConsoleColor.DarkYellow;
                    Console.WriteLine("----------------------------------------------------------------------------------------");
                    Console.ResetColor();
                }
                else
                {

                    // Read DB names
                    FilterMain.SUP_DB = cfg.IniReadValue("GENERAL", "SUP_DB");
                    FilterMain.ACC_DB = cfg.IniReadValue("GENERAL", "ACC_DB");
                    FilterMain.SHA_DB = cfg.IniReadValue("GENERAL", "SHA_DB");
                    FilterMain.LOG_DB = cfg.IniReadValue("GENERAL", "LOG_DB");

                    // Read SQL
                    FilterMain.DB = bool.Parse(cfg.IniReadValue("GENERAL", "ENABLE"));

                    // READ LOGGING
                    FilterMain.ERROR_LOG = cfg.IniReadValue("GENERAL", "LOGGING").ToLower();

                    // Exception stuff
                    FilterMain.EXCEPTION_LOG = bool.Parse(cfg.IniReadValue("GENERAL", "EXCEPTION"));

                    // Load shits
                    string STRUSERID = cfg.IniReadValue("GENERAL", "USER_ID");
                    string[] user_ids = STRUSERID.Split(',');

                    FilterMain.USER_ID.Clear();

                    foreach (string id in user_ids)
                    {
                        if (id != null && id != string.Empty)
                        {
                            FilterMain.USER_ID.Add(id);
                        }
                    }

                    // Files stuff 
                    FilterMain.FILES = cfg.IniReadValue("GENERAL", "FILES").ToLower();

                    // -------------------------------------------------------------------------------------- \\

                    // PIONEER
                    FilterMain.REMOTE_GATEWAY = cfg.IniReadValue("PIONEER", "REMOTE_GATEWAY");
                    FilterMain.REMOTE_AGENT = cfg.IniReadValue("PIONEER", "REMOTE_AGENT");
                    FilterMain.REMOTE_AGENT_2 = cfg.IniReadValue("PIONEER", "REMOTE_AGENT2");
                    FilterMain.GATEWAY_LISTEN_PORT = int.Parse(cfg.IniReadValue("PIONEER", "REMOTE_GATEWAY_PORT"));
                    FilterMain.AGENT_LISTEN_PORT = int.Parse(cfg.IniReadValue("PIONEER", "REMOTE_AGENT_PORT"));
                    FilterMain.AGENT2_LISTEN_PORT = int.Parse(cfg.IniReadValue("PIONEER", "REMOTE_AGENT2_PORT"));
                    FilterMain.PIONEER = bool.Parse(cfg.IniReadValue("PIONEER", "ENABLE"));

                    // GATEWAY
                    FilterMain.GATEWAY_IP = cfg.IniReadValue("GATEWAY", "LISTEN_IP");
                    FilterMain.FAKE_GATEWAY = int.Parse(cfg.IniReadValue("GATEWAY", "LISTEN_PORT"));
                    FilterMain.Captcha_Remove = bool.Parse(cfg.IniReadValue("GATEWAY", "DISABLE_CAPTCHA"));
                    FilterMain.Captcha_Char = cfg.IniReadValue("GATEWAY", "CAPTCHA_CHAR");
                    FilterMain.block_status = bool.Parse(cfg.IniReadValue("GATEWAY", "BLOCK_STATUS"));

                    // -------------------------------------------------------------------------------------- \\

                    // AGENT
                    FilterMain.AGENT_IP = cfg.IniReadValue("AGENT", "LISTEN_IP");
                    FilterMain.FAKE_AGENT = int.Parse(cfg.IniReadValue("AGENT", "LISTEN_PORT"));

                    if (cfg.IniReadValue("AGENT", "SPOOF_IP").Equals("0"))
                    {
                        FilterMain.PROXY = FilterMain.AGENT_IP;
                    }
                    else
                    {
                        FilterMain.PROXY = cfg.IniReadValue("AGENT", "SPOOF_IP");
                    }
                    // -------------------------------------------------------------------------------------- \\

                    // AGENT2
                    if (cfg.IniReadValue("AGENT2", "LISTEN_IP") != string.Empty)
                    {
                        // Agent2 IP
                        FilterMain.AGENT_IP2 = cfg.IniReadValue("AGENT2", "LISTEN_IP");
                        FilterMain.FAKE_AGENT2 = int.Parse(cfg.IniReadValue("AGENT2", "LISTEN_PORT"));
                    }
                    // -------------------------------------------------------------------------------------- \\

                    // GMSTUFF
                    string ACCOUNTS = cfg.IniReadValue("GMSTUFF", "ACCOUNTS");
                    string[] gm_names = ACCOUNTS.Split(',');

                    // IPS
                    string IPS = cfg.IniReadValue("GMSTUFF", "ACCOUNT_IP");
                    string[] gm_ips = IPS.Split(',');

                    // Remove all shitnzels
                    FilterMain.GM_ACCOUNT.Clear();
                    FilterMain.PRIV_IP.Clear();

                    // Add new usernames
                    foreach (string gm_userid in gm_names)
                    {
                        if (gm_userid != null && gm_userid != string.Empty)
                        {
                            // Add all usernames
                            FilterMain.GM_ACCOUNT.Add(gm_userid.ToLower());
                        }
                    }

                    // Add new ips
                    foreach (string priv_ip in gm_ips)
                    {
                        if (priv_ip != null && priv_ip != string.Empty)
                        {
                            // Add all ips
                            FilterMain.PRIV_IP.Add(priv_ip);
                        }
                    }

                    // Read shits
                    FilterMain.GM_LOGIN = bool.Parse(cfg.IniReadValue("GMSTUFF", "BLOCK_LOGIN"));
                    FilterMain.START_VISIBLE = bool.Parse(cfg.IniReadValue("GMSTUFF", "START_VISIBLE"));

                    // -------------------------------------------------------------------------------------- \\
                    // CTF LEVEL
                    FilterMain.CTF_level = int.Parse(cfg.IniReadValue("LEVELS", "CTF_REGISTER_LEVEL"));

                    // ARENA LEVEL
                    FilterMain.ARENA_level = int.Parse(cfg.IniReadValue("LEVELS", "ARENA_REGISTER_LEVEL"));

                    // GLOBAL LEVEL
                    FilterMain.global_level = int.Parse(cfg.IniReadValue("LEVELS", "GLOBAL_LEVEL"));

                    // STALL LEVEL
                    FilterMain.stall_level = int.Parse(cfg.IniReadValue("LEVELS", "STALL_LEVEL"));

                    // JOB REVERSE
                    FilterMain.job_reverse = bool.Parse(cfg.IniReadValue("ADVANCED", "BLOCK_JOB_REVERSE_SCROLL"));

                    // JOB RES SCROLL
                    FilterMain.job_res = bool.Parse(cfg.IniReadValue("ADVANCED", "BLOCK_JOB_RES_SCROLL"));

                    // JOB EXCHANGE
                    FilterMain.job_exchange = bool.Parse(cfg.IniReadValue("ADVANCED", "BLOCK_JOB_EXCHANGE"));

                    // PLUS LIMIT
                    FilterMain.plus_limit = int.Parse(cfg.IniReadValue("ADVANCED", "PLUS_LIMIT"));

                    // LOG PLAYERS
                    FilterMain.log_players = bool.Parse(cfg.IniReadValue("ADVANCED", "LOG_PLAYERS"));
                    // -------------------------------------------------------------------------------------- \\

                    // Silk enabled?
                    FilterMain.SilkEnable = bool.Parse(cfg.IniReadValue("SilkSystem", "SilkSystem"));

                    // Required level to get silk
                    FilterMain.SilkLevel = int.Parse(cfg.IniReadValue("SilkSystem", "SilkLevel"));

                    // Amount
                    FilterMain.SilkAmount = int.Parse(cfg.IniReadValue("SilkSystem", "SilkAmount"));

                    // Delay between each given amount
                    FilterMain.SilkDelay = int.Parse(cfg.IniReadValue("SilkSystem", "SilkDelay")) * 60000;

                    // SilkType
                    FilterMain.SilkType = cfg.IniReadValue("SilkSystem", "SilkType").ToLower();

                    // SilkRewardAfk
                    FilterMain.SilkRewardAfk = bool.Parse(cfg.IniReadValue("SilkSystem", "SilkRewardAfk"));

                    // Notice
                    FilterMain.SilkNotice = bool.Parse(cfg.IniReadValue("SilkSystem", "SilkNotice"));
                    // -------------------------------------------------------------------------------------- \\

                    // Exchange delay
                    FilterMain.Exchangedelay = int.Parse(cfg.IniReadValue("DELAYS", "EXCHANGE"));

                    // Stall delay
                    FilterMain.Stalldelay = int.Parse(cfg.IniReadValue("DELAYS", "STALL"));

                    // Global delay
                    FilterMain.Globaldelay = int.Parse(cfg.IniReadValue("DELAYS", "GLOBAL"));

                    // Reverse delay
                    FilterMain.Reversedelay = int.Parse(cfg.IniReadValue("DELAYS", "REVERSE"));

                    // Restart delay
                    FilterMain.Restartdelay = int.Parse(cfg.IniReadValue("DELAYS", "RESTART"));

                    // Exit delay
                    FilterMain.Logoutdelay = int.Parse(cfg.IniReadValue("DELAYS", "EXIT"));

                    // ZERK delay
                    FilterMain.zerkdelay = int.Parse(cfg.IniReadValue("DELAYS", "ZERK"));
                    // -------------------------------------------------------------------------------------- \\

                    // LIMITS
                    FilterMain.IPLIMIT = int.Parse(cfg.IniReadValue("LIMITS", "IP_LIMIT"));

                    // HWID LIMIT
                    FilterMain.PCLIMIT = int.Parse(cfg.IniReadValue("LIMITS", "PC_LIMIT"));

                    // LIMITS
                    string s = cfg.IniReadValue("LIMITS", "BYPASS");
                    string[] usernames = s.Split(',');

                    // Remove all username
                    FilterMain.LIMIT_BYPASS.Clear();

                    // Add new usernames
                    foreach (string user_id in usernames)
                    {
                        if (user_id != null && user_id != string.Empty)
                        {
                            // Add all usernames
                            FilterMain.LIMIT_BYPASS.Add(user_id.ToLower());
                        }
                    }

                    // CAFE LIMIT
                    FilterMain.CAFELIMIT = int.Parse(cfg.IniReadValue("LIMITS", "CAFE_LIMIT"));

                    // CAFES
                    string ss = cfg.IniReadValue("LIMITS", "CAFE_IP");
                    string[] ips = ss.Split(',');

                    // Remove all ips
                    FilterMain.cafe_list.Clear();

                    // Add all IPs
                    foreach (string ipss in ips)
                    {
                        FilterMain.cafe_list.Add(ipss);
                    }

                    // GLOBAL IP LIMIT
                    FilterMain.IPGLOBAL = int.Parse(cfg.IniReadValue("LIMITS", "IP_GLOBAL"));
                    // -------------------------------------------------------------------------------------- \\

                    // GATEWAY BYTES
                    FilterMain.dMaxBytesPerSec_Gateway = int.Parse(cfg.IniReadValue("PROTECTION", "G_BYTES"));

                    // GATEWAY PACKET/SEC
                    FilterMain.GATEWAY_COUNT = int.Parse(cfg.IniReadValue("PROTECTION", "G_PACKET"));

                    // AGENT BYTES
                    FilterMain.dMaxBytesPerSec_Agent = int.Parse(cfg.IniReadValue("PROTECTION", "A_BYTES"));

                    // AGENT PACKET/SEC
                    FilterMain.AGENT_COUNT = int.Parse(cfg.IniReadValue("PROTECTION", "A_PACKET"));

                    // FLOOD PROTECTION
                    FilterMain.flood_fix = bool.Parse(cfg.IniReadValue("PROTECTION", "FLOOD"));

                    // FLOOD LIMIT
                    FilterMain.FLOOD_LIMIT = int.Parse(cfg.IniReadValue("PROTECTION", "FLOOD_LIMIT"));

                    // FLOOD HANDLE
                    FilterMain.FLOOD_METHOD = cfg.IniReadValue("PROTECTION", "FLOOD_ACTION").ToLower();

                    // EXPLOIT HANDLE
                    FilterMain.EXPLOIT_METHOD = cfg.IniReadValue("PROTECTION", "EXPLOIT_ACTION").ToLower();

                    // PACKET HANDLE
                    FilterMain.PACKET_METHOD = cfg.IniReadValue("PROTECTION", "PACKET_ACTION").ToLower();

                    // UNKNOWN HANDLE
                    FilterMain.UNKNOWN_METHOD = cfg.IniReadValue("PROTECTION", "UNKNOWN_ACTION").ToLower();
                    // -------------------------------------------------------------------------------------- \\

                    // INFO
                    FilterMain.ENABLED = bool.Parse(cfg.IniReadValue("INFO", "ENABLE"));

                    // Server name
                    FilterMain.ServerName = Program.Plis(cfg.IniReadValue("INFO", "SERVER_NAME"));
                    if (FilterMain.ServerName.ToLower().Contains("goofie"))
                    {
                        FilterMain.ServerName = Environment.UserName;
                    }

                    // Fake players
                    FilterMain.FAKE_PLAYERS = int.Parse(cfg.IniReadValue("INFO", "PLAYERS"));

                    // Max online
                    FilterMain.MAX_PLAYERS = int.Parse(cfg.IniReadValue("INFO", "MAX_PLAYERS"));

                    // ShardID
                    FilterMain.ShardID = int.Parse(cfg.IniReadValue("INFO", "SHARD"));
                    // -------------------------------------------------------------------------------------- \\

                    // NOTICE
                    FilterMain.PlusNotice = bool.Parse(cfg.IniReadValue("NOTICE", "PLUSNOTICE"));

                    // Required Plus
                    FilterMain.RequiredPlus = int.Parse(cfg.IniReadValue("NOTICE", "START_PLUS"));
                    // -------------------------------------------------------------------------------------- \\

                    // MISC
                    FilterMain.WelcomeMsg = bool.Parse(cfg.IniReadValue("MISC", "WELCOME_MSG"));

                    // WELCOME TEXT
                    FilterMain.WelcomeText = cfg.IniReadValue("MISC", "WELCOME_TEXT");

                    // WELCOME TEXT ENABLE
                    if (!FilterMain.WelcomeMsg)
                    {
                        FilterMain.WelcomeText = string.Empty;
                    }

                    /// MISC HANDLER IDK WHAT THIS IS
                    FilterMain.MESSAGE_HANDLE = cfg.IniReadValue("MISC", "MESSAGE_HANDLER").ToLower();

                    // DISABLE RESTART BUTTON
                    FilterMain.restart_button = bool.Parse(cfg.IniReadValue("MISC", "DISABLE_RESTART_BUTTON"));

                    // If block status tool, block restart button
                    if (FilterMain.block_status)
                    {
                        FilterMain.restart_button = true;
                    }

                    // AVATAR BLUES
                    FilterMain.avatar_blues = bool.Parse(cfg.IniReadValue("MISC", "DISABLE_AVATAR_BLUES"));
                    // -------------------------------------------------------------------------------------- \\

                    // Translation
                    // ACADEMY NOTICES
                    FilterMain.ACADEMY_CREATION = cfg.IniReadValue("MESSAGES", "ACADEMY_CREATION");
                    FilterMain.ACADEMY_INVITE = cfg.IniReadValue("MESSAGES", "ACADEMY_INVITE");

                    // ANTI-BOT NOTICES
                    FilterMain.BOT_NOTICE_FORTRESS = cfg.IniReadValue("MESSAGES", "BOT_NOTICE_FORTRESS");
                    FilterMain.BOT_NOTICE_CTF = cfg.IniReadValue("MESSAGES", "BOT_NOTICE_CTF");
                    FilterMain.BOT_NOTICE_ARENA = cfg.IniReadValue("MESSAGES", "BOT_NOTICE_ARENA");
                    FilterMain.BOT_NOTICE_FUSE = cfg.IniReadValue("MESSAGES", "BOT_NOTICE_FUSE");
                    FilterMain.BOT_NOTICE_ALCHEMY = cfg.IniReadValue("MESSAGES", "BOT_NOTICE_ALCHEMY");
                    FilterMain.BOT_NOTICE_DISCONNECT = cfg.IniReadValue("MESSAGES", "BOT_NOTICE_DISCONNECT");
                    FilterMain.BOT_NOTICE_AVATAR_BLUES = cfg.IniReadValue("MESSAGES", "BOT_NOTICE_AVATAR_BLUES");
                    FilterMain.BOT_NOTICE_STALL_CREATE = cfg.IniReadValue("MESSAGES", "BOT_NOTICE_STALL_CREATE");
                    FilterMain.BOT_NOTICE_PARTY_CREATE = cfg.IniReadValue("MESSAGES", "BOT_NOTICE_PARTY_CREATE");
                    FilterMain.BOT_NOTICE_PARTY_INVITE = cfg.IniReadValue("MESSAGES", "BOT_NOTICE_PARTY_INVITE");
                    FilterMain.BOT_NOTICE_EXCHANGE = cfg.IniReadValue("MESSAGES", "BOT_NOTICE_EXCHANGE");
                    FilterMain.BOT_NOTICE_PVP = cfg.IniReadValue("MESSAGES", "BOT_NOTICE_PVP");
                    FilterMain.BOT_NOTICE_TRACE = cfg.IniReadValue("MESSAGES", "BOT_NOTICE_TRACE");

                    // CHEAT NOTICES
                    FilterMain.CHEAT_NOTICE = cfg.IniReadValue("MESSAGES", "CHEAT_NOTICE");
                    FilterMain.CHEAT_NOTICE2 = cfg.IniReadValue("MESSAGES", "CHEAT_NOTICE2");
                    FilterMain.CHEAT_NOTICE3 = cfg.IniReadValue("MESSAGES", "CHEAT_NOTICE3");

                    // SKILL NOTICES
                    FilterMain.SKILL_NOTICE = cfg.IniReadValue("MESSAGES", "SKILL_NOTICE");
                    FilterMain.SKILL_NOTICE2 = cfg.IniReadValue("MESSAGES", "SKILL_NOTICE2");
                    FilterMain.SKILL_NOTICE3 = cfg.IniReadValue("MESSAGES", "SKILL_NOTICE3");

                    // BAD WORD NOTICES
                    FilterMain.LOCAL_CHAT = cfg.IniReadValue("MESSAGES", "LOCAL_CHAT");
                    FilterMain.PRIVATE_CHAT = cfg.IniReadValue("MESSAGES", "PRIVATE_CHAT");
                    FilterMain.PARTY_CHAT = cfg.IniReadValue("MESSAGES", "PARTY_CHAT");
                    FilterMain.PARTY_MATCH = cfg.IniReadValue("MESSAGES", "PARTY_MATCH");
                    FilterMain.PRIVATE_MESSAGE = cfg.IniReadValue("MESSAGES", "PRIVATE_MESSAGE");
                    FilterMain.GLOBAL_MESSAGE = cfg.IniReadValue("MESSAGES", "GLOBAL_MESSAGE");
                    FilterMain.STALL_FILTER = cfg.IniReadValue("MESSAGES", "STALL_FILTER");

                    // LEVEL/LIMIT RESTRICTIONS
                    FilterMain.CTF_LEVEL = cfg.IniReadValue("MESSAGES", "CTF_LEVEL");
                    FilterMain.ARENA_LEVEL = cfg.IniReadValue("MESSAGES", "ARENA_LEVEL");
                    FilterMain.GLOBAL_LEVEL = cfg.IniReadValue("MESSAGES", "GLOBAL_LEVEL");
                    FilterMain.STALL_LEVEL = cfg.IniReadValue("MESSAGES", "STALL_LEVEL");
                    FilterMain.UNION_LIMIT = cfg.IniReadValue("MESSAGES", "UNION_LIMIT");
                    FilterMain.UNION_LIMIT2 = cfg.IniReadValue("MESSAGES", "UNION_LIMIT2");
                    FilterMain.GUILD_LIMIT = cfg.IniReadValue("MESSAGES", "GUILD_LIMIT");
                    FilterMain.GUILD_LIMIT2 = cfg.IniReadValue("MESSAGES", "GUILD_LIMIT2");
                    FilterMain.PLUS_LIMIT = cfg.IniReadValue("MESSAGES", "PLUS_LIMIT");

                    // DELAY/EXPLOIT RESTRICTIONS
                    FilterMain.EXCHANGE_DELAY = cfg.IniReadValue("MESSAGES", "EXCHANGE_DELAY");
                    FilterMain.STALL_DELAY = cfg.IniReadValue("MESSAGES", "STALL_DELAY");
                    FilterMain.GLOBAL_DELAY = cfg.IniReadValue("MESSAGES", "GLOBAL_DELAY");
                    FilterMain.REVERSE_DELAY = cfg.IniReadValue("MESSAGES", "REVERSE_DELAY");
                    FilterMain.LOGOUT_DELAY = cfg.IniReadValue("MESSAGES", "LOGOUT_DELAY");
                    FilterMain.RESTART_DELAY = cfg.IniReadValue("MESSAGES", "RESTART_DELAY");
                    FilterMain.ZERK_DELAY = cfg.IniReadValue("MESSAGES", "ZERK_DELAY");
                    FilterMain.STALL_EXPLOIT = cfg.IniReadValue("MESSAGES", "STALL_EXPLOIT");

                    // JOB RESTRICTIONS
                    FilterMain.JOB_NOTICE_REVERSE = cfg.IniReadValue("MESSAGES", "JOB_NOTICE_REVERSE");
                    FilterMain.JOB_NOTICE_TRACE = cfg.IniReadValue("MESSAGES", "JOB_NOTICE_TRACE");
                    FilterMain.JOB_NOTICE_RESCURRENT = cfg.IniReadValue("MESSAGES", "JOB_NOTICE_RESCURRENT");
                    FilterMain.JOB_NOTICE_EXCHANGE = cfg.IniReadValue("MESSAGES", "JOB_NOTICE_EXCHANGE");

                    // MISC MESSAGES
                    FilterMain.SILK_NOTICE = cfg.IniReadValue("MESSAGES", "SILK_NOTICE");
                    FilterMain.GN_NOTICE = cfg.IniReadValue("MESSAGES", "GN_NOTICE");
                    FilterMain.RESTART_BUTTON = cfg.IniReadValue("MESSAGES", "RESTART_BUTTON");
                    FilterMain.AVATAR_BLUES = cfg.IniReadValue("MESSAGES", "AVATAR_BLUES");
                    FilterMain.AVATAR_EXPLOIT = cfg.IniReadValue("MESSAGES", "AVATAR_EXPLOIT");
                    FilterMain.INVISIBLE_EXPLOIT = cfg.IniReadValue("MESSAGES", "INVISIBLE_EXPLOIT");
                    FilterMain.DISCONNECT_MESSAGE = cfg.IniReadValue("MESSAGES", "DISCONNECT_MESSAGE");
                    FilterMain.BAN_MESSAGE = cfg.IniReadValue("MESSAGES", "BAN_MESSAGE");

                    // FILTER NAME
                    FilterMain.FILTER_NAME = cfg.IniReadValue("ENDGAME", "FILTER_NAME");

                    // CURRENT_REGION
                    FilterMain.CURRENT_REGION = bool.Parse(cfg.IniReadValue("ENDGAME", "CURRENT_REGION"));

                    // ANTI_JOB_CHEAT
                    FilterMain.JOB_CHEAT = bool.Parse(cfg.IniReadValue("ENDGAME", "ANTI_JOB_CHEAT"));

                    // ANTI_JOB_TRACE
                    FilterMain.JOB_TRACE = bool.Parse(cfg.IniReadValue("ENDGAME", "ANTI_JOB_TRACE"));

                    // FORTRESS STUFF
                    FilterMain.FORTRESS = bool.Parse(cfg.IniReadValue("ENDGAME", "FORTRESS"));

                    // FORTRESS ANTI TRACE
                    FilterMain.FORTRESS_ANTI_TRACE = bool.Parse(cfg.IniReadValue("ENDGAME", "ANTI_FORTRESS_TRACE"));

                    // FORTRESS ANTI RESURRECTION SCROLL
                    FilterMain.FORTRESS_ANTI_RES_SCROLL = bool.Parse(cfg.IniReadValue("ENDGAME", "ANTI_FORTRESS_RES_SCROLL"));

                    // ACADEMY DISABLED
                    FilterMain.ACADEMY_DISABLED = bool.Parse(cfg.IniReadValue("ENDGAME", "ACADEMY_DISABLED"));

                    // ACADEMY INVITE DISABLED
                    FilterMain.ACADEMY_INVITE_DISABLED = bool.Parse(cfg.IniReadValue("ENDGAME", "ACADEMY_INVITE_DISABLED"));

                    // Guild limit
                    FilterMain.Guild_limit = int.Parse(cfg.IniReadValue("ENDGAME", "GUILD_LIMIT"));

                    // Union limit
                    FilterMain.Union_limit = int.Parse(cfg.IniReadValue("ENDGAME", "UNION_LIMIT"));

                    // ADVANCED HWID SYSTEM
                    FilterMain.STORE_HWID = bool.Parse(cfg.IniReadValue("ENDGAME", "STORE_HWID"));

                    // EPIC SRO SHIT
                    FilterMain.THIEF_MUST_SELL = int.Parse(cfg.IniReadValue("ENDGAME", "THIEF_MUST_SELL"));
                    FilterMain.EPIC_ITEM = bool.Parse(cfg.IniReadValue("ENDGAME", "RESTRICT_ITEMS"));

                    // BOTS SHIT, ME LAZY
                    FilterMain.BOT_ALLOW = bool.Parse(cfg.IniReadValue("BOTS", "BOT_ALLOW"));
                    FilterMain.BOT_ALCHEMY_ELIXIR = bool.Parse(cfg.IniReadValue("BOTS", "BOT_ALCHEMY_ELIXIR"));
                    FilterMain.BOT_ALCHEMY_STONE = bool.Parse(cfg.IniReadValue("BOTS", "BOT_ALCHEMY_STONE"));
                    FilterMain.BOT_ARENA = bool.Parse(cfg.IniReadValue("BOTS", "BOT_BA_ARENA"));
                    FilterMain.BOT_CTF = bool.Parse(cfg.IniReadValue("BOTS", "BOT_CTF_ARENA"));
                    FilterMain.BOT_FORTRESS = bool.Parse(cfg.IniReadValue("BOTS", "BOT_FORTRESS_ARENA"));
                    FilterMain.BOT_DETECTION = bool.Parse(cfg.IniReadValue("BOTS", "BOT_DETECTION"));
                    FilterMain.BOT_WARNING = bool.Parse(cfg.IniReadValue("BOTS", "BOT_CONSOLE_MESSAGE"));
                    FilterMain.BOT_AVATAR_BLUES = bool.Parse(cfg.IniReadValue("BOTS", "BOT_AVATAR_BLUES"));
                    FilterMain.BOT_CREATE_PARTY = bool.Parse(cfg.IniReadValue("BOTS", "BOT_CREATE_PARTY"));
                    FilterMain.BOT_INVITE_PARTY = bool.Parse(cfg.IniReadValue("BOTS", "BOT_INVITE_PARTY"));
                    FilterMain.BOT_EXCHANGE = bool.Parse(cfg.IniReadValue("BOTS", "BOT_EXCHANGE"));
                    FilterMain.BOT_STALL = bool.Parse(cfg.IniReadValue("BOTS", "BOT_OPEN_STALL"));
                    FilterMain.BOT_PVP = bool.Parse(cfg.IniReadValue("BOTS", "BOT_PVP"));
                    FilterMain.BOT_TRACE = bool.Parse(cfg.IniReadValue("BOTS", "BOT_PVP"));
                }
                // -------------------------------------------------------------------------------------- \\

                if (derp)
                {
                    // Inform?
                    Console.ForegroundColor = ConsoleColor.DarkCyan;
                    Console.WriteLine(FilterMain.FILTER + "config/settings.ini was loaded!");
                    Console.ResetColor();
                }

            }
            
            catch(Exception ex)
            {
                // Inform?
                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.WriteLine(FilterMain.FILTER + ex.ToString());
                Console.ResetColor();
            }
        }
    }
}
