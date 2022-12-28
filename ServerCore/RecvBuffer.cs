using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerCore
{
    public class RecvBuffer
    {
        // [rw ] [ ] [ ] [ ] [ ] [ ] [ ] [ ] [ ] [ ] (10byte로 가정)
        // [r ] [ ] [ ] [ ] [ ] [w ] [ ] [ ] [ ] [ ] (10byte로 가정)
        // [r ] [ ] [ ] [ ] [ ] [ ] [ ] [w ] [ ] [ ] (10byte로 가정)
        // [ ] [ ] [ ] [ ] [ ] [ ] [ ] [rw ] [ ] [ ] (10byte로 가정)
        ArraySegment<byte> _buffer;
        // 마우스 커서라고 생각
        int _readPos;
        int _writePos; 

        public RecvBuffer(int bufferSize)
        {
            _buffer = new ArraySegment<byte>(new byte[bufferSize], 0, bufferSize);
            
        }

        public int DataSize { get { return _writePos - _readPos; } }
        // 버퍼의 남은 공간
        public int FreeSize { get { return _buffer.Count - _writePos; } }

        public ArraySegment<byte> ReadSegment
        {
            get { return new ArraySegment<byte>(_buffer.Array, _buffer.Offset + _readPos, DataSize); }
        }
        public ArraySegment<byte> WriteSegment
        {
            get { return new ArraySegment<byte>(_buffer.Array, _buffer.Offset + _writePos, FreeSize); }
        }

        // 버퍼 당겨주기
        public void Clean()
        {
            int dataSize = DataSize;
            // 데이터가 없다면 복사하지 않고 커서 위치만 리셋
            if (dataSize == 0)
            {
                _readPos = _writePos = 0;
            }
            // 데이터가 있다면 시작 위치로 복사
            else
            {
                Array.Copy(_buffer.Array, _buffer.Offset + _readPos, _buffer.Array, _buffer.Offset, dataSize);
                _readPos = 0;
                _writePos = dataSize;
            }
        }
        
        public bool OnRead(int numOfBytes)
        {
            // 버퍼가 꽉 찼다면
            if (FreeSize < numOfBytes)
                return false;
            
            _readPos += numOfBytes;
            return true;
        }
        
        public bool OnWrite(int numOfBytes)
        {
            if(FreeSize < numOfBytes)
                return false;
            
            _writePos += numOfBytes;
            return true;
        }
    }
}
