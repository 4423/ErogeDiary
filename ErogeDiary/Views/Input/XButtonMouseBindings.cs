using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Markup;

namespace ErogeDiary.Views.Input
{
    public class XButtonMouseBinding : MouseBinding
    {
        [ValueSerializer(typeof(MouseGestureValueSerializer)), TypeConverter(typeof(XButtonMouseGestureConverter))]
        public override InputGesture Gesture { get => base.Gesture; set => base.Gesture = value; }
    }


    internal class XButtonMouseGestureConverter : MouseGestureConverter
    {
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object source)
        {
            switch (source.ToString())
            {
                case "XButton1":
                    return new XButtonMouseGesture(MouseButton.XButton1);
                case "XButton2":
                    return new XButtonMouseGesture(MouseButton.XButton2);
            }
            return base.ConvertFrom(context, culture, source);
        }
    }


    internal class XButtonMouseGesture : MouseGesture
    {
        private MouseButton mouseButton;

        public XButtonMouseGesture(MouseButton mouseButton)
        {
            this.mouseButton = mouseButton;
        }

        public override bool Matches(object targetElement, InputEventArgs inputEventArgs)
        {
            var device = inputEventArgs.Device as MouseDevice;
            if (device != null)
            {
                switch (mouseButton)
                {
                    case MouseButton.XButton1:
                        return device.XButton1 == MouseButtonState.Pressed;
                    case MouseButton.XButton2:
                        return device.XButton2 == MouseButtonState.Pressed;
                }
            }
            return false;
        }
    }
}
