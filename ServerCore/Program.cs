using System;

namespace ServerCore
{
    class Program
    {
        static int number;
       
        static void Thread_1()
        {
            for (int i = 0; i < 100000; i++)
            {
                // All or Nothing
                // 동시 다발적이여도 순서가 생긴다.
                Interlocked.Increment(ref number);
                //number++;
            }
        }

        static void Thread_2()
        {
            for (int i = 0; i < 100000; i++)
            {
                Interlocked.Decrement(ref number);
                //number--;
            }
        }

        static void Main(string[] args)
        {
            // atomic = 원자성
            // number++;
            // int temp = number;
            // temp += 1;
            // number = temp;
            
            Task t1 = new Task(Thread_1);
            Task t2 = new Task(Thread_2);
            t1.Start();
            t2.Start();

            Task.WaitAll(t1, t2);

            Console.WriteLine(number);
        }

    }
}