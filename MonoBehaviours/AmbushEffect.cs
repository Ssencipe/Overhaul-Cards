﻿using ModsPlus;
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
    class AmbushEffect : ReversibleEffect
    {
        public Player player;

        public Block block;

        public CharacterData data;

        private Player target;

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
            target = PlayerManager.instance.GetOtherPlayer(GetComponentInParent<Player>());
            player.GetComponentInParent<PlayerCollision>().IgnoreWallForFrames(2);
            transform.root.transform.position = target.transform.position + (target.transform.position - transform.position).normalized;
        }
        public override void OnOnDestroy()
        {
            block.BlockAction = (Action<BlockTrigger.BlockTriggerType>)Delegate.Remove(block.BlockAction, new Action<BlockTrigger.BlockTriggerType>(OnBlock));
        }
    }
}