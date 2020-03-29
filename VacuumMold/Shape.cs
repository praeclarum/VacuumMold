using System;
using System.Numerics;
using CoreGraphics;

using static VacuumMold.Helpers;

namespace VacuumMold
{
    public abstract class Shape
    {
        public abstract Vector2[] SamplePerimeter (float tolerance);
    }

    public class Box : Shape
    {
        public CGSize Size { get; set; }
        
        public Box (CGSize frame)
        {
            Size = frame;
        }

        public override Vector2[] SamplePerimeter (float tolerance)
        {
            return new[] {
                Xy (0, 0),
                Xy (0 + Size.Width, 0),
                Xy (0 + Size.Width, 0 + Size.Height),
                Xy (0, 0 + Size.Height),
            };
        }
    }

    public class Oval : Shape
    {
        public CGSize Size { get; set; }

        public Oval (CGSize size)
        {
            Size = size;
        }

        public override Vector2[] SamplePerimeter (float tolerance)
        {
            var n = 100;
            var enda = 2.0 * Math.PI;
            var da = enda / n;
            var a = 0.0;
            var r = new Vector2[n];
            var w2 = Size.Width / 2;
            var h2 = Size.Height / 2;
            var cx = 0 + w2;
            var cy = 0 + h2;
            for (var i = 0; i < n; i++, a += da) {
                var x = cx + w2 * Math.Cos (a);
                var y = cy + h2 * Math.Sin (a);
                r[i] = Xy (x, y);
            }
            return r;
        }
    }
}
