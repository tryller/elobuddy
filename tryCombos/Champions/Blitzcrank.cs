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
    static class Blitzcrank
    {
        public static Spell.Skillshot Q;
        public static Spell.Active W;
        public static Spell.Active E;
        public static Spell.Active R;

        public static AIHeroClient target;

        public static Menu Menu, QMenu;
        public static string G_charname = Program.myHero.ChampionName;

        public static void Init()
        {
            Q = new Spell.Skillshot(SpellSlot.Q, 980, SkillShotType.Linear, (int)250f, (int)1800f, (int)70f);
            W = new Spell.Active(SpellSlot.W, 0);
            E = new Spell.Active(SpellSlot.E, 150);
            R = new Spell.Active(SpellSlot.R, 550);

            Menu = MainMenu.AddMenu(G_charname, G_charname.ToLower());

            Menu.AddGroupLabel("Welcome to " + G_charname);
            Menu.AddLabel("Addon that contains combos for all champions.");
            Menu.AddLabel("This adodn don't have anny config.");
            Menu.AddLabel("But it have full combo for LOL Champions.");
            Menu.AddSeparator();
            Menu.AddLabel("By Tryller");

            QMenu = Menu.AddSubMenu("Q Menu", "QMenu");
            QMenu.AddLabel("Never Grab");
            foreach (var hero in EntityManager.Heroes.Enemies)
            {
                QMenu.Add("dontgrab" + hero.ChampionName.ToLower(),
                    TargetSelector.GetPriority(hero) <= 2
                        ? new CheckBox(hero.ChampionName)
                        : new CheckBox(hero.ChampionName, false));
            }

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

            if (target != null && target.IsValidTarget(Q.Range)
                && !target.HasBuff("sionpassivezombie")               //sion Passive
                && !target.HasBuff("KarthusDeathDefiedBuff")          //karthus passive
                && !target.HasBuff("kogmawicathiansurprise")          //kog'maw passive
                && !target.HasBuff("zyrapqueenofthorns")              //zyra passive
                && !target.HasBuff("ChronoShift")                     //zilean R
                && !target.HasBuff("yorickrazombie")                  //yorick R
                && !target.HasBuffOfType(BuffType.Invulnerability))                 
            {

                if (Q.IsReady() && target.IsValidTarget(Q.Range))
                {
                    if (target == null || !target.IsValidTarget(Q.Range) || Q.GetPrediction(target).HitChance < HitChance.Medium)
                        return;

                    Q.Cast(Q.GetPrediction(target).CastPosition);
                }

                if (E.IsReady())
                {
                    var enemy = EntityManager.Heroes.Enemies.FirstOrDefault(e => Program.myHero.Distance(e) < 300);
                    if (enemy != null)
                    {
                        Orbwalker.DisableMovement = true;
                        Orbwalker.DisableAttacking = true;

                        E.Cast();
                        EloBuddy.Player.IssueOrder(GameObjectOrder.AttackUnit, enemy);
                        Orbwalker.DisableMovement = false;
                        Orbwalker.DisableAttacking = false;
                    }
                }

                if (R.IsReady() && EntityManager.Heroes.Enemies.Count(e => Program.myHero.Distance(e) < R.Range) >= 1 && R.IsReady() && !target.HasBuffOfType(BuffType.Invulnerability))
                {
                    R.Cast();
                }
            }
        }
    }
}