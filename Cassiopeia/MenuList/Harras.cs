using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;

namespace LightCassiopeia.MenuList
{
    public static class Harras
    {
        private static readonly Menu Menu;
        private static readonly CheckBox _withQ;
        private static readonly CheckBox _withW;
        private static readonly CheckBox _withE;

        public static bool WithQ
        {
            get { return _withQ.CurrentValue; }
        }

        public static bool WithW
        {
            get { return _withW.CurrentValue; }
        }

        public static bool WithE
        {
            get { return _withE.CurrentValue; }
        }

        static Harras()
        {
            Menu = GameMenu.Menu.AddSubMenu("Harras");
            _withQ = Menu.Add("useQ", new CheckBox("Q"));
            _withW = Menu.Add("useW", new CheckBox("W"));
            _withE = Menu.Add("useE", new CheckBox("E"));
        }

        public static void Initialize()
        {
        }
    }
}