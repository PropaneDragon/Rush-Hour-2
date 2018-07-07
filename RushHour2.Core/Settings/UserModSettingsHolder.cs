using RushHour2.Core.Info;
using System;
using System.Xml;
using System.Xml.Serialization;

namespace RushHour2.Core.Settings
{
    public class UserModSettingsHolder
    {
        [XmlElement]
        public bool Enabled = true;

        [XmlIgnore]
        public Version LastVersion = new Version();

        [XmlElement]
        public string LastVersion_String
        {
            get => LastVersion.ToString();
            set => LastVersion = string.IsNullOrEmpty(value) ? new Version() : new Version(value);
        }

        [XmlElement]
        public bool Logging_ToFile = true;

        [XmlElement]
        public bool Logging_ToConsole = true;

        [XmlElement]
        public bool Logging_ToDebug = true;

        [XmlIgnore]
        public TimeSpan Logging_ToFile_Duration = TimeSpan.FromSeconds(5);

        [XmlElement(DataType = "duration")]
        public string Logging_ToFile_Duration_String
        {
            get => XmlConvert.ToString(Logging_ToFile_Duration);
            set => Logging_ToFile_Duration = string.IsNullOrEmpty(value) ? TimeSpan.Zero : XmlConvert.ToTimeSpan(value);
        }

        [XmlElement]
        public bool MessageBoxes_Enabled = true;

        [XmlElement]
        public float Simulation_Speed = 0.25f;

        [XmlElement]
        public bool DateTimeBar_Modify = true;

        [XmlElement]
        public string DateTimeBar_Format = "dddd HH:mm";

        [XmlElement]
        public bool Citizens_Override = true;

        [XmlElement]
        public bool Citizens_IgnoreVehicleCount = false;

        [XmlElement]
        public bool Citizens_AllowLeisureAfterWork = true;

        [XmlElement]
        public bool Tourists_Override = true;

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
        public string StartTime_Work_String
        {
            get => XmlConvert.ToString(StartTime_Work);
            set => StartTime_Work = string.IsNullOrEmpty(value) ? TimeSpan.Zero : XmlConvert.ToTimeSpan(value);
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
            get => XmlConvert.ToString(Duration_Work);
            set => Duration_Work = string.IsNullOrEmpty(value) ? TimeSpan.Zero : XmlConvert.ToTimeSpan(value);
        }

        [XmlIgnore]
        public TimeSpan StartTime_Schools = TimeSpan.FromHours(8.5);
        [XmlIgnore]
        public TimeSpan StartTime_University = TimeSpan.FromHours(10);
        [XmlIgnore]
        public TimeSpan StartTime_Work = TimeSpan.FromHours(9);

        [XmlIgnore]
        public TimeSpan Duration_Schools = TimeSpan.FromHours(6.5);
        [XmlIgnore]
        public TimeSpan Duration_University = TimeSpan.FromHours(6);
        [XmlIgnore]
        public TimeSpan Duration_Work = TimeSpan.FromHours(8);
    }
}
