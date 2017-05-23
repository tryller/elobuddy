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
            //happens when done loading
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
        private static Menu MiscMenu, KatarinaMenu, ComboMenu, LaneClearMenu, LastHitMenu, HarassAutoharass, KillStealMenu, HumanizerMenu;


        public static bool harassNeedToEBack = false;
        private static AIHeroClient target;

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
            else
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
            else
                return 0f;
        }

        public static float RDamage(Obj_AI_Base target)
        {
            if (!R.IsOnCooldown)
                return Player.Instance.CalculateDamageOnUnit(target, DamageType.Magical, (new[] { 0f, 375f, 562.5f, 750f }[R.Level] + 2.85f * Player.Instance.TotalMagicalDamage + 3.3f * (Player.Instance.TotalAttackDamage - Player.Instance.BaseAttackDamage)));
            else
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

            HumanizerMenu = KatarinaMenu.AddSubMenu("Humanizer");
            HumanizerMenu.Add("Q", new Slider("Q delay", 100, 0, 1000));
            HumanizerMenu.Add("W", new Slider("W delay", 110, 0, 1000));
            HumanizerMenu.Add("E", new Slider("E delay", 120, 0, 1000));
            HumanizerMenu.Add("R", new Slider("R delay", 130, 0, 1000));

            MiscMenu = KatarinaMenu.AddSubMenu("Misc");
            MiscMenu.Add("minZhonyaHealth", new Slider("Minimum health % to use Zhonyas", 18, 0, 100));

            //Giving spells values
            Q = new Spell.Targeted(SpellSlot.Q, 600, DamageType.Magical);
            W = new Spell.Active(SpellSlot.W, 150, DamageType.Magical);
            E = new Spell.Skillshot(SpellSlot.E, 700, EloBuddy.SDK.Enumerations.SkillShotType.Circular, 7, null, 150, DamageType.Magical);
            R = new Spell.Active(SpellSlot.R, 550, DamageType.Magical);
            Ignite = new Spell.Targeted(ObjectManager.Player.GetSpellSlotFromName("summonerdot"), 600);

            Drawing.OnEndScene += Damage_Indicator;
            Game.OnTick += Game_OnTick;
        }

        public static void AutoZhonya()
        {
            if (!Zhonya.IsReady() || myHero.IsDead)
                return;

            if (myHero.HealthPercent <= MiscMenu["minZhonyaHealth"].Cast<Slider>().CurrentValue
                && !myHero.IsRecalling() && myHero.CountEnemyChampionsInRange(600) > 0)
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
                Program.Ignite.Cast(target);
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

            AutoZhonya();
            AutoIgnite();

            if (HarassAutoharass["AHQ"].Cast<CheckBox>().CurrentValue)
            {
                CastQ(target);
            }

            if (QDamage(target) >= target.Health && KillStealMenu["Q"].Cast<CheckBox>().CurrentValue)
                CastQ(target);

            if (WDamage(target) >= target.Health && KillStealMenu["W"].Cast<CheckBox>().CurrentValue)
                CastW();

            if (EDamage(target) >= target.Health && KillStealMenu["E"].Cast<CheckBox>().CurrentValue)
                CastE(target.Position);

            if (EDamage(target) + WDamage(target) >= target.Health && KillStealMenu["EW"].Cast<CheckBox>().CurrentValue)
            {
                CastE(target.Position);
                CastW();
            }

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear))
            {
                LaneClear();
            }
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LastHit))
            {

                LastHit();

            }
            if (Orbwalker.ActiveModesFlags.Equals(Orbwalker.ActiveModes.Combo))
            {
                Combo();
            }
            if (Orbwalker.ActiveModesFlags.Equals(Orbwalker.ActiveModes.Harass))
            {
                Harass();
            }
            if (!Orbwalker.ActiveModesFlags.Equals(Orbwalker.ActiveModes.Combo))
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
                daggers.Add(new Dagger() { StartTime = Game.Time + 1.25f, EndTime = Game.Time + 5.1f, Position = ObjectManager.Get<Obj_AI_Minion>().LastOrDefault(a => a.Name == "HiddenMinion" && a.IsValid).Position });
                previouspos = ObjectManager.Get<Obj_AI_Minion>().LastOrDefault(a => a.Name == "HiddenMinion" && a.IsValid).Position;
            }
        }

        private static void Harass()
        {

            if (HarassAutoharass["HQ"].Cast<CheckBox>().CurrentValue && Q.IsReady())
            {
                var target = TargetSelector.GetTarget(Q.Range, DamageType.Magical);
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
                    Core.DelayAction(() => CastQ(target), HumanizerMenu["Q"].Cast<Slider>().CurrentValue + Game.Ping);
                    Core.DelayAction(() => CastW(), HumanizerMenu["W"].Cast<Slider>().CurrentValue + Game.Ping);
                    Core.DelayAction(() => CastE(target.Position), HumanizerMenu["E"].Cast<Slider>().CurrentValue + Game.Ping);
                    if (E.IsOnCooldown)
                        harassNeedToEBack = true;
                }

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

            else if (R.IsReady() && comboNum == 0 && myHero.Distance(target)<=400)
                comboNum = 7;

            //combo 1, Q W and E
            if (comboNum == 1)
            {
                CastQ(target);
                Core.DelayAction(() => CastE(myHero.Position.Extend(target.Position, myHero.Distance(target) + Game.Ping).To3D()), 0);
                Core.DelayAction(() => CastW(), 50 + Game.Ping);

                if (Q.IsOnCooldown && W.IsOnCooldown && E.IsOnCooldown)
                    comboNum = 0;
            }

            //combo 2, Q and E
            if (comboNum == 2)
            {
                CastQ(target);
                Core.DelayAction(() => CastE(myHero.Position.Extend(target.Position, myHero.Distance(target) + Game.Ping).To3D()), 0);

                if (Q.IsOnCooldown && E.IsOnCooldown)
                    comboNum = 0;
            }

            //combo 3, W and E
            if (comboNum == 3)
            {
                Core.DelayAction(() => CastE(myHero.Position.Extend(target.Position, myHero.Distance(target) + Game.Ping).To3D()), 0);
                Core.DelayAction(() => CastW(), 50 + Game.Ping);

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
                R.Cast();
                comboNum = 0;
            }

        }

        private static void Damage_Indicator(EventArgs args)
        {

            foreach (var unit in EntityManager.Heroes.Enemies.Where(x => x.VisibleOnScreen && x.IsValidTarget() && x.IsHPBarRendered))
            {
                var damage = 0f;
                if (Q.IsReady() && W.IsReady() && E.IsReady())
                    damage = QDamage(unit) + WDamage(unit) + EDamage(unit) + (2f * SpinDamage(unit));
                else if (Q.IsReady() && W.IsReady())
                    damage = QDamage(unit) + WDamage(unit) + SpinDamage(unit);
                else if (Q.IsReady() && E.IsReady())
                    damage = QDamage(unit) + EDamage(unit) + SpinDamage(unit);
                else if (W.IsReady() && E.IsReady())
                    damage = EDamage(unit) + WDamage(unit) + SpinDamage(unit);
                else if (Q.IsReady())
                    damage = QDamage(unit);
                else if (W.IsReady())
                    damage = WDamage(unit);
                else if (E.IsReady())
                    damage = EDamage(unit);
                if (!R.IsOnCooldown)
                    damage += RDamage(unit);
                //Chat.Print(damage);
                var Special_X = unit.ChampionName == "Jhin" || unit.ChampionName == "Annie" ? -12 : 0;
                var Special_Y = unit.ChampionName == "Jhin" || unit.ChampionName == "Annie" ? -3 : 9;

                var DamagePercent = ((unit.TotalShieldHealth() - damage) > 0
                    ? (unit.TotalShieldHealth() - damage)
                    : 0) / (unit.MaxHealth + unit.AllShield + unit.AttackShield + unit.MagicShield);
                var currentHealthPercent = unit.TotalShieldHealth() / (unit.MaxHealth + unit.AllShield + unit.AttackShield + unit.MagicShield);
                var StartPoint = new Vector2((int)(unit.HPBarPosition.X + DamagePercent * 107), (int)unit.HPBarPosition.Y - 5 + 14);
                var EndPoint = new Vector2((int)(unit.HPBarPosition.X + currentHealthPercent * 107) + 1, (int)unit.HPBarPosition.Y - 5 + 14);

                Drawing.DrawLine(StartPoint, EndPoint, 9.82f, System.Drawing.Color.SandyBrown);

            }
        }
    }
}
