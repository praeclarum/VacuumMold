using System;

using AppKit;
using CoreGraphics;
using Foundation;
using VacuumMold;

using static VacuumMold.Helpers;

namespace VacuumMoldMac
{
    public partial class ViewController : NSViewController
    {
        public ViewController(IntPtr handle) : base(handle)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            //
            // Generating test views
            //
            var r = new Random ();
            var n = 10 + r.Next (10);

            var b = chamber.Bounds;

            for (var i = 0; i < n; i++) {
                var x = b.Width * r.NextDouble ();
                var y = b.Height * r.NextDouble ();
                var w = b.Width * 0.3f * r.NextDouble ();
                var h = b.Height * 0.3f * r.NextDouble ();
                var f = new CGRect (x, y, w, h);
                var view = new View ();
                if (r.NextDouble () < 0.5) {
                    view.Shape = new Oval (Xy (w, h));
                    view.Position = Xyz (x, y, 0);
                }
                else {
                    view.Shape = new Box (Xy (w, h));
                    view.Position = Xyz (x, y, 0);
                }
                chamber.AddMold (view);
            }
        }
    }
}
