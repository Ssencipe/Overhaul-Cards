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
    class WarpEffect : ReversibleEffect
    {
        public Player player;

        public Block block;

        public CharacterData data;

        private bool init = false;

        private Vector3 initialPos;

        private void Awake()
        {
            player = gameObject.GetComponent<Player>();
        }
        public override void OnStart()
        {
            block.BlockAction = (Action<BlockTrigger.BlockTriggerType>)Delegate.Combine(block.BlockAction, new Action<BlockTrigger.BlockTriggerType>(OnBlock));
            SetLivesToEffect(int.MaxValue);
        }
        private void OnBlock(BlockTrigger.BlockTriggerType trigger)
        {
            {
                if (init == false)
                {
                    initialPos.x = player.transform.position.x;
                    initialPos.y = player.transform.position.y;
                    initialPos.x += UnityEngine.Random.Range(-15f, 15f);
                    initialPos.y += UnityEngine.Random.Range(-5f, 10f);
                    player.GetComponentInParent<PlayerCollision>().IgnoreWallForFrames(2);
                    player.transform.position = new Vector3(initialPos.x, initialPos.y, player.transform.position.z);
                }
            };
        }
        public override void OnOnDestroy()
        {
            block.BlockAction = (Action<BlockTrigger.BlockTriggerType>)Delegate.Remove(block.BlockAction, new Action<BlockTrigger.BlockTriggerType>(OnBlock));
        }
    }
}