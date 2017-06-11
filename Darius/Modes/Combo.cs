using EloBuddy;
using EloBuddy.SDK;
using SharpDX;
using EloBuddy.SDK.Enumerations;
using System.Collections.Generic;

// Using the config like this makes your life easier, trust me
using Settings = AddonTemplate.Config.Modes.Combo;

namespace AddonTemplate.Modes
{
    public sealed class Combo : ModeBase
    {
        public static AIHeroClient Player { get { return ObjectManager.Player; } }
        public override bool ShouldBeExecuted()
        {
            // Only execute this mode when the orbwalker is on combo mode
            return Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo);
        }

        public static void CastR(AIHeroClient target)
        {
            if (!SpellManager.R.IsReady())
                return;

            if (target.HasBuffOfType(BuffType.Invulnerability)
                                && target.HasBuffOfType(BuffType.SpellShield)
                                && target.HasBuff("kindredrnodeathbuff") //Kindred Ult
                                && target.HasBuff("BlitzcrankManaBarrierCD") //Blitz Passive
                                && target.HasBuff("ManaBarrier") //Blitz Passive
                                && target.HasBuff("FioraW") //Fiora W
                                && target.HasBuff("JudicatorIntervention") //Kayle R
                                && target.HasBuff("UndyingRage") //Trynd R
                                && target.HasBuff("BardRStasis") //Bard R
                                && target.HasBuff("ChronoShift") //Zilean R
                                )
                return;


            if (target.IsValidTarget(SpellManager.R.Range) && !target.IsZombie)
            {
                int PassiveCounter = target.GetBuffCount("dariushemo") <= 0 ? 0 : target.GetBuffCount("dariushemo");
                if (Damages.RDamage(target, PassiveCounter) >= target.Health + Damages.PassiveDmg(target, 1))
                {
                    SpellManager.R.Cast(target);
                }
            }
        }

        public override void Execute()
        {
            var target = TargetSelector.GetTarget(E.Range, DamageType.Physical);
            Items.UseItems(target);

            if (Settings.UseE)
            {
                var eprediction = E.GetPrediction(target);
                if (eprediction.HitChance >= HitChance.High)
                {
                    if (E.IsReady() && target != null)
                    {
                        E.Cast(eprediction.CastPosition);
                    }
                }
            }

            if (Settings.UseW)
            {
                if (W.IsReady() && target != null)
                {
                    W.Cast();
                }
            }

            if (Settings.UseQ)
            {
                if (Q.IsReady() && target != null)
                {
                    Q.Cast();
                }
            }

            if (Settings.UseR && R.IsReady())
            {
                CastR(target);
            }
        }
    }
}
                
            

        
    



