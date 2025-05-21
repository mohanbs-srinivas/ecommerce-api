using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ecommerce_api.Models
{
    public class UserDetails
    {
        public class AccessDetails
        {
            public string access_token { get; set; }
            public string token_type { get; set; }
            public string expires_in { get; set; }
            public string refresh_token { get; set; }
            public string scope { get; set; }
        }

        public class Profile
        {
            [JsonProperty("displayName")]
            public string DisplayName { get; set; }

            [JsonProperty("publicAlias")]
            public Guid PublicAlias { get; set; }

            [JsonProperty("emailAddress")]
            public string EmailAddress { get; set; }

            [JsonProperty("id")]
            public string Id { get; set; }

            public bool isAdmin { get; set; }

            public CoreAttributes coreAttributes { get; set; }
        }
        public class CoreAttributes
        {
            public Avatar Avatar { get; set; }
        }
        public class Avatar
        {
            public Value value { get; set; }
            public DateTime timeStamp { get; set; }
        }
        public class Value
        {
            public string value { get; set; }
        }
        public class Identity
        {
            public static AccessDetails accessDetails { get; set; }
            public static Profile profile { get; set; }
        }
        public class Details
        {
            public string token { get; set; }
            public string refreshtoken { get; set; }
            public string email { get; set; }
            public string profileId { get; set; }
            public string displayName { get; set; }
            public string profilePic { get; set; }
            public string location { get; set; }
            public string geoLocationToken { get; set; }
        }
    }
}
