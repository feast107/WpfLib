using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using WpfLib.Controls.PenDrawer.Definition;

namespace WpfLib.Controls.PenDrawer.Model
{
    public class StrokeModel
    {
        public StrokeModel() { }
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

        [JsonProperty("x1")]
        public virtual int X1 { get; set; }
        [JsonProperty("y1")]
        public virtual int Y1 { get; set; }
    }
    public static class StrokeExtension
    {
        public static List<PointModel> StrokeToPoints(this StrokeModel stroke)
        {
            var svgStroke = stroke.Path;
            List<PointModel> points = new ();
            if (string.IsNullOrEmpty(svgStroke)) return points;
            var dataArray = svgStroke.Split('l');
            if (dataArray.Length != 2) return points;
            try
            {
                var tmp = dataArray[0];
                var firstPointArray = tmp.Replace("M", "").Split(' ');
                tmp = firstPointArray[0];
                var firstPointX = Convert.ToInt32(tmp.Trim());
                tmp = firstPointArray[1];
                var firstPointY = Convert.ToInt32(tmp.Trim());
                points.Add( new PointModel()
                {
                    X = firstPointX,
                    Y= firstPointY,
                });
                tmp = dataArray[1];
                var pointsArray = tmp
                    .Replace("l", "")
                    .Replace("-", " -")
                    .Trim().Split(' ');
                for (var i = 0; i < pointsArray.Length; i++)
                {
                    try
                    {
                        if (i + 1 < pointsArray.Length)
                        {
                            tmp = pointsArray[i];
                            firstPointX += Convert.ToInt32(tmp.Trim());
                            tmp = pointsArray[i + 1];
                            firstPointY += Convert.ToInt32(tmp.Trim());
                            points.Add(new PointModel()
                            {
                                X = firstPointX,
                                Y = firstPointY,
                            });
                        }
                    }
                    catch (Exception e){
                        Console.WriteLine(e.Message);
                    }
                    i++;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return points;
        }
        public static string PointsToStroke(this List<PointModel> points)
        {
            if (points == null || points.Count == 0)
            {
                return "";
            }
            string ret = "";
            if (points.Count > 0)
            {
                var point = points[0];
                //第一个点为M起点
                ret += ("M" + point.X + " " + point.Y + "l");
                var oldX = point.X;
                var oldY = point.Y;

                if (points.Count > 1)
                {
                    var j = 1;
                    //L第一个点特殊处理
                    point = points[j];
                    var curX = (point.X - oldX);
                    var curY = (point.Y - oldY);
                    oldX = point.X;
                    oldY = point.Y;
                    ret += curX;
                    if (curY >= 0)
                    {
                        ret += " ";
                    }
                    ret += curY;
                    j++;
                    while (j < points.Count)
                    {
                        point = points[j];
                        curX = (point.X - oldX);
                        curY = (point.Y - oldY);
                        oldX = point.X;
                        oldY = point.Y;
                        switch (curX)
                        {
                            case 0 when curY == 0:
                                j++;
                                continue;
                            case >= 0:
                                ret +=" ";
                                break;
                        }
                        ret+= curX;
                        if (curY >= 0)
                        {
                            ret+=" ";
                        }
                        ret += curY;
                        j++;
                    }
                }
            }
            return ret;
        }
    }
}
