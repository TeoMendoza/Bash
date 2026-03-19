using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

public sealed class PlayerColliderRustGeneratorWindow : EditorWindow
{
    [MenuItem("Tools/Player Collider/2) Generate Rust Collider")]
    public static void Open()
    {
        PlayerColliderRustGeneratorWindow Window = GetWindow<PlayerColliderRustGeneratorWindow>();
        Window.titleContent = new GUIContent("Player Collider Rust");
        Window.Show();
    }

    string ColliderName = "MagicianIdleCollider";
    string ConstantPrefix = "IDLE";
    string OutputFileName = "magician_idle_collider.rs";

    string LegsVerticesText = "";
    string BodyVerticesText = "";
    string HeadVerticesText = "";

    float DuplicateEpsilon = 1e-5f;
    float CoplanarEpsilon = 1e-5f;
    float Margin = 0.0f;

    bool CopyToClipboardAfterGenerate = true;

    Vector2 Scroll;

    void OnGUI()
    {
        Scroll = EditorGUILayout.BeginScrollView(Scroll);

        EditorGUILayout.LabelField("Output Naming", EditorStyles.boldLabel);
        ColliderName = EditorGUILayout.TextField("Collider Function Name", ColliderName);
        ConstantPrefix = EditorGUILayout.TextField("Constant Prefix", ConstantPrefix);
        OutputFileName = EditorGUILayout.TextField("Output File Name", OutputFileName);

        EditorGUILayout.Space(8);

        EditorGUILayout.LabelField("Generation", EditorStyles.boldLabel);
        DuplicateEpsilon = EditorGUILayout.FloatField("Duplicate Epsilon", DuplicateEpsilon);
        CoplanarEpsilon = EditorGUILayout.FloatField("Coplanar Epsilon", CoplanarEpsilon);
        Margin = EditorGUILayout.FloatField("Margin", Margin);
        CopyToClipboardAfterGenerate = EditorGUILayout.Toggle("Copy To Clipboard", CopyToClipboardAfterGenerate);

        EditorGUILayout.Space(8);

        EditorGUILayout.HelpBox("All vertices are automatically shifted so the global lowest y becomes 0. Rust output uses body-part names and a manual CENTER_POINT_HERE placeholder.", MessageType.Info);

        EditorGUILayout.Space(8);

        EditorGUILayout.LabelField("Legs Vertices", EditorStyles.boldLabel);
        LegsVerticesText = EditorGUILayout.TextArea(LegsVerticesText, GUILayout.MinHeight(150));

        EditorGUILayout.Space(8);

        EditorGUILayout.LabelField("Body Vertices", EditorStyles.boldLabel);
        BodyVerticesText = EditorGUILayout.TextArea(BodyVerticesText, GUILayout.MinHeight(150));

        EditorGUILayout.Space(8);

        EditorGUILayout.LabelField("Head Vertices", EditorStyles.boldLabel);
        HeadVerticesText = EditorGUILayout.TextArea(HeadVerticesText, GUILayout.MinHeight(150));

        EditorGUILayout.Space(12);

        if (GUILayout.Button("Generate Rust And Copy"))
        {
            try
            {
                string RustText = BuildRustText();

                if (CopyToClipboardAfterGenerate)
                {
                    EditorGUIUtility.systemCopyBuffer = RustText;
                }

                Debug.Log("Generated player collider Rust text.");
            }
            catch (Exception Exception)
            {
                Debug.LogError(Exception);
            }
        }

        if (GUILayout.Button("Generate Rust And Save File"))
        {
            try
            {
                string RustText = BuildRustText();

                string DefaultDirectory = "Assets";
                string SavePath = EditorUtility.SaveFilePanel("Save Rust Collider File", DefaultDirectory, OutputFileName, "rs");
                if (string.IsNullOrWhiteSpace(SavePath)) return;

                File.WriteAllText(SavePath, RustText, Encoding.UTF8);

                if (CopyToClipboardAfterGenerate)
                {
                    EditorGUIUtility.systemCopyBuffer = RustText;
                }

                Debug.Log($"Saved player collider Rust file to: {SavePath}");
            }
            catch (Exception Exception)
            {
                Debug.LogError(Exception);
            }
        }

        EditorGUILayout.EndScrollView();
    }

    string BuildRustText()
    {
        if (string.IsNullOrWhiteSpace(ColliderName)) throw new Exception("Collider function name cannot be empty.");
        if (string.IsNullOrWhiteSpace(ConstantPrefix)) throw new Exception("Constant prefix cannot be empty.");

        string CleanPrefix = CleanBaseName(ConstantPrefix);

        List<Vector3> ParsedLegVertices = ParseVertices(LegsVerticesText);
        List<Vector3> ParsedBodyVertices = ParseVertices(BodyVerticesText);
        List<Vector3> ParsedHeadVertices = ParseVertices(HeadVerticesText);

        List<Vector3> UniqueLegVertices = DeduplicateVertices(ParsedLegVertices, DuplicateEpsilon);
        List<Vector3> UniqueBodyVertices = DeduplicateVertices(ParsedBodyVertices, DuplicateEpsilon);
        List<Vector3> UniqueHeadVertices = DeduplicateVertices(ParsedHeadVertices, DuplicateEpsilon);

        if (UniqueLegVertices.Count < 4) throw new Exception("Legs vertices must contain at least 4 unique points.");
        if (UniqueBodyVertices.Count < 4) throw new Exception("Body vertices must contain at least 4 unique points.");
        if (UniqueHeadVertices.Count < 4) throw new Exception("Head vertices must contain at least 4 unique points.");

        float LowestY = FindLowestY(UniqueLegVertices, UniqueBodyVertices, UniqueHeadVertices);

        List<Vector3> ShiftedLegVertices = ShiftVerticesY(UniqueLegVertices, -LowestY);
        List<Vector3> ShiftedBodyVertices = ShiftVerticesY(UniqueBodyVertices, -LowestY);
        List<Vector3> ShiftedHeadVertices = ShiftVerticesY(UniqueHeadVertices, -LowestY);

        List<int> LegTriangles = QuickHull3D.BuildHullTriangles(ShiftedLegVertices, CoplanarEpsilon);
        List<int> BodyTriangles = QuickHull3D.BuildHullTriangles(ShiftedBodyVertices, CoplanarEpsilon);
        List<int> HeadTriangles = QuickHull3D.BuildHullTriangles(ShiftedHeadVertices, CoplanarEpsilon);

        ConvexHullData LegHull = new ConvexHullData
        {
            VerticesLocal = ShiftedLegVertices,
            TriangleIndicesLocal = LegTriangles,
            BodyPartName = "LEG",
            ColliderTypeName = "Leg"
        };

        ConvexHullData BodyHull = new ConvexHullData
        {
            VerticesLocal = ShiftedBodyVertices,
            TriangleIndicesLocal = BodyTriangles,
            BodyPartName = "BODY",
            ColliderTypeName = "Body"
        };

        ConvexHullData HeadHull = new ConvexHullData
        {
            VerticesLocal = ShiftedHeadVertices,
            TriangleIndicesLocal = HeadTriangles,
            BodyPartName = "HEAD",
            ColliderTypeName = "Head"
        };

        StringBuilder Builder = new StringBuilder();

        Builder.AppendLine("use crate::*;");
        Builder.AppendLine();

        AppendHullRust(Builder, CleanPrefix, LegHull);
        Builder.AppendLine();
        AppendHullRust(Builder, CleanPrefix, BodyHull);
        Builder.AppendLine();
        AppendHullRust(Builder, CleanPrefix, HeadHull);
        Builder.AppendLine();
        AppendColliderFunction(Builder, ColliderName, CleanPrefix, Margin);

        return Builder.ToString();
    }

    void AppendHullRust(StringBuilder Builder, string Prefix, ConvexHullData Hull)
    {
        string VerticesName = $"{Prefix}_{Hull.BodyPartName}_VERTICES";
        string TriangleIndicesName = $"{Prefix}_{Hull.BodyPartName}_TRIANGLE_INDICES_LOCAL";

        Builder.Append("pub static ");
        Builder.Append(VerticesName);
        Builder.AppendLine(": &[DbVector3] = &[");
        for (int Index = 0; Index < Hull.VerticesLocal.Count; Index++)
        {
            Vector3 Vertex = Hull.VerticesLocal[Index];
            Builder.Append("    DbVector3 { x: ");
            Builder.Append(FloatText(Vertex.x));
            Builder.Append(", y: ");
            Builder.Append(FloatText(Vertex.y));
            Builder.Append(", z: ");
            Builder.Append(FloatText(Vertex.z));
            Builder.AppendLine(" },");
        }
        Builder.AppendLine("];");
        Builder.AppendLine();

        Builder.Append("pub static ");
        Builder.Append(TriangleIndicesName);
        Builder.Append(": &[i32] = &[");
        for (int Index = 0; Index < Hull.TriangleIndicesLocal.Count; Index++)
        {
            if (Index > 0) Builder.Append(", ");
            Builder.Append(Hull.TriangleIndicesLocal[Index].ToString(CultureInfo.InvariantCulture));
        }
        Builder.AppendLine("];");
    }

    void AppendColliderFunction(StringBuilder Builder, string FunctionName, string Prefix, float MarginValue)
    {
        string PrefixLower = Prefix.ToLowerInvariant();

        Builder.Append("pub fn ");
        Builder.Append(FunctionName);
        Builder.AppendLine("() -> ComplexCollider {");

        Builder.Append("    let ");
        Builder.Append(PrefixLower);
        Builder.AppendLine("_leg_hull: ConvexHullCollider = ConvexHullCollider {");
        Builder.Append("        vertices_local: ");
        Builder.Append(Prefix);
        Builder.AppendLine("_LEG_VERTICES.to_vec(),");
        Builder.Append("        triangle_indices_local: ");
        Builder.Append(Prefix);
        Builder.AppendLine("_LEG_TRIANGLE_INDICES_LOCAL.to_vec(),");
        Builder.Append("        margin: ");
        Builder.Append(FloatText(MarginValue));
        Builder.AppendLine(",");
        Builder.AppendLine("        collider_type: ConvexHullColliderType::Leg");
        Builder.AppendLine("    };");

        Builder.Append("    let ");
        Builder.Append(PrefixLower);
        Builder.AppendLine("_body_hull: ConvexHullCollider = ConvexHullCollider {");
        Builder.Append("        vertices_local: ");
        Builder.Append(Prefix);
        Builder.AppendLine("_BODY_VERTICES.to_vec(),");
        Builder.Append("        triangle_indices_local: ");
        Builder.Append(Prefix);
        Builder.AppendLine("_BODY_TRIANGLE_INDICES_LOCAL.to_vec(),");
        Builder.Append("        margin: ");
        Builder.Append(FloatText(MarginValue));
        Builder.AppendLine(",");
        Builder.AppendLine("        collider_type: ConvexHullColliderType::Body");
        Builder.AppendLine("    };");

        Builder.Append("    let ");
        Builder.Append(PrefixLower);
        Builder.AppendLine("_head_hull: ConvexHullCollider = ConvexHullCollider {");
        Builder.Append("        vertices_local: ");
        Builder.Append(Prefix);
        Builder.AppendLine("_HEAD_VERTICES.to_vec(),");
        Builder.Append("        triangle_indices_local: ");
        Builder.Append(Prefix);
        Builder.AppendLine("_HEAD_TRIANGLE_INDICES_LOCAL.to_vec(),");
        Builder.Append("        margin: ");
        Builder.Append(FloatText(MarginValue));
        Builder.AppendLine(",");
        Builder.AppendLine("        collider_type: ConvexHullColliderType::Head");
        Builder.AppendLine("    };");

        Builder.Append("    let ");
        Builder.Append(PrefixLower);
        Builder.AppendLine("_convex_hulls: Vec<ConvexHullCollider> = vec![");
        Builder.Append("        ");
        Builder.Append(PrefixLower);
        Builder.AppendLine("_leg_hull,");
        Builder.Append("        ");
        Builder.Append(PrefixLower);
        Builder.AppendLine("_body_hull,");
        Builder.Append("        ");
        Builder.Append(PrefixLower);
        Builder.AppendLine("_head_hull");
        Builder.AppendLine("    ];");

        Builder.Append("    ComplexCollider { convex_hulls: ");
        Builder.Append(PrefixLower);
        Builder.AppendLine("_convex_hulls, center_point: CENTER_POINT_HERE }");
        Builder.AppendLine("}");
    }

    static float FindLowestY(List<Vector3> LegVertices, List<Vector3> BodyVertices, List<Vector3> HeadVertices)
    {
        bool HasValue = false;
        float LowestY = 0.0f;

        void CheckList(List<Vector3> Vertices)
        {
            for (int Index = 0; Index < Vertices.Count; Index++)
            {
                float CurrentY = Vertices[Index].y;
                if (HasValue == false || CurrentY < LowestY)
                {
                    LowestY = CurrentY;
                    HasValue = true;
                }
            }
        }

        CheckList(LegVertices);
        CheckList(BodyVertices);
        CheckList(HeadVertices);

        return LowestY;
    }

    static List<Vector3> ShiftVerticesY(List<Vector3> Vertices, float YOffset)
    {
        List<Vector3> Result = new List<Vector3>(Vertices.Count);

        for (int Index = 0; Index < Vertices.Count; Index++)
        {
            Vector3 Vertex = Vertices[Index];
            Vertex.y += YOffset;

            if (Mathf.Abs(Vertex.y) <= 1e-6f) Vertex.y = 0.0f;
            if (Vertex.y < 0.0f && Vertex.y > -1e-6f) Vertex.y = 0.0f;

            Result.Add(Vertex);
        }

        return Result;
    }

    static List<Vector3> ParseVertices(string Text)
    {
        List<Vector3> Result = new List<Vector3>();
        if (string.IsNullOrWhiteSpace(Text)) throw new Exception("No vertices provided.");

        string[] Lines = Text.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

        Regex DbVectorStructPattern = new Regex(@"DbVector3\s*\{\s*x:\s*([-\d\.eE\+]+)\s*,\s*y:\s*([-\d\.eE\+]+)\s*,\s*z:\s*([-\d\.eE\+]+)\s*\}", RegexOptions.Compiled);
        Regex DbVectorCtorPattern = new Regex(@"DbVector3\s*\(\s*([-\d\.eE\+]+)\s*f?\s*,\s*([-\d\.eE\+]+)\s*f?\s*,\s*([-\d\.eE\+]+)\s*f?\s*\)", RegexOptions.Compiled);
        Regex PlainPattern = new Regex(@"^\s*([-\d\.eE\+]+)\s*,\s*([-\d\.eE\+]+)\s*,\s*([-\d\.eE\+]+)\s*$", RegexOptions.Compiled);

        for (int Index = 0; Index < Lines.Length; Index++)
        {
            string Line = Lines[Index].Trim();

            Match MatchStruct = DbVectorStructPattern.Match(Line);
            if (MatchStruct.Success)
            {
                float X = ParseFloat(MatchStruct.Groups[1].Value);
                float Y = ParseFloat(MatchStruct.Groups[2].Value);
                float Z = ParseFloat(MatchStruct.Groups[3].Value);
                Result.Add(new Vector3(X, Y, Z));
                continue;
            }

            Match MatchCtor = DbVectorCtorPattern.Match(Line);
            if (MatchCtor.Success)
            {
                float X = ParseFloat(MatchCtor.Groups[1].Value);
                float Y = ParseFloat(MatchCtor.Groups[2].Value);
                float Z = ParseFloat(MatchCtor.Groups[3].Value);
                Result.Add(new Vector3(X, Y, Z));
                continue;
            }

            Match MatchPlain = PlainPattern.Match(Line);
            if (MatchPlain.Success)
            {
                float X = ParseFloat(MatchPlain.Groups[1].Value);
                float Y = ParseFloat(MatchPlain.Groups[2].Value);
                float Z = ParseFloat(MatchPlain.Groups[3].Value);
                Result.Add(new Vector3(X, Y, Z));
                continue;
            }

            throw new Exception($"Unable to parse vertex line: {Line}");
        }

        return Result;
    }

    static float ParseFloat(string Text)
    {
        return float.Parse(Text, NumberStyles.Float, CultureInfo.InvariantCulture);
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

        if (Result.Count < 4) throw new Exception("After deduplication, not enough vertices remain.");
        return Result;
    }

    static string CleanBaseName(string Text)
    {
        if (string.IsNullOrWhiteSpace(Text)) Text = "PLAYER";
        string Clean = Regex.Replace(Text, @"[^A-Za-z0-9]+", "_").Trim('_');
        if (string.IsNullOrWhiteSpace(Clean)) Clean = "PLAYER";
        return Clean.ToUpperInvariant();
    }

    static string FloatText(float Value)
    {
        if (float.IsNaN(Value) || float.IsInfinity(Value)) return "0.0";
        if (Mathf.Abs(Value) <= 1e-6f) Value = 0.0f;

        string Text = Value.ToString("R", CultureInfo.InvariantCulture);
        if (Text.IndexOf('.') < 0 && Text.IndexOf('e') < 0 && Text.IndexOf('E') < 0) Text += ".0";
        return Text;
    }

    sealed class ConvexHullData
    {
        public List<Vector3> VerticesLocal;
        public List<int> TriangleIndicesLocal;
        public string BodyPartName;
        public string ColliderTypeName;
    }
}