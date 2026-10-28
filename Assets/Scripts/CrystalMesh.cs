using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshCollider))]
public class CrystalMesh : MonoBehaviour
{
    [Header("Crystal Shape")]
    [SerializeField] private int sides = 6;
    [SerializeField] private float height = 2f;
    [SerializeField] private float bottomRadius = 0.38f;
    [SerializeField] private float widestRadius = 0.58f;
    [SerializeField] private float upperRadius = 0.42f;

    private void Awake()
    {
        GenerateCrystal();
    }

    private void GenerateCrystal()
    {
        Mesh mesh = new Mesh();
        mesh.name = "AccessCrystalMesh";

        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();

        float bottomY = -height * 0.5f;
        float topY = height * 0.5f;

        // Four important levels:
        // Bottom point
        // Lower ring
        // Middle/widest ring
        // Upper ring
        // Top point

        Vector3 bottomPoint = new Vector3(0f, bottomY, 0f);
        Vector3 topPoint = new Vector3(0f, topY, 0f);

        // Bottom point -> lower ring
        for (int i = 0; i < sides; i++)
        {
            int next = (i + 1) % sides;

            Vector3 a = bottomPoint;
            Vector3 b = GetRingPoint(i, bottomY + height * 0.20f, bottomRadius);
            Vector3 c = GetRingPoint(next, bottomY + height * 0.20f, bottomRadius);

            AddTriangle(vertices, triangles, a, c, b);
        }

        // Lower ring -> widest ring
        for (int i = 0; i < sides; i++)
        {
            int next = (i + 1) % sides;

            Vector3 a = GetRingPoint(i, bottomY + height * 0.20f, bottomRadius);
            Vector3 b = GetRingPoint(next, bottomY + height * 0.20f, bottomRadius);

            Vector3 c = GetRingPoint(
                next,
                bottomY + height * 0.52f,
                widestRadius
            );

            Vector3 d = GetRingPoint(
                i,
                bottomY + height * 0.52f,
                widestRadius
            );

            AddTriangle(vertices, triangles, a, b, c);
            AddTriangle(vertices, triangles, a, c, d);
        }

        // Widest ring -> upper ring
        for (int i = 0; i < sides; i++)
        {
            int next = (i + 1) % sides;

            Vector3 a = GetRingPoint(
                i,
                bottomY + height * 0.52f,
                widestRadius
            );

            Vector3 b = GetRingPoint(
                next,
                bottomY + height * 0.52f,
                widestRadius
            );

            Vector3 c = GetRingPoint(
                next,
                bottomY + height * 0.78f,
                upperRadius
            );

            Vector3 d = GetRingPoint(
                i,
                bottomY + height * 0.78f,
                upperRadius
            );

            AddTriangle(vertices, triangles, a, b, c);
            AddTriangle(vertices, triangles, a, c, d);
        }

        // Upper ring -> top point
        for (int i = 0; i < sides; i++)
        {
            int next = (i + 1) % sides;

            Vector3 a = GetRingPoint(
                i,
                bottomY + height * 0.78f,
                upperRadius
            );

            Vector3 b = GetRingPoint(
                next,
                bottomY + height * 0.78f,
                upperRadius
            );

            Vector3 c = topPoint;

            AddTriangle(vertices, triangles, a, b, c);
        }

        mesh.SetVertices(vertices);
        mesh.SetTriangles(triangles, 0);

        // Because every face has its own vertices,
        // RecalculateNormals gives the crystal sharp/faceted faces.
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();

        MeshFilter meshFilter = GetComponent<MeshFilter>();
        meshFilter.sharedMesh = mesh;

        MeshCollider meshCollider = GetComponent<MeshCollider>();
        meshCollider.sharedMesh = null;
        meshCollider.sharedMesh = mesh;
    }

    private Vector3 GetRingPoint(
        int index,
        float y,
        float radius
    )
    {
        float angle = (Mathf.PI * 2f * index) / sides;

        return new Vector3(
            Mathf.Cos(angle) * radius,
            y,
            Mathf.Sin(angle) * radius
        );
    }

    private void AddTriangle(
        List<Vector3> vertices,
        List<int> triangles,
        Vector3 a,
        Vector3 b,
        Vector3 c
    )
    {
        int startIndex = vertices.Count;

        vertices.Add(a);
        vertices.Add(b);
        vertices.Add(c);

        triangles.Add(startIndex);
        triangles.Add(startIndex + 1);
        triangles.Add(startIndex + 2);
    }
}