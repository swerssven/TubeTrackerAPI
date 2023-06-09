﻿using TubeTrackerAPI.Models;

namespace TubeTrackerAPI.Models.Response
{
    public class MovieResponse
    {
        public int Page { get; set; }
        public List<ExternalMovie> Results { get; set; }
        public int total_pages { get; set; }
        public int total_results { get; set; }
    }
}
