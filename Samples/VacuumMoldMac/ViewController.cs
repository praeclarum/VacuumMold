using System;

using AppKit;
using CoreGraphics;
using Foundation;
using VacuumMold;

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

            var r = new Random ();
            var n = 10 + r.Next (10);

            var b = chamber.Bounds;

            for (var i = 0; i < n; i++) {
                var x = b.Width * r.NextDouble ();
                var y = b.Height * r.NextDouble ();
                var w = 44 + 200 * r.NextDouble ();
                var h = 44 + 200 * r.NextDouble ();
                var f = new CGRect (x, y, w, h);
                var mold = new Mold ();
                if (r.NextDouble () < 0.5) {
                    mold.Shape = new Oval (f);
                }
                else {
                    mold.Shape = new Box (f);
                }
                chamber.AddMold (mold);
            }

            // Do any additional setup after loading the view.
        }

        public override NSObject RepresentedObject
        {
            get
            {
                return base.RepresentedObject;
            }
            set
            {
                base.RepresentedObject = value;
                // Update the view, if already loaded.
            }
        }
    }
}
