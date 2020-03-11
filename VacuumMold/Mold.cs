using System;
using System.Numerics;
using System.Linq;
using SceneKit;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Foundation;

namespace VacuumMold
{
    public class Mold
    {
        private Shape? shape;

        public SCNNode Node { get; }

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

        public Mold ()
        {
            Node = SCNNode.Create ();
        }

        static SCNGeometry CreateGeometry (Shape shape)
        {

            var shapePoints = shape.SamplePerimeter (1);
            var poly = new Poly2Tri.Triangulation.Polygon.Polygon (
                from p in shapePoints
                select new Poly2Tri.Triangulation.Polygon.PolygonPoint (p.X, p.Y));

            var minx = shapePoints.Min (p => p.X);
            var miny = shapePoints.Min (p => p.Y);
            var maxx = shapePoints.Max (p => p.X);
            var maxy = shapePoints.Max (p => p.Y);

            var pad = 10.0f;

            var oframe = new CoreGraphics.CGRect (minx - pad, miny - pad, (maxx - minx) + 2 * pad, (maxy - miny) + 2 * pad);
            var oshapePoints = new Box (oframe).SamplePerimeter(1);
            var opoly = new Poly2Tri.Triangulation.Polygon.Polygon (
                from p in oshapePoints
                select new Poly2Tri.Triangulation.Polygon.PolygonPoint (p.X, p.Y));
            opoly.AddHole (poly);

            Poly2Tri.P2T.Triangulate (opoly);

            Console.WriteLine (opoly.Triangles.Count);

            var tpoints = opoly.Triangles.SelectMany (x => x.Points).Distinct ().ToList ();

            var pointToVertex = new Dictionary<uint, ushort> ();
            for (var i = 0; i < tpoints.Count; i++) {
                pointToVertex[tpoints[i].VertexCode] = (ushort)i;
            }

            var verts = tpoints.Select (x => new SCNVector3 ((nfloat)x.X, (nfloat)x.Y, 0)).ToList ();
            var tris = new List<ushort> ();
            foreach (var t in opoly.Triangles) {
                foreach (var p in t.Points) {
                    tris.Add (pointToVertex[p.VertexCode]);
                }
            }

            var vSize = Marshal.SizeOf (typeof (Vert));
            Debug.Assert (vSize == 12, $"Vert size is incorrect: {vSize}");
            var vertsSource = SCNGeometrySource.FromVertices (verts.ToArray ());

            SCNGeometryElement oelem;
            unsafe {
                var ntris = tris.Count;
                var atris = tris.ToArray ();
                fixed (ushort* ptris = atris) {
                    var oelemData = NSData.FromBytes ((IntPtr)ptris, (nuint)(ntris * 2));
                    oelem = SCNGeometryElement.FromData (oelemData, SCNGeometryPrimitiveType.Triangles, ntris / 3, 2);
                }
            }

            Console.WriteLine (tris);

            var sources = new[] {
                vertsSource,
            };
            var elements = new[] {
                oelem,
            };

            var g = SCNGeometry.Create (sources, elements.ToArray ());
            return g;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 4)]
        struct Vert
        {
            public float X, Y, Z;
        }
    }
}
