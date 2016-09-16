using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Linq;
using Framework;
using System.Data.SqlClient;

namespace ExploitFilter.NetEngine
{
    sealed class AgentContext
    {
        Socket m_ClientSocket = null;
        AsyncServer.E_ServerType m_HandlerType;
        AsyncServer.delClientDisconnect m_delDisconnect;
        object m_Lock = new object();
        Socket m_ModuleSocket = null;

        /*
        Old buffers
        byte[] m_LocalBuffer = new byte[8192];
        byte[] m_RemoteBuffer = new byte[8192];
        */

        // New buffers
        byte[] m_LocalBuffer = new byte[8192];
        byte[] m_RemoteBuffer = new byte[8192];

        Security m_LocalSecurity = new Security();
        Security m_RemoteSecurity = new Security();

        Thread m_TransferPoolThread = null;
        public static Queue<Packet> m_LastPackets = new Queue<Packet>(20);
        int num2 = (int)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds;
        ulong m_BytesRecvFromClient = 0;
        public bool startnotice = false;
        public bool error = false;
        DateTime m_StartTime = DateTime.Now;

        // Module name
        string module_name = "AGENT";
        int length = 0;
        int count_ip = 0;

        // IP LISTS idk
        public static List<string> iplist = new List<string>();
        public static List<string> netcafeiplist = new List<string>();

        // bye bye
        public static List<string> bye = new List<string>();
        public static List<string> bye2 = new List<string>();

        // STALL FIX
        bool StallBlock = true;

        // STALL DELAY
        DateTime laststalltime = new DateTime();

        // EXCHANGE DELAY
        DateTime lastexchangetime = new DateTime();

        // GLOBAL DELAY
        DateTime global_time = new DateTime();

        // REVERSE DELAY
        DateTime reverse_time = new DateTime();

        // RESTART DELAY
        DateTime lastrestarttime = new DateTime();

        // LOGOUT DELAY
        DateTime lastlogouttime = new DateTime();

        // ZERK DELAY
        DateTime lastzerktime = new DateTime();

        // Job cheaters
        short selfWalkLatestRegion = -1;
        bool ignorepackets;
        public uint charObjectID;
        uint TargetTeleport;
        short LastestRegion = 0;
        bool AtFortress = true;
        bool AtJobCave = false;

        // Packet
        int packetCount = 0;
        Timer t1;
        int can_connect_yes = 0;

        // Silk per hour
        Timer silk;

        // Useless crap
        string charname = string.Empty;
        string sql_charname = string.Empty;
        string gm_name = string.Empty;
        string user_id = string.Empty;
        string user_pw = string.Empty;
        string client_ip = string.Empty;
        string hwid = string.Empty;

        // Bypass fixes
        int guild_limit = 0;
        int union_limit = 0;
        bool CharName_sent = false;

        // Timlock
        UInt32 Unique_ID = 0;

        // CharData stuff
        bool Chinese = false;
        UInt32 RefObjID = 0;
        int Scale = 0;
        int CurLevel = 0;
        int MaxLevel = 0;
        ulong ExpOffSet = 0;
        ulong SExpOffSet = 0;
        ulong RemainGold = 0;
        ulong RemainSkillPoint = 0;
        ulong RemainStatPoint = 0;
        int RemainHwan = 0;
        UInt32 GatheredExpPoint = 0;
        UInt32 HP = 0;
        UInt32 MP = 0;
        int AutoInverstEXP = 0;
        int DailyPK = 0;
        int TotalPK = 0;
        UInt32 PkPenatlyPoint = 0;
        int HwanLevel = 0;
        int PVPCape = 0;
        bool thief_town = false;
        bool information = false;

        // Client list game
        public static List<AgentContext> clientlistgame = new List<AgentContext>();

        // Silk per hour
        int SilkOK = 0;
        bool IsAfk = true;

        // User IP
        string ip = string.Empty;
        bool connect = true;

        // Duel system
        public static Dictionary<UInt32, UInt32> Duel = new Dictionary<UInt32, UInt32>();
        public static Dictionary<UInt32, string> Names = new Dictionary<UInt32, string>();
        bool duel_cape = false;
        bool cape_on = false;
        int cape_num = 0;
        bool party = false;

        // Elamidas shit
        public static object locker = new object();

        // Check allowed region
        public static bool isAllowedRegion(short region)
        {
            lock (locker)
            {
                try
                {
                    if (FilterMain.tradeRegions.Contains(region))
                    {
                        return false;
                    }
                    return true;
                }
                catch
                {
                    return true;
                }
            }
        }

        // Check bad words
        public static bool badword(string word)
        {
            lock (locker)
            {
                try
                {
                    foreach (string badword in FilterMain.badwords)
                    {
                        if (word.Contains(badword))
                        {
                            return true;
                        }
                    }
                    return false;
                }
                catch {
                    return false;
                }
            }
        }

        // Count IP list
        public static int ip_count(string ip)
        {
            lock (locker)
            {
                try
                {
                    // Count
                    //int count_ip = FilterMain.ip_list.Count(a => a == ip) + 1;
                    int count_ip = 0;

                    foreach (string ips in FilterMain.ip_list)
                    {
                        if (ips == ip)
                        {
                            count_ip++;
                        }
                    }

                    // Inform, MUST.
                    //Console.ForegroundColor = ConsoleColor.Green;
                    //Console.WriteLine(FilterMain.DATE + "[IPLIMIT] " + ip + ":" + count_ip);
                    //Console.ResetColor();

                    // Return limit
                    return count_ip;
                }
                catch {
                    return 0;
                }
            }
        }

        // Count flood list
        public static int flood_count(string ip)
        {
            lock (locker)
            {
                try
                {
                    // Count
                    int count_ip = 0;

                    foreach (string ips in FilterMain.flood_list)
                    {
                        if (ips == ip)
                        {
                            count_ip++;
                        }
                    }
                    return count_ip;
                }
                catch
                {
                    return 0;
                }
            }
        }

        // COUNT HWID
        public static int hwid_count(string hwid)
        {
            lock (locker)
            {
                try
                {
                    int count_hwid = 0;

                    foreach (string hwids in FilterMain.hwid_list)
                    {
                        if (hwids == hwid)
                        {
                            count_hwid++;
                        }
                    }

                    return count_hwid;
                }
                catch
                {
                    return 0;
                }
            }
        }

        public AgentContext(Socket ClientSocket, AsyncServer.delClientDisconnect delDisconnect)
        {
            this.m_delDisconnect = delDisconnect;
            this.m_ClientSocket = ClientSocket;
            this.m_HandlerType = AsyncServer.E_ServerType.AgentServer;
            this.m_ModuleSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            // Agent count
            FilterMain.agent1++;

            // Register ip
            this.ip = ((IPEndPoint)(m_ClientSocket.RemoteEndPoint)).Address.ToString();

            // GLOBAL IP LIST / FLOOD IP LIST
            FilterMain.flood_list.Add(this.ip);

            #region BAN LIST / BAN SYSTEM
            if (FilterMain.ban_list.Contains(this.ip))
            {
                // Disconnect
                this.connect = false;
                this.DisconnectModuleSocket();
                return;
            }
            #endregion

            #region GLOBAL IP LIMIT
            if (FilterMain.IPGLOBAL > 0)
            {
                // Net CAFE IP
                if (FilterMain.CAFELIMIT > 0 && (FilterMain.cafe_list.Contains(this.ip)))
                {
                    if (flood_count(this.ip) > FilterMain.CAFELIMIT)
                    {
                        // Disconnect
                        this.connect = false;
                        this.DisconnectModuleSocket();
                        return;
                    }
                }
                else
                {
                    // GLOBAL IP LIMIT
                    if (flood_count(this.ip) > FilterMain.IPGLOBAL)
                    {
                        // Disconnect
                        this.connect = false;
                        this.DisconnectModuleSocket();
                        return;
                    }
                }
            }
            #endregion

            #region Flood protection
            if (FilterMain.flood_fix)
            {
                if (flood_count(this.ip) > FilterMain.FLOOD_LIMIT)
                {
                    // IF BAN IP?
                    if (FilterMain.FLOOD_METHOD == "ban")
                    {
                        if (!FilterMain.ban_list.Contains(this.ip))
                        {
                            // Add ban
                            FilterMain.ban_list.Add(this.ip);

                            try
                            {
                                // File exist, write to it.
                                System.IO.StreamWriter file = new System.IO.StreamWriter("config/blacklist.txt", true);
                                if (this.ip.Length > 0)
                                {
                                    file.WriteLine(this.ip + "\n");
                                }
                                file.Close();

                                // Ban log(For checking random bans)
                                System.IO.StreamWriter banlog = new System.IO.StreamWriter("logs/banlog.txt", true);
                                banlog.WriteLine("[" + DateTime.UtcNow + "] {" + module_name + "} Banned {" + this.ip + "} Reason: {FLOOD ATTEMPT}\n");
                                banlog.Close();
                            }
                            catch { }

                            // Inform?
                            Console.ForegroundColor = ConsoleColor.DarkMagenta;
                            Console.WriteLine("[" + DateTime.UtcNow + "][" + module_name + "] IP:{" + this.ip + "} User:{" + this.user_id + "} Reason:{FLOODING} Bytes:{" + this.length + "} Packet_c{" + this.packetCount + "}");
                            Console.ResetColor();
                        }
                    }

                    // Disconnect
                    this.connect = false;
                    this.DisconnectModuleSocket();
                    return;
                }
            }
            #endregion

            try
            {
                if (FilterMain.PIONEER)
                {
                    if (this.connect)
                    {
                        this.m_ModuleSocket.Connect(new IPEndPoint(IPAddress.Parse(FilterMain.REMOTE_AGENT), FilterMain.AGENT_LISTEN_PORT));
                    }
                }
                else
                {
                    if (this.connect)
                    {
                        this.m_ModuleSocket.Connect(new IPEndPoint(IPAddress.Parse(FilterMain.AGENT_IP), FilterMain.AGENT_LISTEN_PORT));
                    }
                }

                // DISABLE MBOT
                //m_LocalSecurity.GenerateSecurity(true, false, false);
                this.m_LocalSecurity.GenerateSecurity(true, true, true);
                this.DoRecvFromClient();
                Send(false);
            } catch { }

            // Packet Timer
            this.t1 = new Timer(new TimerCallback(this.resetPackets), null, 0, 500);

            // Silk per hour
            this.silk = new Timer(new TimerCallback(this.SilkPerHour), null, 0, FilterMain.SilkDelay);
            this.IsAfk = true;

            // Anti trade bug
            this.selfWalkLatestRegion = -1;
            this.ignorepackets = false;

            // Stall exploit
            this.StallBlock = true;

            // Fortress shit
            this.AtFortress = true;

            // Job cave shit
            this.AtJobCave = false;

            // Client list
            clientlistgame.Add(this);

            // Packet length
            this.length = 0;

            // Anti exploit
            this.can_connect_yes = 0;

            // Bypass shits
            this.guild_limit = 0;
            this.union_limit = 0;
            this.CharName_sent = false;

            // Timlock
            this.Unique_ID = 0;

            // CharData parameters
            this.Chinese = false;
            this.RefObjID = 0;
            this.Scale = 0;
            this.CurLevel = 0;
            this.MaxLevel = 0;
            this.ExpOffSet = 0;
            this.SExpOffSet = 0;
            this.RemainGold = 0;
            this.RemainSkillPoint = 0;
            this.RemainStatPoint = 0;
            this.RemainHwan = 0;
            this.GatheredExpPoint = 0;
            this.HP = 0;
            this.MP = 0;
            this.AutoInverstEXP = 0;
            this.DailyPK = 0;
            this.TotalPK = 0;
            this.PkPenatlyPoint = 0;
            this.HwanLevel = 0;
            this.PVPCape = 0;
            this.thief_town = false;
            this.information = false;

            // PVP/DUEL SYSTEM
            this.cape_on = false;
            this.duel_cape = false;
            this.cape_num = 0;
            this.party = false;

        }

        double GetBytesPerSecondFromClient()
        {
            double res = 0.0;

            TimeSpan diff = (DateTime.Now - m_StartTime);
            if (m_BytesRecvFromClient > int.MaxValue)
                m_BytesRecvFromClient = 0;

            if (m_BytesRecvFromClient > 0)
            {
                try
                {
                    unchecked
                    {
                        double div = diff.TotalSeconds;
                        if (diff.TotalSeconds < 1.0)
                            div = 1.0;
                        res = Math.Round((m_BytesRecvFromClient / div), 2);
                    }
                }
                catch
                {
                }
            }

            return res;
        }

        void DisconnectModuleSocket()
        {
            try
            {
                // Cherno shit
                if (this.m_ModuleSocket != null)
                {
                    // Agent count
                    if (FilterMain.agent1 > 0)
                    {
                        FilterMain.agent1--;
                    }

                    // IP LIMIT
                    if (FilterMain.IPLIMIT > 0)
                    {
                        FilterMain.ip_list.Remove(this.ip);
                    }

                    // GLOBAL IP LIST / FLOOD IP LIST
                    FilterMain.flood_list.Remove(this.ip);

                    // Anti bot list
                    if (FilterMain.bot_list.Contains(this.user_id))
                    {
                        FilterMain.bot_list.Remove(this.user_id);
                    }

                    // PC LIMIT
                    if (FilterMain.PCLIMIT > 0 && !(FilterMain.LIMIT_BYPASS.Contains(this.user_id)))
                    {
                        try
                        {
                            if (FilterMain.hwid_user.ContainsKey(this.user_id))
                            {
                                FilterMain.hwid_list.Remove(FilterMain.hwid_user[this.user_id]);
                            }
                        }
                        catch
                        {
                            Console.WriteLine(module_name + " Error removing from HWID list!");
                        }
                    }

                    // LOG DB SYSTEM
                    if ((FilterMain.DB) && (FilterMain.log_players))
                    {
                        try
                        {
                            sqlCon.exec("EXEC [" + FilterMain.SUP_DB + "].[dbo].[_LogMembers] '" + Program.Plis(this.user_id) + "', '" + Program.Plis(this.sql_charname) + "', '" + this.ip + "', 0, 0");
                        }
                        catch { }
                    }

                    // Destroy timers
                    this.t1.Dispose();
                    this.silk.Dispose();

                    // Client list
                    clientlistgame.Remove(this);

                    // DISCONNECT
                    this.m_ModuleSocket.Close();
                }

                // NUll
                this.m_ModuleSocket = null;

            } catch { }
        }

        void OnReceive_FromServer(IAsyncResult iar)
        {
            lock (m_Lock)
            {
                try
                {
                    int nReceived = this.m_ModuleSocket.EndReceive(iar);

                    if (nReceived != 0)
                    {
                        this.m_RemoteSecurity.Recv(m_RemoteBuffer, 0, nReceived);

                        List<Packet> RemotePackets = m_RemoteSecurity.TransferIncoming();

                        if (RemotePackets != null)
                        {
                            foreach (Packet _pck in RemotePackets)
                            {
                                #region Handshake
                                if (_pck.Opcode == 0x5000 || _pck.Opcode == 0x9000)
                                {
                                    Send(true);
                                    continue;
                                }
                                #endregion

                                #region Plus Notice system
                                // Notice spam xD
                                if (_pck.Opcode == 0xB150 && FilterMain.DB && FilterMain.PlusNotice)
                                {
                                    try
                                    {
                                        _pck.Lock(); // Lock
                                        _pck.ReadUInt16();
                                        _pck.ReadUInt8();
                                        int slot = _pck.ReadUInt8();
                                        _pck.ReadUInt64();
                                        int plus = _pck.ReadUInt8();

                                        if (plus >= FilterMain.RequiredPlus && plus > 0)
                                        {
                                            try
                                            {
                                                // Get item name
                                                SqlDataReader reader = sqlCon.Return("_GetName", new SqlParameter("@CharName", Program.Plis(this.sql_charname)), new SqlParameter("@Slot", slot));
                                                reader.Read();

                                                // Item name
                                                string ItemName = reader["Name"].ToString();

                                                // Check if name is not null.
                                                if (ItemName != string.Empty && ItemName != null)
                                                {
                                                    // New method, notice sender.
                                                    sqlCon.exec("INSERT INTO [" + FilterMain.SUP_DB + "].[dbo].[_Notice] (message, added) VALUES('" + Program.Plis(this.sql_charname) + " has increased [" + ItemName + "] to plus (" + plus + ")', '" + DateTime.UtcNow + "')");
                                                }

                                                // Close DB
                                                reader.Close();
                                            }
                                            catch { }
                                        }
                                    }

                                    catch { }
                                }
                                #endregion

                                #region Region reader
                                // SERVER->CLIENT (MOVEMENT)
                                if (_pck.Opcode == 0xB021 && (FilterMain.CURRENT_REGION))
                                {
                                    try
                                    {
                                        _pck.Lock(); // Lock
                                        UInt32 Target = _pck.ReadUInt32(); // Unique ID from player

                                        // Check if target == this.unique_ID
                                        if (Target == this.Unique_ID)
                                        {
                                            byte unk = _pck.ReadUInt8();
                                            short Region = _pck.ReadInt16();
                                            int RegionL = Region.ToString().Length;

                                            // Make sure region is 5 numbers.
                                            if(RegionL == 5 && _pck.GetBytes().Length == 24) {
                                                // Register good.
                                                this.selfWalkLatestRegion = Region;
                                            }
                                            else
                                            {
                                                // Register false.
                                                this.selfWalkLatestRegion = -1;
                                            }

                                            /*Console.WriteLine("Current length: " + RegionL);
                                            Console.WriteLine("Current bytes: " + _pck.GetBytes().Length);
                                            Console.WriteLine("Current region: " + Region);
                                            */
                                        }
                                    }
                                    catch { }
                                }
                                #endregion

                                #region NEW Chardata packet
                                if (_pck.Opcode == 0x3013)
                                {
                                    try
                                    {
                                        /*
                                        Mike guide:

                                        ReadUInt8 = BYTE
                                        ReadUInt16 = SHORT
                                        ReadUInt32 = UINT
                                        ReadUInt64 = ULONG

                                        */
                                        _pck.Lock(); // Lock

                                        // General data
                                        _pck.ReadUInt32(); // Servertime
                                        this.RefObjID = _pck.ReadUInt32(); // RefObjID
                                        this.Scale = _pck.ReadUInt8(); // Scale
                                        this.CurLevel = _pck.ReadUInt8(); // Curlevel
                                        this.MaxLevel = _pck.ReadUInt8(); // Maxlevel
                                        this.ExpOffSet = _pck.ReadUInt64(); // ExpOffset
                                        this.SExpOffSet = _pck.ReadUInt32(); // SExpOffset
                                        this.RemainGold = _pck.ReadUInt64(); // RemainGold
                                        this.RemainSkillPoint = _pck.ReadUInt32(); // RemainSkillPoint
                                        this.RemainStatPoint = _pck.ReadUInt16(); // RemainStatPoints
                                        this.RemainHwan = _pck.ReadUInt8(); // RemainHwan
                                        this.GatheredExpPoint = _pck.ReadUInt32(); // GatheredExpPoint
                                        this.HP = _pck.ReadUInt32(); // HP
                                        this.MP = _pck.ReadUInt32(); // MP
                                        this.AutoInverstEXP = _pck.ReadUInt8(); // AutoInverstExp
                                        this.DailyPK = _pck.ReadUInt8(); // DailyPK
                                        this.TotalPK = _pck.ReadUInt8(); // TotalPK
                                        this.PkPenatlyPoint = _pck.ReadUInt32(); // PKPenaltyPoint
                                        this.PVPCape = _pck.ReadUInt8(); // PVPCape
                                        this.HwanLevel = _pck.ReadUInt8(); // HwanLevel

                                        // Inventory
                                        //Console.WriteLine(_pck.ReadUInt8());

                                        // Race switch.
                                        if (this.RefObjID < 14875)
                                        {
                                            // Char is chinese
                                            this.Chinese = true;
                                        }
                                        else
                                        {
                                            // Char is European
                                            this.Chinese = false;
                                        }


                                        
                                        /*
                                        Console.WriteLine("RefObjID = " + this.RefObjID);
                                        Console.WriteLine("Scale = " + this.Scale);
                                        Console.WriteLine("CurLevel = " + this.CurLevel);
                                        Console.WriteLine("MaxLevel = " + this.MaxLevel);
                                        Console.WriteLine("ExpOffSet = " + this.ExpOffSet);
                                        Console.WriteLine("SExpOffSet = " + this.SExpOffSet);
                                        Console.WriteLine("RemainGold = " + this.RemainGold);
                                        Console.WriteLine("RemainSkillPoint = " + this.RemainSkillPoint);
                                        Console.WriteLine("RemainStatPoint = " + this.RemainStatPoint);
                                        Console.WriteLine("RemainHwan = " + this.RemainHwan);
                                        Console.WriteLine("GatheredExpPoint = " + this.GatheredExpPoint);
                                        Console.WriteLine("HP = " + this.HP);
                                        Console.WriteLine("MP = " + this.MP);
                                        Console.WriteLine("AutoInverstEXP = " + this.AutoInverstEXP);
                                        Console.WriteLine("DailyPK = " + this.DailyPK);
                                        Console.WriteLine("TotalPK = " + this.TotalPK);
                                        Console.WriteLine("PkPenatlyPoint = " + this.PkPenatlyPoint);
                                        Console.WriteLine("HwanLevel = " + this.HwanLevel);
                                        Console.WriteLine("PVPCape = " + this.PVPCape);
                                        Console.WriteLine("Chinese = " + this.Chinese);
                                        Console.WriteLine();
                                        Console.WriteLine();
                                        */
                                    }
                                    catch { }
                                }
                                #endregion

                                #region Party (Player) Left/Banned/Dismissed
                                if(_pck.Opcode == 0x3864)
                                {
                                    try
                                    {
                                        _pck.Lock(); // Lock
                                        byte value = _pck.ReadUInt8(); // Value

                                        // Party dissmissed
                                        if (value == 1)
                                        {
                                            // Register
                                            this.party = false;
                                        }
                                        // Player left or banned
                                        else if (value == 3)
                                        {
                                            // Player who left
                                            uint Player = _pck.ReadUInt32();

                                            // If you are the player
                                            if (Player == this.Unique_ID)
                                            {
                                                this.party = false;
                                            }
                                        }
                                    }
                                    catch { }
                                }
                                #endregion

                                #region ENTER PVP MODE
                                if (_pck.Opcode == 0xB516 && (FilterMain.PVP_DUEL))
                                {
                                    try
                                    {
                                        _pck.Lock(); // Lock

                                        // Packet structur
                                        if (_pck.ReadUInt8() == 1)
                                        {
                                            uint User = _pck.ReadUInt32(); // Unknown
                                            int cape = _pck.ReadUInt8();
                                            /* CAPE COLOR
                                                0 = off
                                                1 = red
                                                2 = black
                                                3 = blue
                                                4 = white
                                                5 = yellow
                                            */
                                            // If this char equip shit
                                            if (User == this.Unique_ID)
                                            {
                                                // Register cape
                                                this.cape_num = cape;

                                                // CharName
                                                if (!Names.ContainsKey(this.Unique_ID))
                                                {
                                                    Names.Add(this.Unique_ID, this.charname);
                                                }

                                                // OFF
                                                if (cape == 0)
                                                {
                                                    this.cape_on = false;
                                                    this.SendNotice("You are no longer in PVP mode!");

                                                    // Duel system
                                                    if (Duel.ContainsKey(this.Unique_ID))
                                                    {
                                                        Duel.Remove(this.Unique_ID);
                                                    }
                                                }
                                                // ON
                                                else if (cape > 0 && cape < 5)
                                                {
                                                    this.cape_on = true;
                                                    this.SendNotice("You are now in DUEL mode!");

                                                    // Duel system
                                                    if (Duel.ContainsKey(this.Unique_ID))
                                                    {
                                                        Duel.Remove(this.Unique_ID);
                                                        Duel.Add(this.Unique_ID, 0);
                                                    }
                                                    else
                                                    {
                                                        Duel.Add(this.Unique_ID, 0);
                                                    }
                                                }
                                                else
                                                {
                                                    this.cape_on = true;
                                                    this.SendNotice("You are now in FFA mode!");

                                                    // Duel system
                                                    if (Duel.ContainsKey(this.Unique_ID))
                                                    {
                                                        Duel.Remove(this.Unique_ID);
                                                        Duel.Add(this.Unique_ID, 1);
                                                    }
                                                    else
                                                    {
                                                        Duel.Add(this.Unique_ID, 1);
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    catch { }
                                }
                                #endregion

                                #region Character Celestial
                                if (_pck.Opcode == 0x3020)
                                {
                                    try
                                    {
                                        _pck.Lock(); // Lock
                                        this.Unique_ID = _pck.ReadUInt32();
                                        //Console.WriteLine(this.Unique_ID);
                                    }
                                    catch { }
                                }
                                #endregion

                                #region Attack packet
                                /*if(_pck.Opcode == 0x3057 && (this.cape_on))
                                {
                                    try
                                    {
                                        _pck.Lock(); // Lock
                                        uint TargetID = _pck.ReadUInt32();
                                        uint unk1 = _pck.ReadUInt16();
                                        byte data1 = _pck.ReadUInt8();
                                        uint unk2 = _pck.ReadUInt32();

                                        continue;
                                    }
                                    catch { }
                                }*/
                                #endregion

                                #region Target packet
                                /*if (_pck.Opcode == 0xB070 && (this.cape_on))
                                {
                                    try
                                    {
                                        //_pck.Lock(); // Lock
                                        byte data1 = _pck.ReadUInt8();
                                        byte data2 = _pck.ReadUInt8();
                                        byte data3 = _pck.ReadUInt8();

                                        if (data1 == 0x01 && data2 == 0x02 && data3 == 0x30)
                                        {
                                            uint SkillID = _pck.ReadUInt32();
                                            uint UniqueID = _pck.ReadUInt32();
                                            uint unk1 = _pck.ReadUInt32();
                                            uint TargetID = _pck.ReadUInt32();

                                            Console.WriteLine("Attacker: " + UniqueID);
                                            Console.WriteLine("SkillID: " + SkillID);
                                            Console.WriteLine("Unknown: " + unk1);
                                            Console.WriteLine("Target: " + TargetID);
                                            Console.WriteLine(Duel[this.Unique_ID]);
                                            Console.WriteLine(Duel[TargetID]);
                                            Console.WriteLine();

                                            if (Duel[this.Unique_ID] != 0)
                                            {
                                                this.SendNotice("You are already in combat with another user.");
                                                continue;
                                            }
                                            else if (Duel[TargetID] != 0)
                                            {
                                                this.SendNotice("This user is already in combat with another user.");
                                            }
                                            if (Duel[TargetID] == 0 && Duel[this.Unique_ID] == 0)
                                            {
                                                // Unregister
                                                Duel.Remove(this.Unique_ID);
                                                Duel.Remove(TargetID);

                                                // Register
                                                Duel.Add(this.Unique_ID, TargetID);
                                                Duel.Add(TargetID, this.Unique_ID);
                                            }

                                            else if (Duel[this.Unique_ID] != TargetID && Duel[TargetID] != this.Unique_ID)
                                            {
                                                this.SendNotice("This user is currently in combat, please find another one!");
                                                continue;
                                            }

                                            //Console.WriteLine(Duel[this.Unique_ID] + ":" + TargetID);
                                            //Console.WriteLine(Duel[TargetID] + ":" + this.Unique_ID);

                                            byte flag = _pck.ReadUInt8();
                                            if (flag == 0x01)
                                            {

                                            }
                                        }
                                    }
                                    catch { }
                                }*/
                                #endregion

                                #region KILLS
                                if (_pck.Opcode == 0x30BF && (FilterMain.PVP_DUEL) && (this.cape_on))
                                {
                                    try
                                    {
                                        _pck.Lock(); // Lock lel
                                        uint unk = _pck.ReadUInt32();
                                        byte unk1 = _pck.ReadUInt8();
                                        byte state = _pck.ReadUInt8();

                                        // No fucking idea, but fixed my error.
                                        if (unk1 == 0)
                                        {
                                            // Killed Target
                                            if (state == 2)
                                            {
                                                // Duel capes only
                                                if (this.cape_num != 5)
                                                {
                                                    uint Target = Duel[this.Unique_ID];

                                                    /*
                                                    Console.WriteLine("Unique ID " + unk);
                                                    Console.WriteLine("Target ID " + Target);
                                                    Console.WriteLine("Attacked ID " + this.Unique_ID);
                                                    */

                                                    if (unk == Target && Duel[unk] == this.Unique_ID)
                                                    {
                                                        // Message
                                                        this.SendNotice("You won against " + Names[Target] + "!");
                                                    }
                                                    else if (unk == this.Unique_ID)
                                                    {
                                                        // Message
                                                        this.SendNotice("You lost against " + Names[Target] + "!");
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    catch { }
                                }
                                #endregion

                                // Send packet
                                m_LocalSecurity.Send(_pck);
                                Send(false);
                            }
                        }
                    }
                    else
                    {
                        try
                        {
                            // Abort connection
                            this.m_TransferPoolThread.Abort();
                        }
                        catch { }

                        // Disconnect
                        this.DisconnectModuleSocket();
                        this.m_delDisconnect.Invoke(ref m_ClientSocket, m_HandlerType);
                        return;
                    }

                    // continue receiving data from remote (server module)
                    this.DoRecvFromServer();
                }
                catch
                {
                    try
                    {
                        // Abort connection
                        this.m_TransferPoolThread.Abort();
                    }
                    catch { }

                    // Disconnect
                    this.DisconnectModuleSocket();
                    this.m_delDisconnect.Invoke(ref m_ClientSocket, m_HandlerType);
                }
            }
        }


        public void Send(bool ToHost)//that codes done by Excellency he fixed mbot for me
        {
            lock (m_Lock)
                foreach (var p in (ToHost ? m_RemoteSecurity : m_LocalSecurity).TransferOutgoing())
                {

                    Socket ss = (ToHost ? m_ModuleSocket : m_ClientSocket);

                    ss.Send(p.Key.Buffer);
                    if (ToHost)
                    {
                        try
                        {
                            m_BytesRecvFromClient += (ulong)p.Key.Size;

                            double nBps = GetBytesPerSecondFromClient();
                            if (nBps > FilterMain.dMaxBytesPerSec_Agent)
                            {

                                // Exploit
                                if (FilterMain.ERROR_LOG.Equals("exploit") || FilterMain.ERROR_LOG.Equals("all"))
                                {
                                    // Inform?
                                    Console.ForegroundColor = ConsoleColor.DarkYellow;
                                    Console.WriteLine("[" + DateTime.UtcNow + "][" + module_name + "] IP:{" + this.ip + "} User:{" + this.user_id + "} Bytes:{" + nBps + "} Bytes:{" + this.length + "} Packet_c{" + this.packetCount + "}");
                                    Console.ResetColor();
                                }

                                try
                                {
                                    // Abort connection
                                    this.m_TransferPoolThread.Abort();
                                }
                                catch { }

                                // Disconnect
                                this.DisconnectModuleSocket();
                                this.m_delDisconnect.Invoke(ref m_ClientSocket, m_HandlerType);
                            }
                        }
                        catch
                        {
                            try
                            {
                                // Abort connection
                                this.m_TransferPoolThread.Abort();
                            }
                            catch { }

                            // Disconnect
                            this.DisconnectModuleSocket();
                            this.m_delDisconnect.Invoke(ref m_ClientSocket, m_HandlerType);
                        }

                    }
                }
        }

        // Reset packet count.
        public void resetPackets(object e)
        {
            this.packetCount = 0;
        }

        #region SilkSystem
        // Silk per hour system.
        public void SilkPerHour(object e)
        {
            try
            {
                // Check if DB connect + Silk system enabled.
                if (FilterMain.DB && FilterMain.SilkEnable)
                {
                    // Store playtime
                    if(FilterMain.log_players)
                    {
                        int Time = FilterMain.SilkDelay / 60000;
                        try
                        {
                            // Avoid error, son <3
                            if (this.sql_charname.Length > 0)
                            {
                                sqlCon.exec("EXEC [" + FilterMain.SUP_DB + "].[dbo].[_LogMembers] '" + Program.Plis(this.user_id) + "', '" + Program.Plis(this.sql_charname) + "', '" + this.ip + "', '" + Time + "', 1");
                            }
                        }
                        catch { }
                    }

                    // Silk reward
                    if (!FilterMain.SilkRewardAfk)
                    {
                        // User is AFK, no reward.
                        if (this.IsAfk)
                        {
                            // Register stuff
                            this.SilkOK = 0;
                        }
                        // User is not afk, reward.
                        else
                        {
                            this.SilkOK = 1;
                        }
                    }
                    // System not enabled, give reward.
                    else
                    {
                        // Register stuff
                        this.SilkOK = 1;
                    }

                    // If char level system
                    if (FilterMain.SilkLevel > 0 && this.SilkOK == 1)
                    {
                        // Check so they choosed a char.
                        if (this.charname.Length > 0)
                        {
                            // GET CHAR LEVEL
                            //SqlDataReader reader = sqlCon.Return("["+FilterMain.SUP_DB+"].[dbo].[_MainFunctions]", new SqlParameter("@Charname", Program.Plis(this.sql_charname)), new SqlParameter("@Action", 2));
                            //reader.Read();

                            if (this.CurLevel < FilterMain.SilkLevel)
                            {
                                // Level not OK, ignore
                                this.SilkOK = 0;
                            }
                            else
                            {
                                // Level OK, lets continue
                                this.SilkOK = 1;
                            }

                            // Close SQL
                            //reader.Close();
                        }
                    }

                    // Check if OK
                    if (this.SilkOK == 1)
                    {
                        // Check if username entered
                        if (this.user_id.Length > 0)
                        {
                            // Check if Silk amount greater then 0
                            if (FilterMain.SilkAmount > 0)
                            {
                                // Should be OK to grant silk.
                                try
                                {
                                    sqlCon.exec("EXEC [" + FilterMain.SUP_DB + "].[dbo].[_SilkSystem] '" + Program.Plis(this.user_id) + "', " + FilterMain.SilkAmount + ", '" + FilterMain.SilkType + "'");
                                }
                                catch { }

                                if (FilterMain.SilkNotice)
                                {
                                    // New shit
                                    string message = FilterMain.SILK_NOTICE.Replace("{reward}", FilterMain.SilkAmount.ToString());

                                    // Notice
                                    this.SendNotice(message);
                                }
                            }
                        }
                    }
                }
            }
            catch { }
        }
        #endregion

        void OnReceive_FromClient(IAsyncResult iar)
        {
            lock (m_Lock)
            {
                try
                {
                    int nReceived = m_ClientSocket.EndReceive(iar);

                    if (nReceived != 0)
                    {

                        m_LocalSecurity.Recv(m_LocalBuffer, 0, nReceived);

                        List<Packet> ReceivedPackets = m_LocalSecurity.TransferIncoming();
                        if (ReceivedPackets != null)
                        {
                            foreach (Packet _pck in ReceivedPackets)
                            {
                                // Length of packet
                                this.length = _pck.GetBytes().Length;

                                // Packet count
                                this.packetCount++;

                                #region Packet protection
                                // Packet count
                                if (this.packetCount >= FilterMain.AGENT_COUNT)
                                {
                                    // Ignore spam plis
                                    if (FilterMain.ERROR_LOG.Equals("unknown") || FilterMain.ERROR_LOG.Equals("all") || FilterMain.ERROR_LOG.Equals("exploit"))
                                    {
                                        // Write to console log
                                        Console.ForegroundColor = ConsoleColor.DarkYellow;
                                        Console.WriteLine("[" + DateTime.UtcNow + "][" + module_name + "] IP:{" + this.ip + "} User:{" + this.user_id + "} Opcode:{0x" + _pck.Opcode.ToString("X") + "} Bytes:{" + this.length + "} Packet_c{" + this.packetCount + "}");
                                        Console.ResetColor();
                                    }

                                    // IF BAN IP?
                                    if (FilterMain.PACKET_METHOD == "ban")
                                    {
                                        if (!FilterMain.ban_list.Contains(this.ip))
                                        {
                                            // Add ban
                                            FilterMain.ban_list.Add(this.ip);

                                            try
                                            {
                                                // File exist, write to it.
                                                System.IO.StreamWriter file = new System.IO.StreamWriter("config/blacklist.txt", true);
                                                if (this.ip.Length > 0)
                                                {
                                                    file.WriteLine(this.ip + "\n");
                                                }
                                                file.Close();

                                                // Ban log(For checking random bans)
                                                System.IO.StreamWriter banlog = new System.IO.StreamWriter("logs/banlog.txt", true);
                                                banlog.WriteLine("[" + DateTime.UtcNow + "] {" + module_name + "} Banned {" + this.ip + "} Reason: {HIGH PPS}\n");
                                                banlog.Close();
                                            }
                                            catch { }

                                            // Inform?
                                            Console.ForegroundColor = ConsoleColor.DarkMagenta;
                                            Console.WriteLine("[" + DateTime.UtcNow + "][" + module_name + "] IP:{" + this.ip + "} User:{" + this.user_id + "} Opcode:{0x" + _pck.Opcode.ToString("X") + "} Bytes:{" + this.length + "} Packet_c{" + this.packetCount + "}");
                                            Console.ResetColor();
                                        }
                                    }

                                    // Disconnect
                                    this.DisconnectModuleSocket();
                                    return;
                                }
                                #endregion

                                #region Logging system
                                // LEGIT OPCODES
                                if (FilterMain.ERROR_LOG.Equals("legit") || FilterMain.ERROR_LOG.Equals("all"))
                                {
                                    if (FilterMain.Opcodes.ContainsKey(Convert.ToUInt16(_pck.Opcode)))
                                    {
                                        // ALLOWED OPCODE
                                        string name = FilterMain.Opcodes[Convert.ToUInt16(_pck.Opcode)];
                                        Console.ForegroundColor = ConsoleColor.Green;
                                        Console.WriteLine("[" + DateTime.UtcNow + "][" + module_name + "] IP:{" + this.ip + "} User:{" + this.user_id + "} Opcode:{" + name + "} Bytes:{" + this.length + "} Packet_c{" + this.packetCount + "}");
                                        Console.ResetColor();
                                    }
                                }

                                // EXPLOIT OPCODES
                                else if (FilterMain.ERROR_LOG.Equals("exploit") || FilterMain.ERROR_LOG.Equals("all"))
                                {
                                    if (FilterMain.BAD_Opcodes.ContainsKey(Convert.ToUInt16(_pck.Opcode)))
                                    {
                                        // SENT EXPLOITS
                                        string name = FilterMain.BAD_Opcodes[Convert.ToUInt16(_pck.Opcode)];
                                        Console.ForegroundColor = ConsoleColor.DarkYellow;
                                        Console.WriteLine("[" + DateTime.UtcNow + "][" + module_name + "] IP:{" + this.ip + "} User:{" + this.user_id + "} Opcode:{0x" + _pck.Opcode.ToString("X") + "} Bytes:{" + this.length + "} Packet_c{" + this.packetCount + "}");
                                        Console.ResetColor();
                                    }
                                }

                                // UNKNOWN OPCODES
                                else if (FilterMain.ERROR_LOG.Equals("unknown") || FilterMain.ERROR_LOG.Equals("all"))
                                {
                                    if (!FilterMain.Opcodes.ContainsKey(Convert.ToUInt16(_pck.Opcode)) && !FilterMain.BAD_Opcodes.ContainsKey(Convert.ToUInt16(_pck.Opcode)))
                                    {
                                        // SENT UNKNOWN SHIT
                                        Console.ForegroundColor = ConsoleColor.DarkRed;
                                        Console.WriteLine("[" + DateTime.UtcNow + "][" + module_name + "] IP:{" + this.ip + "} User:{" + this.user_id + "} Opcode:{0x" + _pck.Opcode.ToString("X") + "} Bytes:{" + this.length + "} Packet_c{" + this.packetCount + "}");
                                        Console.ResetColor();
                                    }
                                }
                                #endregion

                                #region Store system
                                // Store system
                                if (FilterMain.logging)
                                {
                                    if (!FilterMain.Opcodes.ContainsKey(Convert.ToUInt16(_pck.Opcode)) && !FilterMain.BAD_Opcodes.ContainsKey(Convert.ToUInt16(_pck.Opcode)))
                                    {
                                        try
                                        {
                                            // Prevent opening errors.
                                            //FilterMain.unknown_list.Add("0x" + _pck.Opcode.ToString("X"));

                                            // Write/Create w/e
                                            System.IO.StreamWriter file = new System.IO.StreamWriter("logs/unknown.txt", true);
                                            file.WriteLine("[" + DateTime.UtcNow + "][" + module_name + "] IP:{" + this.ip + "} User:{" + this.user_id + "} Opcode:{0x" + _pck.Opcode.ToString("X") + "} Bytes:{" + this.length + "} Packet_c{" + this.packetCount + "}");
                                            file.Close();
                                        }
                                        catch { }

                                        // DISCONNECT
                                        if (FilterMain.UNKNOWN_METHOD.Equals("disconnect"))
                                        {
                                            if (FilterMain.ERROR_LOG.Equals("unknown") || FilterMain.ERROR_LOG.Equals("all"))
                                            {
                                                // Inform?
                                                Console.ForegroundColor = ConsoleColor.DarkRed;
                                                Console.WriteLine("[" + DateTime.UtcNow + "][" + module_name + "] IP:{" + this.ip + "} User:{" + this.user_id + "} Opcode:{0x" + _pck.Opcode.ToString("X") + "} Bytes:{" + this.length + "} Packet_c{" + this.packetCount + "}");
                                                Console.ResetColor();
                                            }

                                            // Disconnect
                                            this.DisconnectModuleSocket();
                                            continue;
                                        }

                                        // BAN
                                        else if (FilterMain.UNKNOWN_METHOD.Equals("ban"))
                                        {
                                            if (!FilterMain.ban_list.Contains(this.ip))
                                            {
                                                // Add ban
                                                FilterMain.ban_list.Add(this.ip);

                                                try
                                                {
                                                    // File exist, write to it.
                                                    System.IO.StreamWriter file = new System.IO.StreamWriter("config/blacklist.txt", true);
                                                    if (this.ip.Length > 0)
                                                    {
                                                        file.WriteLine(this.ip + "\n");
                                                    }
                                                    file.Close();

                                                    // Ban log(For checking random bans)
                                                    System.IO.StreamWriter banlog = new System.IO.StreamWriter("logs/banlog.txt", true);
                                                    banlog.WriteLine("[" + DateTime.UtcNow + "] {" + module_name + "} Banned {" + this.ip + "} Reason: {UNKNOWN PACKET}\n");
                                                    banlog.Close();
                                                }
                                                catch { }

                                                // Inform?
                                                Console.ForegroundColor = ConsoleColor.DarkMagenta;
                                                Console.WriteLine("[" + DateTime.UtcNow + "][" + module_name + "] IP:{" + this.ip + "} User:{" + this.user_id + "} Opcode:{0x" + _pck.Opcode.ToString("X") + "} Bytes:{" + this.length + "} Packet_c{" + this.packetCount + "}");
                                                Console.ResetColor();

                                                // Disconnect
                                                this.DisconnectModuleSocket();
                                                return;
                                            }
                                        }
                                    }
                                }
                                #endregion

                                #region Flood fix for 0x2001
                                // Filter against flood
                                if (_pck.Opcode == 0x2001)
                                {
                                    // Recieve shit
                                    this.DoRecvFromServer();
                                    continue;
                                }
                                #endregion

                                #region Charname packet
                                // Get char name
                                if (_pck.Opcode == 0x7001)
                                {
                                    // Check if already being sent.
                                    if (this.CharName_sent)
                                    {
                                        // Exploit
                                        if (FilterMain.ERROR_LOG.Equals("exploit") || FilterMain.ERROR_LOG.Equals("all"))
                                        {
                                            // Inform?
                                            Console.ForegroundColor = ConsoleColor.DarkYellow;
                                            Console.WriteLine("[" + DateTime.UtcNow + "][" + module_name + "] IP:{" + this.ip + "} User:{" + this.user_id + "} Reason:{CharName exploit} Bytes:{" + this.length + "} Packet_c{" + this.packetCount + "}");
                                            Console.ResetColor();
                                        }

                                        // Disconnect
                                        this.DisconnectModuleSocket();
                                        return;
                                    }

                                    // Register
                                    this.CharName_sent = true;

                                    // Length of charname.
                                    if (this.length > 2)
                                    {
                                        // Read packet
                                        this.charname = _pck.ReadAscii();

                                        // Register SQL charname
                                        this.sql_charname = this.charname;

                                        // Check if exist
                                        if (this.charname.Contains("[") || this.charname.Contains("]"))
                                        {
                                            this.sql_charname = this.sql_charname.Replace("[", "%[%");
                                            this.sql_charname = this.sql_charname.Replace("]", "%]%");
                                            this.sql_charname = this.sql_charname.Replace(" ", string.Empty);
                                        }
                                    }
                                    else
                                    {
                                        // Exploit
                                        if (FilterMain.ERROR_LOG.Equals("exploit") || FilterMain.ERROR_LOG.Equals("all"))
                                        {
                                            // Inform?
                                            Console.ForegroundColor = ConsoleColor.DarkYellow;
                                            Console.WriteLine("[" + DateTime.UtcNow + "][" + module_name + "] IP:{" + this.ip + "} User:{" + this.user_id + "} Opcode:{0x" + _pck.Opcode.ToString("X") + "} Bytes:{" + this.length + "} Packet_c{" + this.packetCount + "}");
                                            Console.ResetColor();
                                        }

                                        // Disconnect
                                        this.DisconnectModuleSocket();
                                        return;
                                    }
                                }
                                #endregion

                                #region Guild limit
                                if (_pck.Opcode == 0x70f3 && (FilterMain.DB) && (FilterMain.Guild_limit > 0))
                                {
                                    // Invite pending limit
                                    if (this.guild_limit >= FilterMain.Guild_limit)
                                    {
                                        this.SendNotice(FilterMain.GUILD_LIMIT);
                                        continue;
                                    }

                                    // Count
                                    this.guild_limit++;

                                    try
                                    {
                                        // Get guild count
                                        SqlDataReader reader = sqlCon.Return("[" + FilterMain.SUP_DB + "].[dbo].[_MainFunctions]", new SqlParameter("@Charname", Program.Plis(this.sql_charname)), new SqlParameter("@Action", 4));
                                        reader.Read();

                                        // Check shit
                                        if (reader.GetInt32(0) >= FilterMain.Guild_limit)
                                        {
                                            // Message handler
                                            if (FilterMain.MESSAGE_HANDLE == "notice")
                                            {
                                                this.SendNotice(FilterMain.GUILD_LIMIT2);
                                            }
                                            else if (FilterMain.MESSAGE_HANDLE == "private")
                                            {
                                                this.SendPM(FilterMain.GUILD_LIMIT2);
                                            }

                                            // Close reader
                                            reader.Close();

                                            // Continue
                                            continue;
                                        }
                                        // Close reader
                                        reader.Close();
                                    }
                                    catch
                                    {
                                        continue;
                                    }
                                }
                                #endregion

                                #region Union limit
                                if (_pck.Opcode == 0x70fb && (FilterMain.DB) && (FilterMain.Union_limit > 0))
                                {
                                    // Invite pending limit
                                    if (this.union_limit >= FilterMain.Union_limit)
                                    {
                                        this.SendNotice(FilterMain.UNION_LIMIT);
                                        continue;
                                    }

                                    // Count
                                    this.union_limit++;

                                    try
                                    {
                                        // Get guild count
                                        SqlDataReader reader = sqlCon.Return("[" + FilterMain.SUP_DB + "].[dbo].[_MainFunctions]", new SqlParameter("@Charname", Program.Plis(this.sql_charname)), new SqlParameter("@Action", 5));
                                        reader.Read();

                                        // Check shit
                                        if (reader.GetInt32(0) >= FilterMain.Union_limit)
                                        {
                                            // Message handler
                                            if (FilterMain.MESSAGE_HANDLE == "notice")
                                            {
                                                this.SendNotice(FilterMain.UNION_LIMIT2);
                                            }
                                            else if (FilterMain.MESSAGE_HANDLE == "private")
                                            {
                                                this.SendPM(FilterMain.UNION_LIMIT2);
                                            }

                                            // Close reader
                                            reader.Close();

                                            // Continue
                                            continue;
                                        }
                                        // Close reader
                                        reader.Close();
                                    }
                                    catch
                                    {
                                        continue;
                                    }
                                }
                                #endregion

                                #region GM security
                                if (_pck.Opcode == 0x7010 && !(FilterMain.USER_ID.Contains(this.user_id)))
                                {
                                    // Get command
                                    uint CommandID = _pck.ReadUInt16();

                                    // GM security
                                    if (!FilterMain.GM_ACCOUNT.Contains(this.user_id) && (FilterMain.GM_ACCOUNT.Count() > 0))
                                    {
                                        this.SendNotice(FilterMain.GM_NOTICE);
                                        this.DisconnectModuleSocket();
                                        return;
                                    }

                                    // Stop GM abuse using (/mobkill 0)
                                    if (CommandID == 20)
                                    {
                                        uint k_mob_id = _pck.ReadUInt32();
                                        Packet p = new Packet(0x7010);
                                        p.WriteUInt16(20);
                                        p.WriteUInt32(k_mob_id);
                                        p.WriteUInt8(1);
                                        m_RemoteSecurity.Send(p);
                                        Send(false);
                                    }
                                }
                                #endregion

                                #region Academy creation disable
                                if (_pck.Opcode == 0x7470)
                                {
                                    if (FilterMain.ACADEMY_DISABLED)
                                    {
                                        if (FilterMain.MESSAGE_HANDLE == "notice")
                                        {
                                            this.SendNotice(FilterMain.ACADEMY_CREATION);
                                        }
                                        else if (FilterMain.MESSAGE_HANDLE == "private")
                                        {
                                            this.SendPM(FilterMain.ACADEMY_CREATION);
                                        }
                                        continue;
                                    }
                                }
                                #endregion

                                #region Academy invite disabled
                                if (_pck.Opcode == 0x7472 || _pck.Opcode == 0x747E || _pck.Opcode == 0x347F)
                                {
                                    // Check if disabled
                                    if (FilterMain.ACADEMY_INVITE_DISABLED)
                                    {
                                        if (FilterMain.MESSAGE_HANDLE == "notice")
                                        {
                                            this.SendNotice(FilterMain.ACADEMY_INVITE);
                                        }
                                        else if (FilterMain.MESSAGE_HANDLE == "private")
                                        {
                                            this.SendPM(FilterMain.ACADEMY_INVITE);
                                        }
                                        continue;
                                    }
                                }
                                #endregion

                                #region TELEPORT PACKET SHIT
                                // Get teleport data packet
                                if (_pck.Opcode == 0x705a)
                                {
                                    _pck.ReadUInt32(); // Unknown

                                    // Fuck silkroad right?
                                    if (_pck.ReadUInt8() == 2)
                                    {
                                        // Read target teleport
                                        this.TargetTeleport = _pck.ReadUInt32();

                                        // New jobcave shit
                                        if (FilterMain.Jobcavetele.Contains(this.TargetTeleport) && (FilterMain.JOB_CHEAT))
                                        {
                                            this.AtJobCave = true;
                                        }
                                        else
                                        {
                                            this.AtJobCave = false;
                                        }

                                        // New fortress shit
                                        if (FilterMain.fortressTele.Contains(this.TargetTeleport) && (!FilterMain.BOT_FORTRESS))
                                        {
                                            // Register fortress
                                            this.AtFortress = true;

                                            // Block bots from entering fortress
                                            if (FilterMain.bot_list.Contains(this.user_id) && (!FilterMain.BOT_FORTRESS))
                                            {
                                                this.SendNotice(FilterMain.BOT_NOTICE_FORTRESS);
                                                continue;
                                            }
                                        }
                                        else
                                        {
                                            // Register fortress
                                            this.AtFortress = false;
                                        }
                                    }
                                }
                                #endregion

                                #region Request weather <3
                                if (_pck.Opcode == 0x750e && FilterMain.FILES.Contains("vsro"))
                                {
                                    // Region shit
                                    this.selfWalkLatestRegion = -1;

                                    // Stall block
                                    this.StallBlock = false;

                                    // PVP cape
                                    this.cape_on = false;
                                    this.cape_num = 0;

                                    // Guild limit
                                    if (FilterMain.DB && (FilterMain.Guild_limit > 0))
                                    {
                                        try
                                        {
                                            SqlDataReader guild = sqlCon.Return("[" + FilterMain.SUP_DB + "].[dbo].[_MainFunctions]", new SqlParameter("@Charname", Program.Plis(this.sql_charname)), new SqlParameter("@Action", 4));
                                            guild.Read();
                                            // Set guild limit
                                            this.guild_limit = guild.GetInt32(0);
                                            guild.Close();
                                        }
                                        catch { }
                                    }

                                    // Union limit
                                    if (FilterMain.DB && (FilterMain.Union_limit > 0))
                                    {
                                        try
                                        {
                                            SqlDataReader union = sqlCon.Return("[" + FilterMain.SUP_DB + "].[dbo].[_MainFunctions]", new SqlParameter("@Charname", Program.Plis(this.sql_charname)), new SqlParameter("@Action", 5));
                                            union.Read();
                                            // Set union limit
                                            this.union_limit = union.GetInt32(0);
                                            union.Close();
                                        }
                                        catch { }
                                    }

                                    // GM anti-invisible
                                    if (FilterMain.GM_ACCOUNT.Contains(this.user_id) && (FilterMain.START_VISIBLE))
                                    {
                                        Packet Nigga = new Packet(0x7010);
                                        Nigga.WriteUInt16(14);
                                        m_RemoteSecurity.Send(Nigga);
                                        Send(false);
                                    }

                                    // Last Region checker || When fortress stuff
                                    if (FilterMain.DB)
                                    {
                                        try
                                        {
                                            // Read fortress information
                                            this.LastestRegion = sqlCon.ReadShort("SELECT LatestRegion FROM [" + FilterMain.SHA_DB + "].[dbo].[_Char] where CharName16 like '" + Program.Plis(this.sql_charname) + "'");

                                            // Register fortress status
                                            if (FilterMain.fortressRegions.Contains(this.LastestRegion))
                                            {
                                                this.AtFortress = true;
                                            }
                                            else
                                            {
                                                this.AtFortress = false;
                                            }

                                            // Register thief town
                                            if (this.LastestRegion == 24758)
                                            {
                                                this.thief_town = true;
                                            }
                                            else
                                            {
                                                this.thief_town = false;
                                            }
                                        }
                                        catch { }

                                        // LOG USERS(DATABASE)
                                        if (FilterMain.log_players && (this.information))
                                        {
                                            try
                                            {
                                                // Log the mother fucker.
                                                sqlCon.exec("EXEC [" + FilterMain.SUP_DB + "].[dbo].[_LogMembers] '" + Program.Plis(this.user_id) + "', '" + Program.Plis(this.sql_charname) + "', '" + this.ip + "', 0, 1");

                                                // Register
                                                this.information = false;
                                            }
                                            catch { }
                                        }
                                    }
                                }
                                #endregion

                                #region JOB ANTI CHEAT, MIKE BEST
                                // CLIENT_PET_TERMINATE
                                if (_pck.Opcode == 0x70c6 && (FilterMain.JOB_CHEAT) && !(FilterMain.USER_ID.Contains(this.user_id)))
                                {
                                    // Check list
                                    if (!(isAllowedRegion(this.selfWalkLatestRegion) && (this.selfWalkLatestRegion != -1)))
                                    {
                                        this.SendNotice(FilterMain.CHEAT_NOTICE2);
                                        continue;
                                    }
                                }

                                // CLIENT_PET_MOVEMENT
                                if (_pck.Opcode == 0x70c5 && (FilterMain.CURRENT_REGION))
                                {
                                    this.selfWalkLatestRegion = -1;
                                }
                                #endregion

                                #region DEAD STATE
                                if (_pck.Opcode == 0x3053 && (FilterMain.PVP_DUEL) && (this.cape_on))
                                {
                                    byte Status = _pck.ReadUInt8();
                                    /*
                                        1 == Resurrect at the present point.
                                        2 == Resurrect at the present point.
                                     */

                                    if (Status == 1 || Status == 2)
                                    {
                                        if (Duel.ContainsKey(this.Unique_ID))
                                        {
                                            if (this.cape_num != 5)
                                            {
                                                // Register (killer)
                                                uint Target = Duel[this.Unique_ID];

                                                // Remove Target(Dead boy)
                                                Duel.Remove(Target);
                                                Duel.Add(Target, 0);

                                                // Remove Attacker(You)
                                                Duel.Remove(this.Unique_ID);
                                                Duel.Add(this.Unique_ID, 0);
                                            }
                                        }
                                    }
                                }
                                #endregion

                                #region MAIN ACTION: Block skills, Block Trace
                                // Main action
                                if (_pck.Opcode == 0x7074 && !(FilterMain.USER_ID.Contains(this.user_id)))
                                {
                                    /*
                                        1 = Attack success
                                        2 = Cancel attack
                                    */
                                    // Derp
                                    if (_pck.ReadUInt8() == 1)
                                    {
                                        // Action
                                        byte num = _pck.ReadUInt8();

                                        /*
                                        num values
                                            1 = Normal attack
                                            2 = Pickup / Cancel attack
                                            3 = Trace
                                            4 = Skill cast
                                        
                                        */

                                        // Attack, SKill cast
                                        if (num == 1 || num == 4)
                                        {
                                            try
                                            {
                                                ushort SkillID = 0;
                                                uint TargetID = 0;

                                                if(num == 1)
                                                {
                                                    _pck.ReadUInt8();
                                                    TargetID = _pck.ReadUInt32();
                                                }
                                                else
                                                {
                                                    SkillID = _pck.ReadUInt16();
                                                    _pck.ReadUInt8(); // Unknown
                                                    _pck.ReadUInt16(); // Unknown
                                                    TargetID = _pck.ReadUInt32();
                                                }

                                                /*
                                                Console.WriteLine(SkillID);
                                                Console.WriteLine(TargetID);
                                                Console.WriteLine();
                                                Console.WriteLine();
                                                */

                                                // SKILL CAST
                                                if (num == 4)
                                                {
                                                    // Check if gay
                                                    if (FilterMain.event_chars.Contains(this.charname))
                                                    {
                                                        // Check skillid
                                                        if (FilterMain.BlockedSkills.Contains(SkillID))
                                                        {
                                                            // Inform, rip <3
                                                            this.SendNotice("You are not allowed to use this skills.");

                                                            // Block em
                                                            continue;
                                                        }
                                                    }

                                                    // Check if current region active.
                                                    if (FilterMain.CURRENT_REGION && FilterMain.FORTRESS)
                                                    {
                                                        // Block skill
                                                        if (FilterMain.FortressSkills.Contains(SkillID))
                                                        {
                                                            //if (FilterMain.fortressRegions.Contains(this.selfWalkLatestRegion) || (this.selfWalkLatestRegion == -1))
                                                            if (this.AtFortress)
                                                            {
                                                                // Message handler
                                                                if (FilterMain.MESSAGE_HANDLE == "notice")
                                                                {
                                                                    this.SendNotice(FilterMain.SKILL_NOTICE);
                                                                }
                                                                else if (FilterMain.MESSAGE_HANDLE == "private")
                                                                {
                                                                    this.SendPM(FilterMain.SKILL_NOTICE);
                                                                }

                                                                // Continue
                                                                continue;
                                                            }
                                                        }
                                                    }
                                                }
                                                
                                                // Check if PVP cape on.
                                                if (FilterMain.PVP_DUEL && this.cape_on)
                                                {
                                                    if (Duel[TargetID] == 0 && Duel[this.Unique_ID] == 0)
                                                    {
                                                        // Unregister
                                                        Duel.Remove(this.Unique_ID);
                                                        Duel.Remove(TargetID);

                                                        // Register
                                                        Duel.Add(this.Unique_ID, TargetID);
                                                        Duel.Add(TargetID, this.Unique_ID);
                                                    }
                                                    //else if (Duel[this.Unique_ID] != TargetID)
                                                    else
                                                    {
                                                        // Avoid people in FFA attacking DUEL mode
                                                        if (Duel[this.Unique_ID] == 1 && Duel[TargetID] != 1)
                                                        {
                                                            this.SendNotice("You can only attack people in FFA mode!");
                                                            continue;
                                                        }
                                                        // Avoid people in DUEL mode to attack FFA mode.
                                                        else if (Duel[TargetID] == 1 && Duel[this.Unique_ID] != 1)
                                                        {
                                                            this.SendNotice("You can only attack people in DUEL mode!");
                                                            continue;
                                                        }

                                                        // Avoid gang bang in DUEL
                                                        if (Duel[TargetID] != 1 && Duel[this.Unique_ID] != 1)
                                                        {
                                                            if (Duel[this.Unique_ID] != TargetID)
                                                            {
                                                                this.SendNotice("You can't attack this user for the moment.");
                                                                continue;
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                            catch { }
                                        }

                                        // PICKUP
                                        if (num == 2)
                                        {
                                            // Check if job cheat
                                            if (FilterMain.CURRENT_REGION && FilterMain.JOB_CHEAT && FilterMain.DB)
                                            {
                                                // Rekt noobs.
                                                if (!isAllowedRegion(this.selfWalkLatestRegion) || this.selfWalkLatestRegion == -1)
                                                {
                                                    try
                                                    {
                                                        // Read SQL shit
                                                        SqlDataReader reader = sqlCon.Return("[" + FilterMain.SUP_DB + "].[dbo].[_MainFunctions]", new SqlParameter("@Charname", Program.Plis(this.sql_charname)), new SqlParameter("@Action", 1));
                                                        reader.Read();

                                                        // Check if wearing job suits
                                                        if (reader.GetInt32(0) == 1)
                                                        {
                                                            // Check if char at jobcave
                                                            if (!this.AtJobCave)
                                                            {
                                                                // Message handler
                                                                if (FilterMain.MESSAGE_HANDLE == "notice")
                                                                {
                                                                    this.SendNotice(FilterMain.CHEAT_NOTICE3);
                                                                }
                                                                else if (FilterMain.MESSAGE_HANDLE == "private")
                                                                {
                                                                    this.SendPM(FilterMain.CHEAT_NOTICE3);
                                                                }

                                                                // Close reader
                                                                reader.Close();

                                                                // Continue
                                                                continue;
                                                            }
                                                        }

                                                        // Close SQL
                                                        reader.Close();
                                                    }
                                                    catch
                                                    {
                                                        continue;
                                                    }
                                                }
                                            }
                                        }

                                        // Read trace shit
                                        if (num == 3)
                                        {
                                            // Anti-trace bot
                                            if (FilterMain.bot_list.Contains(this.user_id) && (!FilterMain.BOT_TRACE))
                                            {
                                                this.SendNotice(FilterMain.BOT_NOTICE_TRACE);
                                                continue;
                                            }

                                            // Check if DB connected
                                            if (FilterMain.DB && FilterMain.JOB_TRACE)
                                            {
                                                try
                                                {
                                                    // Read SQL shit
                                                    SqlDataReader reader = sqlCon.Return("[" + FilterMain.SUP_DB + "].[dbo].[_MainFunctions]", new SqlParameter("@Charname", Program.Plis(this.sql_charname)), new SqlParameter("@Action", 1));
                                                    reader.Read();

                                                    // Check if wearing job suits
                                                    if (reader.GetInt32(0) == 1)
                                                    {
                                                        // Message handler
                                                        if (FilterMain.MESSAGE_HANDLE == "notice")
                                                        {
                                                            this.SendNotice(FilterMain.JOB_NOTICE_TRACE);
                                                        }
                                                        else if (FilterMain.MESSAGE_HANDLE == "private")
                                                        {
                                                            this.SendPM(FilterMain.JOB_NOTICE_TRACE);
                                                        }

                                                        // Close reader
                                                        reader.Close();

                                                        // Continue
                                                        continue;
                                                    }

                                                    // Close SQL
                                                    reader.Close();
                                                }
                                                catch
                                                {
                                                    continue;
                                                }
                                            }

                                            // Stop tracers in fortress
                                            if (FilterMain.FORTRESS_ANTI_TRACE)
                                            {
                                                //if (FilterMain.fortressRegions.Contains(this.selfWalkLatestRegion) || (this.selfWalkLatestRegion == -1))
                                                if (this.AtFortress)
                                                {
                                                    // Message handler
                                                    if (FilterMain.MESSAGE_HANDLE == "notice")
                                                    {
                                                        this.SendNotice(FilterMain.SKILL_NOTICE2);
                                                    }
                                                    else if (FilterMain.MESSAGE_HANDLE == "private")
                                                    {
                                                        this.SendPM(FilterMain.SKILL_NOTICE2);
                                                    }
                                                    continue;
                                                }
                                            }
                                        }
                                    }
                                }
                                #endregion

                                #region CLIENT_MOVE_ITEM
                                if (_pck.Opcode == 0x7034 && (FilterMain.THIEF_MUST_SELL > 0) && (FilterMain.EPIC_ITEM))
                                {
                                    // Action ID.
                                    byte action = _pck.ReadUInt8();
                                    /*
                                        ^ Declare
                                        0 = Inventory
                                        4 = Exchange
                                        9 = Blacksmith sell item
                                        20 = Thief town NPC
                                        30 = Guild input item
                                        31 = Guild output item
                                    */

                                    // Exchange item
                                    if (action == 4)
                                    {
                                        // Get item slot
                                        byte slot = _pck.ReadUInt8();

                                        try
                                        {
                                            // SQL part.
                                            SqlDataReader reader = sqlCon.Return("[" + FilterMain.SUP_DB + "].[dbo].[_EPICSRO]", new SqlParameter("@Charname", Program.Plis(this.sql_charname)), new SqlParameter("@slot", slot));
                                            reader.Read();

                                            // Block, send notice.
                                            if (reader.GetInt32(0) == 1)
                                            {
                                                this.SendNotice("You are not allowed to exchange items with Advanced elixir on!");
                                                continue; // Derp
                                            }

                                            reader.Close();
                                        }
                                        catch
                                        {
                                            continue; // In case of fuck up. (:
                                        }
                                    }
                                    else if (action == 20 && (this.thief_town)) // THIEF TOWN NPC
                                    {
                                        _pck.ReadUInt32(); // Item ID
                                        _pck.ReadUInt8(); // Unknown
                                        ushort amount = _pck.ReadUInt16();

                                        // Check amount.
                                        if (amount != FilterMain.THIEF_MUST_SELL)
                                        {
                                            this.SendNotice("You must sell " + FilterMain.THIEF_MUST_SELL + " units.");
                                            continue;
                                        }
                                    }
                                    else if (action == 30) // Guild storage
                                    {
                                        // Get item slot
                                        byte slot = _pck.ReadUInt8();

                                        try
                                        {
                                            // SQL part.
                                            SqlDataReader reader = sqlCon.Return("[" + FilterMain.SUP_DB + "].[dbo].[_EPICSRO]", new SqlParameter("@Charname", Program.Plis(this.sql_charname)), new SqlParameter("@slot", slot));
                                            reader.Read();

                                            // Block, send notice.
                                            if (reader.GetInt32(0) == 1)
                                            {
                                                this.SendNotice("You are not allowed to exchange items with Advanced elixir on!");
                                                continue; // Derp
                                            }

                                            reader.Close();
                                        }
                                        catch
                                        {
                                            continue; // In case of fuck up. (:
                                        }
                                    }
                                }
                                #endregion

                                #region Private message handle
                                if (_pck.Opcode == 0x7309 && (FilterMain.badwords.Count > 0) && !(FilterMain.USER_ID.Contains(this.user_id)))
                                {
                                    // Get message
                                    string ToUser = _pck.ReadAscii();
                                    string Message = _pck.ReadAscii().ToString().ToLower();

                                    // Check if badword
                                    if (badword(Message))
                                    {
                                        // Message handler
                                        if (FilterMain.MESSAGE_HANDLE == "notice")
                                        {
                                            this.SendNotice(FilterMain.PRIVATE_MESSAGE);
                                        }
                                        else if (FilterMain.MESSAGE_HANDLE == "private")
                                        {
                                            this.SendPM(FilterMain.PRIVATE_MESSAGE);
                                        }
                                        continue;
                                    }
                                }
                                #endregion

                                #region Bad words shit
                                // Bad words
                                if (_pck.Opcode == 0x7025 && !(FilterMain.USER_ID.Contains(this.user_id)))
                                {
                                    // Type of chat.
                                    int type = _pck.ReadUInt8();

                                    // Message
                                    string message = _pck.ReadAscii().ToString().ToLower();

                                    /*
                                    1 = LOCAL CHAT
                                    2 = PRIVATE CHAT
                                    3 = GM CHAT
                                    4 = PARTY CHAT
                                    5 = GUILD CHAT
                                    11 = UNION CHAT

                                    */

                                    // PVP surrender
                                    if (type == 1 || type == 3)
                                    {
                                        // Fyll Fix
                                        if (FilterMain.PVP_DUEL && (this.cape_on))
                                        {
                                            // PVP CHAT COMMANDS
                                            if (message.Contains("!surrender") || message.Contains("!ff") || message.Contains("!draw"))
                                            {
                                                if (Duel.ContainsKey(this.Unique_ID))
                                                {
                                                    if (this.cape_num != 5)
                                                    {
                                                        // Register (killer)
                                                        uint Attacker = Duel[this.Unique_ID];

                                                        // Remove
                                                        Duel.Remove(this.Unique_ID);
                                                        Duel.Remove(Attacker);
                                                        Duel.Add(this.Unique_ID, 0);
                                                        Duel.Add(Attacker, 0);

                                                        // Message
                                                        this.SendNotice("The duel was cancelled!");
                                                    }
                                                }
                                            }
                                        }
                                    }

                                    // BAD WORDS FILTER
                                    if (FilterMain.badwords.Count > 0)
                                    {
                                        // Local chat
                                        if (type == 1)
                                        {
                                            // Bad words checker
                                            if (badword(message))
                                            {
                                                if (FilterMain.MESSAGE_HANDLE == "notice")
                                                {
                                                    this.SendNotice(FilterMain.LOCAL_CHAT);
                                                }
                                                else if (FilterMain.MESSAGE_HANDLE == "private")
                                                {
                                                    this.SendPM(FilterMain.LOCAL_CHAT);
                                                }
                                                continue;
                                            }
                                        }
                                        // Private chat
                                        else if (type == 2)
                                        {
                                            // Bad words checker
                                            if (badword(message))
                                            {
                                                if (FilterMain.MESSAGE_HANDLE == "notice")
                                                {
                                                    this.SendNotice(FilterMain.PRIVATE_CHAT);
                                                }
                                                else if (FilterMain.MESSAGE_HANDLE == "private")
                                                {
                                                    this.SendPM(FilterMain.PRIVATE_CHAT);
                                                }
                                                continue;
                                            }
                                        }
                                        // Party Chat
                                        else if (type == 4)
                                        {
                                            // Bad words checker
                                            if (badword(message))
                                            {
                                                if (FilterMain.MESSAGE_HANDLE == "notice")
                                                {
                                                    this.SendNotice(FilterMain.PARTY_CHAT);
                                                }
                                                else if (FilterMain.MESSAGE_HANDLE == "private")
                                                {
                                                    this.SendPM(FilterMain.PARTY_CHAT);
                                                }
                                                continue;
                                            }
                                        }
                                    }
                                }
                                #endregion

                                #region Party match create/edit
                                if (_pck.Opcode == 0x7069 || _pck.Opcode == 0x706a)
                                {
                                    // PVP DUEL SHIT
                                    if(FilterMain.PVP_DUEL && (FilterMain.PVP_DISABLE_PARTY) && (this.cape_on))
                                    {
                                        // Make sure in duel mode
                                        if(this.cape_num < 5)
                                        {
                                            this.SendNotice("You are not able to join or edit party matches when you are in DUEL mode!");
                                            continue;
                                        }
                                    }

                                    // Register
                                    this.party = true;

                                    // Read shits
                                    uint PartyNumber = _pck.ReadUInt32(); // Party number
                                    uint Type = _pck.ReadUInt32(); // Type
                                    uint Race = _pck.ReadUInt8(); // Race
                                    uint Purpose = _pck.ReadUInt8(); // Purpose of party
                                    uint EntryLevel = _pck.ReadUInt8(); // Enter level
                                    uint MaxLevel = _pck.ReadUInt8(); // Max level
                                    string Title = _pck.ReadAscii().ToString().ToLower(); // Title

                                    // Anti-bot party match
                                    if (FilterMain.bot_list.Contains(this.user_id) && (!FilterMain.BOT_CREATE_PARTY))
                                    {
                                        this.SendNotice(FilterMain.BOT_NOTICE_PARTY_CREATE);
                                        continue;
                                    }

                                    // Bad words checker
                                    if (badword(Title) && FilterMain.badwords.Count > 0)
                                    {
                                        if (FilterMain.MESSAGE_HANDLE == "notice")
                                        {
                                            this.SendNotice(FilterMain.PARTY_MATCH);
                                        }
                                        else if (FilterMain.MESSAGE_HANDLE == "private")
                                        {
                                            this.SendPM(FilterMain.PARTY_MATCH);
                                        }
                                        continue;
                                    }
                                }
                                #endregion

                                #region Party match join
                                if(_pck.Opcode == 0x706d)
                                {
                                    // PVP DUEL DISABLE SHIT
                                    if(FilterMain.PVP_DUEL && (FilterMain.PVP_DISABLE_PARTY) && (this.cape_on))
                                    {
                                        // Make sure in DUEL mode.
                                        if(this.cape_num < 5)
                                        {
                                            this.SendNotice("You are not allowed to join a party match when you are in DUEL mode!");
                                            continue;
                                        }
                                    }

                                    // Register
                                    this.party = true;
                                }
                                #endregion

                                #region Party send invite
                                if(_pck.Opcode == 0x7060)
                                {
                                    // PVP DUEL DISABLE INVITE
                                    if(FilterMain.PVP_DUEL && (FilterMain.PVP_DISABLE_PARTY) && (this.cape_on))
                                    {
                                        // Make sure in DUEL mode.
                                        if(this.cape_num < 5) {
                                            this.SendNotice("You are not allowed to send party invites, when you are inside DUEL mode!");
                                            continue;
                                        }
                                    }

                                    // Anti-bot party match
                                    if (FilterMain.bot_list.Contains(this.user_id) && (!FilterMain.BOT_INVITE_PARTY))
                                    {
                                        this.SendNotice(FilterMain.BOT_NOTICE_PARTY_INVITE);
                                        continue;
                                    }
                                }
                                #endregion

                                #region Party accept invite
                                if (_pck.Opcode == 0x3080)
                                {
                                    // PVP DUEL DISABLE SHIT
                                    if (FilterMain.PVP_DUEL && (FilterMain.PVP_DISABLE_PARTY) && (this.cape_on))
                                    {
                                        // Make sure in DUEL mode.
                                        if (this.cape_num < 5)
                                        {
                                            this.SendNotice("You are not allowed to join parties when you are in DUEL mode!");
                                            continue;
                                        }
                                    }

                                    // Register
                                    this.party = true;
                                }
                                #endregion

                                #region CTF level restriction
                                // CTF Level v2
                                if (_pck.Opcode == 0x74B2)
                                {
                                    // ANTI BOT JOIN
                                    if(FilterMain.bot_list.Contains(this.user_id) && (!FilterMain.BOT_CTF))
                                    {
                                        this.SendNotice(FilterMain.BOT_NOTICE_CTF);
                                        continue;
                                    }

                                    // REQUIRED LEVEL
                                    if (FilterMain.CTF_level > 0)
                                    {
                                        // GET CHAR LEVEL
                                        //SqlDataReader reader = sqlCon.Return("[" + FilterMain.SUP_DB + "].[dbo].[_MainFunctions]", new SqlParameter("@Charname", Program.Plis(this.sql_charname)), new SqlParameter("@Action", 2));
                                        //reader.Read();

                                        // Check level
                                        if (this.CurLevel < FilterMain.CTF_level)
                                        {
                                            // New shit
                                            string message = FilterMain.CTF_LEVEL.Replace("{level}", FilterMain.CTF_level.ToString());

                                            // Message handle
                                            if (FilterMain.MESSAGE_HANDLE == "notice")
                                            {
                                                this.SendNotice(message);
                                            }
                                            else if (FilterMain.MESSAGE_HANDLE == "private")
                                            {
                                                this.SendNotice(message);
                                            }

                                            // Close reader
                                            //reader.Close();

                                            // Continue
                                            continue;
                                        }

                                        // Close SQL
                                        //reader.Close();
                                    }

                                    // Register, clear shit.
                                    if(!FilterMain.ctf_called)
                                    {
                                        try {
                                            FilterMain.ctf_called = true;
                                            FilterMain.CTF_TIMER = new Timer(new TimerCallback(Program.EventChars), null, 0, 2100000);
                                        }
                                        catch
                                        {
                                            Console.WriteLine("Error starting timer, top (:");
                                        }
                                    }

                                    // Blocked skills inside arena.
                                    if(!FilterMain.event_chars.Contains(this.charname))
                                    {
                                        FilterMain.event_chars.Add(this.charname);
                                    }
                                }
                                #endregion

                                #region Arena level restriction
                                if (_pck.Opcode == 0x74D3)
                                {
                                    // ANTI BOT JOIN
                                    if (FilterMain.bot_list.Contains(this.user_id) && (!FilterMain.BOT_ARENA))
                                    {
                                        this.SendNotice(FilterMain.BOT_NOTICE_ARENA);
                                        continue;
                                    }

                                    // Check stuff
                                    if (FilterMain.ARENA_level > 0)
                                    {
                                        // GET CHAR LEVEL
                                        //SqlDataReader reader = sqlCon.Return("[" + FilterMain.SUP_DB + "].[dbo].[_MainFunctions]", new SqlParameter("@Charname", Program.Plis(this.sql_charname)), new SqlParameter("@Action", 2));
                                        //reader.Read();

                                        // Check level
                                        if (this.CurLevel < FilterMain.ARENA_level)
                                        {
                                            // New shit
                                            string message = FilterMain.ARENA_LEVEL.Replace("{level}", FilterMain.ARENA_level.ToString());

                                            // Message handle
                                            if (FilterMain.MESSAGE_HANDLE == "notice")
                                            {
                                                this.SendNotice(message);
                                            }
                                            else if (FilterMain.MESSAGE_HANDLE == "private")
                                            {
                                                this.SendNotice(message);
                                            }

                                            // Close reader
                                            //reader.Close();

                                            // Continue
                                            continue;
                                        }

                                        // Close SQL
                                        //reader.Close();
                                    }

                                    // Register, clear shit.
                                    if (!FilterMain.arena_called)
                                    {
                                        try
                                        {
                                            // Avoid duplicates
                                            if (FilterMain.ARENA_TIMER == null)
                                            {
                                                FilterMain.arena_called = true;
                                                FilterMain.ARENA_TIMER = new Timer(new TimerCallback(Program.EventChars), null, 0, 2100000);
                                            }
                                        }
                                        catch
                                        {
                                            Console.WriteLine("Error starting timer, top (:");
                                        }
                                    }

                                    // Blocked skills inside arena.
                                    if (!FilterMain.event_chars.Contains(this.charname))
                                    {
                                        FilterMain.event_chars.Add(this.charname);
                                    }
                                }
                                #endregion

                                #region CLIENT HANDLE: Scrolls etc...
                                // READ CLIENT_PLAYER_HANDLE
                                if (_pck.Opcode == 0x704c && !(FilterMain.USER_ID.Contains(this.user_id)))
                                {
                                    uint num = _pck.ReadUInt8();
                                    uint value = _pck.ReadUInt16();

                                    /*
                                    0D - InventorySlot
                                    EC - PotionType (EC: Normal | ED: ItemMall)
                                    09 - PotionID (08: HP | 09: ? | 10: MP)
                                    */
                                    //Console.WriteLine(value);

                                    // REVERSE
                                    if ((value == 0x19ED) || (value == 0x19EC))
                                    {
                                        // Block reverse scrolls during job
                                        if (FilterMain.job_reverse && (FilterMain.DB))
                                        {
                                            try {
                                                SqlDataReader reader = sqlCon.Return("[" + FilterMain.SUP_DB + "].[dbo].[_MainFunctions]", new SqlParameter("@Charname", Program.Plis(this.sql_charname)), new SqlParameter("@Action", 1));
                                                reader.Read();

                                                if (reader.GetInt32(0) == 1)
                                                {
                                                    if (FilterMain.MESSAGE_HANDLE == "notice")
                                                    {
                                                        this.SendNotice(FilterMain.JOB_NOTICE_REVERSE);
                                                    }
                                                    else if (FilterMain.MESSAGE_HANDLE == "private")
                                                    {
                                                        this.SendPM(FilterMain.JOB_NOTICE_REVERSE);
                                                    }

                                                    // Close reader
                                                    reader.Close();

                                                    // Continue
                                                    continue;
                                                }

                                                // Close reader
                                                reader.Close();
                                            }
                                            catch
                                            {
                                                continue;
                                            }
                                        }

                                        // Reverse delay usage.
                                        if (FilterMain.Reversedelay > 0)
                                        {
                                            try
                                            {
                                                int gecensaniye = Convert.ToInt32(DateTime.Now.Subtract(this.reverse_time).TotalSeconds);
                                                if (gecensaniye < FilterMain.Reversedelay)
                                                {
                                                    // New shit
                                                    string message = FilterMain.REVERSE_DELAY.Replace("{time}", (FilterMain.Reversedelay - gecensaniye).ToString());

                                                    // Message handle
                                                    if (FilterMain.MESSAGE_HANDLE == "notice")
                                                    {
                                                        this.SendNotice(message);
                                                    }
                                                    else if (FilterMain.MESSAGE_HANDLE == "private")
                                                    {
                                                        this.SendPM(message);
                                                    }
                                                    continue;
                                                }
                                            }

                                            catch { }
                                            this.reverse_time = DateTime.Now;
                                        }
                                    }

                                    // RESCURRENT
                                    if ((value == 0x36ED) || (value == 0x36EC))
                                    {
                                        // Block RES in fortress
                                        if(this.AtFortress && FilterMain.FORTRESS_ANTI_RES_SCROLL)
                                        {
                                            this.SendNotice(FilterMain.SKILL_NOTICE3);
                                            continue;
                                        }

                                        // Block RES when in JOB mode
                                        if (FilterMain.job_res && (FilterMain.DB))
                                        {
                                            try
                                            {
                                                SqlDataReader reader = sqlCon.Return("[" + FilterMain.SUP_DB + "].[dbo].[_MainFunctions]", new SqlParameter("@Charname", Program.Plis(this.sql_charname)), new SqlParameter("@Action", 1));
                                                reader.Read();
                                                if (reader.GetInt32(0) == 1)
                                                {
                                                    if (FilterMain.MESSAGE_HANDLE == "notice")
                                                    {
                                                        this.SendNotice(FilterMain.JOB_NOTICE_RESCURRENT);
                                                    }
                                                    else if (FilterMain.MESSAGE_HANDLE == "private")
                                                    {
                                                        this.SendPM(FilterMain.JOB_NOTICE_RESCURRENT);
                                                    }

                                                    // Close reader
                                                    reader.Close();

                                                    // Continue
                                                    continue;
                                                }

                                                // Close SQL
                                                reader.Close();
                                            }
                                            catch
                                            {
                                                continue;
                                            }
                                        }
                                    }

                                    // GLOBAL DELAY
                                    if ((value == 0x29ED) || (value == 0x29EC))
                                    {
                                        // GLOBAL check bad words
                                        if ((FilterMain.badwords.Count > 0))
                                        {
                                            // Get the message
                                            string message = _pck.ReadAscii().ToString().ToLower();

                                            // Check if the mutherfucker insult.
                                            if (badword(message))
                                            {
                                                if (FilterMain.MESSAGE_HANDLE == "notice")
                                                {
                                                    this.SendNotice(FilterMain.GLOBAL_MESSAGE);
                                                }
                                                else if (FilterMain.MESSAGE_HANDLE == "private")
                                                {
                                                    this.SendPM(FilterMain.GLOBAL_MESSAGE);
                                                }
                                                continue;
                                            }
                                        }

                                        // GLOBAL required level
                                        if (FilterMain.global_level > 0)
                                        {
                                            // OLD CODING
                                            //SqlDataReader reader = sqlCon.Return("[" + FilterMain.SUP_DB + "].[dbo].[_MainFunctions]", new SqlParameter("@Charname", Program.Plis(this.sql_charname)), new SqlParameter("@Action", 2));
                                            //reader.Read();

                                            // Check level
                                            if (this.CurLevel < FilterMain.global_level)
                                            {
                                                // New shit
                                                string message = FilterMain.GLOBAL_LEVEL.Replace("{level}", FilterMain.global_level.ToString());

                                                // Message handle
                                                if (FilterMain.MESSAGE_HANDLE == "notice")
                                                {
                                                    this.SendNotice(message);
                                                }
                                                else if (FilterMain.MESSAGE_HANDLE == "private")
                                                {
                                                    this.SendNotice(message);
                                                }

                                                // Close reader
                                                //reader.Close();

                                                // Continue
                                                continue;
                                            }

                                            // Close SQL
                                            //reader.Close();
                                        }

                                        // GLOBAL DELAY
                                        if (FilterMain.Globaldelay > 0)
                                        {
                                            try
                                            {
                                                int gecensaniye = Convert.ToInt32((DateTime.Now.Subtract(this.global_time)).TotalSeconds);
                                                if (gecensaniye < FilterMain.Globaldelay)
                                                {
                                                    // New shit
                                                    string message = FilterMain.GLOBAL_DELAY.Replace("{time}", (FilterMain.Globaldelay - gecensaniye).ToString());

                                                    // Message handle
                                                    if (FilterMain.MESSAGE_HANDLE == "notice")
                                                    {
                                                        this.SendNotice(message);
                                                    }
                                                    else if (FilterMain.MESSAGE_HANDLE == "private")
                                                    {
                                                        this.SendPM(message);
                                                    }
                                                    continue;
                                                }
                                            }

                                            catch { }
                                            this.global_time = DateTime.Now;
                                        }
                                    }
                                }
                                #endregion

                                #region Zerk delay
                                if(_pck.Opcode == 0x70a7 && !(FilterMain.USER_ID.Contains(this.user_id)))
                                {
                                    // Invisible exploit fix
                                    if(_pck.ReadUInt8() != 1)
                                    {
                                        // Exploit
                                        if (FilterMain.ERROR_LOG.Equals("exploit") || FilterMain.ERROR_LOG.Equals("all"))
                                        {
                                            // Inform?
                                            Console.ForegroundColor = ConsoleColor.DarkYellow;
                                            Console.WriteLine("[" + DateTime.UtcNow + "][" + module_name + "] IP:{" + this.ip + "} User:{" + this.user_id + "} Reason:{INVISIBLE EXPLOIT} Bytes:{" + this.length + "} Packet_c{" + this.packetCount + "}");
                                            Console.ResetColor();
                                        }

                                        // Warn player
                                        this.SendNotice(FilterMain.INVISIBLE_EXPLOIT);
                                        continue;
                                    }

                                    // Zerk delay
                                    if (FilterMain.zerkdelay > 0)
                                    {
                                        try
                                        {
                                            int gecensaniye = Convert.ToInt32((DateTime.Now.Subtract(this.lastzerktime)).TotalSeconds);
                                            if (gecensaniye < FilterMain.zerkdelay)
                                            {
                                                // New shit
                                                string message = FilterMain.ZERK_DELAY.Replace("{time}", (FilterMain.zerkdelay - gecensaniye).ToString());

                                                // Message handle
                                                if (FilterMain.MESSAGE_HANDLE == "notice")
                                                {
                                                    this.SendNotice(message);
                                                }
                                                else if (FilterMain.MESSAGE_HANDLE == "private")
                                                {
                                                    this.SendPM(message);
                                                }
                                                continue;
                                            }
                                        }
                                        catch { }
                                        this.lastzerktime = DateTime.Now;
                                    }
                                }
                                #endregion

                                #region Exit/Restart button
                                // Disable (RESTART) Client
                                if (_pck.Opcode == 0x7005 && !(FilterMain.USER_ID.Contains(this.user_id)))
                                {
                                    // Read the action
                                    uint num = _pck.ReadUInt8();

                                    switch (num)
                                    {
                                        // Restart button
                                        case 2:
                                            {
                                                // Disable restart button
                                                if (FilterMain.restart_button)
                                                {
                                                    // Restart disabled
                                                    if (FilterMain.MESSAGE_HANDLE == "notice")
                                                    {
                                                        this.SendNotice(FilterMain.RESTART_BUTTON);
                                                    }
                                                    else if (FilterMain.MESSAGE_HANDLE == "private")
                                                    {
                                                        this.SendPM(FilterMain.RESTART_BUTTON);
                                                    }
                                                    continue;
                                                }

                                                // Restart button delay
                                                if(FilterMain.Restartdelay > 0)
                                                {
                                                    try
                                                    {
                                                        int gecensaniye = Convert.ToInt32((DateTime.Now.Subtract(this.lastrestarttime)).TotalSeconds);
                                                        if (gecensaniye < FilterMain.Restartdelay)
                                                        {
                                                            // New shit
                                                            string message = FilterMain.RESTART_DELAY.Replace("{time}", (FilterMain.Restartdelay - gecensaniye).ToString());

                                                            // Message handle
                                                            if (FilterMain.MESSAGE_HANDLE == "notice")
                                                            {
                                                                this.SendNotice(message);
                                                            }
                                                            else if (FilterMain.MESSAGE_HANDLE == "private")
                                                            {
                                                                this.SendPM(message);
                                                            }
                                                            continue;
                                                        }
                                                    }
                                                    catch { }
                                                    this.lastrestarttime = DateTime.Now;
                                                }
                                            }
                                            break;


                                        // Exit button
                                        case 1:
                                            {
                                                // Restart button delay
                                                if (FilterMain.Logoutdelay > 0)
                                                {
                                                    try
                                                    {
                                                        int gecensaniye = Convert.ToInt32((DateTime.Now.Subtract(this.lastlogouttime)).TotalSeconds);
                                                        if (gecensaniye < FilterMain.Logoutdelay)
                                                        {
                                                            // New shit
                                                            string message = FilterMain.LOGOUT_DELAY.Replace("{time}", (FilterMain.Logoutdelay - gecensaniye).ToString());

                                                            // Message handle
                                                            if (FilterMain.MESSAGE_HANDLE == "notice")
                                                            {
                                                                this.SendNotice(message);
                                                            }
                                                            else if (FilterMain.MESSAGE_HANDLE == "private")
                                                            {
                                                                this.SendPM(message);
                                                            }
                                                            continue;
                                                        }
                                                    }
                                                    catch { }
                                                    this.lastlogouttime = DateTime.Now;
                                                }
                                            }
                                            break;
                                    }
                                }
                                #endregion

                                #region Alchemy stone packet
                                if(_pck.Opcode == 0x7151 && (!FilterMain.BOT_ALCHEMY_STONE))
                                {
                                    // Anti bot alchemy
                                    if (FilterMain.bot_list.Contains(this.user_id))
                                    {
                                        this.SendNotice(FilterMain.BOT_NOTICE_ALCHEMY);
                                        continue;
                                    }
                                }
                                #endregion

                                #region Alchemy packet
                                // Alchemy packet
                                if (_pck.Opcode == 0x7150)
                                {
                                    // Anti bot alchemy
                                    if(FilterMain.bot_list.Contains(this.user_id) && (!FilterMain.BOT_ALCHEMY_ELIXIR))
                                    {
                                        this.SendNotice(FilterMain.BOT_NOTICE_FUSE);
                                        continue;
                                    }

                                    // Plus limitation
                                    if ((_pck.ReadUInt8() == 2) && (_pck.ReadUInt8() == 3) && (FilterMain.DB) && (FilterMain.plus_limit) > 0)
                                    {
                                        // DERP
                                        _pck.ReadUInt8();

                                        // Item slot
                                        int slot = _pck.ReadUInt8();

                                        try
                                        {
                                            // SQL
                                            SqlDataReader reader = sqlCon.Return("[" + FilterMain.SUP_DB + "].[dbo].[_MainFunctions]", new SqlParameter("@Charname", Program.Plis(this.sql_charname)), new SqlParameter("@Action", 3), new SqlParameter("@Slot", slot));
                                            reader.Read();

                                            // Plus
                                            int plus = reader.GetInt32(0);

                                            // PLUS LIMIT
                                            if (plus >= FilterMain.plus_limit)
                                            {
                                                if (FilterMain.MESSAGE_HANDLE == "notice")
                                                {
                                                    this.SendNotice(FilterMain.PLUS_LIMIT);
                                                }
                                                else if (FilterMain.MESSAGE_HANDLE == "private")
                                                {
                                                    this.SendPM(FilterMain.PLUS_LIMIT);
                                                }

                                                // Close reader
                                                reader.Close();

                                                // Continue
                                                continue;
                                            }

                                            // Close SQL
                                            reader.Close();
                                        }
                                        catch
                                        {
                                            continue;
                                        }
                                    }
                                }
                                #endregion

                                #region Avatar blues
                                if(_pck.Opcode == 0x34A9)
                                {
                                    // Avatar blue name
                                    string avatar_blue = _pck.ReadAscii().ToString().ToLower();

                                    // Disable BOT avatar blues
                                    if(FilterMain.bot_list.Contains(this.user_id) && (!FilterMain.BOT_AVATAR_BLUES))
                                    {
                                        this.SendNotice(FilterMain.BOT_NOTICE_AVATAR_BLUES);
                                        continue;
                                    }

                                    // Disable avatar blues
                                    if (FilterMain.avatar_blues)
                                    {
                                        this.SendNotice(FilterMain.AVATAR_BLUES);
                                        continue;
                                    }

                                    // Disable avatar exploit
                                    if(!avatar_blue.Contains("avatar"))
                                    {
                                        // Exploit
                                        if (FilterMain.ERROR_LOG.Equals("exploit") || FilterMain.ERROR_LOG.Equals("all"))
                                        {
                                            // Inform?
                                            Console.ForegroundColor = ConsoleColor.DarkYellow;
                                            Console.WriteLine("[" + DateTime.UtcNow + "][" + module_name + "] IP:{" + this.ip + "} User:{" + this.user_id + "} Reason:{AVATAR EXPLOIT} Bytes:{" + this.length + "} Packet_c{" + this.packetCount + "}");
                                            Console.ResetColor();
                                        }
                                        // Warn player
                                        this.SendNotice(FilterMain.AVATAR_EXPLOIT);
                                        continue;
                                    }
                                }
                                #endregion

                                #region Welcome message
                                // Send welcome message
                                if (_pck.Opcode == 0x3012 && (!this.startnotice) && (FilterMain.WelcomeMsg))
                                {
                                    // Disconnect botters.
                                    if (FilterMain.bot_list.Contains(this.user_id) && (!FilterMain.BOT_ALLOW))
                                    {
                                        // Inform
                                        this.SendNotice(FilterMain.BOT_NOTICE_DISCONNECT);

                                        // Disconnect
                                        this.DisconnectModuleSocket();
                                        return;
                                    }

                                    // Welcome message
                                    string welcomeText = FilterMain.WelcomeText;

                                    // PLAYER NAME
                                    if (welcomeText.Contains("{playername}"))
                                    {
                                        welcomeText = welcomeText.Replace("{playername}", Program.Plis(this.charname));
                                    }

                                    // SERVER NAME
                                    if (welcomeText.Contains("{servername}"))
                                    {
                                        welcomeText = welcomeText.Replace("{servername}", FilterMain.ServerName);
                                    }

                                    // USER NAME
                                    if (welcomeText.Contains("{username}"))
                                    {
                                        welcomeText = welcomeText.Replace("{username}", Program.Plis(this.user_id));
                                    }

                                    // Send Client notice
                                    if (welcomeText.Length > 3 && welcomeText != string.Empty)
                                    {
                                        if (FilterMain.MESSAGE_HANDLE == "notice")
                                        {
                                            this.SendNotice(welcomeText);
                                        }
                                        else if (FilterMain.MESSAGE_HANDLE == "private")
                                        {
                                            this.SendPM(welcomeText);
                                        }
                                    }

                                    // SET START NOTICE TO ENABLE TO(DIS ALLOW SPAM)
                                    this.startnotice = true;
                                }
                                #endregion

                                #region CONSIGMENT_EPIC_SRO_FILTER
                                if(_pck.Opcode == 0x7508 && (FilterMain.EPIC_ITEM))
                                {
                                    byte Items = _pck.ReadUInt8(); // Number of items.

                                    // Lock items
                                    if(Items > 1)
                                    {
                                        this.SendNotice("Please register one item at the time, please!");
                                        continue;
                                    }

                                    // Number of items they register.
                                    if (Items == 1)
                                    {
                                        byte slot = _pck.ReadUInt8();

                                        try
                                        {
                                            // SQL part.
                                            SqlDataReader reader = sqlCon.Return("[" + FilterMain.SUP_DB + "].[dbo].[_EPICSRO]", new SqlParameter("@Charname", Program.Plis(this.sql_charname)), new SqlParameter("@slot", slot));
                                            reader.Read();

                                            // Block, send notice.
                                            if (reader.GetInt32(0) == 1)
                                            {
                                                this.SendNotice("You are not allowed to sell items with Advanced elixir on!");
                                                continue; // Derp
                                            }

                                            reader.Close();
                                        }
                                        catch
                                        {
                                            continue; // In case of fuck up. (:
                                        }
                                    }
                                }
                                #endregion

                                #region Stall Filter
                                if (_pck.Opcode == 0x70BA && !(FilterMain.USER_ID.Contains(this.user_id)))
                                {
                                    #region EPIC_SRO_ITEMS
                                    // Add item
                                    if (_pck.ReadUInt8() == 2 && (FilterMain.EPIC_ITEM))
                                    {
                                        // Get slots
                                        _pck.ReadUInt8(); // Stall slot :D
                                        byte slot = _pck.ReadUInt8();

                                        try
                                        {
                                            // SQL part.
                                            SqlDataReader reader = sqlCon.Return("[" + FilterMain.SUP_DB + "].[dbo].[_EPICSRO]", new SqlParameter("@Charname", Program.Plis(this.sql_charname)), new SqlParameter("@slot", slot));
                                            reader.Read();

                                            // Block, send notice.
                                            if (reader.GetInt32(0) == 1)
                                            {
                                                this.SendNotice("You are not allowed to sell items with Advanced elixir on!");
                                                continue; // Derp
                                            }

                                            reader.Close();
                                        }
                                        catch
                                        {
                                            continue; // In case of fuck up. (:
                                        }
                                    }
                                    #endregion

                                    // Stall filter
                                    if ((FilterMain.badwords.Count > 0))
                                    {
                                        // Get the message
                                        string message = _pck.ReadAscii().ToString().ToLower();

                                        // Check if the mutherfucker insult.
                                        if (badword(message))
                                        {
                                            if (FilterMain.MESSAGE_HANDLE == "notice")
                                            {
                                                this.SendNotice(FilterMain.STALL_FILTER);
                                            }
                                            else if (FilterMain.MESSAGE_HANDLE == "private")
                                            {
                                                this.SendPM(FilterMain.STALL_FILTER);
                                            }
                                            continue;
                                        }
                                    }
                                }
                                #endregion

                                #region Client PVP // BOT 
                                if (_pck.Opcode == 0x7516)
                                {
                                    // PVP DUEL
                                    if(FilterMain.PVP_DUEL && (FilterMain.PVP_DISABLE_PARTY) && (this.party))
                                    {
                                        // Cape number
                                        byte CapeNum = _pck.ReadUInt8();

                                        // Make sure that is Duel cape we talking bout.
                                        if (CapeNum > 0 && CapeNum < 5)
                                        {
                                            // Send notice
                                            this.SendNotice("You cannot enter DUEL mode when you are in a party!");

                                            // Prevent
                                            continue;
                                        }
                                    }

                                    // BOT anti-exchange.
                                    if (FilterMain.bot_list.Contains(this.user_id) && (!FilterMain.BOT_PVP))
                                    {
                                        // Message handle.
                                        if (FilterMain.MESSAGE_HANDLE == "notice")
                                        {
                                            this.SendNotice(FilterMain.BOT_NOTICE_PVP);
                                        }
                                        else if (FilterMain.MESSAGE_HANDLE == "private")
                                        {
                                            this.SendPM(FilterMain.BOT_NOTICE_PVP);
                                        }

                                        // Prevent
                                        continue;
                                    }
                                }
                                #endregion

                                #region Stall Delay
                                // Stall delay
                                if (_pck.Opcode == 0x70B1 && !(FilterMain.USER_ID.Contains(this.user_id)))
                                {
                                    // BOT anti-stall.
                                    if (FilterMain.bot_list.Contains(this.user_id) && (!FilterMain.BOT_STALL))
                                    {
                                        // Message handle.
                                        if (FilterMain.MESSAGE_HANDLE == "notice")
                                        {
                                            this.SendNotice(FilterMain.BOT_NOTICE_STALL_CREATE);
                                        }
                                        else if (FilterMain.MESSAGE_HANDLE == "private")
                                        {
                                            this.SendPM(FilterMain.BOT_NOTICE_STALL_CREATE);
                                        }

                                        // Prevent
                                        continue;
                                    }

                                    // Stall level limit
                                    if (FilterMain.stall_level > 0)
                                    {
                                        // Check level
                                        if (this.CurLevel < FilterMain.stall_level)
                                        {
                                            // New shit
                                            string message = FilterMain.STALL_LEVEL.Replace("{level}", FilterMain.stall_level.ToString());

                                            // Message handle
                                            if (FilterMain.MESSAGE_HANDLE == "notice")
                                            {
                                                this.SendNotice(message);
                                            }
                                            else if (FilterMain.MESSAGE_HANDLE == "private")
                                            {
                                                this.SendPM(message);
                                            }
                                            continue;
                                        }
                                    }
                                    
                                    // Stall exploit
                                    if(this.StallBlock && FilterMain.FILES.Contains("vsro"))
                                    {
                                        // Message handle
                                        if (FilterMain.MESSAGE_HANDLE == "notice")
                                        {
                                            this.SendNotice(FilterMain.STALL_EXPLOIT);
                                        }
                                        else if (FilterMain.MESSAGE_HANDLE == "private")
                                        {
                                            this.SendPM(FilterMain.STALL_EXPLOIT);
                                        }
                                        continue;
                                    }

                                    // Stall filter
                                    if ((FilterMain.badwords.Count > 0))
                                    {
                                        // Get the message
                                        string message = _pck.ReadAscii().ToString().ToLower();

                                        // Check if the mutherfucker insult.
                                        if (badword(message))
                                        {
                                            if (FilterMain.MESSAGE_HANDLE == "notice")
                                            {
                                                this.SendNotice(FilterMain.STALL_FILTER);
                                            }
                                            else if (FilterMain.MESSAGE_HANDLE == "private")
                                            {
                                                this.SendPM(FilterMain.STALL_FILTER);
                                            }
                                            continue;
                                        }
                                    }

                                    // Stall delay
                                    if (FilterMain.Stalldelay > 0)
                                    {
                                        try
                                        {
                                            int gecensaniye = Convert.ToInt32((DateTime.Now.Subtract(this.laststalltime)).TotalSeconds);
                                            if (gecensaniye < FilterMain.Stalldelay)
                                            {
                                                // New shit
                                                string message = FilterMain.STALL_DELAY.Replace("{time}", (FilterMain.Stalldelay - gecensaniye).ToString());

                                                // Message handle
                                                if (FilterMain.MESSAGE_HANDLE == "notice")
                                                {
                                                    this.SendNotice(message);
                                                }
                                                else if (FilterMain.MESSAGE_HANDLE == "private")
                                                {
                                                    this.SendPM(message);
                                                }
                                                continue;
                                            }
                                        }

                                        catch { }
                                        this.laststalltime = DateTime.Now;
                                    }
                                }
                                #endregion

                                #region Exchange delay
                                // Exchange packet
                                if (_pck.Opcode == 0x7081 && (this.length == 4) && !(FilterMain.USER_ID.Contains(this.user_id)))
                                {
                                    // Stall block
                                    this.StallBlock = true;

                                    // BOT anti-exchange.
                                    if (FilterMain.bot_list.Contains(this.user_id) && (!FilterMain.BOT_EXCHANGE))
                                    {
                                        // Message handle.
                                        if (FilterMain.MESSAGE_HANDLE == "notice")
                                        {
                                            this.SendNotice(FilterMain.BOT_NOTICE_EXCHANGE);
                                        }
                                        else if (FilterMain.MESSAGE_HANDLE == "private")
                                        {
                                            this.SendPM(FilterMain.BOT_NOTICE_EXCHANGE);
                                        }

                                        // Prevent
                                        continue;
                                    }

                                    // Job exchange
                                    if ((FilterMain.job_exchange) && (FilterMain.DB))
                                    {
                                        try
                                        {
                                            SqlDataReader reader = sqlCon.Return("[" + FilterMain.SUP_DB + "].[dbo].[_MainFunctions]", new SqlParameter("@Charname", Program.Plis(this.sql_charname)), new SqlParameter("@Action", 1));
                                            reader.Read();
                                            if (reader.GetInt32(0) == 1)
                                            {
                                                if (FilterMain.MESSAGE_HANDLE == "notice")
                                                {
                                                    this.SendNotice(FilterMain.JOB_NOTICE_EXCHANGE);
                                                }
                                                else if (FilterMain.MESSAGE_HANDLE == "private")
                                                {
                                                    this.SendPM(FilterMain.JOB_NOTICE_EXCHANGE);
                                                }

                                                // Close reader
                                                reader.Close();

                                                // Continue
                                                continue;
                                            }

                                            // Close SQL
                                            reader.Close();
                                        }
                                        catch {
                                            continue;
                                        }
                                    }

                                    // Check if Exchange delay greater then 0.
                                    if (FilterMain.Exchangedelay > 0)
                                    {
                                        try
                                        {
                                            _pck.ReadUInt8();
                                            int okuy = (int)_pck.ReadUInt8();
                                            if (okuy >= 1)
                                            {
                                                int gecensaniye = Convert.ToInt32((DateTime.Now.Subtract(this.lastexchangetime)).TotalSeconds);
                                                if (gecensaniye < FilterMain.Exchangedelay)
                                                {
                                                    // New shit
                                                    string message = FilterMain.EXCHANGE_DELAY.Replace("{time}", (FilterMain.Exchangedelay - gecensaniye).ToString());

                                                    // Message handle
                                                    if (FilterMain.MESSAGE_HANDLE == "notice")
                                                    {
                                                        this.SendNotice(message);
                                                    }
                                                    else if (FilterMain.MESSAGE_HANDLE == "private")
                                                    {
                                                        this.SendPM(message);
                                                    }
                                                    continue;
                                                }
                                            }
                                        }
                                        catch { }
                                        this.lastexchangetime = DateTime.Now;
                                    }
                                }
                                #endregion

                                #region Exchange accept
                                // Exchange accept
                                if (_pck.Opcode == 0x7082 || _pck.Opcode == 0x7083 || _pck.Opcode == 0x7084 && !(FilterMain.USER_ID.Contains(this.user_id)))
                                {
                                    // Stall exploit
                                    this.StallBlock = true;

                                    // Block job exchange
                                    if ((FilterMain.job_exchange) && (FilterMain.DB))
                                    {
                                        try
                                        {
                                            SqlDataReader reader = sqlCon.Return("[" + FilterMain.SUP_DB + "].[dbo].[_MainFunctions]", new SqlParameter("@Charname", Program.Plis(this.sql_charname)), new SqlParameter("@Action", 1));
                                            reader.Read();
                                            if (reader.GetInt32(0) == 1)
                                            {
                                                if (FilterMain.MESSAGE_HANDLE == "notice")
                                                {
                                                    this.SendNotice(FilterMain.JOB_NOTICE_EXCHANGE);
                                                }
                                                else if (FilterMain.MESSAGE_HANDLE == "private")
                                                {
                                                    this.SendPM(FilterMain.JOB_NOTICE_EXCHANGE);
                                                }

                                                // Close reader
                                                reader.Close();

                                                // Continue
                                                continue;
                                            }

                                            // Close SQL
                                            reader.Close();
                                        }
                                        catch
                                        {
                                            continue;
                                        }
                                    }
                                }
                                #endregion

                                #region AFK checking method
                                /*if (_pck.Opcode == 0x2002 || _pck.Opcode == 0x7024 && (FilterMain.SilkEnable))
                                {
                                    // Register afk
                                    this.IsAfk = true;
                                }
                                else if (FilterMain.SilkEnable)
                                {
                                    // Register AFK
                                    this.IsAfk = false;
                                }*/
                                #endregion

                                #region BlockOPCODES and shit
                                // BLOCK OPCODES
                                if (FilterMain.BAD_Opcodes.ContainsKey(Convert.ToUInt16(_pck.Opcode)))
                                {
                                    // Ban exploiters.
                                    if (FilterMain.EXPLOIT_METHOD == "ban")
                                    {
                                        if (!FilterMain.ban_list.Contains(this.ip))
                                        {
                                            // Add ban
                                            FilterMain.ban_list.Add(this.ip);

                                            try
                                            {
                                                // File exist, write to it.
                                                System.IO.StreamWriter file = new System.IO.StreamWriter("config/blacklist.txt", true);
                                                if (this.ip.Length > 0)
                                                {
                                                    file.WriteLine(this.ip + "\n");
                                                }
                                                file.Close();

                                                // Ban log(For checking random bans)
                                                System.IO.StreamWriter banlog = new System.IO.StreamWriter("logs/banlog.txt", true);
                                                banlog.WriteLine("[" + DateTime.UtcNow + "] {" + module_name + "} Banned {" + this.ip + "} Reason: {EXPLOITING}\n");
                                                banlog.Close();
                                            }
                                            catch { }

                                            // Inform?
                                            Console.ForegroundColor = ConsoleColor.DarkMagenta;
                                            Console.WriteLine("[" + DateTime.UtcNow + "][" + module_name + "] IP:{" + this.ip + "} User:{" + this.user_id + "} Opcode:{0x" + _pck.Opcode.ToString("X") + "} Bytes:{" + this.length + "} Packet_c{" + this.packetCount + "}");
                                            Console.ResetColor();
                                        }
                                    }

                                    // Disconnect
                                    this.DisconnectModuleSocket();
                                    return;
                                }
                                #endregion

                                #region Login packet shit
                                // New anti-exploit feature.
                                if (_pck.Opcode == 0x6103)
                                {
                                    // Add
                                    this.can_connect_yes = 1;
                                    this.information = true;

                                    // Some shit
                                    UInt32 uint32_1 = _pck.ReadUInt32();
                                    this.user_id = _pck.ReadAscii().ToLower();
                                    this.user_pw = _pck.ReadAscii();
                                    byte locale = _pck.ReadUInt8();
                                    uint ukn1 = _pck.ReadUInt32();
                                    uint ukn = _pck.ReadUInt16();

                                    // IP LIMIT
                                    if (FilterMain.IPLIMIT > 0)
                                    {
                                        FilterMain.ip_list.Add(this.ip);
                                    }

                                    // PC LIMIT
                                    if (FilterMain.PCLIMIT > 0 && !(FilterMain.LIMIT_BYPASS.Contains(this.user_id)))
                                    {
                                        try
                                        {
                                            if (FilterMain.hwid_user.ContainsKey(this.user_id))
                                            {
                                                FilterMain.hwid_list.Add(FilterMain.hwid_user[this.user_id]);
                                            } 
                                            else
                                            {
                                                Console.WriteLine(module_name + "Error adding PC limit, kicking user.");
                                                this.DisconnectModuleSocket();
                                                return;
                                            }
                                        }
                                        catch
                                        {
                                            Console.WriteLine(module_name + "Error adding PC limit, kicking user.");
                                            this.DisconnectModuleSocket();
                                            return;
                                        }
                                    }

                                    // IP LIMIT DOUBLE CHECK
                                    if (FilterMain.IPLIMIT > 0 && !(FilterMain.LIMIT_BYPASS.Contains(this.user_id)))
                                    {
                                        // Net CAFE ip
                                        if (FilterMain.CAFELIMIT > 0 && (FilterMain.cafe_list.Contains(this.ip)))
                                        {
                                            if (ip_count(this.ip) > FilterMain.CAFELIMIT)
                                            {
                                                // Disconnect
                                                this.DisconnectModuleSocket();
                                                return;
                                            }
                                        }
                                        else
                                        {
                                            // COUNT +1 BECAUSE ALWAYS 1 LESS
                                            if (ip_count(this.ip) > FilterMain.IPLIMIT)
                                            {
                                                // Disconnect
                                                this.DisconnectModuleSocket();
                                                return;
                                            }
                                        }
                                    }

                                    // PC LIMIT DOUBLE CHECK
                                    if (FilterMain.PCLIMIT > 0 && !(FilterMain.LIMIT_BYPASS.Contains(this.user_id)))
                                    {
                                        // Make sure it contains shit
                                        if (FilterMain.hwid_user.ContainsKey(this.user_id))
                                        {
                                            if(hwid_count(FilterMain.hwid_user[this.user_id]) > FilterMain.PCLIMIT)
                                            {
                                                // Disconnect
                                                this.DisconnectModuleSocket();
                                                return;
                                            }
                                        }
                                    }

                                    // BOT DETECTION SYSTEM ENABLED?
                                    if ((FilterMain.BOT_DETECTION))
                                    {
                                        // Anti bot stuff, nobody would understand.
                                        if (locale == 51)
                                        {
                                            Packet p = new Packet(0x6103, true);
                                            p.WriteUInt32(uint32_1);
                                            p.WriteAscii(this.user_id);
                                            p.WriteAscii(this.user_pw);
                                            p.WriteUInt8(22);
                                            p.WriteUInt32(ukn1);
                                            p.WriteUInt16(ukn);
                                            m_RemoteSecurity.Send(p);
                                            Send(true);
                                            continue;
                                        }
                                        else
                                        {
                                            // Bot list
                                            FilterMain.bot_list.Add(this.user_id);

                                            // BOT warning console message
                                            if (FilterMain.BOT_WARNING)
                                            {
                                                // SUPERMIKE error
                                                Console.ForegroundColor = ConsoleColor.DarkCyan;
                                                Console.WriteLine("[" + DateTime.UtcNow + "][" + module_name + "] IP:{" + this.ip + "} User:{" + this.user_id + "} Reason:{BOTTING} Bytes:{" + this.length + "} Packet_c{" + this.packetCount + "}");
                                                Console.ResetColor();
                                            }
                                        }
                                    }
                                }
                                #endregion

                                #region Ignore handshake 
                                // Ignore handshake
                                if (_pck.Opcode == 0x5000 || _pck.Opcode == 0x9000)
                                {
                                    Send(false);
                                    continue;
                                }
                                #endregion

                                #region Clear logs
                                // Clear logs
                                if (m_LastPackets.Count > 100)
                                {
                                    m_LastPackets.Clear();
                                }
                                #endregion

                                #region Big mike exploit protection
                                // Simple method, if not sending CLIENT_CONNECT fuck off.
                                if (this.can_connect_yes != 1)
                                {
                                    if (FilterMain.ERROR_LOG.Equals("all") || FilterMain.ERROR_LOG.Equals("exploit"))
                                    {
                                        Console.ForegroundColor = ConsoleColor.DarkYellow;
                                        Console.WriteLine("[" + DateTime.UtcNow + "][" + module_name + "] IP:{" + this.ip + "} User:{" + this.user_id + "} Opcode:{Exploiting} Bytes:{" + this.length + "} Packet_c{" + this.packetCount + "}");
                                        Console.ResetColor();
                                    }

                                    // Disconnect
                                    this.DisconnectModuleSocket();
                                    return;
                                }
                                #endregion

                                // No clue
                                Packet CopyOfPacket = _pck;
                                m_LastPackets.Enqueue(CopyOfPacket);

                                // Send packets
                                m_RemoteSecurity.Send(_pck);
                                Send(true);
                            }
                        }

                    }
                    else
                    {
                        try
                        {
                            // Abort connection
                            this.m_TransferPoolThread.Abort();
                        } catch { }

                        // Disconnect
                        this.DisconnectModuleSocket();
                        this.m_delDisconnect.Invoke(ref m_ClientSocket, m_HandlerType);
                        return;
                    }


                    this.DoRecvFromClient();
                }
                catch
                {
                    try
                    {
                        // Abort connection
                        this.m_TransferPoolThread.Abort();
                    } catch { }

                    // Disconnect
                    this.DisconnectModuleSocket();
                    this.m_delDisconnect.Invoke(ref m_ClientSocket, m_HandlerType);
                }
            }
        }
        void DoRecvFromServer()
        {
            try
            {
                this.m_ModuleSocket.BeginReceive(m_RemoteBuffer, 0, m_RemoteBuffer.Length,
                SocketFlags.None,
                new AsyncCallback(OnReceive_FromServer), null);
            }
            catch
            {
                try
                {
                    this.m_TransferPoolThread.Abort();
                }
                catch { }

                // Disconnect
                this.DisconnectModuleSocket();
                this.m_delDisconnect.Invoke(ref m_ClientSocket, m_HandlerType);
            }
        }

        // SEND NOTICE
        public void SendNotice(string Message)
        {
            Packet packet = new Packet(0x3026);
            //packet.WriteUInt8(0x07);
            packet.WriteUInt8((byte)7);
            packet.WriteAscii(FilterMain.FILTER_NAME + " " + Message);
            m_LocalSecurity.Send(packet);
            Send(false);
        }

        // Test
        public static void ByeBye(object e)
        {
        /*   
            // Clean the list
            FilterMain.ip_list.Clear();

            // Clean the net cafe list
            FilterMain.ip_cafe.Clear();

            // Clean user list
            bye2.Clear();

            try
            {
                // Connected users
                foreach (AgentContext current in clientlistgame)
                {
                    if (current.m_ClientSocket.Connected)
                    {
                        // IP limit fix
                        if (FilterMain.IPLIMIT > 0)
                        {
                            FilterMain.ip_list.Add(current.ip);
                        }

                        // Net cafe limit fix
                        if(FilterMain.CAFELIMIT > 0)
                        {
                            FilterMain.ip_cafe.Add(current.ip);
                        }

                        // Add username
                        bye2.Add(current.user_id);
                    }
                }

                // HWID limit fix
                if(FilterMain.hwid_system)
                {
                    foreach(string derp in FilterMain.user_list.Keys)
                    {
                        if(!bye2.Contains(derp))
                        {
                            // Inform
                            //Console.WriteLine(derp + " Was removed from hwid list #2.");

                            // Remove value
                            FilterMain.user_list.Remove(derp);
                        }
                    }
                }
            } catch { }

            // Check if using 2 agent
            if ((!FilterMain.AGENT_IP2.Equals("0")) && (FilterMain.AGENT_IP2 != String.Empty) && FilterMain.isPremium)
            {
                AgentContext2.ByeBye();
            }
            */
        }

        public static void WhosOnline()
        {
            try
            {
                foreach (AgentContext current in clientlistgame)
                {
                    if (current.m_ClientSocket.Connected)
                    {
                        Console.WriteLine("Method 1: " + current.user_id + " is online!");
                    }

                    if (current.m_ClientSocket.Connected && current.m_ModuleSocket.Connected)
                    {
                        Console.WriteLine("Method 2: " + current.user_id + " is online!");
                    }

                    // Debug message
                    Console.WriteLine(current.user_id + " debug message : " + current.m_ClientSocket.Connected);
                }
            }
            catch { }
        }

        // SEND NOTICE TO ONLINE
        public static void SendNoticeOnline(string Message)
        {
            try
            {
                foreach (AgentContext current in clientlistgame)
                {
                    if (current.m_ClientSocket.Connected)
                    {
                        Packet packet = new Packet(0x3026);
                        packet.WriteUInt8((byte)7);
                        packet.WriteAscii(FilterMain.FILTER_NAME + " " + Message);
                        current.m_LocalSecurity.Send(packet);
                        current.Send(false);
                    }
                }
            }
            catch { }
        }

        // Disconnect user
        public static void DisconnectUser(string search)
        {
            try {
                foreach (AgentContext current in clientlistgame)
                {
                    if (current.m_ClientSocket.Connected)
                    {
                        if (current.user_id.ToLower().Equals(search) || current.charname.ToLower().Equals(search))
                        {
                            // Send a notice
                            current.SendNotice(FilterMain.DISCONNECT_MESSAGE);

                            // Disconnect
                            current.m_ModuleSocket.Close();
                        }
                    }
                }
            } catch { }
        }

        // Ban user
        public static void BanUser(string search)
        {
            try {
                foreach (AgentContext current in clientlistgame)
                {
                    if (current.m_ClientSocket.Connected)
                    {
                        if (current.user_id.ToLower().Equals(search) || current.charname.ToLower().Equals(search))
                        {
                            // Add ban
                            FilterMain.ban_list.Add(current.ip);

                            try
                            {
                                // File exist, write to it.
                                System.IO.StreamWriter file = new System.IO.StreamWriter("config/blacklist.txt", true);
                                file.WriteLine(current.ip + "\n");
                                file.Close();

                                // Ban log(For checking random bans)
                                System.IO.StreamWriter banlog = new System.IO.StreamWriter("logs/banlog.txt", true);
                                banlog.WriteLine("[" + DateTime.UtcNow + "] {AGENT} Banned {" + current.ip + "} Reason: {BANNED BY COMMAND}\n");
                                banlog.Close();
                            }
                            catch { }

                            // Send a notice
                            current.SendNotice(FilterMain.BAN_MESSAGE);

                            // Disconnect
                            current.m_ModuleSocket.Close();
                        }
                    }
                }
            } catch { }
        }

        // NoticeToPlayer
        public static void NoticeToPlayer(string message, string charname)
        {
            try
            {
                foreach(AgentContext current in clientlistgame)
                {
                    if(current.charname.Equals(charname))
                    {
                        current.SendNotice(message);
                    }
                }
            }
            catch { }
        }

        // SEND CLIENT NOTICE
        public void SendClientNotice(string str)
        {
            Packet err = new Packet(0x300C);
            err.WriteUInt16(3100);
            err.WriteUInt8(1);
            err.WriteAscii(str);
            m_LocalSecurity.Send(err);

            //Инфа с боку
            Packet err_2 = new Packet(0x300C);
            err_2.WriteUInt16(3100);
            err_2.WriteUInt8(2);
            err_2.WriteAscii(str);
            m_LocalSecurity.Send(err_2);
        }

        // SEND PM
        public void SendPM(string strMessage)
        {
            Packet packet = new Packet(0x3026);
            //packet.WriteUInt8(2);
            packet.WriteUInt8((byte)2);
            packet.WriteAscii(FilterMain.FILTER_NAME);
            packet.WriteAscii(strMessage);
            m_LocalSecurity.Send(packet);
            Send(false);
        }

        void DoRecvFromClient()
        {
            try
            {
                this.m_ClientSocket.BeginReceive(m_LocalBuffer, 0, m_LocalBuffer.Length,
                SocketFlags.None,
                new AsyncCallback(OnReceive_FromClient), null);
            }
            catch
            {
                try
                {
                    this.m_TransferPoolThread.Abort();
                }
                catch { }
                this.DisconnectModuleSocket();
                this.m_delDisconnect.Invoke(ref m_ClientSocket, m_HandlerType);
            }
        }
    }
}