﻿using System;
using System.Collections.Generic;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Events;
using EloBuddy.SDK.Enumerations;
using SharpDX;
using WuAIO.Bases;
using WuAIO.Managers;
using WuAIO.Extensions;
using WuAIO.Utilities;
using Circle = EloBuddy.SDK.Rendering.Circle;
using EloBuddy.SDK.Menu.Values;
using ScalingTypes = WuAIO.Managers.DamageManager.ScalingTypes;

namespace WuAIO
{
    class Riven : HeroBase
    {
        readonly AIHeroClient Player = EloBuddy.Player.Instance;
        AIHeroClient Target;

        byte qstate = 1;
        byte rstate = 1;
        float lastq;

        bool r2Actived { get { return rstate == 2 || R.Name == "rivenizunablad" || R1.Name == "rivenizunablad"; } }

        //bool nextStep, forceR, forceW, forceQ, forceE, forceHydra, forceTiamat, forceFlash;

        bool aaFinished;

        List<ComboSpell> _order;
        int _i;

        Spell.Skillshot flash;

        Item tiamat = new Item(ItemId.Tiamat, 350),//400
            hydra = new Item(ItemId.Ravenous_Hydra, 350),//400
            titanic = new Item(ItemId.Titanic_Hydra, 100),
            youmouu = new Item(ItemId.Youmuus_Ghostblade);//150

        readonly Spell.Skillshot Q = new Spell.Skillshot(SpellSlot.Q, 260, SkillShotType.Linear);
        //readonly Spell.Active Q = new Spell.Active(SpellSlot.Q);
        readonly Spell.Active W = new Spell.Active(SpellSlot.W, 250);//250+/-
        readonly Spell.Skillshot E = new Spell.Skillshot(SpellSlot.E, 325, SkillShotType.Linear);
        readonly Spell.Active R1 = new Spell.Active(SpellSlot.R);
        readonly Spell.Skillshot R = new Spell.Skillshot(SpellSlot.R, 900, SkillShotType.Cone, 250, 1600, 900);
        //                                         h                  1000

        public override void CreateMenu()
        {
            base.CreateMenu();

            var menu = MenuManager.AddSubMenu("Drawings");
            {
                menu.NewCheckbox("disable", "Disable", false);
                menu.NewCheckbox("damageindicator", "Damage Indicator");
                menu.NewCheckbox("w", "W");
                menu.NewCheckbox("e", "E");
                menu.NewCheckbox("r", "R");
                menu.NewCheckbox("burst", "Burst possible", true);
                menu.NewCheckbox("flash+w", "Flash+W");
            }

            menu = MenuManager.AddSubMenu("Combo");
            {
                menu.NewCheckbox("q", "Q");
                menu.NewCheckbox("w", "W");
                menu.NewCheckbox("e", "E");
                menu.NewCheckbox("r", "R");
                menu.NewCheckbox("r.noprediction", "Don't use prediction for R (faster cast)", true, true);
                menu.NewSlider("r.minenemies", "Min enemies R", 1, 1, 5, true);
            }

            menu = MenuManager.AddSubMenu("Harass");
            {
                menu.NewCheckbox("q", "Q");
                menu.NewCheckbox("w", "W");
                menu.NewCheckbox("e", "E");
            }

            menu = MenuManager.AddSubMenu("Lane Clear");
            {
                menu.NewCheckbox("q", "Q");
                menu.NewCheckbox("w", "W");
                menu.NewCheckbox("e", "E");
            }

            menu = MenuManager.AddSubMenu("Jungle Clear");
            {
                menu.NewCheckbox("q", "Q");
                menu.NewCheckbox("w", "W");
                menu.NewCheckbox("e", "E");
            }

            menu = MenuManager.AddSubMenu("Flee");
            {
                menu.NewCheckbox("q", "Q");
                menu.NewCheckbox("w", "W");
                menu.NewCheckbox("e", "E");
            }

            menu = MenuManager.AddSubMenu("Misc");
            {
                menu.NewCheckbox("q.keepalive", "Keep Q Alive");
                menu.NewSlider("q1/q2.delay", "Q1/Q2 Cancel Animation Delay", 250, 0, 450);
                menu.NewSlider("q3.delay", "Q3 Cancel Animation Delay", 380, 300, 500);
                menu.NewCheckbox("ks", "KS", true, true);
                menu.NewCheckbox("ks.r.noprediction", "[KS] Don't use prediction for R (faster cast)");
                menu.NewCheckbox("interrupter", "Interrupter", true, true);
                menu.NewCheckbox("gapcloser", "Stun on enemy gapcloser");
                menu.NewKeybind("burst", "Burst/FlashBurst", false, KeyBind.BindTypes.HoldActive, 'J');
            }
        }

        public override void CreateVariables()
        {
            new SkinManager(8);
            
            R.ConeAngleDegrees = 45;
            R.AllowedCollisionCount = int.MaxValue;
            E.AllowedCollisionCount = int.MaxValue;
            //Q.AllowedCollisionCount = int.MaxValue;

            var slot = Player.GetSpellSlotFromName("summonerflash");

            if (slot != SpellSlot.Unknown)
                flash = new Spell.Skillshot(slot, 425, SkillShotType.Linear);

            damageManager = new DamageManager
            (
                new List<Bases.Damage>()
                {
                    new Bases.Damage
                    (
                        Q, DamageType.Physical, new float[] { 0, 10, 30, 50, 70, 90 },

                        new List<Scale>()
                        {
                            new Scale(new float[] { 0, 0.4f, 0.45f, 0.5f, 0.55f, 0.6f }, ScalingTypes.AD)
                        }
                    ),

                    new Bases.Damage
                    (
                        W, DamageType.Physical, new float[] { 0, 50, 80, 110, 140, 170 },

                        new List<Scale>()
                        {
                            new Scale(new float[] { 0, 1, 1, 1, 1, 1 }, ScalingTypes.ADBonus)
                        }
                    ),

                    new Bases.Damage
                    (
                        R, DamageType.Physical, new float[] { 0, 80, 120, 160 },

                        new List<Scale>()
                        {
                            new Scale(new float[] { 0, 0.6f, 0.6f, 0.6f }, ScalingTypes.ADBonus)
                        }
                    )
                }
            );
        }

        public override void PermaActive()
        {
            base.PermaActive();

            Target = TargetSelector.GetTarget(W.Range + E.Range - 30, DamageType.Physical);

            if (W.Range == 250 && Player.HasBuff("RivenFengShuiEngine")) W.Range += 65;
            else if (!Player.HasBuff("RivenFengShuiEngine") && W.Range == 315) W.Range -= 65;

            if (misc.IsActive("burst"))
            {
                Target = TargetSelector.GetTarget((flash != null && flash.IsReady() ? flash.Range : 0) + W.Range + E.Range - 70, DamageType.Physical);
                Burst(Target);
            }

            if (Target == null) ResetCombo();

            if (!Player.HasBuff("recall") && misc.IsActive("q.keepalive") && Q.IsReady() && Game.Time - lastq > 3.55f - ((Game.Ping + 100) / 1000) && Game.Time - lastq < 3.8f - ((Game.Ping + 100) / 1000))
                Q.Cast(Vectors.CorrectSpellRange(Game.CursorPos, Q.Range));
            //Vectors.CorrectSpellRange(Game.CursorPos, Q.Range)

        }

        public override void Drawing_OnDraw(EventArgs args)
        {
            if (Player.IsDead || draw.IsActive("disable")) return;

            if (draw.IsActive("flash+w"))
                Circle.Draw(flash.IsReady() && W.IsReady() ? Color.Blue : Color.Red, flash.Range + W.Range + E.Range, Player.Position);

            if (Target != null && Target.IsValidTarget() && draw.IsActive("burst") && Q.IsReady() && W.IsReady() && E.IsReady() && R.IsReady())
            {
                //<Settings>
                var targetpos = Target.Position.WorldToScreen();
                targetpos = new Vector2(targetpos.X - 30, targetpos.Y - 150);

                var color = System.Drawing.Color.Yellow;
                var message = "Burst possible ";
                var size = 10;
                //</Settings>
                
                if (Player.IsInRange(Target, Q.Range + W.Range))
                {
                    Drawing.DrawText(targetpos, color, message + "(Q+W range)", size);
                }

                else if (Player.IsInRange(Target, E.Range + W.Range))
                {
                    Drawing.DrawText(targetpos, color, message + "(E+W range)", size);
                }

                else if (flash != null && flash.IsReady())
                {
                    if (Player.IsInRange(Target, flash.Range + W.Range))
                    {
                        Drawing.DrawText(targetpos, color, message + "(Flash+W range)", size);
                    }

                    else if (Player.IsInRange(Target, flash.Range + E.Range + W.Range))
                    {
                        Drawing.DrawText(targetpos, color, message + "(Flash+E+W range)", size);
                    }
                }
            }

            if (draw.IsActive("e"))
                Circle.Draw(E.IsReady() ? Color.Blue : Color.Red, E.Range, Player.Position);

            if (draw.IsActive("w"))
                Circle.Draw(W.IsReady() ? Color.Blue : Color.Red, W.Range, Player.Position);

            if (draw.IsActive("r"))
                Circle.Draw(R.IsReady() ? Color.Blue : Color.Red, R.Range, Player.Position);

            return;
        }
        
        public override void KS()
        {
            if (!misc.IsActive("ks") || !EntityManager.Heroes.Enemies.Any(it => R.IsInRange(it))) return;

            if (W.IsReady())
            {
                var bye = EntityManager.Heroes.Enemies.FirstOrDefault(it => it.IsValidTarget(W.Range) && damageManager.SpellDamage(it, SpellSlot.W) >= it.Health);

                if (bye != null) W.Cast();

                else if (Q.IsReady())
                {
                    bye = EntityManager.Heroes.Enemies.FirstOrDefault(it => it.IsValidTarget(W.Range) && damageManager.SpellDamage(it, SpellSlot.W) + damageManager.SpellDamage(it, SpellSlot.Q) >= it.Health);
                    if (bye != null)
                    {
                        EloBuddy.Player.IssueOrder(GameObjectOrder.MoveTo, bye);
                        Core.DelayAction(() => Q.Cast(Vectors.CorrectSpellRange(bye.ServerPosition, Q.Range)), 100);
                        Core.DelayAction(() => W.Cast(), 200);
                    }
                }

                else if (E.IsReady())
                {
                    bye = EntityManager.Heroes.Enemies.FirstOrDefault(it => it.IsValidTarget(W.Range + E.Range) && damageManager.SpellDamage(it, SpellSlot.W) >= it.Health);
                    if (bye != null)
                    {
                        E.Cast(bye);
                        Core.DelayAction(() => W.Cast(), 200);
                    }
                }
            }

            if (R.IsReady() && r2Actived)
            {
                var bye = EntityManager.Heroes.Enemies.FirstOrDefault(it => it.IsValidTarget(R.Range) && TargetSelector.GetPriority(it) >= 3 && GetRDamage(it) >= it.Health);
                if (bye != null)
                {
                    if (misc.IsActive("ks.r.noprediction")) R.Cast(Target.ServerPosition);
                    else R.HitChanceCast(Target, HitChance.Low);
                }
            }
        }
        
        public override void Flee()
        {
            if (W.IsReady() && flee.IsActive("w") && EntityManager.Heroes.Enemies.Any(it => it.IsValidTarget(W.Range))) W.Cast();

            if (E.IsReady() && flee.IsActive("e")) E.Cast(Vectors.CorrectSpellRange(Game.CursorPos, E.Range));

            if (Q.IsReady() && flee.IsActive("q")) Q.Cast(Vectors.CorrectSpellRange(Game.CursorPos, Q.Range));
            //Vectors.CorrectSpellRange(Game.CursorPos, Q.Range)

            return;
        }

        public override void Combo()
        {
            if (Target == null || !Target.IsValidTarget()) return;

            if (E.IsReady() && W.IsReady() && combo.IsActive("e") && combo.IsActive("w"))
            {
                if (Player.IsInRange(Target, E.Range + W.Range - 60))
                {
                    _order = new List<ComboSpell>() { ComboSpell.E, ComboSpell.W };
                    PCombo(Target);
                }
            }
            else
            {
                if (E.IsReady() && combo.IsActive("e")) E.Cast(Vectors.CorrectSpellRange(Target.ServerPosition, E.Range));

                if (W.IsReady() && combo.IsActive("w") && W.IsInRange(Target)) W.Cast();
            }

            if (R.IsReady() && combo.IsActive("r"))
            {
                if (!r2Actived && combo.Value("r.minenemies") >= Player.CountEnemiesInRange(600) && Player.IsInRange(Target, E.Range)) R1.Cast();

                else if (r2Actived && R.IsInRange(Target) && GetRDamage(Target) >= Target.Health)
                {
                    if (combo.IsActive("r.noprediction")) R.Cast(Target.ServerPosition);
                    else R.HitChanceCast(Target, HitChance.Low);
                }
            }

            return;
        }

        public override void Harass()
        {
            if (Target == null || !Target.IsValidTarget()) return;

            if (E.IsReady() && W.IsReady() && harass.IsActive("e") && harass.IsActive("w"))
            {
                if (Player.IsInRange(Target, E.Range + W.Range - 60))
                {
                    _order = new List<ComboSpell>() { ComboSpell.E, ComboSpell.W };
                    PCombo(Target);
                }
            }
            else
            {
                if (E.IsReady() && harass.IsActive("e")) E.Cast(Vectors.CorrectSpellRange(Target.ServerPosition, E.Range));

                if (W.IsReady() && harass.IsActive("w") && W.IsInRange(Target)) W.Cast();
            }

            return;
        }

        public override void LaneClear()
        {
            var minions = EntityManager.MinionsAndMonsters.GetLaneMinions(EntityManager.UnitTeam.Enemy, Player.Position, E.Range).Where(it => it.Health >= 100).OrderBy(it => it.Health);

            if (!minions.Any()) return;

            if (E.IsReady() && laneclear.IsActive("e") && !minions.Any(it => Player.IsInAutoAttackRange(it) && Player.GetAutoAttackDamage(it) >= it.Health)) E.Cast(minions.First());

            if (W.IsReady() && laneclear.IsActive("w") && minions.Count(it => W.IsInRange(it)) >= 3) W.Cast();

            return;
        }

        public override void JungleClear()
        {
            var monsters = EntityManager.MinionsAndMonsters.GetJungleMonsters(Player.Position, E.Range).Where(it => it.Health >= 200);

            if (!monsters.Any()) return;

            if (E.IsReady() && jungleclear.IsActive("e")) E.Cast(monsters.First());

            if (W.IsReady() && jungleclear.IsActive("w") && monsters.Count(it => W.IsInRange(it)) == monsters.Count()) W.Cast();

            return;
        }

        public override void Obj_AI_Base_OnPlayAnimation(Obj_AI_Base sender, GameObjectPlayAnimationEventArgs args)
        {
            if (!sender.IsMe) return;

            switch (args.Animation)
            {
                case "Spell1a":
                    qstate = 2;
                    lastq = Game.Time;

                    if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo) || Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass) || Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.JungleClear))
                    {
                        Core.DelayAction(() => CancelAnimation(), misc.Value("q1/q2.delay") - Game.Ping);
                    }

                    break;

                case "Spell1b":
                    qstate = 3;
                    lastq = Game.Time;

                    if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo) || Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass) || Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.JungleClear))
                    {
                        Core.DelayAction(() => CancelAnimation(), misc.Value("q1/q2.delay") - Game.Ping);
                    }

                    break;

                case "Spell1c":
                    qstate = 1;

                    if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo) || Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass) || Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.JungleClear))
                    {
                        Core.DelayAction(() => CancelAnimation(), misc.Value("q3.delay") - Game.Ping);
                    }

                    break;

                case "Spell2":
                    if ((hydra.IsReady() || tiamat.IsReady()) && EntityManager.Heroes.Enemies.Any(it => it.IsValidTarget(hydra.Range)) || EntityManager.MinionsAndMonsters.GetJungleMonsters(Player.Position, hydra.Range).Any())
                    {
                        if (hydra.IsReady()) hydra.Cast();
                        if (tiamat.IsReady()) tiamat.Cast();
                    }
                    EloBuddy.Player.DoEmote(Emote.Dance);
                    Orbwalker.ResetAutoAttack();

                    break;

                case "Spell3":
                    if ((Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo) || Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Flee)) && youmouu.IsReady()) youmouu.Cast();

                    if (EntityManager.Heroes.Enemies.Any(it => it.IsValidTarget(hydra.Range)) || EntityManager.MinionsAndMonsters.GetJungleMonsters(Player.Position, hydra.Range).Any())
                    {
                        if (hydra.IsReady()) hydra.Cast();
                        if (tiamat.IsReady()) tiamat.Cast();
                    }

                    Orbwalker.ResetAutoAttack();
                    break;

                case "Spell4a":
                    rstate = 2;
                    break;

                case "Spell4b":
                    rstate = 1;
                    EloBuddy.Player.DoEmote(Emote.Dance);
                    break;
            }
        }

        public override void Orbwalker_OnUnkillableMinion(Obj_AI_Base target, Orbwalker.UnkillableMinionArgs args)
        {
            if (Player.IsDead || !Q.IsReady() || !target.IsValidTarget(Q.Range) || damageManager.SpellDamage(target, SpellSlot.Q) < target.Health) return;

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear) || Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LastHit))
            {
                EloBuddy.Player.IssueOrder(GameObjectOrder.MoveTo, target);
                Core.DelayAction(() => Q.Cast(Vectors.CorrectSpellRange(target.Position, Q.Range)), 100);
                //Vectors.CorrectSpellRange(target.Position, Q.Range)
            }
        }

        public override void Gapcloser_OnGapcloser(AIHeroClient sender, Gapcloser.GapcloserEventArgs e)
        {
            if (Player.IsDead || !sender.IsEnemy || !sender.IsValidTarget(W.Range) || !W.IsReady() || !misc.IsActive("gapcloser")) return;

            W.Cast();
        }

        public override void Orbwalker_OnPostAttack(AttackableUnit target, EventArgs args)
        {
            if (Player.IsDead || !target.IsEnemy) return;

            var pos = Vectors.CorrectSpellRange(target.Position, Q.Range);

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
            {
                if (Q.IsReady() && combo.IsActive("q")) Q.Cast(pos);
            }

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass))
            {
                if (Q.IsReady() && harass.IsActive("q")) Q.Cast(pos);
            }

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.JungleClear))
            {
                if (Q.IsReady() && jungleclear.IsActive("q") && Player.GetAutoAttackDamage((Obj_AI_Base)target) < target.Health) Q.Cast(pos);
            }

            if (_order != default(List<ComboSpell>) && _i != _order.Count() && _order[_i] == ComboSpell.AA) aaFinished = true;

            return;
        }

        public override void Interrupter_OnInterruptableSpell(Obj_AI_Base sender, Interrupter.InterruptableSpellEventArgs e)
        {
            if (Player.IsDead || !sender.IsEnemy || !sender.IsValidTarget() || !misc.IsActive("interrupter")) return;

            if (W.IsReady() && W.IsInRange(sender)) W.Cast();
            if (qstate == 3 && Q.IsReady() && Q.IsInRange(sender)) Q.Cast(sender);

            return;
        }
        
        //------------------------------------|| Methods ||--------------------------------------

        public enum ComboSpell
        {
            None = 0, Q = 1, W = 2, E = 3, R = 4, Flash = 5, Hydras = 6, AA = 7
        };

        private void Burst(Obj_AI_Base target)
        {
            if (target == null || !target.IsValidTarget(flash.Range + E.Range + W.Range)) return;

            if (_order != default(List<ComboSpell>)) { PCombo(target); return; }

            if (Player.IsInRange(target, Q.Range + W.Range))
            {
                //Chat.Print("Q.Range + W.Range");
                _order = new List<ComboSpell>() { ComboSpell.Q, ComboSpell.R, ComboSpell.Hydras, ComboSpell.E, ComboSpell.W, ComboSpell.AA, ComboSpell.R };
                PCombo(target);
            }

            else if (Player.IsInRange(target, E.Range + W.Range))
            {
                //Chat.Print("E.Range + W.Range");
                _order = new List<ComboSpell>() { ComboSpell.E, ComboSpell.W, ComboSpell.Hydras, ComboSpell.Q, ComboSpell.R, ComboSpell.AA, ComboSpell.R };
                PCombo(target);
            }

            else if (Player.IsInRange(target, flash.Range + W.Range))
            {
                //Chat.Print("Flash.Range + W.Range");
                _order = new List<ComboSpell>() { ComboSpell.Flash, ComboSpell.W, ComboSpell.Hydras, ComboSpell.Q, ComboSpell.R, ComboSpell.AA, ComboSpell.R };
                PCombo(target);
            }

            else if (Player.IsInRange(target, flash.Range + E.Range + W.Range))
            {
                //Chat.Print("Flash.Range + E.Range + W.Range");
                _order = new List<ComboSpell>() { ComboSpell.E, ComboSpell.Flash, ComboSpell.W, ComboSpell.Hydras, ComboSpell.Q, ComboSpell.R, ComboSpell.AA, ComboSpell.R };
                PCombo(target);
            }
        }

        private void PCombo(Obj_AI_Base target)
        {
            if (_i == _order.Count()) { ResetCombo(); return; }

            switch (_order[_i])
            {
                case ComboSpell.Q:
                    if (Q.IsReady() && Player.IsInRange(target, 200))
                    {
                        if (Q.Cast(Vectors.CorrectSpellRange(target.ServerPosition, 200))) { /*Chat.Print("q");*/ NextStep(); return; }
                    }
                    else EloBuddy.Player.IssueOrder(GameObjectOrder.MoveTo, target);

                    break;

                case ComboSpell.W:
                    if (W.IsReady() && W.IsInRange(target))
                    {
                        if (W.Cast()) { /*Chat.Print("w");*/ NextStep(); return; }
                    }
                    else EloBuddy.Player.IssueOrder(GameObjectOrder.MoveTo, target);

                    break;

                case ComboSpell.E:
                    if (E.IsReady())
                    {
                        if (E.Cast(Vectors.CorrectSpellRange(target.ServerPosition, E.Range))) { /*Chat.Print("e");*/ NextStep(); return; }
                    }
                    else EloBuddy.Player.IssueOrder(GameObjectOrder.MoveTo, target);

                    break;

                case ComboSpell.R:
                    if (R.IsReady())
                    {
                        if (r2Actived && R.IsInRange(target) && R.HitChanceCast(target, HitChance.Low)) { /*Chat.Print("r2");*/ rstate = 1; NextStep(); return; }
                        else if (!r2Actived && R1.Cast()) { /*Chat.Print("r1");*/ rstate = 2; NextStep(); return; }

                        if (!R.IsInRange(target)) EloBuddy.Player.IssueOrder(GameObjectOrder.MoveTo, target);
                    }

                    break;

                case ComboSpell.Flash:
                    //Chat.Print(Player.IsInRange(target, flash.Range + W.Range));

                    if (flash != null && flash.IsReady() && Player.IsInRange(target, flash.Range + W.Range))
                    {
                        if (flash.Cast(Vectors.CorrectSpellRange(target.ServerPosition, flash.Range))) { /*Chat.Print("flash");*/ NextStep(); return; }
                    }
                    else EloBuddy.Player.IssueOrder(GameObjectOrder.MoveTo, target);

                    break;

                case ComboSpell.Hydras:

                    if (!tiamat.IsInRange(target)) EloBuddy.Player.IssueOrder(GameObjectOrder.MoveTo, target);

                    else
                    {
                        if (hydra.IsReady() && hydra.IsInRange(target))
                        {
                            if (hydra.Cast()) { /*Chat.Print("hydra");*/ NextStep(); return; }
                        }

                        if (tiamat.IsReady() && tiamat.IsInRange(target))
                        {
                            if (tiamat.Cast()) { /*Chat.Print("tiamat");*/ NextStep(); return; }
                        }
                    }

                    break;

                case ComboSpell.AA:
                    if (aaFinished) { /*Chat.Print("AA");*/ NextStep(); return; }

                    if (Player.IsInAutoAttackRange(target)) EloBuddy.Player.IssueOrder(GameObjectOrder.AttackTo, target);

                    else EloBuddy.Player.IssueOrder(GameObjectOrder.MoveTo, target);

                    break;

                default:
                    break;
            }
        }

        private void NextStep()
        {
            _i++;
        }

        private void ResetCombo()
        {
            _i = 0;
            _order = default(List<ComboSpell>);
            aaFinished = false;
        }

        private void CancelAnimation()
        {
            EloBuddy.Player.DoEmote(Emote.Dance);

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo) || Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass))
                EloBuddy.Player.IssueOrder(GameObjectOrder.MoveTo, Target);

            else if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.JungleClear))
                EloBuddy.Player.IssueOrder(GameObjectOrder.MoveTo, EntityManager.MinionsAndMonsters.CombinedAttackable.FirstOrDefault());

            Orbwalker.ResetAutoAttack();
        }

        private float GetRDamage(Obj_AI_Base target)
        {
            if (!R.IsReady() || target == null || !target.IsValidTarget(R.Range)) return 0;

            var LostLifeP = ((target.MaxHealth - target.Health) / target.MaxHealth * 100);
            LostLifeP = LostLifeP > 74.5f ? 74.5f : LostLifeP;

            return Player.CalculateDamageOnUnit(target, DamageType.Physical, damageManager.GetSpellBaseDamage(SpellSlot.R) + (damageManager.GetSpellBonusDamage(SpellSlot.R) * LostLifeP * 0.0267f));
        }
    }
}