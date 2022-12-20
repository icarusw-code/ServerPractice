using System;

namespace ServerCore
{
    class Program
    {
        static int number = 0;
        static object _obj = new object();
        
        static void Thread_1()
        {
            for (int i = 0; i < 100000; i++)
            {
                // 상호배제 Mutual Exclusive - 관리가 힘들어진다는 단점
                // C++ : CriticalSection 
                //Monitor.Enter(_obj); // 문을 잠구는 행위
                //{
                //    number++;
                //    //return; // 데드락 DeadLock 위에서 return 을 한경우

                //}
                //Monitor.Exit(_obj); // 잠금을 풀어준다.

                // ======================================================
                
                // 데드락 해결책 -> 잘 사용되지 않음 lock키워드를 사용함
                //try
                //{
                //    Monitor.Enter(_obj);
                //    {
                //        number++;

                //        return;
                //    }
                //}
                //finally
                //{
                //    Monitor.Exit(_obj);
                //}

                lock (_obj)
                {
                    number++;
                }

            }
        }

        static void Thread_2()
        {
            for (int i = 0; i < 100000; i++)
            {
                lock (_obj)
                {
                    number--;
                }
            }
        }

        static void Main(string[] args)
        {
            
            Task t1 = new Task(Thread_1);
            Task t2 = new Task(Thread_2);
            t1.Start();
            t2.Start();

            Task.WaitAll(t1, t2);

            Console.WriteLine(number);
        }

    }
}