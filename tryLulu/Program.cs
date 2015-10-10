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
        public static Menu defaultMenu, comboMenu, harassMenu, laneClearMenu, miscMenu;
        public static Spell.Skillshot Q;
        public static Spell.Targeted W;
        public static Spell.Targeted E;
        public static Spell.Targeted R;
        public static Spell.Targeted Ignite;

        private static void Main(string[] args)
        {
            Loading.OnLoadingComplete += OnLoadingComplete;
        }

        private static void OnLoadingComplete(EventArgs args)
        {
            if (Player.Instance.Hero != Champion.Lulu)
                return;

            Q = new Spell.Skillshot(SpellSlot.Q, 950, SkillShotType.Linear, 60, 1400, 150);
            W = new Spell.Targeted(SpellSlot.W, 650);
            E = new Spell.Targeted(SpellSlot.E, 650);
            R = new Spell.Targeted(SpellSlot.R, 900);

            defaultMenu = MainMenu.AddMenu("Lulu", "Lulu");
            comboMenu = defaultMenu.AddSubMenu("Combo", "comboMenu");
            comboMenu.Add("useQ", new CheckBox("Use Q"));
            comboMenu.Add("useW", new CheckBox("Use W"));
            comboMenu.Add("useE", new CheckBox("Use E"));
            comboMenu.AddLabel("<---> Ultimate Settings <--->");
            comboMenu.Add("useAutoUlt", new CheckBox("Use Auto-Ultimate"));
            comboMenu.Add("minR", new Slider("Minimum health % of ally or Lulu to cast (R)", 15, 0, 100));

            harassMenu = defaultMenu.AddSubMenu("Harass", "harassMenu");
            harassMenu.Add("useQ", new CheckBox("Use Q"));

            laneClearMenu = defaultMenu.AddSubMenu("Lane Clear", "laneClearMenu");
            laneClearMenu.Add("useQ", new CheckBox("Use Q"));
            laneClearMenu.Add("laneMana", new Slider("Minimun mana for Lane Clear", 0, 0, 100));

            miscMenu = defaultMenu.AddSubMenu("Misc", "miscMenu");
            miscMenu.AddLabel("<---> Item Usage <--->");
            miscMenu.Add("useZhonya", new CheckBox("Use Auto-Zhonyas"));
            miscMenu.Add("minZhonyaHealth", new Slider("Minimum health % to use Zhonyas", 15, 0, 100));
            miscMenu.AddLabel("<---> Kill Steal <--->");
            miscMenu.Add("ksQ", new CheckBox("Kill Steal (Q)"));
            miscMenu.Add("ksE", new CheckBox("Kill Steal (E)"));
            miscMenu.Add("useIgnite", new CheckBox("Use Ignite if enemy killable"));
            miscMenu.AddLabel("<---> Anti Gapcloser <--->");
            miscMenu.Add("gapW", new CheckBox("Anti-Gabpcloser with (W)", false));
            miscMenu.Add("gapE", new CheckBox("Anti-Gabpcloser with (E)", false));
            miscMenu.AddLabel("<---> Skin Changer <--->");
            var skin = miscMenu.Add("skinID", new Slider("Skin", 0, 0, 5));
            var sID = new[] {"Classic",
                "Bittersweet Lulu",
                "Wicked Lulu",
                "Dragon Trainer Lulu",
                "Winter Wonder Lulu",
                "Pool Paerty Lulu"};
            skin.DisplayName = sID[skin.CurrentValue];
            skin.OnValueChange += delegate(ValueBase<int> sender, ValueBase<int>.ValueChangeArgs changeArgs)
            {
                sender.DisplayName = sID[changeArgs.NewValue];
            };

            Ignite = new Spell.Targeted(ObjectManager.Player.GetSpellSlotFromName("summonerdot"), 600);

            Gapcloser.OnGapcloser += OnGapcloser;
            Game.OnTick += Game_OnTick;
        }

        private static void OnGapcloser(AIHeroClient sender, Gapcloser.GapcloserEventArgs gapcloser)
        {
            if (miscMenu["gapW"].Cast<CheckBox>().CurrentValue)
            {
                if (W.IsReady() && ObjectManager.Player.Distance(gapcloser.Sender, true) < W.Range * W.Range)
                {
                    W.Cast(gapcloser.Sender);
                }
            }

            if (miscMenu["gapE"].Cast<CheckBox>().CurrentValue)
            {
                if (E.IsReady() && ObjectManager.Player.Distance(gapcloser.Sender, true) < E.Range * E.Range)
                {
                    E.Cast(ObjectManager.Player);
                }
            }
        }

        private static void Game_OnTick(EventArgs args)
        {
            try
            {
                Modes.ChangeSkin();
                Modes.KillSteal();

                if (miscMenu["useZhonya"].Cast<CheckBox>().CurrentValue)
                {
                    Modes.AutoZhonya();
                }

                if (comboMenu["useAutoUlt"].Cast<CheckBox>().CurrentValue)
                {
                    Modes.Ultimate();
                }

                if (miscMenu["useIgnite"].Cast<CheckBox>().CurrentValue && Ignite.IsReady())
                {
                    Modes.AutoIgnite();
                }

                switch (Orbwalker.ActiveModesFlags)
                {
                    case Orbwalker.ActiveModes.Combo:
                        Modes.Combo();
                        break;

                    case Orbwalker.ActiveModes.Harass:
                        Modes.Harass();
                        break;

                    case Orbwalker.ActiveModes.LaneClear:
                        Modes.LaneClear();
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.Write("Error: "+ ex.Message.ToString());
            }
        }
    }
}