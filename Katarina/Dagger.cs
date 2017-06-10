using System.Collections.Generic;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using SharpDX;

namespace Katarina
{
    class Daggers
    {
        public static IEnumerable<Obj_AI_Base> GetDaggers()
        {
            return ObjectManager.Get<Obj_AI_Base>().Where(a => a.Name == "HiddenMinion" && a.IsValid && a.Health == 100);
        }

        public static Vector3 GetClosestDagger()
        {
            var dagger = GetDaggers();
            if (!dagger.Any() || dagger == null || dagger.Count() <= 0) return new Vector3();
            var t = dagger.Where(p => p.Distance(Player.Instance) >= 125).OrderBy(p => p.Distance(Player.Instance.Position)).FirstOrDefault();
            return t == null ? new Vector3() : t.Position;
        }
    }
}
