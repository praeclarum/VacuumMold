using System;
using System.Collections.Concurrent;
using AppKit;
using CoreGraphics;
using Foundation;
using SceneKit;

namespace VacuumMold
{
    [Register ("VacuumChamber")]
    public class VacuumChamber : SCNView
    {
        public VacuumChamber (CGRect frame)
            : base (frame) => Initialize ();
        public VacuumChamber (NSCoder coder)
            : base (coder) => Initialize ();
        public VacuumChamber (IntPtr handle)
            : base (handle) => Initialize ();
        public VacuumChamber (NSObjectFlag t)
            : base (t) => Initialize ();

        void Initialize ()
        {
            BackgroundColor = NSColor.Black;
        }
    }
}
