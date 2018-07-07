using ColossalFramework.UI;
using ICities;
using System;

namespace RushHour2.UI.Extensions
{
    public static class UIHelperBaseExtensions
    {
        public static object AddTimeSpanSlider(this UIHelperBase helperBase, string text, TimeSpan minimum, TimeSpan maximum, TimeSpan defaultValue, OnValueChanged valueChanged)
        {
            var resolution = (1f / 60f) * 5f;
            var sliderObject = helperBase.AddSlider(text, (float)minimum.TotalHours, (float)maximum.TotalHours, resolution, (float)defaultValue.TotalHours, valueChanged) as UIComponent;
            if (sliderObject is UISlider slider)
            {
                slider.eventValueChanged += (component, eventParam) => UpdateTooltipForTimeSpan(component);
                slider.eventTooltipHover += (component, value) => UpdateTooltipForTimeSpan(component);
            }

            return sliderObject;
        }

        private static void UpdateTooltipForTimeSpan(UIComponent component)
        {
            if (component is UISlider slider)
            {
                var currentTime = TimeSpan.FromHours(slider.value);
                var temporaryDate = DateTime.Today.Add(currentTime);

                slider.tooltip = temporaryDate.ToString("HH:mm");
                slider.Update();
                slider.RefreshTooltip();
            }
        }
    }
}
