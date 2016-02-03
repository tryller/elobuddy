using System;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Enumerations;
using EloBuddy.SDK.Events;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using EloBuddy.SDK.Rendering;
using SharpDX;

namespace tryCombos.Champions
{
    static class Ryze
    {
        private static Spell.Skillshot Q;
        private static Spell.Targeted W;
        private static Spell.Targeted E;
        private static Spell.Active R;

        public static AIHeroClient target;
        public static Menu Menu;
        public static string G_charname = Program.myHero.ChampionName;

        public static void Init()
        {
            Q = new Spell.Skillshot(SpellSlot.Q, 900, SkillShotType.Linear, 250, 1700, 100);
            W = new Spell.Targeted(SpellSlot.W, 600);
            E = new Spell.Targeted(SpellSlot.E, 600);
            R = new Spell.Active(SpellSlot.R);

            Menu = MainMenu.AddMenu(G_charname, G_charname.ToLower());

            Menu.AddGroupLabel("Welcome to " + G_charname);
            Menu.AddLabel("Addon that contains combos for all champions.");
            Menu.AddLabel("This adodn don't have anny config.");
            Menu.AddLabel("But it have full combo for LOL Champions.");
            Menu.AddSeparator();
            Menu.AddLabel("By Tryller");

            Game.OnTick += Game_OnTick;
        }

        static void Game_OnTick(EventArgs args)
        {
            switch (Orbwalker.ActiveModesFlags)
            {
                case Orbwalker.ActiveModes.Combo:
                    OnCombo();
                    break;
            }
        }

        public static void OnCombo()
        {
            target = TargetSelector.GetTarget(Q.Range, DamageType.Magical);
            if (target == null || !target.IsValid || target.IsZombie)
                return;

            //Passive
            bool Pasive = Program.myHero.HasBuff("ryzepassivecharged");
            var Stacks = Program.myHero.GetBuffCount("ryzepassivestack");
            bool StacksBuff = Program.myHero.HasBuff("ryzepassivestack");

            if (target != null && target.IsValidTarget(Q.Range)
                && !target.HasBuff("sionpassivezombie")               //sion Passive
                && !target.HasBuff("KarthusDeathDefiedBuff")          //karthus passive
                && !target.HasBuff("kogmawicathiansurprise")          //kog'maw passive
                && !target.HasBuff("zyrapqueenofthorns")              //zyra passive
                && !target.HasBuff("ChronoShift")                     //zilean R
                && !target.HasBuff("yorickrazombie"))                 //yorick R
            {
                var QPred = Q.GetPrediction(target);
                if (target.IsValidTarget(Q.Range))
                {
                    if (Stacks <= 2 || !StacksBuff)
                    {
                        if (target.IsValidTarget(Q.Range) && Q.IsReady())
                        {
                            Q.Cast(QPred.UnitPosition);
                        }
                        if (target.IsValidTarget(W.Range) && W.IsReady())
                        {
                            W.Cast(target);
                        }
                        if (target.IsValidTarget(E.Range) && E.IsReady())
                        {
                            E.Cast(target);
                        }
                        if (R.IsReady())
                        {
                            if (target.IsValidTarget(W.Range)
                                    && target.Health > (Program.myHero.GetSpellDamage(target, SpellSlot.Q) + Program.myHero.GetSpellDamage(target, SpellSlot.E)))
                            {
                                R.Cast();
                            }
                        }
                    }
                }

                if (Stacks == 3)
                {
                    if (target.IsValidTarget(Q.Range) && Q.IsReady())
                    {
                        Q.Cast(QPred.UnitPosition);
                    }
                    if (target.IsValidTarget(E.Range) && E.IsReady())
                    {
                        E.Cast(target);
                    }
                    if (target.IsValidTarget(W.Range) && W.IsReady())
                    {
                        W.Cast(target);
                    }
                    if (R.IsReady())
                    {
                        if (target.IsValidTarget(W.Range)
                                && target.Health > (Program.myHero.GetSpellDamage(target, SpellSlot.Q) + Program.myHero.GetSpellDamage(target, SpellSlot.E)))
                        {
                            if (!E.IsReady() && !Q.IsReady() && !W.IsReady())
                                R.Cast();
                        }
                    }
                }

                if (Stacks == 4)
                {
                    if (target.IsValidTarget(W.Range) && W.IsReady())
                    {
                        W.Cast(target);
                    }
                    if (target.IsValidTarget(Q.Range) && Q.IsReady())
                    {
                        Q.Cast(QPred.UnitPosition);
                    }
                    if (target.IsValidTarget(E.Range) && E.IsReady())
                    {
                        E.Cast(target);
                    }
                    if (R.IsReady())
                    {
                        if (target.IsValidTarget(W.Range)
                                && target.Health > (Program.myHero.GetSpellDamage(target, SpellSlot.Q) + Program.myHero.GetSpellDamage(target, SpellSlot.E)))
                        {
                            R.Cast();
                        }
                    }
                }

                if (Pasive)
                {
                    if (target.IsValidTarget(W.Range) && W.IsReady())
                    {
                        W.Cast(target);
                    }
                    if (target.IsValidTarget(Q.Range) && Q.IsReady())
                    {
                        Q.Cast(QPred.UnitPosition);
                    }
                    if (target.IsValidTarget(E.Range) && E.IsReady())
                    {
                        E.Cast(target);
                    }
                    if (R.IsReady())
                    {
                        if (target.IsValidTarget(W.Range)
                            && target.Health > (Program.myHero.GetSpellDamage(target, SpellSlot.Q) + Program.myHero.GetSpellDamage(target, SpellSlot.E)))
                        {
                            R.Cast();
                        }
                    }
                }
                else
                {
                    if (W.IsReady()
                        && target.IsValidTarget(W.Range))
                        W.Cast(target);

                    if (Q.IsReady()
                        && target.IsValidTarget(Q.Range))
                        Q.Cast(QPred.UnitPosition);

                    if (E.IsReady()
                        && target.IsValidTarget(E.Range))
                        E.Cast(target);
                }
                if (!R.IsReady() || Stacks != 4) return;

                if (Q.IsReady() || W.IsReady() || E.IsReady()) return;

                R.Cast();
            }
        }
    }
}