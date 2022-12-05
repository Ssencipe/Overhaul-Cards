using System;
using System.Runtime.CompilerServices;
using HarmonyLib;

namespace OverhaulCards.Extensions
{
    public class CharacterDataAdditionalData
    {
        //public float[whatever];
        public OutOfBoundsHandler outOfBoundsHandler;

        public CharacterDataAdditionalData()
        {
            //[whatever] = 0;
            outOfBoundsHandler = null;
        }
    }

    public static class CharacterDataExtension
    {
        public static readonly ConditionalWeakTable<CharacterData, CharacterDataAdditionalData> data =
            new ConditionalWeakTable<CharacterData, CharacterDataAdditionalData>();

        public static CharacterDataAdditionalData GetAdditionalData(this CharacterData block)
        {
            return data.GetOrCreateValue(block);
        }

        public static void AddData(this CharacterData block, CharacterDataAdditionalData value)
        {
            try
            {
                data.Add(block, value);
            }
            catch (Exception) { }
        }
    }

    // CODE FROM PCE
    // get outOfBounds handler assigned to this player
    [HarmonyPatch(typeof(OutOfBoundsHandler), "Start")]
    class OutOfBoundsHandlerPatchStart
    {
        private static void Postfix(OutOfBoundsHandler __instance)
        {
            if (((CharacterData)Traverse.Create(__instance).Field("data").GetValue()).GetAdditionalData().outOfBoundsHandler == null)
            {
                OutOfBoundsHandler[] ooBs = UnityEngine.Object.FindObjectsOfType<OutOfBoundsHandler>();
                foreach (OutOfBoundsHandler ooB in ooBs)
                {
                    if (((CharacterData)Traverse.Create(ooB).Field("data").GetValue()).player.playerID == ((CharacterData)Traverse.Create(__instance).Field("data").GetValue()).player.playerID)
                    {
                        ((CharacterData)Traverse.Create(__instance).Field("data").GetValue()).GetAdditionalData().outOfBoundsHandler = ooB;
                        return;
                    }
                }
            }
        }
    }
}