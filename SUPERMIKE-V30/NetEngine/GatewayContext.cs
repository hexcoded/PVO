using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Linq;
using Framework;
using System.Data.SqlClient;
using System.Text.RegularExpressions;

namespace ExploitFilter.NetEngine
{
    sealed class GatewayContext
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
        DateTime m_StartTime = DateTime.Now;

        // Module name
        string module_name = "GATEWAY";

        // Mike 2k16 pro<3
        int sent_id = 0;
        int sent_list = 0;
        int patch_sent = 0;

        // User information
        string user_id;
        string user_pw;

        // Packet
        int packetCount = 0;
        Timer t1;
        int length = 0;

        // Client list game
        public static List<GatewayContext> clientlistgame = new List<GatewayContext>();

        // Replace
        Regex regex = new Regex(@"[^a-zA-Z0-9|.~}!-&{)-_]", (RegexOptions)0);

        // User IP
        string ip = string.Empty;
        bool connect = true;

        // Locker :D
        public static object locker = new object();

        // Hwid recoded
        string hwid = null;

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

                    return count_hwid +1;
                }
                catch
                {
                    return 0;
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
                    return count_ip + 1;
                }
                catch
                {
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

        public GatewayContext(Socket ClientSocket, AsyncServer.delClientDisconnect delDisconnect)
        {
            this.m_delDisconnect = delDisconnect;
            this.m_ClientSocket = ClientSocket;
            this.m_HandlerType = AsyncServer.E_ServerType.GatewayServer;
            this.m_ModuleSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            // Gateway count
            FilterMain.gateway++;

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

                            try {
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
                        this.m_ModuleSocket.Connect(new IPEndPoint(IPAddress.Parse(FilterMain.REMOTE_GATEWAY), FilterMain.GATEWAY_LISTEN_PORT));
                    }
                }
                else
                {
                    if (this.connect)
                    {
                        this.m_ModuleSocket.Connect(new IPEndPoint(IPAddress.Parse(FilterMain.GATEWAY_IP), FilterMain.GATEWAY_LISTEN_PORT));
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

            // Client list
            clientlistgame.Add(this);

            // Mike pro :3
            this.sent_id = 0;
            this.sent_list = 0;
            this.patch_sent = 0;

            // HWID SYSTEM
            this.hwid = null;
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
                if (this.m_ModuleSocket != null)
                {
                    // Gateway count
                    if (FilterMain.gateway > 0)
                    {
                        FilterMain.gateway--;
                    }

                    // GLOBAL IP LIST / FLOOD IP LIST
                    FilterMain.flood_list.Remove(this.ip);

                    // Destroy timers
                    this.t1.Dispose();

                    // Client list
                    clientlistgame.Remove(this);

                    // DISCONNECT
                    this.m_ModuleSocket.Close();
                }

                // NULL
                this.m_ModuleSocket = null;

            } catch { }
        }

        void OnReceive_FromServer(IAsyncResult iar)
        {
            lock (m_Lock)
            {
                try
                {
                    int nReceived = m_ModuleSocket.EndReceive(iar);

                    if (nReceived != 0)
                    {
                        this.m_RemoteSecurity.Recv(m_RemoteBuffer, 0, nReceived);

                        List<Packet> RemotePackets = m_RemoteSecurity.TransferIncoming();

                        if (RemotePackets != null)
                        {
                            foreach (Packet _pck in RemotePackets)
                            {
                                #region Handshake
                                // Handshake
                                if (_pck.Opcode == 0x5000 || _pck.Opcode == 0x9000)
                                {
                                    Send(true);
                                    continue;
                                }
                                #endregion

                                #region Captcha remover
                                // Captcha remover, trololo!
                                if ((_pck.Opcode == 0x2322) && (FilterMain.Captcha_Remove))
                                {
                                    Packet captchapacket = new Packet(0x6323, false);
                                    captchapacket.WriteAscii(FilterMain.Captcha_Char);
                                    m_RemoteSecurity.Send(captchapacket);
                                    Send(true);
                                    continue;
                                }
                                #endregion

                                #region Login packet
                                // Login packet
                                if (_pck.Opcode == 0xA102)
                                {
                                    // Host
                                    string src_host;
                                    int src_port;
                                    byte res = _pck.ReadUInt8();

                                    // Derp
                                    if (res == 1)
                                    {
                                        uint id = _pck.ReadUInt32();
                                        src_host = _pck.ReadAscii();
                                        src_port = _pck.ReadUInt16();

                                        Packet new_pck = new Packet(0xA102, true);
                                        new_pck.WriteUInt8(res);
                                        new_pck.WriteUInt32(id);

                                        // Check if AGENT2 IP is in use
                                        if ((!FilterMain.AGENT_IP2.Equals("0")) && (FilterMain.AGENT_IP2 != String.Empty))
                                        {
                                            // AGENT ONE
                                            if (src_host == FilterMain.PROXY)
                                            {
                                                // AGENT 1
                                                new_pck.WriteAscii(FilterMain.PROXY);
                                                new_pck.WriteUInt16(FilterMain.FAKE_AGENT);
                                                new_pck.WriteUInt32((uint)0);
                                                new_pck.Lock();
                                                m_LocalSecurity.Send(new_pck);
                                                Send(false);
                                                continue;
                                            }
                                            else
                                            {
                                                if (FilterMain.PIONEER)
                                                {
                                                    // PIONEER SPECIAL
                                                    new_pck.WriteAscii(src_host);
                                                    new_pck.WriteUInt16(FilterMain.FAKE_AGENT2);
                                                    new_pck.WriteUInt32((uint)0);
                                                    new_pck.Lock();
                                                    m_LocalSecurity.Send(new_pck);
                                                    Send(false);
                                                    continue;
                                                }
                                                else
                                                {
                                                    // AGENT 2
                                                    new_pck.WriteAscii(FilterMain.PROXY);
                                                    new_pck.WriteUInt16(FilterMain.FAKE_AGENT2);
                                                    new_pck.WriteUInt32((uint)0);
                                                    new_pck.Lock();
                                                    m_LocalSecurity.Send(new_pck);
                                                    Send(false);
                                                    continue;
                                                }
                                            }
                                        }
                                        else
                                        {
                                            // AGENT 1
                                            new_pck.WriteAscii(FilterMain.PROXY);
                                            new_pck.WriteUInt16(FilterMain.FAKE_AGENT);
                                            new_pck.WriteUInt32((uint)0);
                                            new_pck.Lock();
                                            m_LocalSecurity.Send(new_pck);
                                            Send(false);
                                            continue;
                                        }

                                    }
                                }
                                #endregion

                                #region Online count
                                if (_pck.Opcode == 0xA101 && !(FilterMain.ENABLED))
                                {
                                    // Reset shits
                                    FilterMain.cur_players = 0;
                                    FilterMain.max_players = 0;

                                    // Timlock pl0x
                                    byte flag = 0x00;

                                    // Global lits0x
                                    flag = _pck.ReadUInt8();
                                    while (flag == 0x01)
                                    {
                                        // Read stuff
                                        _pck.ReadUInt8();
                                        _pck.ReadAscii();

                                        // New flag
                                        flag = _pck.ReadUInt8();
                                    }

                                    // Shard list
                                    flag = _pck.ReadUInt8();
                                    while (flag == 0x01)
                                    {
                                        _pck.ReadUInt16(); // Shard ID
                                        _pck.ReadAscii(); // Shard Name

                                        // Current players
                                        FilterMain.cur_players = (FilterMain.cur_players + _pck.ReadUInt16());

                                        // Max players
                                        FilterMain.max_players = (FilterMain.max_players + _pck.ReadUInt16());

                                        // Status
                                        FilterMain.status = _pck.ReadUInt8();
                                        /*
                                            1 = ONLINE
                                            0 = CHECK
                                        */

                                        // Unknown
                                        _pck.ReadUInt8();

                                        // Flag shits
                                        flag = _pck.ReadUInt8();
                                    }
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
                            this.m_TransferPoolThread.Abort();
                        }
                        catch { }
                        this.DisconnectModuleSocket();
                        this.m_delDisconnect.Invoke(ref m_ClientSocket, m_HandlerType);
                        return;
                    }
                    DoRecvFromServer();
                }
                catch (Exception AnyEx)
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
                            if (nBps > FilterMain.dMaxBytesPerSec_Gateway)
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
                                if (this.packetCount >= FilterMain.GATEWAY_COUNT)
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

                                                try {
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
                                    // Recieve
                                    this.DoRecvFromServer();

                                    // K-guard
                                    if (this.length != 12)
                                    {
                                        Console.WriteLine("Debug #3");
                                        this.DisconnectModuleSocket();
                                        return;
                                    }

                                    // Continue
                                    continue;
                                }
                                #endregion

                                #region K-guard shits
                                if (_pck.Opcode == 0x2002)
                                {
                                    if (this.length != 0)
                                    {
                                        Console.WriteLine("Debug #2");
                                        this.DisconnectModuleSocket();
                                        return;
                                    }
                                }
                                #endregion

                                #region Fake playercount
                                // Server list
                                if (_pck.Opcode == 0x6101)
                                {
                                    // Block server status tools? :3
                                    if (FilterMain.block_status && this.patch_sent != 1)
                                    {
                                        // Inform
                                        Console.ForegroundColor = ConsoleColor.DarkYellow;
                                        Console.WriteLine("[" + DateTime.UtcNow + "][" + module_name + "] IP:{" + this.ip + "} User:{" + this.user_id + "} Reason:{STATUS TOOL} Bytes:{" + this.length + "} Packet_c{" + this.packetCount + "}");
                                        Console.ResetColor();

                                        // Disconnect
                                        this.DisconnectModuleSocket();
                                        return;
                                    }

                                    // Register
                                    this.sent_list = 1;

                                    // K-guard shits
                                    if (this.length != 0)
                                    {
                                        Console.WriteLine("Debug #1");
                                        this.DisconnectModuleSocket();
                                        return;
                                    }

                                    // Fake players
                                    if (FilterMain.ENABLED && (FilterMain.FAKE_PLAYERS > 0))
                                    {
                                        // Current players
                                        FilterMain.cur_players = (FilterMain.shard_players + FilterMain.FAKE_PLAYERS);

                                        // Fancy fix for sro players :D
                                        if (FilterMain.cur_players >= FilterMain.MAX_PLAYERS)
                                        {
                                            FilterMain.cur_players = FilterMain.MAX_PLAYERS;
                                        }

                                        FilterMain.max_players = FilterMain.MAX_PLAYERS;
                                        FilterMain.status = 1;

                                        Packet response1 = new Packet(0xA101, true);
                                        response1.WriteUInt8(0x01); //flag
                                        response1.WriteUInt8(0x14); //unk
                                        response1.WriteAscii("SRO_Vietnam_TestLocal [F] 0");
                                        response1.WriteUInt8(0x00); //flag

                                        response1.WriteUInt8(0x01); //flag
                                        response1.WriteUInt16(FilterMain.ShardID); //shardID
                                        response1.WriteAscii(FilterMain.ServerName); //name
                                        response1.WriteUInt16(FilterMain.cur_players); //online
                                        response1.WriteUInt16(FilterMain.MAX_PLAYERS); //maxplayers
                                        response1.WriteUInt8(0x01); //Status
                                        response1.WriteUInt8(0x14); //unk
                                        response1.WriteUInt8(0x00); //flag

                                        // Send fake packet :3
                                        m_LocalSecurity.Send(response1);
                                        Send(false);
                                        continue;
                                    }
                                }
                                #endregion

                                #region ID RESPONSE // Exploit fix
                                if (_pck.Opcode == 0x6100)
                                {
                                    this.sent_id = 1;
                                }
                                #endregion

                                #region Patch response // Exploit fix
                                if (_pck.Opcode == 0x6106)
                                {
                                    this.patch_sent = 1;
                                }
                                #endregion

                                #region HARDWARE PACKET OPCODE
                                if (_pck.Opcode == 0x9001 && (FilterMain.PCLIMIT > 0))
                                {
                                    // User HWID
                                    this.hwid = regex.Replace(_pck.ReadAscii(), string.Empty);
                                }
                                #endregion

                                #region Login packet
                                // Login packet
                                if (_pck.Opcode == 0x6102)
                                {
                                    // Check shit
                                    byte locale = _pck.ReadUInt8();
                                    this.user_id = _pck.ReadAscii().ToLower();
                                    this.user_pw = _pck.ReadAscii();
                                    ushort ServerID = _pck.ReadUInt16();

                                    #region New anti exploit(gateway)
                                    // Check news
                                    if (this.sent_list != 1 || this.sent_id != 1 && !(FilterMain.USER_ID.Contains(this.user_id)))
                                    {
                                        // Logging
                                        if (FilterMain.ERROR_LOG.Equals("unknown") || FilterMain.ERROR_LOG.Equals("all") || FilterMain.ERROR_LOG.Equals("exploit"))
                                        {
                                            // Inform
                                            Console.ForegroundColor = ConsoleColor.DarkYellow;
                                            Console.WriteLine("[" + DateTime.UtcNow + "][" + module_name + "] IP:{" + this.ip + "} User:{" + this.user_id + "} Reason:{WEIRD BEHAVIOUR} Bytes:{" + this.length + "} Packet_c{" + this.packetCount + "}");
                                            Console.ResetColor();
                                        }

                                        // Byebye
                                        this.DisconnectModuleSocket();
                                        return;
                                    }
                                    #endregion

                                    #region Pioneer shit
                                    // LOGIN LOG
                                    if (FilterMain.PIONEER)
                                    {
                                        // Ban log(For checking random bans)
                                        System.IO.StreamWriter userlog = new System.IO.StreamWriter("logs/login.txt", true);
                                        userlog.WriteLine("[" + DateTime.UtcNow + "] StrUserID: {" + this.user_id + "} IP: {" + this.ip + "}\n");
                                        userlog.Close();
                                    }
                                    #endregion

                                    #region GM STUFF
                                    // GM ONLY LOGIN
                                    if (FilterMain.GM_LOGIN && !(FilterMain.GM_ACCOUNT.Contains(this.user_id)))
                                    {
                                        // Disconnect
                                        this.DisconnectModuleSocket();
                                        return;
                                    }

                                    // GM PRIV IP
                                    if (FilterMain.PROXY.Contains(FilterMain.AGENT_IP) && FilterMain.GM_ACCOUNT.Contains(this.user_id))
                                    {
                                        // If not allowed ip.
                                        if (FilterMain.PRIV_IP.Count() > 0)
                                        {
                                            if (!FilterMain.PRIV_IP.Contains(this.ip))
                                            {
                                                // Logging
                                                Console.ForegroundColor = ConsoleColor.White;
                                                Console.WriteLine("[" + DateTime.UtcNow + "][" + module_name + "] IP:{" + this.ip + "} User:{" + this.user_id + "} Reason:{PRIV_IP} Bytes:{" + this.length + "} Packet_c{" + this.packetCount + "}");
                                                Console.ResetColor();

                                                // Disconnect
                                                this.DisconnectModuleSocket();
                                                return;
                                            }
                                        }
                                    }
                                    #endregion

                                    #region IP LIMIT SHIT
                                    // IP LIMIT ERROR MESSAGE
                                    if ((FilterMain.IPLIMIT > 0) && !(FilterMain.LIMIT_BYPASS.Contains(this.user_id)))
                                    {
                                        if (FilterMain.CAFELIMIT > 0 && (FilterMain.cafe_list.Contains(this.ip)))
                                        {
                                            // COUNT +1 BECAUSE ALWAYS 1 LESS
                                            if (ip_count(this.ip) > FilterMain.CAFELIMIT)
                                            {
                                                // Send client ERROR
                                                Packet new_packet = new Packet(0xA102, false);
                                                new_packet.WriteUInt8(0x02);
                                                new_packet.WriteUInt8(8); // ip limit error
                                                m_LocalSecurity.Send(new_packet);
                                                Send(false);

                                                // Disconnect
                                                this.DisconnectModuleSocket();

                                                // Continue
                                                return;
                                            }
                                        }
                                        else
                                        {
                                            // COUNT +1 BECAUSE ALWAYS 1 LESS
                                            if (ip_count(this.ip) > FilterMain.IPLIMIT)
                                            {
                                                // Send client ERROR
                                                Packet new_packet = new Packet(0xA102, false);
                                                new_packet.WriteUInt8(0x02);
                                                new_packet.WriteUInt8(8); // ip limit error
                                                m_LocalSecurity.Send(new_packet);
                                                Send(false);

                                                // Disconnect
                                                this.DisconnectModuleSocket();

                                                // Continue
                                                return;
                                            }
                                        }
                                    }
                                    #endregion

                                    #region PC LIMIT SHIT
                                    if (FilterMain.PCLIMIT > 0 &&!(FilterMain.LIMIT_BYPASS.Contains(this.user_id)))
                                    {
                                        // Check length and if not null.
                                        if (this.hwid == null)
                                        {
                                            // Debug
                                            Console.BackgroundColor = ConsoleColor.Red;
                                            Console.ForegroundColor = ConsoleColor.White;
                                            Console.WriteLine(module_name + " HWID was never sent from CLIENT->SUPERMIKE");
                                            Console.ResetColor();

                                            // Send client error
                                            Packet new_packet = new Packet(0xA102, false);
                                            new_packet.WriteUInt8(0x02);
                                            new_packet.WriteUInt8(12); // PC LIMIT ERROR
                                            m_LocalSecurity.Send(new_packet);
                                            Send(false);

                                            // Disconnect
                                            this.DisconnectModuleSocket();
                                            return;
                                        }

                                        // Check HWID LIMIT
                                        if (hwid_count(this.hwid) > FilterMain.PCLIMIT)
                                        {
                                            // Send client error
                                            Packet new_packet = new Packet(0xA102, false);
                                            new_packet.WriteUInt8(0x02);
                                            new_packet.WriteUInt8(10); // PC LIMIT ERROR
                                            m_LocalSecurity.Send(new_packet);
                                            Send(false);

                                            // Disconnect
                                            this.DisconnectModuleSocket();
                                            return;
                                        }

                                        // Advanced system? :D
                                        if (FilterMain.STORE_HWID && (FilterMain.DB))
                                        {
                                            try
                                            {
                                                SqlDataReader reader = sqlCon.Return("_HWIDCHECK", new SqlParameter("@StrUserID", Program.Plis(this.user_id)), new SqlParameter("@HWID", this.hwid));
                                                reader.Read();
                                                int value = reader.GetInt32(0);

                                                // SQL result
                                                if (value != 1)
                                                {
                                                    // Send client error
                                                    Packet new_packet = new Packet(0xA102, false);
                                                    new_packet.WriteUInt8(0x02);
                                                    new_packet.WriteUInt8(11); // PC LIMIT ERROR
                                                    m_LocalSecurity.Send(new_packet);
                                                    Send(false);

                                                    // Close reader
                                                    reader.Close();

                                                    // Disconnect
                                                    this.DisconnectModuleSocket();
                                                    return;
                                                }
                                            }
                                            catch {
                                                // Debug
                                                Console.BackgroundColor = ConsoleColor.Red;
                                                Console.ForegroundColor = ConsoleColor.White;
                                                Console.WriteLine("SQL error, lel :D");
                                                Console.ResetColor();

                                                // Send client error
                                                Packet new_packet = new Packet(0xA102, false);
                                                new_packet.WriteUInt8(0x02);
                                                new_packet.WriteUInt8(11); // PC LIMIT ERROR
                                                m_LocalSecurity.Send(new_packet);
                                                Send(false);

                                                // Disconnect
                                                this.DisconnectModuleSocket();
                                                return;
                                            }
                                        }

                                        // Avoid disconnects @gateway
                                        try
                                        {
                                            // Add user to dictionary list, simple enough? :D
                                            if (!FilterMain.hwid_user.ContainsKey(this.user_id))
                                            {
                                                FilterMain.hwid_user.Add(this.user_id, this.hwid);
                                            }
                                            else
                                            {
                                                FilterMain.hwid_user.Remove(this.user_id);
                                                FilterMain.hwid_user.Add(this.user_id, this.hwid);
                                            }
                                        }
                                        catch {
                                            // Debug
                                            Console.BackgroundColor = ConsoleColor.Red;
                                            Console.ForegroundColor = ConsoleColor.White;
                                            Console.WriteLine("Dictionary error, lel :D");
                                            Console.ResetColor();
                                        }
                                    }
                                    #endregion

                                    #region ANTI BOT STUFF
                                    // Anti bot stuff, nobody would understand.
                                    if (locale == 51 && (FilterMain.BOT_DETECTION))
                                    {
                                        Packet login = new Packet(0x6102, true);
                                        login.WriteUInt8(22);
                                        login.WriteAscii(this.user_id);
                                        login.WriteAscii(this.user_pw);
                                        login.WriteUInt16(ServerID);
                                        m_RemoteSecurity.Send(login);
                                        Send(true);
                                        continue;
                                    }
                                    #endregion
                                }
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
                        }
                        catch { }

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
                    }
                    catch { }

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
                this.m_delDisconnect.Invoke(ref m_ClientSocket, m_HandlerType);
                this.m_delDisconnect.Invoke(ref m_ClientSocket, m_HandlerType);
            }
        }
    }
}
