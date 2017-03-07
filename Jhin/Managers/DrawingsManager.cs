using EloBuddy;
using EloBuddy.SDK.Menu;
using Jhin;
using Jhin.Utilities;

namespace Jhin.Managers
{
    public static class DrawingsManager
    {
        public static Menu Menu
        {
            get { return MenuManager.GetSubMenu("Drawings"); }
        }

        public static void Initialize()
        {
            Drawing.OnDraw += delegate
            {
                if (Program.MyHero.IsDead || Menu.CheckBox("Disable"))
                {
                    return;
                }
                Program.CurrentChampion.OnDraw();
                CircleManager.Draw();
                ToggleManager.Draw();
            };
            Drawing.OnEndScene += delegate
            {
                if (Program.MyHero.IsDead || Menu.CheckBox("Disable"))
                {
                    return;
                }
                Program.CurrentChampion.OnEndScene();
                if (Menu.CheckBox("DamageIndicator"))
                {
                    DamageIndicator.Draw();
                }
            };
        }
    }
}