using System;
using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Tagging;
using Microsoft.VisualStudio.Utilities;

namespace Rabobank.SonarCompanion_VSIntegration
{
    [Export(typeof (ITaggerProvider))]
    [ContentType("code")]
    [TagType(typeof (SonarIssueTag))]
    public class SonarIssueTaggerProvider : ITaggerProvider
    {
        private readonly ISonarIssuesService _sonarIssuesService;

        [ImportingConstructor]
        public SonarIssueTaggerProvider(ISonarIssuesService sonarIssuesService)
        {
            _sonarIssuesService = sonarIssuesService;
        }

        public ITagger<T> CreateTagger<T>(ITextBuffer buffer) where T : ITag
        {
            if (buffer == null)
            {
                throw new ArgumentNullException("buffer");
            }

            return new SonarIssueTagger(_sonarIssuesService) as ITagger<T>;
        }
    }
}