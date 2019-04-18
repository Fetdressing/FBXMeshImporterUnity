using System;
using System.IO;
using System.Text;

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

using System.Runtime.InteropServices;

public class ImportDataTypes : MonoBehaviour
{
    public struct Scene
    {
        public Header header;

        public Body body;

        public struct Header
        {
            public int nrMeshes;
            public int nrLights;
            public int nrCameras;
        }

        public struct Body
        {
            public Mesh[] meshes;
            public Light[] lights;
        }

        public static Scene Read(FileStream fs)
        {
            Scene newObject = new Scene();

            newObject.header = ReadHeader(fs);
            newObject.body = ReadBody(fs, newObject.header);

            return newObject;
        }

        private static Header ReadHeader(FileStream fs)
        {
            Header header = new Header();

            byte[] byteArray = new byte[Marshal.SizeOf(typeof(Header))];

            fs.Read(byteArray, 0, Marshal.SizeOf(typeof(Header)));
            header = Importer.BinaryToStruct<Header>(byteArray);

            return header;
        }

        private static Body ReadBody(FileStream fs, Header header)
        {
            Body body = new Body();

            body.meshes = ReadMeshes(fs, header.nrMeshes);
            body.lights = ReadLights(fs, header.nrLights);

            return body;
        }

        private static Mesh[] ReadMeshes(FileStream fs, int nrObjects)
        {
            Mesh[] objectArray = new Mesh[nrObjects];

            for (int i = 0; i < nrObjects; i++)
            {
                Mesh newObject = Mesh.Read(fs);
                objectArray[i] = newObject;
            }

            return objectArray;
        }

        private static Light[] ReadLights(FileStream fs, int nrObjects)
        {
            Light[] objectArray = new Light[nrObjects];

            for (int i = 0; i < nrObjects; i++)
            {
                Light newObject = Light.Read(fs);
                objectArray[i] = newObject;
            }

            return objectArray;
        }
    }

    public struct Mesh
    {
        public Header header;

        public Body body;

        public struct Header
        {
            public Transform transform;
            public Material material;
            public int nrVertices;
        }

        public struct Body
        {
            public Vertex[] vertices;
        }

        public static Mesh Read(FileStream fs)
        {
            Mesh mesh = new Mesh();

            mesh.header = ReadHeader(fs);
            mesh.body = ReadBody(fs, mesh.header);

            return mesh;
        }

        private static Header ReadHeader(FileStream fs)
        {
            Header header = new Header();

            // Read material.
            header.material = new Material();
            header.material = Material.Read(fs);

            byte[] byteArray = new byte[Marshal.SizeOf(typeof(Header))];

            fs.Read(byteArray, 0, Marshal.SizeOf(typeof(Header)));
            header = Importer.BinaryToStruct<Header>(byteArray);

            return header;
        }

        private static Body ReadBody(FileStream fs, Header header)
        {
            Body body = new Body();

            body.vertices = ReadVertices(fs, header.nrVertices);

            return body;
        }

        private static Vertex[] ReadVertices(FileStream fs, int nrVertices)
        {
            Vertex[] vertices = new Vertex[nrVertices];

            for (int i = 0; i < nrVertices; i++)
            {
                byte[] byteArray = new byte[Marshal.SizeOf(typeof(Vertex))];

                fs.Read(byteArray, 0, Marshal.SizeOf(typeof(Vertex)));
                Vertex vertex = Importer.BinaryToStruct<Vertex>(byteArray);

                vertices[i] = vertex;
            }

            return vertices;
        }
    }

    public struct Light
    {
        public Header header;

        public struct Header
        {
            public Transform transform;
            public float range;
            public float intensity;
        }

        public static Light Read(FileStream fs)
        {
            Light light = new Light();

            light.header = ReadHeader(fs);

            return light;
        }

        private static Header ReadHeader(FileStream fs)
        {
            Header header = new Header();

            byte[] byteArray = new byte[Marshal.SizeOf(typeof(Header))];

            fs.Read(byteArray, 0, Marshal.SizeOf(typeof(Header)));
            header = Importer.BinaryToStruct<Header>(byteArray);

            return header;
        }
    }

    public struct Material
    {
        public Header header;

        public struct Header
        {
            public Text diffuseName;
            public Vec4 color;
        }

        public static Material Read(FileStream fs)
        {
            Material newObject = new Material();

            newObject.header = ReadHeader(fs);

            return newObject;
        }

        private static Header ReadHeader(FileStream fs)
        {
            Header header = new Header();

            // Read names.
            header.diffuseName = new Text();
            header.diffuseName = Text.Read(fs);

            byte[] byteArray = new byte[Marshal.SizeOf(typeof(Header))];

            fs.Read(byteArray, 0, Marshal.SizeOf(typeof(Header)));
            header = Importer.BinaryToStruct<Header>(byteArray);

            return header;
        }
    }

    public struct Text
    {
        public Header header;

        public Body body;

        public struct Header
        {
            public int nrChars;
        }

        public struct Body
        {
            public char[] charArray;
        }

        public static Text Read(FileStream fs)
        {
            Text text = new Text();

            text.header = ReadHeader(fs);
            text.body = ReadBody(fs, text.header);

            return text;
        }

        private static Header ReadHeader(FileStream fs)
        {
            Header header = new Header();

            byte[] byteArray = new byte[Marshal.SizeOf(typeof(Header))];

            fs.Read(byteArray, 0, Marshal.SizeOf(typeof(Header)));
            header = Importer.BinaryToStruct<Header>(byteArray);

            return header;
        }

        private static Body ReadBody(FileStream fs, Header header)
        {
            Body body = new Body();

            body.charArray = ReadChars(fs, header.nrChars);

            return body;
        }

        private static char[] ReadChars(FileStream fs, int nrObjects)
        {
            char[] objectArray = new char[nrObjects];

            for (int i = 0; i < nrObjects; i++)
            {
                byte[] byteArray = new byte[Marshal.SizeOf(typeof(char))];

                fs.Read(byteArray, 0, Marshal.SizeOf(typeof(char)));
                char newObject = Importer.BinaryToStruct<char>(byteArray);

                objectArray[i] = newObject;
            }

            return objectArray;
        }
    }

    public struct Transform
    {
        public Vec3 position;
        public Vec3 rotation;
        public Vec3 scale;
    }

    public struct Vertex
    {
        public Vec3 position;
        public Vec3 normal;
        public Vec2 uv;
    }

    public struct Vec4
    {
        public double x;
        public double y;
        public double z;
        public double w;
    }

    public struct Vec3
    {
        public double x;
        public double y;
        public double z;
    }

    public struct Vec2
    {
        public double x;
        public double y;
    }
}
