using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace ExploitFilter.NetEngine
{
    sealed class AsyncServer
    {
        Socket m_ListenerSock = null;
        E_ServerType m_ServerType;

        public static int m_ClientCount_Gateway = 0, m_ClientCount_Agent = 0, m_ClientCount_Agent2 = 0;

        ManualResetEvent m_Waiter = new ManualResetEvent(false);
        Thread m_AcceptInitThread = null;

        // Random
        Random rnd = new Random();

        public int GatewayClientCount
        {
            get
            {
                return m_ClientCount_Gateway;
            }
        }

        public int AgentClientCount
        {
            get
            {
                return m_ClientCount_Agent;
            }
        }

        public int AgentClientCount2
        {
            get
            {
                return m_ClientCount_Agent2;
            }
        }

        public enum E_ServerType : byte
        {
            GatewayServer,
            AgentServer,
            AgentServer2
        }
        public delegate void delClientDisconnect(ref Socket ClientSocket, E_ServerType HandlerType);




        public bool Start(string BindAddr, int nPort, E_ServerType ServType)
        {
            bool res = false;
            if (m_ListenerSock != null)
            {
                throw new Exception(FilterMain.FILTER+"Trying to start server on socket which is already in use");
            }

            m_ServerType = ServType;

            m_ListenerSock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            try
            {
                m_ListenerSock.Bind(new IPEndPoint(IPAddress.Parse(BindAddr), nPort));
                m_ListenerSock.Listen(rnd.Next(1, 65535));

                m_AcceptInitThread = new Thread(AcceptInitThread);
                m_AcceptInitThread.Start();

                //Console.ForegroundColor = ConsoleColor.DarkCyan;
                //Console.WriteLine(FilterMain.FILTER + "FORWARDING COMPLETE " + IPAddress.Parse(BindAddr) + ":" + nPort);
                //Console.ResetColor();
            }
            catch (SocketException SocketEx)
            {
                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.WriteLine(FilterMain.FILTER+"Could not bind/listen/BeginAccept socket. Exception: {0}", SocketEx.ToString());
                Console.ResetColor();
            }
            
            return res;
        }

        void AcceptInitThread()
        {
            while (m_ListenerSock != null)
            {
                m_Waiter.Reset();
                try
                {
                    m_ListenerSock.BeginAccept(
                        new AsyncCallback(AcceptConnectionCallback), null
                        );
                }
                catch{}
                m_Waiter.WaitOne();
            }
        }

        //asynchronous callback on connection accepted
        void AcceptConnectionCallback(IAsyncResult iar)
        {
            Socket ClientSocket = null;

            //AcceptInitThread sleeps...
            m_Waiter.Set();

            try
            {
                ClientSocket = m_ListenerSock.EndAccept(iar);
            }

            catch (SocketException SocketEx)
            {
                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.WriteLine(FilterMain.FILTER+"AcceptConnectionCallback()::SocketException while EndAccept. Exception: {0}", SocketEx.ToString());
                Console.ResetColor();

            }
            catch (ObjectDisposedException ObjDispEx)
            {
                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.WriteLine(FilterMain.FILTER+"AcceptConnectionCallback()::ObjectDisposedException while EndAccept. Is server shutting down ? Exception: {0}", ObjDispEx.ToString());
                Console.ResetColor();
            }

            try
            {
                switch (m_ServerType)
                {
                    case E_ServerType.GatewayServer:
                        {
                            //pass socket to gateway context handler
                            new GatewayContext(ClientSocket, OnClientDisconnect);
                            //m_ClientCount_Gateway++;
                            Console.Title = string.Format("Client count [GatewayServer: {0}] [AgentServer1: {1}] [AgentServer2: {2}]", FilterMain.gateway, FilterMain.agent1, FilterMain.agent2);
                        }
                        break;
                    case E_ServerType.AgentServer:
                        {
                            //pass socket to agent context handler
                            new AgentContext(ClientSocket, OnClientDisconnect);
                            //m_ClientCount_Agent++;
                            //FilterMain.cur_players++;
                            Console.Title = string.Format("Client count [GatewayServer: {0}] [AgentServer1: {1}] [AgentServer2: {2}]", FilterMain.gateway, FilterMain.agent1, FilterMain.agent2); ;
                        }
                        break;
                    case E_ServerType.AgentServer2:
                        {
                            //pass socket to agent context handler
                            new AgentContext2(ClientSocket, OnClientDisconnect);
                            //m_ClientCount_Agent2++;
                            //FilterMain.cur_players++;
                            Console.Title = string.Format("Client count [GatewayServer: {0}] [AgentServer1: {1}] [AgentServer2: {2}]", FilterMain.gateway, FilterMain.agent1, FilterMain.agent2);
                        }
                        break;
                    default:
                        {
                            Console.ForegroundColor = ConsoleColor.Yellow;
                            Console.WriteLine(FilterMain.FILTER+"AcceptConnectionCallback()::Unknown server type");
                            Console.ResetColor();
                            //Environment.Exit(0);
                        }
                        break;
                }
            }
            catch (SocketException SocketEx)
            {
                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.WriteLine(FilterMain.FILTER+"AcceptConnectionCallback()::Error while starting context. Exception: {0}", SocketEx.ToString());
                Console.ResetColor();
            }
        }

        void OnClientDisconnect(ref Socket ClientSock, E_ServerType HandlerType)
        {
            // Check
            if (ClientSock == null)
            {
                return;
            }

            switch (HandlerType)
            {
                case E_ServerType.GatewayServer:
                    {
                        //m_ClientCount_Gateway--;
                        Console.Title = string.Format("Client count [GatewayServer: {0}] [AgentServer1: {1}] [AgentServer2: {2}]", FilterMain.gateway, FilterMain.agent1, FilterMain.agent2);
                    }
                    break;
                case E_ServerType.AgentServer:
                    {
                        //m_ClientCount_Agent--;
                        //FilterMain.cur_players--;
                        Console.Title = string.Format("Client count [GatewayServer: {0}] [AgentServer1: {1}] [AgentServer2: {2}]", FilterMain.gateway, FilterMain.agent1, FilterMain.agent2);
                    }
                    break;
                case E_ServerType.AgentServer2:
                    {
                        //m_ClientCount_Agent2--;
                        //FilterMain.cur_players--;
                        Console.Title = string.Format("Client count [GatewayServer: {0}] [AgentServer1: {1}] [AgentServer2: {2}]", FilterMain.gateway, FilterMain.agent1, FilterMain.agent2);
                    }
                    break;
            }

            try
            {
                ClientSock.Close();
            }
            catch (SocketException SocketEx)
            {
                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.WriteLine(FilterMain.FILTER+"OnClientDisconnect()::Error closing socket. Exception: {0}", SocketEx.ToString());
                Console.ResetColor();
            }
            catch (ObjectDisposedException ObjDispEx)
            {
                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.WriteLine(FilterMain.FILTER+"OnClientDisconnect()::Error closing socket (socket already disposed?). Exception: {0}", ObjDispEx.ToString());
                Console.ResetColor();
            }
            catch
            {
                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.WriteLine(FilterMain.FILTER + "Something went wrong with Async systems.");
                Console.ResetColor();
            }

            
            ClientSock = null;
            GC.Collect();
        }
    }

}
