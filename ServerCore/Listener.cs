﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ServerCore
{
    public class Listener
    {
        Socket _listenSocket;
        Func<Session> _sessionFactory;
        
        public void Init(IPEndPoint endPoint, Func<Session> sessionFactory, int register = 10, int backlog = 100)
        {
            // 문지기(소켓 생성)
            _listenSocket = new Socket(endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            _sessionFactory += sessionFactory;

            // 문지기 교육
            _listenSocket.Bind(endPoint);

            // 영업시작
            // backlog : 최대 대기수
            _listenSocket.Listen(backlog);

            for(int i = 0; i < register; i++)
            {
                SocketAsyncEventArgs args = new SocketAsyncEventArgs();
                args.Completed += new EventHandler<SocketAsyncEventArgs>(OnAcceptCompleted);
                // 낚시대를 던진다.
                RegisterAccept(args);
            }
        }

        void RegisterAccept(SocketAsyncEventArgs args)
        {
            // 재사용하기 위해 초기화 필요
            args.AcceptSocket = null;

            bool pending = _listenSocket.AcceptAsync(args);
            if (pending == false)
                OnAcceptCompleted(null, args);
            
        }

        // 물고기가 잡혔으니까 낚시대를 끌어올렸다.
        void OnAcceptCompleted(object sender, SocketAsyncEventArgs args)
        {
            // 성공
            if(args.SocketError == SocketError.Success)
            {
                //TODO
                Session session = _sessionFactory.Invoke();
                session.Start(args.AcceptSocket);
                session.OnConnected(args.AcceptSocket.RemoteEndPoint);
            }
            else
            // 실패
            {
                Console.WriteLine(args.SocketError.ToString());
            }
            // 낚시대를 다시 던진다.
            RegisterAccept(args);
        }
    }
}
