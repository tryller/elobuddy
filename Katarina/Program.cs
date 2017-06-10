using System;
using System.Collections.Generic;
using System.Linq;
using EloBuddy;

using EloBuddy.SDK;
using EloBuddy.SDK.Events;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using SharpDX;


namespace Katarina
{
    class Program
    {
        private static Item Zhonyas_Hourglass = new Item((int)ItemId.Zhonyas_Hourglass);//3157
        private static Item Bilgewater_Cutlass = new Item((int)ItemId.Bilgewater_Cutlass);//3144
        private static Item Hextech_Gunblade = new Item((int)ItemId.Hextech_Gunblade);//3146

        static void Main(string[] args)
        {
            Loading.OnLoadingComplete += Loading_OnLoadingComplete;
        }

        //makes Player.Instance Player
        private static AIHeroClient myHero = Player.Instance;

        //Katarina Spells
        public static Spell.Targeted Q;
        public static Spell.Active W;
        public static Spell.Skillshot E;
        public static Spell.Active R;
        public static Spell.Targeted Ignite;

        //Declare the menu
        private static Menu MiscMenu, KatarinaMenu, ComboMenu, LaneClearMenu, LastHitMenu, HarassAutoharass, KillStealMenu;

        private static AIHeroClient target;
        public static bool harassNeedToEBack = false;

        private static void Loading_OnLoadingComplete(EventArgs args)
        {
            //Makes sure you are Katarina fdsgfdgdsgsd
            if (myHero.ChampionName != "Katarina")
                return;

            KatarinaMenu = MainMenu.AddMenu("Katarina", "P1 Katarina");

            //Creates a SubMenu
            ComboMenu = KatarinaMenu.AddSubMenu("Combo");
            ComboMenu.Add("R.Killable", new CheckBox("Use R if target is killable", false));

            LaneClearMenu = KatarinaMenu.AddSubMenu("Lane Clear");
            LaneClearMenu.Add("Q", new CheckBox("Use Q in lane clear"));

            LastHitMenu = KatarinaMenu.AddSubMenu("LastHit");
            LastHitMenu.Add("Q", new CheckBox("Use Q in last hit"));

            HarassAutoharass = KatarinaMenu.AddSubMenu("Harass/AutoHarass");
            HarassAutoharass.Add("HQ", new CheckBox("Use Q in harass"));
            HarassAutoharass.Add("AHQ", new CheckBox("Use Q in auto harass"));

            KillStealMenu = KatarinaMenu.AddSubMenu("Killsteal");
            KillStealMenu.Add("Q", new CheckBox("Use Q to killsteal"));
            KillStealMenu.Add("E", new CheckBox("Use E to killsteal"));

            MiscMenu = KatarinaMenu.AddSubMenu("Misc");
            MiscMenu.Add("saveHealth", new Slider("Minimum health % to use Zhonyas", 18, 0, 100));
            MiscMenu.AddLabel("-------------");
            MiscMenu.Add("useHumanizer", new CheckBox("Use Humanize"));
            MiscMenu.Add("delayTime", new Slider("Delay time for distance", 150, 0, 1500));
            MiscMenu.Add("delayTimeCasts", new Slider("Delay time between casts", 120, 0, 1500));

            //Giving spells values
            Q = new Spell.Targeted(SpellSlot.Q, 600, DamageType.Magical);
            W = new Spell.Active(SpellSlot.W, 150, DamageType.Magical);
            E = new Spell.Skillshot(SpellSlot.E, 700, EloBuddy.SDK.Enumerations.SkillShotType.Circular, 7, null, 150, DamageType.Magical);
            R = new Spell.Active(SpellSlot.R, 550, DamageType.Magical);
            Ignite = new Spell.Targeted(ObjectManager.Player.GetSpellSlotFromName("summonerdot"), 600);

            Game.OnTick += Game_OnTick;
            Game.OnTick += Game_OnTick1;
            Spellbook.OnCastSpell += Spellbook_OnCastSpell;
        }

        private static bool GetRBufff()
        {
            return Player.Instance.Spellbook.IsChanneling;
        }

        public static float SpinDamage(Obj_AI_Base target)
        {
            return Player.Instance.CalculateDamageOnUnit(target, DamageType.Magical, ((myHero.Level / 1.75f) + 3f) * myHero.Level + 71.5f + 1.25f * (Player.Instance.TotalAttackDamage - Player.Instance.BaseAttackDamage) + myHero.TotalMagicalDamage * new[] { .55f, .70f, .80f, 1.00f }[R.Level]);
        }

        public static float QDamage(Obj_AI_Base target)
        {
            if (Q.IsReady())
                return Player.Instance.CalculateDamageOnUnit(target, DamageType.Magical, new[] { 0f, 75f, 105f, 135f, 165f, 195f }[Q.Level] + 0.3f * Player.Instance.TotalMagicalDamage);

            return 0f;
        }

        public static float WDamage(Obj_AI_Base target)
        {
            return 0f;
        }

        public static float EDamage(Obj_AI_Base target)
        {
            if (E.IsReady())
                return Player.Instance.CalculateDamageOnUnit(target, DamageType.Magical, new[] { 0f, 25f, 40f, 55f, 70f, 85f }[E.Level] + 0.25f * Player.Instance.TotalMagicalDamage + 0.5f * myHero.TotalAttackDamage);

            return 0f;
        }

        public static float RDamage(Obj_AI_Base target)
        {
            if (!R.IsOnCooldown)
                return Player.Instance.CalculateDamageOnUnit(target, DamageType.Magical, (new[] { 0f, 375f, 562.5f, 750f }[R.Level] + 2.85f * Player.Instance.TotalMagicalDamage + 3.3f * (Player.Instance.TotalAttackDamage - Player.Instance.BaseAttackDamage)));

            return 0f;
        }

        private static DateTime assemblyLoadTime = DateTime.Now;
        public class LatestCast
        {
            public static float Tick = 0;
            public static float Timepass;
            public static float X = 0;
            public static float Y = 0;
            public static double Distance;
            public static double Delay;
            public static int count = 0;
            public static double SavedTime = 0;

        }

        public static float CurrentTick
        {
            get
            {
                return (int)DateTime.Now.Subtract(assemblyLoadTime).TotalMilliseconds;
            }
        }

        public static void Spellbook_OnCastSpell(Spellbook sender, SpellbookCastSpellEventArgs eventArgs)
        {
            if (!MiscMenu["useHumanizer"].Cast<CheckBox>().CurrentValue)
                return;

            if (!sender.Owner.IsMe)
                return;

            var delayTime = MiscMenu["delayTime"].Cast<Slider>().CurrentValue;
            var delayTimeCasts = MiscMenu["delayTimeCasts"].Cast<Slider>().CurrentValue;

            Vector2 tempvect = new Vector2(LatestCast.X, LatestCast.Y);
            LatestCast.Timepass = CurrentTick - LatestCast.Tick;

            LatestCast.Distance = tempvect.Distance(eventArgs.StartPosition);
            LatestCast.Delay = (LatestCast.Distance * 0.001 * delayTime); ;
            if (CurrentTick < LatestCast.Tick + LatestCast.Delay && LatestCast.Timepass < 300)
            {
                eventArgs.Process = false;
                LatestCast.count += 1;
                LatestCast.SavedTime += LatestCast.Delay;

            }

            if (eventArgs.Process == true && LatestCast.Timepass > delayTimeCasts)
            {
                LatestCast.X = eventArgs.StartPosition.X;
                LatestCast.Y = eventArgs.StartPosition.Y;
                LatestCast.Tick = CurrentTick;
            }
        }

        private static void Game_OnTick(EventArgs args)
        {
            target = TargetSelector.GetTarget(E.Range, DamageType.Magical);

            if (GetRBufff())
            {
                Orbwalker.DisableMovement = true;
                Orbwalker.DisableAttacking = true;
            }
            else
            {
                Orbwalker.DisableMovement = false;
                Orbwalker.DisableAttacking = false;
            }

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear))
            {
                LaneClear();
            }
            else if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LastHit))
            {

                LastHit();

            }
            else if (Orbwalker.ActiveModesFlags.Equals(Orbwalker.ActiveModes.Combo))
            {
                Combo();
            }
            else if (Orbwalker.ActiveModesFlags.Equals(Orbwalker.ActiveModes.Harass))
            {
                Harass();
            }
            else if (!Orbwalker.ActiveModesFlags.Equals(Orbwalker.ActiveModes.Combo))
            {
                Orbwalker.DisableAttacking = false;
            }
        }

        private static void Harass()
        {
            target = TargetSelector.GetTarget(Q.Range, DamageType.Magical);
            if (target == null || target.IsInvulnerable || GetRBufff())
                return;

            if (HarassAutoharass["HQ"].Cast<CheckBox>().CurrentValue && Q.IsReady())
            {
                if (target.IsValidTarget())
                    CastQ(target);
            }
        }

        private static void Game_OnTick1(EventArgs args)
        {
            AutoIgnite();
            AutoZhonya();
            if (target == null || target.IsInvulnerable || GetRBufff())
                return;

            if (HarassAutoharass["AHQ"].Cast<CheckBox>().CurrentValue && !target.Position.IsUnderTurret())
            {
                if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LastHit) || Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear))
                    return;

                CastQ(target);
            }

            if (QDamage(target) >= target.Health && KillStealMenu["Q"].Cast<CheckBox>().CurrentValue)
                CastQ(target);


            if (EDamage(target) >= target.Health && KillStealMenu["E"].Cast<CheckBox>().CurrentValue)
                E.Cast(target.Position);
        }

        public static void AutoZhonya()
        {
            if (!Zhonyas_Hourglass.IsReady() || myHero.IsDead)
                return;

            if (myHero.HealthPercent <= MiscMenu["saveHealth"].Cast<Slider>().CurrentValue &&
                !myHero.IsRecalling() && myHero.CountEnemyChampionsInRange(1000) > 0)
            {
                Zhonyas_Hourglass.Cast();
            }
        }

        public static void AutoIgnite()
        {
            var target = TargetSelector.GetTarget(650, DamageType.Magical);
            if (target == null || target.IsInvulnerable)
                return;

            if (target.IsValidTarget(Ignite.Range) && target.Health < myHero.GetSummonerSpellDamage(target, DamageLibrary.SummonerSpells.Ignite))
                Ignite.Cast(target);
        }

        public static void CastQ(Obj_AI_Base target)
        {
            Q.Cast(target);
        }

        private static void CastW()
        {
            W.Cast();
        }


        private static void LaneClear()
        {
            var minions = EntityManager.MinionsAndMonsters.EnemyMinions.Where(a => a.Distance(Player.Instance) < Q.Range).OrderBy(a => a.Health);
            var minion = minions.FirstOrDefault();
            if (minion == null) return;

            if (LaneClearMenu["Q"].Cast<CheckBox>().CurrentValue && (QDamage(minion) > minion.Health) && Q.IsReady())
            {
                CastQ(minion);
            }
        }

        private static void LastHit()
        {

            var minions = EntityManager.MinionsAndMonsters.EnemyMinions.Where(x => x.Distance(Player.Instance) < Q.Range).OrderBy(a => a.Health);
            var minion = minions.FirstOrDefault();
            //print(minion);
            if (!minion.IsValidTarget())
                return;
            if (LastHitMenu["Q"].Cast<CheckBox>().CurrentValue && (QDamage(minion) >= minion.Health) && Q.IsReady())
            {
                CastQ(minion);
            }
        }

        private static void Combo()
        {
            target = TargetSelector.GetTarget(E.Range, DamageType.Magical);
            if (target == null || target.IsInvulnerable || GetRBufff())
                return;

            AutoIgnite();

            //Q
            if (Q.IsReady())
                Q.Cast(target);

            //E
            if (E.IsReady() && target.IsInRange(Player.Instance.Position, E.Range))
            {
                var d = Daggers.GetClosestDagger();
                if (target.Position.IsInRange(d, W.Range)) E.Cast(Damage.GetBestDaggerPoint(d, target));
                else
                    if (Player.Instance.Distance(target) >= W.Range)
                    E.Cast(target.Position);
            }

            if (Bilgewater_Cutlass.IsOwned() && Bilgewater_Cutlass.IsReady())
                Bilgewater_Cutlass.Cast(target);

            if (Hextech_Gunblade.IsOwned() && Hextech_Gunblade.IsReady())
                Hextech_Gunblade.Cast(target);

            //W
            if (W.IsReady() && Q.IsOnCooldown && E.IsOnCooldown && myHero.CountEnemyChampionsInRange(250) > 0)
                CastW();

            //R
            if (ComboMenu["R.Killable"].Cast<CheckBox>().CurrentValue)
            {
                if (R.IsReady() && target.IsInRange(myHero.Position, R.Range) &&
                    RDamage(target) > target.TotalShieldHealth() + target.HPRegenRate * 2 &&
                    !Q.IsReady() && !W.IsReady() && !E.IsReady())
                {
                    Orbwalker.DisableMovement = true;
                    Orbwalker.DisableAttacking = true;
                    R.Cast();
                }
            }
            else
            {
                if (R.IsReady() && target.IsInRange(myHero.Position, R.Range) && !Q.IsReady() && !W.IsReady() && !E.IsReady())
                {
                    Orbwalker.DisableMovement = true;
                    Orbwalker.DisableAttacking = true;
                    R.Cast();
                }
            }
        }
    }
}
