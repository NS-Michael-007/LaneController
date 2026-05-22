using System;
using CitiesHarmony.API;
using LaneController.KianCommons.Utils;
using LaneController.Manager;
using LaneController.Tool;

namespace LaneController.LifeCycle
{
    public class LaneControllerMod : LifeCycleBase
    {
        public const string HARMONY_ID = "cs.lanecontroller";

        public override string ModName => "Lane Controller";
        public override string Description => "Adjust lane paths.";

        public override void Start()
        {
            HarmonyHelper.DoOnHarmonyReady(delegate
            {
                HarmonyUtil.InstallHarmony(HARMONY_ID);
            });
        }

        public override void OnSettingsUI(UIHelper helper)
        {
            try { LaneControllerSettings.OnSettingsUI(helper); } catch (Exception ex) { ex.Log(); }
        }

        public override void Load()
        {
            try
            {
                LaneControllerManager.Ensure();
                if (LaneControllerTool.Instance == null)
                {
                    LaneControllerTool.Create();
                }
            }
            catch (Exception ex)
            {
                Log.Info("[LaneController] 初始化工具失败，请检查资源和本地化配置！");
                ex.Log();
            }
        }

        public override void HotReload()
        {
            try
            {
                SerializableDataExtension.Load();
                Load();
            }
            catch (Exception ex) { ex.Log(); }
        }

        public override void UnLoad()
        {
            try
            {
                LaneControllerTool.Remove();
                LaneControllerManager.Release();
            }
            catch (Exception ex) { ex.Log(); }
        }

        public override void End()
        {
            try { HarmonyUtil.UninstallHarmony(HARMONY_ID); } catch (Exception ex) { ex.Log(); }
        }
    }
}