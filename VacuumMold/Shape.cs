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
        public Vector2 Size { get; set; }
        
        public Box (Vector2 frame)
        {
            Size = frame;
        }

        public override Vector2[] SamplePerimeter (float tolerance)
        {
            var w2 = Size.X / 2;
            var h2 = Size.Y / 2;
            return new[] {
                Xy (-w2, -h2),
                Xy (w2, -h2),
                Xy (w2, h2),
                Xy (-w2, h2),
            };
        }
    }

    public class Oval : Shape
    {
        public Vector2 Size { get; set; }

        public Oval (Vector2 size)
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
            var w2 = Size.X / 2;
            var h2 = Size.Y / 2;
            for (var i = 0; i < n; i++, a += da) {
                var x = w2 * Math.Cos (a);
                var y = h2 * Math.Sin (a);
                r[i] = Xy (x, y);
            }
            return r;
        }
    }
}
