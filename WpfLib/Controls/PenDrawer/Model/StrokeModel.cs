using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using WpfLib.Controls.PenDrawer.Definition;

namespace WpfLib.Controls.PenDrawer.Model
{
    public class StrokeModel
    {
        /// <summary>
        /// Svg path
        /// </summary>
        [JsonProperty("p")]
        public virtual string Path { get; set; }
        [JsonProperty("c")]
        public virtual StrokeColor Color { get; set; }
        [JsonProperty("t")]
        public virtual StrokeThickness Thickness { get; set; }
        [JsonProperty("s")]
        public virtual long Timestamp { get; set; }
    }
    public static class StrokeExtension
    {
        public static List<PointModel> ToPoints(this StrokeModel stroke)
        {
            throw new NotImplementedException();
        }
    }
}
