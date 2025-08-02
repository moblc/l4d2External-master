// AimbotController.cs (修正版)
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Swed32;
using l4d2External;

namespace left4dead2Menu
{
    // 新增枚举：瞄准模式选择
    public enum AimbotMode
    {
        ClosestDistance = 0,  // 距离最近的目标
        ClosestToCrosshair = 1 // 离准星最近的目标
    }

    internal class AimbotController
    {
        public AimbotController()
        {
        }

        public Entity? FindBestTarget(Entity localPlayer, List<Entity> potentialTargets, AimbotTarget targetSelection, bool isAimbotAreaEnabled, float aimbotAreaRadius, float fovVisualRadius, Renderer renderer, float screenWidth, float screenHeight, AimbotMode aimbotMode = AimbotMode.ClosestDistance)
        {
            // 修改筛选逻辑：普通感染者不判断血量，其他实体判断血量
            var liveTargets = potentialTargets.Where(t => IsCommonInfected(t) || t.health > 0).ToList();
            if (liveTargets.Count == 0) return null;

            List<Entity> validTargets;
            Vector2 screenCenter = new Vector2(screenWidth / 2, screenHeight / 2);

            if (isAimbotAreaEnabled)
            {
                validTargets = liveTargets.Where(t => t.magnitude <= aimbotAreaRadius).ToList();
            }
            else
            {
                validTargets = liveTargets.Where(t =>
                {
                    Vector3 aimPos = GetTargetPosition(t, targetSelection);
                    if (renderer.WorldToScreen(aimPos, out Vector2 screenPos, screenWidth, screenHeight))
                    {
                        return Vector2.Distance(screenCenter, screenPos) <= fovVisualRadius;
                    }
                    return false;
                }).ToList();
            }

            if (validTargets.Count > 0)
            {
                // 根据瞄准模式选择不同的排序方式
                switch (aimbotMode)
                {
                    case AimbotMode.ClosestDistance:
                        // 原有逻辑：按距离排序
                        return validTargets.OrderBy(t => t.magnitude).First();

                    case AimbotMode.ClosestToCrosshair:
                        // 新逻辑：按离准星距离排序
                        return validTargets
                            .Select(t => new { Target = t, ScreenDistance = GetScreenDistance(t, targetSelection, renderer, screenWidth, screenHeight, screenCenter) })
                            .Where(x => x.ScreenDistance >= 0) // 过滤掉无法投射到屏幕的目标
                            .OrderBy(x => x.ScreenDistance)
                            .FirstOrDefault()?.Target;

                    default:
                        return validTargets.OrderBy(t => t.magnitude).First();
                }
            }

            return null;
        }

        // 新方法：计算目标在屏幕上离准星的距离
        private float GetScreenDistance(Entity target, AimbotTarget targetSelection, Renderer renderer, float screenWidth, float screenHeight, Vector2 screenCenter)
        {
            Vector3 aimPos = GetTargetPosition(target, targetSelection);
            if (renderer.WorldToScreen(aimPos, out Vector2 screenPos, screenWidth, screenHeight))
            {
                return Vector2.Distance(screenCenter, screenPos);
            }
            return -1; // 返回-1表示无法投射到屏幕
        }

        // 新方法：判断实体是否是普通感染者
        private bool IsCommonInfected(Entity entity)
        {
            if (string.IsNullOrEmpty(entity.modelName)) return false;
            return entity.modelName.ToLower().Contains("infected/common");
        }

        public void ExecuteMouseAimbot(Entity target, Entity localPlayer, AimbotTarget targetSelection, float smoothness, Renderer renderer, float screenWidth, float screenHeight)
        {
            Vector3 aimPos = GetTargetPosition(target, targetSelection);
            if (renderer.WorldToScreen(aimPos, out Vector2 targetScreenPos, screenWidth, screenHeight))
            {
                Vector2 screenCenter = new Vector2(screenWidth / 2, screenHeight / 2);
                float deltaX = targetScreenPos.X - screenCenter.X;
                float deltaY = targetScreenPos.Y - screenCenter.Y;

                float moveX = deltaX * smoothness;
                float moveY = deltaY * smoothness;

                NativeMethods.SimulateMouseMove((int)moveX, (int)moveY);
            }
        }

        private Vector3 GetTargetPosition(Entity target, AimbotTarget targetSelection)
        {
            if (targetSelection == AimbotTarget.Chest)
            {
                return new Vector3(target.origin.X, target.origin.Y, target.origin.Z + (target.viewOffset.Z * 0.7f));
            }
            return target.abs;
        }
    }
}