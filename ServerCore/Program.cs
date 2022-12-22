using System;

namespace ServerCore
{
    class Program
    {
        static ThreadLocal<string> ThreadName = new ThreadLocal<string>(() => {
            return $"My Name is {Thread.CurrentThread.ManagedThreadId}";
        });
        //static string ThreadName;

        static void WhoAmI()
        {
            //ThreadName = $"My Name is {Thread.CurrentThread.ManagedThreadId}";
            
            bool repeat = ThreadName.IsValueCreated;
            
            if(repeat)
                Console.WriteLine(ThreadName.Value + "(repeat)");
            else
                Console.WriteLine(ThreadName.Value);

            //Thread.Sleep(1000);

            //Console.WriteLine(ThreadName);
        }

        static void Main(string[] args)
        {

            ThreadPool.SetMinThreads(1, 1);
            ThreadPool.SetMaxThreads(3, 3);

            Parallel.Invoke(WhoAmI, WhoAmI, WhoAmI, WhoAmI, WhoAmI, WhoAmI);

        }
        
    }

    
}