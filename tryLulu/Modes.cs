using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Menu.Values;

namespace tryLulu
{
    class Modes
    {
        public static AIHeroClient _Player { get { return ObjectManager.Player; } }
        private static long LastQCast = 0;
        private static long LastECast = 0;

        public static float GetDamage(SpellSlot spell, Obj_AI_Base target)
        {
            float ap = _Player.FlatMagicDamageMod + _Player.BaseAbilityDamage;
            if (spell == SpellSlot.E)
            {
                if (!Program.E.IsReady())
                    return 0;
                return _Player.CalculateDamageOnUnit(target, DamageType.Magical, 55f + 25f * (Program.E.Level - 1) + 55 / 100 * ap);
            }
            return 0;
        }

        public static void Combo()
        {
            var target = myTarget.GetTarget(850, DamageType.Magical);
            {
                if (Program.W.IsReady() && target.IsValidTarget(Program.W.Range) && Program.comboMenu["useW"].Cast<CheckBox>().CurrentValue && Environment.TickCount > LastQCast + Program.Q.CastDelay * 1000)
                {
                    Program.W.Cast(target);
                }

                if (Program.Q.IsReady() && Program.comboMenu["useQ"].Cast<CheckBox>().CurrentValue && target.IsValidTarget(Program.Q.Range) && target != null && target.IsVisible && !target.IsDead)
                {
                    var qPred = Program.Q.GetPrediction(target);
                    Program.Q.Cast(qPred.UnitPosition);
                }

                if (Program.E.IsReady() && Program.comboMenu["useE"].Cast<CheckBox>().CurrentValue && target.IsValidTarget(Program.E.Range) && target != null && target.IsVisible && !target.IsDead)
                {
                    Program.E.Cast(target);
                }
            }
        }

        public static void Harass()
        {
            var target = myTarget.GetTarget(850, DamageType.Magical);
            {
                if (Program.Q.IsReady() && Program.harassMenu["useQ"].Cast<CheckBox>().CurrentValue && target.IsValidTarget(Program.Q.Range))
                {
                    var qPred = Program.Q.GetPrediction(target);
                    Program.Q.Cast(qPred.UnitPosition);
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
            if (_Player.IsDead || !Program.Ignite.IsReady() || !Program.ksMenu["useIgnite"].Cast<CheckBox>().CurrentValue)
                return;

            var target = myTarget.GetTarget(650, DamageType.Magical);
            if (target.IsValidTarget(Program.Ignite.Range) && target.Health < _Player.GetSummonerSpellDamage(target, DamageLibrary.SummonerSpells.Ignite))
                Program.Ignite.Cast(target);
        }

        public static void LaneClear()
        {
            var laneclearQ = Program.laneClearMenu["useQ"].Cast<CheckBox>().CurrentValue;
            var laneclearMinMana = Program.laneClearMenu["laneMana"].Cast<Slider>().CurrentValue;

            Obj_AI_Base minion = EntityManager.GetLaneMinions(
                    EntityManager.UnitTeam.Enemy,
                    _Player.Position.To2D(),
                    600,
                    true).FirstOrDefault();

            if (minion != null && Player.Instance.ManaPercent > laneclearMinMana)
            {
                if (laneclearQ && Program.Q.IsReady())
                {
                    var Qpred = Program.Q.GetPrediction(minion);
                    Program.Q.Cast(Qpred.UnitPosition);
                }
            }
        }

        public static void Ultimate()
        {
            if (Program.comboMenu["useAutoUlt"].Cast<CheckBox>().CurrentValue)
            {
                foreach (var ally in EntityManager.Heroes.Allies)
                {
                    if (ally.IsValidTarget(Program.R.Range))
                    {
                        var c = ally.CountEnemiesInRange(300);
                        if (c >= 1 + 1 + 1 || ally.HealthPercent <= Program.comboMenu["minR"].Cast<Slider>().CurrentValue && c >= 1)
                        {
                            Program.R.Cast(ally);
                        }
                    }
                }

                var ec = _Player.CountEnemiesInRange(300);
                if (ec >= 1 + 1 + 1 || _Player.HealthPercent <= 15 && ec >= 1)
                {
                    Program.R.Cast(_Player);
                }
            }
        }
    }
}