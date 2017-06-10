using EloBuddy;
using EloBuddy.SDK;
using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Katarina
{
    static class Extensions
    {
        public static bool IsUnderEnemyTurret(this Vector3 d)
        {
            return EntityManager.Turrets.Enemies.Any((Obj_AI_Turret turret) => turret.IsInRange(d, turret.GetAutoAttackRange(null)) && turret.IsAlive());
        }
    }

    class Damage
    {
        public static readonly Random getrandom = new Random();
        private static int[] DaggerDamages =
        {
            75, 80, 87, 94, 102, 111, 120, 131, 143, 155, 168, 183, 198, 214, 231, 248, 267, 287
        };
        private static Dictionary<int, int> DaggerMultiplers = new Dictionary<int, int>() { { 1, 55 }, { 6, 70 }, { 11, 85 }, { 16, 100 } };

        private static float GetAPMultipler()
        {
            foreach (var a in DaggerMultiplers) if (Player.Instance.Level <= a.Key) return (a.Value / 100);
            return 0.55f;
        }

        public static double GetPDamage(Obj_AI_Base target)
        {
            int singleDagger =
                DaggerDamages[Player.Instance.Level - 1] +
                (int)(Player.Instance.TotalAttackDamage - Player.Instance.BaseAttackDamage) +
                (int)(GetAPMultipler() * Player.Instance.TotalMagicalDamage);

            int totalDamage = 0;
            foreach (var dagger in Daggers.GetDaggers())
                if (target.Position.IsInRange(dagger, Program.W.Range + 75))
                    totalDamage += singleDagger;

            return totalDamage * (100 - target.PercentMagicReduction) / 100;
        }

        public static int GetAditionalDelay()
        {
            return getrandom.Next(50, 100);
        }


        public static Vector3 GetBestDaggerPoint(Vector3 position, Obj_AI_Base target)
        {
            if (target.Position.IsInRange(position, 150)) return position;
            return position.Extend(target, 150).To3D();
        }
    }
}
