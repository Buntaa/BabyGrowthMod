using MCM.Abstractions.Base.Global;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
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
    }
}
