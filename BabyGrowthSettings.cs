﻿using MCM.Abstractions.Attributes;
using MCM.Abstractions.Attributes.v2;
using MCM.Abstractions.Base.Global;

namespace BabyGrowthMod
{
   public class BabyGrowthSettings : AttributeGlobalSettings<BabyGrowthSettings>
    {
        [SettingPropertyFloatingInteger("Growth Rate", 1f, 40f, "0.0", Order = 0, RequireRestart = false, HintText = "Adjusts the rate at which all children grow.")]
        [SettingPropertyGroup("Baby Growth Settings")]
        public float NewGrowthRate { get; set; } = 15f;

        [SettingPropertyBool("Affect Only Players Children", Order = 1, RequireRestart = false, HintText = "When enabled, the mod will only affect the growth rate of the player's children.")]
        [SettingPropertyGroup("Baby Growth Settings")]
        public bool AffectOnlyPlayersChildren { get; set; } = false;

        [SettingPropertyBool("Super Speed", Order = 2, RequireRestart = false, HintText ="When enabled, the mod will make your children grow SUPER fast. Good for people who wanna do quick runs or for fun.")]
        [SettingPropertyGroup("Baby Growth Settings")]

        public bool SuperSpeed { get; set; } = false;

        [SettingPropertyBool("Affect Everyone", Order = 3, RequireRestart = false, HintText = "WARNING: This setting applied with super speed will most likely kill off everyone really fast. USE AT YOUR OWN RISK. [Setting suggested by nexus user: ProlificGamer60]")]
        [SettingPropertyGroup("Baby Growth Settings")]

        public bool AffectEveryone { get; set; } = false;


        public override string Id => "BabyGrowthMod_v1.0.7";

        public override string DisplayName => "Baby Growth Mod";

        public override string FolderName => "BabyGrowthMod";

        public override string FormatType => "xml";
    }
}
