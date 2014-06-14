using System;
using System.Windows;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;

namespace Rabobank.SonarCompanion_VSIntegration.Margin
{
    public class SimpleScrollBar : IVerticalScrollBar
    {
        private readonly IVerticalScrollBar _realScrollBar;
        private readonly IWpfTextViewMargin _realScrollBarMargin;
        private readonly ScrollMapWrapper _scrollMap = new ScrollMapWrapper();
        private readonly IScrollMapFactoryService _scrollMapFactory;
        private readonly IWpfTextView _textView;
        private double _trackSpanBottom;
        private double _trackSpanTop;
        private bool _useElidedCoordinates;

        public SimpleScrollBar(IWpfTextViewHost host, IWpfTextViewMargin containerMargin, FrameworkElement container,
            IScrollMapFactoryService scrollMapFactoryService)
        {
            _textView = host.TextView;

            _realScrollBarMargin =
                containerMargin.GetTextViewMargin(PredefinedMarginNames.VerticalScrollBar) as IWpfTextViewMargin;
            if (_realScrollBarMargin != null)
            {
                _realScrollBar = _realScrollBarMargin as IVerticalScrollBar;
                if (_realScrollBar != null)
                {
                    _realScrollBarMargin.VisualElement.IsVisibleChanged += OnScrollBarIsVisibleChanged;
                    _realScrollBar.TrackSpanChanged += OnScrollBarTrackSpanChanged;
                }
            }
            ResetTrackSpan();

            _scrollMapFactory = scrollMapFactoryService;
            _useElidedCoordinates = false;
            ResetScrollMap();

            _scrollMap.MappingChanged += delegate { RaiseTrackChangedEvent(); };

            container.SizeChanged += OnContainerSizeChanged;
        }

        /// <summary>
        ///     If true, map to the view's scrollbar; else map to the scrollMap.
        /// </summary>
        public bool UseElidedCoordinates
        {
            get { return _useElidedCoordinates; }
            set
            {
                if (value != _useElidedCoordinates)
                {
                    _useElidedCoordinates = value;
                    ResetScrollMap();
                }
            }
        }

        private bool UseRealScrollBarTrackSpan
        {
            get
            {
                try
                {
                    return (_realScrollBar != null) && (_realScrollBarMargin != null) &&
                           (_realScrollBarMargin.VisualElement.Visibility == Visibility.Visible);
                }
                catch
                {
                    return false;
                }
            }
        }

        private void ResetScrollMap()
        {
            if (_useElidedCoordinates && UseRealScrollBarTrackSpan)
            {
                _scrollMap.ScrollMap = _realScrollBar.Map;
            }
            else
            {
                _scrollMap.ScrollMap = _scrollMapFactory.Create(_textView, !_useElidedCoordinates);
            }
        }

        private void ResetTrackSpan()
        {
            if (UseRealScrollBarTrackSpan)
            {
                _trackSpanTop = _realScrollBar.TrackSpanTop;
                _trackSpanBottom = _realScrollBar.TrackSpanBottom;
            }
            else
            {
                _trackSpanTop = 0.0;
                _trackSpanBottom = _textView.ViewportHeight;
            }

            //Ensure that the length of the track span is never 0.
            _trackSpanBottom = Math.Max(_trackSpanTop + 1.0, _trackSpanBottom);
        }

        private void RaiseTrackChangedEvent()
        {
            EventHandler handler = TrackSpanChanged;
            if (handler != null)
                handler(this, new EventArgs());
        }

        private void OnScrollBarIsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            ResetTrackSpan();

            if (_useElidedCoordinates)
                ResetScrollMap(); //This will indirectly cause RaiseTrackChangedEvent to be called.
            else
                RaiseTrackChangedEvent();
        }

        private void OnContainerSizeChanged(object sender, EventArgs e)
        {
            if (!UseRealScrollBarTrackSpan)
            {
                ResetTrackSpan();
                RaiseTrackChangedEvent();
            }
        }

        private void OnScrollBarTrackSpanChanged(object sender, EventArgs e)
        {
            if (UseRealScrollBarTrackSpan)
            {
                ResetTrackSpan();
                RaiseTrackChangedEvent();
            }
        }

        #region IVerticalScrollBar Members

        public IScrollMap Map
        {
            get { return _scrollMap; }
        }

        public double GetYCoordinateOfBufferPosition(SnapshotPoint bufferPosition)
        {
            double scrollMapPosition = _scrollMap.GetCoordinateAtBufferPosition(bufferPosition);
            return GetYCoordinateOfScrollMapPosition(scrollMapPosition);
        }

        public double GetYCoordinateOfScrollMapPosition(double scrollMapPosition)
        {
            double minimum = _scrollMap.Start;
            double maximum = _scrollMap.End;
            double height = maximum - minimum;

            return TrackSpanTop + ((scrollMapPosition - minimum)*TrackSpanHeight)/(height + _scrollMap.ThumbSize);
        }

        public SnapshotPoint GetBufferPositionOfYCoordinate(double y)
        {
            double minimum = _scrollMap.Start;
            double maximum = _scrollMap.End;
            double height = maximum - minimum;

            double scrollCoordinate = minimum + (y - TrackSpanTop)*(height + _scrollMap.ThumbSize)/TrackSpanHeight;

            return _scrollMap.GetBufferPositionAtCoordinate(scrollCoordinate);
        }

        public double TrackSpanTop
        {
            get { return _trackSpanTop; }
        }

        public double TrackSpanBottom
        {
            get { return _trackSpanBottom; }
        }

        public double TrackSpanHeight
        {
            get { return _trackSpanBottom - _trackSpanTop; }
        }

        public double ThumbHeight
        {
            get
            {
                double minimum = _scrollMap.Start;
                double maximum = _scrollMap.End;
                double height = maximum - minimum;

                return _scrollMap.ThumbSize/(height + _scrollMap.ThumbSize)*TrackSpanHeight;
            }
        }

        public event EventHandler TrackSpanChanged;

        #endregion

        private class ScrollMapWrapper : IScrollMap
        {
            private IScrollMap _scrollMap;

            public IScrollMap ScrollMap
            {
                get { return _scrollMap; }
                set
                {
                    if (_scrollMap != null)
                    {
                        _scrollMap.MappingChanged -= OnMappingChanged;
                    }

                    _scrollMap = value;

                    _scrollMap.MappingChanged += OnMappingChanged;

                    OnMappingChanged(this, new EventArgs());
                }
            }

            public double GetCoordinateAtBufferPosition(SnapshotPoint bufferPosition)
            {
                return _scrollMap.GetCoordinateAtBufferPosition(bufferPosition);
            }

            public bool AreElisionsExpanded
            {
                get { return _scrollMap.AreElisionsExpanded; }
            }

            public SnapshotPoint GetBufferPositionAtCoordinate(double coordinate)
            {
                return _scrollMap.GetBufferPositionAtCoordinate(coordinate);
            }

            public double Start
            {
                get { return _scrollMap.Start; }
            }

            public double End
            {
                get { return _scrollMap.End; }
            }

            public double ThumbSize
            {
                get { return _scrollMap.ThumbSize; }
            }

            public ITextView TextView
            {
                get { return _scrollMap.TextView; }
            }

            public double GetFractionAtBufferPosition(SnapshotPoint bufferPosition)
            {
                return _scrollMap.GetFractionAtBufferPosition(bufferPosition);
            }

            public SnapshotPoint GetBufferPositionAtFraction(double fraction)
            {
                return _scrollMap.GetBufferPositionAtFraction(fraction);
            }

            public event EventHandler MappingChanged;

            private void OnMappingChanged(object sender, EventArgs e)
            {
                EventHandler handler = MappingChanged;
                if (handler != null)
                    handler(sender, e);
            }
        }
    }
}