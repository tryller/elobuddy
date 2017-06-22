using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Enumerations;
using EloBuddy.SDK.Events;
using EloBuddy.SDK.Utils;
using LightCassiopeia.Carry;
using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LightCassiopeia
{
    internal class Program
    {
        private static List<ModeBase> Modes { get; set; }
        private const int BarWidth = 106;
        private const int LineThickness = 9;
        private static readonly Vector2 BarOffset = new Vector2(0, 0);

        private static void Main(string[] args)
        {
            Loading.OnLoadingComplete += OnLoadingComplete;
        }

        private static void OnLoadingComplete(EventArgs args)
        {
            if (Player.Instance.Hero != Champion.Cassiopeia)
                return;

            Chat.Print("LightCassiopeia - Successfully loaded!", Color.ForestGreen);
            GameMenu.Initialize();

            Game.OnTick += OnTick;
            GameObject.OnCreate += GameObject_OnCreate;
            Gapcloser.OnGapcloser += OnGapCloser;
            Interrupter.OnInterruptableSpell += OnPossibleToInterrupt;
            Modes = new List<ModeBase>();
            Modes.AddRange(new ModeBase[]
            {
                new PermaActive(),
                new Combo(),
                new Harras(),
                new LaneClear(),
                new LastHit(),
                new JungleClear()
            });
        }

        public static void GameObject_OnCreate(GameObject sender, EventArgs args)
        {
            var rengar = EntityManager.Heroes.Enemies.FirstOrDefault(a => a.Hero == Champion.Rengar);
            if (sender.Name == "Rengar_LeapSound.troy" && ObjectManager.Player.Distance(Player.Instance.Position) <= SpellManager.R.Range && rengar != null)
            {
                SpellManager.R.Cast(rengar);
            }
        }

        public static void OnGapCloser(AIHeroClient sender, Gapcloser.GapcloserEventArgs e)
        {
            if (sender == null || sender.IsAlly)
                return;

            if ((sender.IsAttackingPlayer || e.End.Distance(Player.Instance.Position) <= 70))
            {
                SpellManager.R.Cast(sender);
            }
        }

        public static void OnPossibleToInterrupt(Obj_AI_Base sender, Interrupter.InterruptableSpellEventArgs interruptableSpellEventArgs)
        {
            if (sender == null || sender.IsAlly)
                return;

            if ((sender.IsAttackingPlayer || sender.Distance(Player.Instance.Position) <= 70))
            {
                SpellManager.R.Cast(sender);
            }
        }

        private static void OnTick(EventArgs args)
        {
            Modes.ForEach(mode =>
            {
                try
                {
                    if (mode.ShouldBeExecuted())
                    {
                        mode.Execute();
                    }
                }
                catch (Exception e)
                {
                    Logger.Log(LogLevel.Error, "Error executing mode '{0}'\n{1}", mode.GetType().Name, e);
                }
            });
        }
    }
}