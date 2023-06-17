using System.ComponentModel;
using System.Text.Json.Serialization;

namespace MarketingCodingAssignment.Models
{
    public class Film
    {
        private string id;

        public string Id
        {
            get => id; set => id = value;
        }
        [JsonPropertyName("title")]
        public string? Title
        {
            get; set;
        }


        public string? budget { get; set; }
        public string? genres { get; set; }
        public string? original_language { get; set; }
        public string? overview { get; set; }
        public string? popularity { get; set; }
        public string? production_companies { get; set; }
        public string? release_date { get; set; }
        public string? revenue { get; set; }
        public string? runtime { get; set; }
        public string? tagline { get; set; }
        public string? vote_average { get; set; }
        public string? vote_count { get; set; }

    }
}
