using HarmonyLib;
using Kitchen;
using KitchenData;
using KitchenLib.Utils;
using TMPro;
using UnityEngine;

namespace FranchiseCardCount.Patches
{
    [HarmonyPatch(typeof(CreateFranchiseTextView), "UpdateData")]
    internal class CreateFranchiseTextViewPatch
    {
        static bool Prefix(CreateFranchiseTextView.ViewData view_data, ref CreateFranchiseTextView __instance)
        {
            TextMeshPro Text = (TextMeshPro)ReflectionUtils.GetField<CreateFranchiseTextView>("Text").GetValue(__instance);
            Color Scrap = (Color)ReflectionUtils.GetField<CreateFranchiseTextView>("Scrap").GetValue(__instance);
            Color Error = (Color)ReflectionUtils.GetField<CreateFranchiseTextView>("Error").GetValue(__instance);
            Color Ready = (Color)ReflectionUtils.GetField<CreateFranchiseTextView>("Ready").GetValue(__instance);
            GlobalLocalisation Localisation = (GlobalLocalisation)ReflectionUtils.GetMethod<CreateFranchiseTextView>("get_Localisation").Invoke(__instance, new object[] { });

            if (view_data.IsScrapMode)
            {
                Text.text = Localisation["BUILDER_SCRAP", new object[1] { view_data.ExpValue }];
                Text.color = Scrap;
            }
            else if (view_data.CardCount > Mod.MaxCardCount)
            {
                Text.text = Localisation["BUILDER_REMOVE_SOME", new object[2] { view_data.CardCount, Mod.MaxCardCount }];
                Text.color = Error;
            }
            else if (view_data.CardCount < Mod.MinCardCount)
            {
                Text.text = Localisation["BUILDER_ADD_SOME", new object[2] { view_data.CardCount, Mod.MinCardCount }];
                Text.color = Error;
            }
            else
            {
                Text.text = Localisation["BUILDER_CREATE_FRANCHISE"];
                Text.color = Ready;
            }

            return false;
        }
    }
}
