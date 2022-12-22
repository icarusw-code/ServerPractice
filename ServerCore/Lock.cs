using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerCore
{
    // 정책
    // 재귀적 락을 허용할지 (No)
    // 스핀란 정책(5000번 -> Yield)
    class Lock
    {
        const int EMPTY_FLAG = 0x00000000;
        const int WRITE_MASK = 0x7FFF0000;
        const int READ_MASK = 0x0000FFFF;
        const int MAX_SPIN_COUNT = 5000;

        // [Unused(1)][WriteThreadId(15)][ReadCount(16)]
        int _flag = EMPTY_FLAG;

        public void WriteLock()
        {
            // 아무도 WriteLock or ReadLock을 획득하고 있지 않을 때, 경합해서 소유권을 얻는다.
            int desired = (Thread.CurrentThread.ManagedThreadId << 16) & WRITE_MASK;
            while (true)
            {   
                for(int i = 0; i < MAX_SPIN_COUNT; i++)
                {
                    // 시도를 해서 성공하면 return
                    //if (_flag == EMPTY_FLAG)
                        //_flag = desired;
                    if(Interlocked.CompareExchange(ref _flag, desired, EMPTY_FLAG) == EMPTY_FLAG)
                    {
                        return;
                    }
                }

                Thread.Yield();
            }
        }

        public void WriteUnlock()
        {
            // 초기화
            Interlocked.Exchange(ref _flag, EMPTY_FLAG);
        }

        public void ReadLock()
        {
            // 아무도 WriteLock을 획득하고 있지 않으면, ReadCount 를 1 늘린다.
            while(true)
            {
                for (int i = 0; i < MAX_SPIN_COUNT; i++)
                {
                    //if((_flag & WRITE_MASK) == 0)
                    //{
                    //    _flag = _flag + 1;
                    //    return;
                    //}
                    int expected = (_flag & READ_MASK); // A(0) B(0)
                    if (Interlocked.CompareExchange(ref _flag, expected + 1, expected) == expected) // A(0 ->1) B(0 -> 1) 둘 중 하나 성공하면 expected 값이 바뀌므로 실행 안됨
                        return;
                }

                Thread.Yield();
            }
            
        }

        public void ReadUnlock()
        {
            // 1줄여주기
            Interlocked.Decrement(ref _flag);
        }
    }
}
