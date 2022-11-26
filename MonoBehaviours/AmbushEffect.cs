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

namespace OverhaulCards.MonoBehaviours
{
    class AmbushEffect : MonoBehaviour
    {
        public Player player;

        public Block block;

        public CharacterData data;

        private Action<BlockTrigger.BlockTriggerType> ambushAction;

        private Player target;

        private void Awake()
        {
            player = gameObject.GetComponent<Player>();
        }
        private void Start()
        {
            if ((bool)block)
            {
                ambushAction = GetDoBlockAction(player, block, data).Invoke;
                block.BlockAction = (Action<BlockTrigger.BlockTriggerType>)Delegate.Combine(block.BlockAction, ambushAction);
            }
        }
        public Action<BlockTrigger.BlockTriggerType> GetDoBlockAction(Player player, Block block, CharacterData data)
        {
            return delegate
            {
                target = PlayerManager.instance.GetOtherPlayer(GetComponentInParent<Player>());
                player.GetComponentInParent<PlayerCollision>().IgnoreWallForFrames(2);
                transform.root.transform.position = target.transform.position + (target.transform.position - transform.position).normalized;
            };
        }
        private void OnDestroy()
        {
            block.BlockAction = (Action<BlockTrigger.BlockTriggerType>)Delegate.Remove(block.BlockAction, ambushAction);
        }
    }
}