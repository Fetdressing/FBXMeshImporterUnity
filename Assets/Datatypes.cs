using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class for handling storage of local data.
/// </summary>
public class Datatypes
{
    /// <summary>
    /// DO NOT CREATE THIS OBJECT DIRECTLY. Only made for inheriting, I would of made it abstract if it weren't for those pesky kids (Wanted to set the isCreated variable)! 
    /// </summary>
    public class CreateableObject
    {
        protected bool isCreated = false;

        /// <summary>
        /// Creates a unity project out of the datatype.
        /// </summary>
        /// <returns>Returns the created unity object.</returns>
        public virtual UnityEngine.Object Create()
        {
            isCreated = true;
            return null;
        }
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
        public List<MorphTarget> morphTargets = new List<MorphTarget>();

        /// <summary>
        /// Bare minimum constructor for testing.
        /// </summary>
        public Mesh(Transform transform, Vector3[] positions, int[] triangles, List<MorphTarget> morphTargets = null)
        {
            this.transform = transform;

            for (int i = 0; i < positions.Length; i++)
            {
                Vertex newVertex = new Vertex(positions[i]);
                vertices.Add(newVertex);
            }

            for (int i = 0; i < triangles.Length; i++)
            {
                this.triangles.Add(triangles[i]);
            }

            this.morphTargets = morphTargets;
        }

        public override UnityEngine.Object Create()
        {
            // Create the unity transform object.
            GameObject transformOB = (UnityEngine.GameObject)transform.Create();

            // Create unity mesh components.
            UnityEngine.Mesh mesh = new UnityEngine.Mesh();
            MeshFilter meshFilter = transformOB.AddComponent<MeshFilter>();
            meshFilter.mesh = mesh;
            MeshRenderer meshRenderer = transformOB.AddComponent<MeshRenderer>();

            // Check so that we have a material.
            if (material != null)
            {
                meshRenderer.material = (UnityEngine.Material)material.Create();
            }
            else
            {
                Debug.Log("Missing material.");
            }

            // Add morph player to gameobject.
            MorphAnimPlayer morphAnimPlayer = transformOB.AddComponent<MorphAnimPlayer>();
            morphAnimPlayer.Set(morphTargets, meshFilter);

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

            // Check if we have normals, otherways generate them.
            if (normals != null && normals.Count == positions.Count)
            {
                mesh.normals = normals.ToArray();
            }

            // Check so that we have uvs.
            if (uvs != null && uvs.Count == positions.Count)
            {
                mesh.uv = uvs.ToArray();
            }

            mesh.triangles = triangles.ToArray();

            // mesh.RecalculateNormals();

            morphAnimPlayer.PlayMorph();

            return mesh;
        }
    }

    public class MorphTarget
    {
        public float interval = 1f;
        public List<Vector3> positions = new List<Vector3>();

        /// <summary>
        /// Constructor.
        /// </summary>
        public MorphTarget(Vector3[] positions, float interval = 1f)
        {
            for (int i = 0; i < positions.Length; i++)
            {
                this.positions.Add(positions[i]);
            }

            this.interval = interval;
        }
    }

    public class Vertex
    {
        public Vector3 position;
        public Vector3 normal;
        public Vector2 uvCoord;    
        
        /// <summary>
        /// Bare minimum constructor.
        /// </summary>
        public Vertex(Vector3 position)
        {
            this.position = position;
        }

        public Vertex(Vector3 position, Vector3 normal, Vector2 uvCoord)
        {
            this.position = position;
            this.normal = normal;
            this.uvCoord = uvCoord;
        }
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
