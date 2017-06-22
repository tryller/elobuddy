using EloBuddy.SDK;

namespace LightCassiopeia.Carry
{
    public abstract class ModeBase
    {
        protected Spell.Skillshot Q
        {
            get { return SpellManager.Q; }
        }

        protected Spell.Skillshot W
        {
            get { return SpellManager.W; }
        }

        protected Spell.Targeted E
        {
            get { return SpellManager.E; }
        }

        protected Spell.Skillshot R
        {
            get { return SpellManager.R; }
        }

        public abstract bool ShouldBeExecuted();

        public abstract void Execute();
    }
}