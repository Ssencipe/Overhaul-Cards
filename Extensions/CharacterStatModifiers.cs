﻿using System;
using System.Runtime.CompilerServices;
using HarmonyLib;
using UnboundLib.Utils;
using UnityEngine;

namespace OverhaulCards.Extensions
{
    public partial class CharacterStatModifiersAdditionalData
    {
        //public float [something];
        public TimeSince timeSinceLastBlockBox;

        public CharacterStatModifiersAdditionalData()
        {
            //[something] = 0;
            timeSinceLastBlockBox = 0;
        }
    }
    public static class CharacterStatModifiersExtension
    {
        public static readonly ConditionalWeakTable<CharacterStatModifiers, CharacterStatModifiersAdditionalData> data = new ConditionalWeakTable<CharacterStatModifiers, CharacterStatModifiersAdditionalData>();

        public static CharacterStatModifiersAdditionalData GetAdditionalData(this CharacterStatModifiers characterstats)
        {
            return data.GetOrCreateValue(characterstats);
        }

        public static void AddData(this CharacterStatModifiers characterstats, CharacterStatModifiersAdditionalData value)
        {
            try
            {
                data.Add(characterstats, value);
            }
            catch (Exception) { }
        }

        // reset additional CharacterStatModifiers when ResetStats is called
        [HarmonyPatch(typeof(CharacterStatModifiers), "ResetStats")]
        class CharacterStatModifiersPatchResetStats
        {
            private static void Prefix(CharacterStatModifiers __instance)
            {
                //__instance.GetAdditionalData().[something] = 0;
                __instance.GetAdditionalData().timeSinceLastBlockBox = 0;
            }
        }

    }
}