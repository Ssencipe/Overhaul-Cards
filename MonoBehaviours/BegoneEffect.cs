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
using Sonigon;
using Sonigon.Internal;
using SoundImplementation;
using ModdingUtils.RoundsEffects;
using UnboundLib.Extensions;
using ModdingUtils.Extensions;

namespace OverhaulCards.MonoBehaviours
{
    public class BegoneEffect : MonoBehaviour
    {
        private readonly float maxDistance = 8f;
        public Block block;
        public Player player;
        public CharacterData data;
        public Gun gun;
        private Action<BlockTrigger.BlockTriggerType> begoner;
        private Action<BlockTrigger.BlockTriggerType> basic;
        private static GameObject begonevisual = null;
        private readonly float updateDelay = 0.1f;
        private readonly float effectCooldown = 0.1f;
        private float startTime;
        private float timeOfLastEffect;
        private bool canTrigger;
        private bool hasTriggered;
        public int numcheck = 0;
        private Vector3 initialPos;

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
                this.begoner = new Action<BlockTrigger.BlockTriggerType>(this.GetDoBlockAction(this.player, this.block).Invoke);
                this.block.BlockAction = (Action<BlockTrigger.BlockTriggerType>)Delegate.Combine(this.block.BlockAction, this.begoner);
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
                    if (this.player.data.currentCards[i].cardName == "Begone")
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
                        if (!block.objectsToSpawn.Contains(BegoneEffect.begoneVisual))
                        {
                            block.objectsToSpawn.Add(BegoneEffect.begoneVisual);
                        }
                        this.canTrigger = true;
                    }

                    else
                    {
                        if (block.objectsToSpawn.Contains(BegoneEffect.begoneVisual))
                        {
                            block.objectsToSpawn.Remove(BegoneEffect.begoneVisual);
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
                                    initialPos.x = player.transform.position.x;
                                    initialPos.y = player.transform.position.y;
                                    initialPos.x += UnityEngine.Random.Range(-15f, 15f);
                                    initialPos.y += UnityEngine.Random.Range(-5f, 10f);
                                    array[i].GetComponentInParent<PlayerCollision>().IgnoreWallForFrames(2);
                                    array[i].transform.position = new Vector3(initialPos.x, initialPos.y, player.transform.position.z);
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

        public static GameObject begoneVisual
        {
            get
            {
                bool flag = BegoneEffect.begonevisual != null;
                GameObject result;
                if (flag)
                {
                    result = BegoneEffect.begonevisual;
                }
                else
                {
                    List<CardInfo> first = ((ObservableCollection<CardInfo>)typeof(CardManager).GetField("activeCards", BindingFlags.Static | BindingFlags.NonPublic).GetValue(null)).ToList<CardInfo>();
                    List<CardInfo> second = (List<CardInfo>)typeof(CardManager).GetField("inactiveCards", BindingFlags.Static | BindingFlags.NonPublic).GetValue(null);
                    List<CardInfo> source = first.Concat(second).ToList<CardInfo>();
                    GameObject original = (from card in source
                                           where card.cardName.ToLower() == "overpower"
                                           select card).First<CardInfo>().GetComponent<CharacterStatModifiers>().AddObjectToPlayer.GetComponent<SpawnObjects>().objectToSpawn[0];
                    BegoneEffect.begonevisual = UnityEngine.Object.Instantiate<GameObject>(original, new Vector3(0f, 100000f, 0f), Quaternion.identity);
                    BegoneEffect.begonevisual.name = "E_Begone";
                    UnityEngine.Object.DontDestroyOnLoad(BegoneEffect.begonevisual);
                    foreach (ParticleSystem particleSystem in BegoneEffect.begonevisual.GetComponentsInChildren<ParticleSystem>())
                    {
                        particleSystem.startColor = Color.green;
                    }
                    BegoneEffect.begonevisual.transform.GetChild(1).GetComponent<LineEffect>().colorOverTime.colorKeys = new GradientColorKey[]
                    {
                        new GradientColorKey(Color.green, 0f)
                    };
                    UnityEngine.Object.Destroy(BegoneEffect.begonevisual.transform.GetChild(2).gameObject);
                    BegoneEffect.begonevisual.transform.GetChild(1).GetComponent<LineEffect>().offsetMultiplier = 0f;
                    BegoneEffect.begonevisual.transform.GetChild(1).GetComponent<LineEffect>().playOnAwake = true;
                    UnityEngine.Object.Destroy(BegoneEffect.begonevisual.GetComponent<FollowPlayer>());
                    BegoneEffect.begonevisual.GetComponent<DelayEvent>().time = 0f;
                    UnityEngine.Object.Destroy(BegoneEffect.begonevisual.GetComponent<SoundUnityEventPlayer>());
                    UnityEngine.Object.Destroy(BegoneEffect.begonevisual.GetComponent<Explosion>());
                    UnityEngine.Object.Destroy(BegoneEffect.begonevisual.GetComponent<Explosion_Overpower>());
                    UnityEngine.Object.Destroy(BegoneEffect.begonevisual.GetComponent<RemoveAfterSeconds>());
                    BegoneSpawner goner = BegoneEffect.begonevisual.AddComponent<BegoneEffect.BegoneSpawner>();
                    result = BegoneEffect.begonevisual;
                }
                return result;
            }
            set
            {
            }
        }
        private class BegoneSpawner : MonoBehaviour
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