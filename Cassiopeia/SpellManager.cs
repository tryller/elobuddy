using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Enumerations;
using System;

namespace LightCassiopeia
{
    public static class SpellManager
    {
        public static Spell.Skillshot Q { get; private set; }
        public static Spell.Skillshot W { get; private set; }
        public static Spell.Targeted E { get; private set; }
        public static Spell.Skillshot R { get; private set; }

        static SpellManager()
        {
            Q = new Spell.Skillshot(SpellSlot.Q, 850, SkillShotType.Circular, 750, 500, 150);
            W = new Spell.Skillshot(SpellSlot.W, 850, SkillShotType.Circular, 250, 500, 150);
            E = new Spell.Targeted(SpellSlot.E, 700);
            R = new Spell.Skillshot(SpellSlot.R, 825, SkillShotType.Cone, 250, null, (int)(80 * Math.PI / 180));
        }

        public static void Initialize()
        {
        }
    }
}