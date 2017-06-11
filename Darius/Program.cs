using System;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Events;
using EloBuddy.SDK.Rendering;
using SharpDX;
using SettingsMisc = AddonTemplate.Config.Modes.MiscMenu;
using SettingsItems = AddonTemplate.Config.Modes.Items;
namespace AddonTemplate
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            // Wait till the loading screen has passed
            Loading.OnLoadingComplete += OnLoadingComplete;
        }

        private static void OnLoadingComplete(EventArgs args)
        {
            // Verify the champion we made this addon for
            if (Player.Instance.ChampionName != "Darius")
            {
                // Champion is not the one we made this addon for,
                // therefore we return
                return;
            }

            // Initialize the classes that we need
            Config.Initialize();
            SpellManager.Initialize();
            ModeManager.Initialize();

            // Listen to events we need
            Obj_AI_Base.OnBuffGain += BuffGain;

        }

        public static void BuffGain(Obj_AI_Base sender, Obj_AI_BaseBuffGainEventArgs buff)
        {
            if (!sender.IsMe) return;

            if (buff.Buff.Type == BuffType.Taunt && SettingsItems.Taunt)
            {
                DoQSS();
            }
            if (buff.Buff.Type == BuffType.Stun && SettingsItems.Stun)
            {
                DoQSS();
            }
            if (buff.Buff.Type == BuffType.Snare && SettingsItems.Snare)
            {
                DoQSS();
            }
            if (buff.Buff.Type == BuffType.Polymorph && SettingsItems.Polymorph)
            {
                DoQSS();
            }
            if (buff.Buff.Type == BuffType.Blind && SettingsItems.Blind)
            {
                DoQSS();
            }
            if (buff.Buff.Type == BuffType.Flee && SettingsItems.Fear)
            {
                DoQSS();
            }
            if (buff.Buff.Type == BuffType.Charm && SettingsItems.Charm)
            {
                DoQSS();
            }
            if (buff.Buff.Type == BuffType.Suppression && SettingsItems.Supression)
            {
                DoQSS();
            }
            if (buff.Buff.Type == BuffType.Silence && SettingsItems.Silence)
            {
                DoQSS();
            }
            if (buff.Buff.Name == "zedulttargetmark")
            {
                UltQSS();
            }
            if (buff.Buff.Name == "VladimirHemoplague")
            {
                UltQSS();
            }
            if (buff.Buff.Name == "FizzMarinerDoom")
            {
                UltQSS();
            }
            if (buff.Buff.Name == "MordekaiserChildrenOfTheGrave")
            {
                UltQSS();
            }
            if (buff.Buff.Name == "PoppyDiplomaticImmunity")
            {
                UltQSS();
            }
        }

        public static void DoQSS()
        {
            var delay = SettingsItems.Delay;
            if (SpellManager.Qss.IsOwned() && SpellManager.Qss.IsReady())
            {
                Core.DelayAction(() => { SpellManager.Qss.Cast(); }, delay);
            }

            if (SpellManager.Mercurial.IsOwned() && SpellManager.Mercurial.IsReady())
            {
                Core.DelayAction(() => { SpellManager.Mercurial.Cast(); }, delay);
            }
        }
        public static void UltQSS()
        {
            if (SpellManager.Qss.IsOwned() && SpellManager.Qss.IsReady())
            {
                Core.DelayAction(() => { SpellManager.Qss.Cast(); }, 1000);
            }

            if (SpellManager.Mercurial.IsOwned() && SpellManager.Mercurial.IsReady())
            {
                Core.DelayAction(() => { SpellManager.Mercurial.Cast(); }, 1000);
            }
        }

        private static void InterrupterOnOnInterruptableSpell(Obj_AI_Base sender, Interrupter.InterruptableSpellEventArgs interruptableSpellEventArgs)
        {
            if (!sender.IsEnemy || !(sender is AIHeroClient) || Player.Instance.IsRecalling())
            {
                return;
            }
            if (SettingsMisc.InterruptE && SpellManager.E.IsReady() && SpellManager.E.IsInRange(sender))
            {
                SpellManager.E.Cast(sender);
            }
        }
    }
}
