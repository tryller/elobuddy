using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using SharpDX;

namespace PandaTeemoReborn
{
    internal class Config
    {        
        public static Menu PandaTeemoReborn,
            ComboMenu,
            HarassMenu,
            LaneClearMenu,
            JungleClearMenu,
            KillStealMenu,
            FleeMenu,
            DrawingMenu,
            MiscMenu,
            AutoShroomMenu;

        static Config()
        {
            PandaTeemoReborn = MainMenu.AddMenu("PandaTeemoReborn", "PTR");
            PandaTeemoReborn.AddGroupLabel("This addon is made by KarmaPanda and should not be redistributed in any way.");
            PandaTeemoReborn.AddGroupLabel(
                "Any unauthorized redistribution without credits will result in severe consequences.");
            PandaTeemoReborn.AddGroupLabel("Thank you for using this addon and have a fun time!");
            
            ComboMenu = PandaTeemoReborn.AddSubMenu("Combo", "Combo");
            ComboMenu.AddLabel("Spell Settings");
            ComboMenu.Add("useQ", new CheckBox("Use Q in Combo"));
            ComboMenu.Add("useW", new CheckBox("Use W in Combo"));
            ComboMenu.Add("useR", new CheckBox("Use R in Combo"));
            ComboMenu.AddLabel("ManaManager");
            ComboMenu.Add("manaQ", new Slider("Mana before casting Q"));
            ComboMenu.Add("manaW", new Slider("Mana before casting W"));
            ComboMenu.Add("manaR", new Slider("Mana before casting R"));
            ComboMenu.AddLabel("Q Settings");
            ComboMenu.Add("checkAA", new Slider("Range to subtract from Q: {0}", 0, 0, 180));
            ComboMenu.AddLabel("R Settings");
            ComboMenu.Add("doubleShroom", new CheckBox("Use Double Shroom Logic"));
            ComboMenu.Add("rPoison", new CheckBox("Cast R only if target isn't Poisoned"));
            ComboMenu.Add("rCharge", new Slider("Charges of R before using R: {0}", 2, 1, 3));
            //ComboMenu.Add("rDelay", new Slider("Delay for R Casting in ms: {0}", 1000, 0, 5000));
            ComboMenu.AddLabel("Misc Settings");
            ComboMenu.Add("adc", new CheckBox("Use Q only on ADC", false));
            ComboMenu.Add("wRange", new CheckBox("Use W only if enemy is in range"));

            HarassMenu = PandaTeemoReborn.AddSubMenu("Harass", "Harass");
            HarassMenu.AddGroupLabel("Spell Settings");
            HarassMenu.Add("useQ", new CheckBox("Use Q in Harass"));
            HarassMenu.Add("useW", new CheckBox("Use W in Harass", false));
            HarassMenu.AddLabel("ManaManager");
            HarassMenu.Add("manaQ", new Slider("Mana before casting Q"));
            HarassMenu.Add("manaW", new Slider("Mana before casting W"));
            HarassMenu.AddLabel("Q Settings");
            HarassMenu.Add("checkAA", new Slider("Range to subtract from Q: {0}", 0, 0, 180));
            HarassMenu.AddLabel("Misc Settings");
            HarassMenu.Add("adc", new CheckBox("Use Q only on ADC", false));
            HarassMenu.Add("wRange", new CheckBox("Use W only if enemy is in range"));

            LaneClearMenu = PandaTeemoReborn.AddSubMenu("LaneClear", "LaneClear");
            LaneClearMenu.AddLabel("Spell Settings");
            LaneClearMenu.Add("useQ", new CheckBox("LaneClear with Q"));
            LaneClearMenu.Add("useR", new CheckBox("LaneClear with R"));
            LaneClearMenu.AddLabel("ManaManager");
            LaneClearMenu.Add("manaQ", new Slider("Q Mana Manager", 50));
            LaneClearMenu.Add("manaR", new Slider("R Mana Manager", 50));
            LaneClearMenu.AddLabel("Q Settings");
            LaneClearMenu.Add("harass", new CheckBox("Use Harass Based Logic"));
            LaneClearMenu.Add("disableLC", new CheckBox("Disable LaneClear Based Logic"));
            LaneClearMenu.AddLabel("R Settings");
            LaneClearMenu.Add("rKillable", new CheckBox("Only cast R if minion(s) are killable"));
            LaneClearMenu.Add("rPoison", new CheckBox("Cast R only if minion isn't Poisoned"));
            LaneClearMenu.Add("rCharge", new Slider("Charges of R before using R: {0}", 2, 1, 3));
            //LaneClearMenu.Add("rDelay", new Slider("Delay for R Casting in ms: {0}", 1000, 0, 5000));
            LaneClearMenu.Add("minionR", new Slider("Minions before casting R: {0}", 3, 1, 4));

            JungleClearMenu = PandaTeemoReborn.AddSubMenu("JungleClear", "JungleClear");
            JungleClearMenu.AddGroupLabel("Spell Settings");
            JungleClearMenu.Add("useQ", new CheckBox("JungleClear with Q"));
            JungleClearMenu.Add("useR", new CheckBox("JungleClear with R"));
            JungleClearMenu.AddLabel("ManaManager");
            JungleClearMenu.Add("manaQ", new Slider("Q Mana Manager", 25));
            JungleClearMenu.Add("manaR", new Slider("R Mana Manager", 25));
            JungleClearMenu.AddLabel("R Settings");
            JungleClearMenu.Add("rKillable", new CheckBox("Only cast R if mob(s) are killable", false));
            JungleClearMenu.Add("rPoison", new CheckBox("Cast R only if mob isn't Poisoned"));
            JungleClearMenu.Add("rCharge", new Slider("Charges of R before using R: {0}", 2, 1, 3));
            //JungleClearMenu.Add("rDelay", new Slider("Delay for R Casting in ms: {0}", 1000, 0, 5000));
            JungleClearMenu.Add("mobR", new Slider("Mobs before casting R: {0}", 1, 1, 4));
            JungleClearMenu.AddLabel("Misc Settings");
            JungleClearMenu.Add("bMob", new CheckBox("Prevent Spell Usage on Small Mobs"));

            KillStealMenu = PandaTeemoReborn.AddSubMenu("Kill Steal", "Kill Steal");
            KillStealMenu.AddGroupLabel("Spell Settings");
            KillStealMenu.Add("useQ", new CheckBox("Kill Steal with Q"));
            KillStealMenu.Add("useR", new CheckBox("Kill Steal with R", false));
            KillStealMenu.AddLabel("ManaManager");
            KillStealMenu.Add("manaQ", new Slider("Q Mana Manager", 25));
            KillStealMenu.Add("manaR", new Slider("R Mana Manager", 25));
            KillStealMenu.AddLabel("R Settings");
            //KillStealMenu.Add("rDelay", new Slider("Delay for R Casting in ms: {0}", 1000, 0, 5000));
            KillStealMenu.Add("doubleShroom", new CheckBox("Use Double Shroom Logic"));

            FleeMenu = PandaTeemoReborn.AddSubMenu("Flee Menu", "Flee");
            FleeMenu.AddGroupLabel("Flee Settings");
            FleeMenu.Add("useW", new CheckBox("Flee with W"));
            FleeMenu.Add("useR", new CheckBox("Flee with R"));
            FleeMenu.AddLabel("R Settings");
            //FleeMenu.Add("rDelay", new Slider("Delay for R Casting in ms: {0}", 1000, 0, 5000));
            FleeMenu.Add("rCharge", new Slider("Charges of R before using R: {0}", 2, 1, 3));

            AutoShroomMenu = PandaTeemoReborn.AddSubMenu("Auto Shroom", "Auto Shroom");
            AutoShroomMenu.AddGroupLabel("Auto Shroom Settings");
            AutoShroomMenu.Add("useR", new CheckBox("AutoShroom with R"));
            AutoShroomMenu.Add("manaR", new Slider("R Mana Manager", 25));
            AutoShroomMenu.Add("rCharge", new Slider("Charges of R before using R: {0}", 2, 1, 3));
            AutoShroomMenu.Add("enableShroom", new CheckBox("Load AutoShroom (Requires F5)"));
            AutoShroomMenu.Add("enableDefaultLocations", new CheckBox("Use Default Locations (Requires F5)"));
            AutoShroomMenu.AddLabel("Debug Mode");
            var enable = AutoShroomMenu.Add("enableDebug", new CheckBox("Enable Debug Mode", false));
            enable.OnValueChange += delegate(ValueBase<bool> sender, ValueBase<bool>.ValueChangeArgs args)
            {
                if (!args.NewValue)
                {
                    Chat.Print("PandaTeemo | Debug Mode Disabled", System.Drawing.Color.LawnGreen);
                }
                else
                {
                    Chat.Print("PandaTeemo | Debug Mode Enabled", System.Drawing.Color.Red);
                }
            };
            var save = AutoShroomMenu.Add("saveButton", new KeyBind("Save Configuration", false, KeyBind.BindTypes.HoldActive, 'K'));
            save.OnValueChange += delegate(ValueBase<bool> sender, ValueBase<bool>.ValueChangeArgs args)
            {
                if (!args.NewValue)
                {
                    return;
                }

                if (Extensions.MenuValues.AutoShroom.DebugMode)
                {
                    save.CurrentValue = false;
                    AutoShroom.SavePositions();
                }
            };
            AutoShroomMenu.AddLabel("Shroom Location Adder");
            AutoShroomMenu.Add("posMode", new ComboBox("Position Mode", 0, "Save Mouse", "Save Player Position"));
            var add = AutoShroomMenu.Add("newposButton", new KeyBind("Save Position", false, KeyBind.BindTypes.HoldActive, 'L'));
            add.OnValueChange += delegate(ValueBase<bool> sender, ValueBase<bool>.ValueChangeArgs args)
            {
                if (!args.NewValue)
                {
                    return;
                }

                if (Extensions.MenuValues.AutoShroom.DebugMode)
                {
                    add.CurrentValue = false;

                    Vector3 newPosition = Vector3.Zero;

                    switch (Extensions.MenuValues.AutoShroom.PositionMode.CurrentValue)
                    {
                        case 0:
                            newPosition = Game.CursorPos;
                            break;
                        case 1:
                            newPosition = Player.Instance.Position;
                            break;
                    }

                    if (newPosition != Vector3.Zero && !AutoShroom.ShroomPosition.Contains(newPosition))
                    {
                        AutoShroom.AddShroomLocation(newPosition);
                        AutoShroom.SavePositions();
                    }
                }
            };
            var remove = AutoShroomMenu.Add("delposButton", new KeyBind("Delete Position", false, KeyBind.BindTypes.HoldActive, 'U'));
            remove.OnValueChange += delegate(ValueBase<bool> sender, ValueBase<bool>.ValueChangeArgs args)
            {
                if (!args.NewValue)
                {
                    return;
                }

                if (Extensions.MenuValues.AutoShroom.DebugMode)
                {
                    remove.CurrentValue = false;
                }

                Vector3 newPosition = Vector3.Zero;

                switch (Extensions.MenuValues.AutoShroom.PositionMode.CurrentValue)
                {
                    case 0:
                        newPosition = Game.CursorPos;
                        break;
                    case 1:
                        newPosition = Player.Instance.Position;
                        break;
                }

                if (newPosition == Vector3.Zero) return;

                var nearbyShrooms = AutoShroom.PlayerAssignedShroomPosition.Where(pos => pos.IsInRange(newPosition, SpellManager.R.Radius)).ToList();

                if (!nearbyShrooms.Any())
                {
                    return;
                }

                AutoShroom.RemoveShroomLocations(nearbyShrooms);
                AutoShroom.SavePositions();
            };

            DrawingMenu = PandaTeemoReborn.AddSubMenu("Drawing", "Drawing");
            DrawingMenu.AddGroupLabel("Drawing Settings");
            DrawingMenu.Add("drawQ", new CheckBox("Draw Q Range"));
            DrawingMenu.Add("drawR", new CheckBox("Draw R Range"));
            DrawingMenu.Add("drawautoR", new CheckBox("Draw Auto Shroom Positions"));
            DrawingMenu.Add("drawdoubleR", new CheckBox("Draw Double Shroom Prediction", false));
            
            MiscMenu = PandaTeemoReborn.AddSubMenu("Misc", "Misc");
            MiscMenu.AddGroupLabel("Spell Settings");
            MiscMenu.Add("autoQ", new CheckBox("Automatic Q", false));
            MiscMenu.Add("autoW", new CheckBox("Automatic W", false));
            MiscMenu.Add("intq", new CheckBox("Interrupt with Q"));
            MiscMenu.Add("gapR", new CheckBox("Gapcloser with R"));
        }

        public static void Initialize()
        {
        }
    }
}