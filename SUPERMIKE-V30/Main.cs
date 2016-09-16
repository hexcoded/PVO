using System;
using System.Collections.Generic;
using System.Threading;

namespace ExploitFilter
{
    sealed class FilterMain
    {
        // MAIN SETTINGS
        // -------------------------------------------------------------------------------------- \\
        // Timer(s)
        public static Timer t1, t4, t5, t6, ToPlayer, MemoryLeak, Event, CTF_TIMER, ARENA_TIMER;
        // Main options no cry
        public static bool Downloaded = false;
        public static bool Ignore = false;

        // Read shard data
        public static int cur_players = 0;
        public static int max_players = 1000;
        public static int status = 0;

        // General data
        public static Int32 gateway, agent1, agent2 = 0;

        // FILTER NAME
        public static string FILTER = "[SUPERMIKE] ";
        public static string DATE = "[" + DateTime.UtcNow + "]";

        // SUPPORTED FILES
        public static string FILES = "vSRO";

        // UPDATED AT
        public static int UPDATED = 30;

        // COUNT EXPLOITS
        public static int exploit_count = 0;

        // FLOOD FIX
        public static bool flood_fix = false;

        // HWID
        public static bool hwid_system = false;

        // ERROR LOGGING
        public static string ERROR_LOG = "off";

        // EXCEPTION LOGGING
        public static bool EXCEPTION_LOG = true;

        // LOG
        public static bool logs = false;

        // LOGGING SYSTEM
        public static bool logging = true;

        // DATABASE SYSTEM
        public static string host = string.Empty;
        public static string user = string.Empty;
        public static string pass = string.Empty;
        public static string shard = string.Empty;
        public static bool DB = false;
        // -------------------------------------------------------------------------------------- \\
        // REAL IP SETTINGS
        // -------------------------------------------------------------------------------------- \\
        public static string GATEWAY_IP = "127.0.0.1";
        public static string AGENT_IP = "127.0.0.1";
        public static string AGENT_IP2 = "127.0.0.1";
        // REAL = PROXY IP OF PROXY(MAINSERVER)
        // -------------------------------------------------------------------------------------- \\

        // PROXY IP SETTINGS
        // -------------------------------------------------------------------------------------- \\
        public static string PROXY = string.Empty;
        // PROXY = REAL IP OF PROXY(MAINSERVER)
        // -------------------------------------------------------------------------------------- \\

        // REAL PORTS
        // -------------------------------------------------------------------------------------- \\
        public static int GATEWAY_LISTEN_PORT = 1337;
        public static int AGENT_LISTEN_PORT = 1338;
        public static int AGENT2_LISTEN_PORT = 1338;
        // GATEWAY, AGENT = FAKE IF PROXY(MAINSERVER)
        // -------------------------------------------------------------------------------------- \\

        // FAKE PORTS
        // -------------------------------------------------------------------------------------- \\
        public static int FAKE_GATEWAY = 15779;
        public static int FAKE_AGENT = 15884;
        public static int FAKE_AGENT2 = 15886;
        // GATEWAY, AGENT = REAL IF PROXY(MAINSERVER)
        // -------------------------------------------------------------------------------------- \\

        // Something for manuel
        public static string REMOTE_GATEWAY = "127.0.0.1";
        public static string REMOTE_AGENT = "127.0.0.1";
        public static string REMOTE_AGENT_2 = "127.0.0.1";
        public static bool PIONEER = false;

        // Dictionary for EXPLOIT / LOG
        // -------------------------------------------------------------------------------------- \\
        public static Dictionary<ushort, string> Opcodes = new Dictionary<ushort, string>();
        public static Dictionary<ushort, string> BAD_Opcodes = new Dictionary<ushort, string>();
        // -------------------------------------------------------------------------------------- \\

        // Exploit MaxBytes
        // -------------------------------------------------------------------------------------- \\
        public static int dMaxBytesPerSec_Gateway = 1000;
        public static int dMaxBytesPerSec_Agent = 1000;
        // -------------------------------------------------------------------------------------- \\

        // Exploit Max packet count
        // -------------------------------------------------------------------------------------- \\
        public static int GATEWAY_COUNT = 25;
        public static int AGENT_COUNT = 25;
        // -------------------------------------------------------------------------------------- \\

        // CUSTOM FEATURES
        // -------------------------------------------------------------------------------------- \\
        // SQL DATABASES
        public static string SUP_DB = "SUPERMIKE";
        public static string ACC_DB = "SRO_VT_ACCOUNT";
        public static string SHA_DB = "SRO_VT_SHARD";
        public static string LOG_DB = "SRO_VT_LOG";

        // New shits
        public static int player_limit = 250;

        // SERVER NAME
        public static bool block_status = false;
        public static string ServerName = Environment.UserName;
        public static int shard_players = 0;
        public static int FAKE_PLAYERS = 0;
        public static int MAX_PLAYERS = 1000;
        public static int ShardID = 64;
        public static bool ENABLED = false;
        public static List<string> USER_ID = new List<string>();

        // REMOVE CAPTCHA?
        public static bool Captcha_Remove = false;

        // CAPTCHA CHAR / IF REMOVE?
        public static string Captcha_Char = string.Empty;

        // [SilkSystem] \\
        public static bool SilkEnable = false;
        public static int SilkLevel = 0;
        public static int SilkAmount = 1;
        public static int SilkDelay = 3600000;
        public static bool SilkNotice = false;
        public static string SilkType = "SILK";
        public static bool SilkRewardAfk = false;

        // [DELAYS] \\
        public static int Stalldelay = 10;
        public static int Exchangedelay = 10;
        public static int Globaldelay = 0;
        public static int Reversedelay = 0;
        public static int Logoutdelay = 0;
        public static int Restartdelay = 0;
        public static int zerkdelay = 0;

        // [LIMITS] \\
        public static int IPLIMIT = 0;
        public static int IPGLOBAL = 0;
        public static int CAFELIMIT = 0;
        public static int PCLIMIT = 0;
        public static List<string> ip_list = new List<string>();
        public static List<string> flood_list = new List<string>();
        public static List<string> cafe_list = new List<string>();
        public static List<string> hwid_list = new List<string>();
        public static Dictionary<string, string> hwid_user = new Dictionary<string, string>();
        public static List<string> LIMIT_BYPASS = new List<string>();

        // New idea
        public static List<string> last_opcodes = new List<string>();

        // [PROTECTION] \\
        public static int FLOOD_LIMIT = 30;
        public static string FLOOD_METHOD = string.Empty;
        public static string EXPLOIT_METHOD = string.Empty;
        public static string PACKET_METHOD = string.Empty;
        public static string UNKNOWN_METHOD = string.Empty;

        // [GMSTUFF] \\
        public static List<string> GM_ACCOUNT = new List<string>();
        public static List<string> PRIV_IP = new List<string>();
        public static bool GM_LOGIN = false;
        public static bool START_VISIBLE = false;
        public static bool GM_WHITE = true;

        // [ADVANCED] \\
        public static bool job_reverse = false;
        public static bool job_res = false;
        public static bool job_exchange = false;
        public static int CTF_level = 0;
        public static int ARENA_level = 0;
        public static int global_level = 0;
        public static int stall_level = 0;
        public static int plus_limit = 0;
        public static bool log_players = false;

        // [NOTICE] \\
        public static bool PlusNotice = false;
        public static int RequiredPlus = 0;

        // [MISC] \\
        public static bool WelcomeMsg = false;
        public static string WelcomeText = string.Empty;
        public static string MESSAGE_HANDLE = string.Empty;
        public static bool restart_button = false;
        public static bool avatar_blues = false;

        // BAN LIST
        public static List<string> ban_list = new List<string>();

        // UNKNOWN LIST
        public static List<string> unknown_list = new List<string>();

        // [BOTS] \\
        public static List<string> bot_list = new List<string>();
        public static bool BOT_ALLOW = true;
        public static bool BOT_ALCHEMY_ELIXIR = true;
        public static bool BOT_ALCHEMY_STONE = true;
        public static bool BOT_AVATAR_BLUES = true;
        public static bool BOT_CREATE_PARTY = true;
        public static bool BOT_INVITE_PARTY = true;
        public static bool BOT_EXCHANGE = true;
        public static bool BOT_STALL = true;
        public static bool BOT_ARENA = true;
        public static bool BOT_CTF = true;
        public static bool BOT_FORTRESS = true;
        public static bool BOT_DETECTION = false;
        public static bool BOT_WARNING = false;
        public static bool BOT_PVP = true;
        public static bool BOT_TRACE = true;

        // Translations
        public static string SILK_NOTICE = "SilkSystem : You have received 1 silk(s), for playing on the server!";
        public static string GN_NOTICE = "You are not allowed to use GM commands";

        public static string ACADEMY_CREATION = "Academy create: This system has been disabled!";
        public static string ACADEMY_INVITE = "Academy invite: This system has been disabled!";

        public static string BOT_NOTICE_FORTRESS = "Third-part programs aren't allowed to enter fortress!";
        public static string BOT_NOTICE_CTF = "Third-part programs aren't allowed to join CTF!";
        public static string BOT_NOTICE_ARENA = "Third-part programs aren't allowed to join Arena!";
        public static string BOT_NOTICE_FUSE = "Third-part programs aren't allowed to fuse items!";
        public static string BOT_NOTICE_ALCHEMY = "Third-part programs aren't allowed to fuse items!";
        public static string BOT_NOTICE_DISCONNECT = "Third-part programs aren't allowed to enter this server!";
        public static string BOT_NOTICE_AVATAR_BLUES = "Third-part programs aren't allowed to change avatar blues!";
        public static string BOT_NOTICE_STALL_CREATE = "Third-part programs aren't allowed to create a stall!";
        public static string BOT_NOTICE_PARTY_CREATE = "Third-part programs aren't allowed to create party matches!";
        public static string BOT_NOTICE_PARTY_INVITE = "Third-part programs aren't allowed to invite to party matches!";
        public static string BOT_NOTICE_EXCHANGE = "Third-part programs aren't allowed to exchange others!";
        public static string BOT_NOTICE_PVP = "Third-part programs aren't allowed to use PVP cape!";
        public static string BOT_NOTICE_TRACE = "Third-part programs aren't allowed to trace!";

        public static string CHEAT_NOTICE = "You are now being monitored! Don't try to cheat!";
        public static string CHEAT_NOTICE2 = "Cannot terminate vehicles and transports near towns. You can drop goods when your 300m out of town. PS: Click on the ground not the sky!";
        public static string CHEAT_NOTICE3 = "Picking items during job inside towns isnt allowed. PS: Click on the ground not the sky!";

        public static string SKILL_NOTICE = "You cannot use this skill inside fortress arena.";
        public static string SKILL_NOTICE2 = "You cannot trace inside the fortress arena.";
        public static string SKILL_NOTICE3 = "Rescurrent scrolls are disabled in fortress war!";

        public static string LOCAL_CHAT = "Do you kiss your mother with that mouth?";
        public static string PRIVATE_CHAT = "Do you kiss your mother with that mouth?";
        public static string PARTY_CHAT = "Do you kiss your mother with that mouth?";
        public static string PARTY_MATCH = "Cowabunga, dude!";
        public static string PRIVATE_MESSAGE = "Do you kiss your mother with that mouth?";
        public static string GLOBAL_MESSAGE = "You may not write this word, sorry!";
        public static string STALL_FILTER = "You may not write this word, sorry!";

        public static string CTF_LEVEL = "You are too low level, you need atleast level (1) to enter CTF!";
        public static string ARENA_LEVEL = "You are too low level, you need atleast level (1) to enter Arena!";
        public static string GLOBAL_LEVEL = "You are too low level, you need atleast level (1) to use this item.";
        public static string STALL_LEVEL = "You are too low level, you need atleast level ({level}) to open a stall.";
        public static string UNION_LIMIT = "Union limit: Please teleport before inviting more unions!";
        public static string UNION_LIMIT2 = "Union limit: You have reached the max concurrent unions!";
        public static string GUILD_LIMIT = "Guild limit: Please teleport before inviting more members!";
        public static string GUILD_LIMIT2 = "Guild Limit: You have reached the max concurrent members!";
        public static string PLUS_LIMIT = "Plus limit: This item has reached it's maximum plus.";


        public static string EXCHANGE_DELAY = "Exchange delay: You must wait {time} more seconds.";
        public static string STALL_DELAY = "Stall delay: You must wait {time} more seconds.";
        public static string GLOBAL_DELAY = "Global delay: You must wait {time} more seconds.";
        public static string REVERSE_DELAY = "Reverse delay: You must wait {time} more seconds.";
        public static string LOGOUT_DELAY = "Logout delay: You must wait {time} more seconds.";
        public static string RESTART_DELAY = "Restart delay: You must wait {time} more seconds.";
        public static string ZERK_DELAY = "Zerk delay: You must wait {time} more seconds.";
        public static string STALL_EXPLOIT = "Stall exploit: Please teleport before opening stall.";

        public static string JOB_NOTICE_REVERSE = "Reverse scrolls are disabled under job state!";
        public static string JOB_NOTICE_TRACE = "You cannot trace during job mode, sorry!";
        public static string JOB_NOTICE_RESCURRENT = "Rescurrent scrolls are disabled under job state!";
        public static string JOB_NOTICE_EXCHANGE = "Exchange: Disabled under job state!";

        public static string GM_NOTICE = "You are not allowed to use GM commands";
        public static string RESTART_BUTTON = "The restart function is disabled, use exit!";
        public static string AVATAR_BLUES = "You cannot grant blues on avatars, sorry!";
        public static string AVATAR_EXPLOIT = "You cannot grant these blues on avatars!";
        public static string INVISIBLE_EXPLOIT = "This exploit has been blocked, nice try!";
        public static string DISCONNECT_MESSAGE = "You have been disconnted from the server!";
        public static string BAN_MESSAGE = "You have been banned from the server!";

        // [PVP] \\
        public static bool PVP_DUEL = false;
        public static string PVP_WIN = string.Empty;
        public static string PVP_LOOSE = string.Empty;
        public static bool PVP_RANKING = false;
        public static int PVP_TITLE = 0;
        public static bool PVP_DISABLE_PARTY = false;
        public static bool PVP_DISABLE_OUTSIDERS = false;
        public static bool PVP_DISABLE_ZERK = false;

        // [PREMIUM] \\
        public static bool JOB_CHEAT = false;
        public static bool JOB_TRACE = false;
        public static bool CURRENT_REGION = false;
        public static bool STORE_HWID = false;
        public static bool FORTRESS = false;
        public static bool FORTRESS_ANTI_TRACE = false;
        public static bool FORTRESS_ANTI_RES_SCROLL = false;
        public static int Guild_limit = 0;
        public static int Union_limit = 0;
        public static bool ACADEMY_DISABLED = false;
        public static bool ACADEMY_INVITE_DISABLED = false;
        public static int THIEF_MUST_SELL = 0;
        public static string FILTER_NAME = "[SUPERMIKE]";


        // EPIC SRO
        public static bool EPIC_ITEM = false;

        // - PREMIUM LISTS
        public static List<int> fortressRegions = new List<int>();
        public static List<short> Regions = new List<short>();
        public static List<uint> fortressTele = new List<uint>();
        public static List<uint> Jobcavetele = new List<uint>();
        public static List<short> tradeRegions = new List<short>();
        public static List<int> FortressSkills = new List<int>();
        public static List<int> BlockedSkills = new List<int>();
        public static List<string> badwords = new List<string>();

        // CTF / BATTLE ARENA STUFF
        public static List<string> event_chars = new List<string>();
        public static bool ctf_called = false;
        public static bool arena_called = false;
        public static bool timer = false;
        // -------------------------------------------------------------------------------------- \\
    }
}