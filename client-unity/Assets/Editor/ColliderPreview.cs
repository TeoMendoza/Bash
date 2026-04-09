using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

public class BodyColliderPreviewTool : EditorWindow
{
    private string HeadInput = "";
    private string BodyInput = "";
    private string LegsInput = "";

    private Material PreviewMaterial;
    private string ParentObjectName = "BodyColliderPreview";
    private float SideEpsilon = 0.0001f;

    [MenuItem("Tools/Body Collider Preview Tool")]
    public static void OpenWindow()
    {
        BodyColliderPreviewTool Window = GetWindow<BodyColliderPreviewTool>("Collider Preview");
        Window.minSize = new Vector2(500, 650);
    }

    private void OnGUI()
    {
        GUILayout.Label("Body Collider Preview Generator", EditorStyles.boldLabel);
        EditorGUILayout.Space();

        EditorGUILayout.HelpBox(
            "Paste Rust-style DbVector3 lists or plain vector triples. The tool will generate preview meshes in the scene for Head, Body, and Legs.",
            MessageType.Info
        );

        PreviewMaterial = (Material)EditorGUILayout.ObjectField("Preview Material", PreviewMaterial, typeof(Material), false);
        ParentObjectName = EditorGUILayout.TextField("Parent Object Name", ParentObjectName);
        SideEpsilon = EditorGUILayout.FloatField("Side Epsilon", SideEpsilon);

        EditorGUILayout.Space();
        GUILayout.Label("Head Vertices", EditorStyles.boldLabel);
        HeadInput = EditorGUILayout.TextArea(HeadInput, GUILayout.MinHeight(140));

        EditorGUILayout.Space();
        GUILayout.Label("Body Vertices", EditorStyles.boldLabel);
        BodyInput = EditorGUILayout.TextArea(BodyInput, GUILayout.MinHeight(140));

        EditorGUILayout.Space();
        GUILayout.Label("Legs Vertices", EditorStyles.boldLabel);
        LegsInput = EditorGUILayout.TextArea(LegsInput, GUILayout.MinHeight(140));

        EditorGUILayout.Space();

        GUILayout.BeginHorizontal();

        if (GUILayout.Button("Generate Preview", GUILayout.Height(32))) {
            GeneratePreview();
        }

        if (GUILayout.Button("Delete Preview", GUILayout.Height(32))) {
            DeletePreview();
        }

        GUILayout.EndHorizontal();
    }

    private void GeneratePreview()
    {
        List<Vector3> HeadVertices = ParseDbVectors(HeadInput);
        List<Vector3> BodyVertices = ParseDbVectors(BodyInput);
        List<Vector3> LegsVertices = ParseDbVectors(LegsInput);

        GameObject ExistingParent = GameObject.Find(ParentObjectName);
        if (ExistingParent != null) {
            DestroyImmediate(ExistingParent);
        }

        GameObject ParentObject = new GameObject(ParentObjectName);
        Undo.RegisterCreatedObjectUndo(ParentObject, "Create Collider Preview");

        CreateHullChild(ParentObject.transform, "HeadPreview", HeadVertices);
        CreateHullChild(ParentObject.transform, "BodyPreview", BodyVertices);
        CreateHullChild(ParentObject.transform, "LegsPreview", LegsVertices);

        Selection.activeGameObject = ParentObject;
    }

    private void DeletePreview()
    {
        GameObject ExistingParent = GameObject.Find(ParentObjectName);
        if (ExistingParent != null) {
            DestroyImmediate(ExistingParent);
        }
    }

    private void CreateHullChild(Transform ParentTransform, string ChildName, List<Vector3> Vertices)
    {
        if (Vertices == null || Vertices.Count < 4) {
            return;
        }

        Mesh HullMesh = BuildConvexHullMesh(Vertices, SideEpsilon);
        if (HullMesh == null) {
            return;
        }

        GameObject ChildObject = new GameObject(ChildName);
        ChildObject.transform.SetParent(ParentTransform, false);
        ChildObject.transform.localPosition = Vector3.zero;
        ChildObject.transform.localRotation = Quaternion.identity;
        ChildObject.transform.localScale = Vector3.one;

        MeshFilter MeshFilterComponent = ChildObject.AddComponent<MeshFilter>();
        MeshRenderer MeshRendererComponent = ChildObject.AddComponent<MeshRenderer>();

        MeshFilterComponent.sharedMesh = HullMesh;

        if (PreviewMaterial != null) {
            MeshRendererComponent.sharedMaterial = PreviewMaterial;
        }
    }

    private List<Vector3> ParseDbVectors(string InputText)
    {
        List<Vector3> ParsedVertices = new List<Vector3>();

        if (string.IsNullOrWhiteSpace(InputText)) {
            return ParsedVertices;
        }

        Regex RustPattern = new Regex(
            @"DbVector3\s*\{\s*x:\s*([-+]?\d*\.?\d+(?:[eE][-+]?\d+)?)\s*,\s*y:\s*([-+]?\d*\.?\d+(?:[eE][-+]?\d+)?)\s*,\s*z:\s*([-+]?\d*\.?\d+(?:[eE][-+]?\d+)?)\s*\}",
            RegexOptions.Multiline
        );

        MatchCollection RustMatches = RustPattern.Matches(InputText);

        if (RustMatches.Count > 0) {
            foreach (Match CurrentMatch in RustMatches) {
                float XValue = float.Parse(CurrentMatch.Groups[1].Value, CultureInfo.InvariantCulture);
                float YValue = float.Parse(CurrentMatch.Groups[2].Value, CultureInfo.InvariantCulture);
                float ZValue = float.Parse(CurrentMatch.Groups[3].Value, CultureInfo.InvariantCulture);

                ParsedVertices.Add(new Vector3(XValue, YValue, ZValue));
            }

            return ParsedVertices;
        }

        Regex PlainPattern = new Regex(
            @"\(?\s*([-+]?\d*\.?\d+(?:[eE][-+]?\d+)?)\s*,\s*([-+]?\d*\.?\d+(?:[eE][-+]?\d+)?)\s*,\s*([-+]?\d*\.?\d+(?:[eE][-+]?\d+)?)\s*\)?",
            RegexOptions.Multiline
        );

        MatchCollection PlainMatches = PlainPattern.Matches(InputText);

        foreach (Match CurrentMatch in PlainMatches) {
            float XValue = float.Parse(CurrentMatch.Groups[1].Value, CultureInfo.InvariantCulture);
            float YValue = float.Parse(CurrentMatch.Groups[2].Value, CultureInfo.InvariantCulture);
            float ZValue = float.Parse(CurrentMatch.Groups[3].Value, CultureInfo.InvariantCulture);

            ParsedVertices.Add(new Vector3(XValue, YValue, ZValue));
        }

        return ParsedVertices;
    }

    private Mesh BuildConvexHullMesh(List<Vector3> Points, float Epsilon)
    {
        int PointCount = Points.Count;
        Vector3 HullCenter = ComputeAverage(Points);

        HashSet<string> AddedFaces = new HashSet<string>();
        List<int> Triangles = new List<int>();

        for (int FirstIndex = 0; FirstIndex < PointCount - 2; FirstIndex++) {
            for (int SecondIndex = FirstIndex + 1; SecondIndex < PointCount - 1; SecondIndex++) {
                for (int ThirdIndex = SecondIndex + 1; ThirdIndex < PointCount; ThirdIndex++) {
                    Vector3 FirstPoint = Points[FirstIndex];
                    Vector3 SecondPoint = Points[SecondIndex];
                    Vector3 ThirdPoint = Points[ThirdIndex];

                    Vector3 FaceNormal = Vector3.Cross(SecondPoint - FirstPoint, ThirdPoint - FirstPoint);
                    if (FaceNormal.sqrMagnitude < Epsilon * Epsilon) {
                        continue;
                    }

                    bool HasPositiveSide = false;
                    bool HasNegativeSide = false;

                    for (int TestIndex = 0; TestIndex < PointCount; TestIndex++) {
                        if (TestIndex == FirstIndex || TestIndex == SecondIndex || TestIndex == ThirdIndex) {
                            continue;
                        }

                        float Distance = Vector3.Dot(FaceNormal, Points[TestIndex] - FirstPoint);

                        if (Distance > Epsilon) {
                            HasPositiveSide = true;
                        }
                        else if (Distance < -Epsilon) {
                            HasNegativeSide = true;
                        }

                        if (HasPositiveSide && HasNegativeSide) {
                            break;
                        }
                    }

                    if (HasPositiveSide && HasNegativeSide) {
                        continue;
                    }

                    int[] SortedFaceIndices = new int[] { FirstIndex, SecondIndex, ThirdIndex };
                    Array.Sort(SortedFaceIndices);

                    string FaceKey = SortedFaceIndices[0] + "|" + SortedFaceIndices[1] + "|" + SortedFaceIndices[2];
                    if (AddedFaces.Contains(FaceKey)) {
                        continue;
                    }

                    AddedFaces.Add(FaceKey);

                    Vector3 FaceCenter = (FirstPoint + SecondPoint + ThirdPoint) / 3f;
                    Vector3 OutwardDirection = FaceCenter - HullCenter;

                    if (Vector3.Dot(FaceNormal, OutwardDirection) < 0f) {
                        Triangles.Add(FirstIndex);
                        Triangles.Add(ThirdIndex);
                        Triangles.Add(SecondIndex);
                    }
                    else {
                        Triangles.Add(FirstIndex);
                        Triangles.Add(SecondIndex);
                        Triangles.Add(ThirdIndex);
                    }
                }
            }
        }

        if (Triangles.Count == 0) {
            return null;
        }

        Mesh ResultMesh = new Mesh();
        ResultMesh.name = "PreviewHullMesh";
        ResultMesh.indexFormat = Points.Count > 65000
            ? UnityEngine.Rendering.IndexFormat.UInt32
            : UnityEngine.Rendering.IndexFormat.UInt16;

        ResultMesh.SetVertices(Points);
        ResultMesh.SetTriangles(Triangles, 0);
        ResultMesh.RecalculateNormals();
        ResultMesh.RecalculateBounds();

        return ResultMesh;
    }

    private Vector3 ComputeAverage(List<Vector3> Points)
    {
        Vector3 Sum = Vector3.zero;

        for (int Index = 0; Index < Points.Count; Index++) {
            Sum += Points[Index];
        }

        return Sum / Points.Count;
    }
}