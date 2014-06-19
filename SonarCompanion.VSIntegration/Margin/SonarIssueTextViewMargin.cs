using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Formatting;
using SonarCompanion.API;

namespace Rabobank.SonarCompanion_VSIntegration.Margin
{
    public class SonarIssueTextViewMargin : Canvas, IWpfTextViewMargin
    {
        public static readonly Brush GlyphColor = Brushes.Purple;
        private const double GlyphHeight = 2.0;
        private readonly IEnumerable<SonarIssue> _sonarIssues;
        private SimpleScrollBar _scrollBar;
        private readonly EventHandler _scrollBarOnTrackSpanChanged;

        public SonarIssueTextViewMargin(IEnumerable<SonarIssue> sonarIssues, IWpfTextViewHost textViewHost,
            IWpfTextViewMargin containerMargin, IScrollMapFactoryService scrollMapFactoryService)
        {
            _sonarIssues = sonarIssues;

            Width = textViewHost
                .TextView
                .Options
                .GetOptionValue(DefaultTextViewHostOptions.ChangeTrackingMarginWidthOptionId);

            _scrollBar = new SimpleScrollBar(textViewHost, containerMargin, this, scrollMapFactoryService);
            
            _scrollBarOnTrackSpanChanged = (sender, args) => DrawMargins();
            _scrollBar.TrackSpanChanged += _scrollBarOnTrackSpanChanged;
        }

        public double MarginElementOffset
        {
            get { return 1.0; }
        }

        public void Dispose()
        {
            _scrollBar.TrackSpanChanged -= _scrollBarOnTrackSpanChanged;
            _scrollBar = null;
        }

        public ITextViewMargin GetTextViewMargin(string marginName)
        {
            return string.Equals(marginName, "SonarIssueMargin", StringComparison.InvariantCultureIgnoreCase)
                ? this
                : null;
        }

        public double MarginSize
        {
            get { return ActualHeight; }
        }

        public bool Enabled { get { return true; } }

        public FrameworkElement VisualElement
        {
            get { return this; }
        }

        private void DrawMargins()
        {
            if (!Dispatcher.CheckAccess())
            {
                Dispatcher.Invoke(DrawMargins);
                return;
            }

            Children.Clear();

            foreach (var sonarIssue in _sonarIssues)
            {
                var line = _scrollBar
                    .Map
                    .TextView
                    .TextSnapshot
                    .Lines
                    .Single(l => l.LineNumber == sonarIssue.Line - 1);

                var top = _scrollBar.GetYCoordinateOfBufferPosition(line.Start);

                var rect = new Rectangle
                {
                    Height = GlyphHeight,
                    Width = Width - MarginElementOffset,
                    Focusable = false,
                    IsHitTestVisible = true,
                    Fill = GlyphColor,
                    ToolTip = sonarIssue.Message,
                    Tag = sonarIssue,
                    Cursor = Cursors.Hand
                };
                
                SetLeft(rect, MarginElementOffset);
                SetTop(rect, top);

                Debug.WriteLine("DrawMargins: top: " + top);

                rect.MouseUp += HandleMouseUp;

                Children.Add(rect);
            }
        }

        private void HandleMouseUp(object sender, MouseButtonEventArgs e)
        {
            var rect = sender as Rectangle;

            if (rect == null)
            {
                return;
            }

            var sonarIssue = rect.Tag as SonarIssue;
            if (sonarIssue == null)
            {
                return;
            }

            ITextView textView = _scrollBar.Map.TextView;

            ITextSnapshotLine snapshotLine =
                textView.TextSnapshot.Lines.SingleOrDefault(line => line.LineNumber == sonarIssue.Line - 1);

            if (snapshotLine != null)
            {
                ITextViewLine textViewLine = textView.GetTextViewLineContainingBufferPosition(snapshotLine.Start);

                if (textViewLine != null)
                {
                    _scrollBar.Map.TextView.Caret.MoveTo(textViewLine, snapshotLine.Start.Position);

                    _scrollBar.Map.TextView.Caret.EnsureVisible();
                }
            }
        }
    }
}