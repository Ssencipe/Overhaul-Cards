using ModsPlus;
using System;
using Photon.Pun;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnboundLib;
using UnboundLib.Cards;
using UnboundLib.Networking;
using UnityEngine;
using UnboundLib.Utils;
using ModdingUtils.Extensions;
using ModdingUtils.MonoBehaviours;

namespace OverhaulCards.MonoBehaviours
{
    public class EmbiggenEffect : CounterReversibleEffect, ISingletonEffect
    {
        public int CardAmount { get; set; } = 0;

        private float counter = 0;
        private bool modifiersActive = false;

        public void Activate()
        {
            counter += Cards.Embiggen.EmbiggenDuration * CardAmount;
        }

        public override CounterStatus UpdateCounter()
        {
            counter -= TimeHandler.deltaTime;
            if (!modifiersActive && counter > 0)
            {
                return CounterStatus.Apply;
            }
            else if (counter <= 0)
            {
                Reset();
                return CounterStatus.Remove;
            }
            return CounterStatus.Wait;
        }

        public override void UpdateEffects()
        {
            ColorEffect effect = player.gameObject.AddComponent<ColorEffect>();
            effect.SetColor(Color.blue);
            characterDataModifier.health_mult = 2f;
            characterDataModifier.maxHealth_mult = 2f;
            characterStatModifiersModifier.sizeMultiplier_mult = 2f;
        }

        public override void OnApply()
        {
            modifiersActive = true;
        }
        public override void OnRemove()
        {
            modifiersActive = false;
            UnityEngine.GameObject.Destroy(this.gameObject.GetOrAddComponent<ColorEffect>());
        }
        public override void Reset()
        {
            counter = 0;
            modifiersActive = false;
            UnityEngine.GameObject.Destroy(this.gameObject.GetOrAddComponent<ColorEffect>());
        }

        public override void OnStart()
        {
            applyImmediately = false;
            SetLivesToEffect(int.MaxValue);

            block.BlockAction += OnBlock;
        }
        public override void OnOnDestroy()
        {
            block.BlockAction -= OnBlock;
        }

        private void OnBlock(BlockTrigger.BlockTriggerType trigger)
        {
            if (trigger == BlockTrigger.BlockTriggerType.Default ||
                trigger == BlockTrigger.BlockTriggerType.Echo ||
                trigger == BlockTrigger.BlockTriggerType.ShieldCharge)
            {
                Activate();
            }
        }
    }
}