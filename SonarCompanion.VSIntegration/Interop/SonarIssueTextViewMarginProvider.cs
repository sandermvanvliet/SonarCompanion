using System.Collections.Generic;
using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.TextManager.Interop;
using Microsoft.VisualStudio.Utilities;
using SonarCompanion.API;
using SonarCompanion_VSIntegration.Factories;
using SonarCompanion_VSIntegration.Margin;
using SonarCompanion_VSIntegration.Services;

namespace SonarCompanion_VSIntegration.Interop
{
    [Export(typeof (IWpfTextViewMarginProvider))]
    [Name("SonarIssueMargin")]
    [ContentType("text")]
    [TextViewRole(PredefinedTextViewRoles.PrimaryDocument)]
    [MarginContainer(PredefinedMarginNames.VerticalScrollBarContainer)]
    [Order(After = PredefinedMarginNames.VerticalScrollBar)]
    public class SonarIssueTextViewMarginProvider : IWpfTextViewMarginProvider
    {
        private readonly IScrollMapFactoryService _scrollMapFactoryService;
        private readonly ISonarIssuesService _sonarIssuesService;

        [ImportingConstructor]
        public SonarIssueTextViewMarginProvider(SonarIssuesServiceFactory sonarIssuesServiceFactory,
            IScrollMapFactoryService scrollMapFactoryService)
        {
            _sonarIssuesService = sonarIssuesServiceFactory.Create();
            _scrollMapFactoryService = scrollMapFactoryService;
        }

        public IWpfTextViewMargin CreateMargin(IWpfTextViewHost wpfTextViewHost, IWpfTextViewMargin marginContainer)
        {
            string fileName = GetFileNameFromTextBuffer(wpfTextViewHost.TextView.TextBuffer);

            IEnumerable<SonarIssue> issues = _sonarIssuesService.GetIssuesForFile(fileName);

            return new SonarIssueTextViewMargin(issues, wpfTextViewHost, marginContainer, _scrollMapFactoryService);
        }

        private string GetFileNameFromTextBuffer(ITextBuffer buffer)
        {
            IPersistFileFormat persistFileFormat;
            buffer.Properties.TryGetProperty(typeof (IVsTextBuffer), out persistFileFormat);

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