using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneCreator : MonoBehaviour
{
    public Material defaultMaterial;

    // Singleton.
    private static SceneCreator instance;
    public static SceneCreator Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<SceneCreator>();
            }

            return instance;
        }
    }

    public static Material GetNewDefaultMaterial()
    {
        Material newMaterial = Object.Instantiate(Instance.defaultMaterial);
        return newMaterial;
    }

    private void Start()
    {
        Datatypes.Transform tra = new Datatypes.Transform();
        tra.position = Vector3.zero;
        tra.rotation = Vector3.zero;
        tra.scale = Vector3.one;
        tra.name = "Stipotele";

        Datatypes.Light light = new Datatypes.Light();
        light.intensity = 10;
        light.range = 25;
        light.transform = tra;
        light.Create();

        // Basic Quad.
        List<Vector3> vertices = new List<Vector3>();

        vertices.Add(new Vector3(0, 0, 0));
        vertices.Add(new Vector3(1, 0, 0));
        vertices.Add(new Vector3(0, 1, 0));
        vertices.Add(new Vector3(1, 1, 0));

        List<int> tri = new List<int>();

        //  Lower left triangle.
        tri.Add(0);
        tri.Add(2);
        tri.Add(1);

        //  Upper right triangle.   
        tri.Add(2);
        tri.Add(3);
        tri.Add(1);
        // Basic Quad.

        //*************
        // Basic Quad morph position TEST.
        List<Vector3> verticesMorph = new List<Vector3>();

        verticesMorph.Add(new Vector3(0, 0, 0));
        verticesMorph.Add(new Vector3(2, 0, 0));
        verticesMorph.Add(new Vector3(0, 1, 0));
        verticesMorph.Add(new Vector3(2, 1, 0));
        Datatypes.MorphTarget morphTarget1 = new Datatypes.MorphTarget(verticesMorph.ToArray());
        Datatypes.MorphTarget morphTarget2 = new Datatypes.MorphTarget(vertices.ToArray());

        List<Datatypes.MorphTarget> morphTargets = new List<Datatypes.MorphTarget>();
        morphTargets.Add(morphTarget1);
        morphTargets.Add(morphTarget2);

        // TESTING MORPH
        Datatypes.Mesh mesh = new Datatypes.Mesh(tra, vertices.ToArray(), tri.ToArray(), morphTargets);
        mesh.Create();
        //*************

        CreateMesh(vertices, tri, defaultMaterial);
    }

    public void CreateMesh(List<Vector3> vertices, List<int> triangles, Material material, string name = "myMesh")
    {
        GameObject meshOB = new GameObject();
        meshOB.name = name;
        Mesh mesh = new Mesh();

        MeshFilter meshFilter = meshOB.AddComponent<MeshFilter>();
        meshFilter.mesh = mesh;

        MeshRenderer meshRenderer = meshOB.AddComponent<MeshRenderer>();
        meshRenderer.material = material;

        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.RecalculateNormals();
    }
}
