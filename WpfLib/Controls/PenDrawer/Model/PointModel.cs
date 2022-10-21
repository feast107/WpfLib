﻿using Newtonsoft.Json;

namespace WpfLib.Controls.PenDrawer.Model
{
    public abstract class PointModel
    {
        [JsonProperty("x")]
        public int X { get; set; }
        [JsonProperty("y")]
        public int Y { get; set; }
    }
}
