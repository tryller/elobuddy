using System;
using System.Collections.Generic;
using System.Linq;
using EloBuddy;

using EloBuddy.SDK;
using EloBuddy.SDK.Events;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using SharpDX;


namespace P1_Katarina
{
    class Program
    {
        private static readonly Item Zhonya = new Item(3157);

        static void Main(string[] args)
        {
            Loading.OnLoadingComplete += Loading_OnLoadingComplete;
        }

        //makes Player.Instance Player
        private static AIHeroClient myHero = Player.Instance;

        //Katarina Spells
        private static Spell.Targeted Q;
        private static Spell.Active W;
        private static Spell.Skillshot E;
        private static Spell.Active R;
        public static Spell.Targeted Ignite;

        private static List<Dagger> daggers = new List<Dagger>();
        private static Vector3 previouspos;
        private static List<float> daggerstart = new List<float>();
        private static List<float> daggerend = new List<float>();
        public static List<Vector2> daggerpos = new List<Vector2>();
        public static Vector3 qdaggerpos;
        public static Vector3 wdaggerpos;
        public static int comboNum;


        public class Dagger
        {
            public float StartTime { get; set; }
            public float EndTime { get; set; }
            public Vector3 Position { get; set; }
            public int Width = 230;
        }

        //Declare the menu
        private static Menu MiscMenu, KatarinaMenu, ComboMenu, LaneClearMenu, LastHitMenu, HarassAutoharass, KillStealMenu;

        private static AIHeroClient target;
        public static bool harassNeedToEBack = false;

        private static bool HasRBuff()
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

        private static void Loading_OnLoadingComplete(EventArgs args)
        {
            //Makes sure you are Katarina fdsgfdgdsgsd
            if (myHero.ChampionName != "Katarina")
                return;

            KatarinaMenu = MainMenu.AddMenu("Katarina", "P1 Katarina");

            //Creates a SubMenu
            ComboMenu = KatarinaMenu.AddSubMenu("Combo");
            ComboMenu.AddLabel("I don't know what to have here, if you have any suggestions please tell me");
            ComboMenu.Add("EAA", new CheckBox("Only use e if target is outside auto attack range"));

            LaneClearMenu = KatarinaMenu.AddSubMenu("Lane Clear");
            LaneClearMenu.Add("Q", new CheckBox("Use Q in lane clear"));

            LastHitMenu = KatarinaMenu.AddSubMenu("LastHit");
            LastHitMenu.Add("Q", new CheckBox("Use Q in last hit"));

            HarassAutoharass = KatarinaMenu.AddSubMenu("Harass/AutoHarass");
            HarassAutoharass.Add("HQ", new CheckBox("Use Q in harass"));
            HarassAutoharass.Add("CC", new CheckBox("Use E reset combo in harass"));
            HarassAutoharass.Add("AHQ", new CheckBox("Use Q in auto harass"));

            KillStealMenu = KatarinaMenu.AddSubMenu("Killsteal");
            KillStealMenu.Add("Q", new CheckBox("Use Q to killsteal"));
            KillStealMenu.Add("R", new CheckBox("Use R to killsteal", false));

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

            if (HasRBuff())
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

            for (var index = daggers.Count - 1; index >= 0; index--)
            {
                if (myHero.Distance(daggers[index].Position) <= daggers[index].Width && Game.Time >= daggers[index].StartTime || daggers[index] == null || Game.Time >= daggers[index].EndTime)
                {
                    daggers.RemoveAt(index);

                }
            }

            var DaggerFirst = ObjectManager.Get<Obj_AI_Minion>().LastOrDefault(a => a.Name == "HiddenMinion" && a.IsValid).Position;
            if (ObjectManager.Get<Obj_AI_Minion>().LastOrDefault(a => a.Name == "HiddenMinion" && a.IsValid).Position != previouspos)
            {
                //print("Added dagger");
                daggers.Add(new Dagger() { StartTime = Game.Time + 1.25f, EndTime = Game.Time + 5.1f, Position = ObjectManager.Get<Obj_AI_Minion>().LastOrDefault(a => a.Name == "HiddenMinion" && a.IsValid).Position });
                previouspos = ObjectManager.Get<Obj_AI_Minion>().LastOrDefault(a => a.Name == "HiddenMinion" && a.IsValid).Position;
            }
        }

        private static void Harass()
        {

            if (HarassAutoharass["HQ"].Cast<CheckBox>().CurrentValue && Q.IsReady())
            {
                target = TargetSelector.GetTarget(Q.Range, DamageType.Magical);
                if (target.IsValidTarget())
                    CastQ(target);
            }
            if (HarassAutoharass["CC"].Cast<CheckBox>().CurrentValue)
            {
                if (harassNeedToEBack && E.IsReady())
                {
                    myHero.Spellbook.CastSpell(E.Slot, wdaggerpos, false, false);
                    harassNeedToEBack = false;
                }


                target = TargetSelector.GetTarget(Q.Range, DamageType.Magical);
                if (target.IsValidTarget() && !harassNeedToEBack)
                {
                    CastQ(target);
                    CastW();
                    CastE(target.Position);
                    if (E.IsOnCooldown)
                        harassNeedToEBack = true;
                }

            }
        }

        private static void Game_OnTick1(EventArgs args)
        {
            AutoIgnite();
            AutoZhonya();

            if (HarassAutoharass["AHQ"].Cast<CheckBox>().CurrentValue)
            {
                target = TargetSelector.GetTarget(Q.Range, DamageType.Magical);
                CastQ(target);
            }
            target = TargetSelector.GetTarget(Q.Range, DamageType.Magical);
            if (QDamage(target) >= target.Health && KillStealMenu["Q"].Cast<CheckBox>().CurrentValue)
                CastQ(target);
            target = TargetSelector.GetTarget(W.Range, DamageType.Magical);
            if (WDamage(target) >= target.Health && KillStealMenu["W"].Cast<CheckBox>().CurrentValue)
                CastW();
            target = TargetSelector.GetTarget(E.Range, DamageType.Magical);
            if (EDamage(target) >= target.Health && KillStealMenu["E"].Cast<CheckBox>().CurrentValue)
                CastE(target.Position);
            target = TargetSelector.GetTarget(E.Range, DamageType.Magical);
            if (EDamage(target) + WDamage(target) >= target.Health && KillStealMenu["EW"].Cast<CheckBox>().CurrentValue)
            {
                CastE(target.Position);
                CastW();
            }
        }

        public static void AutoZhonya()
        {
            if (!Zhonya.IsReady() || myHero.IsDead)
                return;

            if (myHero.HealthPercent <= MiscMenu["saveHealth"].Cast<Slider>().CurrentValue &&
                !myHero.IsRecalling() && myHero.CountEnemyChampionsInRange(1000) > 0)
            {
                Zhonya.Cast();
            }
        }

        public static void AutoIgnite()
        {
            if (myHero.IsDead)
                return;

            var target = TargetSelector.GetTarget(650, DamageType.Magical);
            if (target.IsValidTarget(Ignite.Range) && target.Health < myHero.GetSummonerSpellDamage(target, DamageLibrary.SummonerSpells.Ignite))
                Ignite.Cast(target);
        }

        public static void CastQ(Obj_AI_Base target)
        {
            Q.Cast(target);
            qdaggerpos = ObjectManager.Get<Obj_AI_Minion>().LastOrDefault(a => a.Name == "HiddenMinion" && a.IsValid).Position;
        }

        private static void CastW()
        {
            W.Cast();
            wdaggerpos = myHero.Position;
        }

        private static void CastE(Vector3 target)
        {
            if (daggers.Count == 0 && !HasRBuff())
                E.Cast(target);
            foreach (Dagger dagger in daggers)
            {
                if (target.Distance(dagger.Position) <= 550)
                    myHero.Spellbook.CastSpell(E.Slot, dagger.Position.Extend(target, 150).To3D(), false, false);

                else if (ComboMenu["EAA"].Cast<CheckBox>().CurrentValue && target.Distance(myHero) >= myHero.GetAutoAttackRange())
                    E.Cast(target);
                else if (!ComboMenu["EAA"].Cast<CheckBox>().CurrentValue)
                    E.Cast(target);
                else
                    return;
            }
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
            if (target != null && target.IsValidTarget(E.Range)
    && !target.HasBuff("sionpassivezombie")               //sion Passive
    && !target.HasBuff("KarthusDeathDefiedBuff")          //karthus passive
    && !target.HasBuff("kogmawicathiansurprise")          //kog'maw passive
    && !target.HasBuff("zyrapqueenofthorns")              //zyra passive
    && !target.HasBuff("ChronoShift"))                     //zilean R
            {
                AutoIgnite();

                if (E.IsReady() && Q.IsReady() && W.IsReady() && comboNum == 0)
                {
                    if (!HasRBuff() || (HasRBuff() && target.Health < QDamage(target) + WDamage(target) + EDamage(target) + (2f * SpinDamage(target))))
                        comboNum = 1;
                }

                else if (E.IsReady() && Q.IsReady() && comboNum == 0)
                {
                    if (!HasRBuff() || (HasRBuff() && target.Health < QDamage(target) + EDamage(target) + SpinDamage(target)))
                        comboNum = 2;
                }

                else if (W.IsReady() && E.IsReady() && comboNum == 0)
                {
                    if (!HasRBuff() || (HasRBuff() && target.Health < EDamage(target) + SpinDamage(target)))
                        comboNum = 3;
                }

                else if (E.IsReady() && comboNum == 0)
                {
                    if (!HasRBuff() || (HasRBuff() && target.Health < EDamage(target)))
                        comboNum = 4;
                }

                else if (Q.IsReady() && comboNum == 0)
                {
                    if (!HasRBuff() || (HasRBuff() && target.Health < QDamage(target)))
                        comboNum = 5;
                }

                else if (W.IsReady() && comboNum == 0 && myHero.Distance(target) <= 300)
                {
                    if (!HasRBuff())
                        comboNum = 6;
                }

                else if (R.IsReady() && comboNum == 0 && myHero.Distance(target) <= 400)
                    comboNum = 7;

                //combo 1, Q W and E
                if (comboNum == 1)
                {
                    CastQ(target);
                    CastE(myHero.Position.Extend(target.Position, myHero.Distance(target) + 140).To3D());
                    CastW();

                    if (Q.IsOnCooldown && W.IsOnCooldown && E.IsOnCooldown)
                        comboNum = 0;
                }

                //combo 2, Q and E
                if (comboNum == 2)
                {
                    CastQ(target);
                    CastE(myHero.Position.Extend(target.Position, myHero.Distance(target) + 140).To3D());

                    if (Q.IsOnCooldown && E.IsOnCooldown)
                        comboNum = 0;
                }

                //combo 3, W and E
                if (comboNum == 3)
                {
                    CastE(myHero.Position.Extend(target.Position, myHero.Distance(target) + 140).To3D());
                    CastW();


                    if (W.IsOnCooldown && E.IsOnCooldown)
                        comboNum = 0;
                }

                //combo 4, E
                if (comboNum == 4)
                {
                    CastE(target.Position);
                    comboNum = 0;
                }

                //combo 5, Q
                if (comboNum == 5)
                {
                    CastQ(target);
                    comboNum = 0;
                }

                //combo 6, W
                if (comboNum == 6)
                {
                    CastW();
                    comboNum = 0;
                }

                //combo 7, R
                if (comboNum == 7)
                {
                    if (R.IsReady() && myHero.Distance(target.Position) <= R.Range && target != null &&
                        target.IsVisible && !target.IsDead && !Q.IsReady() && !W.IsReady() && !E.IsReady())
                    {
                        Orbwalker.DisableMovement = true;
                        Orbwalker.DisableAttacking = true;
                        R.Cast();
                        comboNum = 0;
                    }
                }
            }
        }
    }
}
