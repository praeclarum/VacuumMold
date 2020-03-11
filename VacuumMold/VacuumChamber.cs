using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using AppKit;
using CoreGraphics;
using Foundation;
using SceneKit;

namespace VacuumMold
{
    [Register ("VacuumChamber")]
    public class VacuumChamber : SCNView
    {
        readonly SCNScene scene = SCNScene.Create ();


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

        public void AddMold (Mold mold)
        {
            scene.RootNode.AddChildNode (mold.Node);
        }

        public void RemoveMold (Mold mold)
        {
        }
    }
}
