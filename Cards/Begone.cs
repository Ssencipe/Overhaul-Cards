using ModsPlus;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnboundLib;
using UnboundLib.Cards;
using UnboundLib.Utils;
using UnboundLib.Networking;
using UnityEngine;
using System.Reflection;
using OverhaulCards.MonoBehaviours;
using OverhaulCards.Extensions;
using ModdingUtils.Extensions;
using ModdingUtils.RoundsEffects;

namespace OverhaulCards.Cards
{
    class Begone : CustomCard
    {
        private static float rangePerCard = 5f;
        private static float durationPerCard = 2f;

        public override void SetupCard(CardInfo cardInfo, Gun gun, ApplyCardStats cardStats, CharacterStatModifiers statModifiers, Block block)
        {
        }

        public override void OnAddCard(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {
            BegoneEffect gravityMono = player.gameObject.AddComponent<BegoneEffect>();
            block.objectsToSpawn.Add(BegoneEffect.begoneVisual);
            characterStats.health *= 1.20f;
            block.cdAdd += 1f;
            block.GetAdditionalData().begoneRange += Begone.rangePerCard;
        }

        public override void OnRemoveCard(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {

        }

        protected override string GetTitle()
        {
            return "Begone";
        }

        protected override string GetDescription()
        {
            return "Blocking teleports nearby enemies to random locations.";
        }

        protected override GameObject GetCardArt()
        {
            return null;
        }

        protected override CardInfo.Rarity GetRarity()
        {
            return CardInfo.Rarity.Uncommon;
        }

        protected override CardInfoStat[] GetStats()
        {
            return new CardInfoStat[]
            {
                new CardInfoStat
                {
                    positive = true,
                    stat = "Health",
                    amount = "+20%",
                    simepleAmount = CardInfoStat.SimpleAmount.aLittleBitOf
                },
                new CardInfoStat
                {
                    positive = false,
                    stat = "Block Cooldown",
                    amount = "+1s",
                    simepleAmount = CardInfoStat.SimpleAmount.notAssigned
                },
            };
        }

        protected override CardThemeColor.CardThemeColorType GetTheme()
        {
            return CardThemeColor.CardThemeColorType.MagicPink;
        }

        public override string GetModName()
        {
            return "OHC";
        }
    }
}