using ColossalFramework.UI;
using ICities;
using System;

namespace RushHour2.UI.Extensions
{
    public static class UIHelperBaseExtensions
    {
        public static object AddSliderWithTooltip(this UIHelperBase helperBase, string text, float min, float max, float step, float defaultValue, OnValueChanged eventCallback, Func<float, string> tooltipFunction)
        {
            var sliderObject = helperBase.AddSlider(text, min, max, step, defaultValue, eventCallback);
            if (sliderObject is UISlider slider)
            {
                slider.eventValueChanged += (component, eventParam) => UpdateTooltipUsingFunction(component, tooltipFunction);
                slider.eventTooltipHover += (component, value) => UpdateTooltipUsingFunction(component, tooltipFunction);
            }

            return sliderObject;
        }

        public static object AddTimeSpanHoursSlider(this UIHelperBase helperBase, string text, TimeSpan minimum, TimeSpan maximum, TimeSpan defaultValue, OnValueChanged valueChanged, string format = "HH:mm")
        {
            var resolution = (1f / 60f) * 5f;
            var sliderObject = helperBase.AddSlider(text, (float)minimum.TotalHours, (float)maximum.TotalHours, resolution, (float)defaultValue.TotalHours, valueChanged) as UIComponent;
            if (sliderObject is UISlider slider)
            {
                slider.eventValueChanged += (component, eventParam) => UpdateTooltipForTimeSpan(component, (sliderValue) => TimeSpan.FromHours(sliderValue), format);
                slider.eventTooltipHover += (component, value) => UpdateTooltipForTimeSpan(component, (sliderValue) => TimeSpan.FromHours(sliderValue), format);
            }

            return sliderObject;
        }
        public static object AddTimeSpanSecondsSlider(this UIHelperBase helperBase, string text, TimeSpan minimum, TimeSpan maximum, TimeSpan defaultValue, OnValueChanged valueChanged, string format = "s")
        {
            var sliderObject = helperBase.AddSlider(text, (float)minimum.TotalSeconds, (float)maximum.TotalSeconds, 1, (float)defaultValue.TotalSeconds, valueChanged) as UIComponent;
            if (sliderObject is UISlider slider)
            {
                slider.eventValueChanged += (component, eventParam) => UpdateTooltipForTimeSpan(component, (sliderValue) => TimeSpan.FromSeconds(sliderValue), format);
                slider.eventTooltipHover += (component, value) => UpdateTooltipForTimeSpan(component, (sliderValue) => TimeSpan.FromSeconds(sliderValue), format);
            }

            return sliderObject;
        }

        private static void UpdateTooltipForTimeSpan(UIComponent component, Func<float, TimeSpan> valueToTimeSpanFunction, string format)
        {
            UpdateTooltipUsingFunction(component, value =>
            {
                var currentTime = valueToTimeSpanFunction.Invoke(value);
                var temporaryDate = DateTime.Today.Add(currentTime);

                return temporaryDate.ToString(format);
            });
        }

        private static void UpdateTooltipUsingFunction(UIComponent component, Func<float, string> tooltipFunction)
        {
            var tooltipValue = "";

            if (component is UISlider slider)
            {
                tooltipValue = tooltipFunction.Invoke(slider.value);
            }

            if (tooltipValue != null)
            {
                component.tooltip = tooltipValue;
                component.Update();
                component.RefreshTooltip();
            }
        }
    }
}
