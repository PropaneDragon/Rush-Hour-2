using System;
using System.Xml;
using System.Xml.Serialization;

namespace RushHour2.Core.Settings
{
    public class UserModSettingsHolder
    {
        public bool Enabled = true;

        [XmlIgnore]
        public Version LastVersion = new Version();

        public string LastVersion_String
        {
            get => LastVersion.ToString();
            set => LastVersion = string.IsNullOrEmpty(value) ? new Version() : new Version(value);
        }
        
        public string Language = "English (GB)";
        public bool Log_Citizen_Status = false;
        public bool Logging_ToFile = true;
        public bool Logging_ToConsole = true;
        public bool Logging_ToDebug = true;

        [XmlIgnore]
        public TimeSpan Logging_ToFile_Duration = TimeSpan.FromSeconds(5);

        [XmlElement(DataType = "duration")]
        public string Logging_ToFile_Duration_String
        {
            get => XmlConvert.ToString(Logging_ToFile_Duration);
            set => Logging_ToFile_Duration = string.IsNullOrEmpty(value) ? TimeSpan.Zero : XmlConvert.ToTimeSpan(value);
        }
        
        public bool MessageBoxes_Enabled = true;
        public float Simulation_Speed = 0.25f;
        public bool DateTimeBar_Modify = true;
        public bool Time_24Hour = true;
        public string DateTimeBar_Format = "dddd HH:mm";

        public bool Citizens_Override = true;
        public bool Citizens_IgnoreVehicleCount = false;
        public bool Citizens_AllowLeisureAfterWork = true;
        public bool Citizens_ReactToWeather = true;

        public bool Tourists_Override = true;
        
        public bool Buildings_OverrideLights_1 = true;
        public bool Buildings_OverrideSchoolLights_1 = true;
        public bool Buildings_OverrideCommercialLights_1 = false;
        public bool Buildings_OverrideOfficeLights_1 = false;
        public bool Buildings_OverrideIndustrialLights_1 = false;
        public bool Buildings_OverrideResidentialLights_1 = false;

        [XmlElement(DataType = "duration")]
        public string StartTime_Schools_String
        {
            get => XmlConvert.ToString(StartTime_Schools);
            set => StartTime_Schools = string.IsNullOrEmpty(value) ? TimeSpan.Zero : XmlConvert.ToTimeSpan(value);
        }

        [XmlElement(DataType = "duration")]
        public string StartTime_University_String
        {
            get => XmlConvert.ToString(StartTime_University);
            set => StartTime_University = string.IsNullOrEmpty(value) ? TimeSpan.Zero : XmlConvert.ToTimeSpan(value);
        }

        [XmlElement(DataType = "duration")]
        public string StartTime_Commercial_Weekday_String
        {
            get => XmlConvert.ToString(StartTime_Commercial_Weekday);
            set => StartTime_Commercial_Weekday = string.IsNullOrEmpty(value) ? TimeSpan.Zero : XmlConvert.ToTimeSpan(value);
        }

        [XmlElement(DataType = "duration")]
        public string StartTime_Commercial_Weekend_String
        {
            get => XmlConvert.ToString(StartTime_Commercial_Weekend);
            set => StartTime_Commercial_Weekend = string.IsNullOrEmpty(value) ? TimeSpan.Zero : XmlConvert.ToTimeSpan(value);
        }

        [XmlElement(DataType = "duration")]
        public string StartTime_Industrial_Weekday_String
        {
            get => XmlConvert.ToString(StartTime_Industrial_Weekday);
            set => StartTime_Industrial_Weekday = string.IsNullOrEmpty(value) ? TimeSpan.Zero : XmlConvert.ToTimeSpan(value);
        }

        [XmlElement(DataType = "duration")]
        public string StartTime_Industrial_Weekend_String
        {
            get => XmlConvert.ToString(StartTime_Industrial_Weekend);
            set => StartTime_Industrial_Weekend = string.IsNullOrEmpty(value) ? TimeSpan.Zero : XmlConvert.ToTimeSpan(value);
        }

        [XmlElement(DataType = "duration")]
        public string StartTime_Offices_Weekday_String
        {
            get => XmlConvert.ToString(StartTime_Offices_Weekday);
            set => StartTime_Offices_Weekday = string.IsNullOrEmpty(value) ? TimeSpan.Zero : XmlConvert.ToTimeSpan(value);
        }

        [XmlElement(DataType = "duration")]
        public string Duration_Schools_String
        {
            get => XmlConvert.ToString(Duration_Schools);
            set => Duration_Schools = string.IsNullOrEmpty(value) ? TimeSpan.Zero : XmlConvert.ToTimeSpan(value);
        }

        [XmlElement(DataType = "duration")]
        public string Duration_University_String
        {
            get => XmlConvert.ToString(Duration_University);
            set => Duration_University = string.IsNullOrEmpty(value) ? TimeSpan.Zero : XmlConvert.ToTimeSpan(value);
        }

        [XmlElement(DataType = "duration")]
        public string Duration_Work_String
        {
            get => XmlConvert.ToString(Duration_Commercial_Weekday);
            set => Duration_Commercial_Weekday = string.IsNullOrEmpty(value) ? TimeSpan.Zero : XmlConvert.ToTimeSpan(value);
        }

        [XmlIgnore]
        public TimeSpan StartTime_Schools = TimeSpan.FromHours(8.5);
        [XmlIgnore]
        public TimeSpan StartTime_University = TimeSpan.FromHours(10);
        [XmlIgnore]
        public TimeSpan StartTime_Commercial_Weekday = TimeSpan.FromHours(8);
        [XmlIgnore]
        public TimeSpan StartTime_Commercial_Weekend = TimeSpan.FromHours(8);
        [XmlIgnore]
        public TimeSpan StartTime_Industrial_Weekday = TimeSpan.FromHours(9);
        [XmlIgnore]
        public TimeSpan StartTime_Industrial_Weekend = TimeSpan.FromHours(9);
        [XmlIgnore]
        public TimeSpan StartTime_Offices_Weekday = TimeSpan.FromHours(9);

        [XmlIgnore]
        public TimeSpan Duration_Schools = TimeSpan.FromHours(6.5);
        [XmlIgnore]
        public TimeSpan Duration_University = TimeSpan.FromHours(6);
        [XmlIgnore]
        public TimeSpan Duration_Commercial_Weekday = TimeSpan.FromHours(12);
        [XmlIgnore]
        public TimeSpan Duration_Commercial_Weekend = TimeSpan.FromHours(12);
        [XmlIgnore]
        public TimeSpan Duration_Industrial_Weekday = TimeSpan.FromHours(8);
        [XmlIgnore]
        public TimeSpan Duration_Industrial_Weekend = TimeSpan.FromHours(8);
        [XmlIgnore]
        public TimeSpan Duration_Offices_Weekday = TimeSpan.FromHours(8);
    }
}
