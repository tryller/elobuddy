using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Menu.Values;

namespace tryCassiopeia
{
    public class Extensions
    {
        public static float GetDamage(SpellSlot spell, Obj_AI_Base target)
        {
            float ap = ObjectManager.Player.FlatMagicDamageMod + ObjectManager.Player.BaseAbilityDamage;
            if (spell == SpellSlot.E)
            {
                if (!Program.E.IsReady())
                    return 0;

                return ObjectManager.Player.CalculateDamageOnUnit(target, DamageType.Magical, 55f + 25f * (Program.E.Level - 1) + 55 / 100 * ap);
            }

            return 0;
        }
    }
}
