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
        public CGRect Frame { get; set; }

        public Box (CGRect frame)
        {
            Frame = frame;
        }

        public override Vector2[] SamplePerimeter (float tolerance)
        {
            return new[] {
                Xy (Frame.X, Frame.Y),
                Xy (Frame.X + Frame.Width, Frame.Y),
                Xy (Frame.X + Frame.Width, Frame.Y + Frame.Height),
                Xy (Frame.X, Frame.Y + Frame.Height),
            };
        }
    }

    public class Oval : Shape
    {
        public CGRect Frame { get; set; }

        public Oval (CGRect frame)
        {
            Frame = frame;
        }

        public override Vector2[] SamplePerimeter (float tolerance)
        {
            var n = 100;
            var enda = 2.0 * Math.PI;
            var da = enda / n;
            var a = 0.0;
            var r = new Vector2[n];
            var w2 = Frame.Size.Width / 2;
            var h2 = Frame.Size.Height / 2;
            var cx = Frame.X + w2;
            var cy = Frame.Y + h2;
            for (var i = 0; i < n; i++, a += da) {
                var x = cx + w2 * Math.Cos (a);
                var y = cy + h2 * Math.Sin (a);
                r[i] = Xy (x, y);
            }
            return r;
        }
    }
}
