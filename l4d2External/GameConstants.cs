// GameConstants.cs (已更新)
namespace left4dead2Menu
{
    // --- 定义瞄准区域的枚举 ---
    // 移动到此处以便在命名空间中全局访问
    public enum AimbotTarget
    {
        Head,
        Chest
    }

    internal static class GameConstants
    {
        public const int SW_HIDE = 0;
        public const int SW_SHOW = 5;

        public const int SURVIVOR_TEAM = 2;
        public const int INFECTED_TEAM = 3;

        public const int LIFE_STATE_ALIVE = 2;
    }
}