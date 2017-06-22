using EloBuddy;
using EloBuddy.SDK;
using LightCassiopeia.MenuList;
using System.Linq;

namespace LightCassiopeia.Carry
{
    internal class LastHit : ModeBase
    {
        public override bool ShouldBeExecuted()
        {
            return Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LastHit);
        }

        public override void Execute()
        {
            if (MenuList.Misc.LasthitE)
                foreach (var minion in EntityManager.MinionsAndMonsters.EnemyMinions.Where(minions => minions.Health < Damage.GetEDamage(minions)))
                {
                    E.Cast(minion);
                }
        }
    }
}