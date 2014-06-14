using System.ComponentModel.Composition;
using System.Linq;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Utilities;
using Rabobank.SonarCompanion_VSIntegration.Margin;

namespace Rabobank.SonarCompanion_VSIntegration
{
    [Export(typeof (IWpfTextViewMarginProvider))]
    [Name("SonarIssueMargin")]
    [ContentType("text")]
    [TextViewRole(PredefinedTextViewRoles.PrimaryDocument)]
    [MarginContainer(PredefinedMarginNames.VerticalScrollBarContainer)]
    [Order(After = PredefinedMarginNames.VerticalScrollBar)]
    public class SonarIssueTextViewMarginProvider : IWpfTextViewMarginProvider
    {
        private readonly ISonarIssuesService _sonarIssuesService;
        private readonly IScrollMapFactoryService _scrollMapFactoryService;

        [ImportingConstructor]
        public SonarIssueTextViewMarginProvider(ISonarIssuesService sonarIssuesService, IScrollMapFactoryService scrollMapFactoryService)
        {
            _sonarIssuesService = sonarIssuesService;
            _scrollMapFactoryService = scrollMapFactoryService;
        }

        public IWpfTextViewMargin CreateMargin(IWpfTextViewHost wpfTextViewHost, IWpfTextViewMargin marginContainer)
        {
            var fileName = GetFileNameFromTextBuffer(wpfTextViewHost.TextView.TextBuffer);

            var issues = _sonarIssuesService.GetIssuesForFile(fileName);

            return new SonarIssueTextViewMargin(issues, wpfTextViewHost, marginContainer, _scrollMapFactoryService);
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
    }
}