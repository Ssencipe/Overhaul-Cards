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
using ModdingUtils.MonoBehaviours;
using OverhaulCards.Extensions;
using System.Collections.ObjectModel;

namespace OverhaulCards.MonoBehaviours
{
    public class BoxedInEffect : MonoBehaviour
    {
        [PunRPC]
        public void RPCA_FixBox()
        {
            var oneObj = MapManager.instance.currentMap.Map.gameObject.transform.GetChild(MapManager.instance.currentMap.Map.gameObject.transform.childCount - 1);
            var oneRigid = oneObj.GetComponent<Rigidbody2D>();
            oneRigid.isKinematic = false;
            oneRigid.simulated = true;
        }

        [PunRPC]
        public void RPCA_BigBox()
        {
            var oneObj = MapManager.instance.currentMap.Map.gameObject.transform.GetChild(MapManager.instance.currentMap.Map.gameObject.transform.childCount - 1);
            oneObj.transform.localScale *= 1.25f;
        }
    }
}