using System;
using System.Numerics;
using System.Linq;
using SceneKit;

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
                if (shape != null)
                    CreateGeometry (shape);
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

        static void CreateGeometry (Shape shape)
        {
            var shapePoints = shape.SamplePerimeter (1);
            var oshapePoints = new Box (new CoreGraphics.CGRect (0, 0, 2000, 1000)).SamplePerimeter(1);

            var poly = new Poly2Tri.Triangulation.Polygon.Polygon (
                from p in shapePoints
                select new Poly2Tri.Triangulation.Polygon.PolygonPoint (p.X, p.Y));

            var opoly = new Poly2Tri.Triangulation.Polygon.Polygon (
                from p in oshapePoints
                select new Poly2Tri.Triangulation.Polygon.PolygonPoint (p.X, p.Y));
            opoly.AddHole (poly);

            Poly2Tri.P2T.Triangulate (opoly);

            Console.WriteLine (poly.Triangles);
        }
    }
}
