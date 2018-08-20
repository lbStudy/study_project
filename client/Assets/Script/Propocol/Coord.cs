using System;
using System.Collections.Generic;
using System.Text;

namespace Data
{
    public struct Coord
    {
        public Coord(int x, int z)
        {
            this.x = x;
            this.z = z;
        }
        public int x;
        public int z;

        public bool IsNear(Coord coord)
        {
            int a = x - coord.x;
            int b = z - coord.z;
            return a < 2 && a > -2 && b < 2 && b > -2;
        }
        public bool IsEqual(Coord coord)
        {
            return x == coord.x && z == coord.z;
        }
        public override string ToString()
        {
            return $"({x},{z})";
        }
    }
}
