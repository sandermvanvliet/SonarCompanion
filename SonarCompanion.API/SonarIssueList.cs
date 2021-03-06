﻿using System.Collections.Generic;

namespace SonarCompanion.API
{
    public class SonarIssueList
    {
        public bool MaxResultsReached { get; set; }
        public Paging Paging { get; set; }
        public List<SonarIssue> Issues { get; set; }
        public List<SonarComponent> Components { get; set; }
        public List<SonarRule> Rules { get; set; }
        public List<SonarProject> Projects { get; set; }
    }
}