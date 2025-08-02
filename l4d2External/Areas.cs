// Areas.cs (Refactorizado)
using ImGuiNET;
using System.Numerics;
using l4d2External;
using System;

namespace left4dead2Menu
{
    internal class Areas
    {
        /// <summary>
        /// 在指定中心点绘制圆形区域
        /// </summary>
        public void DrawCircleArea(ImDrawListPtr drawList, Vector3 center, Renderer renderer, float screenWidth, float screenHeight, float radius, int segments, Vector4 color)
        {
            if (center == Vector3.Zero) return;

            uint uintColor = ImGui.GetColorU32(color);
            float step = (float)(2 * Math.PI / segments);
            Vector2? prevScreenPos = null;
            Vector2? firstScreenPos = null;

            for (int i = 0; i <= segments; i++)
            {
                float angle = i * step;

                Vector3 worldPos = new Vector3(
                    center.X + (float)Math.Cos(angle) * radius,
                    center.Y + (float)Math.Sin(angle) * radius,
                    center.Z
                );

                if (renderer.WorldToScreen(worldPos, out Vector2 currentScreenPos, screenWidth, screenHeight))
                {
                    if (prevScreenPos.HasValue)
                    {
                        drawList.AddLine(prevScreenPos.Value, currentScreenPos, uintColor, 1.5f);
                    }
                    if (!firstScreenPos.HasValue)
                    {
                        firstScreenPos = currentScreenPos;
                    }
                    prevScreenPos = currentScreenPos;
                }
            }

            if (firstScreenPos.HasValue && prevScreenPos.HasValue)
            {
                drawList.AddLine(prevScreenPos.Value, firstScreenPos.Value, uintColor, 1.5f);
            }
        }
    }
}