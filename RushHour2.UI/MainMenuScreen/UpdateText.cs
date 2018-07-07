using ColossalFramework.UI;
using RushHour2.Core.Info;
using RushHour2.Core.Settings;
using UnityEngine;

namespace RushHour2.UI.MainMenuScreen
{
    public static class UpdateText
    {
        public static bool DisplayIfRequired()
        {
            if (UserModSettings.RecentlyUpdated)
            {
                var menuArea = UIView.Find<UIPanel>("MenuArea");
                if (menuArea != null)
                {
                    var updateText = menuArea.AddUIComponent<UILabel>();
                    updateText.text = $"{Details.BaseModName} has updated!";
                    updateText.tooltip = "Click for more information";
                    updateText.bottomColor = new Color32(255, 0, 0, 255);
                    updateText.useGradient = true;
                    updateText.textScale = 1f;
                    updateText.eventClicked += UpdateText_Clicked;
                    updateText.Update();

                    updateText.relativePosition = new Vector3((menuArea.width / 2f) - (updateText.width / 2f), 10);

                    return true;
                }
            }

            return false;
        }

        private static void UpdateText_Clicked(UIComponent component, UIMouseEventParameter eventParam)
        {
            FileSystemUtils.OpenURLInOverlayOrBrowser($"https://steamcommunity.com/sharedfiles/filedetails/changelog/{Details.Info.publishedFileID.AsUInt64}");
        }
    }
}
