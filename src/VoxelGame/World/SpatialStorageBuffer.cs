using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VoxelGame.World
{
    public class SpatialStorageBuffer<T>
    {
        private T[] _items;
        private int _length;
        private int _numItems;

        public SpatialStorageBuffer(int length)
        {
            _items = new T[length * length * length];
            _length = length;
            _numItems = _items.Length;
        }

        public int Length { get { return _length; } }

        public int NumItems { get { return _numItems; } }

        public void Resize(int newLength)
        {
            T[] newItems = new T[newLength];
            for (int x = 0; x < _length; x++)
            {
                for (int y = 0; y < _length; y++)
                {
                    for (int z = 0; z < _length; z++)
                    {
                        newItems[x + (y * newLength) + (z * newLength * newLength)] = this[x, y, z];
                    }
                }
            }

            _items = newItems;
            _length = newLength;
        }

        public T this[int x, int y, int z]
        {
            get
            {
                if (x < 0 || y < 0 || z < 0 || x >= _length || y >= _length || z >= _length)
                {
                    throw new ArgumentOutOfRangeException(string.Format("x, y, z, length = {0}, {1}, {2}, {3}", x, y, z, _length));
                }
                return _items[x + (y * _length) + (z * _length * _length)];
            }
            set
            {
                if (x < 0 || y < 0 || z < 0 || x >= _length || y >= _length || z >= _length)
                {
                    throw new ArgumentOutOfRangeException(string.Format("x, y, z, length = {0}, {1}, {2}, {3}", x, y, z, _length));
                }
                _items[x + (y * _length) + (z * _length * _length)] = value;
            }
        }

        public T this[int index]
        {
            get
            {
                return _items[index];
            }
            set
            {
                _items[index] = value;
            }
        }
    }
}
