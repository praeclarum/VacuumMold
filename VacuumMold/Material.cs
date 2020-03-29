using System;
using System.Numerics;
using AppKit;
using SceneKit;

using static VacuumMold.Helpers;

namespace VacuumMold
{
    public class Material
    {
        public readonly SCNMaterial SCNMaterial;

        public Material (SCNMaterial sceneMaterial)
        {
            SCNMaterial = sceneMaterial;
        }

        public Material (Vector4 color, double roughness, double metalness)
        {
            SCNMaterial = SCNMaterial.Create ();
            SCNMaterial.LightingModelName = SCNLightingModel.PhysicallyBased;
            SCNMaterial.Diffuse.ContentColor = NSColor.FromRgba (color.X, color.Y, color.Z, color.W);
            SCNMaterial.Roughness.ContentColor = NSColor.FromWhite ((float)roughness, 1);
            SCNMaterial.Metalness.ContentColor = NSColor.FromWhite ((float)metalness, 1);
        }
    }

    //struct Person
    //{
    //    int Age;
    //}
    // int numberOfPeople = 0;
    // printf("How many people?");
    // scanf("%d", &numberOfPeople);
    // Person *people = malloc(numberOfPeople * sizeof(Person));
    // for (int i = 0; i < numberofPeople; i++) {
    //   // printf scanf stuff &person[i].Age
    // }
    // int sum;
    // for (int i = 0; i < numberofPeople; i++) {
    //   sum += person[i].Age
    // }
    // int avgAge = sum / numberOfPeople;

    public static class Materials
    {
        public static Material Plastic (Vector4 color, double roughness = 0.5)
        {
            return new Material (color, roughness, 0);

        }

        public static Material Gold (double roughness = 0.5) =>
            new Material (Xyzw (1.000, 0.766, 0.336, 1), roughness, 1);
        public static Material Silver (double roughness = 0.5) =>
            new Material (Xyzw (0.972, 0.960, 0.915, 1), roughness, 1);
        public static Material Copper (double roughness = 0.5) =>
            new Material (Xyzw (0.955, 0.637, 0.538, 1), roughness, 1);
    }
}
