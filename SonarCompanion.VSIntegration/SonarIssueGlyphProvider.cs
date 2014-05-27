using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Formatting;

namespace Rabobank.SonarCompanion_VSIntegration
{
    public class SonarIssueGlyphProvider : IGlyphFactory
    {
        private const double _glyphSize = 16;

        public UIElement GenerateGlyph(IWpfTextViewLine line, IGlyphTag tag)
        {
            var sonarTag = tag as SonarIssueTag;

            // Ensure we can draw a glyph for this marker. 
            if (sonarTag == null)
            {
                return null;
            }

            var glyph = new Rectangle
            {
                Fill = Brushes.Red,
                Height = 8,
                Width = _glyphSize,
                ToolTip = sonarTag.Message
            };

            return glyph;
        }
    }
}