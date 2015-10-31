using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Menu.Values;

namespace tryKatarina
{
    class Modes
    {
        public static AIHeroClient _Player { get { return ObjectManager.Player; } }
        private static readonly Item Zhonya = new Item(3157);

        public static void ChangeSkin()
        {
            var style = Program.miscMenu["skinID"].DisplayName;

            switch (style)
            {
                case "Classic":
                    _Player.SetSkinId(0);
                    break;

                case "Mercenary Katarina":
                    _Player.SetSkinId(1);
                    break;

                case "Red Card Katarina":
                    _Player.SetSkinId(2);
                    break;

                case "Bilgewater Katarina":
                    _Player.SetSkinId(3);
                    break;

                case "Kitty Cat Katarina":
                    _Player.SetSkinId(4);
                    break;

                case "High Command Katarina":
                    _Player.SetSkinId(5);
                    break;

                case "Sandstorm Katarina":
                    _Player.SetSkinId(6);
                    break;

                case "Slay Belle Katarina":
                    _Player.SetSkinId(7);
                    break;

                case "Warring  Kingdoms Katarina":
                    _Player.SetSkinId(8);
                    break;
            }
        }

        public static void AutoZhonya()
        {
            if (!Zhonya.IsReady())
                return;

            if (_Player.HealthPercent <= Program.miscMenu["minZhonyaHealth"].Cast<Slider>().CurrentValue
                && !_Player.IsRecalling())
            {
                Zhonya.Cast();
            }
        }

        public static void AutoIgnite()
        {
            if (_Player.IsDead)
                return;

            var target = TargetSelector.GetTarget(650, DamageType.Magical);
            if (target.IsValidTarget(Program.Ignite.Range) && target.Health < _Player.GetSummonerSpellDamage(target, DamageLibrary.SummonerSpells.Ignite))
                Program.Ignite.Cast(target);
        }

        public static void Combo()
        {
            var target = TargetSelector.GetTarget(850, DamageType.Magical);
            {
                if ((target == null) || (!Program.Q.IsReady() && !Program.W.IsReady() &&
                    !Program.E.IsReady() && !Program.R.IsReady()))
                {
                    return;
                }

                bool useQ = Program.comboMenu["useQ"].Cast<CheckBox>().CurrentValue;
                bool useW = Program.comboMenu["useW"].Cast<CheckBox>().CurrentValue;
                bool useE = Program.comboMenu["useE"].Cast<CheckBox>().CurrentValue;
                bool useR = Program.comboMenu["useR"].Cast<CheckBox>().CurrentValue;

                if (useQ)
                {
                    if (Program.Q.IsReady() && target.IsValidTarget(Program.Q.Range) && target != null && target.IsVisible && !target.IsDead)
                    {
                        Program.Q.Cast(target);
                    }
                }

                if (useE)
                {
                    if (Program.E.IsReady() && target.IsValidTarget(Program.E.Range) && target != null && target.IsVisible && !target.IsDead)
                    {
                        Program.E.Cast(target);
                    }
                }

                if (useW)
                {
                    if (Program.W.IsReady() && target.IsValidTarget(Program.W.Range) && target != null && target.IsVisible && !target.IsDead)
                    {
                        Program.W.Cast(target);
                    }
                }

                if (useR)
                {
                    if (Program.R.IsReady() && target.IsValidTarget(Program.R.Range) && target != null && target.IsVisible && !target.IsDead)
                    {
                        Program.R.Cast(target);
                    }
                }
            }
        }
    }
}
