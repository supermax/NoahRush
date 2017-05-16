using System.Collections.Generic;
using TMS.Common.Serialization.Json;

namespace Osge.Axion.State.JsonData
{
    [JsonDataContract]
    public class DailyInfo
    {
        [JsonDataMember(Name = "reward")]
        public int RewardAmount { get; set; }

        [JsonDataMember(Name = "duration")]
        public int Duration { get; set; }
    }

    [JsonDataContract]
    public class RewardsContainer
    {
        [JsonDataMember(Name = "reward")]
        public List<List<int>> Rewards { get; set; }

        //[JsonDataMember(Name = "daily")]
        //public DailyInfo DailyRewardInfo { get; set; }



        /*public long RewardType(int index)
        {
            return Rewards[index][0];
        }

        public long RewardQuantity(int index)
        {
            return Rewards[index][1];
        }*/
    }

    [JsonDataContract]
    public class Theme
    {
        [JsonDataMember(Name = "bg_image")]
        public string BackgroundImage { get; set; }

        [JsonDataMember(Name = "cat_header")]
        public string CategoryHeader { get; set; }

        [JsonDataMember(Name = "gems_color")]
        public string GemsColor { get; set; }

        [JsonDataMember(Name = "bonus_line")]
        public string BonusLine { get; set; }
    }

    [JsonDataContract]
    public class Package
    {
        [JsonDataMember(Name = "title")]
        public string Name { get; set; }

        [JsonDataMember(Name = "order")]
        public long InnerOrder { get; set; }

        //[JsonDataMember(Name = "priorty")]
        //public long Priorty { get; set; }

        [JsonDataMember(Name = "own")]
        public RewardsContainer OwnRewards { get; set; }

        [JsonDataMember(Name = "alliance")]
        public RewardsContainer AllianceRewards { get; set; }

        //[JsonDataMember(Name = "theme")]
        //public Theme Theme { get; set; }
    }

    [JsonDataContract]
    public class Tier
    {        
        [JsonDataMember(Name = "packages")]
        public Dictionary<string, Package> Packages { get; set; }
    }

    [JsonDataContract]
    public class Category
    {
        [JsonDataMember(Name = "start")]
        public long Start { get; set; }
    
        [JsonDataMember(Name = "end")]
        public long End { get; set; }
    
        [JsonDataMember(Name = "duration")]
        public long Duration { get; set; }
    
        [JsonDataMember(Name = "cooldown")]
        public long Cooldown { get; set; }
    
        [JsonDataMember(Name = "tiers")]
        public Dictionary<string, Tier> Tiers { get; set; }
    }

    //[JsonDataContract]
    //public class VcPackagesData
    //{ 
    //    public Dictionary<string, Category> Categories { get; set; }
    //}
}

