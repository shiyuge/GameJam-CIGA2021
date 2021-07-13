using System;
using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.IO;
using UnityEngine.UI;
using UnityEditor.Experimental.SceneManagement;


[CustomEditor(typeof(ComponentBinderEx))]
public class ComponentBinderExEditor : Editor
{
    private static readonly string LUA_PATH = "Assets/Lua/Modules/";
    private static readonly string COMPONENTS_START = "----------COMPONENTS BEGIN----------";
    private static readonly string COMPONENTS_END = "----------COMPONENTS END----------";

    private Vector2 scrollPos;
    private ComponentBinderEx binder;

    List<ComponentBinderEx.Entry> entries = new List<ComponentBinderEx.Entry>();

    void OnEnable()
    {
        binder = target as ComponentBinderEx;

        if (binder.entries == null)
        {
            entries = new List<ComponentBinderEx.Entry>();
        }
        else
        {
            entries = binder.entries;
        }
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        GUILayout.Label("Preview", EditorStyles.boldLabel);
        // 打印出所有查找到的Component

        scrollPos = GUILayout.BeginScrollView(scrollPos, GUILayout.Height(300));

        ComponentBinderEx.Entry deleteEntry = null;
        foreach (ComponentBinderEx.Entry entry in entries)
        {
            GUILayout.BeginHorizontal();

            entry.componentType = (ComponentBinderEx.ComponentType)EditorGUILayout.EnumPopup(entry.componentType, GUILayout.Width(100));
            if (entry.value)
            {
                entry.value = entry.value.GetComponent(entry.GetEntryType());
            }

            UnityEngine.Object obj;
            
            
            if (entry.value)
            {
                obj = EditorGUILayout.ObjectField((UnityEngine.Object)entry.value, typeof(Transform), true);
            }
            else
            {
                obj = EditorGUILayout.ObjectField(null, typeof(Transform), true);
            }
            if (entry.value != obj && obj != null)
            {
                Transform trarnsform = (Transform)obj;
                entry.key = obj.name;
                entry.value = trarnsform.GetComponent(entry.GetEntryType());
            }
            
            entry.key = GUILayout.TextField(entry.key, GUILayout.Width(180));


            if (GUILayout.Button("-", GUILayout.Width(20)))
            {
                deleteEntry = entry;
            }
            GUILayout.EndHorizontal();
        }

        if (deleteEntry != null)
        {
            entries.Remove(deleteEntry);
        }

        
        GUILayout.EndScrollView();

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Add", GUILayout.Width(200)))
        {
            this.entries.Add(new ComponentBinderEx.Entry("", ComponentBinderEx.ComponentType.Transform, null));
            binder.SetEntries(entries);
        }

        
        GUILayout.EndHorizontal();

        // 导出按钮，将查找到的Component信息赋值给ComponentBinderEx，并打印注释到相应的lua文件
        if (GUILayout.Button("Export", GUILayout.Width(200)))
        {
            // 设置ComponentBinderEx
            binder.SetEntries(entries);
            if (PrefabStageUtility.GetCurrentPrefabStage() != null)
            {
                 string path = PrefabStageUtility.GetCurrentPrefabStage().prefabAssetPath;
                 GameObject go = PrefabStageUtility.GetCurrentPrefabStage().prefabContentsRoot;
                 GameObject prefab = (GameObject)AssetDatabase.LoadAssetAtPath(path, typeof(GameObject));
                Debug.LogError(go == binder.gameObject);
                PrefabUtility.SaveAsPrefabAsset(binder.gameObject, path);
                //UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(binder.gameObject.scene);
                //  PrefabUtility.ReplacePrefab(binder.gameObject, prefab, ReplacePrefabOptions.ConnectToPrefab);
                //PrefabUtility.ApplyPrefabInstance(go, InteractionMode.UserAction);
                // GameObject selectGO = Selection.activeGameObject;
                // PrefabType selectPrefabType = PrefabUtility.GetPrefabType(selectGO);
                // EditorUtility.SetDirty(selectGO);
                // Debug.LogError(PrefabUtility.GetPrefabParent(selectGO));
            }
            else
            {
                GameObject selectGO = Selection.activeGameObject;
                PrefabType selectPrefabType = PrefabUtility.GetPrefabType(selectGO);
                Debug.LogError(selectPrefabType);
                if (selectPrefabType == PrefabType.PrefabInstance)
                {
                    UnityEngine.Object prefabAsset = PrefabUtility.GetPrefabParent(selectGO);
                    if (prefabAsset != null)
                    {
                        PrefabUtility.ReplacePrefab(selectGO, prefabAsset, ReplacePrefabOptions.ConnectToPrefab);
                    }
                }
                else
                {
                    Debug.LogError("选中的实例不是Prefab实例！");
                }
            }


            AssetDatabase.SaveAssets();

            LuaBehaviour lua = binder.gameObject.GetComponent<LuaBehaviour>();
            // 设置lua文件名
            string filename = LUA_PATH + lua.fileName + ".lua";

            // 文件已存在，将lua代码缓存
            Directory.CreateDirectory(Directory.GetParent(filename).ToString());
            if (File.Exists(filename))
            {              
                WriteComponents(filename, RealCurrentCodes(filename));
            }
            else
            {
                var windowName = Path.GetFileNameWithoutExtension(lua.fileName);
                WriteComponents(filename, GenCodesByTemplate(windowName));
            }

            AssetDatabase.Refresh();
        }
    }

    //读取文件中除去头部注释的其它代码行
    List<string> RealCurrentCodes(string filename)
    {
        var codes = new List<string>();
        var reader = new StreamReader(filename, System.Text.Encoding.UTF8);

        bool found = false;
        while (!reader.EndOfStream)
        {
            string line = reader.ReadLine();
            if (line == COMPONENTS_END)
            {
                found = true;
                continue;
            }

            // 已找到COMPONENTS_END注释，开始记录lua代码
            if (found)
                codes.Add(line);
        }
        reader.Close();
        // 没有找到Components注释段，重新读取整个文件并缓存
        if (found == false)
        {
            reader = new StreamReader(filename, System.Text.Encoding.UTF8);
            while (!reader.EndOfStream)
            {
                codes.Add(reader.ReadLine());
            }
            reader.Close();
        }

        return codes;
    }

    //根据模板文件生成新代码文件
    List<string> GenCodesByTemplate(string windowName)
    {
        var codes = new List<string>();
        StreamReader reader = new StreamReader(Application.dataPath + "/Editor/Lua/TemplateWindow.lua", System.Text.Encoding.UTF8);
        while (!reader.EndOfStream)
        {
            string line = reader.ReadLine();
            line = line.Replace("%Template%", windowName);
            codes.Add(line);
        }
        reader.Close();
        return codes;
    }

    void WriteComponents(string filename, List<string> codes)
    {
        StreamWriter writer = new StreamWriter(filename, false, System.Text.Encoding.UTF8);

        // 写入Component注释段
        writer.WriteLine(COMPONENTS_START);

        var windowName = Path.GetFileNameWithoutExtension(filename);
        if (windowName.EndsWith("Window"))
            writer.WriteLine("---@class {0} : UIWindow", windowName);
        else
            writer.WriteLine("---@class {0}", windowName);
        foreach (ComponentBinderEx.Entry entry in entries)
        {
            string typeString = entry.componentType.ToString();

            if (entry.value.GetType() == typeof(LuaBehaviour))
            {
                var entryLuaBehavior = entry.value.gameObject.GetComponent<LuaBehaviour>();
                var entryModuleName = Path.GetFileNameWithoutExtension(entryLuaBehavior.fileName);
                typeString = entryModuleName;
            }
            writer.WriteLine("---@field {0} {1}", entry.key, typeString);
        }
        writer.WriteLine(COMPONENTS_END);

        // 写入lua代码
        foreach (string code in codes)
        {
            writer.WriteLine(code);
        }
        
        writer.Close();
    }
}
