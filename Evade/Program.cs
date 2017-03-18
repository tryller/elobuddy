﻿using System;
using EloBuddy.SDK.Events;

namespace EvadePlus
{
    internal static class Program
    {
        public static bool DeveloperMode = false;

        private static SkillshotDetector _skillshotDetector;
        private static EvadePlus _evade;

        private static void Main(string[] args)
        {
            Loading.OnLoadingComplete += OnLoadingComplete;
        }

        private static void OnLoadingComplete(EventArgs args)
        {
            _skillshotDetector = new SkillshotDetector(DeveloperMode ? DetectionTeam.AnyTeam : DetectionTeam.EnemyTeam);
            _evade = new EvadePlus(_skillshotDetector);
            EvadeMenu.CreateMenu();
        }
    }
}
