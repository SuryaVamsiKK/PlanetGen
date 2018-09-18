using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Planet : MonoBehaviour {

    [Range(2,256)]
    public int res = 10;
    public Material mat;
    public int planetRadius = 1;
    public float sizeOfChunk = 2;
    public Vector3 ChunkPos = new Vector3(0, 0, 0);
    public int numOfChunks = 6;

    [SerializeField, HideInInspector]
    MeshFilter[] meshFilters;
    PlanetFace[] planetFaces;

    private void OnValidate()
    {
        Initialize();
        Gen();
    }

    void Initialize()
    {
        if (res % 2 != 0)
        {
            res += 1;
        }

        if (meshFilters == null || meshFilters.Length == 0)
        {
            meshFilters = new MeshFilter[numOfChunks];
        }
        planetFaces = new PlanetFace[numOfChunks];

        Vector3[] directions = { Vector3.up, Vector3.down, Vector3.left, Vector3.right, Vector3.forward, Vector3.back };

        for (int i = 0; i < numOfChunks; i++)
        {
            if (meshFilters[i] == null)
            {
                GameObject meshObj = new GameObject("mesh");
                GameObject FaceGroup = new GameObject("FaceGroup_"+i);
                FaceGroup.transform.parent = this.transform;
                meshObj.transform.parent = FaceGroup.transform;

                meshObj.AddComponent<MeshRenderer>().sharedMaterial = mat;
                meshFilters[i] = meshObj.AddComponent<MeshFilter>();
                meshFilters[i].sharedMesh = new Mesh();
            }

            planetFaces[i] = new PlanetFace(meshFilters[i].sharedMesh, res + 1, directions[i], planetRadius, sizeOfChunk, ChunkPos);
        }
    }

    void Gen()
    {
        foreach (PlanetFace face in planetFaces)
        {
            face.ConstructMesh();
        }
    }
}
