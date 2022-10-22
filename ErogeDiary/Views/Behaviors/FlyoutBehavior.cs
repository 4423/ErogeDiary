using Microsoft.Xaml.Behaviors;
using ModernWpf.Controls.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ErogeDiary.Views.Behaviors
{
    public class FlyoutBehavior
    {
        public static readonly DependencyProperty IsVisibleProperty =
            DependencyProperty.RegisterAttached("IsOpen", 
                                                typeof(bool), 
                                                typeof(FlyoutBehavior),
                                                new PropertyMetadata(IsOpenChangedCallback));

        public static readonly DependencyProperty ParentProperty =
            DependencyProperty.RegisterAttached("Parent",
                                                typeof(FrameworkElement),
                                                typeof(FlyoutBehavior),
                                                null);

        public static bool GetIsOpen(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsVisibleProperty);
        }

        public static void SetIsOpen(DependencyObject obj, bool value)
        {
            obj.SetValue(IsVisibleProperty, value);
        }

        public static void SetParent(DependencyObject obj, FrameworkElement value)
        {
            obj.SetValue(ParentProperty, value);
        }

        public static FrameworkElement GetParent(DependencyObject obj)
        {
            return (FrameworkElement)obj.GetValue(ParentProperty);
        }

        private static void IsOpenChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var fb = d as FlyoutBase;
            if (fb == null)
            {
                return;
            }

            var isOpen = (bool)e.NewValue;
            if (isOpen)
            {
                fb.Closed += FlyoutClosed;
                fb.ShowAt(GetParent(d));
            }
            else
            {
                fb.Closed -= FlyoutClosed;
                fb.Hide();
            }
        }

        private static void FlyoutClosed(object sender, object e)
        {
            SetIsOpen(sender as DependencyObject, false);
        }
    }
}
