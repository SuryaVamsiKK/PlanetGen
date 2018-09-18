using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanetFace
{
    Mesh mesh;
    int resolution;
    int temp_Resolution;
    int planetRadius;
    Vector3 localup;
    Vector3 axisA;
    Vector3 axisB;
    Vector3 ChunkPos;

    float sizeOfChunk;

    public PlanetFace(Mesh mesh, int resolution, Vector3 localup, int planetRadius, float sizeOfChunk, Vector3 ChunkPos)
    {
        this.ChunkPos = ChunkPos;
        this.sizeOfChunk = sizeOfChunk;
        this.mesh = mesh;
        this.resolution = resolution;
        this.localup = localup;
        this.planetRadius = planetRadius;

        axisA = new Vector3(localup.y, localup.z, localup.x);
        axisB = Vector3.Cross(localup, axisA);
    }

    public void ConstructMesh()
    {
        temp_Resolution = (resolution - 1) / 2;
        temp_Resolution += 1;
        //temp_Resolution = resolution; // for full
        Vector3[] vertices = new Vector3[temp_Resolution * temp_Resolution];
        int[] triangles = new int[(temp_Resolution - 1) * (temp_Resolution - 1) * 6];


        int triIndex = 0;
        for (int y = 0; y < temp_Resolution; y++)
        {
            for (int x = 0; x < temp_Resolution; x++)
            {
                int i = x + y * temp_Resolution;
                Vector2 percent = new Vector2(x, y) / (resolution - 1);
                Vector3 pointOnUnitCube = (localup + (percent.x - 0.5f) * sizeOfChunk * axisA + (percent.y - 0.5f) * sizeOfChunk * axisB) + ChunkPos;
                Vector3 pointOnUnitSphere = pointOnUnitCube.normalized * planetRadius;
               
                vertices[i] = pointOnUnitSphere;

                if (x != temp_Resolution - 1 && y != temp_Resolution - 1)
                {
                    triangles[triIndex] = i;
                    triangles[triIndex + 1] = i + temp_Resolution + 1;
                    triangles[triIndex + 2] = i + temp_Resolution;

                    triangles[triIndex + 3] = i;
                    triangles[triIndex + 4] = i + 1;
                    triangles[triIndex + 5] = i + temp_Resolution + 1;
                    triIndex += 6;
                }
            }
        }
        mesh.Clear();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
        //MeshHelper.Subdivide(mesh);
       //recalculateVerts();
    }

    void recalculateVerts()
    {
        Vector3[] vertss = mesh.vertices;
        for (int i = 0; i < vertss.Length; i++)
        {
            vertss[i] = vertss[i].normalized * planetRadius;
        }
        mesh.vertices = vertss;
    }
}
