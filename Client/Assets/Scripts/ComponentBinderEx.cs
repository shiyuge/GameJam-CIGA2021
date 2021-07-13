using UnityEngine;
using System.Collections.Generic;
using LuaInterface;
using UnityEngine.UI;
using System;

public class ComponentBinderEx : MonoBehaviour
{
    //这个字段用于序列化输出，不要更改
    public enum ComponentType
    {
        Transform,
        Button,
        Image,
        RawImage,
        ScrollView,
        Slider,
        Toggle,
        RectTransform,
        Text,
        LuaBehaviour,
    }

    public static Type[] TypeList = { typeof(Transform), typeof(Button), typeof(Image), typeof(RawImage), typeof(ScrollRect), typeof(Slider), typeof(Toggle),
        typeof(RectTransform), typeof(Text), typeof(LuaBehaviour)};
    public static ComponentType[] ComponentList = { ComponentType.Transform, ComponentType.Button, ComponentType.Image, ComponentType.RawImage, ComponentType.ScrollView, ComponentType.Slider, ComponentType.Toggle,
        ComponentType.RectTransform, ComponentType.Text, ComponentType.LuaBehaviour };

    //字段用于序列化，慎重修改
    [System.Serializable]    
    public class Entry
    {
        public string key;      // 变量名
        public Component value; // 指向的Component
        public ComponentType componentType;

        public Entry(string key, ComponentType type, Component value)
        {
            this.key = key;
            this.componentType = type;
            this.value = value;
        }

        public Type GetEntryType()
        {
            for (int i = 0; i < ComponentList.Length; i++)
            {
                if (ComponentList[i] == componentType)
                {
                    return TypeList[i];
                }
            }
            return null;
        }
    }

    [HideInInspector]
    public List<Entry> entries;
    
    private void OnDestroy()
    {
        entries = null;
    }

    /// <summary>
    /// 将Components存入lua对象
    /// </summary>
    /// <param name="table"></param>
    public void SetLuaData(LuaTable table)
    {
        foreach (Entry entry in entries)
        {
            if (string.IsNullOrEmpty(entry.key))
            {
                continue;
            }

            if (entry.value == null)
                throw new System.Exception(string.Format("[{0}] Component is null!", entry.key));

            
            if (entry.value.GetType() == typeof(LuaBehaviour))
            {
                LuaTable tempTable = table;
                string tempKey = entry.key;
                Component tempValue = entry.value;

                (entry.value as LuaBehaviour).rebindCallback = () =>
                {
                    tempTable[tempKey] = (tempValue as LuaBehaviour).GetLuaObject();
                };
                (entry.value as LuaBehaviour).GetLuaObject();
            }
            else
            {
                table[entry.key] = entry.value;
            }
        }
    }

    /// <summary>
    /// 外部传入查找到的Component信息
    /// </summary>
    /// <param name="entries"></param>
    public void SetEntries(List<Entry> entries)
    {
        this.entries = entries;
    }
}
