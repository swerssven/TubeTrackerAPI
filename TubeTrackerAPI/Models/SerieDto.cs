﻿using TubeTrackerAPI.TubeTrackerEntities;

namespace TubeTrackerAPI.Models
{
    public class SerieDto
    {
        public int SerieId { get; set; }

        public int SerieApiId { get; set; }

        public string TitleEn { get; set; }

        public string TitleEs { get; set; }

        public string DescriptionEn { get; set; }

        public string DescriptionEs { get; set; }

        public string Actors { get; set; }

        public string Creators { get; set; }

        public string GenresEn { get; set; }

        public string GenresEs { get; set; }

        public DateTime? PremiereDate { get; set; }

        public string Poster { get; set; }

        public string Backdrop { get; set; }

        public bool? watched { get; set; }

        public bool? favorite { get; set; }

        public DateTime? DateAddedFavorite { get; set; }
    }
}
