// Program.cs (修正最终版)
using ClickableTransparentOverlay;
using ImGuiNET;
using l4d2External;
using System.Text;
using Swed32;
using System.Numerics;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace left4dead2Menu
{
    class Program : Overlay
    {
        private static readonly object _listLock = new object();
        private readonly Swed swed = new Swed("left4dead2");
        private readonly Offsets offsets = new Offsets();
        private readonly Entity localPlayer = new Entity();
        private readonly List<Entity> commonInfected = new List<Entity>();
        private readonly List<Entity> specialInfected = new List<Entity>();
        private readonly List<Entity> bossInfected = new List<Entity>();
        private readonly List<Entity> survivors = new List<Entity>();

        private EntityManager entityManager = null!;
        private AimbotController aimbotController = null!;
        private GuiManager guiManager = null!;
        private Renderer renderer = null!;
        private BunnyHop bunnyHopController = null!;
        private Areas areaController = new Areas();
        private IntPtr clientModule, engineModule;

        private bool hasPerformedMeleeShove = false;

        // --- 解决方案：添加缺失的变量声明 ---
        private Entity? currentAimbotTarget = null;

        // --- 自瞄配置 ---
        private bool enableAimbot = true;
        // private float aimbotTargetZOffset = 0f; // TODO: 暂未使用，临时注释
        private float aimbotSmoothness = 1.00f;
        private AimbotTarget aimbotTargetSelection = AimbotTarget.Head;
        private AimbotMode aimbotModeSelection = AimbotMode.ClosestDistance; // 新增：瞄准模式选择
        private bool aimbotOnBosses = true;
        private bool aimbotOnSpecials = true;
        private bool aimbotOnCommons = true; // 改为 true，启用对普通感染者的自瞄
        private bool aimbotOnSurvivors = false;
        private bool drawFovCircle = true;
        private float fovCircleVisualRadius = 200.0f;
        private Vector4 fovCircleColor = new Vector4(1, 1, 1, 0.5f);
        private readonly int specialAimbotKey = 0x10; // 改为Shift键 (VK_SHIFT)
        private bool enableAimbotArea = false;
        private float aimbotAreaRadius = 300.0f;
        private int aimbotAreaSegments = 40;
        private Vector4 aimbotAreaColor = new Vector4(1, 0, 1, 0.7f);

        // --- ESP配置 ---
        private bool enableEsp = true;
        private bool espOnBosses = true;
        private bool espOnSpecials = true;
        private bool espOnCommons = true;
        private bool espOnSurvivors = false;
        private Vector4 espColorBosses = new Vector4(1, 0, 0, 1);
        private Vector4 espColorSpecials = new Vector4(1, 0.6f, 0, 1);
        private Vector4 espColorCommons = new Vector4(0.8f, 0.8f, 0.8f, 0.7f);
        private Vector4 espColorSurvivors = new Vector4(0.2f, 0.8f, 1, 1);
        private bool espDrawHead = false;
        private bool espDrawBody = false;

        // --- 其他配置 ---
        private bool enableBunnyHop = true;
        private bool enableMeleeArea = true;
        private float meleeAreaRadius = 80.0f;
        private int meleeAreaSegments = 40;
        private Vector4 meleeAreaColor = new Vector4(0, 1, 1, 0.7f);
        private bool meleeOnCommons = true;
        private bool meleeOnHunter = true;
        private bool meleeOnSmoker = true;
        private bool meleeOnBoomer = true;
        private bool meleeOnJockey = true;
        private bool meleeOnSpitter = false;
        private bool meleeOnCharger = false;

        private void InitializeLogicModules()
        {
            clientModule = swed.GetModuleBase("client.dll");
            engineModule = swed.GetModuleBase("engine.dll");
            if (clientModule == IntPtr.Zero || engineModule == IntPtr.Zero) return;

            entityManager = new EntityManager(swed, offsets, Encoding.ASCII);
            aimbotController = new AimbotController();
            guiManager = new GuiManager();
            renderer = new Renderer(swed, engineModule, offsets);
            bunnyHopController = new BunnyHop(swed, offsets);
        }

        protected override void Render()
        {
            float screenWidth = ImGui.GetIO().DisplaySize.X;
            float screenHeight = ImGui.GetIO().DisplaySize.Y;
            Vector2 centerScreen = new Vector2(screenWidth / 2, screenHeight / 2);

            ImGui.Begin("Left 4 Dead 2 Cheat");

            guiManager.DrawMenuControls(
                // Aimbot
                ref enableAimbot, /* ref aimbotTargetZOffset, */ ref drawFovCircle, ref fovCircleVisualRadius, ref aimbotSmoothness,
                ref aimbotTargetSelection, ref aimbotModeSelection, // 添加新的瞄准模式参数
                ref aimbotOnBosses, ref aimbotOnSpecials, ref aimbotOnCommons, ref aimbotOnSurvivors,
                ref enableAimbotArea, ref aimbotAreaRadius, ref aimbotAreaSegments, ref aimbotAreaColor,
                // ESP
                ref enableEsp, ref espOnBosses, ref espColorBosses, ref espOnSpecials, ref espColorSpecials,
                ref espOnCommons, ref espColorCommons, ref espOnSurvivors, ref espColorSurvivors,
                ref espDrawHead, ref espDrawBody,
                // Others
                ref enableBunnyHop,
                ref enableMeleeArea, ref meleeAreaRadius, ref meleeAreaSegments, ref meleeAreaColor,
                ref meleeOnCommons,
                ref meleeOnHunter, ref meleeOnSmoker, ref meleeOnBoomer,
                ref meleeOnJockey, ref meleeOnSpitter, ref meleeOnCharger
            );
            ImGui.End();

            if (renderer != null)
            {
                renderer.UpdateViewMatrix();

                // 变量'currentAimbotTarget'在此处已可访问
                if (enableAimbot && NativeMethods.GetAsyncKeyState(specialAimbotKey) < 0)
                {
                    // 1. 构建潜在目标列表
                    var aimTargets = new List<Entity>();
                    lock (_listLock)
                    {
                        if (aimbotOnBosses) aimTargets.AddRange(bossInfected);
                        if (aimbotOnSpecials) aimTargets.AddRange(specialInfected);
                        if (aimbotOnCommons) aimTargets.AddRange(commonInfected);
                        if (aimbotOnSurvivors) aimTargets.AddRange(survivors);
                    }

                    // 2. 找到最佳目标
                    if (aimTargets.Count > 0)
                    {
                        var bestTarget = aimbotController.FindBestTarget(localPlayer, aimTargets, aimbotTargetSelection, enableAimbotArea, aimbotAreaRadius, fovCircleVisualRadius, renderer, screenWidth, screenHeight, aimbotModeSelection);

                        // 3. 如果找到目标，对其执行操作
                        if (bestTarget != null)
                        {
                            aimbotController.ExecuteMouseAimbot(bestTarget, localPlayer, aimbotTargetSelection, aimbotSmoothness, renderer, screenWidth, screenHeight);
                        }
                    }
                }

                if (enableMeleeArea && localPlayer.address != IntPtr.Zero)
                {
                    areaController.DrawCircleArea(ImGui.GetBackgroundDrawList(), localPlayer.origin, renderer, screenWidth, screenHeight, meleeAreaRadius, meleeAreaSegments, meleeAreaColor);
                }

                if (enableAimbotArea && localPlayer.address != IntPtr.Zero)
                {
                    areaController.DrawCircleArea(ImGui.GetBackgroundDrawList(), localPlayer.origin, renderer, screenWidth, screenHeight, aimbotAreaRadius, aimbotAreaSegments, aimbotAreaColor);
                }

                if (enableAimbot && drawFovCircle && !enableAimbotArea)
                {
                    renderer.DrawFovCircle(ImGui.GetBackgroundDrawList(), centerScreen, fovCircleVisualRadius, fovCircleColor);
                }
                if (enableEsp)
                {
                    List<Entity> commonSnapshot, specialSnapshot, bossSnapshot, survivorSnapshot;
                    lock (_listLock)
                    {
                        commonSnapshot = new List<Entity>(commonInfected);
                        specialSnapshot = new List<Entity>(specialInfected);
                        bossSnapshot = new List<Entity>(bossInfected);
                        survivorSnapshot = new List<Entity>(survivors);
                    }
                    renderer.RenderAll(
                        ImGui.GetBackgroundDrawList(), screenWidth, screenHeight,
                        commonSnapshot, specialSnapshot, bossSnapshot, survivorSnapshot,
                        espOnBosses, espColorBosses, espOnSpecials, espColorSpecials,
                        espOnCommons, espColorCommons, espOnSurvivors, espColorSurvivors,
                        espDrawHead, espDrawBody
                    );
                }
            }
        }

        // --- MainLogicLoop AHORA ES MÁS SIMPLE ---
        void MainLogicLoop()
        {
            InitializeLogicModules();
            if (entityManager == null || bunnyHopController == null) return;

            while (true)
            {
                // 仅负责更新实体和不依赖UI的逻辑
                lock (_listLock)
                {
                    entityManager.ReloadEntities(localPlayer, commonInfected, specialInfected, bossInfected, survivors, clientModule);
                }

                if (localPlayer.address != IntPtr.Zero)
                {
                    bunnyHopController.Update(localPlayer.address, enableBunnyHop);

                    if (enableMeleeArea)
                    {
                        var meleeTargets = new List<Entity>();
                        lock (_listLock)
                        {
                            if (meleeOnCommons) meleeTargets.AddRange(commonInfected);

                            foreach (var special in specialInfected)
                            {
                                switch (special.SimpleName)
                                {
                                    case "Hunter": if (meleeOnHunter) meleeTargets.Add(special); break;
                                    case "Smoker": if (meleeOnSmoker) meleeTargets.Add(special); break;
                                    case "Boomer": if (meleeOnBoomer) meleeTargets.Add(special); break;
                                    case "Jockey": if (meleeOnJockey) meleeTargets.Add(special); break;
                                    case "Spitter": if (meleeOnSpitter) meleeTargets.Add(special); break;
                                    case "Charger": if (meleeOnCharger) meleeTargets.Add(special); break;
                                }
                            }
                        }

                        bool enemyInMeleeRange = meleeTargets.Any(e => e.health > 0 && e.magnitude <= meleeAreaRadius);

                        if (enemyInMeleeRange && !hasPerformedMeleeShove)
                        {
                            NativeMethods.SimulateRightClick();
                            hasPerformedMeleeShove = true;
                        }
                        else if (!enemyInMeleeRange)
                        {
                            hasPerformedMeleeShove = false;
                        }
                    }
                }

                Thread.Sleep(5);
            }
        }

        static void Main(string[] args)
        {
            Program program = new Program();
            IntPtr consoleHandle = NativeMethods.GetConsoleWindow();
            NativeMethods.ShowWindow(consoleHandle, GameConstants.SW_HIDE);
            Thread mainLogicThread = new Thread(program.MainLogicLoop) { IsBackground = true };
            mainLogicThread.Start();
            program.Start().Wait();
        }
    }
}