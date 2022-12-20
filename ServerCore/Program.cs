using System;

namespace ServerCore
{
    class Program
    {
        static void Main(string[] args)
        {
            int[,] arr = new int[10000, 10000];
         
            // 인접한 곳을 미리 가지고 있기 때문에 시간이 더 빠르다.
            {
                long now = DateTime.Now.Ticks;
                for (int y = 0; y < 10000; y++)
                {
                    for (int x = 0; x < 10000; x++)
                    {
                        arr[y, x] = 1;
                    }
                }

                long end = DateTime.Now.Ticks;
                Console.WriteLine($"(y,x) 순서 걸린 시간 {end - now}");
            }

            // 캐시를 활용할 수 없는 코드.
            {
                long now = DateTime.Now.Ticks;
                for (int y = 0; y < 10000; y++)
                {
                    for (int x = 0; x < 10000; x++)
                    {
                        arr[x, y] = 1;
                    }
                }
                long end = DateTime.Now.Ticks;
                Console.WriteLine($"(x,y) 순서 걸린 시간 {end - now}");
            }


        }

    }
}