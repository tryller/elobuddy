﻿using EloBuddy;
using EloBuddy.SDK;
using LightCassiopeia.MenuList;
using System.Linq;

namespace LightCassiopeia.Carry
{
    internal class Combo : ModeBase
    {
        public override bool ShouldBeExecuted()
        {
            return Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo);
        }

        public override void Execute()
        {
            var target = TargetSelector.GetTarget(E.Range, DamageType.Magical);

            //R
            if (MenuList.Combo.WithR && R.IsReady())
            {
                foreach (var enemy in ObjectManager.Get<AIHeroClient>().Where(enemy => enemy.Distance(Player.Instance.Position) <= R.Range))
                {
                    if (enemy.CountEnemyChampionsInRange(R.Range) >= MenuList.Combo.CountEnemiesInR)
                    {
                        R.Cast(enemy.Position);
                    }
                }
            }

            //Q
            if (MenuList.Combo.WithQ && Q.IsReady() && target.IsValidTarget(Q.Range))
                Q.Cast(target.Position);

            //E
            if (MenuList.Combo.WithE && E.IsReady() && target.IsValidTarget(E.Range))
            {
                if (target.HasBuffOfType(BuffType.Poison))
                    E.Cast(target);
            }

            //W
            if (MenuList.Combo.WithW && W.IsReady() && target.IsValidTarget(W.Range))
                W.Cast(target.Position);
        }
    }
}