using Kitchen;
using KitchenMods;
using Unity.Collections;
using Unity.Entities;

namespace FranchiseCardCount.Systems
{
    [UpdateAfter(typeof(CreateCardSelectors))]
    internal class UpdateCreateCardSelectors : FranchiseBuilderFirstFrameSystem, IModSystem
    {
        private EntityQuery Cards;

        protected override void Initialise()
        {
            base.Initialise();
            Cards = GetEntityQuery(typeof(CCardPedestal));
        }

        protected override void OnUpdate()
        {
            using var entities = Cards.ToEntityArray(Allocator.Temp);
            using var cards = Cards.ToComponentDataArray<CCardPedestal>(Allocator.Temp);

            for (int i = 0; i < entities.Length; i++)
            {
                var entity = entities[i];
                var card = cards[i];

                if (!card.IsForcedCard)
                {
                    card.IsSelected = false;
                    Set(entity, card);
                }
            }
        }
    }
}
