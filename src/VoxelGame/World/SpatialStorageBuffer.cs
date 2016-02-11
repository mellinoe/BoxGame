using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VoxelGame.World
{
    public class SpatialStorageBuffer<T>
    {
        private T[] _items;

        private int _numItems;

        private int _width;
        private int _height;
        private int _depth;

        public SpatialStorageBuffer(int width, int height, int depth)
        {
            _items = new T[width * height * depth];
            _width = width;
            _height = height;
            _depth = depth;
            _numItems = _items.Length;
        }

        public int NumItems { get { return _numItems; } }

        public void Resize(int newLength)
        {
            T[] newItems = new T[newLength];
            for (int x = 0; x < _width; x++)
            {
                for (int y = 0; y < _height; y++)
                {
                    for (int z = 0; z < _depth; z++)
                    {
                        newItems[x + (y * newLength) + (z * newLength * newLength)] = this[x, y, z];
                    }
                }
            }

            _items = newItems;
            _numItems = newLength;
        }

        public T this[int x, int y, int z]
        {
            get
            {
                if (x < 0 || y < 0 || z < 0 || x >= _width || y >= _height || z >= _depth)
                {
                    throw new ArgumentOutOfRangeException();
                }
                return _items[x + (y * _width) + (z * _height * _width)];
            }
            set
            {
                if (x < 0 || y < 0 || z < 0 || x >= _width || y >= _height || z >= _depth)
                {
                    throw new ArgumentOutOfRangeException();
                }
                _items[x + (y * _width) + (z * _height * _width)] = value;
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
