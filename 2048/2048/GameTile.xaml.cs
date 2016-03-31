﻿using System;

#if NETFX_CORE
using Windows.UI;
using Windows.UI.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
#elif (WINDOWS_PHONE || NETFX_451)
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
#endif

namespace _2048
{
    public sealed partial class GameTile
    {
        private readonly Color[] _backColors =
        {
            Color.FromArgb(255, 159, 192, 255),
            Color.FromArgb(255, 168, 255, 99),
            Color.FromArgb(255, 255, 255, 104),
            Color.FromArgb(255, 255, 221, 140),
            Color.FromArgb(255, 255, 170, 63),
            Color.FromArgb(255, 255, 223, 50),
            Color.FromArgb(255, 206, 255, 86),
            Color.FromArgb(255, 118, 255, 138),
            Color.FromArgb(255, 89, 118, 255),
            Color.FromArgb(255, 205, 96, 255),
            Color.FromArgb(255, 255, 78, 59),
        };

        private readonly Color[] _foreColors =
        {
            Color.FromArgb(0xff, 0x77, 0x6e, 0x65),
            Color.FromArgb(0xff, 0x77, 0x6e, 0x65),
            Color.FromArgb(0xff, 0x77, 0x6e, 0x65),
            Color.FromArgb(0xff, 0x77, 0x6e, 0x65),
            Color.FromArgb(255, 255, 255, 255),
            Color.FromArgb(0xff, 0x77, 0x6e, 0x65),
            Color.FromArgb(0xff, 0x77, 0x6e, 0x65),
            Color.FromArgb(0xff, 0x77, 0x6e, 0x65),
            Color.FromArgb(255, 255, 255, 255),
            Color.FromArgb(255, 255, 255, 255),
            Color.FromArgb(255, 255, 255, 255),
        };

        private int _value;
        public int Value
        {
            get
            {
                return _value;
            }
            set
            {
                _value = value;
                _textBlock.Text = _value > 0 ? _value.ToString() : "";
                TileBorder.Background = new SolidColorBrush(value > 0 ? _backColors[(int)Math.Log(_value, 2) - 1] : Color.FromArgb(0xff, 0xbb, 0xab, 0xb0));
                _textBlock.Foreground = value > 0 ? new SolidColorBrush(_foreColors[(int)Math.Log(_value, 2) - 1]) : _textBlock.Foreground;
            }
        }

        private readonly TextBlock _textBlock;

        private int _x;
        private int _y;

        public GameTile(int x, int y, bool transparentBorder = false)
        {
            InitializeComponent();

            _textBlock = new TextBlock();
            _textBlock.Text = "";
            _textBlock.FontSize = 64;
            _textBlock.FontWeight = FontWeights.Bold;
            _textBlock.TextAlignment = TextAlignment.Center;
            _textBlock.VerticalAlignment = VerticalAlignment.Center;
            TileBorder.Child = _textBlock;

            if (transparentBorder)
            {
                ContentBorder.BorderBrush = new SolidColorBrush(Colors.Transparent);
            }

            _x = x;
            _y = y;
        }

        private Storyboard _newTileStoryboard = new Storyboard();
        public void BeginNewTileAnimation()
        {
            var scaleXAnimation = Animation.CreateDoubleAnimation(0.1, 1.0, 1200000);
            var scaleYAnimation = Animation.CreateDoubleAnimation(0.1, 1.0, 1200000);

            Storyboard.SetTarget(scaleXAnimation, TileBorder);
            Storyboard.SetTargetName(scaleXAnimation, "AnimatedScaleTransform");
            Storyboard.SetTargetProperty(scaleXAnimation, Animation.CreatePropertyPath("(UIElement.RenderTransform).(TransformGroup.Children)[0].(ScaleTransform.ScaleX)"));

            //((TransformGroup)RenderTransform).Children

            Storyboard.SetTarget(scaleYAnimation, TileBorder);
            Storyboard.SetTargetName(scaleYAnimation, "AnimatedScaleTransform");
            Storyboard.SetTargetProperty(scaleYAnimation, Animation.CreatePropertyPath("(UIElement.RenderTransform).(TransformGroup.Children)[0].(ScaleTransform.ScaleY)"));

            _newTileStoryboard.Children.Clear();
            _newTileStoryboard.Children.Add(scaleXAnimation);
            _newTileStoryboard.Children.Add(scaleYAnimation);
            _newTileStoryboard.Begin();
        }

        private Storyboard _doubledStoryboard = new Storyboard();
        public void BeginDoubledAnimation()
        {
            var scaleXAnimation = Animation.CreateDoubleAnimation(1.0, 1.2, 1200000);
            scaleXAnimation.AutoReverse = true;

            var scaleYAnimation = Animation.CreateDoubleAnimation(1.0, 1.2, 1200000);
            scaleYAnimation.AutoReverse = true;

            Storyboard.SetTarget(scaleXAnimation, TileBorder);
            Storyboard.SetTargetName(scaleXAnimation, "AnimatedScaleTransform");
            Storyboard.SetTargetProperty(scaleXAnimation, Animation.CreatePropertyPath("(UIElement.RenderTransform).(TransformGroup.Children)[0].(ScaleTransform.ScaleX)"));

            Storyboard.SetTarget(scaleYAnimation, TileBorder);
            Storyboard.SetTargetName(scaleYAnimation, "AnimatedScaleTransform");
            Storyboard.SetTargetProperty(scaleYAnimation, Animation.CreatePropertyPath("(UIElement.RenderTransform).(TransformGroup.Children)[0].(ScaleTransform.ScaleY)"));

            _doubledStoryboard.Children.Clear();
            _doubledStoryboard.Children.Add(scaleXAnimation);
            _doubledStoryboard.Children.Add(scaleYAnimation);

            _doubledStoryboard.Completed += (sender, o) => SetValue(Panel.ZIndexProperty, 0);

            _doubledStoryboard.Begin();
        }

        public void StopAnimations()
        {
            _newTileStoryboard.Stop();
            _doubledStoryboard.Stop();
            SetValue(Panel.ZIndexProperty, 0);
        }
    }
}
