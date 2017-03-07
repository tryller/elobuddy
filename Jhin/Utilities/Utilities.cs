using System.Collections.Generic;
using EloBuddy;
using EloBuddy.SDK;
using Jhin;
using Jhin.Model;
using SharpDX;

namespace Jhin.Utilities
{
    public class BestPositionResult
    {
        public int Hits;
        public List<Obj_AI_Base> ObjectsHit = new List<Obj_AI_Base>();
        public Vector3 Position;
        public Obj_AI_Base Target;

        public BestPositionResult()
        {
            Hits = -1;
        }
    }

    public class BestDamageResult
    {
        public float Damage;
        public List<SpellBase> List = new List<SpellBase>();
        public float Mana;
        public Obj_AI_Base Target;

        public bool IsKillable
        {
            get { return Target != null && Damage >= Target.TotalShieldHealth(); }
        }

        public bool Q
        {
            get { return List.Contains(Program.CurrentChampion.Q); }
        }

        public bool W
        {
            get { return List.Contains(Program.CurrentChampion.W); }
        }

        public bool E
        {
            get { return List.Contains(Program.CurrentChampion.E); }
        }

        public bool R
        {
            get { return List.Contains(Program.CurrentChampion.R); }
        }

        public bool Contains(SpellBase spell)
        {
            return List.Contains(spell);
        }
    }
}