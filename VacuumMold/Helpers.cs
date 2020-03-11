using System;
using System.Numerics;

namespace VacuumMold
{
    public static class Helpers
    {
        public static Vector2 Xy (float x, float y) => new Vector2 ((float)x, (float)y);
        public static Vector2 Xy (nfloat x, nfloat y) => new Vector2 ((float)x, (float)y);
        public static Vector2 Xy (double x, double y) => new Vector2 ((float)x, (float)y);

        public static Vector3 Xyz (float x, float y, float z) => new Vector3 ((float)x, (float)y, z);
        public static Vector3 Xyz (nfloat x, nfloat y, nfloat z) => new Vector3 ((float)x, (float)y, (float)z);
        public static Vector3 Xyz (double x, double y, double z) => new Vector3 ((float)x, (float)y, (float)z);
        public static Vector3 Xyz (Vector2 xy, float z) => new Vector3 (xy.X, xy.Y, z);
    }
}
