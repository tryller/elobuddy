/*
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

namespace tryLulu
{
    internal class Program
    {
        public static Menu defaultMenu, comboMenu, harassMenu, laneClearMenu, ksMenu;
        public static Spell.Skillshot Q;
        public static Spell.Targeted W;
        public static Spell.Targeted E;
        public static Spell.Targeted R;
        public static Spell.Targeted Ignite;

        private static void Main(string[] args)
        {
            Loading.OnLoadingComplete += OnComplete;
        }

        private static void OnComplete(EventArgs args)
        {
            if (Player.Instance.Hero != Champion.Lulu)
                return;

            Bootstrap.Init(null);

            Q = new Spell.Skillshot(SpellSlot.Q, 950, SkillShotType.Linear, 60, 1400, 150);
            W = new Spell.Targeted(SpellSlot.W, 650);
            E = new Spell.Targeted(SpellSlot.E, 650);
            R = new Spell.Targeted(SpellSlot.R, 900);

            defaultMenu = MainMenu.AddMenu("Lulu", "tryLulu");
            comboMenu = defaultMenu.AddSubMenu("Combo", "comboMenu");
            comboMenu.Add("useQ", new CheckBox("Use Q"));
            comboMenu.Add("useW", new CheckBox("Use W"));
            comboMenu.Add("useE", new CheckBox("Use E"));
            comboMenu.AddSeparator(30);
            comboMenu.Add("useAutoUlt", new CheckBox("Use Auto-Ultimate"));
            comboMenu.Add("minR", new Slider("Minimum health of ally or Lulu to cast (R)", 15, 0, 100));

            harassMenu = defaultMenu.AddSubMenu("Harass", "harassMenu");
            harassMenu.Add("useQ", new CheckBox("Use Q"));

            laneClearMenu = defaultMenu.AddSubMenu("Lane Clear", "laneClearMenu");
            laneClearMenu.Add("useQ", new CheckBox("Use Q"));
            laneClearMenu.Add("laneMana", new Slider("Minimun mana for Lane Clear", 0, 0, 100));

            ksMenu = defaultMenu.AddSubMenu("Kill Steal", "ksMenu");
            ksMenu.Add("useIgnite", new CheckBox("Use Ignite"));

            Ignite = new Spell.Targeted(ObjectManager.Player.GetSpellSlotFromName("summonerdot"), 600);
            Game.OnTick += Game_OnTick;
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

            // Auto ult
            if (comboMenu["useAutoUlt"].Cast<CheckBox>().CurrentValue)
            {
                Modes.Ultimate();
            }

            // Auto Ignite
            if (ksMenu["useIgnite"].Cast<CheckBox>().CurrentValue && Ignite.IsReady())
            {
                Modes.AutoIgnite();
            }
        }
    }
}
