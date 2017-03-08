﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Enumerations;
using EloBuddy.SDK.Events;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using EloBuddy.SDK.Rendering;
using SharpDX;
using SharpDX.Direct3D9;
using Font = SharpDX.Direct3D9.Font;
using Color = System.Drawing.Color;

namespace Kayle
{
    internal class Program
    {
        public static Menu Menu, ComboMenu, HarassMenu, Ulti, Heal, LaneClearMenu, JungleClearMenu, KillStealMenu, Misc;
        public static AIHeroClient _Player
        {
            get { return ObjectManager.Player; }
        }
        public static Spell.Targeted Q;
        public static Spell.Targeted W;
        public static Spell.Active E;
        public static Spell.Targeted R;
        private static Font thm;
        public static Spell.Targeted Ignite;

        static void Main(string[] args)
        {
            Loading.OnLoadingComplete += OnLoadingComplete;
        }

        private static void OnLoadingComplete(EventArgs args)
        {
            if (!_Player.ChampionName.Contains("Kayle")) return;
            Chat.Print("Doctor's Kayle Loaded!", Color.Orange);
            Q = new Spell.Targeted(SpellSlot.Q, 650);
            W = new Spell.Targeted(SpellSlot.W, 900);
            E = new Spell.Active(SpellSlot.E, (uint)Player.Instance.GetAutoAttackRange());
            R = new Spell.Targeted(SpellSlot.R, 900);
            Ignite = new Spell.Targeted(ObjectManager.Player.GetSpellSlotFromName("summonerdot"), 600);
            thm = new Font(Drawing.Direct3DDevice, new FontDescription { FaceName = "Tahoma", Height = 22, Weight = FontWeight.Bold, OutputPrecision = FontPrecision.Default, Quality = FontQuality.ClearType });
            Menu = MainMenu.AddMenu("Kayle", "Kayle");
            Menu.AddGroupLabel("Doctor7");
            ComboMenu = Menu.AddSubMenu("Combo Settings", "Combo");
            ComboMenu.AddGroupLabel("Combo Settings");
            ComboMenu.Add("ComboQ", new CheckBox("Use [Q] Combo"));
            ComboMenu.Add("ComboE", new CheckBox("Use [E] Combo"));

            Ulti = Menu.AddSubMenu("Ultimate Settings", "Ulti");
            Ulti.AddGroupLabel("Ultimate Settings");
            Ulti.Add("ultiR2", new CheckBox("Use [R]"));
            Ulti.Add("Alhp", new Slider("HP Use [R]", 25));
            Ulti.AddGroupLabel("Use [R] On");
            foreach (var target in EntityManager.Heroes.Allies)
            {
                Ulti.Add("useRon" + target.ChampionName, new CheckBox("" + target.ChampionName));
            }

            Heal = Menu.AddSubMenu("Heal Settings", "Heal");
            Heal.AddGroupLabel("Heal Settings");
            Heal.Add("healW2", new CheckBox("Use [W] Allies"));
            Heal.Add("ManaHeal", new Slider("Mana Use Heal", 20));
            Heal.Add("AlW", new Slider("Allies HP Use [W]", 70));
            Heal.AddGroupLabel("Use [W] On");
            foreach (var target in EntityManager.Heroes.Allies)
            {
                Heal.Add("useWon" + target.ChampionName, new CheckBox("" + target.ChampionName));
            }

            HarassMenu = Menu.AddSubMenu("Harass Settings", "Harass");
            HarassMenu.AddGroupLabel("Harass Settings");
            HarassMenu.Add("HarassQ", new CheckBox("Use [Q] Harass"));
            HarassMenu.Add("HarassE", new CheckBox("Use [E] Harass"));
            HarassMenu.Add("ManaHR", new Slider("Mana For Harass", 50));

            LaneClearMenu = Menu.AddSubMenu("LaneClear Settings", "LaneClear");
            LaneClearMenu.AddGroupLabel("Lane Clear Settings");
            LaneClearMenu.Add("QLC", new CheckBox("Use [Q] LaneClear", false));
            LaneClearMenu.Add("ELC", new CheckBox("Use [E] LaneClear"));
            LaneClearMenu.Add("ManaLC", new Slider("Mana For LaneClear", 50));
            LaneClearMenu.AddGroupLabel("Lasthit Settings");
            LaneClearMenu.Add("QLH", new CheckBox("Use [Q] Lasthit"));
            LaneClearMenu.Add("ManaLH", new Slider("Mana For Lasthit", 50));

            JungleClearMenu = Menu.AddSubMenu("JungleClear Settings", "JungleClear");
            JungleClearMenu.AddGroupLabel("JungleClear Settings");
            JungleClearMenu.Add("QJungle", new CheckBox("Use [Q] JungleClear"));
            JungleClearMenu.Add("EJungle", new CheckBox("Use [E] JungleClear"));
            JungleClearMenu.Add("ManaJC", new Slider("Mana For JungleClear", 30));

            KillStealMenu = Menu.AddSubMenu("KillSteal Settings", "KillSteal");
            KillStealMenu.AddGroupLabel("KillSteal Settings");
            KillStealMenu.Add("KsQ", new CheckBox("Use [Q] KillSteal"));
            KillStealMenu.Add("ign", new CheckBox("Use [Ignite] KillSteal"));

            Misc = Menu.AddSubMenu("Misc Settings", "Misc");
            Misc.AddGroupLabel("Drawing Settings");
            Misc.Add("DrawQ", new CheckBox("[Q] Range", false));
            Misc.Add("DrawE", new CheckBox("[E] Range", false));
            Misc.Add("DrawR", new CheckBox("[R] - [W] Range", false));
            Misc.Add("DrawIE", new CheckBox("Status [E] Buff", false));

            Drawing.OnDraw += Drawing_OnDraw;
            Game.OnUpdate += Game_OnUpdate;
        }

        private static void Drawing_OnDraw(EventArgs args)
        {
            if (Misc["DrawR"].Cast<CheckBox>().CurrentValue)
            {
                new Circle() { Color = Color.Orange, BorderWidth = 3, Radius = R.Range }.Draw(_Player.Position);
            }
			
            if (Misc["DrawE"].Cast<CheckBox>().CurrentValue && Player.Instance.HasBuff("JudicatorRighteousFury"))
            {
                new Circle() { Color = Color.Red, BorderWidth = 3, Radius = E.Range }.Draw(_Player.Position);
            }
			
            if (Misc["DrawQ"].Cast<CheckBox>().CurrentValue)
            {
                new Circle() { Color = Color.Orange, BorderWidth = 3, Radius = Q.Range }.Draw(_Player.Position);
            }
			
            if (Misc["DrawIE"].Cast<CheckBox>().CurrentValue)
            {
                Vector2 ft = Drawing.WorldToScreen(_Player.Position);
                if (Player.Instance.HasBuff("JudicatorRighteousFury"))
                {
                    DrawFont(thm, "Righteous Fury : " + ETime(Player.Instance), (float)(ft[0] - 125), (float)(ft[1] + 50), SharpDX.Color.Pink);
                }
            }
        }

        public static float ETime(Obj_AI_Base target)
        {
            if (target.HasBuff("JudicatorRighteousFury"))
            {
                return Math.Max(0, target.GetBuff("JudicatorRighteousFury").EndTime) - Game.Time;
            }
            return 0;
        }

        private static void Game_OnUpdate(EventArgs args)
        {
            E = new Spell.Active(SpellSlot.E, (uint)Player.Instance.GetAutoAttackRange());

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear))
            {
                LaneClear();
            }
			
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LastHit))
            {
                LastHit();
            }
			
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass))
            {
                Harass();
            }
			
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
			
            KillSteal();
            Ultimate();
            Heals();
        }

        private static void Combo()
        {
            var target = TargetSelector.GetTarget(Q.Range, DamageType.Magical);
            var useQ = ComboMenu["ComboQ"].Cast<CheckBox>().CurrentValue;
            var useE = ComboMenu["ComboE"].Cast<CheckBox>().CurrentValue;
            if (target != null)
            {
                if (useQ && Q.IsReady() && target.IsValidTarget(Q.Range))
                {
                    Q.Cast(target);
                }
				
                if (useE && E.IsReady() && target.IsValidTarget(550))
                {
                    E.Cast();
                }
            }
        }
		
        public static bool Tru(Vector3 position)
        {
            return ObjectManager.Get<Obj_AI_Turret>().Any(turret => turret.IsValidTarget(950) && turret.IsEnemy);
        }
		
        private static void Ultimate()
        {
            var almin = Ulti["Alhp"].Cast<Slider>().CurrentValue;
            var useR2 = Ulti["ultiR2"].Cast<CheckBox>().CurrentValue;
            var target = EntityManager.Heroes.Allies.Where(a => a.IsValidTarget() && a.Distance(_Player.Position) <= R.Range && !a.IsDead && !a.IsZombie && !a.HasBuff("kindredrnodeathbuff") && !a.HasBuff("Undying Rage") && !a.HasBuff("JudicatorIntervention") && !a.HasBuff("Recall"));
            foreach (var ally in target)
            {
                if (useR2 && !Player.Instance.IsInShopRange() && R.IsReady() && (!Player.Instance.IsRecalling()) && (ally.Position.CountEnemyChampionsInRange(R.Range) >= 1 || Tru(ally.Position)))
                {
                    if (Ulti["useRon" + ally.ChampionName].Cast<CheckBox>().CurrentValue && (ally.HealthPercent <= almin || ally.HasBuff("ZedR")))
                    {
                        R.Cast(ally);
                    }
                }
            }
        }

        private static void Heals()
        {
            var almin = Heal["AlW"].Cast<Slider>().CurrentValue;
            var useW2 = Heal["healW2"].Cast<CheckBox>().CurrentValue;
            var mana = Heal["ManaHeal"].Cast<Slider>().CurrentValue;
            var target = EntityManager.Heroes.Allies.Where(a => a.IsValidTarget() && a.Distance(_Player.Position) <= W.Range && !a.IsDead && !a.IsZombie && !a.HasBuff("kindredrnodeathbuff") && !a.HasBuff("JudicatorIntervention") && !a.HasBuff("Recall"));
            if (Player.Instance.ManaPercent <= mana) return;
            foreach (var target2 in target)
            {
                if (useW2 && W.IsReady() && !Player.Instance.IsInShopRange() && !Player.Instance.IsRecalling())
                {
                    if (Heal["useWon" + target2.ChampionName].Cast<CheckBox>().CurrentValue && (target2.HealthPercent <= almin || target2.HasBuff("ZedR")))
                    {
                        W.Cast(target2);
                    }
                }
            }
        }

        private static void LaneClear()
        {
            var useQ = LaneClearMenu["QLC"].Cast<CheckBox>().CurrentValue;
            var useE = LaneClearMenu["ELC"].Cast<CheckBox>().CurrentValue;
            var mana = LaneClearMenu["ManaLC"].Cast<Slider>().CurrentValue;
            var minion = EntityManager.MinionsAndMonsters.GetLaneMinions().Where(a => a.Distance(Player.Instance) <= Q.Range).OrderBy(a => a.Health).FirstOrDefault();
            if (Player.Instance.ManaPercent < mana) return;
            if (minion != null)
            {
                if (useE && E.IsReady() && minion.IsValidTarget(550) && _Player.Position.CountEnemyMinionsInRange(550) >= 3)
                {
                    E.Cast();
                }
				
                if (useQ && Q.IsReady() && minion.IsValidTarget(Q.Range) && Player.Instance.GetSpellDamage(minion, SpellSlot.Q) >= minion.TotalShieldHealth())
                {
                    Q.Cast(minion);
                }
            }
        }

        public static void DrawFont(Font vFont, string vText, float jx, float jy, ColorBGRA jc)
        {
            vFont.DrawText(null, vText, (int)jx, (int)jy, jc);
        }

        private static void LastHit()
        {
            var useQ = LaneClearMenu["QLH"].Cast<CheckBox>().CurrentValue;
            var mana = LaneClearMenu["ManaLH"].Cast<Slider>().CurrentValue;
            var minion = EntityManager.MinionsAndMonsters.GetLaneMinions().Where(a => a.Distance(Player.Instance) <= Q.Range).OrderBy(a => a.Health).FirstOrDefault();
            if (Player.Instance.ManaPercent < mana) return;
            if (minion != null)
            {
                if (useQ && Q.IsReady() && minion.IsValidTarget(Q.Range) && Player.Instance.GetSpellDamage(minion, SpellSlot.Q) >= minion.TotalShieldHealth())
                {
                    Q.Cast(minion);
                }
            }
        }

        private static void Harass()
        {
            var useQ = HarassMenu["HarassQ"].Cast<CheckBox>().CurrentValue;
            var useE = HarassMenu["HarassE"].Cast<CheckBox>().CurrentValue;
            var mana = HarassMenu["ManaHR"].Cast<Slider>().CurrentValue;
            var target = TargetSelector.GetTarget(Q.Range, DamageType.Magical);
            if (Player.Instance.ManaPercent <= mana) return;
            if (target != null)
            {
                if (useQ && Q.IsReady() && target.IsValidTarget(Q.Range))
                {
                    Q.Cast(target);
                }
				
                if (useE && E.IsReady() && target.IsValidTarget(550))
                {
                    E.Cast();
                }
            }
        }

        public static void JungleClear()
        {
            var useQ = JungleClearMenu["QJungle"].Cast<CheckBox>().CurrentValue;
            var useE = JungleClearMenu["EJungle"].Cast<CheckBox>().CurrentValue;
            var mana = JungleClearMenu["ManaJC"].Cast<Slider>().CurrentValue;
            var monster = EntityManager.MinionsAndMonsters.GetJungleMonsters(Player.Instance.Position, 550).OrderByDescending(a => a.MaxHealth).FirstOrDefault();
            if (Player.Instance.ManaPercent <= mana) return;
            if (monster != null)
            {
                if (useQ && Q.IsReady() && monster.IsValidTarget(Q.Range))
                {
                    Q.Cast(monster);
                }
				
                if (useE && E.IsReady() && monster.IsValidTarget(Q.Range))
                {
                    E.Cast();
                }
            }
        }

        public static void Flee()
        {
            var target = TargetSelector.GetTarget(Q.Range, DamageType.Physical);
            if (target != null)
            {
                if (Q.IsReady() && target.IsValidTarget(Q.Range))
                {
                    Q.Cast(target);
                }
            }
			
            if (W.IsReady())
            {
                W.Cast(Player.Instance);
            }
        }

        private static void KillSteal()
        {
            var KsQ = KillStealMenu["KsQ"].Cast<CheckBox>().CurrentValue;
            foreach (var target in EntityManager.Heroes.Enemies.Where(hero => hero.IsValidTarget(Q.Range) && !hero.HasBuff("JudicatorIntervention") && !hero.HasBuff("kindredrnodeathbuff") && !hero.HasBuff("Undying Rage") && !hero.IsDead && !hero.IsZombie))
            {
                if (KsQ && Q.IsReady() && target.IsValidTarget(Q.Range))
                {
                    if (target.TotalShieldHealth() <= Player.Instance.GetSpellDamage(target, SpellSlot.Q))
                    {
                        Q.Cast(target);
                    }
                }

                if (Ignite != null && KillStealMenu["ign"].Cast<CheckBox>().CurrentValue && Ignite.IsReady())
                {
                    if (target.TotalShieldHealth() <= _Player.GetSummonerSpellDamage(target, DamageLibrary.SummonerSpells.Ignite))
                    {
                        Ignite.Cast(target);
                    }
                }
            }
        }
    }
}
