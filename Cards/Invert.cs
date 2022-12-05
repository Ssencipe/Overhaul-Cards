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
    class Invert : CustomCard
    {
        private static float rangePerCard = 5f;
        private static float durationPerCard = 2f;

        public override void SetupCard(CardInfo cardInfo, Gun gun, ApplyCardStats cardStats, CharacterStatModifiers statModifiers, Block block)
        {
        }

        public override void OnAddCard(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {
            InvertEffect gravityMono = player.gameObject.AddComponent<InvertEffect>();
            block.objectsToSpawn.Add(InvertEffect.invertVisual);
            block.cdAdd += 1f;
            block.GetAdditionalData().invertRange += Invert.rangePerCard;
            block.GetAdditionalData().invertDuration += Invert.durationPerCard;
        }

        public override void OnRemoveCard(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {

        }

        protected override string GetTitle()
        {
            return "Invert";
        }

        protected override string GetDescription()
        {
            return "Blocking inverts nearby enemies' gravity and slows them temporarily.";
        }

        protected override GameObject GetCardArt()
        {
            return null;
        }

        protected override CardInfo.Rarity GetRarity()
        {
            return CardInfo.Rarity.Common;
        }

        protected override CardInfoStat[] GetStats()
        {
            return new CardInfoStat[]
            {
                new CardInfoStat
                {
                    positive = false,
                    stat = "Block Cooldown",
                    amount = "+1s",
                    simepleAmount = CardInfoStat.SimpleAmount.notAssigned
                },
                new CardInfoStat
                {
                    positive = false,
                    stat = "Ability Cooldown",
                    amount = "0.1s",
                    simepleAmount = CardInfoStat.SimpleAmount.notAssigned
                }
            };
        }

        protected override CardThemeColor.CardThemeColorType GetTheme()
        {
            return CardThemeColor.CardThemeColorType.EvilPurple;
        }

        public override string GetModName()
        {
            return "OHC";
        }
    }
}