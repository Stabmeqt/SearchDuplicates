using System;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Media.Animation;
using System.Windows;

namespace SearchDuplicates
{
    public class GifImage : Image
    {
        private GifBitmapDecoder _decoder;
        private Int32Animation _animation;

        #region FrameIndex

        public static readonly DependencyProperty FrameIndexProperty = 
            DependencyProperty.Register
            ("FrameIndex", typeof(int), typeof(GifImage), new UIPropertyMetadata
                (0, ChangingFrameIndex));

        public int FrameIndex
        {
            get { return (int)GetValue(FrameIndexProperty); }
            set { SetValue(FrameIndexProperty, value); }
        }

        private static void ChangingFrameIndex
            (DependencyObject obj, DependencyPropertyChangedEventArgs ev)
        {
            var gifImage = obj as GifImage;
            if (gifImage != null) gifImage.ChangeFrameIndex((int)ev.NewValue);
        }

        private void ChangeFrameIndex(int index)
        {
            Source = _decoder.Frames[index];
        }

        #endregion

        #region GifSource
        
        public static readonly DependencyProperty GifSourceProperty = 
            DependencyProperty.Register("GifSource", typeof(System.Windows.Media.ImageSource), typeof
            (GifImage), new UIPropertyMetadata
                (null, GifSourceChanged));
        public System.Windows.Media.ImageSource GifSource
        {
            get { return (System.Windows.Media.ImageSource)GetValue(GifSourceProperty); }
            set { SetValue(GifSourceProperty, value); }
        }

        private static void GifSourceChanged(DependencyObject obj, DependencyPropertyChangedEventArgs ev)
        {
            var gifImage = obj as GifImage;
            if (gifImage != null) gifImage.GifSourceChanged(ev.NewValue.ToString());
        }

        private void GifSourceChanged(string newSource)
        {
            if (_animation != null)
                BeginAnimation(FrameIndexProperty, null);

            _decoder = new GifBitmapDecoder
                (new Uri(newSource), BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.Default);
            Source = _decoder.Frames[0];

            int count = _decoder.Frames.Count;
            _animation = new Int32Animation(0, count - 1,
                                            new Duration(new TimeSpan(0, 0, 0, count/10,
// ReSharper disable PossibleLossOfFraction
                                                                      (int) ((count/10.0 - count/10)*1000))))
// ReSharper restore PossibleLossOfFraction
                             {RepeatBehavior = RepeatBehavior.Forever};
            BeginAnimation(FrameIndexProperty, _animation);
        }
        #endregion
    }
}
