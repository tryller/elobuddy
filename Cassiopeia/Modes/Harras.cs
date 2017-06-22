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
            if (MenuList.Harras.WithQ && Q.IsReady())
            {
                var target = TargetSelector.GetTarget(Q.Range, DamageType.Magical, Player.Instance.Position);
                var qPred = Q.GetPrediction(target);
                if (qPred.HitChancePercent >= Misc.skillsPredition)
                    Q.Cast(qPred.CastPosition);
            }

            if (MenuList.Harras.WithW && W.IsReady())
            {
                var target = TargetSelector.GetTarget(W.Range, DamageType.Magical, Player.Instance.Position);
                var wPred = SpellManager.W.GetPrediction(target);
                if (wPred.HitChancePercent >= Misc.skillsPredition)
                    W.Cast(wPred.CastPosition);
            }
            if (MenuList.Harras.WithE && E.IsReady())
            {
                foreach (var enemy in EntityManager.Heroes.Enemies.Where(en => en.IsValidTarget(E.Range)))
                {
                    if (enemy.HasBuffOfType(BuffType.Poison))
                        E.Cast(enemy);
                }
            }
        }
    }
}