using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using UnityEditor;
using UnityEngine;

public sealed class PlayerLocalVerticesExtractorWindow : EditorWindow
{
    [MenuItem("Tools/Player Collider/1) Extract Local Vertices")]
    public static void Open()
    {
        PlayerLocalVerticesExtractorWindow Window = GetWindow<PlayerLocalVerticesExtractorWindow>();
        Window.titleContent = new GUIContent("Player Local Vertices");
        Window.Show();
    }

    GameObject TargetObject;
    bool IncludeChildren = false;
    bool UseSharedMeshFallback = false;

    bool Deduplicate = true;
    float DuplicateEpsilon = 1e-5f;

    bool AutoSimplify = true;
    float SimplifyCellSize = 0.04f;
    bool EnforceMaxVertexCount = true;
    int MaxVertexCount = 350;

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
        UseSharedMeshFallback = EditorGUILayout.Toggle("Use SharedMesh Fallback", UseSharedMeshFallback);

        EditorGUILayout.Space(6);

        EditorGUILayout.LabelField("Output", EditorStyles.boldLabel);
        Deduplicate = EditorGUILayout.Toggle("Deduplicate", Deduplicate);
        using (new EditorGUI.DisabledScope(Deduplicate == false))
        {
            DuplicateEpsilon = EditorGUILayout.FloatField("Duplicate Epsilon", DuplicateEpsilon);
        }

        EditorGUILayout.Space(6);

        EditorGUILayout.LabelField("Auto Simplification", EditorStyles.boldLabel);
        AutoSimplify = EditorGUILayout.Toggle("Auto Simplify", AutoSimplify);
        using (new EditorGUI.DisabledScope(AutoSimplify == false))
        {
            SimplifyCellSize = EditorGUILayout.FloatField("Simplify Cell Size", SimplifyCellSize);
            EnforceMaxVertexCount = EditorGUILayout.Toggle("Enforce Max Vertex Count", EnforceMaxVertexCount);

            using (new EditorGUI.DisabledScope(EnforceMaxVertexCount == false))
            {
                MaxVertexCount = EditorGUILayout.IntField("Max Vertex Count", MaxVertexCount);
            }
        }

        EditorGUILayout.Space(6);

        CopyAsDbVector3Lines = EditorGUILayout.Toggle("Copy As DbVector3 Lines", CopyAsDbVector3Lines);

        EditorGUILayout.Space(10);

        if (GUILayout.Button("Copy Local Vertices To Clipboard"))
        {
            try
            {
                if (TargetObject == null) throw new Exception("No target GameObject selected.");

                List<Vector3> LocalVertices = ExtractLocalVertices(TargetObject, IncludeChildren, UseSharedMeshFallback);
                int OriginalCount = LocalVertices.Count;

                if (Deduplicate)
                {
                    LocalVertices = DeduplicateVertices(LocalVertices, DuplicateEpsilon);
                }

                int DeduplicatedCount = LocalVertices.Count;

                if (AutoSimplify)
                {
                    LocalVertices = SimplifyVerticesByGrid(LocalVertices, SimplifyCellSize);

                    if (EnforceMaxVertexCount && MaxVertexCount > 0 && LocalVertices.Count > MaxVertexCount)
                    {
                        LocalVertices = ReduceToMaxVertexCount(LocalVertices, MaxVertexCount);
                    }
                }

                string Text = CopyAsDbVector3Lines ? FormatAsDbVector3Lines(LocalVertices) : FormatAsCsvLines(LocalVertices);
                EditorGUIUtility.systemCopyBuffer = Text;

                Debug.Log(
                    $"Copied {LocalVertices.Count} player local vertices to clipboard. " +
                    $"Original: {OriginalCount}, After Deduplicate: {DeduplicatedCount}, Final: {LocalVertices.Count}"
                );
            }
            catch (Exception Exception)
            {
                Debug.LogError(Exception);
            }
        }

        EditorGUILayout.EndScrollView();
    }

    static List<Vector3> ExtractLocalVertices(GameObject RootObject, bool IncludeChildren, bool UseSharedMeshFallback)
    {
        List<(SkinnedMeshRenderer Renderer, Mesh Mesh)> SkinnedMeshes = new List<(SkinnedMeshRenderer, Mesh)>();

        void TryAdd(GameObject GameObject)
        {
            SkinnedMeshRenderer SkinnedMeshRenderer = GameObject.GetComponent<SkinnedMeshRenderer>();
            if (SkinnedMeshRenderer == null) return;

            Mesh Mesh = new Mesh();
            Mesh.name = $"{GameObject.name}BakedSnapshot";
            SkinnedMeshRenderer.BakeMesh(Mesh);

            if (Mesh.vertexCount == 0)
            {
                UnityEngine.Object.DestroyImmediate(Mesh);

                if (UseSharedMeshFallback && SkinnedMeshRenderer.sharedMesh != null)
                {
                    Mesh = SkinnedMeshRenderer.sharedMesh;
                    SkinnedMeshes.Add((SkinnedMeshRenderer, Mesh));
                }

                return;
            }

            SkinnedMeshes.Add((SkinnedMeshRenderer, Mesh));
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

        if (SkinnedMeshes.Count == 0) throw new Exception("No SkinnedMeshRenderer found on target (or children).");

        List<Vector3> LocalVertices = new List<Vector3>();
        Matrix4x4 RootWorldToLocalMatrix = RootObject.transform.worldToLocalMatrix;

        for (int MeshIndex = 0; MeshIndex < SkinnedMeshes.Count; MeshIndex++)
        {
            (SkinnedMeshRenderer Renderer, Mesh Mesh) Entry = SkinnedMeshes[MeshIndex];
            Vector3[] RendererLocalVertices = Entry.Mesh.vertices;

            for (int VertexIndex = 0; VertexIndex < RendererLocalVertices.Length; VertexIndex++)
            {
                Vector3 WorldVertex = Entry.Renderer.transform.TransformPoint(RendererLocalVertices[VertexIndex]);
                Vector3 RootLocalVertex = RootWorldToLocalMatrix.MultiplyPoint3x4(WorldVertex);
                LocalVertices.Add(RootLocalVertex);
            }

            if (ReferenceEquals(Entry.Mesh, Entry.Renderer.sharedMesh) == false)
            {
                UnityEngine.Object.DestroyImmediate(Entry.Mesh);
            }
        }

        if (LocalVertices.Count < 4) throw new Exception("Skinned mesh has fewer than 4 vertices.");
        return LocalVertices;
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

    static List<Vector3> SimplifyVerticesByGrid(List<Vector3> Vertices, float CellSize)
    {
        if (Vertices == null || Vertices.Count == 0) return new List<Vector3>();
        if (CellSize <= 0.0f) return new List<Vector3>(Vertices);

        Dictionary<GridKey, GridBucket> Buckets = new Dictionary<GridKey, GridBucket>();

        for (int Index = 0; Index < Vertices.Count; Index++)
        {
            Vector3 Vertex = Vertices[Index];

            int X = Mathf.FloorToInt(Vertex.x / CellSize);
            int Y = Mathf.FloorToInt(Vertex.y / CellSize);
            int Z = Mathf.FloorToInt(Vertex.z / CellSize);

            GridKey Key = new GridKey(X, Y, Z);

            if (Buckets.TryGetValue(Key, out GridBucket ExistingBucket))
            {
                ExistingBucket.Sum += Vertex;
                ExistingBucket.Count += 1;
                Buckets[Key] = ExistingBucket;
            }
            else
            {
                GridBucket NewBucket = new GridBucket();
                NewBucket.Sum = Vertex;
                NewBucket.Count = 1;
                Buckets.Add(Key, NewBucket);
            }
        }

        List<Vector3> Result = new List<Vector3>(Buckets.Count);

        foreach (KeyValuePair<GridKey, GridBucket> Entry in Buckets)
        {
            GridBucket Bucket = Entry.Value;
            Result.Add(Bucket.Sum / Bucket.Count);
        }

        return Result;
    }

    static List<Vector3> ReduceToMaxVertexCount(List<Vector3> Vertices, int MaxCount)
    {
        if (Vertices == null || Vertices.Count <= MaxCount) return new List<Vector3>(Vertices);
        if (MaxCount < 4) throw new Exception("Max vertex count must be at least 4.");

        Bounds VertexBounds = new Bounds(Vertices[0], Vector3.zero);
        Vector3 CenterAccumulator = Vector3.zero;

        for (int Index = 0; Index < Vertices.Count; Index++)
        {
            VertexBounds.Encapsulate(Vertices[Index]);
            CenterAccumulator += Vertices[Index];
        }

        Vector3 Center = CenterAccumulator / Vertices.Count;

        List<VertexWithScore> RankedVertices = new List<VertexWithScore>(Vertices.Count);

        for (int Index = 0; Index < Vertices.Count; Index++)
        {
            Vector3 Vertex = Vertices[Index];
            float DistanceFromCenter = (Vertex - Center).sqrMagnitude;

            float BoundaryScore = 0.0f;
            BoundaryScore += Mathf.Abs(Vertex.x - VertexBounds.min.x) < 0.0001f ? 1000.0f : 0.0f;
            BoundaryScore += Mathf.Abs(Vertex.x - VertexBounds.max.x) < 0.0001f ? 1000.0f : 0.0f;
            BoundaryScore += Mathf.Abs(Vertex.y - VertexBounds.min.y) < 0.0001f ? 1000.0f : 0.0f;
            BoundaryScore += Mathf.Abs(Vertex.y - VertexBounds.max.y) < 0.0001f ? 1000.0f : 0.0f;
            BoundaryScore += Mathf.Abs(Vertex.z - VertexBounds.min.z) < 0.0001f ? 1000.0f : 0.0f;
            BoundaryScore += Mathf.Abs(Vertex.z - VertexBounds.max.z) < 0.0001f ? 1000.0f : 0.0f;

            VertexWithScore Item = new VertexWithScore();
            Item.Vertex = Vertex;
            Item.Score = BoundaryScore + DistanceFromCenter;
            RankedVertices.Add(Item);
        }

        RankedVertices.Sort((Left, Right) => Right.Score.CompareTo(Left.Score));

        List<Vector3> Result = new List<Vector3>(MaxCount);
        for (int Index = 0; Index < MaxCount; Index++)
        {
            Result.Add(RankedVertices[Index].Vertex);
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
            Vector3 Vertex = Vertices[Index];
            Builder.Append("DbVector3 { x: ");
            Builder.Append(FloatText(Vertex.x));
            Builder.Append(", y: ");
            Builder.Append(FloatText(Vertex.y));
            Builder.Append(", z: ");
            Builder.Append(FloatText(Vertex.z));
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
            Vector3 Vertex = Vertices[Index];
            Builder.Append(FloatText(Vertex.x));
            Builder.Append(',');
            Builder.Append(FloatText(Vertex.y));
            Builder.Append(',');
            Builder.Append(FloatText(Vertex.z));
            Builder.Append('\n');
        }
        return Builder.ToString();
    }

    struct GridKey : IEquatable<GridKey>
    {
        public int X;
        public int Y;
        public int Z;

        public GridKey(int X, int Y, int Z)
        {
            this.X = X;
            this.Y = Y;
            this.Z = Z;
        }

        public bool Equals(GridKey Other)
        {
            return X == Other.X && Y == Other.Y && Z == Other.Z;
        }

        public override bool Equals(object Object)
        {
            return Object is GridKey Other && Equals(Other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int Hash = 17;
                Hash = (Hash * 31) + X;
                Hash = (Hash * 31) + Y;
                Hash = (Hash * 31) + Z;
                return Hash;
            }
        }
    }

    struct GridBucket
    {
        public Vector3 Sum;
        public int Count;
    }

    struct VertexWithScore
    {
        public Vector3 Vertex;
        public float Score;
    }
}