using System;
using System.Numerics;
using System.Linq;
using SceneKit;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Foundation;
using AppKit;

using static VacuumMold.Helpers;

namespace VacuumMold
{
    public class View
    {
        public SCNNode Node { get; }

        private Shape? shape;

        private Material backgroundMaterial = Materials.Plastic (Xyzw (1.0, 0.8, 0.8, 1), roughness: 0.1);
        public Material BackgroundMaterial {
            get => backgroundMaterial;
            set => backgroundMaterial = value; }
        public Material ForegroundMaterial { get; set; } = Materials.Silver (roughness: 0.0);

        public Shape? Shape {
            get => shape;
            set {
                shape = value;
                if (shape != null) {
                    Node.Geometry = CreateGeometry (shape);
                }
            }
        }

        public Vector3 Position {
            get => Node.Position.ToVector3 ();
            set => Node.Position = value.ToSCNVector3 ();
        }

        float pad = 16.0f;
        float elevation = 16.0f;

        public View ()
        {
            Node = SCNNode.Create ();
        }

        SCNGeometry CreateGeometry (Shape shape)
        {
            //
            // Load the shape
            //
            var shapePoints = shape.SamplePerimeter (1);
            var ipoly = new Poly2Tri.Triangulation.Polygon.Polygon (
                from p in shapePoints
                select new Poly2Tri.Triangulation.Polygon.PolygonPoint (p.X, p.Y));
            var minx = shapePoints.Min (p => p.X);
            var miny = shapePoints.Min (p => p.Y);
            var maxx = shapePoints.Max (p => p.X);
            var maxy = shapePoints.Max (p => p.Y);

            //
            // Tesselate the outer element
            //
            var osize = Xy ((maxx - minx) + 2 * pad, (maxy - miny) + 2 * pad);
            var oshapePoints = new Box (osize).SamplePerimeter (1);
            var opoly = new Poly2Tri.Triangulation.Polygon.Polygon (
                from p in oshapePoints
                select new Poly2Tri.Triangulation.Polygon.PolygonPoint (p.X, p.Y));
            opoly.AddHole (ipoly);

            //
            // Triangulate the outer polygon (with the hole in it)
            //
            Poly2Tri.P2T.Triangulate (opoly);

            //
            // Create the outer element
            //
            var opoints = opoly.Triangles.SelectMany (x => x.Points).Distinct ().ToList ();
            var opointToVertex = new Dictionary<uint, ushort> ();
            for (var i = 0; i < opoints.Count; i++) {
                opointToVertex[opoints[i].VertexCode] = (ushort)i;
            }
            var verts = opoints.Select (x => new SCNVector3 ((nfloat)x.X, (nfloat)x.Y, 0)).ToList ();
            var otris = new List<ushort> ();
            foreach (var t in opoly.Triangles) {
                foreach (var p in t.Points) {
                    otris.Add (opointToVertex[p.VertexCode]);
                }
            }
            SCNGeometryElement oelem;
            unsafe {
                var ntris = otris.Count;
                var atris = otris.ToArray ();
                fixed (ushort* ptris = atris) {
                    var oelemData = NSData.FromBytes ((IntPtr)ptris, (nuint)(ntris * 2));
                    oelem = SCNGeometryElement.FromData (oelemData, SCNGeometryPrimitiveType.Triangles, ntris / 3, 2);
                }
            }

            //
            // Triangulate the inner polygon (foreground)
            //
            Poly2Tri.P2T.Triangulate (ipoly);

            //
            // Create the inner element
            //
            var ipoints = ipoly.Triangles.SelectMany (x => x.Points).Distinct ().ToList ();
            var ipointToVertex = new Dictionary<uint, ushort> ();
            for (var i = 0; i < ipoints.Count; i++) {
                ipointToVertex[ipoints[i].VertexCode] = (ushort)(i + verts.Count);
            }
            verts.AddRange (ipoints.Select (x => new SCNVector3 ((nfloat)x.X, (nfloat)x.Y, elevation)));
            var itris = new List<ushort> ();
            foreach (var t in ipoly.Triangles) {
                foreach (var p in t.Points) {
                    itris.Add (ipointToVertex[p.VertexCode]);
                }
            }
            SCNGeometryElement ielem;
            unsafe {
                var ntris = itris.Count;
                var atris = itris.ToArray ();
                fixed (ushort* ptris = atris) {
                    var ielemData = NSData.FromBytes ((IntPtr)ptris, (nuint)(ntris * 2));
                    ielem = SCNGeometryElement.FromData (ielemData, SCNGeometryPrimitiveType.Triangles, ntris / 3, 2);
                }
            }

            //
            // Create the wall element
            //
            var wtris = new List<ushort> ();
            ushort lastIVert = 0;
            ushort lastOVert = 0;
            for (var i = 0; i <= ipoly.Points.Count; i++) {
                var ip = ipoly.Points[i % ipoly.Points.Count];
                var ivert = ipointToVertex[ip.VertexCode];
                var overt = opointToVertex[ip.VertexCode];

                if (i > 0) {
                    wtris.Add (overt);
                    wtris.Add (ivert);
                    wtris.Add (lastOVert);

                    wtris.Add (lastOVert);
                    wtris.Add (ivert);
                    wtris.Add (lastIVert);
                }

                lastIVert = ivert;
                lastOVert = overt;
            }
            SCNGeometryElement welem;
            unsafe {
                var ntris = wtris.Count;
                var atris = wtris.ToArray ();
                fixed (ushort* ptris = atris) {
                    var welemData = NSData.FromBytes ((IntPtr)ptris, (nuint)(ntris * 2));
                    welem = SCNGeometryElement.FromData (welemData, SCNGeometryPrimitiveType.Triangles, ntris / 3, 2);
                }
            }

            //
            // Create the outer crease
            //
            var creaseLineSegments = new List<ushort> ();
            var creaseSharpnesses = new List<float> ();
            for (var i = 0; i <= opoly.Points.Count; i++) {
                var op = opoly.Points[i % opoly.Points.Count];
                var overt = opointToVertex[op.VertexCode];
                if (i > 0) {
                    creaseSharpnesses.Add (5f);
                    creaseLineSegments.Add (lastOVert);
                    creaseLineSegments.Add (overt);
                }
                lastOVert = overt;
            }
            for (var i = 0; i <= ipoly.Points.Count; i++) {
                var ip = ipoly.Points[i % ipoly.Points.Count];
                var ivert = ipointToVertex[ip.VertexCode];
                if (i > 0) {
                    creaseSharpnesses.Add (5f);
                    creaseLineSegments.Add (lastIVert);
                    creaseLineSegments.Add (ivert);
                }
                lastIVert = ivert;
            }

            SCNGeometryElement celem;
            unsafe {
                var n = creaseLineSegments.Count;
                var ar = creaseLineSegments.ToArray ();
                fixed (ushort* p = ar) {
                    var data = NSData.FromBytes ((IntPtr)p, (nuint)(n * 2));
                    celem = SCNGeometryElement.FromData (data, SCNGeometryPrimitiveType.Line, n / 2, 2);
                }
            }
            SCNGeometrySource csource;
            unsafe {
                Console.WriteLine (creaseSharpnesses);
                var n = creaseSharpnesses.Count;
                var ar = creaseSharpnesses.ToArray ();
                fixed (float* p = ar) {
                    var data = NSData.FromBytes ((IntPtr)p, (nuint)(n * 4));
                    csource = SCNGeometrySource.FromData (data,
                        SCNGeometrySourceSemantics.EdgeCrease, n, true, 1, 4, 0, 4);
                }
            }

            //
            // Create the geometry
            //
            var sources = new[] {
                SCNGeometrySource.FromVertices (verts.ToArray ()),
            };
            var elements = new[] {
                oelem, ielem, welem
            };
            var g = SCNGeometry.Create (sources, elements.ToArray ());

            var omat = BackgroundMaterial.SCNMaterial;

            var imat = ForegroundMaterial.SCNMaterial;

            var wmat = Materials.Gold(roughness: 0.1).SCNMaterial;

            g.Materials = new[] { omat, imat, wmat };

            g.EdgeCreasesElement = celem;
            g.EdgeCreasesSource = csource;
            g.SubdivisionLevel = 4;

            return g;
        }
    }
}
