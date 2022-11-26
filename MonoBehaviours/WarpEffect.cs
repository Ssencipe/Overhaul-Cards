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

namespace OverhaulCards.Cards
{
    class WarpEffect : MonoBehaviour
    {
        public Player player;

        public Block block;

        public CharacterData data;

        private Action<BlockTrigger.BlockTriggerType> warpAction;

        private bool init = false;

        private Vector3 initialPos;

        private void Awake()
        {
            player = gameObject.GetComponent<Player>();
        }
        private void Start()
        {
            if ((bool)block)
            {
                warpAction = GetDoBlockAction(player, block, data).Invoke;
                block.BlockAction = (Action<BlockTrigger.BlockTriggerType>)Delegate.Combine(block.BlockAction, warpAction);
            }
        }
        public Action<BlockTrigger.BlockTriggerType> GetDoBlockAction(Player player, Block block, CharacterData data)
        {
            return delegate
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
        private void OnDestroy()
        {
            block.BlockAction = (Action<BlockTrigger.BlockTriggerType>)Delegate.Remove(block.BlockAction, warpAction);
        }
    }
}