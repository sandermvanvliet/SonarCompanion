using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Tagging;

namespace Rabobank.SonarCompanion_VSIntegration
{
    public class SonarIssueTagger : ITagger<SonarIssueTag>
    {
        [ImportingConstructor]
        public SonarIssueTagger(ISonarIssuesService sonarIssuesService)
        {
            _sonarIssuesService = sonarIssuesService;
        }

        private readonly ISonarIssuesService _sonarIssuesService;

        public IEnumerable<ITagSpan<SonarIssueTag>> GetTags(NormalizedSnapshotSpanCollection spans)
        {
            if (!spans.Any())
            {
                return null;
            }

            var fileName = GetFileNameFromTextBuffer(spans.First().Snapshot.TextBuffer);
            if (string.IsNullOrEmpty(fileName))
            {
                return null;
            }

            var issues = _sonarIssuesService.GetIssuesForFile(fileName);

            var retval = new List<TagSpan<SonarIssueTag>>();

            foreach (var span in spans)
            {
                var lineNumber = span.Start.GetContainingLine().LineNumber;

                retval.AddRange(issues.Where(i => i.Line == lineNumber).Select(i => new TagSpan<SonarIssueTag>(span, new SonarIssueTag(i.Message))));
            }

            return retval;
        }

        private SonarIssueTag GetTagFor(string fileName, int lineNumber)
        {
            var sonarIssue = _sonarIssuesService.GetIssueFor(fileName, lineNumber);

            return sonarIssue == null ? null : new SonarIssueTag(sonarIssue.Message);
        }

        private string GetFileNameFromTextBuffer(ITextBuffer buffer)
        {
            IPersistFileFormat persistFileFormat;
            buffer.Properties.TryGetProperty(typeof(Microsoft.VisualStudio.TextManager.Interop.IVsTextBuffer), out persistFileFormat);

            if (persistFileFormat == null)
            {
                return null;
            }

            string fileName;
            uint iii;
            persistFileFormat.GetCurFile(out fileName, out iii);

            return fileName;
        }

        public event EventHandler<SnapshotSpanEventArgs> TagsChanged;
    }
}