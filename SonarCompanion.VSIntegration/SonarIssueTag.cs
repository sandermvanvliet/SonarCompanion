using Microsoft.VisualStudio.Text.Editor;

namespace Rabobank.SonarCompanion_VSIntegration
{
    public class SonarIssueTag : IGlyphTag
    {
        public string Message { get; private set; }

        public SonarIssueTag(string message)
        {
            Message = message;
        }
    }
}