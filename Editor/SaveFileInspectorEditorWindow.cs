using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace SBG.Memento.Editor
{
	public class SaveFileInspectorEditorWindow : EditorWindow
	{
		private readonly string[] seperators = new string[] {"/", "\\"};

		private bool runtimeMode;
		private string path;
		private string filename;
		private SaveData cache;
		private string base64String;

		private Vector2 scrollPos = Vector2.zero;
		private Dictionary<string, bool> foldouts = new Dictionary<string, bool>();


		[MenuItem("SBG/Debugging/Savefile Debugger")]
		public static void ShowWindow()
		{
			GetWindow<SaveFileInspectorEditorWindow>("Memento Savefile Debugger");
		}

        private void OnGUI()
		{
            if (GUILayout.Button("Show Savefiles in Explorer", GUILayout.Height(30))) System.Diagnostics.Process.Start(Application.persistentDataPath);

            if (Application.isPlaying) DrawRuntimeGUI();
			else DrawEditTimeGUI();

            if (cache == null) return;

            EditorGUILayout.LabelField(filename, EditorStyles.boldLabel);
            EditorGUILayout.LabelField($"Path: {path}", EditorStyles.miniLabel);
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Save Data:", EditorStyles.boldLabel);
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
            DisplaySubData(cache);
            EditorGUILayout.EndScrollView();
        }

		private void DrawRuntimeGUI()
		{
			if (!runtimeMode)
			{
				runtimeMode = true;
				cache = InternalProcessing.GetData(SaveType.GameFile);
                foldouts.Clear();
                filename = "Runtime Data";
                path = "-";
            }
		}

		private void DrawEditTimeGUI()
		{
			if (runtimeMode) runtimeMode = false;

            if (GUILayout.Button("Select File", GUILayout.Height(25)))
            {
                string basePath = $"{Application.persistentDataPath}/Memento";
                path = EditorUtility.OpenFilePanel("Open Save File", basePath, "bin,st");
                if (File.Exists(path))
                {
                    cache = InternalProcessing.LoadFromBinaryFile(path);
                    foldouts.Clear();

                    string[] pathChunks = path.Split(seperators, StringSplitOptions.RemoveEmptyEntries);
                    filename = pathChunks[pathChunks.Length - 1];
                }
            }

			base64String = EditorGUILayout.TextArea(base64String, GUILayout.MinHeight(50));
			if (GUILayout.Button("Load from Base64 String", GUILayout.Height(25)))
			{
				cache = InternalProcessing.LoadFromBase64(base64String);
                foldouts.Clear();
				filename = "Generated Savefile";
				path = "Base64";
            }
        }

		private void DisplaySubData(SaveData data)
        {
			ArrayList dataArray = data.GetKeys();
			string key;

			EditorGUILayout.BeginVertical(EditorStyles.helpBox);

            for (int i = 0; i < dataArray.Count; i++)
            {
				key = dataArray[i] as string;
				object entry = InternalProcessing.GetRawDataEntry(data, key);
				DisplayEntry(key, entry);
            }

			EditorGUILayout.EndVertical();
        }

		private void DisplayEntry(string id, object entry)
        {
			if (entry is SaveData)
			{
				if (!foldouts.ContainsKey(id)) foldouts.Add(id, false);
				foldouts[id] = EditorGUILayout.Foldout(foldouts[id], id);

				if (!foldouts[id]) return;

				EditorGUI.indentLevel++;
				DisplaySubData(entry as SaveData);
				EditorGUI.indentLevel--;
			}
			else if (entry is Array)
			{
				Array array = entry as Array;

				if (entry is SaveData[] || array.Length > 4) DisplayArray(id, array);
				else DisplayStructArray(id, array);
			}
			else
			{
				EditorGUILayout.BeginHorizontal();
				EditorGUILayout.PrefixLabel(id);
				EditorGUILayout.LabelField(entry != null ? entry.ToString() : "null");
				EditorGUILayout.EndHorizontal();
			}
		}

		private void DisplayArray(string id, Array array)
        {
			if (!foldouts.ContainsKey(id)) foldouts.Add(id, false);
			foldouts[id] = EditorGUILayout.Foldout(foldouts[id], id);

			if (!foldouts[id]) return;

			EditorGUILayout.BeginVertical(EditorStyles.helpBox);
			EditorGUI.indentLevel++;
			for (int i = 0; i < array.Length; i++)
			{
				DisplayEntry(i.ToString(), array.GetValue(i));
			}
			EditorGUI.indentLevel--;
			EditorGUILayout.EndVertical();
		}

		private void DisplayStructArray(string id, Array array)
        {
			string valueString = "";

			for (int i = 0; i < array.Length; i++)
			{
				if (i > 0) valueString += "; ";
				valueString += array.GetValue(i).ToString();
			}

			if (array.Length == 0)
            {
				valueString = "[EMPTY ARRAY]";
            }

			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.PrefixLabel(id);
			EditorGUILayout.LabelField(valueString);
			EditorGUILayout.EndHorizontal();
		}
    }
}