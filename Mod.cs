using Kitchen;
using KitchenLib;
using KitchenLib.Event;
using KitchenLib.Preferences;
using KitchenMods;
using System.Reflection;
using UnityEngine;

namespace FranchiseCardCount
{
    internal class Mod : BaseMod, IModSystem
    {
        public const string MOD_GUID = "io.zkz.plateup.franchisecardcount";
        public const string MOD_NAME = "Franchise Card Count";
        public const string MOD_VERSION = "0.1.0";
        public const string MOD_AUTHOR = "ZekNikZ";
        public const string MOD_GAMEVERSION = ">=1.1.4";

        public static PreferenceManager PreferenceManager = new PreferenceManager(MOD_GUID);

        // Min card count
        public static PreferenceInt PrefMinCardCount = PreferenceManager.RegisterPreference(new PreferenceInt("MinCardCount", 3));
        public static int MinCardCount => PrefMinCardCount.Get();

        // Max card count
        public static PreferenceBool PrefMaxCardCount = PreferenceManager.RegisterPreference(new PreferenceBool("MaxCardCount", false));
        public static int MaxCardCount => PrefMaxCardCount.Get() ? MinCardCount : int.MaxValue;

        public Mod() : base(MOD_GUID, MOD_NAME, MOD_AUTHOR, MOD_VERSION, MOD_GAMEVERSION, Assembly.GetExecutingAssembly()) { }

        protected override void OnInitialise()
        {
            LogWarning($"{MOD_GUID} v{MOD_VERSION} in use!");
        }

        protected override void OnUpdate()
        {
        }

        protected override void OnPostActivate(KitchenMods.Mod mod)
        {
            PreferenceManager.Load();

            ModsPreferencesMenu<MainMenuAction>.RegisterMenu(MOD_NAME, typeof(PreferencesMenu<MainMenuAction>), typeof(MainMenuAction));
            ModsPreferencesMenu<PauseMenuAction>.RegisterMenu(MOD_NAME, typeof(PreferencesMenu<PauseMenuAction>), typeof(PauseMenuAction));
            Events.PreferenceMenu_MainMenu_CreateSubmenusEvent += (s, args) =>
            {
                args.Menus.Add(typeof(PreferencesMenu<MainMenuAction>), new PreferencesMenu<MainMenuAction>(args.Container, args.Module_list));
            };
            Events.PreferenceMenu_PauseMenu_CreateSubmenusEvent += (s, args) =>
            {
                args.Menus.Add(typeof(PreferencesMenu<PauseMenuAction>), new PreferencesMenu<PauseMenuAction>(args.Container, args.Module_list));
            };
        }
        #region Logging
        public static void LogInfo(string _log) { Debug.Log($"[{MOD_NAME}] " + _log); }
        public static void LogWarning(string _log) { Debug.LogWarning($"[{MOD_NAME}] " + _log); }
        public static void LogError(string _log) { Debug.LogError($"[{MOD_NAME}] " + _log); }
        public static void LogInfo(object _log) { LogInfo(_log.ToString()); }
        public static void LogWarning(object _log) { LogWarning(_log.ToString()); }
        public static void LogError(object _log) { LogError(_log.ToString()); }
        #endregion
    }
}
