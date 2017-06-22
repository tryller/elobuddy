using EloBuddy;
using EloBuddy.SDK;
using System.Linq;

namespace LightCassiopeia.Carry
{
    public sealed class JungleClear : ModeBase
    {
        public override bool ShouldBeExecuted()
        {
            return Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.JungleClear);
        }

        public override void Execute()
        {
            var monsters = EntityManager.MinionsAndMonsters.Monsters.Where(monster => monster.IsMonster);
            if (MenuList.Farm.WithQ)
            {
                foreach (var monster in monsters.Where(mon => mon.IsValidTarget(Q.Range)))
                {
                    Q.Cast(monster);
                }
            }

            if (MenuList.Farm.WithE)
            {
                foreach (var monster in monsters.Where(mon => mon.IsValidTarget(E.Range)).Where(mon => mon.HasBuffOfType(BuffType.Poison)))
                {
                    E.Cast(monster);
                }
            }

            if (MenuList.Farm.WithW)
            {
                foreach (var monster in monsters.Where(mon => mon.IsValidTarget(W.Range)))
                {
                    W.Cast(monster);
                }
            }
        }
    }
}