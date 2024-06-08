using System;
using UnityEngine;
using WorldAPI.ButtonAPI.Buttons;
using WorldAPI.ButtonAPI.MM.Carousel.Items;
using VRC.UI.Core.Styles;
using Il2CppInterop.Runtime;
using CLogs = HexedTools.HookUtils.Logs;

namespace WorldAPI;


internal static class Logs {
    internal static void Log(string message, ConsoleColor color = ConsoleColor.White) => CLogs.Log(message, Color: color, Name: "WorldAPI");

    internal static void Error(Exception e, string message) => Error(message, e);

    internal static void Error(string message, Exception e = null) => CLogs.Error(message, e, "WorldAPI");
}

public class APIBase {
    public class Events {
        public static Action<VRCToggle, bool> onVRCToggleValChange = new((er, str) => { });
        public static Action<CToggle, bool> onCToggleValChange = new((er, str) => { });
    }
    /// <summary>
    ///  Set this if u want to override what happens when a button/ tgl throws an error
    /// </summary>
    public static Action<Exception, string> ErrorCallBack { get; set; } = new Action<Exception, string>((er, str) => {
        Logs.Error($"The ButtonAPI had an Error At {str}", er);
    });

    private static int HasChecked = -1;

    public static Sprite DefaultButtonSprite; // Override these if u want custom ones
    public static Sprite OffSprite, OnSprite; // Override these if u want custom ones
    public static GameObject QuickMenu, ColpButtonGrp, ButtonGrp, ButtonGrpText;
    public static Transform Button, Tab, MenuPage, Slider;


    public static GameObject MMM, MMMpageTemplate, MMMCarouselPageTemplate, MMMCarouselButtonTemplate, MMMTabTemplate;
    public static GameObject MMCTgl, MMBtnGRP;

    internal static Transform UserInterface => VRCUiManager.field_Private_Static_VRCUiManager_0?.transform;


    public static bool IsReady() {
        if (HasChecked == 1) return true; // Check and good
        else if (HasChecked == 0) return false; // Check and Bad
        HasChecked = 1; // Return true



        if ((QuickMenu = UserInterface.Find("Canvas_QuickMenu(Clone)")?.gameObject) == null) {
            Logs.Error("QuickMenu Is Null!");
            return false; // return false
        }

        if ((MMM = UserInterface.Find("Canvas_MainMenu(Clone)")?.gameObject) == null) {
            Logs.Error("MainMenu Is Null!"); 
            return false;
        }
        if ((Button = QuickMenu?.transform.Find("CanvasGroup/Container/Window/QMParent/Menu_Dashboard/ScrollRect/Viewport/VerticalLayoutGroup/Buttons_QuickActions/Button_Respawn")) == null) {
            Logs.Error("Button Is Null!");
            return false;
        }
        if ((Slider = QuickMenu?.transform.Find("CanvasGroup/Container/Window/QMParent/Menu_QM_GeneralSettings/Panel_QM_ScrollRect/Viewport/VerticalLayoutGroup/DisplayAndVisualAdjustments/QM_Settings_Panel/VerticalLayoutGroup/ScreenBrightness")) == null) {
            Logs.Error("Slider Is Null!");
            return false; 
        }
        if ((MenuPage = QuickMenu?.transform.Find("CanvasGroup/Container/Window/QMParent/Menu_Dashboard")) == null) {
            Logs.Error("MenuTab Is Null!");
            return false; 
        }
        if ((Tab = QuickMenu?.transform.Find("CanvasGroup/Container/Window/Page_Buttons_QM/HorizontalLayoutGroup/Page_DevTools")) == null) {
            Logs.Error("Tab Is Null!");
            return false; 
        }
        if ((ButtonGrp = QuickMenu?.transform.Find("CanvasGroup/Container/Window/QMParent/Menu_Dashboard/ScrollRect/Viewport/VerticalLayoutGroup/Buttons_QuickActions")?.gameObject) == null) {
            Logs.Error("ButtonGrp Is Null!");
            return false; 
        }
        if ((ButtonGrpText = QuickMenu?.transform.Find("CanvasGroup/Container/Window/QMParent/Menu_Dashboard/ScrollRect/Viewport/VerticalLayoutGroup/Header_QuickActions")?.gameObject) == null) {
            Logs.Error("ButtonGrpText Is Null!");
            return false; 
        }
        if ((ColpButtonGrp = QuickMenu?.transform.Find("CanvasGroup/Container/Window/QMParent/Menu_QM_GeneralSettings/Panel_QM_ScrollRect/Viewport/VerticalLayoutGroup/YourAvatar")?.gameObject) == null) {
            Logs.Error("ColpButtonGrp Is Null!");
            return false; 
        }


        if ((MMMpageTemplate = MMM?.transform.Find("Container/MMParent/Menu_MM_Profile")?.gameObject) == null) {
            Logs.Error("Main Menu Template Is Null!");
            return false; 
        }

        if ((MMMCarouselPageTemplate = MMM?.transform.Find("Container/MMParent/Menu_Settings")?.gameObject) == null) {
            Logs.Error("Menu_Settings Is Null!");
            return false; 
        }
        if ((MMMTabTemplate = MMM?.transform.Find("Container/PageButtons/HorizontalLayoutGroup/Page_Profile")?.gameObject) == null) {
            Logs.Error("Main Menu Tab Is Null!");
            return false; 
        }
        if ((MMMCarouselButtonTemplate = MMM?.transform.Find("Container/MMParent/Menu_Settings/Menu_MM_DynamicSidePanel/Panel_SectionList/ScrollRect_Navigation_Container/ScrollRect_Navigation/Viewport/VerticalLayoutGroup/Cell_MM_Audio & Voice")?.gameObject) == null) {
            Logs.Error("MMMCarouselButtonTemplate Is Null!");
            return false; 
        }
        if ((MMBtnGRP = MMM?.transform.Find("Container/MMParent/Menu_Settings/Menu_MM_DynamicSidePanel/Panel_SectionList/ScrollRect_Navigation_Container/ScrollRect_Content/Viewport/VerticalLayoutGroup/Debug/ManageCachedData")?.gameObject) == null) {
            Logs.Error("MMBtnGRP Is Null!");
            return false; 
        }
        if ((MMCTgl = MMM?.transform.Find("Container/MMParent/Menu_Settings/Menu_MM_DynamicSidePanel/Panel_SectionList/ScrollRect_Navigation_Container/ScrollRect_Content/Viewport/VerticalLayoutGroup/Mirrors/PersonalMirror/Settings_Panel_1/VerticalLayoutGroup/PersonalMirror")?.gameObject) == null) {
            Logs.Error("MMCTgl Is Null!");
            return false; 
        }
        if (!GetToglSprites()) HasChecked = 0;
        return HasChecked != 0;
    }

    private static bool GetToglSprites() {
        StyleEngine styleEngine = QuickMenu.GetComponent<StyleEngine>();
        var resources = styleEngine.field_Public_StyleResource_0.resources;
        for (int i = 0; i < resources.Count; i++) {
            if (resources[i]?.obj?.GetIl2CppType() == null) continue;
            if (resources[i].obj.GetIl2CppType() != Il2CppType.Of<Sprite>()) continue;

            if (resources[i].obj.name.Equals("Decline")) OffSprite = resources[i].obj.Cast<Sprite>();
            if (resources[i].obj.name.Equals("Checkmark")) OnSprite = resources[i].obj.Cast<Sprite>();
            if (OffSprite != null && OnSprite != null) break;
        }

        if (OffSprite == null) {
            Logs.Error("OffSprite Is Null!");
            return false;
        }
        if (OnSprite == null) {
            Logs.Error("OnSprite Is Null!");
            return false;
        }
        return true;
    }

    internal static void SafelyInvolk(Action action, string name) { 
        try {
            action.Invoke();
        } catch (Exception e) {
            ErrorCallBack.Invoke(e, name);
        }
    }

    internal static void SafelyInvolk(bool state, Action<bool> action, string name) { 
        try {
            action.Invoke(state);
        } catch (Exception e) {
            ErrorCallBack.Invoke(e, name);
        }
    }
}
