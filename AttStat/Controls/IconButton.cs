using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace AttStat.Controls
{
    class IconButton : Button
    {
        public static readonly DependencyProperty CornerRadiusProperty
            = DependencyProperty.Register("CornerRadius",typeof(CornerRadius), typeof(IconButton));
        public static readonly DependencyProperty ImageSourceProperty
            = DependencyProperty.Register("ImageSource", typeof(ImageSource), typeof(IconButton));
        public static readonly DependencyProperty ImageWidthProperty
            = DependencyProperty.Register("ImageWidth", typeof(double), typeof(IconButton));
        public static readonly DependencyProperty MouseOverColorProperty
            = DependencyProperty.Register("MouseOverColor", typeof(Brush), typeof(IconButton));
        public static readonly DependencyProperty MouseDownColorProperty
            = DependencyProperty.Register("MouseDownColor", typeof(Brush), typeof(IconButton));
        public static readonly DependencyProperty TextProperty
            = DependencyProperty.Register("Text", typeof(string), typeof(IconButton));
        public CornerRadius CornerRadius
        {
            get => (CornerRadius)GetValue(CornerRadiusProperty);
            set =>SetValue(CornerRadiusProperty, value);
        }
        public ImageSource ImageSource
        {
            get => (ImageSource)GetValue(ImageSourceProperty);
            set => SetValue(ImageSourceProperty, value);
        }
        public double ImageWidth
        {
            get => (double)GetValue(ImageWidthProperty);
            set => SetValue(ImageWidthProperty, value);
        }
        public Brush MouseOverColor
        {
            get => (Brush)GetValue(MouseOverColorProperty);
            set => SetValue(MouseOverColorProperty, value);
        }
        public Brush MouseDownColor
        {
            get => (Brush)GetValue(MouseDownColorProperty);
            set => SetValue(MouseDownColorProperty, value);
        }
        public string Text
        {
            get => (string)GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }
    }
}
