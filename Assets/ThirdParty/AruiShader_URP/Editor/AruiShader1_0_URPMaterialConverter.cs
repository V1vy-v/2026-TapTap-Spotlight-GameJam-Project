using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public static class AruiShader1_0_URPMaterialConverter
{
    private class TextureState
    {
        public Texture texture;
        public Vector2 scale;
        public Vector2 offset;
    }

    [MenuItem("Tools/AruiShader1.0 URP/转换选中 Built-in 材质为 URP")]
    private static void ConvertSelectedMaterials()
    {
        Shader urpShader = Shader.Find("VFX/AruiShader1.0_URP");
        if(urpShader == null)
        {
            EditorUtility.DisplayDialog(
                "AruiShader1.0 URP",
                "没有找到 VFX/AruiShader1.0_URP。\n请先导入 AruiShader1_0_URP.shader。",
                "确定"
            );
            return;
        }

        Object[] selected = Selection.GetFiltered(typeof(Material), SelectionMode.Assets);
        if(selected == null || selected.Length == 0)
        {
            EditorUtility.DisplayDialog(
                "AruiShader1.0 URP",
                "请先在 Project 面板选中一个或多个 AruiShader 材质球。",
                "确定"
            );
            return;
        }

        int convertedCount = 0;

        foreach(Object item in selected)
        {
            Material material = item as Material;
            if(material == null) continue;

            Dictionary<string, float> floatValues = new Dictionary<string, float>();
            Dictionary<string, Color> colorValues = new Dictionary<string, Color>();
            Dictionary<string, TextureState> textureValues = new Dictionary<string, TextureState>();

            CacheMaterialProperties(material, floatValues, colorValues, textureValues);

            Undo.RecordObject(material, "转换 AruiShader1.0 材质为 URP");
            material.shader = urpShader;
            RestoreMaterialProperties(material, floatValues, colorValues, textureValues);

            EditorUtility.SetDirty(material);
            convertedCount++;
        }

        AssetDatabase.SaveAssets();
        EditorUtility.DisplayDialog(
            "AruiShader1.0 URP",
            "已转换 " + convertedCount + " 个材质。\n\n接触边缘 / 软粒子 / 深度边缘发光需要在 URP Asset 或相机中开启 Depth Texture。",
            "确定"
        );
    }

    [MenuItem("Tools/AruiShader1.0 URP/检查 Depth Texture 设置")]
    private static void ShowDepthTextureGuide()
    {
        EditorUtility.DisplayDialog(
            "AruiShader1.0 URP 深度设置",
            "要使用接触边缘、软粒子淡出和深度边缘发光：\n\n1. 打开当前 Universal Render Pipeline Asset。\n2. 开启 Depth Texture。\n3. 若单个相机仍无深度纹理，在相机的 Universal Additional Camera Data 中开启 Requires Depth Texture。\n\n不开启时，其他核心贴图、颜色、溶解、菲尼尔、顶点、速度线功能仍可使用。",
            "确定"
        );
    }

    private static void CacheMaterialProperties(
        Material material,
        Dictionary<string, float> floatValues,
        Dictionary<string, Color> colorValues,
        Dictionary<string, TextureState> textureValues)
    {
        if(material == null || material.shader == null) return;

        int propertyCount = ShaderUtil.GetPropertyCount(material.shader);
        for(int index = 0; index < propertyCount; index++)
        {
            string propertyName = ShaderUtil.GetPropertyName(material.shader, index);
            ShaderUtil.ShaderPropertyType type = ShaderUtil.GetPropertyType(material.shader, index);

            if(type == ShaderUtil.ShaderPropertyType.Color)
            {
                colorValues[propertyName] = material.GetColor(propertyName);
            }
            else if(type == ShaderUtil.ShaderPropertyType.Float || type == ShaderUtil.ShaderPropertyType.Range)
            {
                floatValues[propertyName] = material.GetFloat(propertyName);
            }
            else if(type == ShaderUtil.ShaderPropertyType.TexEnv)
            {
                textureValues[propertyName] = new TextureState
                {
                    texture = material.GetTexture(propertyName),
                    scale = material.GetTextureScale(propertyName),
                    offset = material.GetTextureOffset(propertyName)
                };
            }
        }
    }

    private static void RestoreMaterialProperties(
        Material material,
        Dictionary<string, float> floatValues,
        Dictionary<string, Color> colorValues,
        Dictionary<string, TextureState> textureValues)
    {
        foreach(KeyValuePair<string, float> pair in floatValues)
        {
            if(material.HasProperty(pair.Key))
            {
                material.SetFloat(pair.Key, pair.Value);
            }
        }

        foreach(KeyValuePair<string, Color> pair in colorValues)
        {
            if(material.HasProperty(pair.Key))
            {
                material.SetColor(pair.Key, pair.Value);
            }
        }

        foreach(KeyValuePair<string, TextureState> pair in textureValues)
        {
            if(!material.HasProperty(pair.Key)) continue;

            material.SetTexture(pair.Key, pair.Value.texture);
            material.SetTextureScale(pair.Key, pair.Value.scale);
            material.SetTextureOffset(pair.Key, pair.Value.offset);
        }
    }
}
