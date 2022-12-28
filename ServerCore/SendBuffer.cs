using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerCore
{
    public class SendBufferHelper
    {
        public static ThreadLocal<SendBuffer> CurrentBuffer = new ThreadLocal<SendBuffer>(() => { return null; });

        public static int ChnkSize { get; set; } = 4096 * 100;

        public static ArraySegment<byte> Open(int reserveSize)
        {
            if (CurrentBuffer.Value == null)
                CurrentBuffer.Value = new SendBuffer(ChnkSize);

            if(CurrentBuffer.Value.FreeSize < reserveSize)
                CurrentBuffer.Value = new SendBuffer(ChnkSize);

            return CurrentBuffer.Value.Open(reserveSize);
        }

        public static ArraySegment<byte> Close(int usedSize)
        {
            return CurrentBuffer.Value.Close(usedSize);
        }
    }

    public class SendBuffer
    {
        // [u] [ ] [ ] [ ] [ ] [ ] [ ] [ ] [ ] [ ] (10byte로 가정)
        // [ ] [ ] [ ] [ ] [u] [ ] [ ] [ ] [ ] [ ] (10byte로 가정)
        byte[] _buffer;
        int _useSize = 0;

        public int FreeSize { get { return _buffer.Length - _useSize; } }

        public SendBuffer(int chunkSize)
        {
            _buffer = new byte[chunkSize];
        }
        
        public ArraySegment<byte> Open(int reserveSize) 
        {
            if (FreeSize < reserveSize)
                return null;

            return new ArraySegment<byte>(_buffer, _useSize, reserveSize);
        }

        public ArraySegment<byte> Close(int usedSize) 
        {
            ArraySegment<byte> segment = new ArraySegment<byte>(_buffer, _useSize, usedSize);
            _useSize += usedSize;

            return segment;
        }
    }
}
