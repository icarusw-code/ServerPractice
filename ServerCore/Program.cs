using System;

namespace ServerCore
{
    class Program
    {
        // 최적화 하지말아라.(C#에서는 사용 지양 부가적 기능이 많음)
        // Release 모드에서는 최적화가 되어서 코드가 다르게 실행될 수 도 있다.
        volatile static bool _stop = false;

        static void ThreadMain()
        {
            Console.WriteLine("쓰레드 시작!");

            while (_stop == false)
            { 
                // 누군가가 stop 신호를 해주기를 기다린다.
            }
                

            Console.WriteLine("쓰레드 종료!");
            
        }

        static void Main(string[] args)
        {
            Task t = new Task(ThreadMain);
            t.Start();

            // 1초간 잠시 멈춤
            Thread.Sleep(1000);

            _stop = true;

            Console.WriteLine("Stop 호출!");
            Console.WriteLine("종료 대기중");

            t.Wait();
            
            Console.WriteLine("종료 성공");
        }

    }
}