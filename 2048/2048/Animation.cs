using System;

#if NETFX_CORE
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media.Animation;
#elif (WINDOWS_PHONE || NETFX_451)
using System.Windows;
using System.Windows.Media.Animation;
#endif

namespace _2048
{
    class Animation
    {

#if NETFX_CORE
        public static string CreatePropertyPath(string PropertyPath)
        {
            return PropertyPath;
        }
#elif (WINDOWS_PHONE || NETFX_451)
        public static PropertyPath CreatePropertyPath(string propertyPath)
        {
            return new PropertyPath(propertyPath);
        }
#endif

        public static DoubleAnimation CreateDoubleAnimation(double? @from, double? to, long durationTicks)
        {
            var animation = new DoubleAnimation();
            animation.From = @from;
            animation.To = to;
            animation.Duration = new Duration(new TimeSpan(durationTicks));
#if NETFX_CORE
            animation.EnableDependentAnimation = true;
#endif
            return animation;
        }
    }
}
