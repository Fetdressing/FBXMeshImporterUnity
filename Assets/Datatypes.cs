using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class for handling storage of local data.
/// </summary>
public class Datatypes
{
    public abstract class CreateableObject
    {
        /// <summary>
        /// Creates a unity project out of the datatype.
        /// </summary>
        /// <returns>Returns the created unity object.</returns>
        public abstract UnityEngine.Object Create();
    }

    public class Scene : CreateableObject
    {
        public List<Mesh> meshes = new List<Mesh>();
        public List<Light> lights = new List<Light>();
        public List<Camera> cameras = new List<Camera>();
        // Store materials here or keep them per mesh?

        public override Object Create()
        {
            for (int i = 0; i < meshes.Count; i++)
            {
                meshes[i].Create();
            }

            for (int i = 0; i < lights.Count; i++)
            {
                lights[i].Create();
            }

            for (int i = 0; i < cameras.Count; i++)
            {
                cameras[i].Create();
            }

            // Let's not return scenes ;)
            return null;
        }
    }

    public class Transform : CreateableObject
    {
        public string name;
        public Vector3 position;
        public Vector3 rotation;
        public Vector3 scale;

        public override UnityEngine.Object Create()
        {
            GameObject go = new GameObject();
            go.name = name;

            go.transform.position = position;
            go.transform.rotation = Quaternion.Euler(rotation);
            go.transform.localScale = scale;

            return go;
        }
    }

    #region Mesh
    public class Mesh : CreateableObject
    {
        public Transform transform;
        public List<Vertex> vertices = new List<Vertex>();
        public List<int> triangles = new List<int>();
        public Material material;

        public override UnityEngine.Object Create()
        {
            // Create the unity transform object.
            GameObject transformOB = (UnityEngine.GameObject)transform.Create();

            // Create unity mesh components.
            UnityEngine.Mesh mesh = new UnityEngine.Mesh();
            MeshFilter meshFilter = transformOB.AddComponent<MeshFilter>();
            meshFilter.mesh = mesh;
            MeshRenderer meshRenderer = transformOB.AddComponent<MeshRenderer>();
            meshRenderer.material = (UnityEngine.Material)material.Create();

            // --Separate the vertices values into separate lists with its members.
            List<Vector3> positions = new List<Vector3>();
            List<Vector3> normals = new List<Vector3>();
            List<Vector2> uvs = new List<Vector2>();

            for (int i = 0; i < vertices.Count; i++)
            {
                positions.Add(vertices[i].position);
                normals.Add(vertices[i].normal);
                uvs.Add(vertices[i].uvCoord);
            }
            // --Separate the vertices values into separate lists with its members.

            mesh.vertices = positions.ToArray();
            mesh.normals = normals.ToArray();
            mesh.uv = uvs.ToArray();

            mesh.triangles = triangles.ToArray();
            //mesh.RecalculateNormals();

            return mesh;
        }
    }

    public class Vertex
    {
        public Vector3 position;
        public Vector3 normal;
        public Vector2 uvCoord;        
    }

    public class Material : CreateableObject
    {
        public string name;
        public Vector4 color;
        public Texture2D texture;

        public override UnityEngine.Object Create()
        {
            UnityEngine.Material newMat = SceneCreator.GetNewDefaultMaterial();
            newMat.name = name;

            if (texture != null)
            {
                newMat.mainTexture = texture;
            }

            newMat.color = color;

            return newMat;
        }
    }
    #endregion

    public class Light : CreateableObject
    {
        public Transform transform;
        public float range;
        public float intensity;

        public override UnityEngine.Object Create()
        {
            // Create the unity transform object.
            GameObject transformOB = (UnityEngine.GameObject)transform.Create();

            // Create the unity light.
            UnityEngine.Light light = transformOB.AddComponent<UnityEngine.Light>();
            light.range = range;
            light.intensity = intensity;

            return light;
        }
    }

    public class Camera : CreateableObject
    {
        public Transform transform;
        public float fov;

        public override UnityEngine.Object Create()
        {
            // Create the unity transform object.
            GameObject transformOB = (UnityEngine.GameObject)transform.Create();

            // Create the unity camera object.
            UnityEngine.Camera camera = transformOB.AddComponent<UnityEngine.Camera>();
            camera.fieldOfView = fov;

            return camera;
        }
    }
}
