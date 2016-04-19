using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EloBuddy;
using EloBuddy.SDK;
using AutoBuddy;

namespace AutoBuddy.MainLogics
{
    class SummonerSpells
    {
        public static Spell.Active Heal = null;
        public static Spell.Active Barrier = null;
        public static Spell.Active Ghost = null;

        public static AIHeroClient target = null;
        public void Init()
        {
            var heal = AutoWalker.myHero.Spellbook.Spells.Where(x => x.Name.Contains("heal"));
            SpellDataInst Heal1 = heal.Any() ? heal.First() : null;
            if (Heal1 != null)
            {
                Heal = new Spell.Active(Heal1.Slot);
            }

            var barrier = AutoWalker.myHero.Spellbook.Spells.Where(x => x.Name.Contains("barrier"));
            SpellDataInst Barrier1 = barrier.Any() ? barrier.First() : null;
            if (Barrier1 != null)
            {
                Barrier = new Spell.Active(Barrier1.Slot);
            }

            var ghost = AutoWalker.myHero.Spellbook.Spells.Where(x => x.Name.Contains("ghost"));
            SpellDataInst Ghost1 = ghost.Any() ? ghost.First() : null;
            if (Heal1 != null)
            {
                Ghost = new Spell.Active(Ghost1.Slot);
            }

            Game.OnUpdate += Game_OnUpdate;
        }

        private void Game_OnUpdate(EventArgs args)
        {
            if (AutoWalker.myHero.CountEnemiesInRange(900) > 0)
            {
                if (AutoWalker.myHero.HealthPercent <= 28)
                {
                    if (Heal != null && Heal.IsReady())
                    {
                        Heal.Cast();
                    }
                }
                else if (AutoWalker.myHero.HealthPercent <= 18)
                {
                    if (Barrier != null && Barrier.IsReady())
                    {
                        Barrier.Cast();
                    }
                }

                if (target == null)
                {
                    target = EntityManager.Heroes.Enemies.Where(en => en.Distance(AutoWalker.myHero) < 600 + en.BoundingRadius)
                        .OrderBy(en => en.Health)
                        .FirstOrDefault();
                }

                if (target != null && Ghost != null && Ghost.IsReady() && AutoWalker.myHero.HealthPercent() / target.HealthPercent() > 2 &&
                        target.Distance(AutoWalker.myHero) > AutoWalker.myHero.AttackRange + target.BoundingRadius + 150 &&
                        target.Distance(target.Position.GetNearestTurret()) > 1500)
                {
                    Ghost.Cast();
                }
            }
        }
    }
}
