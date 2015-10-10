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
        private static readonly Item Zhonya = new Item(3157);

        public static void AutoZhonya()
        {
            if (!Zhonya.IsReady())
                return;

            if (_Player.HealthPercent <= Program.miscMenu["minZhonyaHealth"].Cast<Slider>().CurrentValue
                && !_Player.IsRecalling)
            {
                Zhonya.Cast();
            }
        }

        public static void KillSteal()
        {
            foreach (AIHeroClient enemy in EntityManager.Heroes.Enemies)
            {
                if (enemy.IsValidTarget(Program.Q.Range))
                {
                    if (Program.miscMenu["ksQ"].Cast<CheckBox>().CurrentValue && (_Player.GetSpellDamage(enemy, SpellSlot.Q) >= enemy.Health))
                    {
                        Program.Q.Cast(enemy);
                    }

                    if (Program.E.IsReady() && Program.miscMenu["ksE"].Cast<CheckBox>().CurrentValue && (_Player.GetSpellDamage(enemy, SpellSlot.E) >= enemy.Health))
                    {
                        Program.E.Cast(enemy);
                    }
                }
            }
        }

        public static void ChangeSkin() 
        {
            var style = Program.miscMenu["skinID"].DisplayName;

            switch (style)
            {
                case "Classic":
                    _Player.SetSkinId(0);
                    break;
                case "Bittersweet Lulu":
                    _Player.SetSkinId(1);
                    break;
                case "Wicked Lulu":
                    _Player.SetSkinId(2);
                    break;
                case "Dragon Trainer Lulu":
                    _Player.SetSkinId(3);
                    break;
                case "Winter Wonder Lulu":
                    _Player.SetSkinId(4);
                    break;
                case "Pool Paerty Lulu":
                    _Player.SetSkinId(5);
                    break;
            }
        }

        public static float GetDamage(SpellSlot spell, Obj_AI_Base target)
        {
            float ap = _Player.FlatMagicDamageMod + _Player.BaseAbilityDamage;
            if (spell == SpellSlot.Q)
            {
                if (!Program.Q.IsReady())
                    return 0;

                return _Player.CalculateDamageOnUnit(target, DamageType.Magical, 55f + 25f * (Program.E.Level - 1) + 55 / 100 * ap);
            }
            return 0;
        }

        public static void Combo()
        {
            var target = myTarget.GetTarget(850, DamageType.Magical);
            {
                bool useQ = Program.comboMenu["useQ"].Cast<CheckBox>().CurrentValue;
                bool useW = Program.comboMenu["useW"].Cast<CheckBox>().CurrentValue;
                bool useE = Program.comboMenu["useE"].Cast<CheckBox>().CurrentValue;

                if (useW)
                {
                    if (Program.W.IsReady() && target.IsValidTarget(Program.W.Range) && target != null && target.IsVisible && !target.IsDead)
                    {
                        Program.W.Cast(target);
                    }
                }

                if (useQ)
                {
                    if (Program.Q.IsReady() && target.IsValidTarget(Program.Q.Range) && target != null && target.IsVisible && !target.IsDead)
                    {
                        var qPred = Program.Q.GetPrediction(target);
                        Program.Q.Cast(qPred.UnitPosition);
                    }
                }

                if (useE)
                {
                    if (Program.E.IsReady() && target.IsValidTarget(Program.E.Range) && target != null && target.IsVisible && !target.IsDead)
                    {
                        Program.E.Cast(target);
                    }
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

        public static void AutoIgnite()
        {
            if (_Player.IsDead)
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

            if (minion != null && _Player.ManaPercent > laneclearMinMana)
            {
                if (laneclearQ && Program.Q.IsReady())
                {
                    Program.Q.Cast(minion);
                }
            }
        }

        public static void Ultimate()
        {
            foreach (var ally in EntityManager.Heroes.Allies)
            {
                if (ally.IsValidTarget(Program.R.Range))
                {
                    var c = ally.CountEnemiesInRange(300);
                    if (c >= 1 + 1 + 1 || ally.HealthPercent <= Program.comboMenu["minR"].Cast<Slider>().CurrentValue
                        && c >= 1 && !ally.IsRecalling)
                    {
                        Program.R.Cast(ally);
                    }
                }
            }

            var ec = _Player.CountEnemiesInRange(300);
            if (ec >= 1 + 1 + 1 || _Player.HealthPercent <= Program.comboMenu["minR"].Cast<Slider>().CurrentValue
                && ec >= 1 && !_Player.IsRecalling)
            {
                Program.R.Cast(_Player);
            }
        }
    }
}