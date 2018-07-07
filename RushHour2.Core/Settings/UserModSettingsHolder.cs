using System;
using System.Xml;
using System.Xml.Serialization;

namespace RushHour2.Core.Settings
{
    public class UserModSettingsHolder
    {
        [XmlElement]
        public bool Enabled = true;

        [XmlElement]
        public bool Logging_ToFile = true;
        [XmlElement]
        public bool Logging_ToConsole = true;
        [XmlElement]
        public bool Logging_ToDebug = true;

        [XmlElement]
        public float Simulation_Speed = 0.25f;

        [XmlElement]
        public bool DateTimeBar_Modify = true;
        [XmlElement]
        public string DateTimeBar_Format = "dddd HH:mm";

        [XmlElement]
        public bool Citizens_IgnoreVehicleCount = false;
        
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
