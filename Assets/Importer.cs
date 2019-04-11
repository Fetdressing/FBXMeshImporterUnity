using System;
using System.IO;
using System.Text;

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

using System.Runtime.InteropServices;

public class Importer : MonoBehaviour
{
    public string resourceFileName;

    public string resourceBinaryFileName;

    public struct TestStruct
    {
        public char myChar;

        public int GetSize()
        {
            int size = 0;
            //int size += Marshal.SizeOf(typeof(T));

            return size;
        }
    }

    #region CHAR
    public void Write()
    {
        string finalPath = System.IO.Path.Combine(Application.streamingAssetsPath, resourceFileName);

        if (System.IO.File.Exists(finalPath))
        {
            print("Opened file");

            StreamWriter sw = new StreamWriter(finalPath, false, Encoding.ASCII);
            sw.WriteLine("Bajs");

            sw.Close();
        }
    }

    public void Read()
    {
        string finalPath = System.IO.Path.Combine(Application.streamingAssetsPath, resourceFileName);

        if (System.IO.File.Exists(finalPath))
        {
            StreamReader sr = new StreamReader(finalPath, Encoding.ASCII);
            print(sr.ReadLine());

            sr.Close();
        }
    }
    #endregion

    public void WriteBinary()
    {
        string finalPath = System.IO.Path.Combine(Application.streamingAssetsPath, resourceFileName);

        if (System.IO.File.Exists(finalPath))
        {
            print("Opened file");

            FileStream fs = new FileStream(finalPath, FileMode.Open);

            TestStruct tStruct = new TestStruct();
            tStruct.myChar = 'L';
            byte[] byteArray = StructToBinary(tStruct);

            fs.Write(byteArray, 0, byteArray.Length);

            fs.Close();
        }
    }

    public void ReadBinary()
    {
        string finalPath = System.IO.Path.Combine(Application.streamingAssetsPath, resourceFileName);

        if (System.IO.File.Exists(finalPath))
        {
            FileStream fs = new FileStream(finalPath, FileMode.Open);

            byte[] byteArray = new byte[Marshal.SizeOf(typeof(TestStruct))];

            fs.Read(byteArray, 0, Marshal.SizeOf(typeof(TestStruct)));
            TestStruct testStruct = BinaryToStruct<TestStruct>(byteArray);

            print(testStruct.myChar);

            fs.Close();
        }
    }

    /// <summary>
    /// Convert structure to binary.
    /// </summary>
    /// <typeparam name="T">Type of structure.</typeparam>
    /// <param name="structure">Structure to convert.</param>
    /// <returns>Array of bytes.</returns>
    public static byte[] StructToBinary<T>(T structure) where T : struct
    {
        int size = Marshal.SizeOf(typeof(T));
        byte[] byteArray = new byte[size];
        System.IntPtr ptr = Marshal.AllocHGlobal(size);
        Marshal.StructureToPtr(structure, ptr, true);
        Marshal.Copy(ptr, byteArray, 0, size);
        Marshal.FreeHGlobal(ptr);
        return byteArray;
    }

    /// <summary>
    /// Convert structure array to binary.
    /// </summary>
    /// <typeparam name="T">Type of structure.</typeparam>
    /// <param name="structureArray">Structure array to convert.</param>
    /// <returns>Array of bytes.</returns>
    public static byte[] StructArrayToBinary<T>(T[] structureArray) where T : struct
    {
        int stride = Marshal.SizeOf(typeof(T));
        byte[] byteArray = new byte[stride * structureArray.Length];

        System.IntPtr ptr = Marshal.AllocHGlobal(stride);

        for (int i = 0; i < structureArray.Length; ++i)
        {
            Marshal.StructureToPtr(structureArray[i], ptr, true);
            Marshal.Copy(ptr, byteArray, i * stride, stride);
        }

        Marshal.FreeHGlobal(ptr);

        return byteArray;
    }

    /// <summary>
    /// Convert binary to structure.
    /// </summary>
    /// <typeparam name="T">Type of structure.</typeparam>
    /// <param name="byteArray">Array of bytes to convert.</param>
    /// <returns>The structure.</returns>
    public static T BinaryToStruct<T>(byte[] byteArray) where T : struct
    {
        int size = Marshal.SizeOf(typeof(T));
        System.IntPtr ptr = Marshal.AllocHGlobal(size);
        Marshal.Copy(byteArray, 0, ptr, size);
        T structure = (T)Marshal.PtrToStructure(ptr, typeof(T));
        Marshal.FreeHGlobal(ptr);
        return structure;
    }

    /// <summary>
    /// Convert binary to structure array.
    /// </summary>
    /// <typeparam name="T">Type of structure.</typeparam>
    /// <param name="byteArray">Array of bytes to convert.</param>
    /// <returns>The structure array.</returns>
    public static T[] BinaryToStructArray<T>(byte[] byteArray) where T : struct
    {
        int stride = Marshal.SizeOf(typeof(T));
        T[] structureArray = new T[byteArray.Length / stride];

        System.IntPtr ptr = Marshal.AllocHGlobal(stride);

        for (int i = 0; i < structureArray.Length; ++i)
        {
            Marshal.Copy(byteArray, i * stride, ptr, stride);
            structureArray[i] = (T)Marshal.PtrToStructure(ptr, typeof(T));
        }

        Marshal.FreeHGlobal(ptr);

        return structureArray;
    }

    [CustomEditor(typeof(Importer))]
    public class TestOnInspector : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            Importer myScript = (Importer)target;
            if (GUILayout.Button("Write"))
            {
                myScript.Write();
            }

            if (GUILayout.Button("Read"))
            {
                myScript.Read();
            }

            if (GUILayout.Button("WriteBinary"))
            {
                myScript.WriteBinary();
            }

            if (GUILayout.Button("ReadBinary"))
            {
                myScript.ReadBinary();
            }
        }
    }
}
