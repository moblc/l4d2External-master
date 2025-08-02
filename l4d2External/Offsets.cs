// Offsets.cs (已更新)
using System;

namespace l4d2External
{
    internal class Offsets
    {
        // --- 视图矩阵偏移量已更新 ---
        public int ViewMatrix = 0x601FDC;
        public int ViewMatrixOffset = 0x2E4;

        // 引擎偏移量
        public int engineAngles = 0x4268EC;
        public int engineAnglesOffset = 0x4AAC;

        // 客户端偏移量
        public int localplayer = 0x726BD8;
        public int entityList = 0x73A574 + 0x10;

        // 实体偏移量
        public int Health = 0xEC;
        public int MaxHealth = 0x1FDC; // 新增最大血量偏移
        public int Lifestate = 0x144;
        public int JumpFlag = 0xF0;
        public int ViewOffset = 0xF4;
        public int Origin = 0x124;
        public int TeamNum = 0xE4;
        public int ModelName = 0x60;
    }
}