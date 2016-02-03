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
    static class Akali
    {
        private static readonly Item Hex = new Item(3146);
        private static readonly Item Cutlass = new Item(3144);

        public static Spell.Targeted Q;
        //public static Spell.Skillshot W;
        public static Spell.Active E;
        public static Spell.Targeted R;

        public static AIHeroClient target;

        public static Menu Menu;
        public static string G_charname = Program.myHero.ChampionName;

        public static void Init()
        {
            Q = new Spell.Targeted(SpellSlot.Q, 600);
            //W = new Spell.Skillshot(SpellSlot.W, 700, SkillShotType.Circular);
            E = new Spell.Active(SpellSlot.E, 325);
            R = new Spell.Targeted(SpellSlot.R, 700);

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

        public static void CastItems(AIHeroClient target)
        {
            if (!Program.myHero.IsRecalling() && Hex.IsReady())
            {
                Hex.Cast(target);
            }

            if (!Program.myHero.IsRecalling() && Cutlass.IsReady())
            {
                Cutlass.Cast(target);
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

                if (R.IsReady() && Program.myHero.Distance(target.Position) <= R.Range && !target.HasBuffOfType(BuffType.Invulnerability))
                {
                    R.Cast(target);
                }

                if (E.IsReady() && target.IsValidTarget(E.Range) && !target.HasBuffOfType(BuffType.Invulnerability))
                {
                    E.Cast();
                }
            }
        }
    }
}