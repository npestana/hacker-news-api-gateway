using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Api.Infrastructure.Repository.Dto
{
    public class HackerNewsStoryDto
    {
        [JsonPropertyName("by")]
        public string Author { get; set; }

        [JsonPropertyName("kids")]
        public List<int> Comments { get; set; }
        
        [JsonPropertyName("score")]
        public int Score { get; set; }
        
        [JsonPropertyName("time")]
        public long Time { get; set; }

        [JsonPropertyName("title")]
        public string Title { get; set; }

        [JsonPropertyName("url")]
        public string Url { get; set; }
    }
}
