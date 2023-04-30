using TubeTrackerAPI.TubeTrackerEntities;

namespace TubeTrackerAPI.Models
{
    public class SeasonsEpisodesListDto
    {
        public int TotalNumEpisodes { get; set; }

        public int TotalNumSeasons { get; set; }

        public List<SeasonsDto> SeasonsList { get; set; }
    }

    public class SeasonsDto
    {
        public int NumSeason { get; set; }

        public List<SeasonsEpisodeDto> EpisodesList { get; set; }
    }

    public partial class SeasonsEpisodeDto
    {
        public int SeasonsEpisodesId { get; set; }

        public int SerieId { get; set; }

        public int NumSeason { get; set; }

        public int NumEpisode { get; set; }

        public int? EpisodeDuration { get; set; }

        public string TitleEpisodeEn { get; set; }

        public string TitleEpisodeEs { get; set; }

        public DateTime? PremiereDate { get; set; }

        public bool? Watched { get; set; }
    }
}
