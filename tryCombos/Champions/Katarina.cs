using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
    static class Katarina
    {
        public static Spell.Targeted Q;
        public static Spell.Active W;
        public static Spell.Targeted E;
        public static Spell.Active R;

        public static AIHeroClient target;

        public static Menu Menu, Humanizer;
        public static string G_charname = Program.myHero.ChampionName;

        private static DateTime assemblyLoadTime = DateTime.Now;
        public class LatestCast
        {
            public static float Tick = 0;
            public static float Timepass;
            public static float X = 0;
            public static float Y = 0;
            public static double Distance;
            public static double Delay;
            public static int count = 0;
            public static double SavedTime = 0;

        }

        public static void Init()
        {
            Q = new Spell.Targeted(SpellSlot.Q, 675);
            W = new Spell.Active(SpellSlot.W, 375);
            E = new Spell.Targeted(SpellSlot.E, 700);
            R = new Spell.Active(SpellSlot.R, 550);

            Menu = MainMenu.AddMenu(G_charname, G_charname.ToLower());

            Menu.AddLabel("Welcome to " + G_charname);
            Menu.AddLabel("Addon that contains combos for all champions.");
            Menu.AddLabel("This adodn don't have anny config.");
            Menu.AddLabel("But it have full combo for LOL Champions.");
            Menu.AddLabel("By Tryller");

            Humanizer = Menu.AddSubMenu("Humanizer", "Humanizer");
            Humanizer.Add("useHumanizer", new CheckBox("Use Humanize"));
            Humanizer.Add("delayTime", new Slider("Delay time for distance", 500, 0, 1500));
            Humanizer.Add("delayTimeCasts", new Slider("Delay time between casts", 800, 0, 1500));

            Spellbook.OnCastSpell += Spellbook_OnCastSpell;
            Game.OnTick += Game_OnTick;
        }

        static void Game_OnTick(EventArgs args)
        {
            if (!Program.myHero.HasBuff("katarinarsound") || !Program.myHero.Spellbook.IsChanneling)
            {
                Orbwalker.DisableAttacking = false;
                Orbwalker.DisableMovement = false;
            }
            else
            {
                Orbwalker.DisableAttacking = true;
                Orbwalker.DisableMovement = true;
            }

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

            if (target != null && target.IsValidTarget(Q.Range)
                && !target.HasBuff("sionpassivezombie")               //sion Passive
                && !target.HasBuff("KarthusDeathDefiedBuff")          //karthus passive
                && !target.HasBuff("kogmawicathiansurprise")          //kog'maw passive
                && !target.HasBuff("zyrapqueenofthorns")              //zyra passive
                && !target.HasBuff("ChronoShift")                     //zilean R
                && !target.HasBuff("yorickrazombie"))                 //yorick R
            {

                if (Q.IsReady() && target.IsValidTarget(Q.Range) && !target.HasBuffOfType(BuffType.Invulnerability))
                {
                    Q.Cast(target);
                }

                if (E.IsReady() && target.IsValidTarget(E.Range) && !target.HasBuffOfType(BuffType.Invulnerability))
                {
                    E.Cast(target);
                }

                if (W.IsReady() && Program.myHero.Distance(target.Position) <= W.Range && !target.HasBuffOfType(BuffType.Invulnerability))
                {
                    W.Cast();
                }


                if (R.IsReady() && Program.myHero.Distance(target.Position) <= R.Range && !target.HasBuffOfType(BuffType.Invulnerability)
                    && !Q.IsReady() && !W.IsReady() && !E.IsReady())
                {
                    Orbwalker.DisableMovement = true;
                    Orbwalker.DisableAttacking = true;
                    R.Cast();
                }
            }
        }

        public static float CurrentTick
        {
            get
            {
                return (int)DateTime.Now.Subtract(assemblyLoadTime).TotalMilliseconds;
            }
        }

        public static void Spellbook_OnCastSpell(Spellbook sender, SpellbookCastSpellEventArgs eventArgs)
        {
            if (!Humanizer["useHumanizer"].Cast<CheckBox>().CurrentValue)
                return;

            if (!sender.Owner.IsMe)
                return;

            var delayTime = Humanizer["delayTime"].Cast<Slider>().CurrentValue;
            var delayTimeCasts = Humanizer["delayTimeCasts"].Cast<Slider>().CurrentValue;

            Vector2 tempvect = new Vector2(LatestCast.X, LatestCast.Y);
            LatestCast.Timepass = CurrentTick - LatestCast.Tick;

            LatestCast.Distance = tempvect.Distance(eventArgs.StartPosition);
            LatestCast.Delay = (LatestCast.Distance * 0.001 * delayTime); ;
            if (CurrentTick < LatestCast.Tick + LatestCast.Delay && LatestCast.Timepass < 300)
            {
                eventArgs.Process = false;
                LatestCast.count += 1;
                LatestCast.SavedTime += LatestCast.Delay;

            }

            if (eventArgs.Process == true && LatestCast.Timepass > delayTimeCasts)
            {
                LatestCast.X = eventArgs.StartPosition.X;
                LatestCast.Y = eventArgs.StartPosition.Y;
                LatestCast.Tick = CurrentTick;
            }
        }
    }
}