using System;
using System.Linq;
using System.Collections.Generic;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Events;
using EloBuddy.SDK.Enumerations;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using SharpDX;

namespace T7_Fizz
{
    class Program
    {
        public static Spell.Targeted Q { get; private set; }
        public static Spell.Active W { get; private set; }
        public static Spell.Skillshot E { get; private set; }
        public static Spell.Skillshot R { get; private set; }


        static void Main(string[] args)
        {
            Loading.OnLoadingComplete += OnLoadingComplete;
        }

        public static AIHeroClient myhero { get { return ObjectManager.Player; } }
        public static Menu menu, combo, misc, blocking, flee;
        private static Spell.Targeted ignt = new Spell.Targeted(myhero.GetSpellSlotFromName("summonerdot"), 550);

        public static Item potion { get; private set; }
        public static Item biscuit { get; private set; }

        private static void OnLoadingComplete(EventArgs args)
        {
            if (Player.Instance.Hero != Champion.Fizz)
                return;

            Q = new Spell.Targeted(SpellSlot.Q, 550);
            W = new Spell.Active(SpellSlot.W, (uint)Player.Instance.GetAutoAttackRange());
            E = new Spell.Skillshot(SpellSlot.E, 400, SkillShotType.Circular, 250, int.MaxValue, 330);
            R = new Spell.Skillshot(SpellSlot.R, 1300, SkillShotType.Circular, 250, 1200, 80);

            Obj_AI_Base.OnProcessSpellCast += OnProcessSpellCast;
            Game.OnTick += OnTick;
            potion = new Item((int)ItemId.Health_Potion);
            biscuit = new Item((int)ItemId.Total_Biscuit_of_Rejuvenation);
            Player.LevelSpell(SpellSlot.E);
            DatMenu();
            SpellBlock.Initialize();
        }

        private static void OnTick(EventArgs args)
        {
            if (myhero.IsDead) return;

            var flags = Orbwalker.ActiveModesFlags;

            if (flags.HasFlag(Orbwalker.ActiveModes.Combo))
            {
                Combo();
            }

            if (flags.HasFlag(Orbwalker.ActiveModes.Flee))
            {
                if (check(flee, "EFLEE") && E.IsReady())
                {
                    E.Cast(myhero.Position.Extend(Game.CursorPos, E.Range).To3D());
                    E.Cast(myhero.Position.Extend(Game.CursorPos, E.Range).To3D());
                    // Core.DelayAction(() => DemSpells.E.Cast(myhero.Position.Extend(Game.CursorPos, DemSpells.E.Range).To3D()), 50);
                }
            }

            Misc();
        }

        public static bool check(Menu submenu, string sig)
        {
            return submenu[sig].Cast<CheckBox>().CurrentValue;
        }

        private static int slider(Menu submenu, string sig)
        {
            return submenu[sig].Cast<Slider>().CurrentValue;
        }

        private static int comb(Menu submenu, string sig)
        {
            return submenu[sig].Cast<ComboBox>().CurrentValue;
        }

        private static bool key(Menu submenu, string sig)
        {
            return submenu[sig].Cast<KeyBind>().CurrentValue;
        }

        private static float ComboDamage(AIHeroClient Nigga)
        {
            if (Nigga != null)
            {
                float TotalDamage = 0;

                if (Q.IsReady()) { TotalDamage += QDamage(Nigga); }

                if (W.IsReady()) { TotalDamage += WDamage(Nigga); }

                if (E.IsReady()) { TotalDamage += EDamage(Nigga); }

                if (R.IsReady()) { TotalDamage += RDamage(Nigga); }

                TotalDamage += myhero.GetAutoAttackDamage(Nigga) * 2;

                return TotalDamage;
            }
            return 0;
        }

        private static float QDamage(AIHeroClient target)
        {
            int index = Q.Level - 1;

            var QDamage = (target.HasBuff("fizzrbonusbuff") ? new[] { 12, 30, 48, 66, 84 }[index] : new[] { 10, 25, 40, 55, 70 }[index]) +
                          ((target.HasBuff("fizzrbonusbuff") ? 0.35f : 0.42f) * myhero.FlatMagicDamageMod);

            return myhero.CalculateDamageOnUnit(target, DamageType.Physical, QDamage);
        }

        private static float WDamage(AIHeroClient target)
        {
            int index = W.Level - 1;

            var WDamage = (target.HasBuff("fizzrbonusbuff") ? new[] { 24, 36, 48, 60, 72 }[index] : new[] { 20, 30, 40, 50, 60 }[index]) +
                          ((target.HasBuff("fizzrbonusbuff") ? 0.54f : 0.45f) * myhero.FlatMagicDamageMod) +
                          ((target.HasBuff("fizzrbonusbuff") ? new[] { 0.048f, 0.054f, 0.06f, 0.066f, 0.072f }[index] : new[] { 0.04f, 0.045f, 0.05f, 0.055f, 0.06f }[index]) *
                          (target.MaxHealth - target.Health));

            return myhero.CalculateDamageOnUnit(target, DamageType.Magical, WDamage);
        }

        private static float EDamage(AIHeroClient target)
        {
            int index = E.Level - 1;

            var EDamage = (target.HasBuff("fizzrbonusbuff") ? new[] { 84, 144, 204, 264, 324 }[index] : new[] { 70, 120, 170, 220, 270 }[index]) +
                          ((target.HasBuff("fizzrbonusbuff") ? 0.9f : 0.75f) * myhero.FlatMagicDamageMod);

            return myhero.CalculateDamageOnUnit(target, DamageType.Magical, EDamage);
        }

        private static float RDamage(AIHeroClient target)
        {
            int index = R.Level - 1;

            var RDamage = new[] { 200, 325, 450 }[index] +
                          myhero.FlatMagicDamageMod;

            return myhero.CalculateDamageOnUnit(target, DamageType.Magical, RDamage);
        }

        private static void Combo()
        {
            var target = TargetSelector.GetTarget(R.Range, DamageType.Magical);
            if (target == null && target.IsInvulnerable)
                return;

            //R
            if (check(combo, "CR") && R.IsReady() && target.IsValidTarget(R.Range - 30) &&
                !target.IsInvulnerable && !target.HasUndyingBuff() && RDamage(target) > target.TotalShieldHealth())
            {

                var Rpred = R.GetPrediction(target);
                if (Rpred.CollisionObjects.Where(x => x is AIHeroClient || x.Name.ToLower().Contains("yasuo")).Count() == 0 &&
                    Rpred.HitChance >= HitChance.High)
                {
                    R.Cast(Rpred.CastPosition);
                }
            }

            if (check(combo, "CQ") && Q.CanCast(target))
            {
                Q.Cast(target);
            }

            if (check(combo, "CW") && W.CanCast(target))
            {
                W.Cast();
            }

            if (check(combo, "CE") && E.IsReady() && target.IsValidTarget(-10 + E.Range * 2))
            {
                CastE(target);
            }
        }

        private static void CastE(AIHeroClient target)
        {
            var Epred = E.GetPrediction(target);

            if (Epred.HitChance >= HitChance.High)
            {
                switch (target.Distance(myhero.Position) < 400)
                {
                    case true:
                        if (E.Name == "FizzJump" && E.Cast(Epred.CastPosition))
                        {
                            if (E.Cast(target.Position)) return;
                        }
                        break;
                    case false:

                        if (E.Name == "FizzJump" && E.Cast(myhero.Position.Extend(Epred.CastPosition, E.Range - 1).To3D()))
                        {
                            if (E.Cast(Epred.CastPosition))
                            {
                                return;
                            }
                        }
                        break;
                }
            }
        }

        private static void Misc()
        {
            var target = TargetSelector.GetTarget(1000, DamageType.Magical, Player.Instance.Position);

            if (check(misc, "autoign") && ignt.IsReady() && target.IsValidTarget(ignt.Range) &&
                myhero.GetSummonerSpellDamage(target, DamageLibrary.SummonerSpells.Ignite) > target.Health)
            {
                ignt.Cast(target);
            }
        }

        public static void OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            /*
               =====================
                Credits => JokerArt
               =====================                           
            */

            var unit = sender as AIHeroClient;

            if (unit == null || !unit.IsValid)
            {
                return;
            }

            if (!unit.IsEnemy || !check(blocking, "BLOCK") || !E.IsReady())
            {
                return;
            }

            if (Evade.SpellDatabase.GetByName(args.SData.Name) != null && !check(blocking, "evade"))
                return;

            if (!SpellBlock.Contains(unit, args))
                return;

            if (args.End.Distance(Player.Instance) == 0)
                return;

            var castUnit = unit;
            var type = args.SData.TargettingType;

            if (!unit.IsValidTarget())
            {
                var target = TargetSelector.GetTarget(E.Range, DamageType.Mixed);
                if (target == null || !target.IsValidTarget(E.Range))
                {
                    target = TargetSelector.SelectedTarget;
                }

                if (target != null && target.IsValidTarget(E.Range))
                {
                    castUnit = target;
                }
            }

            if (unit.ChampionName.Equals("Darius") && args.Slot == SpellSlot.Q && unit.Distance(myhero.Position) < 420)
            {
                Core.DelayAction(() => BlockE(), 700);
            }
          /*  if (unit.ChampionName.Equals("Blitzcrank") && args.Slot == SpellSlot.Q)
            {
                Core.DelayAction(() => BlockE(), 250);
            }*/
            if (unit.ChampionName.Equals("Malphite") && args.Slot == SpellSlot.R && myhero.Position.Distance(args.End) < 300)
            {
                Core.DelayAction(() => BlockE(),
                    ((int)(args.Start.Distance(Player.Instance) / 700 * 1000) -
                    (int)(args.End.Distance(Player.Instance) / 700) - 250) + 100);
            }
            if (unit.ChampionName.Equals("Morgana") && args.Slot == SpellSlot.R && myhero.Position.Distance(args.End) < 300)
            {
                Core.DelayAction(() => BlockE(), 3000);
            }
            if (unit.ChampionName.Equals("KogMaw") && args.Slot == SpellSlot.R && myhero.Position.Distance(args.End) < 240)
            {
                Core.DelayAction(() => BlockE(), 1200);
            }
            if (unit.ChampionName.Equals("Ziggs") && args.Slot == SpellSlot.R && myhero.Position.Distance(args.End) < 550)
            {
                Core.DelayAction(() => BlockE(),
                    ((int)(args.Start.Distance(Player.Instance) / 2800 * 1000) -
                    (int)(args.End.Distance(Player.Instance) / 2800) - 250) + 900);
            }
            if (unit.ChampionName.Equals("Karthus"))
            {
                if (args.Slot == SpellSlot.R)
                {
                    Core.DelayAction(() => BlockE(), 2500);
                }
                else if (args.Slot == SpellSlot.Q && Prediction.Position.PredictUnitPosition(myhero, 499).Distance(args.End) < 100)
                {
                    Core.DelayAction(() => BlockE(), 450);
                }
            }
            if (unit.ChampionName.Equals("Shen") && args.Slot == SpellSlot.E && args.End.Distance(myhero.Position) < 100)
            {
                Core.DelayAction(() => BlockE(),
                    ((int)(args.Start.Distance(Player.Instance) / 1600 * 1000) -
                    (int)(args.End.Distance(Player.Instance) / 1600) - 250) + 250);
            }
            if (unit.ChampionName.Equals("Zyra"))
            {
                Core.DelayAction(() => BlockE(),
                    (int)(args.Start.Distance(Player.Instance) / args.SData.MissileSpeed * 1000) -
                    (int)(args.End.Distance(Player.Instance) / args.SData.MissileSpeed) - 250);
            }
            if (unit.ChampionName.Equals("Amumu") && args.Slot == SpellSlot.R && unit.Distance(myhero.Position) <= 550)
            {
                BlockE();
            }
            if (args.End.Distance(Player.Instance) < 250)
            {
                if (unit.ChampionName.Equals("Bard") && args.End.Distance(Player.Instance) < 300)
                {
                    Core.DelayAction(() => BlockE(), (int)(unit.Distance(Player.Instance) / 7f) + 400);
                }
                else if (unit.ChampionName.Equals("Ashe"))
                {
                    Core.DelayAction(() => BlockE(),
                        (int)(args.Start.Distance(Player.Instance) / args.SData.MissileSpeed * 1000) -
                        (int)args.End.Distance(Player.Instance));
                    return;
                }
                else if (unit.ChampionName.Equals("Varus") || unit.ChampionName.Equals("TahmKench") ||
                         unit.ChampionName.Equals("Lux"))
                {
                    if (unit.ChampionName.Equals("Lux") && args.Slot == SpellSlot.R)
                    {
                        Core.DelayAction(() => BlockE(), 400);
                    }
                    Core.DelayAction(() => BlockE(),
                        (int)(args.Start.Distance(Player.Instance) / args.SData.MissileSpeed * 1000) -
                        (int)(args.End.Distance(Player.Instance) / args.SData.MissileSpeed) - 250);

                }
                else if (unit.ChampionName.Equals("Amumu"))
                {
                    if (sender.Distance(Player.Instance) < 1100)
                        Core.DelayAction(() => BlockE(),
                            (int)(args.Start.Distance(Player.Instance) / args.SData.MissileSpeed * 1000) -
                            (int)(args.End.Distance(Player.Instance) / args.SData.MissileSpeed) - 250);
                }
            }

            if (args.Target != null && type.Equals(SpellDataTargetType.Unit))
            {
                if (!args.Target.IsMe ||
                    (args.Target.Name.Equals("Barrel") && args.Target.Distance(Player.Instance) > 200 &&
                     args.Target.Distance(Player.Instance) < 400))
                {
                    return;
                }

                if (unit.ChampionName.Equals("Nautilus") ||
                    (unit.ChampionName.Equals("Caitlyn") && args.Slot.Equals(SpellSlot.R)))
                {
                    var d = unit.Distance(Player.Instance);
                    var travelTime = d / 3200;
                    var delay = Math.Floor(travelTime * 1000) + 900;
                    Core.DelayAction(() => BlockE(), (int)delay);
                    return;
                }
                BlockE();
            }

            if (type.Equals(SpellDataTargetType.Unit))
            {
                if (unit.ChampionName.Equals("Bard") && args.End.Distance(Player.Instance) < 300)
                {
                    Core.DelayAction(() => BlockE(), 400 + (int)(unit.Distance(Player.Instance) / 7f));
                }
                else if (unit.ChampionName.Equals("Riven") && args.End.Distance(Player.Instance) < 260)
                {
                    BlockE();
                }
                else
                {
                    BlockE();
                }
            }
            else if (type.Equals(SpellDataTargetType.LocationAoe) &&
                     args.End.Distance(Player.Instance) < args.SData.CastRadius)
            {
                if (unit.ChampionName.Equals("Annie") && args.Slot.Equals(SpellSlot.R))
                {
                    return;
                }
                BlockE();
            }
            else if (type.Equals(SpellDataTargetType.Cone) &&
                     args.End.Distance(Player.Instance) < args.SData.CastRadius)
            {
                BlockE();
            }
            else if (type.Equals(SpellDataTargetType.SelfAoe) || type.Equals(SpellDataTargetType.Self))
            {
                var d = args.End.Distance(Player.Instance.ServerPosition);
                var p = args.SData.CastRadius > 5000 ? args.SData.CastRange : args.SData.CastRadius;
                if (d < p)
                    BlockE();
            }
        }

        public static void BlockE()
        {
            switch (comb(blocking, "BLOCKMODE"))
            {
                case 0:
                    E.Cast(Player.Instance.Position);
                    break;
                case 1:
                    if (Game.CursorPos.Distance(myhero.Position) > 399)
                    {
                        E.Cast(myhero.Position.Extend(Game.CursorPos, R.Range - 1).To3D());
                    }
                    else
                    {
                        E.Cast(Game.CursorPos);
                    }                   
                    break;
            }
        }


        public static void DatMenu()
        {
            menu = MainMenu.AddMenu("T7 " + "Fizz", "Fizz".ToLower());
            combo = menu.AddSubMenu("Combo", "combo");
            misc = menu.AddSubMenu("Misc", "misc");
            flee = menu.AddSubMenu("Flee", "fleee");
            blocking = menu.AddSubMenu("Spellblocking", "blocks");

            menu.AddGroupLabel("Welcome to T7 Fizz And Thank You For Using!");
            menu.AddLabel("Author: Toyota7");
            menu.AddSeparator();
            menu.AddLabel("Please Report Any Bugs And If You Have Any Requests Feel Free To PM Me <3");

            combo.AddLabel("Spells");
            combo.Add("CQ", new CheckBox("Use Q", true));
            combo.Add("CW", new CheckBox("Use W", true));
            combo.Add("CE", new CheckBox("Use E", true));
            combo.Add("CR", new CheckBox("Use R", true));
            combo.AddSeparator();
            combo.Add("CIGNT", new CheckBox("Use Ignite", false));
       //     combo.Add("EGAP", new CheckBox("Use E To Gapclose if enemy out of range", true));

            blocking.Add("BLOCK", new CheckBox("Auto Block Spells With E"));
            blocking.Add("evade", new CheckBox("Evade Integration"));
            blocking.Add("BLOCKMODE", new ComboBox("Jump To", 1, "Player Position", "Mouse Position"));
            blocking.AddSeparator();

            flee.AddGroupLabel("Flee");
            flee.Add("EFLEE", new CheckBox("Use E To Flee", true));
            flee.AddLabel("(Casts To Mouse Position)");
            flee.AddSeparator();           

            misc.AddLabel("Killsteal");
            misc.Add("autoign", new CheckBox("Auto Ignite If Killable", false));
        }
    }
}
