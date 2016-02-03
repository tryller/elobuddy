using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Events;
using tryCombos.Champions;

namespace tryCombos
{
    public static class Program
    {
        public static AIHeroClient myHero
        {
            get { return Player.Instance; }
        }

        static void Main(string[] args)
        {
            Loading.OnLoadingComplete += Game_OnStart;
        }

        private static void Game_OnStart(EventArgs args)
        {
            var champion = myHero.ChampionName;

            switch (champion)
            {
                case "Akali":
                    Akali.Init();
                    break;
                case "Katarina":
                    Katarina.Init();
                    break;
                case "Ryze":
                    Ryze.Init();
                    break;
                case "Warwick":
                    Warwick.Init();
                    break;
                case "Yasuo":
                    Yasuo.Init();
                    break;
            }
        }
    }
}
