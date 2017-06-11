using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Events;

namespace Farofakids_Heimerdinger
{
    internal class Program
    {

        public static void Main()
        {
            Loading.OnLoadingComplete += Loading_OnLoadingComplete;
        }

        public static void Loading_OnLoadingComplete(EventArgs args)
        {
            if (Player.Instance.ChampionName != "Heimerdinger") return;
            SPELLS.Initialize();
            MENUS.Initialize();
            Game.OnTick += Game_OnTick;
            Interrupter.OnInterruptableSpell += MODES.Interrupter_OnInterruptableSpell;
            Gapcloser.OnGapcloser += MODES.Gapcloser_OnGapcloser;

        }

        public static void Game_OnTick(EventArgs args)
        {
            if (Player.Instance.IsDead) return;

            switch (Orbwalker.ActiveModesFlags)
            {
                case Orbwalker.ActiveModes.Combo:
                    MODES.Combo();
                    break;
                case Orbwalker.ActiveModes.Harass:
                    MODES.Harras();
                    break;
            }

            if (MENUS.AutoHarras && !SPELLS.R.IsReady())
            {
                MODES.AutoHarras();
            }

            if (MENUS.KS)
            {
                MODES.KS();
            }

        }
    }
}