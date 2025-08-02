// ESP.cs (修改版)
using ImGuiNET;
using System;
using System.Numerics;

namespace left4dead2Menu
{
    internal static class ESP
    {
        public static void DrawBox(ImDrawListPtr drawList, Vector2 topLeft, Vector2 bottomRight, uint color)
        {
            drawList.AddRect(topLeft, bottomRight, color, 0, ImDrawFlags.None, 1.5f);
        }

        public static void DrawHealthBar(ImDrawListPtr drawList, Vector2 boxTopLeft, Vector2 boxBottomRight, int currentHealth, int maxHealth)
        {
            // 安全检查：确保最大血量大于0，避免除零错误
            if (maxHealth <= 0)
            {
                maxHealth = 1; // 设置最小值以避免除零
            }

            // 限制当前血量在合理范围内
            currentHealth = Math.Max(0, Math.Min(currentHealth, maxHealth));

            // 计算血量百分比：当前血量 / 最大血量
            float healthPercentage = (float)currentHealth / maxHealth;
            float boxWidth = boxBottomRight.X - boxTopLeft.X;

            Vector2 healthBarStart = new Vector2(boxTopLeft.X, boxBottomRight.Y + 3);
            Vector2 healthBarEnd = new Vector2(boxTopLeft.X + (boxWidth * healthPercentage), boxBottomRight.Y + 7);

            Vector2 healthBarBgEnd = new Vector2(boxBottomRight.X, boxBottomRight.Y + 7);
            uint colorRed = ImGui.GetColorU32(new Vector4(1, 0, 0, 0.7f));
            uint colorGreen = ImGui.GetColorU32(new Vector4(0, 1, 0, 1f));

            // 绘制红色背景（代表损失的血量）
            drawList.AddRectFilled(healthBarStart, healthBarBgEnd, colorRed);
            // 绘制绿色前景（代表当前血量）
            drawList.AddRectFilled(healthBarStart, healthBarEnd, colorGreen);
        }

        // --- 新增方法：绘制头部骨骼 ---
        public static void DrawHeadBone(ImDrawListPtr drawList, Vector2 boxTopLeft, float boxWidth, float boxHeight, uint color)
        {
            // 在头部位置绘制圆圈，相对于方框大小
            Vector2 headCenter = new Vector2(boxTopLeft.X + boxWidth / 2, boxTopLeft.Y + boxHeight * 0.1f);
            float headRadius = boxWidth * 0.15f; // 半径相对于宽度
            drawList.AddCircle(headCenter, headRadius, color, 12, 1.5f);
        }

        // --- 新增方法：绘制身体/躯干 ---
        public static void DrawBodyBone(ImDrawListPtr drawList, Vector2 boxTopLeft, float boxWidth, float boxHeight, uint color)
        {
            // 在躯干位置绘制矩形，相对于方框大小
            Vector2 bodyTopLeft = new Vector2(boxTopLeft.X + boxWidth * 0.2f, boxTopLeft.Y + boxHeight * 0.2f);
            Vector2 bodyBottomRight = new Vector2(boxTopLeft.X + boxWidth * 0.8f, boxTopLeft.Y + boxHeight * 0.6f);
            drawList.AddRect(bodyTopLeft, bodyBottomRight, color, 0, ImDrawFlags.None, 1.5f);
        }
        /// <summary>
        /// 在ESP方框上方绘制实体名称
        /// </summary>
        public static void DrawName(ImDrawListPtr drawList, string? name, Vector2 boxTopLeft, float boxWidth, uint color)
        {
            // 检查是否为空已正确处理，现在签名匹配
            if (string.IsNullOrEmpty(name)) return;

            Vector2 textSize = ImGui.CalcTextSize(name);
            Vector2 textPos = new Vector2(
                boxTopLeft.X + (boxWidth / 2) - (textSize.X / 2),
                boxTopLeft.Y - textSize.Y - 2
            );
            drawList.AddText(textPos, color, name);
        }

        /// <summary>
        /// 显示实体名称和血量信息
        /// </summary>
        public static void DrawNameWithHealth(ImDrawListPtr drawList, string? name, int currentHealth, int maxHealth, Vector2 boxTopLeft, float boxWidth, uint color)
        {
            if (string.IsNullOrEmpty(name)) return;

            // 构建显示文本：名字 + 血量信息
            string displayText = $"{name} [{currentHealth}/{maxHealth}]";
            
            Vector2 textSize = ImGui.CalcTextSize(displayText);
            Vector2 textPos = new Vector2(
                boxTopLeft.X + (boxWidth / 2) - (textSize.X / 2),
                boxTopLeft.Y - textSize.Y - 2
            );
            
            // 绘制完整的文本
            drawList.AddText(textPos, color, displayText);
        }
    }
}