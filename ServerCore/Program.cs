using System;

namespace ServerCore
{
    class Lock
    {
        // 처음에 연 상태로 시작할지, 닫힌 상태일지 결정
        //AutoResetEvent _available = new AutoResetEvent(true);
        ManualResetEvent _available = new ManualResetEvent(true);

        public void Acquire()
        {
            _available.WaitOne(); // 입장 시도
            _available.Reset(); // 문을 닫는다.
        }

        public void Release()
        {
            _available.Set(); // flag = true 로 만듬 // 문을 열어준다.
        }
    }

    class Program
    {
        static int _num = 0;

        static Lock _lock = new Lock();

        static void Thread_1()
        {
            for(int i = 0; i < 100000; i++)
            {
                _lock.Acquire();
                _num++;
                _lock.Release();
            }
        }

        static void Thread_2()
        {
            for (int i = 0; i < 100000; i++)
            {
                _lock.Acquire();
                _num--;
                _lock.Release();
            }
        }

       static void Main(string[] args)
        {
            Task t1 = new Task(Thread_1);
            Task t2 = new Task(Thread_2);

            t1.Start();
            t2.Start();

            Task.WaitAll(t1, t2);

            Console.WriteLine(_num);
        }

    }
}