using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;

namespace LightCassiopeia.MenuList
{
    public static class Misc
    {
        private static readonly Menu Menu;
        private static readonly CheckBox _onlyPoisoned;
        private static readonly CheckBox _lasthitE;
        private static readonly Slider _skillsPredction;

        public static int skillsPredition
        {
            get { return _skillsPredction.CurrentValue; }
        }

        public static bool LasthitE
        {
            get { return _lasthitE.CurrentValue; }
        }

        public static bool Poisoned
        {
            get { return _onlyPoisoned.CurrentValue; }
        }

        static Misc()
        {
            Menu = GameMenu.Menu.AddSubMenu("Misc");
            _skillsPredction = Menu.Add("skillsPredition", new Slider("Skills Prediction (Q / W)", 90, 1, 100));
            Menu.AddSeparator();
            _onlyPoisoned = Menu.Add("onlypoisoned", new CheckBox("Use E only if target is poisoned!", true));
            Menu.AddSeparator();
            Menu.AddLabel("Lasthiting");
            _lasthitE = Menu.Add("lasthite", new CheckBox("Last hit using E", true));
        }

        public static void Initialize()
        {
        }
    }
}