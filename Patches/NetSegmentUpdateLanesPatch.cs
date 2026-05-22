using HarmonyLib;
using LaneController.Manager;
using UnityEngine;

namespace LaneController.Patches
{
    [HarmonyPatch(typeof(NetSegment), "UpdateLanes")]
    public static class NetSegmentUpdateLanesPatch
    {
        // 使用 __instance 也可以，但直接用 segmentID 最稳妥
        public static void Postfix(ushort segmentID)
        {
            try
            {
                if (LaneControllerManager.Instance != null && segmentID != 0)
                {
                    LaneControllerManager.Instance.UpateLanes(segmentID);
                }
            }
            catch (System.Exception ex)
            {
                // 捕获异常，防止游戏卡死，同时输出到日志以便调试
                Debug.LogError($"[LaneController] Patch Error on Segment {segmentID}: {ex}");
            }
        }
    }
}