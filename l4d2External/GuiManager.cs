// GuiManager.cs (Synchronized Final Version)
using ImGuiNET;
using System.Numerics;

namespace left4dead2Menu
{
    internal class GuiManager
    {
        public void DrawMenuControls(
            // Aimbot
            ref bool enableAimbot, ref bool drawFovCircle,
            ref float fovCircleVisualRadius, ref float aimbotSmoothness,
            ref AimbotTarget aimbotTarget, ref AimbotMode aimbotMode, // 添加新的瞄准模式参数
            ref bool aimbotOnBosses, ref bool aimbotOnSpecials, ref bool aimbotOnCommons, ref bool aimbotOnSurvivors,
            // 新增自瞄区域功能
            ref bool enableAimbotArea, ref float aimbotAreaRadius, ref int aimbotAreaSegments, ref Vector4 aimbotAreaColor,

            // ESP
            ref bool enableEsp, ref bool espOnBosses, ref Vector4 espColorBosses,
            ref bool espOnSpecials, ref Vector4 espColorSpecials,
            ref bool espOnCommons, ref Vector4 espColorCommons,
            ref bool espOnSurvivors, ref Vector4 espColorSurvivors, // 修复参数名称
            ref bool espDrawHead, ref bool espDrawBody,

            // Others
            ref bool enableBunnyHop,
            ref bool enableMeleeArea, ref float meleeAreaRadius, ref int meleeAreaSegments, ref Vector4 meleeAreaColor,
            // --- 已更新的近战筛选参数 ---
            ref bool meleeOnCommons,
            ref bool meleeOnHunter, ref bool meleeOnSmoker, ref bool meleeOnBoomer,
            ref bool meleeOnJockey, ref bool meleeOnSpitter, ref bool meleeOnCharger
            )
        {
            if (ImGui.BeginTabBar("Main Menu"))
            {
                if (ImGui.BeginTabItem("Aimbot"))
                {
                    ImGui.Checkbox("Enable Aimbot", ref enableAimbot);
                    if (enableAimbot)
                    {
                        ImGui.SeparatorText("Aimbot Configuration");
                        ImGui.SliderFloat("Smoothness", ref aimbotSmoothness, 0.01f, 1.0f, "%.2f");

                        ImGui.SeparatorText("Target Part");
                        int aimbotTargetInt = (int)aimbotTarget;
                        if (ImGui.RadioButton("Head", ref aimbotTargetInt, (int)AimbotTarget.Head))
                        {
                            aimbotTarget = AimbotTarget.Head;
                        }
                        ImGui.SameLine();
                        if (ImGui.RadioButton("Chest", ref aimbotTargetInt, (int)AimbotTarget.Chest))
                        {
                            aimbotTarget = AimbotTarget.Chest;
                        }

                        ImGui.SeparatorText("Aimbot Mode");
                        int aimbotModeInt = (int)aimbotMode;
                        if (ImGui.RadioButton("Closest Distance", ref aimbotModeInt, (int)AimbotMode.ClosestDistance))
                        {
                            aimbotMode = AimbotMode.ClosestDistance;
                        }
                        ImGui.SameLine();
                        if (ImGui.RadioButton("Closest to Crosshair", ref aimbotModeInt, (int)AimbotMode.ClosestToCrosshair))
                        {
                            aimbotMode = AimbotMode.ClosestToCrosshair;
                        }

                        ImGui.SeparatorText("Targets");
                        ImGui.Checkbox("Bosses (Tank, Witch)", ref aimbotOnBosses);
                        ImGui.Checkbox("Special Infected", ref aimbotOnSpecials);
                        ImGui.Checkbox("Common Infected", ref aimbotOnCommons);
                        ImGui.Checkbox("Survivors", ref aimbotOnSurvivors);

                        ImGui.SeparatorText("Aimbot FOV");
                        ImGui.Checkbox("Show FOV Circle", ref drawFovCircle);
                        ImGui.SliderFloat("FOV Radius", ref fovCircleVisualRadius, 10.0f, 500.0f, "%.0f px");

                        ImGui.SeparatorText("Aimbot Area (Radius Mode)");
                        ImGui.Checkbox("Enable Aimbot Area", ref enableAimbotArea);
                        if (enableAimbotArea)
                        {
                            ImGui.SliderFloat("Aimbot Area Radius", ref aimbotAreaRadius, 50.0f, 1000.0f, "%.0f u");
                            ImGui.SliderInt("Circle Segments", ref aimbotAreaSegments, 12, 100);
                            ImGui.ColorEdit4("Area Color", ref aimbotAreaColor, ImGuiColorEditFlags.NoInputs);
                        }
                    }
                    ImGui.EndTabItem();
                }

                if (ImGui.BeginTabItem("ESP"))
                {
                    ImGui.Checkbox("Enable ESP", ref enableEsp);
                    if (enableEsp)
                    {
                        ImGui.SeparatorText("Visibility & Colors");

                        ImGui.Checkbox("Bosses", ref espOnBosses);
                        ImGui.SameLine();
                        ImGui.ColorEdit4("##Boss Color", ref espColorBosses, ImGuiColorEditFlags.NoInputs);

                        ImGui.Checkbox("Special Infected", ref espOnSpecials);
                        ImGui.SameLine();
                        ImGui.ColorEdit4("##Special Color", ref espColorSpecials, ImGuiColorEditFlags.NoInputs);

                        ImGui.Checkbox("Common Infected", ref espOnCommons);
                        ImGui.SameLine();
                        ImGui.ColorEdit4("##Common Color", ref espColorCommons, ImGuiColorEditFlags.NoInputs);

                        ImGui.Checkbox("Survivors", ref espOnSurvivors);
                        ImGui.SameLine();
                        ImGui.ColorEdit4("##Survivor Color", ref espColorSurvivors, ImGuiColorEditFlags.NoInputs);

                        ImGui.SeparatorText("Bone Display");
                        ImGui.Checkbox("Draw Head", ref espDrawHead);
                        ImGui.Checkbox("Draw Body", ref espDrawBody);
                    }
                    ImGui.EndTabItem();
                }

                if (ImGui.BeginTabItem("Others"))
                {
                    ImGui.SeparatorText("Movement");
                    ImGui.Checkbox("Enable Bunny Hop", ref enableBunnyHop);

                    ImGui.SeparatorText("Melee Area (Auto Attack)");
                    ImGui.Checkbox("Enable Melee Area", ref enableMeleeArea);
                    if (enableMeleeArea)
                    {
                        ImGui.SliderFloat("Melee Area Radius", ref meleeAreaRadius, 50.0f, 300.0f, "%.0f u");
                        ImGui.SliderInt("Circle Segments", ref meleeAreaSegments, 12, 100);
                        ImGui.ColorEdit4("Area Color", ref meleeAreaColor, ImGuiColorEditFlags.NoInputs);

                        ImGui.SeparatorText("Melee Targets");
                        ImGui.Checkbox("Common Infected", ref meleeOnCommons);

                        // First row of specials
                        ImGui.Checkbox("Hunter", ref meleeOnHunter);
                        ImGui.SameLine();
                        ImGui.Checkbox("Smoker", ref meleeOnSmoker);
                        ImGui.SameLine();
                        ImGui.Checkbox("Boomer", ref meleeOnBoomer);

                        // Second row of specials
                        ImGui.Checkbox("Jockey", ref meleeOnJockey);
                        ImGui.SameLine();
                        ImGui.Checkbox("Spitter", ref meleeOnSpitter);
                        ImGui.SameLine();
                        ImGui.Checkbox("Charger", ref meleeOnCharger);
                    }

                    ImGui.EndTabItem();
                }

                ImGui.EndTabBar();
            }
        }
    }
}