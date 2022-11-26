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
        public override void SetupCard(CardInfo cardInfo, Gun gun, ApplyCardStats cardStats, CharacterStatModifiers statModifiers)
        {

        }
        public override void OnAddCard(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {
            InvertEffect invertEffect = player.gameObject.AddComponent<InvertEffect>();
            block.objectsToSpawn.Add(InvertEffect.invertVisual);
            characterStats.movementSpeed *= 1.20f;
            block.cooldown += 0.5f;
        }
        public override void OnRemoveCard()
        {

        }

        protected override string GetTitle()
        {
            return "Invert";
        }
        protected override string GetDescription()
        {
            return "Teleports you to random opponent on block.";
        }
        protected override GameObject GetCardArt()
        {
            return null;
        }
        protected override CardInfo.Rarity GetRarity()
        {
            return CardInfo.Rarity.Rare;
        }
        protected override CardInfoStat[] GetStats()
        {
            return new CardInfoStat[]
            {
                new CardInfoStat()
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
            return CardThemeColor.CardThemeColorType.EvilPurple;
        }
        public override string GetModName()
        {
            return "OHC";
        }
    }
}
