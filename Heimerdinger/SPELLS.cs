using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Enumerations;

namespace Farofakids_Heimerdinger
{
    internal class SPELLS
    {
        public static Spell.Skillshot Q, W, W1, E, E1, E2, E3;
        public static Spell.Active R;


        public static void Initialize()
        {
            Q = new Spell.Skillshot(SpellSlot.Q, 325, SkillShotType.Linear, 250, 2000, 90);
            W = new Spell.Skillshot(SpellSlot.W, 1100, SkillShotType.Linear, 500, 3000, 40);
            W1 = new Spell.Skillshot(SpellSlot.W, 1100, SkillShotType.Linear, 500, 3000, 40);
            E = new Spell.Skillshot(SpellSlot.E, 925, SkillShotType.Circular, 500, 1200, 120);
            E1 = new Spell.Skillshot(SpellSlot.E, 925, SkillShotType.Circular, 500, 1200, 120);
            E2 = new Spell.Skillshot(SpellSlot.E, 1125, SkillShotType.Circular, 250 + E1.CastDelay, 1200, 120);
            E3 = new Spell.Skillshot(SpellSlot.E, 1325, SkillShotType.Circular, 300 + E2.CastDelay, 1200, 120);
            R = new Spell.Active(SpellSlot.R, 100);
        }

        public static float GetComboDamage(Obj_AI_Base enemy)
        {
            var aa = Player.Instance.GetAutoAttackDamage(enemy, true);

            var damage = aa;

            if (E.IsReady() && MENUS.UseECombo)
                damage += Player.Instance.GetSpellDamage(enemy, SpellSlot.E);

            if (E.IsReady() && MENUS.UseECombo)
                damage += Player.Instance.GetSpellDamage(enemy, SpellSlot.E);

            if (W.IsReady() && MENUS.UseWCombo)
                damage += Player.Instance.GetSpellDamage(enemy, SpellSlot.W);

            if (W.IsReady() && MENUS.UseWCombo && MENUS.UseRCombo)
                damage += Player.Instance.GetSpellDamage(enemy, SpellSlot.W) * 2.2f;

            return (float)damage;
        }

        public static float GetWDamage(Obj_AI_Base enemy)
        {
            var target = TargetSelector.GetTarget(W.Range + 200, DamageType.Magical);
            if (target == null) return (float)0;
            double damage = 0d;

            if (W.IsReady())
                damage += Player.Instance.GetSpellDamage(target, SpellSlot.W);

            return (float)damage * 2;
        }

        public static float GetW1Damage(Obj_AI_Base enemy)
        {
            var target = TargetSelector.GetTarget(W.Range + 200, DamageType.Magical);
            if (target == null) return (float)0;
            double damage = 0d;

            if (W1.IsReady() && R.IsReady())
                damage += Player.Instance.GetSpellDamage(target, SpellSlot.W);

            return (float)damage * 2;
        }

        public static float GetEDamage(Obj_AI_Base enemy)
        {
            var target = TargetSelector.GetTarget(W.Range + 200, DamageType.Magical);
            if (target == null) return (float)0;
            double damage = 0d;

            if (E.IsReady())
                damage += Player.Instance.GetSpellDamage(target, SpellSlot.E);

            return (float)damage * 2;
        }

    }
}
