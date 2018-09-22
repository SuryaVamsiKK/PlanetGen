using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class tester : MonoBehaviour {

    public Material mat;
    public int temp_Resolution;
    public int planetRadius;
    public float sizeOfChunk = 1;
    //[HideInInspector]
    public bool gen = false;


    Vector3 ChunkPos = new Vector3(0, 0, 0);
    Vector3 localup = Vector3.up;
    GameObject meshObj;
    public Types Type;
    LODs currentLOD = LODs.LOD1;
    [HideInInspector]
    public float treshhold = 0.5f;

    public GameObject player;
    [HideInInspector]
    public float subdivx;
    [HideInInspector]
    public float subdivy;

    void OnValidate ()
    {
        meshObj = this.gameObject;

        if (meshObj.GetComponent<MeshFilter>() == null)
        {
            meshObj.AddComponent<MeshFilter>();
            meshObj.GetComponent<MeshFilter>().sharedMesh = new Mesh();
        }
        if (meshObj.GetComponent<MeshRenderer>() == null)
        {
            meshObj.AddComponent<MeshRenderer>().sharedMaterial = mat;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(player.transform.position, 0.1f);
    }

    void Update()
    {
        if (Vector3.Distance(player.transform.position, meshObj.transform.position) < treshhold)
        {
            currentLOD = LODs.LOD2;
        }
        else
        {
            currentLOD = LODs.LOD1;
            if (gen == true)
            {
                ConstructMesh(this.GetComponent<MeshFilter>().sharedMesh, subdivx, subdivy, sizeOfChunk);
                gen = false;
            }
        }

        if (currentLOD == LODs.LOD2)
        {
            Destroy(meshObj.GetComponent<MeshRenderer>());
            Destroy(meshObj.GetComponent<MeshFilter>());
            GameObject[,] subs = new GameObject[2, 2];
            if (meshObj.transform.childCount == 0)
            {
                for (int i = 0; i < 2; i++)
                {
                    for (int j = 0; j < 2; j++)
                    {
                        subs[i, j] = new GameObject();
                        subs[i, j].transform.parent = meshObj.transform;
                        subs[i, j].AddComponent<MeshFilter>();
                        subs[i, j].GetComponent<MeshFilter>().sharedMesh = new Mesh();
                        if (subs[i, j].GetComponent<MeshRenderer>() == null)
                        {
                            subs[i, j].AddComponent<MeshRenderer>().sharedMaterial = mat;
                        }
                        subs[i, j].AddComponent<tester>().treshhold = treshhold/2;
                        subs[i, j].GetComponent<tester>().player = player;
                        subs[i, j].GetComponent<tester>().planetRadius = planetRadius;
                        subs[i, j].GetComponent<tester>().temp_Resolution = temp_Resolution;
                        subs[i, j].GetComponent<tester>().mat = mat;
                        subs[i, j].GetComponent<tester>().sizeOfChunk = sizeOfChunk/2;
                        subs[i, j].GetComponent<tester>().Type = Type;
                    }
                }

                float subdiv = (temp_Resolution / 2) - 0.5f;


                subs[0, 1].GetComponent<tester>().subdivx = -subdiv;
                subs[1, 0].GetComponent<tester>().subdivx = subdiv;
                subs[0, 0].GetComponent<tester>().subdivx = -subdiv;
                subs[1, 1].GetComponent<tester>().subdivx = subdiv;

                subs[0, 1].GetComponent<tester>().subdivy = subdiv;
                subs[1, 0].GetComponent<tester>().subdivy = -subdiv;
                subs[0, 0].GetComponent<tester>().subdivy = -subdiv;
                subs[1, 1].GetComponent<tester>().subdivy = subdiv;

                for (int i = 0; i < 2; i++)
                {
                    for (int j = 0; j < 2; j++)
                    {
                        subs[i, j].GetComponent<tester>().gen = true;
                    }
                }

                //ConstructMesh(subs[0, 1].GetComponent<MeshFilter>().sharedMesh, -subdiv, subdiv, sizeOfChunk / 2);
                //ConstructMesh(subs[1, 0].GetComponent<MeshFilter>().sharedMesh, subdiv, -subdiv, sizeOfChunk / 2);
                //ConstructMesh(subs[0, 0].GetComponent<MeshFilter>().sharedMesh, -subdiv, -subdiv, sizeOfChunk / 2);
                //ConstructMesh(subs[1, 1].GetComponent<MeshFilter>().sharedMesh, subdiv, subdiv, sizeOfChunk / 2);
            }
        }
    }

    public void ConstructMesh(Mesh mesh, float xsub, float ysub, float CSize)
    {
        Vector3 axisA = new Vector3();
        Vector3 axisB = new Vector3();

        axisA = new Vector3(localup.y, localup.z, localup.x);
        axisB = Vector3.Cross(localup, axisA);

        Vector3[] vertices = new Vector3[temp_Resolution * temp_Resolution];
        int[] triangles = new int[(temp_Resolution - 1) * (temp_Resolution - 1) * 6];


        int triIndex = 0;
        for (int y = 0; y < temp_Resolution; y++)
        {
            for (int x = 0; x < temp_Resolution; x++)
            {
                int i = x + y * temp_Resolution;
                Vector2 percent = new Vector2(x + xsub, y + ysub) / (temp_Resolution - 1);
                Vector3 pointOnUnitCube = (localup + (percent.x - 0.5f) * CSize * axisA + (percent.y - 0.5f) * CSize * axisB) - new Vector3(0,1,0);
                Vector3 pointOnUnitSphere = pointOnUnitCube.normalized * planetRadius;

                if (Type == Types.Flat)
                {
                    vertices[i] = pointOnUnitCube;
                }

                if (Type == Types.Infalten)
                {
                    vertices[i] = pointOnUnitSphere;
                }

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

        Mesh tempmesh = new Mesh();

        //Bounds bounds = new Bounds(vertices[0], vertices[vertices.Length - 1]);
        //for (int i = 0; i < vertices.Length; i++)
        //{
        //    bounds.Encapsulate(vertices[i]);
        //}
        //this.transform.position = bounds.center;

        mesh.Clear();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
    }
}

public enum Types { Flat, Infalten };
public enum LODs { LOD1, LOD2, LOD3, LOD4};
