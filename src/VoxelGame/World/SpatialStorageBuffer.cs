using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VoxelGame.World
{
    public class SpatialStorageBuffer<T>
    {
        private T[] _chunks;
        private int _length;

        public SpatialStorageBuffer(int length)
        {
            _chunks = new T[length * length * length];
            _length = length;
        }

        public int Length { get { return _length; } }

        public void Resize(int newLength)
        {
            T[] newChunks = new T[newLength];
            for (int x = 0; x < _length; x++)
            {
                for (int y = 0; y < _length; y++)
                {
                    for (int z = 0; z < _length; z++)
                    {
                        newChunks[x + (y * newLength) + (z * newLength * newLength)] = this[x, y, z];
                    }
                }
            }

            _chunks = newChunks;
            _length = newLength;
        }

        public T this[int x, int y, int z]
        {
            get
            {
                if (x < 0 || y < 0 || z < 0 || x >= _length || y >= _length || z >= _length)
                {
                    throw new ArgumentOutOfRangeException();
                }
                return _chunks[x + (y * _length) + (z * _length * _length)];
            }
            set
            {
                if (x < 0 || y < 0 || z < 0 || x >= _length || y >= _length || z >= _length)
                {
                    throw new ArgumentOutOfRangeException();
                }
                _chunks[x + (y * _length) + (z * _length * _length)] = value;
            }
        }
    }
}
