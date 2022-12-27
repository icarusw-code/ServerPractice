using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ServerCore
{
    class Session
    {
        Socket _socket;
        int _disconnected = 0;

        object _lock = new object();
        Queue<byte[]> _sendQueue = new Queue<byte[]>();
        List<ArraySegment<byte>> _pendingList = new List<ArraySegment<byte>>();
        // send는 실제로 완료할때 까지는 queue에 다 넣어주고
        // 완료되면 sendQueue를 확인해서
        // 이어서 처리한다.
        SocketAsyncEventArgs _sendArgs = new SocketAsyncEventArgs();
        SocketAsyncEventArgs _recvArgs = new SocketAsyncEventArgs();


        public void Start(Socket socket)
        {
            _socket = socket;
            
            _recvArgs.Completed += new EventHandler<SocketAsyncEventArgs>(OnRecvCompleted);
            _recvArgs.SetBuffer(new byte[1024], 0, 1024);

            _sendArgs.Completed += new EventHandler<SocketAsyncEventArgs>(OnSendCompleted);
            RegisterRecv();
        }

        public void Send(byte[] sendBuff)
        {
            lock(_lock)
            {                
                _sendQueue.Enqueue(sendBuff);
                // 처음이다.
                if (_pendingList.Count == 0)
                    RegisterSend();
            }
        }

        // 한번만 실행해야 함
        public void Disconnect()
        {
            // 락을 사용해서 멀티쓰레드 환경에서 중복 실행을 방지
            if (Interlocked.Exchange(ref _disconnected, 1) == 1)
                return;

            _socket.Shutdown(SocketShutdown.Both);
            _socket.Close();
        }

        #region 네트워크 통신

        void RegisterSend()
        {
            // 이 방법과 list를 사용하는 방법중 골라서 사용
            //byte[] buff = _sendQueue.Dequeue();
            // Send의 경우 실제 우리가 사용할 buffer를 넣어준다.
            //_sendArgs.SetBuffer(buff, 0, buff.Length);

            _pendingList.Clear();
            while (_sendQueue.Count > 0)
            {
                byte[] buff = _sendQueue.Dequeue();
                _pendingList.Add(new ArraySegment<byte>(buff, 0, buff.Length)); // 배열, 시작인덱스, 길이
            }

            _sendArgs.BufferList = _pendingList;

            bool pending = _socket.SendAsync(_sendArgs);
            if (pending == false)
                OnSendCompleted(null, _sendArgs);
        }

        void OnSendCompleted(object sender, SocketAsyncEventArgs args)
        {
            lock(_lock)
            {
                if (args.BytesTransferred > 0 && args.SocketError == SocketError.Success)
                {
                    //TODO
                    try
                    {
                        _sendArgs.BufferList = null;
                        _pendingList.Clear();

                        Console.WriteLine($"Transferred bytes: {args.BytesTransferred}");

                        if (_sendQueue.Count > 0)
                        {
                            RegisterSend();
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine($"OnSendCompleted  Failed {e}");
                    }
                }
                else
                {
                    //TODO
                    Disconnect();
                }
            }
        }

        void RegisterRecv()
        {
            bool pending = _socket.ReceiveAsync(_recvArgs);
            // 바로 성공
            if (pending == false)
                OnRecvCompleted(null, _recvArgs);
        }

        void OnRecvCompleted(object sender, SocketAsyncEventArgs args)
        {
            if (args.BytesTransferred > 0 && args.SocketError == SocketError.Success)
            {
                try
                {
                    //TODO
                    string recvData = Encoding.UTF8.GetString(args.Buffer, args.Offset, args.BytesTransferred);
                    Console.WriteLine($"[From client] {recvData}");
                    RegisterRecv();                   
                }
                catch (Exception e)
                {
                    Console.WriteLine($"OnRecvCompleted  Failed {e}");
                }
            }
            else
            {
                // TODO Disconnect
                Disconnect();
            }
        }
        #endregion
    }
}
