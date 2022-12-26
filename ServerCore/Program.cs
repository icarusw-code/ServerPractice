using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace ServerCore
{
    class Program
    {

        static void Main(string[] args)
        {
            // DNS(Domain Name System)
            string host = Dns.GetHostName();
            IPHostEntry ipHost = Dns.GetHostEntry(host);
            IPAddress ipAddr = ipHost.AddressList[0];
            IPEndPoint endPoint = new IPEndPoint(ipAddr, 7777);

            // 문지기(소켓 생성)
            Socket listenSocket = new Socket(endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            try
            {
                // 문지기 교육
                listenSocket.Bind(endPoint);

                // 영업시작
                // backlog : 최대 대기수
                listenSocket.Listen(10);

                while (true)
                {
                    Console.WriteLine("Listening....");

                    // 손님 입장
                    Socket clientSocket = listenSocket.Accept();

                    // 받는다.
                    byte[] recvBuff = new byte[1024];
                    int recvBytes = clientSocket.Receive(recvBuff);
                    string recvData = Encoding.UTF8.GetString(recvBuff, 0, recvBytes);
                    Console.WriteLine($"[From client] {recvData}");

                    // 보낸다.
                    byte[] sendBuff = Encoding.UTF8.GetBytes("Welcome to MMORPG Server ! ");
                    clientSocket.Send(sendBuff);

                    // 쫒아낸다.
                    clientSocket.Shutdown(SocketShutdown.Both);
                    clientSocket.Close();

                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }

        }

    }

    
}