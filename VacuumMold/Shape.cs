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
}
