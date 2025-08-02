// Renderer.cs (手动矩阵读取最终版)
using ImGuiNET;
using System.Numerics;
using System.Collections.Generic;
using l4d2External;
using Swed32;
using System;
using System.Linq;

namespace left4dead2Menu
{
    internal class Renderer
    {
        private readonly Swed swed;
        private readonly IntPtr engine;
        private readonly Offsets offsets;
        private Matrix4x4 viewMatrix;

        public Renderer(Swed swed, IntPtr engine, Offsets offsets)
        {
            this.swed = swed;
            this.engine = engine;
            this.offsets = offsets;
            this.viewMatrix = new Matrix4x4();
        }

        public void UpdateViewMatrix()
        {
            try
            {
                var viewMatrixBase = swed.ReadPointer(engine, offsets.ViewMatrix);
                if (viewMatrixBase != IntPtr.Zero)
                {
                    var matrixAddress = viewMatrixBase + offsets.ViewMatrixOffset;
                    byte[] matrixBytes = swed.ReadBytes(matrixAddress, 64);
                    if (matrixBytes != null && matrixBytes.Length == 64)
                    {
                        float[] matrixFloats = new float[16];
                        Buffer.BlockCopy(matrixBytes, 0, matrixFloats, 0, 64);
                        viewMatrix = new Matrix4x4(
                            matrixFloats[0], matrixFloats[1], matrixFloats[2], matrixFloats[3],
                            matrixFloats[4], matrixFloats[5], matrixFloats[6], matrixFloats[7],
                            matrixFloats[8], matrixFloats[9], matrixFloats[10], matrixFloats[11],
                            matrixFloats[12], matrixFloats[13], matrixFloats[14], matrixFloats[15]
                        );
                    }
                }
            }
            catch { viewMatrix = new Matrix4x4(); }
        }

        public bool WorldToScreen(Vector3 worldPos, out Vector2 screenPos, float screenWidth, float screenHeight)
        {
            screenPos = Vector2.Zero;
            Matrix4x4 transposedMatrix = Matrix4x4.Transpose(viewMatrix);
            Vector4 clipCoords = Vector4.Transform(worldPos, transposedMatrix);

            if (clipCoords.W < 0.1f) return false;

            Vector3 ndc = new Vector3(clipCoords.X / clipCoords.W, clipCoords.Y / clipCoords.W, clipCoords.Z / clipCoords.W);

            screenPos.X = (ndc.X + 1.0f) * 0.5f * screenWidth;
            screenPos.Y = (1.0f - ndc.Y) * 0.5f * screenHeight;

            return true;
        }

        public void DrawFovCircle(ImDrawListPtr drawList, Vector2 centerScreen, float radius, Vector4 color)
        {
            if (radius > 0)
            {
                drawList.AddCircle(centerScreen, radius, ImGui.GetColorU32(color), 32, 1.5f);
            }
        }

        public void RenderAll(
            ImDrawListPtr drawList, float screenWidth, float screenHeight,
            List<Entity> common, List<Entity> special, List<Entity> bosses, List<Entity> survivors,
            bool espOnBosses, Vector4 espColorBosses, bool espOnSpecials, Vector4 espColorSpecials,
            bool espOnCommons, Vector4 espColorCommons, bool espOnSurvivors, Vector4 espColorSurvivors,
            bool espDrawHead, bool espDrawBody)
        {
            // 普感不绘制血条，其他实体绘制血条
            if (espOnCommons) RenderESPForCommonInfected(drawList, common, espColorCommons, screenWidth, screenHeight, espDrawHead, espDrawBody);
            if (espOnSpecials) RenderESPForEntitiesWithHealthBar(drawList, special, espColorSpecials, screenWidth, screenHeight, espDrawHead, espDrawBody);
            if (espOnBosses) RenderESPForEntitiesWithHealthBar(drawList, bosses, espColorBosses, screenWidth, screenHeight, espDrawHead, espDrawBody);
            if (espOnSurvivors) RenderESPForEntitiesWithHealthBar(drawList, survivors, espColorSurvivors, screenWidth, screenHeight, espDrawHead, espDrawBody);
        }

        // 专门为普感设计的渲染方法（不绘制血条）
        private void RenderESPForCommonInfected(ImDrawListPtr drawList, List<Entity> entities, Vector4 color, float screenWidth, float screenHeight, bool drawHead, bool drawBody)
        {
            if (entities == null) return;
            uint uintColor = ImGui.GetColorU32(color);

            foreach (var entity in entities)
            {
                if (entity == null) continue;

                if (WorldToScreen(entity.abs, out Vector2 screenHead, screenWidth, screenHeight) &&
                    WorldToScreen(entity.origin, out Vector2 screenFeet, screenWidth, screenHeight))
                {
                    if (screenFeet.X < 0 || screenFeet.X > screenWidth || screenHead.Y < 0 || screenFeet.Y > screenHeight) continue;

                    float height = Math.Abs(screenHead.Y - screenFeet.Y);
                    if (height <= 2 || height > screenHeight) continue;

                    float width = height / 2.1f;
                    Vector2 topLeft = new Vector2(screenFeet.X - width / 2, screenHead.Y);
                    Vector2 bottomRight = new Vector2(screenFeet.X + width / 2, screenFeet.Y);

                    ESP.DrawName(drawList, entity.SimpleName, topLeft, width, uintColor);
                    ESP.DrawBox(drawList, topLeft, bottomRight, uintColor);

                    if (drawHead)
                    {
                        ESP.DrawHeadBone(drawList, topLeft, width, height, uintColor);
                    }
                    if (drawBody)
                    {
                        ESP.DrawBodyBone(drawList, topLeft, width, height, uintColor);
                    }

                    // 普感不绘制血条
                }
            }
        }

        // 为其他实体设计的渲染方法（绘制血条）
        private void RenderESPForEntitiesWithHealthBar(ImDrawListPtr drawList, List<Entity> entities, Vector4 color, float screenWidth, float screenHeight, bool drawHead, bool drawBody)
        {
            if (entities == null) return;
            uint uintColor = ImGui.GetColorU32(color);

            foreach (var entity in entities)
            {
                if (entity == null) continue;

                if (WorldToScreen(entity.abs, out Vector2 screenHead, screenWidth, screenHeight) &&
                    WorldToScreen(entity.origin, out Vector2 screenFeet, screenWidth, screenHeight))
                {
                    if (screenFeet.X < 0 || screenFeet.X > screenWidth || screenHead.Y < 0 || screenFeet.Y > screenHeight) continue;

                    float height = Math.Abs(screenHead.Y - screenFeet.Y);
                    if (height <= 2 || height > screenHeight) continue;

                    float width = height / 2.1f;
                    Vector2 topLeft = new Vector2(screenFeet.X - width / 2, screenHead.Y);
                    Vector2 bottomRight = new Vector2(screenFeet.X + width / 2, screenFeet.Y);

                    ESP.DrawName(drawList, entity.SimpleName, topLeft, width, uintColor);
                    ESP.DrawBox(drawList, topLeft, bottomRight, uintColor);

                    if (drawHead)
                    {
                        ESP.DrawHeadBone(drawList, topLeft, width, height, uintColor);
                    }
                    if (drawBody)
                    {
                        ESP.DrawBodyBone(drawList, topLeft, width, height, uintColor);
                    }

                    // 非普感实体绘制血条
                    ESP.DrawHealthBar(drawList, topLeft, bottomRight, entity.health, entity.maxHealth);
                }
            }
        }
    }
}