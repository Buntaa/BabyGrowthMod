using MCM.Abstractions.Base.Global;
using System;
using System.Collections.Generic;
using System.Reflection;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.CharacterDevelopment;
using TaleWorlds.CampaignSystem.ComponentInterfaces;
using TaleWorlds.Core;
using TaleWorlds.GauntletUI.Data;
using TaleWorlds.MountAndBlade;

namespace BabyGrowthMod
{
    public class SubModule : MBSubModuleBase
    {
        protected override void OnGameStart(Game game, IGameStarter gameStarterObject)
        {
            base.OnGameStart(game, gameStarterObject);

            if (game.GameType is Campaign)
            {
                CampaignEvents.DailyTickEvent.AddNonSerializedListener(this, OnDailyTick);
            }
        }
        private void OnDailyTick()
        {
            if (GlobalSettings<BabyGrowthSettings>.Instance.AffectOnlyPlayersChildren)
            {
                ApplyGrowthRateToPlayerChildren();
            }

            else
            {
                ApplyGrowthRatesToAllChildren();
            }

            if (GlobalSettings<BabyGrowthSettings>.Instance.AffectEveryone)
            {
                ApplyGrowthRateToEveryone();
            }

            if (GlobalSettings<BabyGrowthSettings>.Instance.SuperSpeed)
            {
                float newGrowthRate = GlobalSettings<BabyGrowthSettings>.Instance.NewGrowthRate;
                foreach(Hero hero in Hero.AllAliveHeroes)
                {
                    if (hero.IsChild && hero.IsKnownToPlayer && hero.Clan == Hero.MainHero.Clan && hero.Age < (float)Campaign.Current.Models.AgeModel.HeroComesOfAge)
                    {
                        hero.SetBirthDay(hero.BirthDay - CampaignTime.Days(newGrowthRate * 100));
                    }
                }
            }
        }
        private void OnDeath()
        {
            if (Hero.MainHero.IsDead)
            {
                throw new NotImplementedException();
            }
        }

        private void ApplyGrowthRateToPlayerChildren()
        {
            float newGrowthRate = GlobalSettings<BabyGrowthSettings>.Instance.NewGrowthRate;
            foreach (Hero hero in Hero.AllAliveHeroes)
            {
                if (hero.IsChild && hero.IsKnownToPlayer && hero.Clan == Hero.MainHero.Clan && hero.Age < (float)Campaign.Current.Models.AgeModel.HeroComesOfAge)
                {
                    hero.SetBirthDay(hero.BirthDay - CampaignTime.Days(newGrowthRate + 1f));
                }
            }

        }
        private void ApplyGrowthRatesToAllChildren()
        {
            float newGrowthRate = GlobalSettings<BabyGrowthSettings>.Instance.NewGrowthRate;
            foreach (Hero hero in Hero.AllAliveHeroes)
            {
                if (hero.IsChild && hero.Age < (float)Campaign.Current.Models.AgeModel.HeroComesOfAge)
                {
                    hero.SetBirthDay(hero.BirthDay - CampaignTime.Days(newGrowthRate - 1f));
                }
            }
        }
        private void ApplyGrowthRateToEveryone()
        {
            float newGrowthRate = GlobalSettings<BabyGrowthSettings>.Instance.NewGrowthRate;
            foreach (Hero hero in Hero.AllAliveHeroes)
            {
                if (hero.IsAlive && hero.Age > 18)
                {
                    hero.SetBirthDay(hero.BirthDay - CampaignTime.Days(newGrowthRate - 1f));
                }
            }
        }
        // simple inheritence system (FOR NOW)
        private void Inheritence()
        {
            foreach(Hero hero in Hero.AllAliveHeroes)
            {
                // if player is male
                if (hero.Clan == Hero.MainHero.Clan && hero.Age > 18 && hero.Father == Hero.MainHero && hero.Father.IsDead)
                {
                    int main_hero_gold = hero.Father.Gold;
                    hero.ChangeHeroGold(main_hero_gold);
                }

                // if player is female 
                if (hero.Clan == Hero.MainHero.Clan && hero.Age > 18 && hero.Mother == Hero.MainHero && hero.Mother.IsDead)
                {
                    int main_hero_gold = hero.Mother.Gold;
                    hero.ChangeHeroGold(main_hero_gold);
                }
            }  
        }
        // IDEA: On main hero death all traits from the main hero get transfered to the next selected hero
        private void InheritTraits()
        {
            Hero mh = Hero.MainHero;

            if (mh.IsDead)
            {
                Hero? nextHero = null; 
                foreach (Hero hero in Hero.AllAliveHeroes)
                {
                    if (hero.IsPlayerCompanion)
                    {
                        nextHero = hero;
                        break;
                    }
                }
                if (nextHero != null)
                {
                    IEnumerable<TraitObject> main_hero_traits = (IEnumerable<TraitObject>)mh.GetHeroTraits();

                    foreach (TraitObject trait_object in main_hero_traits)
                    {
                        Random random = new Random();
                        int random_trait_level = random.Next(1, 20);
                        nextHero.SetTraitLevel(trait_object, random_trait_level);
                    }
                }
            }
        }
        // method for handling SkillObjects and puting them in a list by using reflection 
        private List<SkillObject> GetDefaultSkills()
        {
            List<SkillObject> player_skills = new List<SkillObject>();

            Type type = typeof(SkillObject);
            FieldInfo[] fields = type.GetFields(BindingFlags.Public | BindingFlags.Static);

            foreach (FieldInfo field in fields)
            {
                if (field.FieldType == typeof(SkillObject))
                {
                    SkillObject skill_object = (SkillObject)field.GetValue(null);
                    player_skills.Add(skill_object);
                }
            }
            return player_skills;
        }
        // Same ideas as inheriting traits but with skills. 
        private void InheritSkills()
        {
            Hero mh = Hero.MainHero;
            List<SkillObject> player_skills = GetDefaultSkills();

            if (mh.IsDead)
            {
                Hero? next_hero = null;

                foreach (Hero hero in Hero.AllAliveHeroes)
                {
                    if (hero.IsPlayerCompanion)
                    {
                        next_hero = hero;
                        break;
                    }
                }
                if (next_hero != null)
                {
                    foreach (SkillObject skill_object in player_skills)
                    {
                        int skill_value = mh.GetSkillValue(skill_object);
                        next_hero.AddSkillXp(skill_object, skill_value);
                    }
                }
            }
        }
    }
}
