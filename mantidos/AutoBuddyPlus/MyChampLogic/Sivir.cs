using System.Linq;
using AutoBuddy.MainLogics;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Enumerations;

namespace AutoBuddy.MyChampLogic
{
    internal class Sivir : IChampLogic
    {
        public float MaxDistanceForAA { get { return int.MaxValue; } }
        public float OptimalMaxComboDistance { get { return AutoWalker.myHero.AttackRange; } }
        public float HarassDistance { get { return AutoWalker.myHero.AttackRange; } }


        public static Spell.Skillshot Q { get; private set; }
        public static Spell.Skillshot QLine { get; private set; }
        public static Spell.Active W { get; private set; }
        public static Spell.Active E { get; private set; }
        public static Spell.Active R { get; private set; }

        public Sivir()
        {
            skillSequence = new[] { 1, 2, 3, 1, 1, 4, 1, 2, 1, 2, 4, 2, 2, 3, 3, 4, 3, 3 };
            ShopSequence =
                "3340:Buy,1036:Buy,2003:StartHpPot,1053:Buy,1042:Buy,1001:Buy,3006:Buy,1036:Buy,1038:Buy,3072:Buy,2003:StopHpPot,1042:Buy,1051:Buy,3086:Buy,1042:Buy,1042:Buy,1043:Buy,3085:Buy,2015:Buy,3086:Buy,3094:Buy,1018:Buy,1038:Buy,3031:Buy,1037:Buy,3035:Buy,3033:Buy";

            Q = new Spell.Skillshot(SpellSlot.Q, 1250, SkillShotType.Linear, 250, 1350, 90)
            {
                AllowedCollisionCount = int.MaxValue
            };
            QLine = new Spell.Skillshot(SpellSlot.Q, 1250, SkillShotType.Linear, 250, 1350, 90);
            W = new Spell.Active(SpellSlot.W, 750);
            E = new Spell.Active(SpellSlot.E);
            R = new Spell.Active(SpellSlot.R, 1000);

            Game.OnTick += Game_OnTick;
        }

        public int[] skillSequence { get; private set; }
        public LogicSelector Logic { get; set; }

        public string ShopSequence { get; private set; }

        public void Harass(AIHeroClient target)
        {

        }

        public void Survi()
        {
            AIHeroClient target =
                EntityManager.Heroes.Enemies.FirstOrDefault(
                    chase => chase.Distance(AutoWalker.myHero) < 600 && chase.IsVisible());
            if (Q.IsReady())
            {
                if (target != null && target.IsValidTarget())
                {
                    Q.Cast(target.ServerPosition);
                }
            }
        }

        public void Combo(AIHeroClient target)
        {
            if (target.HealthPercent <= 45)
            {
                if (Q.IsReady())
                {
                    if (target != null && target.IsValidTarget())
                    {
                        Q.Cast(target.ServerPosition);
                    }
                }

                if (W.IsReady())
                {
                    if (target != null && target.IsValidTarget())
                    {
                        W.Cast();
                        Orbwalker.ResetAutoAttack();
                    }
                }

                if (R.IsReady())
                {
                    if (ObjectManager.Player.Position.CountAlliesInRange(1000) >= 2
                      && ObjectManager.Player.Position.CountEnemiesInRange(2000) >= 2)
                    {
                        R.Cast();
                    }
                }
            }
        }

        private void Game_OnTick(System.EventArgs args)
        {

        }
    }
}