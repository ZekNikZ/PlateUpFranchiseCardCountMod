using Kitchen.Modules;
using Kitchen;
using KitchenLib;
using System.Collections.Generic;
using UnityEngine;

namespace FranchiseCardCount
{
    internal class PreferencesMenu<T> : KLMenu<T>
    {
        public PreferencesMenu(Transform container, ModuleList moduleList) : base(container, moduleList)
        {
        }

        public override void Setup(int player_id)
        {
            AddLabel("Settings Profile");
            AddProfileSelector(Mod.MOD_GUID, newProfile =>
            {
                Mod.PreferenceManager.Save();
                Mod.PreferenceManager.SetProfile(newProfile);
                Mod.PreferenceManager.Load();
            }, Mod.PreferenceManager);

            AddLabel("Minimum Card Count");
            Add(new Option<int>(
                new List<int>
                {
                    0, 1, 2, 3, 4, 5
                },
                Mod.PrefMinCardCount.Get(),
                new List<string>
                {
                    "0", "1", "2", "3", "4", "5"
                }
            )).OnChanged += delegate (object _, int newVal)
            {
                Mod.PrefMinCardCount.Set(newVal);
            };
            AddInfo("The minimum amount of cards that must be selected on the franchise screen.");

            AddLabel("Maximum Card Count");
            Add(new Option<bool>(
                new List<bool>
                {
                    false, true
                },
                Mod.PrefMaxCardCount.Get(),
                new List<string>
                {
                    "Unlimited", "Same as Minimum"
                }
            )).OnChanged += delegate (object _, bool newVal)
            {
                Mod.PrefMaxCardCount.Set(newVal);
            };
            AddInfo("The maximum amount of cards that can be selected on the franchise screen.");

            AddButton("Apply", delegate
            {
                Mod.PreferenceManager.Save();
                RequestPreviousMenu();
            });

            AddButton(Localisation["MENU_BACK_SETTINGS"], delegate
            {
                RequestPreviousMenu();
            });
        }
    }
}
