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
    public class InvertEffect : MonoBehaviour
    {
        private readonly float maxDistance = 8f;
        public Block block;
        public Player player;
        public CharacterData data;
        public Gun gun;
        private Action<BlockTrigger.BlockTriggerType> gravy;
        private Action<BlockTrigger.BlockTriggerType> basic;
        private static GameObject invertvisual = null;
        private readonly float updateDelay = 0.1f;
        private readonly float effectCooldown = 0.1f;
        private float startTime;
        private float timeOfLastEffect;
        private bool canTrigger;
        private bool hasTriggered;
        public int numcheck = 0;

        private void Start()
        {
            this.player = this.GetComponent<Player>();
            this.block = this.GetComponent<Block>();
            this.data = this.GetComponent<CharacterData>();
            this.gun = this.GetComponent<Gun>();
            this.ResetEffectTimer();
            this.ResetTimer();
            this.canTrigger = true;
            this.hasTriggered = false;
            this.basic = this.block.BlockAction;

            bool flag = this.block;
            if (flag)
            {
                this.gravy = new Action<BlockTrigger.BlockTriggerType>(this.GetDoBlockAction(this.player, this.block).Invoke);
                this.block.BlockAction = (Action<BlockTrigger.BlockTriggerType>)Delegate.Combine(this.block.BlockAction, this.gravy);
            }
        }

        public void Destroy()
        {
            UnityEngine.Object.Destroy(this);
        }

        public void OnDestroy()
        {
            this.block.BlockAction = this.basic;
        }

        private void Update()
        {
            if (Time.time >= this.startTime + this.updateDelay)
            {
                int i = 0;
                while (i <= this.player.data.currentCards.Count - 1)
                {
                    if (this.player.data.currentCards[i].cardName == "Invert")
                    {
                        this.numcheck += 1;
                    }

                    i++;
                }

                if (numcheck > 0)
                {
                    this.ResetTimer();

                    if (Time.time >= this.timeOfLastEffect + this.effectCooldown)
                    {
                        this.canTrigger = true;
                    }

                    else
                    {
                        if (block.objectsToSpawn.Contains(InvertEffect.invertVisual))
                        {
                            block.objectsToSpawn.Remove(InvertEffect.invertVisual);
                        }

                    }
                }

                else
                {
                    UnityEngine.Object.Destroy(this);
                }
            }

        }


        public Action<BlockTrigger.BlockTriggerType> GetDoBlockAction(Player player, Block block)
        {
            return delegate (BlockTrigger.BlockTriggerType trigger)
            {
                bool flag = trigger != BlockTrigger.BlockTriggerType.None;
                if (flag)
                {
                    Vector2 a = block.transform.position;
                    Player[] array = PlayerManager.instance.players.ToArray();
                    for (int i = 0; i < array.Length; i++)
                    {
                        bool flag2 = array[i].playerID == player.playerID;
                        if (!flag2)
                        {
                            bool flag3 = Vector2.Distance(a, array[i].transform.position) < this.maxDistance && PlayerManager.instance.CanSeePlayer(player.transform.position, array[i]).canSee;
                            if (flag3)
                            {
                                HealthHandler component = array[i].transform.GetComponent<HealthHandler>();
                                if (this.canTrigger)
                                {
                                    component.TakeDamage(15f * Vector2.down, array[i].transform.position, this.player.data.weaponHandler.gameObject, this.player, true, true);
                                    this.player.data.stats.gravity = -2f;
                                    this.hasTriggered = true;
                                }
                            }
                        }
                    }
                    if (this.hasTriggered)
                    {
                        this.hasTriggered = false;
                        this.canTrigger = false;
                        this.ResetEffectTimer();
                    }
                }
            };
        }

        private void ResetTimer()
        {
            this.startTime = Time.time;
            numcheck = 0;
        }
        private void ResetEffectTimer()
        {
            this.timeOfLastEffect = Time.time;
        }

        public static GameObject invertVisual
        {
            get
            {
                bool flag = InvertEffect.invertvisual != null;
                GameObject result;
                if (flag)
                {
                    result = InvertEffect.invertvisual;
                }
                else
                {
                    List<CardInfo> first = ((ObservableCollection<CardInfo>)typeof(CardManager).GetField("activeCards", BindingFlags.Static | BindingFlags.NonPublic).GetValue(null)).ToList<CardInfo>();
                    List<CardInfo> second = (List<CardInfo>)typeof(CardManager).GetField("inactiveCards", BindingFlags.Static | BindingFlags.NonPublic).GetValue(null);
                    List<CardInfo> source = first.Concat(second).ToList<CardInfo>();
                    GameObject original = (from card in source
                                           where card.cardName.ToLower() == "overpower"
                                           select card).First<CardInfo>().GetComponent<CharacterStatModifiers>().AddObjectToPlayer.GetComponent<SpawnObjects>().objectToSpawn[0];
                    InvertEffect.invertvisual = UnityEngine.Object.Instantiate<GameObject>(original, new Vector3(0f, 100000f, 0f), Quaternion.identity);
                    InvertEffect.invertvisual.name = "E_Invert";
                    UnityEngine.Object.DontDestroyOnLoad(InvertEffect.invertvisual);
                    //foreach (ParticleSystem particleSystem in InvertEffect.invertvisual.GetComponentsInChildren<ParticleSystem>())
                    //{
                    //    particleSystem.startColor = Color.magenta;
                    //}
                    InvertEffect.invertvisual.transform.GetChild(1).GetComponent<LineEffect>().colorOverTime.colorKeys = new GradientColorKey[]
                    {
                        new GradientColorKey(Color.magenta, 0f)
                    };
                    UnityEngine.Object.Destroy(InvertEffect.invertvisual.transform.GetChild(2).gameObject);
                    InvertEffect.invertvisual.transform.GetChild(1).GetComponent<LineEffect>().offsetMultiplier = 0f;
                    InvertEffect.invertvisual.transform.GetChild(1).GetComponent<LineEffect>().playOnAwake = true;
                    UnityEngine.Object.Destroy(InvertEffect.invertvisual.GetComponent<FollowPlayer>());
                    InvertEffect.invertvisual.GetComponent<DelayEvent>().time = 0f;
                    //UnityEngine.Object.Destroy(InvertEffect.invertvisual.GetComponent<SoundUnityEventPlayer>());
                    UnityEngine.Object.Destroy(InvertEffect.invertvisual.GetComponent<Explosion>());
                    UnityEngine.Object.Destroy(InvertEffect.invertvisual.GetComponent<Explosion_Overpower>());
                    UnityEngine.Object.Destroy(InvertEffect.invertvisual.GetComponent<RemoveAfterSeconds>());
                    InvertSpawner grav = InvertEffect.invertvisual.AddComponent<InvertEffect.InvertSpawner>();
                    result = InvertEffect.invertvisual;
                }
                return result;
            }
            set
            {
            }
        }
        private class InvertSpawner : MonoBehaviour
        {
            private void Start()
            {
                bool flag = !(this.gameObject.GetComponent<SpawnedAttack>().spawner != null);
                if (!flag)
                {
                    this.gameObject.transform.localScale = new Vector3(1f, 1f, 1f);
                    this.gameObject.AddComponent<RemoveAfterSeconds>().seconds = 5f;
                    this.gameObject.transform.GetChild(1).GetComponent<LineEffect>().SetFieldValue("inited", false);
                    typeof(LineEffect).InvokeMember("Init", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.InvokeMethod, null, this.gameObject.transform.GetChild(1).GetComponent<LineEffect>(), new object[0]);
                    this.gameObject.transform.GetChild(1).GetComponent<LineEffect>().radius = 6f;
                    this.gameObject.transform.GetChild(1).GetComponent<LineEffect>().SetFieldValue("startWidth", 0.5f);
                    this.gameObject.transform.GetChild(1).GetComponent<LineEffect>().Play();
                }
            }
        }
    }
}