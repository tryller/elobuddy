using System;
using System.Collections.Generic;
using EloBuddy;
using EloBuddy.SDK.Events;
using Jhin.Champions;
using Jhin.Managers;
using Jhin.Utilities;

namespace Jhin
{
    // ReSharper disable once InconsistentNaming
    public static class Program
    {
        public static ChampionBase CurrentChampion;

        public static HashSet<Champion> SupportedChampions = new HashSet<Champion>
        {
            Champion.Jhin,
        };

        public static bool Initialized;

        public static List<Action> Initializers = new List<Action>
        {
            SpellManager.Initialize,
            UnitManager.Initialize,
            DrawingsManager.Initialize,
            MissileManager.Initialize,
            YasuoWallManager.Initialize,
            DamageIndicator.Initialize,
            ItemManager.Initialize,
            FpsBooster.Initialize,
            CacheManager.Initialize,
            LanguageTranslator.Initialize
        };

        public static AIHeroClient MyHero
        {
            get { return Player.Instance; }
        }

        public static void WriteInConsole(string message)
        {
            Console.WriteLine("Jhin: " + message);
        }

        public static void WriteInChat(string message)
        {
            Chat.Print("Jhin: " + message);
        }

        private static void Main(string[] args)
        {
            Loading.OnLoadingComplete += OnLoad;
        }

        private static void OnLoad(EventArgs args)
        {
            if (MyHero.Hero != Champion.Jhin)
            {
                return;
            }

            new Champions.Jhin();
        }
    }
}