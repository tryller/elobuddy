using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Menu.Values;

namespace tryCassiopeia
{
    class Modes
    {
        public static AIHeroClient _Player { get { return ObjectManager.Player; } }
        private static long LastQCast = 0;
        private static long LastECast = 0;

        public static float GetDamage(SpellSlot spell, Obj_AI_Base target)
        {
            float ap = ObjectManager.Player.FlatMagicDamageMod + ObjectManager.Player.BaseAbilityDamage;
            if (spell == SpellSlot.E)
            {
                if (!Program.E.IsReady())
                    return 0;
                return ObjectManager.Player.CalculateDamageOnUnit(target, DamageType.Magical, 55f + 25f * (Program.E.Level - 1) + 55 / 100 * ap);
            }
            return 0;
        }

        public static void Combo()
        {
            var target = myTarget.GetTarget(850, DamageType.Magical);
            {
                if (Program.Q.IsReady() && Program.comboMenu["useQ"].Cast<CheckBox>().CurrentValue && target.IsValidTarget(Program.Q.Range))
                {
                    Program.Q.Cast(target);
                }

                if (Program.W.IsReady() && target.IsValidTarget(Program.W.Range) && Program.comboMenu["useW"].Cast<CheckBox>().CurrentValue && Environment.TickCount > LastQCast + Program.Q.CastDelay * 1000)
                {
                    Program.W.Cast(target);
                }

                if (Program.E.IsReady() && Program.comboMenu["useE"].Cast<CheckBox>().CurrentValue && target.IsValidTarget(Program.E.Range) && target != null && target.IsVisible && !target.IsDead)
                {
                    if ((target.HasBuffOfType(BuffType.Poison)))
                    {
                        if (target.IsValidTarget(Program.E.Range))
                        {
                            Program.E.Cast(target);
                        }
                    }
                }

            }
        }

        public static void Harass()
        {
            var target = myTarget.GetTarget(850, DamageType.Magical);
            {
                if (Program.E.IsReady() && Program.harassMenu["useE"].Cast<CheckBox>().CurrentValue && target.IsValidTarget(Program.E.Range) && target != null && target.IsVisible && !target.IsDead)
                {
                    if ((target.HasBuffOfType(BuffType.Poison)))
                    {
                        if (target.IsValidTarget(Program.E.Range))
                        {
                            Program.E.Cast(target);
                        }
                    }
                }

                if (Program.Q.IsReady() && Program.harassMenu["useQ"].Cast<CheckBox>().CurrentValue && target.IsValidTarget(Program.Q.Range))
                {
                    Program.Q.Cast(target);
                }
            }
        }

        public static void LastHit()
        {
            foreach (var minion in ObjectManager.Get<Obj_AI_Minion>().Where(x => Program.E.IsInRange(x)
                && !x.IsDead
                && x.IsEnemy
                && x.HasBuffOfType(BuffType.Poison)
                && x.Health + 5 < GetDamage(SpellSlot.E, x)))
            {
                Program.E.Cast(minion);
            }
        }

        public static void AutoIgnite()
        {
            if (ObjectManager.Player.IsDead || !Program.Ignite.IsReady() || !Program.ksMenu["useIgnite"].Cast<CheckBox>().CurrentValue)
                return;

            var target = myTarget.GetTarget(850, DamageType.Magical);
            if (target.IsValidTarget(Program.Ignite.Range) && target.Health < ObjectManager.Player.GetSummonerSpellDamage(target, DamageLibrary.SummonerSpells.Ignite))
                Program.Ignite.Cast(target);
        }

        public static void LaneClear()
        {
            var laneclearQ = Program.laneClearMenu["useQ"].Cast<CheckBox>().CurrentValue;
            var laneclearW = Program.laneClearMenu["useW"].Cast<CheckBox>().CurrentValue;
            var laneclearE = Program.laneClearMenu["useE"].Cast<CheckBox>().CurrentValue;
            var laneclearMinMana = Program.laneClearMenu["laneMana"].Cast<Slider>().CurrentValue;

            Obj_AI_Base minion =
                EntityManager.GetLaneMinions(
                    EntityManager.UnitTeam.Enemy,
                    ObjectManager.Player.Position.To2D(),
                    600,
                    true).FirstOrDefault();
            if (minion != null && Player.Instance.ManaPercent > laneclearMinMana)
            {
                if (laneclearQ && Program.Q.IsReady())
                {
                    var Qpred = Program.Q.GetPrediction(minion);
                    Program.Q.Cast(Qpred.UnitPosition);
                }

                if (laneclearW && Program.W.IsReady())
                    Program.W.Cast(minion);

                if (laneclearE && Program.E.IsReady() && minion.HasBuffOfType(BuffType.Poison))
                    Program.E.Cast(minion);
            }
        }

        public static void ToggleHarass()
        {
            var target = myTarget.GetTarget(850, DamageType.Magical);
            {
                if (Program.Q.IsReady() && Program.harassMenu["useQToggle"].Cast<KeyBind>().CurrentValue && target.IsValidTarget(Program.Q.Range))
                {
                    Program.Q.Cast(target);
                }
            }
        }

        public static void Ultimate()
        {
            var target = myTarget.GetTarget(500, DamageType.Magical);
            var castPred = Program.R.GetPrediction(target);

            foreach (var enemy in ObjectManager.Get<AIHeroClient>().Where(enemy => enemy.Distance(_Player) <= Program.R.Range))
            {
                if (enemy.CountEnemiesInRange(500) >= Program.comboMenu["minR"].Cast<Slider>().CurrentValue && Program.comboMenu["useAutoUlt"].Cast<CheckBox>().CurrentValue && enemy.IsFacing(ObjectManager.Player))
                {
                    Program.R.Cast(target.Position);
                }
            }
        }
    }
}