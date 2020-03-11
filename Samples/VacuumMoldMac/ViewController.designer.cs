// WARNING
//
// This file has been generated automatically by Visual Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;
using System.CodeDom.Compiler;

namespace VacuumMoldMac
{
	[Register ("ViewController")]
	partial class ViewController
	{
		[Outlet]
		VacuumMold.VacuumChamber chamber { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (chamber != null) {
				chamber.Dispose ();
				chamber = null;
			}
		}
	}
}
