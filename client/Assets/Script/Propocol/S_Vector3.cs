using System;

namespace Data
{
    [ProtoBuf.ProtoContract]
    public struct S_Vector3
    {
        [ProtoBuf.ProtoMember(1)]
        public float x;
        [ProtoBuf.ProtoMember(2)]
        public float y;
        [ProtoBuf.ProtoMember(3)]
        public float z;

        public static S_Vector3 zero = new S_Vector3(0, 0, 0);
        public static S_Vector3 one = new S_Vector3(1, 1, 1);

        public S_Vector3(float x, float y, float z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }
        public void SetY(float y)
        {
            this.y = y;
        }
        /// <summary>
        /// 求向量长度
        /// </summary>
        /// <returns></returns>
        public float magnitude()
        {
            return (float)Math.Sqrt(x * x + y * y + z * z);
        }
        public float magnitudeXZ()
        {
            return (float)Math.Sqrt(x * x + z * z);
        }
        public float sqrMagnitude()
        {
            return x * x + y * y + z * z;
        }
        /// <summary>
        /// 求单位向量
        /// </summary>
        /// <returns></returns>
        public S_Vector3 normal()
        {
            float m = magnitude();
            if(m == 0)
            {
                return new S_Vector3(0, 0, 0);
            }
            float k = 1 / m;
            return new S_Vector3(k * x, k * y, k * z);
        }
        public void Add(S_Vector3 v1)
        {
            x += v1.x;
            y += v1.y;
            z += v1.z;
        }
        public void Sub(S_Vector3 v1)
        {
            x -= v1.x;
            y -= v1.y;
            z -= v1.z;
        }
        public void Mul(float v)
        {
            x *= v;
            y *= v;
            z *= v;
        }
        public override string ToString()
        {
            return $"({x},{y},{z})";
        }
        public static S_Vector3 operator +(S_Vector3 v1, S_Vector3 v2)
        {
            return new S_Vector3(v1.x + v2.x, v1.y + v2.y, v1.z + v2.z);
        }
        public static S_Vector3 operator -(S_Vector3 v1, S_Vector3 v2)
        {
            return new S_Vector3(v1.x - v2.x, v1.y - v2.y, v1.z - v2.z);
        }
        public static S_Vector3 operator *(S_Vector3 v1, float f)
        {
            return new S_Vector3(v1.x * f, v1.y * f, v1.z * f);
        }
        public static bool operator ==(S_Vector3 v1, S_Vector3 v2)
        {
            return v1.x == v2.x && v1.y == v2.y && v1.z == v2.z;
        }
        public static bool operator !=(S_Vector3 v1, S_Vector3 v2)
        {
            return v1.x != v2.x || v1.y != v2.y || v1.z != v2.z;
        }
        public static float Distance(S_Vector3 v1, S_Vector3 v2)
        {
            float a = v1.x - v2.x;
            float b = v1.y - v2.y;
            float c = v1.z - v2.z;
            return (float)Math.Sqrt(a * a + b * b + c * c);
        }
        public static float DistanceXZ(S_Vector3 v1, S_Vector3 v2)
        {
            float a = v1.x - v2.x;
            float c = v1.z - v2.z;
            return (float)Math.Sqrt(a * a + c * c);
        }
        public static float SqrDis(S_Vector3 v1, S_Vector3 v2)
        {
            float a = v1.x - v2.x;
            float b = v1.y - v2.y;
            float c = v1.z - v2.z;
            return a * a + b * b + c * c;
        }
        public static float SqrDisXZ(S_Vector3 v1, S_Vector3 v2)
        {
            float a = v1.x - v2.x;
            float c = v1.z - v2.z;
            return a * a + c * c;
        }
    }
}
