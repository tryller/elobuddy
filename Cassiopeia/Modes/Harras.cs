using EloBuddy;
using EloBuddy.SDK;
using LightCassiopeia.MenuList;
using System.Linq;

namespace LightCassiopeia.Carry
{
    public sealed class Harras : ModeBase
    {
        public override bool ShouldBeExecuted()
        {
            return Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass);
        }

        public override void Execute()
        {
            var target = TargetSelector.GetTarget(Q.Range, DamageType.Magical, Player.Instance.Position);
            if (MenuList.Harras.WithQ && Q.IsReady())
                Q.Cast(target.Position);

            if (MenuList.Harras.WithW && W.IsReady())
                W.Cast(target.Position);

            if (MenuList.Harras.WithE && E.IsReady())
            {
                if (target.HasBuffOfType(BuffType.Poison))
                    E.Cast(target);
            }
        }
    }
}