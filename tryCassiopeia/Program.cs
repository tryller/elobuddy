/*
    v1.1.1 - Add ignite if enemy is killable.
    v1.1.0 - Add lane clear.
    v1.0.0 - Initial Release.
*/

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

namespace tryCassiopeia
{
    internal class Program
    {
        public static Menu defaultMenu, comboMenu, harassMenu, laneClearMenu, ksMenu;
        public static Spell.Skillshot Q;
        public static Spell.Skillshot W;
        public static Spell.Targeted E;
        public static Spell.Skillshot R;
        public static Spell.Targeted Ignite;

        private static long LastQ = 0;
        private static long LastE = 0;

        private static void Main(string[] args)
        {
            Loading.OnLoadingComplete += OnComplete;
        }

        private static void OnComplete(EventArgs args)
        {
            if (Player.Instance.Hero != Champion.Cassiopeia)
                return;

            Bootstrap.Init(null);
            myTarget.init();
            Q = new Spell.Skillshot(SpellSlot.Q, 850, SkillShotType.Circular, 750, int.MaxValue, 150);
            W = new Spell.Skillshot(SpellSlot.W, 850, SkillShotType.Circular, 250, 2500, 250);
            E = new Spell.Targeted(SpellSlot.E, 700);
            R = new Spell.Skillshot(SpellSlot.R, 825, SkillShotType.Cone, (int)0.6f, int.MaxValue, (int)(80 * Math.PI / 180));


            defaultMenu = MainMenu.AddMenu("Cassiopeia", "tryCassiopeia");
            comboMenu = defaultMenu.AddSubMenu("Combo Menu", "comboMenu");
            comboMenu.Add("useQ", new CheckBox("Use Q"));
            comboMenu.Add("useW", new CheckBox("Use W"));
            comboMenu.Add("useE", new CheckBox("Use E"));
            comboMenu.AddSeparator();
            comboMenu.Add("useAutoUlt", new CheckBox("Use Auto-Ultimate"));
            comboMenu.Add("ultimateInterrupt", new CheckBox("Use (R) to Interrupt Spells"));
            comboMenu.Add("minR", new Slider("Minimum enemies to cast (R)", 2, 1, 5));

            harassMenu = defaultMenu.AddSubMenu("Harass Menu", "harassMenu");
            harassMenu.Add("useQ", new CheckBox("Use Q"));
            harassMenu.Add("useE", new CheckBox("Use E"));
            harassMenu.Add("useQToggle", new KeyBind("Q Toggle Harass", false, KeyBind.BindTypes.PressToggle, 'A'));

            laneClearMenu = defaultMenu.AddSubMenu("Lane Clear Menu", "laneClearMenu");
            laneClearMenu.Add("useQ", new CheckBox("Use Q"));
            laneClearMenu.Add("useW", new CheckBox("Use W"));
            laneClearMenu.Add("useE", new CheckBox("Use E"));
            laneClearMenu.Add("laneMana", new Slider("Minimun mana for Lane Clear", 0, 0, 100));

            ksMenu = defaultMenu.AddSubMenu("Kill Steal Menu", "ksMenu");
            ksMenu.Add("useIgnite", new CheckBox("Use Q"));

            Ignite = new Spell.Targeted(ObjectManager.Player.GetSpellSlotFromName("summonerdot"), 600);
            Interrupter.OnInterruptableSpell += OnInterruptableSpell;
            Game.OnTick += Game_OnTick;
        }

        private static void OnInterruptableSpell(Obj_AI_Base sender, Interrupter.InterruptableSpellEventArgs e)
        {
            if (comboMenu["ultimateInterruptt"].Cast<CheckBox>().CurrentValue)
            {
                if (sender.IsValidTarget(R.Range))
                {
                    if (R.IsReady())
                        R.Cast(sender);
                }
            }
        }

        private static void Game_OnTick(EventArgs args)
        {
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
            {
                Modes.Combo();
            }

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass))
            {
                Modes.Harass();
            }

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LastHit))
            {
                Modes.LastHit();
            }

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear))
            {
                Modes.LaneClear();
            }

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.JungleClear))
            {
                //JungleClear();
            }

            {//Ult control
                Modes.Ultimate();
            }
            {//Toggle harass
                Modes.ToggleHarass();
            }
            {//ignite
                Modes.AutoIgnite();
            }
        }
    }
}
