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
using ModdingUtils.Extensions;
using ModdingUtils.RoundsEffects;
using OverhaulCards.Extensions;
using OverhaulCards.Utils;
using OverhaulCards.Effects;
using System.Collections.ObjectModel;
using Random = UnityEngine.Random;
using CharacterStatModifiersExtension = OverhaulCards.Extensions.CharacterStatModifiersExtension;

namespace OverhaulCards.MonoBehaviours
{
    public class EndlessVolleyEffect : HitSurfaceEffect
    {
        static readonly System.Random rng = new System.Random() { };

        private Player player;
        private Gun gun;
        private GunAmmo gunAmmo;
        private Vector3 crosshair = MainCam.instance.cam.ScreenToWorldPoint(Input.mousePosition);

        public void Awake()
        {
            player = gameObject.GetComponent<Player>();
            gun = player.GetComponent<Holding>().holdable.GetComponent<Gun>();
            gunAmmo = player.GetComponent<Holding>().holdable.GetComponentInChildren<GunAmmo>();
        }
        public override void Hit(Vector2 position, Vector2 normal, Vector2 velocity)
        {
            if (gun.numberOfProjectiles > 5 || gunAmmo.maxAmmo > 12)
            {
                var rnd = Random.Range(0, Math.Max(gun.numberOfProjectiles, gunAmmo.maxAmmo));
                if (rnd > 6)
                {
                    return;
                }
            }

            var newGun = player.gameObject.AddComponent<SplittingGun>();

            SpawnBulletsEffect effect = player.gameObject.AddComponent<SpawnBulletsEffect>();
            // set the position and direction to fire
            Vector2 parallel = ((Vector2)Vector3.Cross(Vector3.forward, crosshair.normalized)).normalized;
            List<Vector3> positions = GetPositions(crosshair, crosshair.normalized, parallel);
            List<Vector3> directions = GetDirections(crosshair, positions);
            effect.SetPositions(positions);
            effect.SetDirections(directions);
            effect.SetNumBullets(5);
            effect.SetTimeBetweenShots(0f);
            effect.SetInitialDelay(0f);

            // copy private gun stats over and reset a few public stats
            SpawnBulletsEffect.CopyGunStats(this.gun, newGun);

            newGun.numberOfProjectiles = 1;
            newGun.spread = 0;
            newGun.reflects = 0;
            newGun.projectileColor = Color.white;
            newGun.projectiles = (from e in Enumerable.Range(0, newGun.numberOfProjectiles) from x in newGun.projectiles select x).ToList().Take(newGun.numberOfProjectiles).ToArray();
            newGun.damage = 1f;
            newGun.projectileSpeed = 0.6f;
            newGun.damageAfterDistanceMultiplier = 1f;
            newGun.GetAdditionalData().inactiveDelay = 0.1f;

            newGun.objectsToSpawn = new[]
            {
                PreventRecursion.stopRecursionObjectToSpawn,
            };

            // set the gun of the spawnbulletseffect
            effect.SetGun(newGun);
        }

        private List<Vector3> GetPositions(Vector2 position, Vector2 normal, Vector2 parallel)
        {
            List<Vector3> res = new List<Vector3>() { };

            for (int i = 0; i < 5; i++)
            {
                res.Add(new Vector2(crosshair.x + (-5 + 2 * i), 30));
            }

            return res;
        }

        private List<Vector3> GetDirections(Vector2 position, List<Vector3> shootPos)
        {
            List<Vector3> res = new List<Vector3>() { };

            foreach (Vector3 shootposition in shootPos)
            {
                res.Add(Vector3.down);
            }

            return res;
        }
    }
}