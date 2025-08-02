// Entity.cs (Corregido)
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace l4d2External
{
    internal class Entity
    {
        public IntPtr address { get; set; }
        public int health { get; set; }
        public int maxHealth { get; set; } // 新增最大血量属性
        public int teamNum { get; set; }
        public int lifeState { get; set; }
        public int jumpflag { get; set; }
        public Vector3 origin { get; set; }
        public Vector3 viewOffset { get; set; }
        public Vector3 abs { get; set; }
        public float magnitude { get; set; }
        public string? SimpleName { get; set; }

        // 解决CS8618警告：属性现在可以为空
        public string? modelName { get; set; }
        public Entity()
        {
            address = IntPtr.Zero;
            origin = Vector3.Zero;
            abs = Vector3.Zero;
            viewOffset = Vector3.Zero;
        }
    }
}