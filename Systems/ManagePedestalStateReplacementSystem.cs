using HarmonyLib;
using Kitchen;
using KitchenData;
using KitchenMods;
using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using Unity.Entities;

namespace FranchiseCardCount.Systems
{
    [UpdateAfter(typeof(ManagePedestalState))]
    internal class ManagePedestalStateReplacementSystem : FranchiseBuilderSystem, IModSystem
    {
        [HarmonyPatch(typeof(ManagePedestalState), "OnUpdate")]
        [HarmonyPrefix]
        static bool DisableVanillaSystem()
        {
            return false;
        }

        private EntityQuery Pedestals;

        protected override void Initialise()
        {
            base.Initialise();
            Pedestals = GetEntityQuery(typeof(CCardPedestal), typeof(CItemHolder));
        }

        protected override void OnUpdate()
        {
            NativeArray<Entity> nativeArray = Pedestals.ToEntityArray(Allocator.Temp);
            NativeArray<CCardPedestal> nativeArray2 = Pedestals.ToComponentDataArray<CCardPedestal>(Allocator.Temp);
            NativeArray<CEndgameUnlock> nativeArray3 = GetBuffer<CEndgameUnlock>(GetSingletonEntity<SEndgameStats>()).ToNativeArray(Allocator.Temp);
            int num = nativeArray2.Count((CCardPedestal p) => p.IsSelected && !p.IsForcedCard);
            CCardPedestal[] array = nativeArray2.ToArray();
            for (int i = 0; i < array.Length; i++)
            {
                array[i].BlockedBy = 0;
                array[i].UntoggleableTooManyCards = false;
            }
            for (int j = 0; j < nativeArray.Length; j++)
            {
                Entity entity = nativeArray[j];
                CCardPedestal component = array[j];
                if (num >= Mod.MaxCardCount /* 3 */)
                {
                    component.UntoggleableTooManyCards = !component.IsSelected;
                }
                else
                {
                    component.UntoggleableTooManyCards = false;
                }
                if (!component.IsSelected)
                {
                    if (base.Data.TryGet<Unlock>(component.CardID, out var output, warn_if_fail: true))
                    {
                        List<Unlock> requires = output.Requires;
                        component.BlockedBy = 0;
                        foreach (Unlock item in requires)
                        {
                            if ((from p in nativeArray3
                                 where p.FromFranchise
                                 select p into c
                                 select c.UnlockID).Contains(item.ID) || (from p in nativeArray2
                                                                          where p.IsSelected
                                                                          select p into c
                                                                          select c.CardID).Contains(item.ID))
                            {
                                continue;
                            }
                            component.BlockedBy = item.ID;
                            break;
                        }
                    }
                }
                else
                {
                    foreach (CCardPedestal item2 in nativeArray2)
                    {
                        if (item2.IsSelected && base.Data.TryGet<Unlock>(item2.CardID, out var output2, warn_if_fail: true))
                        {
                            List<Unlock> requires2 = output2.Requires;
                            if (requires2.Select((Unlock p) => p.ID).Contains(component.CardID))
                            {
                                component.BlockedBy = item2.CardID;
                            }
                        }
                    }
                }
                SetComponent(entity, component);
            }
            if (TryGetSingleton<SClaimExpSelector>(out var value))
            {
                value.ExpValue = nativeArray3.Sum((CEndgameUnlock c) => (int)(base.Data.TryGet<ICard>(c.UnlockID, out var output3, warn_if_fail: true) ? output3.ExpReward : Unlock.RewardLevel.None));
                SetSingleton(value);
            }
            if (TryGetSingleton<SCreateFranchiseSelector>(out var value2))
            {
                value2.CardCount = num;
                if (num >= Mod.MinCardCount && num <= Mod.MaxCardCount /* num == 3 */)
                {
                    base.EntityManager.AddComponent<CSelectorEnabled>(value2.Selector);
                }
                else
                {
                    base.EntityManager.RemoveComponent<CSelectorEnabled>(value2.Selector);
                }
                SetSingleton(value2);
            }
            nativeArray.Dispose();
            nativeArray2.Dispose();
            nativeArray3.Dispose();
        }
    }
}
