﻿using System;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Enumerations;
using EloBuddy.SDK.Events;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using SharpDX;

namespace Tristana
{
    static class Program
    {
        public static Spell.Active Q;
        public static Spell.Skillshot W;
        public static Spell.Targeted E;
        public static Spell.Targeted R;

        public static AIHeroClient _Player
        {
            get { return ObjectManager.Player; }
        }
        public static Menu Menu, SpellMenu, JungleMenu, HarassMenu, LaneMenu, Misc;

        static void Main(string[] args)
        {
            Loading.OnLoadingComplete += OnLoadingComplete;
        }

// Menu

        private static void OnLoadingComplete(EventArgs args)
        {
            if (!_Player.ChampionName.Contains("Tristana")) return;
            Chat.Print("Doctor's Tristana Loaded!", Color.Orange);
            uint level = (uint)Player.Instance.Level;
            Q = new Spell.Active(SpellSlot.Q);
            W = new Spell.Skillshot(SpellSlot.W, 900, SkillShotType.Circular, 450, int.MaxValue, 180);
            E = new Spell.Targeted(SpellSlot.E, 550 + level * 7);
            R = new Spell.Targeted(SpellSlot.R, 550 + level * 7);

            Menu = MainMenu.AddMenu("Doctor's Tristana", "Tristana");
            SpellMenu = Menu.AddSubMenu("Combo Settings", "Combo");
            SpellMenu.AddGroupLabel("Combo Settings");
            SpellMenu.Add("ComboQ", new CheckBox("Use [Q] Combo"));
            SpellMenu.Add("ComboE", new CheckBox("Use [E] Combo"));
            SpellMenu.AddGroupLabel("Combo [E] On");
            foreach (var target in EntityManager.Heroes.Enemies)
            {
                SpellMenu.Add("useECombo" + target.ChampionName, new CheckBox("" + target.ChampionName));
            }
            SpellMenu.AddGroupLabel("KillSteal Settings");
            SpellMenu.Add("ERKs", new CheckBox("KillSteal [ER]"));
            SpellMenu.Add("RKs", new CheckBox("Automatic [R] KillSteal"));
            SpellMenu.Add("RKb", new KeyBind(" Semi Manual [R] KillSteal", false, KeyBind.BindTypes.HoldActive, 'R'));
            SpellMenu.AddGroupLabel("[W] KillSteal Settings");
            SpellMenu.Add("WKs", new CheckBox("Use [W] KillSteal", false));
            SpellMenu.Add("CTurret", new CheckBox("Dont Use [W] KillSteal Under Turet"));
            SpellMenu.Add("Attack", new Slider("Use [W] KillSteal If Can Kill Enemy With x Attack", 2, 1, 6));
            SpellMenu.Add("MinW", new Slider("Use [W] KillSteal If Enemies Around Target <=", 2, 1, 5));
            SpellMenu.AddLabel("Always Use [W] KillSteal If Slider Enemies Around = 5");

            HarassMenu = Menu.AddSubMenu("Harass Settings", "Harass");
            HarassMenu.AddGroupLabel("Harass Settings");
            HarassMenu.Add("HarassQ", new CheckBox("Use [Q] Harass", false));
            HarassMenu.Add("HarassE", new CheckBox("Use [E] Harass"));
            HarassMenu.AddSeparator();
            HarassMenu.AddGroupLabel("Use [E] On");
            foreach (var target in EntityManager.Heroes.Enemies)
            {
                HarassMenu.Add("HarassE" + target.ChampionName, new CheckBox("" + target.ChampionName));
            }
            HarassMenu.Add("manaHarass", new Slider("Min Mana For Harass", 50, 0, 100));

            LaneMenu = Menu.AddSubMenu("Laneclear Settings", "Clear");
            LaneMenu.AddGroupLabel("Laneclear Settings");
            LaneMenu.Add("ClearQ", new CheckBox("Use [Q] Laneclear", false));
            LaneMenu.Add("ClearE", new CheckBox("Use [E] Laneclear", false));
            LaneMenu.Add("manaFarm", new Slider("Min Mana For LaneClear", 50, 0, 100));

            JungleMenu = Menu.AddSubMenu("JungleClear Settings", "JungleClear");
            JungleMenu.AddGroupLabel("JungleClear Settings");
            JungleMenu.Add("jungleQ", new CheckBox("Use [Q] JungleClear"));
            JungleMenu.Add("jungleE", new CheckBox("Use [E] JungleClear"));
            JungleMenu.Add("jungleW", new CheckBox("Use [W] JungleClear", false));
            JungleMenu.Add("manaJung", new Slider("Min Mana For JungleClear", 50, 0, 100));

            Misc = Menu.AddSubMenu("Misc Settings", "Misc");
            Misc.AddGroupLabel("Anti Gapcloser");
            Misc.Add("antiGap", new CheckBox("Anti Gapcloser", false));
            Misc.Add("antiRengar", new CheckBox("Anti Rengar"));
            Misc.Add("antiKZ", new CheckBox("Anti Kha'Zix"));
            Misc.Add("inter", new CheckBox("Use [R] Interupt", false));

            Game.OnTick += Game_OnTick;
            Gapcloser.OnGapcloser += Gapcloser_OnGapCloser;
            Interrupter.OnInterruptableSpell += Interupt;
            GameObject.OnCreate += GameObject_OnCreate;

        }

// Damage

        public static float EDamage(Obj_AI_Base target)
        {
            float Edamage = 0;
            if (target.HasBuff("tristanaecharge"))
            {
                Edamage += (float)(Player.Instance.GetSpellDamage(target, SpellSlot.E) * (target.GetBuffCount("tristanaecharge") * 0.30)) + Player.Instance.GetSpellDamage(target, SpellSlot.E);
            }

            return Edamage;
        }

        public static float RDamage(Obj_AI_Base target)
        {
            return _Player.CalculateDamageOnUnit(target, DamageType.Magical,
                (float)(new[] { 0, 300, 400, 500 }[R.Level] + 1.0f * _Player.FlatMagicDamageMod));
        }

        public static float GetDamage(AIHeroClient target)
        {
            if (target != null)
            {
                float Damage = 0;

                if (E.IsLearned) { Damage += EDamage(target); }
                if (R.IsLearned) { Damage += RDamage(target); }

                return Damage;
            }
            return 0;
        }

// Flee Mode

        private static void Flee()
        {
            if (W.IsReady())
            {
                var cursorPos = Game.CursorPos;
                var castPos = Player.Instance.Position.Distance(cursorPos) <= W.Range ? cursorPos : Player.Instance.Position.Extend(cursorPos, W.Range).To3D();
                W.Cast(castPos);
            }
        }

// Interrupt

        private static void Interupt(Obj_AI_Base sender, Interrupter.InterruptableSpellEventArgs i)
        {
            var Inter = Misc["inter"].Cast<CheckBox>().CurrentValue;
            if (!sender.IsEnemy || !(sender is AIHeroClient) || Player.Instance.IsRecalling())
            {
                return;
            }
			
            if (Inter && R.IsReady() && i.DangerLevel == DangerLevel.Medium && R.IsInRange(sender))
            {
                R.Cast(sender);
            }
        }

//Harass Mode

        private static void Harass()
        {
            var useQ = HarassMenu["HarassQ"].Cast<CheckBox>().CurrentValue;
            var useE = HarassMenu["HarassE"].Cast<CheckBox>().CurrentValue;
            var mana = HarassMenu["manaHarass"].Cast<Slider>().CurrentValue;
            foreach (var target in EntityManager.Heroes.Enemies.Where(e => e.IsValidTarget(E.Range) && !e.IsDead && !e.IsZombie))
            {
                if (useQ && Q.IsReady() && target.IsValidTarget(E.Range))
                {
                    Q.Cast();
                }
				
                if (useE && E.IsReady() && target.IsValidTarget(E.Range) && target.HealthPercent >= 10 && _Player.ManaPercent >= mana)
                {
                    if (HarassMenu["HarassE" + target.ChampionName].Cast<CheckBox>().CurrentValue)
                    {
                        E.Cast(target);
                    }
                }
            }
        }

//Combo Mode

        private static void Combo()
        {
            var useQ = SpellMenu["ComboQ"].Cast<CheckBox>().CurrentValue;
            var useE = SpellMenu["ComboE"].Cast<CheckBox>().CurrentValue;
            foreach (var target in EntityManager.Heroes.Enemies.Where(e => e.IsValidTarget(E.Range) && !e.IsDead && !e.IsZombie))
            {
                if (useQ && Q.IsReady() && target.IsValidTarget(E.Range))
                {
                    Q.Cast();
                }

                if (useE && E.IsReady() && target.IsValidTarget(E.Range) && target.HealthPercent >= 10)
                {
                    if (SpellMenu["useECombo" + target.ChampionName].Cast<CheckBox>().CurrentValue)
                    {
                        E.Cast(target);
                    }
                }
            }
        }

//LaneClear Mode

        private static void LaneClear()
        {
            var useQ = LaneMenu["ClearQ"].Cast<CheckBox>().CurrentValue;
            var useE = LaneMenu["ClearE"].Cast<CheckBox>().CurrentValue;
            var mana = LaneMenu["manaFarm"].Cast<Slider>().CurrentValue;
            foreach (var minion in EntityManager.MinionsAndMonsters.GetLaneMinions().Where(e => e.IsValidTarget(E.Range)))
            {
                if (useE && E.IsReady() && minion.HealthPercent >= 70 && minion.IsValidTarget(E.Range) && _Player.ManaPercent >= mana)
                {
                    E.Cast(minion);
                }

                if (useQ && Q.IsReady() && minion.IsValidTarget(E.Range) && _Player.CountEnemyMinionsInRange(E.Range) >= 3)
                {
                    Q.Cast();
                }
            }
        }

// JungleClear Mode

        private static void JungleClear()
        {
            var monster = EntityManager.MinionsAndMonsters.GetJungleMonsters().OrderByDescending(j => j.Health).FirstOrDefault(j => j.IsValidTarget(E.Range));
            var useQ = JungleMenu["jungleQ"].Cast<CheckBox>().CurrentValue;
            var useW = JungleMenu["jungleW"].Cast<CheckBox>().CurrentValue;
            var useE = JungleMenu["jungleE"].Cast<CheckBox>().CurrentValue;
            var mana = JungleMenu["manaJung"].Cast<Slider>().CurrentValue;
            if (monster != null)
            {
                if (useQ && Q.IsReady() && monster.IsValidTarget(E.Range))
                {
                    Q.Cast();
                }
				
                if (_Player.ManaPercent < mana) return;

                if (useW && W.IsReady() && monster.IsValidTarget(W.Range))
                {
                    W.Cast(monster.Position);
                }
				
                if (useE && E.IsReady() && monster.IsValidTarget(E.Range))
                {
                    E.Cast(monster);
                }
            }
        }


// Anti Rengar

        private static void GameObject_OnCreate(GameObject sender, EventArgs args)
        {
            var rengar = EntityManager.Heroes.Enemies.Find(e => e.ChampionName.Equals("Rengar"));
            var khazix = EntityManager.Heroes.Enemies.Find(e => e.ChampionName.Equals("Khazix"));
            if (rengar != null)
            {
                if (sender.Name == ("Rengar_LeapSound.troy") && Misc["antiRengar"].Cast<CheckBox>().CurrentValue && R.IsReady() && sender.Position.Distance(_Player) <= 300)
                {
                    R.Cast(rengar);
                }
            }

            if (khazix != null)
            {
                if (sender.Name == ("Khazix_Base_E_Tar.troy") && Misc["antiKZ"].Cast<CheckBox>().CurrentValue && R.IsReady() && sender.Position.Distance(_Player) <= 300)
                {
                    R.Cast(khazix);
                }
            }
        }

        private static void Gapcloser_OnGapCloser(Obj_AI_Base sender, Gapcloser.GapcloserEventArgs args)
        {
            if (Misc["antiGap"].Cast<CheckBox>().CurrentValue && R.IsReady() && args.Sender.Distance(_Player) <= 325)
            {
                R.Cast(args.Sender);
            }
        }

// KillSteal

        private static void KillSteal()
        {
            var target = EntityManager.Heroes.Enemies.Where(e => e.IsValidTarget(W.Range) && !e.HasBuff("JudicatorIntervention") && !e.HasBuff("kindredrnodeathbuff") && !e.HasBuff("Undying Rage") && !e.IsDead && !e.IsZombie);
            var RKill = SpellMenu["RKs"].Cast<CheckBox>().CurrentValue;
            var WKill = SpellMenu["WKs"].Cast<CheckBox>().CurrentValue;
            var WAttack = SpellMenu["Attack"].Cast<Slider>().CurrentValue;
            var minW = SpellMenu["MinW"].Cast<Slider>().CurrentValue;
            foreach (var target2 in target)
            {
                if (R.IsReady() && target2.IsValidTarget(R.Range) && (RKill || SpellMenu["RKb"].Cast<KeyBind>().CurrentValue))
                {
                    if (target2.TotalShieldHealth() <= RDamage(target2))
                    {
                        R.Cast(target2);
                    }
                }

                if (WKill && W.IsReady() && target2.IsValidTarget(W.Range))
                {
                    if (target2.TotalShieldHealth() <= Player.Instance.GetAutoAttackDamage(target2) * WAttack && Player.Instance.Mana > W.Handle.SData.Mana * 2 && Player.Instance.HealthPercent >= 10 && target2.Position.CountEnemyChampionsInRange(600) <= minW)
                    {
                        var turret = SpellMenu["CTurret"].Cast<CheckBox>().CurrentValue;
                        if (target2.HasBuff("tristanaecharge"))
                        {
                            if (target2.TotalShieldHealth() > EDamage(target2))
                            {
                                if (turret)
                                {
                                    if (!target2.Position.UnderTuret())
                                    {
                                       W.Cast(target2.ServerPosition);
                                    }
                                }
                                else
                                {
                                   W.Cast(target2.ServerPosition);
                                }
                            }
                        }
                        else
                        {
                            if (turret)
                            {
                                if (!target2.Position.UnderTuret())
                                {
                                   W.Cast(target2.ServerPosition);
                                }
                            }
                            else
                            {
                               W.Cast(target2.ServerPosition);
                            }
                        }
                    }
                }

                if (SpellMenu["ERKs"].Cast<CheckBox>().CurrentValue && R.IsReady() && target2.IsValidTarget(R.Range) && target2.HasBuff("tristanaecharge"))
                {
                    if (target2.TotalShieldHealth() <= GetDamage(target2))
                    {
                        R.Cast(target2);
                    }
                }
            }
        }


// Under Turet

        private static bool UnderTuret(this Vector3 position)
        {
            return EntityManager.Turrets.Enemies.Where(a => a.Health > 0 && !a.IsDead).Any(a => a.Distance(position) < 950);
        }

// Game Update

        private static void Game_OnTick(EventArgs args)
        {
            uint level = (uint)Player.Instance.Level;
            E = new Spell.Targeted(SpellSlot.E, 550 + level * 7);
            R = new Spell.Targeted(SpellSlot.R, 550 + level * 7);

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.JungleClear))
            {
                JungleClear();
            }
			
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Flee))
            {
                Flee();
            }
			
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
            {
                Combo();
            }
			
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass))
            {
                Harass();
            }
			
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear))
            {
                LaneClear();
            }
			
            KillSteal();
        }
    }
}
