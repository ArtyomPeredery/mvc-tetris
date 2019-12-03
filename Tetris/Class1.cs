using Newtonsoft.Json;
using System;
using System.Collections.Generic;
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
    /*   class User
       {
           public string Name;
           public IPAddress IpAddr;
           public TcpClient Connection;
           private const int bufLen = 2048;        

           public User( IPAddress ipAddr, TcpClient connection)
           {           
               IpAddr = ipAddr;
               Connection = connection;
           }



           public void GetTcpMessages(TetrisGrid grid)
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

                       int[,] getgrid = JsonConvert.DeserializeAnonymousType<int[,]>(message, grid.Grid2);
                       lock (Program.locker)
                       {
                           grid.Grid2 = getgrid;
                       }
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

           public void HistoryRecieve(ref List<string> History, User client)
           {
               byte[] HistoryItemBytes;
               foreach (string HistoryItem in History)
               {
                   HistoryItemBytes = Encoding.ASCII.GetBytes(HistoryItem);
                   client.Connection.GetStream().Write(HistoryItemBytes, 0, HistoryItemBytes.Length);
               }
           }

       }
       */
    /*  class UDP
      {
          private UdpClient UdpSender;
          private readonly IPAddress IpAdressBroadcast = IPAddress.Parse("192.168.100.255");//IPAddress.Broadcast;
          private IPEndPoint ipEndPointBroadcast;
          private UdpClient UdpListener = null;

          public void Connect(string key, int port)
          {
              try
              {
                  UdpSender = new UdpClient(port, AddressFamily.InterNetwork);
                  ipEndPointBroadcast = new IPEndPoint(IpAdressBroadcast, port);
                  byte[] KeyBytes = Encoding.ASCII.GetBytes(key);
                  int sendedData = UdpSender.Send(KeyBytes, KeyBytes.Length, ipEndPointBroadcast);                
                  UdpSender.Close();
              }
              catch (Exception ex)
              {
                  MessageBox.Show(ex.ToString());
              }
          }

          public void Receive(User Users, int TcpPort, int UdpPort, TetrisGrid grid)//если пришел udp-пакет, добавляем отправителя в список, отправляем ему tcp-пакет  с своим логином
          {
              UdpListener = new UdpClient();
              try
              {
                  IPEndPoint ClientEndPoint = new IPEndPoint(IPAddress.Any, UdpPort);

                  UdpListener.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
                  UdpListener.ExclusiveAddressUse = false;
                  UdpListener.Client.Bind(ClientEndPoint);
                  while (true)
                  {
                      Byte[] data = UdpListener.Receive(ref ClientEndPoint);
                      string Key = Encoding.ASCII.GetString(data);
                      if (Key == "key")
                      {
                          Users.IpAddr = ClientEndPoint.Address;

                          TcpClient NewTcp = new TcpClient();
                          NewTcp.Connect(new IPEndPoint(ClientEndPoint.Address, TcpPort));

                          Users.Connection = NewTcp;

                          StartClientReceive(Users, grid);
                          break;
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

          public void StartClientReceive(User client, TetrisGrid grid)
          {
              Thread ClientThread = new Thread(() => { client.GetTcpMessages(grid); });
              ClientThread.Start();
          }

      }
      */

    /*  class TCP
      {
          private const int bufLen = 2048;        
          TcpListener tcpListener;

          public void SendMessage(User clients, string Message)
          {
              byte[] MessageBytes = Encoding.ASCII.GetBytes(Message);

                  clients.Connection.GetStream().Write(MessageBytes, 0, MessageBytes.Length);

          }

          public void Listen(User clients, int port, TetrisGrid grid)//ожидание сообщения
          {
              tcpListener = new TcpListener(IPAddress.Any, port);
              tcpListener.Start();
              try
              {
                  while (true)
                  {
                      TcpClient client = tcpListener.AcceptTcpClient();
                      IPAddress SenderIP = ((IPEndPoint)client.Client.RemoteEndPoint).Address;                    

                      StartClientReceive(clients, grid);
                  }
              }
              catch(Exception ex)
              {
                  MessageBox.Show(ex.ToString());
              }
              finally
              {
                  tcpListener.Stop();
              }
          }

          public void StartClientReceive(User client, TetrisGrid grid)
          {
              Thread ClientThread = new Thread(() => { client.GetTcpMessages(grid); });
              ClientThread.IsBackground = true;
              ClientThread.Start();
          }

      }*/
  }
  
