using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using Tetris.Controller;
using System.Threading.Tasks;

namespace Tetris.Model
{
   class User
    {        
        public IPAddress IpAddr;
        public TcpClient Connection;
        private const int bufLen = 2048;        
        public const string key = "key";
        TcpListener tcpListener;
        private object locker;
        Thread TcpThread = null;


        public User(IPAddress ipAddr, TcpClient connection)
        {          
            IpAddr = ipAddr;
            Connection = connection;
        }        

        public void  GetTcpMessages(TetrisGrid grid, int port)
        {           

            NetworkStream OneUserStream = Connection.GetStream(); // метод получения сообщения из потока tcp
            try
            {
                while (true)
                {
                    byte[] byteMessage = new byte[bufLen];
                    StringBuilder MessageBuilder = new StringBuilder();
                    string message;
                    int RecBytes = 0;
                    do
                    {
                        RecBytes = OneUserStream.Read(byteMessage, 0, byteMessage.Length);
                        MessageBuilder.Append(Encoding.UTF8.GetString(byteMessage, 0, RecBytes));
                    }
                    while (OneUserStream.DataAvailable);

                    message = MessageBuilder.ToString();

                    int [,] getgrid = JsonConvert.DeserializeAnonymousType<int [,]>(message,grid.Grid2 );                                        

                    grid.Grid2 = getgrid;


                }
            }
            catch
            {
              
            }
            finally
            {
                if (OneUserStream != null)
                    OneUserStream.Close();
                if (Connection != null)
                    Connection.Close();

            }
        }



        public void ConnectTcp(User user,  int port, TetrisGrid g1)
        {
            while (true)
            {
                try
                {
                    tcpListener = new TcpListener(IPAddress.Any, port);
                    tcpListener.Start();
                    TcpClient client = tcpListener.AcceptTcpClient();
                    lock (locker)
                    {
                        user.Connection = client;

                    }
                    TcpThread = new Thread(() => { user.GetTcpMessages(g1, port); });

                    break;
                }
                catch { }
            }
            
        }

    }    
    
    class UDP
    {

        private UdpClient UdpSender;
        private readonly IPAddress IpAdressBroadcast = IPAddress.Parse("192.168.100.255");//IPAddress.Broadcast;
        private IPEndPoint ipEndPointBroadcast;
        private UdpClient UdpListener = null;
        private TcpListener tcpListener;
        public const int UdpPort = 55555;

        public void Connect(int port)
        {
            try
            {
                UdpSender = new UdpClient(port, AddressFamily.InterNetwork);
                ipEndPointBroadcast = new IPEndPoint(IpAdressBroadcast, port);
                byte[] keyBytes = Encoding.ASCII.GetBytes(User.key);
                int sendedData = UdpSender.Send(keyBytes, keyBytes.Length, ipEndPointBroadcast);                
                UdpSender.Close();
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        public void Receive(int TcpPort, int UdpPort, User user, TetrisGrid grid)//если пришел udp-пакет, добавляем отправителя в список, отправляем ему tcp-пакет  с своим логином
        {            
            UdpListener = new UdpClient();
            Thread connect = null;
                try
                {
                    IPEndPoint ClientEndPoint = new IPEndPoint(IPAddress.Any, UdpPort);

                    UdpListener.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
                    UdpListener.ExclusiveAddressUse = false;
                    UdpListener.Client.Bind(ClientEndPoint);               
               

                connect = new Thread( () => { user.ConnectTcp(user,TcpPort, grid); }    );
                connect.Start();
                while (true)
                    {
                        Byte[] data = UdpListener.Receive(ref ClientEndPoint);
                        string ReceiveKey = Encoding.ASCII.GetString(data);
                        if (ReceiveKey == User.key)
                        {
                            user.IpAddr = ClientEndPoint.Address;
                            TcpClient NewTcp = new TcpClient();
                            NewTcp.Connect(new IPEndPoint(ClientEndPoint.Address, TcpPort));
                            user.Connection = NewTcp;
                            UdpListener.Close();
                            
                        }
                    }

                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());                    
                }            
        }     

    }


    class TCP
    {
        private const int bufLen = 64;        
        TcpListener tcpListener;        
        
        public void SendGrid(User user, TetrisGrid grid)
        {
            try
            {                                               

                    string jsonObject = JsonConvert.SerializeObject(grid.Grid1 , Formatting.Indented, new JsonSerializerSettings
                    {
                        TypeNameHandling = TypeNameHandling.All,
                        PreserveReferencesHandling = PreserveReferencesHandling.Objects
                    });
                    byte[] MessageBytes = Encoding.ASCII.GetBytes(jsonObject);

                    if (user != null)
                    user.Connection.GetStream().Write(MessageBytes, 0, MessageBytes.Length);


                
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.ToString());                
            }
        }

        public void Listen(User user, int port, TetrisGrid grid)//ожидание сообщения
        {
            tcpListener = new TcpListener(IPAddress.Any, port);
            tcpListener.Start();
            try
            {
                while (true)
                {
                    TcpClient client = tcpListener.AcceptTcpClient();
                    IPAddress SenderIP = ((IPEndPoint)client.Client.RemoteEndPoint).Address;


                    Thread ClientThread = new Thread(() => { user.GetTcpMessages(grid, port); });
                    ClientThread.IsBackground = true;
                    ClientThread.Start();
                    
                }
            }
            catch
            {
                Console.WriteLine("Ошибка приема сообщений.");
            }
            finally
            {
                tcpListener.Stop();
            }
        }

       
       
    }

    class NetworkCreate
    {
        //  Thread UdpListener = null;
        private const int UdpPort = 55555;
        private const int TcpPort = 55550;
        Thread recThread = null;
        public NetworkCreate(User user, UDP udp, TetrisGrid grid)
        {
            udp.Connect(UdpPort);
            recThread = new Thread(() => { udp.Receive(TcpPort, UdpPort, user, grid); });
            recThread.Start();
            Thread.Sleep(999);
           // udp.Connect(UdpPort);

           
        }








    }
}

