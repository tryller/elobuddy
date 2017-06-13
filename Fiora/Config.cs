using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using System;
using System.Collections.Generic;
using System.Linq;
using SharpDX;

namespace JokerFioraBuddy
{
    public static class Config
    {
        private const string MenuName = "Joker Fiora 2.0.0.2";

        private static readonly Menu Menu;

        static Config() 
        {
            Menu = MainMenu.AddMenu(MenuName, MenuName.ToLower());
            Menu.AddGroupLabel("Welcome to Joker Fiora Addon!");
            Menu.AddLabel("Features:");
            Menu.AddLabel("- Epic Combo! 100-0 in 2 seconds.");
            Menu.AddLabel("- Auto Shield Block (W).");
            Menu.AddLabel("- Auto Dispell Channelling Spells (W).");
            Menu.AddLabel("- Harass Mode with all spells.");
            Menu.AddLabel("- Last Hit Mode with Q.");
            Menu.AddLabel("- Lane Clear Mode with Q/E.");
            Menu.AddLabel("- Flee Mode with Q.");
            Menu.AddLabel("- Smart Target Selector.");
            Menu.AddLabel("- Auto-Ignite!");
            Menu.AddLabel("- Champion 1 shot combo indicator!");
            Menu.AddLabel("All customizable! Featuring Youmuu's Ghostblade / Ravenous Hydra / Blade of the Ruined King");
            Menu.AddLabel("Credits to: Danny - Main Coder / Trees - Shield Block / Fluxy - Target Selector 2");

            Modes.Initialize();
            ShieldBlock.Initialize();
            Dispell.Initialize();
            Misc.Initialize();
        }

        public static void Initialize() 
        {
 
        }

        public static class ShieldBlock
        {
            public static readonly Menu Menu;

            public static bool BlockSpells
            {
                get { return Menu["blockSpellsW"].Cast<CheckBox>().CurrentValue; }
            }

            public static bool EvadeIntegration
            {
                get { return Menu["evade"].Cast<CheckBox>().CurrentValue; }
            }

            static ShieldBlock()
            {
                Menu = Config.Menu.AddSubMenu("Spell Block");
                Menu.AddGroupLabel("Core Options");
                Menu.Add("blockSpellsW", new CheckBox("Auto-Block Spells (W)"));
                Menu.Add("evade", new CheckBox("Evade Integration"));
                Menu.AddSeparator();

                Menu.AddGroupLabel("Enemies spells to block");
            }

            public static void Initialize()
            {

            }
        }

        public static class Dispell
        {
            public static readonly Menu Menu;

            public static bool DispellSpells
            {
                get { return Menu["dispellSpellsW"].Cast<CheckBox>().CurrentValue; }
            }

            static Dispell()
            {
                Menu = Config.Menu.AddSubMenu("Dispeller");
                Menu.AddGroupLabel("Core Options");
                Menu.Add("dispellSpellsW", new CheckBox("Auto-Dispell Channeling Spells (W)"));
               Menu.AddSeparator();

                Menu.AddGroupLabel("Enemies spells to dispell");
            }

            public static void Initialize()
            {

            }
        }

        public static class Misc
        {
            private static readonly Menu Menu;

            public static int SkinID
            {
                get { return Menu["skinid"].Cast<Slider>().CurrentValue; }
            }

            public static bool enableSkinHack
            {
                get { return Menu["skinhack"].Cast<CheckBox>().CurrentValue; }
            }

            public static bool enableLevelUP
            {
                get { return Menu["evolveskills"].Cast<CheckBox>().CurrentValue; }
            }

            public static Slider SkinSlider = new Slider("SkinID : ({0})", 0, 0, 4);
            public static CheckBox SkinEnable = new CheckBox("Enable");
            public static CheckBox EvolveEnable = new CheckBox("Enable");


            static Misc()
            {
                Menu = Config.Menu.AddSubMenu("Misc");
                Menu.AddGroupLabel("Skin Hack");
                Menu.Add("skinhack", SkinEnable);
                Menu.Add("skinid", SkinSlider);
                Menu.AddGroupLabel("Auto Level Skills");
                Menu.Add("evolveskills", EvolveEnable);

                SkinSlider.OnValueChange += SkinSlider_OnValueChange;
                SkinEnable.OnValueChange += SkinEnable_OnValueChange;
            }

            private static void SkinEnable_OnValueChange(ValueBase<bool> sender, ValueBase<bool>.ValueChangeArgs args)
            {
                if (!Misc.enableSkinHack)
                {
                    Player.SetSkinId(0);
                    return;
                }

                Player.SetSkinId(Misc.SkinID);
            }

            private static void SkinSlider_OnValueChange(ValueBase<int> sender, ValueBase<int>.ValueChangeArgs args)
            {
                if (!Misc.enableSkinHack)
                {
                    Player.SetSkinId(0);
                    return;
                }

                Player.SetSkinId(Misc.SkinID);
            }

            public static void Initialize()
            {

            }

        }

        public static class Modes
        {
            private static readonly Menu Menu;

            static Modes()
            {
                Menu = Config.Menu.AddSubMenu("Modes");

                Combo.Initialize();
                Menu.AddSeparator();

                Harass.Initialize();
                Menu.AddSeparator();

                LaneClear.Initialize();
                Menu.AddSeparator();

                LastHit.Initialize();
                Menu.AddSeparator();

                Flee.Initialize();
                Menu.AddSeparator();

                Perma.Initialize();
                
            }

            public static void Initialize()
            {

            }

            public static class Combo
            {
                static List<EloBuddy.AIHeroClient> enemies = EntityManager.Heroes.Enemies;
                public static bool UseQ
                {
                    get { return Menu["comboUseQ"].Cast<CheckBox>().CurrentValue; }
                }

                public static bool UseE
                {
                    get { return Menu["comboUseE"].Cast<CheckBox>().CurrentValue; }
                }

                public static bool UseR
                {
                    get { return Menu["comboUseR"].Cast<CheckBox>().CurrentValue; }
                }

                public static bool UseTiamatHydra
                {
                    get { return Menu["comboUseTiamatHydra"].Cast<CheckBox>().CurrentValue; }
                }

                public static bool UseCutlassBOTRK
                {
                    get { return Menu["comboUseCutlassBOTRK"].Cast<CheckBox>().CurrentValue; }
                }

                public static bool UseYomuus
                {
                    get { return Menu["comboUseYomuus"].Cast<CheckBox>().CurrentValue; }
                }

                public static bool useRonTarget(string x)
                {
                     return Menu[x].Cast<CheckBox>().CurrentValue; 
                }

                public static int rSliderValue()
                {
                    return Menu["useRSlider"].Cast<Slider>().CurrentValue;
                }
                static Combo()
                {
                    Menu.AddGroupLabel("Combo");
                    Menu.Add("comboUseQ", new CheckBox("Use Q"));
                    Menu.Add("comboUseE", new CheckBox("Use E"));
                    Menu.Add("comboUseR", new CheckBox("Use R"));
                    Menu.Add("useRSlider", new Slider("Use R when HP is below  ({0}%)", 70));
                    Menu.Add("comboUseTiamatHydra", new CheckBox("Use Tiamat / Hydra"));
                    Menu.Add("comboUseCutlassBOTRK", new CheckBox("Use Bilgewater Cutlass / Blade of the Ruined King"));
                    Menu.Add("comboUseYomuus", new CheckBox("Use Youmuu's Ghostblade"));
                    Menu.AddSeparator();
                }

                public static void Initialize()
                {
                    Menu.AddLabel("Use R on");
                    foreach (var unit in enemies)
                    {
                        Menu.Add(unit.ChampionName, new CheckBox(unit.ChampionName));
                    }
                }
            }

            public static class Harass
            {
                public static bool UseQ
                {
                    get { return Menu["harassUseQ"].Cast<CheckBox>().CurrentValue; }
                }

                public static bool UseE
                {
                    get { return Menu["harassUseE"].Cast<CheckBox>().CurrentValue; }
                }

                public static bool UseR
                {
                    get { return Menu["harassUseR"].Cast<CheckBox>().CurrentValue; }
                }

                public static bool UseTiamatHydra
                {
                    get { return Menu["harassUseTiamatHydra"].Cast<CheckBox>().CurrentValue; }
                }

                public static int Mana
                {
                    get { return Menu["harassMana"].Cast<Slider>().CurrentValue; }
                }

                static Harass()
                {
                    Menu.AddGroupLabel("Harrass");
                    Menu.Add("harassUseQ", new CheckBox("Use Q"));
                    Menu.Add("harassUseE", new CheckBox("Use E"));
                    Menu.Add("harassUseR", new CheckBox("Use R", false));
                    Menu.Add("harassUseTiamatHydra", new CheckBox("Use Tiamat / Hydra"));
                    Menu.Add("harassMana", new Slider("Maximum mana usage in percent ({0}%)", 40));
                }

                public static void Initialize()
                {

                }
            }

            public static class LaneClear
            {
                public static bool UseQ
                {
                    get { return Menu["lcUseQ"].Cast<CheckBox>().CurrentValue; }
                }

                public static bool UseE
                {
                    get { return Menu["lcUseE"].Cast<CheckBox>().CurrentValue; }
                }

                public static bool UseTiamatHydra
                {
                    get { return Menu["lcUseTiamatHydra"].Cast<CheckBox>().CurrentValue; }
                }

                public static int Mana
                {
                    get { return Menu["lcMana"].Cast<Slider>().CurrentValue; }
                }

                static LaneClear()
                {
                    Menu.AddGroupLabel("Lane Clear");
                    Menu.Add("lcUseQ", new CheckBox("Use Q"));
                    Menu.Add("lcUseE", new CheckBox("Use E"));
                    Menu.Add("lcUseTiamatHydra", new CheckBox("Use Tiamat / Hydra"));
                    Menu.Add("lcMana", new Slider("Maximum mana usage in percent ({0}%)", 40));
                }

                public static void Initialize()
                {

                }
            }

            public static class LastHit
            {
                public static bool UseQ
                {
                    get { return Menu["lhUseQ"].Cast<CheckBox>().CurrentValue; }
                }

                public static int Mana
                {
                    get { return Menu["lhMana"].Cast<Slider>().CurrentValue; }
                }

                static LastHit()
                {
                    Menu.AddGroupLabel("Last Hit");
                    Menu.Add("lhUseQ", new CheckBox("Use Q"));
                    Menu.Add("lhMana", new Slider("Maximum mana usage in percent ({0}%)", 40));
                }

                public static void Initialize()
                {

                }
            }

            public static class Flee
            {
                public static bool UseQ
                {
                    get { return Menu["fleeUseQ"].Cast<CheckBox>().CurrentValue; }
                }

                static Flee()
                {
                    Menu.AddGroupLabel("Flee");
                    Menu.Add("fleeUseQ", new CheckBox("Use Q"));
                }

                public static void Initialize()
                {

                }
            }

            public static class Perma
            {
                static Slider igniteModeSlider = new Slider("Ignite Mode : Smart", 1, 0, 1);

               

                public static bool UseIgnite
                {
                    get { return Menu["permaUseIG"].Cast<CheckBox>().CurrentValue; }
                }

                public static bool UseW
                {
                    get { return Menu["useWifKillable"].Cast<CheckBox>().CurrentValue; }
                }

                public static int igniteMode
                {
                    get { return Menu["igniteMode"].Cast<Slider>().CurrentValue; }
                }

                
                static Perma()
                {
                    Menu.AddGroupLabel("Perma Active");
                    Menu.Add("permaUseIG", new CheckBox("Auto-Ignite Champions"));
                    Menu.Add("igniteMode", igniteModeSlider);
                    Menu.Add("useWifKillable", new CheckBox("Auto-W if enemy is killable"));

                }

                public static void Initialize()
                {
                    igniteModeSlider.OnValueChange += IgniteModeSlider_OnValueChange;
                }

                private static void IgniteModeSlider_OnValueChange(ValueBase<int> sender, ValueBase<int>.ValueChangeArgs args)
                {
                    if (igniteModeSlider.CurrentValue == 0)
                        igniteModeSlider.DisplayName = "Ignite Mode : Normal Mode";
                    else
                        igniteModeSlider.DisplayName = "Ignite Mode : Smart Mode";
                }
            }
        }
    }
}
