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
    class AmbushEffect : ReversibleEffect
    {
        public Player player;
        public Block block;
        public CharacterData data;
        private List<Player> enemies = new List<Player>();
        public override void OnStart()
        {
            block.BlockAction = (Action<BlockTrigger.BlockTriggerType>)Delegate.Combine(block.BlockAction, new Action<BlockTrigger.BlockTriggerType>(OnBlock));
        }
        private void OnBlock(BlockTrigger.BlockTriggerType trigger)
        {
            enemies = PlayerManager.instance.players.Where(p => p.teamID != player.teamID).ToList();
            foreach (Player enemy in enemies)
            {
                enemy.GetComponentInParent<PlayerCollision>().IgnoreWallForFrames(2);
                enemy.transform.root.transform.position = transform.root.transform.position + (enemy.transform.position - transform.position).normalized;
            }
        }
        public override void OnOnDestroy()
        {
            block.BlockAction = (Action<BlockTrigger.BlockTriggerType>)Delegate.Remove(block.BlockAction, new Action<BlockTrigger.BlockTriggerType>(OnBlock));
        }
    }
}