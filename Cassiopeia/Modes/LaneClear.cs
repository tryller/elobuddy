using EloBuddy;
using EloBuddy.SDK;
using System.Linq;

namespace LightCassiopeia.Carry
{
    public sealed class LaneClear : ModeBase
    {
        public override bool ShouldBeExecuted()
        {
            return Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear);
        }

        public override void Execute()
        {
            var minions = EntityManager.MinionsAndMonsters.EnemyMinions.Where(moba => moba.IsMinion);
            if (MenuList.Farm.WithE)
            {
                foreach (var minion in minions)
                {
                    if (minion.Health <= Damage.GetEDamage(minion))
                        E.Cast(minion);
                }
            }
            if (MenuList.Farm.WithQ)
            {
                foreach (var minion in minions.Where(mi => mi.IsValidTarget(Q.Range)))
                {
                    Q.Cast(minion);
                }
            }

            if (MenuList.Farm.WithW)
            {
                foreach (var minion in minions.Where(mi => mi.IsValidTarget(W.Range)))
                {
                    W.Cast(minion);
                }
            }
        }
    }
}