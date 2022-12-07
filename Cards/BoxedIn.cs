using ModsPlus;
using OverhaulCards.MonoBehaviours;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnboundLib;
using UnboundLib.Cards;
using UnityEngine;
using UnityEngine.Events;
using OverhaulCards.Extensions;
using Photon.Pun;


namespace OverhaulCards.Cards
{
    class BoxedIn : CustomCard
    {
        private GameObject reclyObj;
        private Vector3 boxPos;

        protected override string GetTitle()
        {
            return "Boxed In";
        }

        protected override string GetDescription()
        {
            return "Blocking spawns a box at your cursor.";
        }

        public override void OnAddCard(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {
            block.cdAdd += 0.5f;
            characterStats.health *= 1.25f;

            player.gameObject.AddComponent<BoxedInEffect>();
            reclyObj = new GameObject("recycling");
            reclyObj.AddComponent<MonoBehaviour>();
            var jump = reclyObj.AddComponent<PlayerDoJump>();
            jump.multiplier = 1;
            var trigger = reclyObj.AddComponent<BlockTrigger>();
            trigger.blockRechargeEvent = new UnityEvent();
            trigger.successfulBlockEvent = new UnityEvent();
            trigger.triggerSuperFirstBlock = new UnityEvent();
            trigger.triggerFirstBlockThatDelaysOthers = new UnityEvent();
            trigger.triggerEventEarly = new UnityEvent();
            trigger.triggerEvent = new UnityEvent();
            trigger.triggerEvent.AddListener(() =>
            {
                if (jump.GetComponentInParent<CharacterStatModifiers>().GetAdditionalData().timeSinceLastBlockBox > 1.5f)
                {
                    jump.GetComponentInParent<CharacterStatModifiers>().GetAdditionalData().timeSinceLastBlockBox = 0;
                    if (PhotonNetwork.IsMasterClient)
                    {
                        jump.ExecuteAfterSeconds(0.08f, () =>
                        {
                            boxPos.x = MainCam.instance.cam.ScreenToWorldPoint(Input.mousePosition).x;
                            boxPos.y = MainCam.instance.cam.ScreenToWorldPoint(Input.mousePosition).y;
                            PhotonNetwork.Instantiate("4 map objects/Box_Destructible", boxPos, Quaternion.identity);
                        });
                        jump.ExecuteAfterSeconds(0.15f, () =>
                        {
                            var parent = jump.transform.parent;
                            parent.GetComponent<PhotonView>().RPC("RPCA_FixBox", RpcTarget.All);
                            parent.GetComponent<PhotonView>().RPC("RPCA_BigBox", RpcTarget.All);
                        });
                    }
                    // var rem = box.AddComponent<RemoveAfterSeconds>();
                    // rem.seconds = 4;

                }
            });
            reclyObj.transform.SetParent(player.gameObject.transform);
        }

        public override void SetupCard(CardInfo cardInfo, Gun gun, ApplyCardStats cardStats, CharacterStatModifiers statModifiers)
        {
            cardInfo.allowMultiple = false;
        }

        protected override CardInfoStat[] GetStats()
        {
            return new[]
            {
                new CardInfoStat
                {
                    amount = "+25%",
                    positive = true,
                    simepleAmount = CardInfoStat.SimpleAmount.notAssigned,
                    stat = "Health"
                },
                new CardInfoStat
                {
                    amount = "+0.5s",
                    positive = true,
                    simepleAmount = CardInfoStat.SimpleAmount.notAssigned,
                    stat = "Block Cooldown"
                }
            };
        }

        protected override CardInfo.Rarity GetRarity()
        {
            return CardInfo.Rarity.Uncommon;
        }

        protected override GameObject GetCardArt()
        {
            return null;
        }

        protected override CardThemeColor.CardThemeColorType GetTheme()
        {
            return CardThemeColor.CardThemeColorType.TechWhite;
        }

        public override string GetModName()
        {
            return "OHC";
        }

        public override void OnRemoveCard()
        {
            DestroyImmediate(reclyObj);
        }

    }
}