using ColossalFramework;
using ColossalFramework.UI;
using LaneController.KianCommons.UI;
using UnityEngine;
using System.Globalization;
using LaneController.ModsCommon; // 引入本地化类

namespace LaneController.LifeCycle
{
    public static class LaneControllerSettings
    {
        public static SavedBool ShowToolTips;
        public static readonly SavedInputKey ActivationShortcut;

        // 保存不透明度设置 (默认 1.0，范围 0.1 到 1.0)
        public static SavedFloat UIOpacity;

        // 保存语言设置 (0: Auto, 1: English, 2: 简体中文)
        public static SavedInt LanguageOption;

        public static string SettingsFile => "LaneControllerMod";

        static LaneControllerSettings()
        {
            ShowToolTips = new SavedBool("ShowToolTips", SettingsFile, def: true, autoUpdate: true);
            ActivationShortcut = new SavedInputKey("ActivationShortcut", SettingsFile, SavedInputKey.Encode(KeyCode.L, control: false, shift: false, alt: false), autoUpdate: true);

            UIOpacity = new SavedFloat("UIOpacity", SettingsFile, 1.0f, autoUpdate: true);
            LanguageOption = new SavedInt("LanguageOption", SettingsFile, 0, autoUpdate: true);

            if (GameSettings.FindSettingsFileByName(SettingsFile) == null)
            {
                GameSettings.AddSettingsFile(new SettingsFile
                {
                    fileName = SettingsFile
                });
            }

            // 游戏启动时，应用保存的语言设置
            ApplyLanguage(LanguageOption.value);
        }

        public static void OnSettingsUI(UIHelper helper)
        {
            // 1. 语言下拉框
            string[] languages = { "跟随系统 (Auto)", "English", "简体中文 (zh-cn)" };
            helper.AddDropdown("Language / 语言", languages, LanguageOption.value, (index) =>
            {
                LanguageOption.value = index;
                ApplyLanguage(index);
            });

            // 2. 不透明度滑块
            UISlider opacitySlider = (UISlider)helper.AddSlider("UI Opacity / 界面不透明度", 0.1f, 1.0f, 0.05f, UIOpacity.value, (value) =>
            {
                UIOpacity.value = value;
                if (LaneController.UI.LaneControllerPanel.Instance != null)
                {
                    LaneController.UI.LaneControllerPanel.Instance.opacity = value;
                }
            });

            if (opacitySlider != null)
            {
                UILabel percentLabel = opacitySlider.AddUIComponent<UILabel>();
                percentLabel.textScale = 0.85f;
                percentLabel.textColor = Color.white;
                percentLabel.zOrder = 10;
                percentLabel.text = Mathf.RoundToInt(UIOpacity.value * 100f) + "%";
                percentLabel.relativePosition = new Vector3((opacitySlider.width - percentLabel.width) / 2f, 6f);

                opacitySlider.eventValueChanged += (component, val) =>
                {
                    percentLabel.text = Mathf.RoundToInt(val * 100f) + "%";
                    percentLabel.relativePosition = new Vector3((opacitySlider.width - percentLabel.width) / 2f, 6f);
                };
            }

            // 3. 原有的快捷键和开关
            helper.AddSavedToggle("Show Tooltips / 显示提示", ShowToolTips);
            UIKeymappingsPanel uIKeymappingsPanel = helper.AddKeymappingsPanel();
            uIKeymappingsPanel.AddKeymapping("Hotkey / 快捷键", ActivationShortcut);
        }

        // 应用语言的核心方法
        private static void ApplyLanguage(int index)
        {
            if (index == 0) // 跟随系统
                CommonLocalize.Culture = CultureInfo.InstalledUICulture;
            else if (index == 1) // 强制英文
                CommonLocalize.Culture = new CultureInfo("en");
            else if (index == 2) // 强制中文
                CommonLocalize.Culture = new CultureInfo("zh-cn");
        }
    }
}