// <auto-generated />
//
// To parse this JSON data, add NuGet 'Newtonsoft.Json' then do:
//
//    using QuickType;
//
//    var chartData = ChartData.FromJson(jsonString);

    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;
    public partial class ChartData
    {
        [JsonProperty("formatVersion")]
        public long FormatVersion { get; set; }

        [JsonProperty("offset")]
        public double Offset { get; set; }

        [JsonProperty("judgeLineList")]
        public JudgeLineList[] JudgeLineList { get; set; }
    }

    public partial class JudgeLineList
    {
        [JsonProperty("bpm")]
        public float Bpm { get; set; }

        [JsonProperty("notesAbove")]
        public NotesAbove[] NotesAbove { get; set; }

        [JsonProperty("notesBelow")]
        public NotesBelow[] NotesBelow { get; set; }

        [JsonProperty("speedEvents")]
        public SpeedEvent[] SpeedEvents { get; set; }

        [JsonProperty("judgeLineMoveEvents")]
        public JudgeLineMoveEvent[] JudgeLineMoveEvents { get; set; }

        [JsonProperty("judgeLineRotateEvents")]
        public JudgeLineEvent[] JudgeLineRotateEvents { get; set; }

        [JsonProperty("judgeLineDisappearEvents")]
        public JudgeLineEvent[] JudgeLineDisappearEvents { get; set; }
    }

    public partial class JudgeLineEvent
    {
        [JsonProperty("startTime")]
        public double StartTime { get; set; }

        [JsonProperty("endTime")]
        public double EndTime { get; set; }

        [JsonProperty("start")]
        public double Start { get; set; }

        [JsonProperty("end")]
        public double End { get; set; }
    }

    public partial class JudgeLineMoveEvent
    {
        [JsonProperty("startTime")]
        public double StartTime { get; set; }

        [JsonProperty("endTime")]
        public double EndTime { get; set; }

        [JsonProperty("start")]
        public double Start { get; set; }

        [JsonProperty("end")]
        public double End { get; set; }

        [JsonProperty("start2")]
        public double Start2 { get; set; }

        [JsonProperty("end2")]
        public double End2 { get; set; }
    }

    public partial class NotesAbove
    {
        [JsonProperty("type")]
        public long Type { get; set; }

        [JsonProperty("time")]
        public long Time { get; set; }

        [JsonProperty("positionX")]
        public double PositionX { get; set; }

        [JsonProperty("holdTime")]
        public double HoldTime { get; set; }

        [JsonProperty("speed")]
        public double Speed { get; set; }

        [JsonProperty("floorPosition")]
        public double FloorPosition { get; set; }
    }
    public partial class NotesBelow
    {
        [JsonProperty("type")]
        public long Type { get; set; }

        [JsonProperty("time")]
        public long Time { get; set; }

        [JsonProperty("positionX")]
        public double PositionX { get; set; }

        [JsonProperty("holdTime")]
        public double HoldTime { get; set; }

        [JsonProperty("speed")]
        public double Speed { get; set; }

        [JsonProperty("floorPosition")]
        public double FloorPosition { get; set; }
    }

public partial class SpeedEvent
    {
        [JsonProperty("startTime")]
        public double StartTime { get; set; }

        [JsonProperty("endTime")]
        public double EndTime { get; set; }

        [JsonProperty("value")]
        public double Value { get; set; }
    }

    public partial class ChartData
    {
        public static ChartData FromJson(string json) => JsonConvert.DeserializeObject<ChartData>(json,Converter.Settings);
    }

    public static class Serialize
    {
        public static string ToJson(this ChartData self) => JsonConvert.SerializeObject(self, Converter.Settings);
    }

    internal static class Converter
    {
        public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
            DateParseHandling = DateParseHandling.None,
            Converters =
            {
                new IsoDateTimeConverter { DateTimeStyles = DateTimeStyles.AssumeUniversal }
            },
        };
    }

