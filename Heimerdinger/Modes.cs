using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Events;
using EloBuddy.SDK.Enumerations;
using SharpDX;

namespace Farofakids_Heimerdinger
{
    internal class MODES
    {
        public static void Interrupter_OnInterruptableSpell(Obj_AI_Base sender, Interrupter.InterruptableSpellEventArgs args)
        {
            if (!MENUS.InterruptSpells) return;
            if (SPELLS.E.IsReady() && sender.IsValidTarget(SPELLS.E.Range))
                SPELLS.E.Cast(sender.Position);
        }

        public static void Gapcloser_OnGapcloser(AIHeroClient sender, Gapcloser.GapcloserEventArgs gapcloser)
        {
            if (!MENUS.AntiGap) return;
            if (SPELLS.E.IsReady() && gapcloser.Sender.IsValidTarget(SPELLS.E.Range))
            {
                SPELLS.E.Cast(gapcloser.End);
            }
        }

        public static void Combo()
        {
            var target = TargetSelector.GetTarget(SPELLS.W.Range, DamageType.Magical);
            if (target == null)
                return;

            if (SPELLS.Q.IsReady() && SPELLS.R.IsReady() && MENUS.UseQRCombo &&
                MENUS.UseQCombo && target.IsValidTarget(650) &&
                Player.Instance.Position.CountEnemiesInRange(650) >= MENUS.QRcount)
            {
                SPELLS.R.Cast();
                SPELLS.Q.Cast(Player.Instance.ServerPosition.Extend(target.Position, +300).To3D());
            }
            else
            {
                if (SPELLS.Q.IsReady() && MENUS.UseQCombo && target.IsValidTarget(650) &&

                    Player.Instance.Position.CountEnemiesInRange(650) >= 1)
                {
                    SPELLS.Q.Cast(Player.Instance.Position.Extend(target.Position, +300).To3D());
                }
            }
            if (SPELLS.E3.IsReady() && SPELLS.R.IsReady() && MENUS.UseERCombo &&
                MENUS.UseRCombo &&
                target.Position.CountEnemiesInRange(450 - 250) >=
                MENUS.ERcount)
            {
                CastER(target);
            }
            else
            {
                if (SPELLS.E.IsReady() && MENUS.UseECombo && target.IsValidTarget(SPELLS.E.Range))
                {
                    var ePrediction = SPELLS.E.GetPrediction(target);
                    if (ePrediction.HitChance >= HitChance.High)
                    {
                        SPELLS.E.Cast(ePrediction.CastPosition);
                    }

                }
                if (SPELLS.W.IsReady() && MENUS.UseWRCombo && MENUS.UseRCombo &&
                    SPELLS.R.IsReady() && target.IsValidTarget(SPELLS.W.Range) &&
                    SPELLS.GetComboDamage(target) >= target.Health)
                {
                    var wPrediction = SPELLS.W.GetPrediction(target);
                    SPELLS.R.Cast();

                    if (wPrediction.HitChance >= HitChance.High)
                    {
                        Core.DelayAction(() =>
                        {
                            {
                                SPELLS.W.Cast(wPrediction.CastPosition);
                            }
                        }, SPELLS.W.CastDelay);
                    }

                }
                else
                {
                    if (SPELLS.W.IsReady() && MENUS.UseWCombo && target.IsValidTarget(SPELLS.W.Range))
                    {
                        var wPrediction = SPELLS.W.GetPrediction(target);
                        if (wPrediction.HitChance >= HitChance.High)
                        {
                            SPELLS.W.Cast(wPrediction.CastPosition);
                        }
                    }
                }
            }
        }

        internal static void Harras()
        {
            var target = TargetSelector.GetTarget(SPELLS.W.Range, DamageType.Magical);
            if (target == null || !target.IsValid)
                return;
            if (SPELLS.W.IsReady() && target.IsValidTarget() && MENUS.UseWHarras && Player.Instance.ManaPercent > MENUS.HarassMana)
            {
                var wPrediction = SPELLS.W.GetPrediction(target);
                if (wPrediction.HitChance >= HitChance.High)
                {
                    SPELLS.W.Cast(wPrediction.CastPosition);
                }
            }
        }

        internal static void KS()
        {
            var target = TargetSelector.GetTarget(SPELLS.E.Range + 200, DamageType.Magical);
            if (target == null) return;
            if (target.Health < SPELLS.GetEDamage(target))
            {
                var ePrediction = SPELLS.E.GetPrediction(target);
                if (ePrediction.HitChance >= HitChance.Medium)
                {
                    SPELLS.E.Cast(ePrediction.CastPosition);
                }
                if (ePrediction.HitChance >= HitChance.High)
                {
                    SPELLS.E.Cast(ePrediction.CastPosition);
                }
                return;
            }

            target = TargetSelector.GetTarget(SPELLS.W.Range + 200, DamageType.Magical);
            if (target == null) return;
            if (target.Health < SPELLS.GetWDamage(target))
            {
                var prediction = SPELLS.W.GetPrediction(target);
                if (prediction.HitChance >= HitChance.High && prediction.CollisionObjects.Count(h => h.IsEnemy && !h.IsDead && h is Obj_AI_Minion) < 2)
                {

                    SPELLS.W.Cast(prediction.CastPosition);
                    return;
                }
            }

            target = TargetSelector.GetTarget(SPELLS.W.Range + 200, DamageType.Magical);
            if (target == null) return;
            if (target.Health < SPELLS.GetW1Damage(target) && SPELLS.R.IsReady())
            {
                var prediction = SPELLS.W.GetPrediction(target);
                if (prediction.HitChance >= HitChance.High && prediction.CollisionObjects.Count(h => h.IsEnemy && !h.IsDead && h is Obj_AI_Minion) < 2)
                {
                    SPELLS.R.Cast();
                    SPELLS.W.Cast(prediction.CastPosition);
                    return;
                }
            }
        }

        internal static void AutoHarras()
        {
            var target = TargetSelector.GetTarget(SPELLS.W.Range, DamageType.Magical);
            if (target == null || !target.IsValid)
                return;
            if (SPELLS.W.IsReady() && target.IsValidTarget() && MENUS.AutoHarras && Player.Instance.ManaPercent  > MENUS.HarassMana)
            {
                var wPrediction = SPELLS.W.GetPrediction(target);
                if (wPrediction.HitChance >= HitChance.High)
                {
                    SPELLS.W.Cast(wPrediction.CastPosition);
                }
            }
        }

        private static void CastER(Obj_AI_Base target)
        {
            PredictionResult prediction;

            if (ObjectManager.Player.Distance(target) < SPELLS.E1.Range)
            {
                var oldrange = SPELLS.E1.Range;
                SPELLS.E1.Range = SPELLS.E2.Range;
                prediction = SPELLS.E1.GetPrediction(target);
                SPELLS.E1.Range = oldrange;
            }
            else if (ObjectManager.Player.Distance(target) < SPELLS.E2.Range)
            {
                var oldrange = SPELLS.E2.Range;
                SPELLS.E2.Range = SPELLS.E3.Range;
                prediction = SPELLS.E2.GetPrediction(target);
                SPELLS.E2.Range = oldrange;
            }
            else if (ObjectManager.Player.Distance(target) < SPELLS.E3.Range)
            {
                prediction = SPELLS.E3.GetPrediction(target);
            }
            else
            {
                return;
            }

            if (prediction.HitChance >= HitChance.High)
            {
                if (ObjectManager.Player.ServerPosition.Distance(prediction.CastPosition) <= SPELLS.E1.Range + SPELLS.E1.Width)
                {
                    Vector3 p;
                    if (ObjectManager.Player.ServerPosition.Distance(prediction.CastPosition) > 300)
                    {
                        p = prediction.CastPosition -
                            100 *
                            (prediction.CastPosition.To2D() - ObjectManager.Player.ServerPosition.To2D()).Normalized()
                                .To3D();
                    }
                    else
                    {
                        p = prediction.CastPosition;
                    }
                    SPELLS.R.Cast();
                    SPELLS.E1.Cast(p);
                }
                else if (ObjectManager.Player.ServerPosition.Distance(prediction.CastPosition) <=
                         ((SPELLS.E1.Range + SPELLS.E1.Range) / 2))
                {
                    var p = ObjectManager.Player.ServerPosition.To2D()
                        .Extend(prediction.CastPosition.To2D(), SPELLS.E1.Range - 100);
                    {
                        SPELLS.R.Cast();
                        SPELLS.E1.Cast(p.To3D());
                    }
                }
                else
                {
                    var p = ObjectManager.Player.ServerPosition.To2D() +
                            SPELLS.E1.Range *
                            (prediction.CastPosition.To2D() - ObjectManager.Player.ServerPosition.To2D()).Normalized
                                ();

                    {
                        SPELLS.R.Cast();
                        SPELLS.E1.Cast(p.To3D());
                    }
                }
            }
        }
    }

}

