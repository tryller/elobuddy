using System;
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

namespace tryCombos.Champions
{
    static class Yasuo
    {
        public static Spell.Skillshot SteelTempest;
        public static Spell.Targeted E, Q3;
        public static Spell.Skillshot W;
        public static Spell.Active R;

        public static AIHeroClient target;

        public static bool dashing;
        public static bool IsDashing = false;

        public static float HealthPercent { get { return Program.myHero.Health / Program.myHero.MaxHealth * 100; } }

        public static Menu Menu;
        public static string G_charname = Program.myHero.ChampionName;

        public static void Init()
        {
            SteelTempest = new Spell.Skillshot(SpellSlot.Q, 475, EloBuddy.SDK.Enumerations.SkillShotType.Linear, (int)250f, (int)8700f, (int)15f);
            Q3 = new Spell.Targeted(SpellSlot.Q, 1000);
            W = new Spell.Skillshot(SpellSlot.W, 400, EloBuddy.SDK.Enumerations.SkillShotType.Cone);
            E = new Spell.Targeted(SpellSlot.E, 475);
            R = new Spell.Active(SpellSlot.R, 1200);

            Menu = MainMenu.AddMenu(G_charname, G_charname.ToLower());

            Menu.AddGroupLabel("Welcome to " + G_charname);
            Menu.AddLabel("Addon that contains combos for all champions.");
            Menu.AddLabel("This adodn don't have anny config.");
            Menu.AddLabel("But it have full combo for LOL Champions.");
            Menu.AddSeparator();
            Menu.AddLabel("By Tryller");

            Game.OnTick += Game_OnTick;
        }

        static void Game_OnTick(EventArgs args)
        {
            switch (Orbwalker.ActiveModesFlags)
            {
                case Orbwalker.ActiveModes.Combo:
                    OnCombo();
                    break;
            }
        }

        public static bool isDashing()
        {
            return IsDashing;
        }

        public static bool Q3READY(AIHeroClient unit)
        {
            return Program.myHero.HasBuff("YasuoQ3W");
        }

        public static Obj_AI_Base GetEnemy(float range, GameObjectType type)
        {
            return ObjectManager.Get<Obj_AI_Base>().Where(a => a.IsEnemy
            && a.Type == type
            && a.Distance(ObjectManager.Player) <= range
            && !a.IsDead
            && !a.IsInvulnerable
            && a.IsValidTarget(range)).FirstOrDefault();
        }

        public static Vector2 GetDashingEnd(Obj_AI_Base unit)
        {
            if (!unit.IsValidTarget())
            {
                return Vector2.Zero;
            }

            var baseX = ObjectManager.Player.Position.X;
            var baseY = ObjectManager.Player.Position.Y;
            var targetX = unit.Position.X;
            var targetY = unit.Position.Y;

            var vector = new Vector2(targetX - baseX, targetY - baseY);
            var sqrt = Math.Sqrt(vector.X * vector.X + vector.Y * vector.Y);

            var x = (float)(baseX + (E.Range * (vector.X / sqrt)));
            var y = (float)(baseY + (E.Range * (vector.Y / sqrt)));

            return new Vector2(x, y);
        }

        public static bool CanCastE(Obj_AI_Base unit)
        {
            return !unit.HasBuff("YasuoDashWrapper");
        }

        public static bool IsKnockedUp(AIHeroClient unit)
        {
            return unit.HasBuffOfType(BuffType.Knockup) || unit.HasBuffOfType(BuffType.Knockback);
        }

        public static bool UnderEnemyTower(Vector2 pos)
        {
            return ObjectManager.Get<Obj_AI_Turret>().Any(i => !i.IsDead && i.Distance(pos) <= 900 + Program.myHero.BoundingRadius);
        }

        public static Vector2 PosAfterE(Obj_AI_Base unit)
        {
            if (!unit.IsValidTarget())
            {
                return Vector2.Zero;
            }

            var baseX = Program.myHero.Position.X;
            var baseY = Program.myHero.Position.Y;
            var targetX = unit.Position.X;
            var targetY = unit.Position.Y;

            var vector = new Vector2(targetX - baseX, targetY - baseY);
            var sqrt = Math.Sqrt(vector.X * vector.X + vector.Y * vector.Y);

            var x = (float)(baseX + (E.Range * (vector.X / sqrt)));
            var y = (float)(baseY + (E.Range * (vector.Y / sqrt)));

            return new Vector2(x, y);
        }

        public static void UseItems(Obj_AI_Base unit)
        {
            if (Item.HasItem((int)ItemId.Blade_of_the_Ruined_King, Program.myHero) && Item.CanUseItem((int)ItemId.Blade_of_the_Ruined_King)
               && HealthPercent <= 35)
            {
                Item.UseItem((int)ItemId.Blade_of_the_Ruined_King, unit);
            }
            if (Item.HasItem((int)ItemId.Bilgewater_Cutlass, Program.myHero) && Item.CanUseItem((int)ItemId.Bilgewater_Cutlass)
               && unit.IsValidTarget())
            {
                Item.UseItem((int)ItemId.Bilgewater_Cutlass, unit);
            }
            if (Item.HasItem((int)ItemId.Youmuus_Ghostblade, Program.myHero) && Item.CanUseItem((int)ItemId.Youmuus_Ghostblade)
               && Program.myHero.Distance(unit.Position) <= Program.myHero.GetAutoAttackRange())
            {
                Item.UseItem((int)ItemId.Youmuus_Ghostblade);
            }
            if (Item.HasItem((int)ItemId.Ravenous_Hydra_Melee_Only, Program.myHero) && Item.CanUseItem((int)ItemId.Ravenous_Hydra_Melee_Only)
               && Program.myHero.Distance(unit.Position) <= 400)
            {
                Item.UseItem((int)ItemId.Ravenous_Hydra_Melee_Only);
            }
            if (Item.HasItem((int)ItemId.Tiamat_Melee_Only, Program.myHero) && Item.CanUseItem((int)ItemId.Tiamat_Melee_Only)
               && Program.myHero.Distance(unit.Position) <= 400)
            {
                Item.UseItem((int)ItemId.Tiamat_Melee_Only);
            }
            if (Item.HasItem((int)ItemId.Randuins_Omen, Program.myHero) && Item.CanUseItem((int)ItemId.Randuins_Omen)
               && Program.myHero.Distance(unit.Position) <= 400)
            {
                Item.UseItem((int)ItemId.Randuins_Omen);
            }
        }

        public static Vector2 getNextPos(AIHeroClient unit)
        {
            Vector2 dashPos = unit.Position.To2D();
            if (unit.IsMoving && unit.Path.Count() != 0)
            {
                Vector2 tpos = unit.Position.To2D();
                Vector2 path = unit.Path[0].To2D() - tpos;
                path.Normalize();
                dashPos = tpos + (path * 100);
            }
            return dashPos;
        }

        public static void putWallBehind(AIHeroClient unit)
        {
            if (!W.IsReady() || !E.IsReady() || unit.IsMelee)
                return;
            Vector2 dashPos = getNextPos(unit);
            //PredictionResult po = Prediction.Position.PredictUnitPosition(, (int)0.5f);

            float dist = Program.myHero.Distance(unit.Position);
            if (!unit.IsMoving || Program.myHero.Distance(dashPos) <= dist + 40)
                if (dist < 330 && dist > 100 && W.IsReady())
                {
                    W.Cast(unit.Position);
                }
        }

        public static bool CanCastDelayR(AIHeroClient unit)
        {
            var buff = unit.Buffs.FirstOrDefault(i => i.Type == BuffType.Knockback || i.Type == BuffType.Knockup);
            return buff != null && buff.EndTime - Game.Time <= (buff.EndTime - buff.StartTime) / 3;
        }

        public static bool AlliesNearTarget(Obj_AI_Base unit, float range)
        {
            return EntityManager.Heroes.Allies.Where(tar => tar.Distance(unit) < range).Any(tar => tar != null);
        }

        public static void OnCombo()
        {
            target = TargetSelector.GetTarget(E.Range, DamageType.Magical);
            if (target == null || !target.IsValid || target.IsZombie)
                return;

            if (target != null && target.IsValidTarget(E.Range)
                && !target.HasBuff("sionpassivezombie")               //sion Passive
                && !target.HasBuff("KarthusDeathDefiedBuff")          //karthus passive
                && !target.HasBuff("kogmawicathiansurprise")          //kog'maw passive
                && !target.HasBuff("zyrapqueenofthorns")              //zyra passive
                && !target.HasBuff("ChronoShift")                     //zilean R
                && !target.HasBuff("yorickrazombie"))                 //yorick R
            {
                if (E.IsReady())
                {
                    AIHeroClient enemy = (AIHeroClient)GetEnemy(1300, GameObjectType.AIHeroClient);
                    if (enemy != null
                        && Extensions.Distance(GetDashingEnd(enemy), enemy) <= ObjectManager.Player.GetAutoAttackRange()
                        //wont e unless in AA range after
                        && enemy.Distance(ObjectManager.Player) <= E.Range
                        && enemy.Distance(ObjectManager.Player) >= ObjectManager.Player.GetAutoAttackRange())
                    {
                        E.Cast(enemy);
                        dashing = true;
                        if (SteelTempest.IsReady())
                            SteelTempest.Cast(enemy.Position);
                    }
                    else if (enemy != null
                             && enemy.Distance(ObjectManager.Player) > Player.Instance.GetAutoAttackRange(enemy)) //use minions to get to champ
                    {
                        var minion =
                            ObjectManager.Get<Obj_AI_Minion>()
                                .OrderByDescending(m => m.Distance(ObjectManager.Player))
                                .FirstOrDefault(
                                    m =>
                                        m.IsValidTarget(E.Range)
                                        && (m.Distance(target) < ObjectManager.Player.Distance(target))
                                        && ObjectManager.Player.IsFacing(m) && CanCastE(m) && (!UnderEnemyTower(PosAfterE(m))));

                        if (minion != null && (PosAfterE(minion).Distance(enemy) < Player.Instance.Distance(enemy)))
                        {
                            Console.Write("E Cast minions");
                            E.Cast(minion);
                        }
                    }

                }


                if (SteelTempest.IsReady())
                {
                    PredictionResult QPred = SteelTempest.GetPrediction(target);
                    if (!isDashing() && SteelTempest.Range == 1000)
                    {
                        SteelTempest.Cast(QPred.CastPosition);
                        Core.DelayAction(Orbwalker.ResetAutoAttack, 250);
                    }
                    else if (SteelTempest.Range == 1000 && Q3READY(Program.myHero) && isDashing() && Program.myHero.Distance(target) <= 250 * 250)
                    {
                        SteelTempest.Cast(QPred.CastPosition); Core.DelayAction(Orbwalker.ResetAutoAttack, 250);
                    }
                    else if (!Q3READY(Program.myHero) && SteelTempest.Range == 475)
                    {
                        SteelTempest.Cast(QPred.CastPosition); Core.DelayAction(Orbwalker.ResetAutoAttack, 250);
                    }
                }

                if (Orbwalker.CanAutoAttack)
                {
                    AIHeroClient enemy =
                        (AIHeroClient)GetEnemy(ObjectManager.Player.GetAutoAttackRange(), GameObjectType.AIHeroClient);

                    if (enemy != null)
                        Orbwalker.ForcedTarget = enemy;
                }
                dashing = false;



                if (target != null)
                {
                    //ITEMS
                    if (target.IsValidTarget())
                    {
                        UseItems(target);
                    }

                    //EW
                    if (E.IsReady())
                    {
                        //Wind wall
                        putWallBehind(target);
                        /*
                        if (Program.DogeMenu["smartW"].Cast<CheckBox>().CurrentValue && Program.wallCasted &&
                            Program.myHero.Distance(TsTarget.Position) < 300)
                        {
                            Program.eBehindWall(TsTarget);
                        }
                        */

                        //ignite
                    }

                    //R
                    if (R.IsReady())
                    {
                        List<AIHeroClient> enemies = EntityManager.Heroes.Enemies;
                        foreach (AIHeroClient enemy in enemies)
                        {
                            if (Program.myHero.Distance(enemy) <= 1200)
                            {
                                var enemiesKnockedUp =
                                    ObjectManager.Get<AIHeroClient>()
                                        .Where(x => x.IsValidTarget(R.Range))
                                        .Where(x => x.HasBuffOfType(BuffType.Knockup));

                                var enemiesKnocked = enemiesKnockedUp as IList<AIHeroClient> ??
                                                     enemiesKnockedUp.ToList();
                                if (enemy.IsValidTarget(R.Range) &&
                                    CanCastDelayR(enemy) &&
                                    enemiesKnocked.Count() >= 1)
                                {
                                    R.Cast();
                                }
                            }
                            if (enemy.IsValidTarget(R.Range))
                            {
                                if (IsKnockedUp(enemy) && CanCastDelayR(enemy) &&
                                    (enemy.Health / enemy.MaxHealth * 100 <= 35))
                                {
                                    R.Cast();
                                }
                                else if (IsKnockedUp(enemy)  &&
                                         CanCastDelayR(enemy) &&
                                         enemy.HealthPercent >= 50)
                                {
                                    if (AlliesNearTarget(target, 600))
                                    {
                                        R.Cast();
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}