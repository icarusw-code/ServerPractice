using System;

namespace ServerCore
{
    class Program
    {
        static int _num = 0;

        // 가진 정보가 많다. 잠근횟수, ThreadId 가지고 있음 ...
        static Mutex _lock = new Mutex();

        static void Thread_1()
        {
            for(int i = 0; i < 10000; i++)
            {
                _lock.WaitOne();
                _num++;
                _lock.ReleaseMutex();
            }
        }

        static void Thread_2()
        {
            for (int i = 0; i < 10000; i++)
            {
                _lock.WaitOne();
                _num--;
                _lock.ReleaseMutex();
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