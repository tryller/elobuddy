using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;

namespace Farofakids_Heimerdinger
{
    internal class MENUS
    {
        private static Menu FarofakidsHeimerdingerMenu, ComboMenu, HarassMenu, MiscMenu;

        public static void Initialize()
        {
            FarofakidsHeimerdingerMenu = MainMenu.AddMenu("Farofakids Heimerdinger", "Farofakids-Heimerdinger");
            FarofakidsHeimerdingerMenu.AddGroupLabel("Farofakids Heimerdinger");

            // Combo Menu
            ComboMenu = FarofakidsHeimerdingerMenu.AddSubMenu("Combo Features", "ComboFeatures");
            ComboMenu.AddGroupLabel("Combo Features");
            ComboMenu.Add("UseQCombo", new CheckBox("Use Q"));
            ComboMenu.Add("UseQRCombo", new CheckBox("Use Q Upgrade", false));
            ComboMenu.Add("QRcount", new Slider("Minimum Enemies for Q upgrade", 2, 1, 5));
            ComboMenu.Add("UseWCombo", new CheckBox("Use W"));
            ComboMenu.Add("UseWRCombo", new CheckBox("Use W Upgrade"));
            ComboMenu.Add("UseECombo", new CheckBox("Use E"));
            ComboMenu.Add("UseERCombo", new CheckBox("Use ER"));
            ComboMenu.Add("ERcount", new Slider("Minimum Enemies for E upgrade", 3, 1, 5));
            ComboMenu.Add("UseRCombo", new CheckBox("Use R"));
            ComboMenu.Add("KS", new CheckBox("Killsteal"));

            // Harass Menu
            HarassMenu = FarofakidsHeimerdingerMenu.AddSubMenu("Harass Features", "HarassFeatures");
            HarassMenu.AddGroupLabel("Harass Features");
            HarassMenu.Add("UseWHarras", new CheckBox("Use W"));
            HarassMenu.Add("AutoHarras", new CheckBox("Auto Harass W", false));
            HarassMenu.AddSeparator(1);
            HarassMenu.Add("HarassMana", new Slider("Mana Limiter at Mana %", 40));
            // Setting Menu
            MiscMenu = FarofakidsHeimerdingerMenu.AddSubMenu("Settings", "Settings");
            MiscMenu.AddGroupLabel("Settings");
            MiscMenu.AddLabel("Interrupter");
            MiscMenu.Add("InterruptSpells", new CheckBox("Interrupt spells - E"));
            MiscMenu.Add("AntiGap", new CheckBox("Anti Gapcloser - E"));

        }

        //combo
        public static bool UseQCombo { get { return ComboMenu["UseQCombo"].Cast<CheckBox>().CurrentValue; } }
        public static bool UseQRCombo { get { return ComboMenu["UseQRCombo"].Cast<CheckBox>().CurrentValue; } }
        public static int QRcount { get { return ComboMenu["QRcount"].Cast<Slider>().CurrentValue; } }
        public static bool UseWCombo { get { return ComboMenu["UseWCombo"].Cast<CheckBox>().CurrentValue; } }
        public static bool UseWRCombo { get { return ComboMenu["UseWRCombo"].Cast<CheckBox>().CurrentValue; } }
        public static bool UseECombo { get { return ComboMenu["UseECombo"].Cast<CheckBox>().CurrentValue; } }
        public static bool UseERCombo { get { return ComboMenu["UseERCombo"].Cast<CheckBox>().CurrentValue; } }
        public static int ERcount { get { return ComboMenu["ERcount"].Cast<Slider>().CurrentValue; } }
        public static bool UseRCombo { get { return ComboMenu["UseRCombo"].Cast<CheckBox>().CurrentValue; } }
        public static bool KS { get { return ComboMenu["KS"].Cast<CheckBox>().CurrentValue; } }

        //harras
        public static bool UseWHarras { get { return HarassMenu["UseWHarras"].Cast<CheckBox>().CurrentValue; } }
        public static bool AutoHarras { get { return HarassMenu["AutoHarras"].Cast<CheckBox>().CurrentValue; } }
        public static int HarassMana { get { return HarassMenu["HarassMana"].Cast<Slider>().CurrentValue; } }

        //misc
        public static bool InterruptSpells { get { return MiscMenu["InterruptSpells"].Cast<CheckBox>().CurrentValue; } }
        public static bool AntiGap { get { return MiscMenu["AntiGap"].Cast<CheckBox>().CurrentValue; } }

    }
}
