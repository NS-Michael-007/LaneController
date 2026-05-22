using System.Globalization;
using System.Reflection;
using System.Resources;

namespace LaneController.ModsCommon
{
    public class CommonLocalize
    {
        // 必须与 csproj 里的 LogicalName 完全一致
        private static readonly ResourceManager _resourceManager =
            new ResourceManager("LaneController.ModsCommon.CommonLocalize", Assembly.GetExecutingAssembly());

        private static CultureInfo resourceCulture;

        public static CultureInfo Culture
        {
            get { return resourceCulture; }
            set { resourceCulture = value; }
        }

        private static string GetString(string name)
        {
            try
            {
                return _resourceManager.GetString(name, resourceCulture ?? CultureInfo.CurrentUICulture) ?? name;
            }
            catch
            {
                return name;
            }
        }

        // ---- 基础词条 ----
        public static string ControlPoint1 => GetString("ControlPoint1");
        public static string ControlPoint2 => GetString("ControlPoint2");
        public static string EndPoint => GetString("EndPoint");
        public static string FieldPanel_ScrollWheel => GetString("FieldPanel_ScrollWheel");
        public static string HorizontalShift => GetString("HorizontalShift");
        public static string LaneEditor => GetString("LaneEditor");
        public static string MessageBox_Cancel => GetString("MessageBox_Cancel");
        public static string MessageBox_DontShowAgain => GetString("MessageBox_DontShowAgain");
        public static string MessageBox_MoreInfo => GetString("MessageBox_MoreInfo");
        public static string MessageBox_No => GetString("MessageBox_No");
        public static string MessageBox_OK => GetString("MessageBox_OK");
        public static string MessageBox_Yes => GetString("MessageBox_Yes");
        public static string Mod_Status => GetString("Mod_Status");
        public static string Panel_Additional => GetString("Panel_Additional");
        public static string SelectionMessage => GetString("SelectionMessage");
        public static string Settings_TranslationDescription => GetString("Settings_TranslationDescription");
        public static string StartPoint => GetString("StartPoint");
        public static string VerticalShift => GetString("VerticalShift");

        // ---- 热键与面板词条 ----
        public static string Key_Control => GetString("Key_Control");
        public static string Key_Alt => GetString("Key_Alt");
        public static string Key_Shift => GetString("Key_Shift");
        public static string Key_Enter => GetString("Key_Enter");
        public static string Key_Tab => GetString("Key_Tab");
        public static string Settings_PressAnyKey => GetString("Settings_PressAnyKey");
        public static string ListPanel_ScrollWheel => GetString("ListPanel_ScrollWheel");

        // ---- 右键菜单与提示语（涵盖所有可能的命名，防止报错） ----
        public static string ResetControlPoints => GetString("ResetControlPoints");
        public static string Header_ResetControlPoints => GetString("ResetControlPoints");

        public static string ApplyBetweenIntersections => GetString("ApplyBetweenIntersections");
        public static string Header_ApplyBetweenIntersections => GetString("ApplyBetweenIntersections");

        public static string ApplyToWholeStreet => GetString("ApplyToWholeStreet");
        public static string Header_ApplyToWholeStreet => GetString("ApplyToWholeStreet");

        public static string HoldControlForVerticalShift => GetString("HoldControlForVerticalShift");
        public static string Mod_HoldControlForVerticalShift => GetString("HoldControlForVerticalShift");
    }
}