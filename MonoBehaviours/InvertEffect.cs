using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnboundLib;
using UnityEngine;
using UnboundLib.Utils;
using ModdingUtils.MonoBehaviours;
using OverhaulCards.Cards;
using System.Collections.ObjectModel;
using SoundImplementation;
using ModdingUtils.Extensions;

namespace OverhaulCards.MonoBehaviours
{
    public class InvertEffect : CounterReversibleEffect, ISingletonEffect
    {
        public int CardAmount { get; set; } = 0;

        public Block block;
        public Player player;
        public CharacterData data;
        public Gun gun;
        private float timeStarted = 0;
        private List<Player> enemies = new List<Player>();
        private List<Player> affected = new List<Player>();
        private static GameObject invertvisual = null;
        private readonly float updateDelay = 0.1f;
        private readonly float effectCooldown = 0.1f;
        private float startTime;
        private float timeOfLastEffect;
        public int numcheck = 0;
        private float counter = 0;
        private bool modifiersActive = false;

        public void Activate()
        {
            counter += Cards.Invert.InvertDuration * CardAmount;
            if (!modifiersActive)
            {
                enemies = PlayerManager.instance.players.Where(p => p.teamID != player.teamID).ToList();
                timeStarted = Time.time;

                modifiersActive = true;
                UpdateEffects();
            }
        }

        public void FixedUpdate()
        {
            if (modifiersActive)
            {
                if (Time.time - timeStarted > Invert.InvertDuration * CardAmount)
                {
                    modifiersActive = false;
                }
            }
        }

        public override void OnStart()
        {
            player = gameObject.GetComponentInParent<Player>();
            block = player.GetComponent<Block>();
            block.BlockAction += OnBlock;
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
            foreach (Player enemy in enemies)
            {
                Vector3 dir = enemy.transform.position - player.transform.position;
                float distance = dir.magnitude;
                dir.Normalize();

                float distance_squared = Mathf.Clamp(distance * distance, 20f, float.MaxValue);

                if (distance <= 2.5f * CardAmount)
                {
                    affected.Add(enemy);
                    enemy.GetComponentInChildren<CharacterStatModifiersModifier>().gravity_mult *= -10f;
                    enemy.GetComponentInChildren<CharacterStatModifiersModifier>().movementSpeed_mult *= 0.1f;

                }
            }
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
        public override void OnApply()
        {
            modifiersActive = true;
        }
        public override void OnRemove()
        {
            modifiersActive = false;
        }
        public override void Reset()
        {
            counter = 0;
            modifiersActive = false;
        }
        //Below this point is code for block VFX
        public override void OnUpdate()
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

                if (Time.time >= this.timeOfLastEffect + this.effectCooldown)
                {
                    if (!block.objectsToSpawn.Contains(InvertEffect.invertVisual))
                    {
                        block.objectsToSpawn.Add(InvertEffect.invertVisual);
                    }
                }

                else
                {
                    if (block.objectsToSpawn.Contains(InvertEffect.invertVisual))
                    {
                        block.objectsToSpawn.Remove(InvertEffect.invertVisual);
                    }

                }
            }

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
                    foreach (ParticleSystem particleSystem in InvertEffect.invertvisual.GetComponentsInChildren<ParticleSystem>())
                    {
                        particleSystem.startColor = Color.yellow;
                    }
                    InvertEffect.invertvisual.transform.GetChild(1).GetComponent<LineEffect>().colorOverTime.colorKeys = new GradientColorKey[]
                    {
                        new GradientColorKey(Color.yellow, 0f)
                    };
                    UnityEngine.Object.Destroy(InvertEffect.invertvisual.transform.GetChild(2).gameObject);
                    InvertEffect.invertvisual.transform.GetChild(1).GetComponent<LineEffect>().offsetMultiplier = 0f;
                    InvertEffect.invertvisual.transform.GetChild(1).GetComponent<LineEffect>().playOnAwake = true;
                    UnityEngine.Object.Destroy(InvertEffect.invertvisual.GetComponent<FollowPlayer>());
                    InvertEffect.invertvisual.GetComponent<DelayEvent>().time = 0f;
                    UnityEngine.Object.Destroy(InvertEffect.invertvisual.GetComponent<SoundUnityEventPlayer>());
                    UnityEngine.Object.Destroy(InvertEffect.invertvisual.GetComponent<Explosion>());
                    UnityEngine.Object.Destroy(InvertEffect.invertvisual.GetComponent<Explosion_Overpower>());
                    UnityEngine.Object.Destroy(InvertEffect.invertvisual.GetComponent<RemoveAfterSeconds>());
                    InvertSpawner inverse = InvertEffect.invertvisual.AddComponent<InvertEffect.InvertSpawner>();
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