using UnityEngine;
using UnityEditor;
using System.IO;
using System.Text.RegularExpressions;
using System.Text;

public class CommentExtractor : EditorWindow
{
    private string _resultText = "";
    private Vector2 _scrollPos = Vector2.zero;
    private GUIStyle _textAreaStyle = new GUIStyle(EditorStyles.textArea);

    [MenuItem("Tools/CommentExtractor")]
    public static void ShowWindow()
    {
        GetWindow<CommentExtractor>();
    }

    private void OnEnable()
    {
        _textAreaStyle.padding = new RectOffset(10, 10, 10, 10);
        _textAreaStyle.richText = true;
        _textAreaStyle.fontSize = 12;
    }

    private void OnGUI()
    {
        GUILayout.BeginHorizontal();

        if (GUILayout.Button("Extract comments.", GUILayout.Height(30)))
        {
            Extract();
            GUI.FocusControl(null);
            Repaint();
        }
        else if (GUILayout.Button("Clear window.", GUILayout.Height(30)))
        {
            _resultText = "";
            _scrollPos = Vector2.zero;
            GUI.FocusControl(null);
            Repaint();
        }

        GUILayout.EndHorizontal();

        GUILayout.Label("Extraction result.", EditorStyles.boldLabel);
        _scrollPos = EditorGUILayout.BeginScrollView(_scrollPos);

        // Display in text area.
        EditorGUILayout.SelectableLabel(_resultText, _textAreaStyle, GUILayout.ExpandHeight(true));

        EditorGUILayout.EndScrollView();
    }

    private void Extract()
    {
        string[] scriptGuids = AssetDatabase.FindAssets("t:Script");
        StringBuilder sb = new StringBuilder();

        foreach (string guid in scriptGuids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            if (!path.StartsWith("Assets/Scripts/", System.StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            string[] lines = File.ReadAllLines(path);
            string fullContent = string.Join("\n", lines);

            sb.AppendLine($"■ File: {path}");

            // Extracting commented sections using regular expressions.
            var matches = Regex.Matches(fullContent, @"//.*?$|/\*.*?\*/", RegexOptions.Multiline | RegexOptions.Singleline);

            foreach (Match match in matches)
            {
                // Get line number.
                int lineNo = GetLineNumber(fullContent, match.Index);
                string comment = match.Value.Trim();
                sb.AppendLine($"<color=grey>L{lineNo,6}:</color> {comment}");
            }

            sb.AppendLine("--------------------------------------------------");
        }

        _resultText = sb.ToString();
    }

    private int GetLineNumber(string content, int index)
    {
        return content.Substring(0, index).Split('\n').Length;
    }
}