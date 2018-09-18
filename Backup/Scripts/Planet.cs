using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Planet : MonoBehaviour {

    [Range(2,256)]
    public int res = 10;
    public Material mat;
    public int planetRadius = 1;
    public int values = 1;
    public int planePosX = 0;
    public int planePosY = 0;

    [SerializeField, HideInInspector]
    MeshFilter[,,] meshFilters;
    PlanetFace[,,] planetFaces;

    private void OnValidate()
    {
        Initialize();
        Gen();
    }

    void Initialize()
    {
        int rootedValue = Mathf.RoundToInt(Mathf.Sqrt(values));

        if (meshFilters == null || meshFilters.Length == 0)
        {
            meshFilters = new MeshFilter[6,rootedValue, rootedValue];
        }
        planetFaces = new PlanetFace[6,rootedValue, rootedValue];

        Vector3[] directions = { Vector3.up, Vector3.down, Vector3.left, Vector3.right, Vector3.forward, Vector3.back };

        for (int i = 0; i < 6; i++)
        {
            GameObject FaceGroup = new GameObject("FaceGroup_" + i);
            FaceGroup.transform.parent = this.transform;
            for (int a = 0; a < rootedValue; a++)
            {
                for (int b = 0; b < rootedValue; b++)
                {
                    if (meshFilters[i,a,b] == null) 
                    {
                        GameObject meshObj = new GameObject("mesh_"+a+"_"+b);
                        meshObj.transform.parent = FaceGroup.transform;

                        meshObj.AddComponent<MeshRenderer>().sharedMaterial = mat;
                        meshFilters[i,a,b] = meshObj.AddComponent<MeshFilter>();
                        meshFilters[i,a,b].sharedMesh = new Mesh();
                    }
                    planetFaces[i,a,b] = new PlanetFace(meshFilters[i,a,b].sharedMesh, res, directions[i], planetRadius, values, a, b);
                }                
            }         
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
