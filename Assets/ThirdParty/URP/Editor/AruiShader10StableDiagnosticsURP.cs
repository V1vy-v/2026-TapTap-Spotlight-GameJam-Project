using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

public class AruiShader10StableDiagnosticsURP : EditorWindow
{
    private Vector2 scroll;
    private readonly List<string> results = new List<string>();

    [MenuItem("Tools/AruiShader1.0 URP/稳定性检查 V1.0")]
    public static void OpenWindow()
    {
        AruiShader10StableDiagnosticsURP window = GetWindow<AruiShader10StableDiagnosticsURP>("AruiShader1.0 稳定性检查");
        window.minSize = new Vector2(420f, 360f);
        window.Show();
    }

    private void OnGUI()
    {
        EditorGUILayout.LabelField("AruiShader1.0 稳定重构检查 V1.0", EditorStyles.boldLabel);
        EditorGUILayout.HelpBox(
            "检查当前选中材质或整个特效层级。修复只同步 RenderType、Blend 因子和无效 Render Queue，不覆盖贴图、颜色或特效参数。",
            MessageType.Info
        );

        if(GUILayout.Button("检查当前选择", GUILayout.Height(28f))) RunCheck(false);
        if(GUILayout.Button("检查并修复当前选择", GUILayout.Height(28f))) RunCheck(true);

        EditorGUILayout.Space(5f);
        scroll = EditorGUILayout.BeginScrollView(scroll);
        for(int index = 0; index < results.Count; index++)
        {
            EditorGUILayout.LabelField(results[index], EditorStyles.wordWrappedLabel);
        }
        EditorGUILayout.EndScrollView();
    }

    private void RunCheck(bool repair)
    {
        results.Clear();
        List<Material> materials = CollectSelectedMaterials();
        if(materials.Count == 0)
        {
            results.Add("没有找到材质。请选择材质球，或选择包含 Renderer 的特效根节点。");
            return;
        }

        int validCount = 0;
        int repairedCount = 0;
        for(int index = 0; index < materials.Count; index++)
        {
            Material material = materials[index];
            if(material == null || material.shader == null || material.shader.name != "VFX/AruiShader1.0_URP") continue;
            validCount++;
            if(CheckOneMaterial(material, repair)) repairedCount++;
        }

        results.Insert(0, "检查完成：找到 " + validCount + " 个目标材质，修复 " + repairedCount + " 个。");
    }

    private bool CheckOneMaterial(Material material, bool repair)
    {
        bool changed = false;
        string prefix = "[" + material.name + "] ";

        if(material.HasProperty("_MainColorMix"))
        {
            results.Add(prefix + "主贴图颜色混合=" + material.GetFloat("_MainColorMix").ToString("F3") + "；新材质稳定默认=1。");
        }

        int savedQueue = material.HasProperty("_RenderQueueValue")
            ? Mathf.RoundToInt(material.GetFloat("_RenderQueueValue"))
            : 3000;
        int safeQueue = Mathf.Clamp(savedQueue, 2500, 5000);

        if(material.renderQueue < 2500 || material.renderQueue > 5000)
        {
            results.Add(prefix + "真实 Render Queue 无效：" + material.renderQueue + "，建议修复为 " + safeQueue + "。");
            if(repair)
            {
                Undo.RecordObject(material, "修复 AruiShader1.0 Render Queue");
                material.renderQueue = safeQueue;
                changed = true;
            }
        }

        if(material.GetTag("RenderType", false, string.Empty) != "Transparent")
        {
            results.Add(prefix + "RenderType 不是 Transparent。");
            if(repair)
            {
                Undo.RecordObject(material, "修复 AruiShader1.0 RenderType");
                material.SetOverrideTag("RenderType", "Transparent");
                changed = true;
            }
        }

        if(repair)
        {
            int blendMode = material.HasProperty("_BlendMode")
                ? Mathf.Clamp(Mathf.RoundToInt(material.GetFloat("_BlendMode")), 0, 3)
                : 0;
            ApplyBlend(material, blendMode);
            if(material.HasProperty("_RenderQueueValue")) material.SetFloat("_RenderQueueValue", safeQueue);
            if(material.renderQueue != safeQueue) material.renderQueue = safeQueue;
            EditorUtility.SetDirty(material);
        }

        if(!changed) results.Add(prefix + "渲染状态检查通过。");
        return changed;
    }

    private static void ApplyBlend(Material material, int mode)
    {
        if(mode == 0)
        {
            SetFloat(material, "_SrcBlend", (float)BlendMode.SrcAlpha);
            SetFloat(material, "_DstBlend", (float)BlendMode.OneMinusSrcAlpha);
        }
        else if(mode == 1)
        {
            SetFloat(material, "_SrcBlend", (float)BlendMode.SrcAlpha);
            SetFloat(material, "_DstBlend", (float)BlendMode.One);
        }
        else if(mode == 2)
        {
            SetFloat(material, "_SrcBlend", (float)BlendMode.OneMinusDstColor);
            SetFloat(material, "_DstBlend", (float)BlendMode.One);
        }
        else
        {
            SetFloat(material, "_SrcBlend", (float)BlendMode.One);
            SetFloat(material, "_DstBlend", (float)BlendMode.OneMinusSrcAlpha);
        }
    }

    private static void SetFloat(Material material, string propertyName, float value)
    {
        if(material != null && material.HasProperty(propertyName)) material.SetFloat(propertyName, value);
    }

    private static List<Material> CollectSelectedMaterials()
    {
        List<Material> result = new List<Material>();
        AddUnique(result, Selection.activeObject as Material);

        GameObject selected = Selection.activeGameObject;
        if(selected != null)
        {
            Renderer[] renderers = selected.GetComponentsInChildren<Renderer>(true);
            for(int rendererIndex = 0; rendererIndex < renderers.Length; rendererIndex++)
            {
                Renderer renderer = renderers[rendererIndex];
                if(renderer == null) continue;
                Material[] materials = renderer.sharedMaterials;
                if(materials == null) continue;
                for(int materialIndex = 0; materialIndex < materials.Length; materialIndex++)
                {
                    AddUnique(result, materials[materialIndex]);
                }
            }
        }
        return result;
    }

    private static void AddUnique(List<Material> list, Material material)
    {
        if(material != null && !list.Contains(material)) list.Add(material);
    }
}
