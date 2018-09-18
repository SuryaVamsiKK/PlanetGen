using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanetFace
{
    Mesh mesh;
    int resolution;
    int planetRadius;
    Vector3 localup;
    Vector3 axisA;
    Vector3 axisB;
    int values;
    float changeValX;
    float changeValY;

    public PlanetFace(Mesh mesh, int resolution, Vector3 localup, int planetRadius, int values, float changeValX, float changeValY)
    {
        this.mesh = mesh;
        this.values = values;
        this.resolution = resolution;
        this.localup = localup;
        this.planetRadius = planetRadius;
        this.changeValX = changeValX;
        this.changeValY = changeValY;

        axisA = new Vector3(localup.y, localup.z, localup.x);
        axisB = Vector3.Cross(localup, axisA);
    }

    public void ConstructMesh()
    {
        Vector3[] vertices = new Vector3[resolution * resolution];
        int[] triangles = new int[(resolution - 1) * (resolution - 1) * 6];

        int triIndex = 0;

        for (int y = 0; y < resolution; y++)
        {
            for (int x = 0; x < resolution; x++)
            {
                int i = x + y * resolution;
                Vector2 percent = new Vector2();
                percent = new Vector2(x + (resolution * changeValX), y + (resolution * changeValY)) / ((resolution * Mathf.RoundToInt(Mathf.Sqrt(values))) - 1);

                Vector3 pointOnUnitCube = localup + (percent.x - 0.5f) * 2 * axisA + (percent.y - 0.5f) * 2 * axisB;
                Vector3 pointOnUnitSphere = pointOnUnitCube.normalized * planetRadius;
                vertices[i] = pointOnUnitSphere;
                if (x == 0 && y == 0)
                {
                    Debug.Log(percent);
                }

                if (x != resolution - 1 && y != resolution - 1)
                {
                    triangles[triIndex] = i;
                    triangles[triIndex + 1] = i + resolution + 1;
                    triangles[triIndex + 2] = i + resolution;

                    triangles[triIndex + 3] = i;
                    triangles[triIndex + 4] = i + 1;
                    triangles[triIndex + 5] = i + resolution + 1;
                    triIndex += 6;
                }
            }
        }
        mesh.Clear();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
    }
}
