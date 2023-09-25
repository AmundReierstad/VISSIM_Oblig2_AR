using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.IO;
using UnityEditor.Playables;
using UnityEngine.UIElements;

public class TriangleSurface : MonoBehaviour
{
    private MeshFilter _meshFilter;
    [SerializeField] TextAsset _textFileToRead;

    // Start is called before the first frame update
    private void Awake()
    {
        _meshFilter= GetComponent<MeshFilter>();
        using var stream = new StreamReader(new MemoryStream(_textFileToRead.bytes));
        using var reader = new StreamReader(stream.BaseStream);
        GeometryFromStream(reader);
    }

    private void GeometryFromStream(StreamReader reader)
    {
        if (reader == null)
        {
            throw new ArgumentNullException(nameof(reader));
        }

        int numIndices = int.Parse(Utilites.ReadUntil(reader, '\n').Trim());
        int numVertices = int.Parse(Utilites.ReadUntil(reader, '\n').Trim());

        List<int> indices = new List<int>();
        for (int i = 0; i < numIndices; i++)
        {
            indices.Add(int.Parse(Utilites.ReadUntil(reader, '\n').Trim()));
        }

        List<Vertex> vertices = new List<Vertex>();
        for (int i = 0; i < numVertices; i++)
        {
            vertices.Add(Vertex.LoadFromFile(reader));
        }

        List<int> neighbours = new List<int>();
        for (int i = 0; i < numIndices; i++)
        {
            neighbours.Add(int.Parse(Utilites.ReadUntil(reader, '\n').Trim()));
        }

        reader.Close();



        Mesh newMesh = new Mesh();

        newMesh.vertices = vertices.Select(v => v.Position).ToArray();
        ;
        newMesh.triangles = indices.ToArray();
        // Re-calculate normals and bounds
        newMesh.RecalculateNormals();
        newMesh.RecalculateBounds();

        _meshFilter.mesh = newMesh;
    }
}
