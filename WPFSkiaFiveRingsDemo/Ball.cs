using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPFSkiaFiveRingsDemo
{
    public class Ball
    {
        public double X { get; set; }
        public double Y { get; set; }
        public double VX { get; set; }
        public double VY { get; set; }
        public int Radius { get; set; }
        public bool Dragged { get; set; } = false;
        public SKColor sKColor { get; set; } = SKColors.Blue;
        public bool CheckPoint(SKPoint sKPoint)
        {
            var d = Math.Sqrt(Math.Pow(sKPoint.X - X, 2) + Math.Pow(sKPoint.Y - Y, 2));
            return this.Radius >= d;
        }
    }
}
