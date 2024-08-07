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

    public static Sprite DefaultButtonSprite; // Override these if u want custom ones
    public static Sprite OffSprite, OnSprite; // Override these if u want custom ones
    public static GameObject QuickMenu, ColpButtonGrp, ButtonGrp, ButtonGrpText;
    public static Transform Button, Tab, MenuPage, Slider;


    public static GameObject MMM, MMMpageTemplate, MMMCarouselPageTemplate, MMMCarouselButtonTemplate, MMMTabTemplate;
    public static GameObject MMCTgl, MMBtnGRP;

    internal static Transform UserInterface => VRCUiManager.field_Private_Static_VRCUiManager_0?.transform;

    static bool HasChecked;
    public static bool IsReady() {
        if (HasChecked) return true; // Check and good

        if ((QuickMenu = UserInterface.Find("Canvas_QuickMenu(Clone)")?.gameObject) == null) throw new NullReferenceException("QuickMenu Is Null!");

        if ((Button = QuickMenu?.transform.Find("CanvasGroup/Container/Window/QMParent/Menu_Here/ScrollRect/Viewport/VerticalLayoutGroup/Buttons_WorldActions/Button_NewInstance")) == null) throw new NullReferenceException("Button Is Null!");
        if ((MenuPage = QuickMenu?.transform.Find("CanvasGroup/Container/Window/QMParent/Menu_Dashboard")) == null) throw new NullReferenceException("MenuTab Is Null!");
        if ((Tab = QuickMenu?.transform.Find("CanvasGroup/Container/Window/Page_Buttons_QM/HorizontalLayoutGroup/Page_DevTools")) == null) throw new NullReferenceException("Tab Is Null!");
        if ((ButtonGrp = QuickMenu?.transform.Find("CanvasGroup/Container/Window/QMParent/Menu_Camera/Scrollrect/Viewport/VerticalLayoutGroup/Buttons")
            ?.gameObject) == null)
            throw new NullReferenceException("ButtonGrp Is Null!");
        if ((ButtonGrpText = QuickMenu?.transform.Find("CanvasGroup/Container/Window/QMParent/Menu_Camera/Scrollrect/Viewport/VerticalLayoutGroup/Header_H3")
            ?.gameObject) == null)
            throw new NullReferenceException("ButtonGrpText Is Null!");
        if ((ColpButtonGrp = QuickMenu?.transform.Find("CanvasGroup/Container/Window/QMParent/Menu_QM_GeneralSettings/Panel_QM_ScrollRect/Viewport/VerticalLayoutGroup/YourAvatar")?.gameObject) == null) 
            throw new NullReferenceException("ColpButtonGrp Is Null!");
        if ((Slider = QuickMenu?.transform.Find("CanvasGroup/Container/Window/QMParent/Menu_QM_GeneralSettings/Panel_QM_ScrollRect/Viewport/VerticalLayoutGroup/DisplayAndVisualAdjustments/QM_Settings_Panel/VerticalLayoutGroup/ScreenBrightness")) == null)
            throw new NullReferenceException("Slider Is Null!");

        if ((MMM = UserInterface.Find("Canvas_MainMenu(Clone)")?.gameObject) == null) throw new NullReferenceException("MainMenu Is Null!");
        if ((MMMpageTemplate = MMM?.transform.Find("Container/MMParent/Menu_MM_Profile")?.gameObject) == null) throw new NullReferenceException("Main Menu Template Is Null!");
        if ((MMMCarouselPageTemplate = MMM?.transform.Find("Container/MMParent/Menu_Settings")?.gameObject) == null) throw new NullReferenceException("Menu_Settings Is Null!");
        if ((MMMTabTemplate = MMM?.transform.Find("Container/PageButtons/HorizontalLayoutGroup/Page_Profile")?.gameObject) == null) throw new NullReferenceException("Main Menu Tab Is Null!");
        if ((MMMCarouselButtonTemplate = MMM?.transform.Find("Container/MMParent/Menu_Settings/Menu_MM_DynamicSidePanel/Panel_SectionList/ScrollRect_Navigation_Container/ScrollRect_Navigation/Viewport/VerticalLayoutGroup/Cell_MM_Audio & Voice")?.gameObject) == null)
            throw new NullReferenceException("MMMCarouselButtonTemplate Is Null!");
        if ((MMBtnGRP = MMM?.transform.Find("Container/MMParent/Menu_Settings/Menu_MM_DynamicSidePanel/Panel_SectionList/ScrollRect_Navigation_Container/ScrollRect_Content/Viewport/VerticalLayoutGroup/Debug/ManageCachedData")?.gameObject) == null)
            throw new NullReferenceException("MMBtnGRP Is Null!");
        if ((MMCTgl = MMM?.transform.Find("Container/MMParent/Menu_Settings/Menu_MM_DynamicSidePanel/Panel_SectionList/ScrollRect_Navigation_Container/ScrollRect_Content/Viewport/VerticalLayoutGroup/Mirrors/PersonalMirror/Settings_Panel_1/VerticalLayoutGroup/PersonalMirror")?.gameObject) == null)
            throw new NullReferenceException("MMCTgl Is Null!");
        
        GetToglSprites();
        return (HasChecked = true);
    }

    private static void GetToglSprites() {
        StyleEngine styleEngine = QuickMenu.GetComponent<StyleEngine>();
        var resources = styleEngine.field_Public_StyleResource_0.resources;
        for (int i = 0; i < resources.Count; i++) {
            if (resources[i]?.obj?.GetIl2CppType() == null) continue;
            if (resources[i].obj.GetIl2CppType() != Il2CppType.Of<Sprite>()) continue;

            if (resources[i].obj.name.Equals("Decline")) OffSprite = resources[i].obj.Cast<Sprite>();
            if (resources[i].obj.name.Equals("Checkmark")) OnSprite = resources[i].obj.Cast<Sprite>();
            if (OffSprite != null && OnSprite != null) break;
        }

        if (OffSprite == null) throw new NullReferenceException("OffSprite Is Null!");
        if (OnSprite == null) throw new NullReferenceException("OnSprite Is Null!");
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
