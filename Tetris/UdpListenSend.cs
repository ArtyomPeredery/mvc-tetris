using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Tetris.Model;

namespace Tetris
{
    


    class UdpListenSend
    {
        private UdpClient UdpSender;
        private readonly IPAddress IpAdressBroadcast = IPAddress.Parse("192.168.43.255");//IPAddress.Broadcast;
        private IPEndPoint ipUserEndPoint;
        private UdpClient UdpListener = null;
        private readonly string key = "KEY";
        public IPAddress UserIP = null;
        public int UdpSendPort = 55555;
        public int ListenPiecesPort = 48048;
        public Random rand = new Random();
        public int ReceiveGridPort;
        public int SendGridPort;
        private Thread RecThread = null;
        public TetrisGrid Grid;
        public int Score = 0;
        public bool Start = false;
        public int[] Pieces = new int[255];
        public UdpListenSend(TetrisGrid grid, int score,int[] pieces)
        {
            Grid = grid;
            Score = score;
            Pieces = pieces;           

        }

        public void SendRequest()
        {
            
            ReceiveGridPort = rand.Next(55000, 56000);           
            try
            {
                UdpSender = new UdpClient(UdpSendPort, AddressFamily.InterNetwork);
                ipUserEndPoint = new IPEndPoint(IpAdressBroadcast,UdpSendPort);
                byte[] KeyBytes = Encoding.ASCII.GetBytes(key+ ReceiveGridPort.ToString());
                int sendedData = UdpSender.Send(KeyBytes, KeyBytes.Length, ipUserEndPoint);
                UdpSender.Close();
            }
            catch 
            {              
            }
        }

      

        public void GetIpAddressFromUser()
        {
            UdpListener = new UdpClient();
            try
            {
                IPEndPoint ClientEndPoint = new IPEndPoint(IPAddress.Any, UdpSendPort);

                UdpListener.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
                UdpListener.ExclusiveAddressUse = false;
                UdpListener.Client.Bind(ClientEndPoint);
                while (true)
                {
                    Byte[] data = UdpListener.Receive(ref ClientEndPoint);
                    string GetKey = Encoding.ASCII.GetString(data);
                    if (GetKey[0] == 'P')
                    {

                        // for (int i=0; i<masstr.Length;i++)
                        string[] strget = GetKey.Split(' ');

                        for (int i = 0; i < Pieces.Length; i++)
                        {
                            try
                            {
                                Pieces[i] = int.Parse(strget[i]);
                            }
                            catch { }
                        }


                        string str = GetKey.Substring(1, GetKey.Length - 1);
                        StreamWriter SW = new StreamWriter(new FileStream("FileName.txt", FileMode.Create, FileAccess.Write));
                        SW.Write(GetKey);
                        SW.Close();
                        

                    }
                                       
                    if (GetKey.Substring(0, 3) == key)
                    {
                        SendGridPort = int.Parse(GetKey.Substring(3, GetKey.Length - 3));
                        
                        UserIP = ClientEndPoint.Address;
                        Start = true;
                        MessageBox.Show("Opponent was finded. You can play with him");                        
                        break;
                    }
                }

                UdpListener.Close();
                Thread.Sleep(150);
                try
                {

                    string SendStr = "";
                    for (int i = 0; i < Pieces.Length; i++)
                    {
                        SendStr = SendStr + " " + Pieces[i].ToString();

                    }                                                        

                    UdpSender = new UdpClient(UdpSendPort, AddressFamily.InterNetwork);
                        ipUserEndPoint = new IPEndPoint(UserIP, UdpSendPort);
                        byte[] KeyBytes = Encoding.ASCII.GetBytes("P" + SendStr);
                        int sendedData = UdpSender.Send(KeyBytes, KeyBytes.Length, ipUserEndPoint);                        
                    Thread.Sleep(100);
                        ipUserEndPoint = new IPEndPoint(IpAdressBroadcast, UdpSendPort);
                     KeyBytes = Encoding.ASCII.GetBytes(key + ReceiveGridPort.ToString());
                     sendedData = UdpSender.Send(KeyBytes, KeyBytes.Length, ipUserEndPoint);
                    UdpSender.Close();

                    RecThread = new Thread(() => { GetGridUdp(Grid); }  );
                    RecThread.Start();
                }
                catch
                {                    
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            finally
            {
                UdpListener.Close();
            }
        }


        public void SendGrid(TetrisGrid grid)
        {

            string jsonObject = JsonConvert.SerializeObject(grid.Grid1, Formatting.Indented, new JsonSerializerSettings { });



            try
            {
                if (UserIP != null)
                {
                    UdpSender = new UdpClient(SendGridPort, AddressFamily.InterNetwork);
                    ipUserEndPoint = new IPEndPoint(UserIP, SendGridPort);
                    byte[] SendData = Encoding.ASCII.GetBytes(jsonObject);
                    int sendedData = UdpSender.Send(SendData, SendData.Length, ipUserEndPoint);
                    UdpSender.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }

        }




        public void GetGridUdp(TetrisGrid grid)
        {
            UdpListener = new UdpClient();
            try
            {
                IPEndPoint ClientEndPoint = new IPEndPoint(IPAddress.Any, ReceiveGridPort);

                UdpListener.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
                UdpListener.ExclusiveAddressUse = false;
                UdpListener.Client.Bind(ClientEndPoint);
                while (true)
                {
                    Byte[] data = UdpListener.Receive(ref ClientEndPoint);
                    string Message = Encoding.ASCII.GetString(data);

                    if (Message.Substring(0, 1) == "S")
                    {
                        
                        Score = int.Parse(Message.Substring(1, Message.Length - 1));

                    }
                    else
                    {
                        int[,] getgrid = JsonConvert.DeserializeAnonymousType<int[,]>(Message, grid.Grid2);
                        grid.Grid2 = getgrid;         
                        

                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            finally
            {
                UdpListener.Close();
            }


        }

        public void SendScore(int Score)
        {
            


            try
            {
                if (UserIP != null)
                {
                    UdpSender = new UdpClient(SendGridPort, AddressFamily.InterNetwork);
                    ipUserEndPoint = new IPEndPoint(UserIP, SendGridPort);
                    byte[] SendData = Encoding.ASCII.GetBytes("S"+Score.ToString());
                    int sendedData = UdpSender.Send(SendData, SendData.Length, ipUserEndPoint);
                    UdpSender.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }



        }


        public void SendPieces()
        {            
            for (int i = 0; i < Pieces.Length; i++)
            {
                Pieces[i] = rand.Next(0, 7);

            }

            try
            {
                UdpSender = new UdpClient(SendGridPort, AddressFamily.InterNetwork);
                ipUserEndPoint = new IPEndPoint(UserIP, SendGridPort);
                byte[] KeyBytes = Encoding.ASCII.GetBytes("P" + Pieces.ToString());
                int sendedData = UdpSender.Send(KeyBytes, KeyBytes.Length, ipUserEndPoint);
                UdpSender.Close();
            }
            catch
            {
            }
        }
    }
}
