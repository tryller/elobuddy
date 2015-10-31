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

namespace tryKatarina
{
    class Program
    {
        public static Menu defaultMenu, comboMenu, harassMenu, laneClearMenu, miscMenu;
        public static Spell.Targeted Q;
        public static Spell.Active W;
        public static Spell.Targeted E;
        public static Spell.Active R;
        public static Spell.Targeted Ignite;

        static void Main(string[] args)
        {
            Loading.OnLoadingComplete += OnLoadingComplete;
        }

        private static void OnLoadingComplete(EventArgs args)
        {
            if (Player.Instance.Hero != Champion.Katarina)
                return;

            Q = new Spell.Targeted(SpellSlot.Q, 675);
            W = new Spell.Active(SpellSlot.W, 375);
            E = new Spell.Targeted(SpellSlot.E, 700);
            R = new Spell.Active(SpellSlot.R, 550);

            defaultMenu = MainMenu.AddMenu("Katarina", "Katarina");
            comboMenu = defaultMenu.AddSubMenu("Combo", "comboMenu");
            comboMenu.Add("useQ", new CheckBox("Use Q"));
            comboMenu.Add("useW", new CheckBox("Use W"));
            comboMenu.Add("useE", new CheckBox("Use E"));
            comboMenu.Add("useR", new CheckBox("Use R"));

            harassMenu = defaultMenu.AddSubMenu("Harass", "harassMenu");
            harassMenu.Add("useQ", new CheckBox("Use Q"));

            laneClearMenu = defaultMenu.AddSubMenu("Lane Clear", "laneClearMenu");
            laneClearMenu.Add("useQ", new CheckBox("Use Q"));
            laneClearMenu.Add("useW", new CheckBox("Use W"));
            laneClearMenu.Add("useE", new CheckBox("Use E"));

            miscMenu = defaultMenu.AddSubMenu("Misc", "miscMenu");
            miscMenu.AddLabel("<---> Item Usage <--->");
            miscMenu.Add("useZhonya", new CheckBox("Use Auto-Zhonyas"));
            miscMenu.Add("minZhonyaHealth", new Slider("Minimum health % to use Zhonyas", 15, 0, 100));
            miscMenu.AddLabel("<---> Kill Steal <--->");
            miscMenu.Add("ksQ", new CheckBox("Kill Steal (Q)"));
            miscMenu.Add("ksE", new CheckBox("Kill Steal (E)"));
            miscMenu.Add("useIgnite", new CheckBox("Use Ignite if enemy killable"));
            miscMenu.AddLabel("<---> Skin Changer <--->");
            var skin = miscMenu.Add("skinID", new Slider("Skin", 0, 0, 8));
            var sID = new[] {"Classic",
                "Mercenary Katarina",
                "Red Card Katarina",
                "Bilgewater Katarina",
                "Kitty Cat Katarina",
                "High Command Katarina",
            "Sandstorm Katarina",
            "Slay Belle Katarina",
            "Warring  Kingdoms Katarina"};
            skin.DisplayName = sID[skin.CurrentValue];
            skin.OnValueChange += delegate(ValueBase<int> sender, ValueBase<int>.ValueChangeArgs changeArgs)
            {
                sender.DisplayName = sID[changeArgs.NewValue];
            };

            Ignite = new Spell.Targeted(ObjectManager.Player.GetSpellSlotFromName("summonerdot"), 600);
            Game.OnTick += Game_OnTick;
        }

        static void Game_OnTick(EventArgs args)
        {
            try
            {
               // Modes.ChangeSkin();
               // Modes.KillSteal();

                if (miscMenu["useZhonya"].Cast<CheckBox>().CurrentValue)
                {
                    Modes.AutoZhonya();
                }

                if (miscMenu["useIgnite"].Cast<CheckBox>().CurrentValue && Ignite.IsReady())
                {
                    Modes.AutoIgnite();
                }

                switch (Orbwalker.ActiveModesFlags)
                {
                    case Orbwalker.ActiveModes.Combo:
                        Orbwalker.DisableAttacking = true;
                        Orbwalker.DisableMovement = true;
                        Modes.Combo();
                        Orbwalker.DisableAttacking = false;
                        Orbwalker.DisableMovement = false;
                        break;

                    case Orbwalker.ActiveModes.Harass:
                      //  Modes.Harass();
                        break;

                    case Orbwalker.ActiveModes.LaneClear:
                       // Modes.LaneClear();
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.Write("Error: " + ex.Message.ToString());
            }
        }
    }
}
