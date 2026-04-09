using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using UnityEditor;
using UnityEngine;

public sealed class WorldVerticesExtractorWindow : EditorWindow
{
    [MenuItem("Tools/Map Collider/1) Extract World Vertices")]
    public static void Open()
    {
        WorldVerticesExtractorWindow Window = GetWindow<WorldVerticesExtractorWindow>();
        Window.titleContent = new GUIContent("World Vertices");
        Window.Show();
    }

    GameObject TargetObject;
    bool IncludeChildren = false;
    bool PreferMeshCollider = true;
    bool UseSharedMesh = true;

    bool Deduplicate = true;
    float DuplicateEpsilon = 1e-5f;

    bool CopyAsDbVector3Lines = true;
    Vector2 Scroll;

    void OnGUI()
    {
        Scroll = EditorGUILayout.BeginScrollView(Scroll);

        EditorGUILayout.LabelField("Target", EditorStyles.boldLabel);
        TargetObject = (GameObject)EditorGUILayout.ObjectField("GameObject", TargetObject, typeof(GameObject), true);
        IncludeChildren = EditorGUILayout.Toggle("Include Children", IncludeChildren);

        EditorGUILayout.Space(6);

        EditorGUILayout.LabelField("Mesh Source", EditorStyles.boldLabel);
        PreferMeshCollider = EditorGUILayout.Toggle("Prefer MeshCollider", PreferMeshCollider);
        UseSharedMesh = EditorGUILayout.Toggle("Use Shared Mesh", UseSharedMesh);

        EditorGUILayout.Space(6);

        EditorGUILayout.LabelField("Output", EditorStyles.boldLabel);
        Deduplicate = EditorGUILayout.Toggle("Deduplicate", Deduplicate);
        using (new EditorGUI.DisabledScope(Deduplicate == false))
        {
            DuplicateEpsilon = EditorGUILayout.FloatField("Duplicate Epsilon", DuplicateEpsilon);
        }
        CopyAsDbVector3Lines = EditorGUILayout.Toggle("Copy As DbVector3 Lines", CopyAsDbVector3Lines);

        EditorGUILayout.Space(10);

        if (GUILayout.Button("Copy World Vertices To Clipboard"))
        {
            try
            {
                if (TargetObject == null) throw new Exception("No target GameObject selected.");

                List<Vector3> WorldVertices = ExtractWorldVertices(TargetObject, IncludeChildren, PreferMeshCollider, UseSharedMesh);

                if (Deduplicate) WorldVertices = DeduplicateVertices(WorldVertices, DuplicateEpsilon);

                string Text = CopyAsDbVector3Lines ? FormatAsDbVector3Lines(WorldVertices) : FormatAsCsvLines(WorldVertices);
                EditorGUIUtility.systemCopyBuffer = Text;

                Debug.Log($"Copied {WorldVertices.Count} world vertices to clipboard.");
            }
            catch (Exception Exception)
            {
                Debug.LogError(Exception);
            }
        }

        EditorGUILayout.EndScrollView();
    }

    static List<Vector3> ExtractWorldVertices(GameObject RootObject, bool IncludeChildren, bool PreferMeshCollider, bool UseSharedMesh)
    {
        List<(Mesh Mesh, Transform Transform)> Meshes = new List<(Mesh, Transform)>();

        void TryAdd(GameObject GameObject)
        {
            Mesh Mesh = null;

            if (PreferMeshCollider)
            {
                MeshCollider MeshCollider = GameObject.GetComponent<MeshCollider>();
                if (MeshCollider != null) Mesh = MeshCollider.sharedMesh;
            }

            if (Mesh == null)
            {
                MeshFilter MeshFilter = GameObject.GetComponent<MeshFilter>();
                if (MeshFilter != null) Mesh = UseSharedMesh ? MeshFilter.sharedMesh : MeshFilter.mesh;
            }

            if (Mesh != null) Meshes.Add((Mesh, GameObject.transform));
        }

        TryAdd(RootObject);

        if (IncludeChildren)
        {
            Transform[] Children = RootObject.GetComponentsInChildren<Transform>(true);
            for (int Index = 0; Index < Children.Length; Index++)
            {
                Transform Child = Children[Index];
                if (Child == RootObject.transform) continue;
                TryAdd(Child.gameObject);
            }
        }

        if (Meshes.Count == 0) throw new Exception("No MeshFilter or MeshCollider found on target (or children).");

        List<Vector3> WorldVertices = new List<Vector3>();

        for (int MeshIndex = 0; MeshIndex < Meshes.Count; MeshIndex++)
        {
            (Mesh Mesh, Transform Transform) Entry = Meshes[MeshIndex];
            Vector3[] LocalVertices = Entry.Mesh.vertices;

            for (int VertexIndex = 0; VertexIndex < LocalVertices.Length; VertexIndex++)
            {
                WorldVertices.Add(Entry.Transform.TransformPoint(LocalVertices[VertexIndex]));
            }
        }

        if (WorldVertices.Count < 4) throw new Exception("Mesh has fewer than 4 vertices.");
        return WorldVertices;
    }

    static List<Vector3> DeduplicateVertices(List<Vector3> Vertices, float Epsilon)
    {
        List<Vector3> Result = new List<Vector3>();
        float EpsilonSq = Epsilon * Epsilon;

        for (int IndexA = 0; IndexA < Vertices.Count; IndexA++)
        {
            Vector3 Candidate = Vertices[IndexA];
            bool IsDuplicate = false;

            for (int IndexB = 0; IndexB < Result.Count; IndexB++)
            {
                if ((Candidate - Result[IndexB]).sqrMagnitude <= EpsilonSq)
                {
                    IsDuplicate = true;
                    break;
                }
            }

            if (IsDuplicate == false) Result.Add(Candidate);
        }

        return Result;
    }

    static string FloatText(float Value)
    {
        string Text = Value.ToString("R", CultureInfo.InvariantCulture);
        if (Text.IndexOf('.') < 0 && Text.IndexOf('e') < 0 && Text.IndexOf('E') < 0) Text += ".0";
        return Text;
    }

    static string FormatAsDbVector3Lines(List<Vector3> Vertices)
    {
        StringBuilder Builder = new StringBuilder();
        for (int Index = 0; Index < Vertices.Count; Index++)
        {
            Vector3 V = Vertices[Index];
            Builder.Append("DbVector3 { x: ");
            Builder.Append(FloatText(V.x));
            Builder.Append(", y: ");
            Builder.Append(FloatText(V.y));
            Builder.Append(", z: ");
            Builder.Append(FloatText(V.z));
            Builder.Append(" },");
            Builder.Append('\n');
        }
        return Builder.ToString();
    }

    static string FormatAsCsvLines(List<Vector3> Vertices)
    {
        StringBuilder Builder = new StringBuilder();
        for (int Index = 0; Index < Vertices.Count; Index++)
        {
            Vector3 V = Vertices[Index];
            Builder.Append(FloatText(V.x));
            Builder.Append(',');
            Builder.Append(FloatText(V.y));
            Builder.Append(',');
            Builder.Append(FloatText(V.z));
            Builder.Append('\n');
        }
        return Builder.ToString();
    }
}