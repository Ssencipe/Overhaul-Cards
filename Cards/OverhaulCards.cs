﻿using BepInEx;
using UnboundLib;
using UnboundLib.Cards;
using HarmonyLib;
using CardChoiceSpawnUniqueCardPatch.CustomCategories;
using ModsPlus;

namespace OverhaulCards.Cards

{
    // These are the mods required for our mod to work
    [BepInDependency("com.willis.rounds.unbound", BepInDependency.DependencyFlags.HardDependency)]
    [BepInDependency("pykess.rounds.plugins.moddingutils", BepInDependency.DependencyFlags.HardDependency)]
    [BepInDependency("pykess.rounds.plugins.cardchoicespawnuniquecardpatch", BepInDependency.DependencyFlags.HardDependency)]
    // Declares our mod to Bepin
    [BepInPlugin(ModId, ModName, Version)]
    // The game our mod is associated with
    [BepInProcess("Rounds.exe")]
    public class OverhaulCards : BaseUnityPlugin
    {
        private const string ModId = "ssencipe.overhaul.cards";
        private const string ModName = "OverhaulCards";
        public const string Version = "0.0.1"; // What version are we on (major.minor.patch)?
        public const string ModInitials = "OHC";

        public static OverhaulCards instance { get; private set; }


        void Awake()
        {
            // Use this to call any harmony patch files your mod may have
            var harmony = new Harmony(ModId);
            harmony.PatchAll();
        }
        void Start()
        {
            instance = this;
            CustomCard.BuildCard<SuperJump>();
            CustomCard.BuildCard<LegDay>();
            CustomCard.BuildCard<Impact>();
            CustomCard.BuildCard<WaveForm>();
            CustomCard.BuildCard<Shrink>();
            CustomCard.BuildCard<Gunslinger>();
            CustomCard.BuildCard<Bulletslinger>();
            CustomCard.BuildCard<Delay>();
            CustomCard.BuildCard<Warp>();
            CustomCard.BuildCard<Porcupine>();
            CustomCard.BuildCard<Flea>();
            CustomCard.BuildCard<BulletWall>();
            CustomCard.BuildCard<Ambush>();
            CustomCard.BuildCard<EyeBeam>();
            //CustomCard.BuildCard<Invert>();
        }
    }
}