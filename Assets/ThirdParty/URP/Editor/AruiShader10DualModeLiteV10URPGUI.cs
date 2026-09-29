using UnityEngine;
using UnityEditor;
using UnityEngine.Rendering;

public class AruiShader10DualModeLiteV10URPGUI : ShaderGUI
{
    private const float TextureBoxSize = 62f;
    private static bool coreFold = true;
    private static bool renderFold = false;

    public override void OnGUI(MaterialEditor editor, MaterialProperty[] properties)
    {
        Material material = editor.target as Material;
        if(material == null) return;

        SyncAutomaticRenderRules(material);
        DrawHeader(material);

        if(GUILayout.Button("打开 AruiShader1.0 URP 完整控制台 V1.0", GUILayout.Height(30)))
        {
            AruiShader10ConsoleWindowV10URP.Open(material);
        }

        EditorGUILayout.Space(4);
        coreFold = EditorGUILayout.Foldout(coreFold, "常用参数", true, EditorStyles.foldoutHeader);
        if(coreFold)
        {
            DrawProperty(editor, properties, "_MainTex");
            DrawProperty(editor, properties, "_MainAlphaStrength");
            DrawProperty(editor, properties, "_UseThreeColor");

            if(Get(material, "_UseThreeColor") > 0.5f)
            {
                DrawProperty(editor, properties, "_ColorA");
                DrawProperty(editor, properties, "_ColorB");
                DrawProperty(editor, properties, "_ColorC");
            }
            else
            {
                DrawProperty(editor, properties, "_SingleColor");
                DrawProperty(editor, properties, "_UseBackFaceColor");
                if(Get(material, "_UseBackFaceColor") > 0.5f)
                {
                    DrawProperty(editor, properties, "_BackFaceColor");
                    DrawProperty(editor, properties, "_BackFaceColorSoftness");
                }
            }

            DrawProperty(editor, properties, "_Opacity");
            DrawProperty(editor, properties, "_EmissionIntensity");
            DrawProperty(editor, properties, "_UseDissolveModule");
            if(Get(material, "_UseDissolveModule") > 0.5f)
            {
                DrawProperty(editor, properties, "_DissolveAmount");
            }
        }

        EditorGUILayout.Space(4);
        renderFold = EditorGUILayout.Foldout(renderFold, "渲染快捷设置", true, EditorStyles.foldoutHeader);
        if(renderFold)
        {
            Popup(material, "_BlendMode", "混合模式", new [] {"Alpha透明", "Add叠加", "Soft Add柔和叠加", "Premultiply预乘Alpha"});
            Popup(material, "_CullDisplayMode", "显示面模式", new [] {"双面显示", "显示正面", "显示背面"});
            DrawProperty(editor, properties, "_ZWriteToggle");
            DrawProperty(editor, properties, "_RenderQueueValue");
        }

        EditorGUILayout.Space(4);
        EditorGUILayout.HelpBox("轻量模式只保留常用参数。需要完整模块、材质体检、快捷工具、粒子控制与高级功能时，点击上方按钮打开完整控制台。", MessageType.Info);
    }

    private void DrawHeader(Material material)
    {
        Rect row = EditorGUILayout.GetControlRect(false, 30f);
        EditorGUI.DrawRect(row, new Color(0.14f, 0.14f, 0.14f, 1f));

        EditorGUI.LabelField(
            new Rect(row.x + 8f, row.y + 5f, row.width - 90f, row.height - 8f),
            "AruiShader1.0 URP · 轻量材质面板 V1.0",
            EditorStyles.boldLabel
        );

        Rect tag = new Rect(row.xMax - 72f, row.y + 6f, 64f, row.height - 12f);
        GUI.Label(tag, "双模式", EditorStyles.centeredGreyMiniLabel);
    }

    private const float ScrollbarSafetyWidth = 18f;

    private Rect GetSafeControlRect(bool hasLabel, float height)
    {
        Rect row = EditorGUILayout.GetControlRect(hasLabel, height);
        row.width = Mathf.Max(1f, row.width - ScrollbarSafetyWidth);
        return row;
    }

    private void DrawShaderPropertySafe(MaterialEditor editor, Rect rect, MaterialProperty property, string label)
    {
        float oldLabelWidth = EditorGUIUtility.labelWidth;
        float oldFieldWidth = EditorGUIUtility.fieldWidth;

        EditorGUIUtility.labelWidth = Mathf.Clamp(rect.width * 0.26f, 92f, 160f);
        EditorGUIUtility.fieldWidth = 48f;
        editor.ShaderProperty(rect, property, label);
        EditorGUIUtility.labelWidth = oldLabelWidth;
        EditorGUIUtility.fieldWidth = oldFieldWidth;
    }

    private void DrawProperty(MaterialEditor editor, MaterialProperty[] properties, string name)
    {
        MaterialProperty prop = FindProperty(name, properties, false);
        if(prop == null) return;

        if(prop.type == MaterialProperty.PropType.Texture)
        {
            DrawTextureProperty(prop);
            return;
        }

        Rect row = GetSafeControlRect(true, EditorGUIUtility.singleLineHeight + 2f);
        DrawShaderPropertySafe(editor, row, prop, prop.displayName);
    }

    private static Vector2 DrawSafeVector2Row(Rect rect, string label, Vector2 value)
    {
        float gap = rect.width < 240f ? 2f : 3f;
        float mainLabelWidth = Mathf.Clamp(rect.width * 0.16f, 36f, 50f);
        float axisLabelWidth = rect.width < 240f ? 12f : 16f;

        float available = rect.width - mainLabelWidth - gap * 3f;
        float axisFieldWidth = Mathf.Max(42f, available * 0.5f);

        Rect labelRect = new Rect(rect.x, rect.y + 1f, mainLabelWidth, rect.height - 2f);
        Rect xRect = new Rect(labelRect.xMax + gap, rect.y, axisFieldWidth, rect.height);
        Rect yRect = new Rect(xRect.xMax + gap, rect.y, axisFieldWidth, rect.height);

        EditorGUI.LabelField(labelRect, label, EditorStyles.miniLabel);

        float oldLabelWidth = EditorGUIUtility.labelWidth;
        float oldFieldWidth = EditorGUIUtility.fieldWidth;
        EditorGUIUtility.labelWidth = axisLabelWidth;
        EditorGUIUtility.fieldWidth = Mathf.Max(22f, axisFieldWidth - axisLabelWidth - 2f);

        // 使用带 X / Y 标签的 FloatField，恢复 Unity 原生标签拖拽。
        value.x = EditorGUI.FloatField(xRect, new GUIContent("X", "拖动 X 标签可连续调整数值"), value.x);
        value.y = EditorGUI.FloatField(yRect, new GUIContent("Y", "拖动 Y 标签可连续调整数值"), value.y);

        EditorGUIUtility.labelWidth = oldLabelWidth;
        EditorGUIUtility.fieldWidth = oldFieldWidth;
        return value;
    }

    private void DrawTextureProperty(MaterialProperty prop)
    {
        float line = EditorGUIUtility.singleLineHeight;
        float previewSize = 54f;
        Rect row = GetSafeControlRect(false, 114f);

        float rightX = row.x + previewSize + 8f;
        float rightWidth = Mathf.Max(120f, row.xMax - rightX);

        Rect titleRect = new Rect(row.x, row.y + 2f, row.width, line);
        Rect previewRect = new Rect(row.x, row.y + 25f, previewSize, previewSize);
        Rect objectRect = new Rect(rightX, row.y + 25f, rightWidth, line);
        Rect tilingRect = new Rect(rightX, row.y + 50f, rightWidth, line);
        Rect offsetRect = new Rect(rightX, row.y + 76f, rightWidth, line);

        EditorGUI.LabelField(titleRect, prop.displayName);

        bool oldMixedValue = EditorGUI.showMixedValue;
        EditorGUI.showMixedValue = prop.hasMixedValue;

        EditorGUI.BeginChangeCheck();
        Texture nextTexture = (Texture)EditorGUI.ObjectField(previewRect, prop.textureValue, typeof(Texture), false);
        bool textureChanged = EditorGUI.EndChangeCheck();

        EditorGUI.ObjectField(
            objectRect,
            new GUIContent(prop.textureValue != null ? prop.textureValue.name : "未设置贴图"),
            prop.textureValue,
            typeof(Texture),
            false
        );

        Vector4 st = prop.textureScaleAndOffset;
        Vector2 tiling = new Vector2(st.x, st.y);
        Vector2 offset = new Vector2(st.z, st.w);

        EditorGUI.BeginChangeCheck();
        tiling = DrawSafeVector2Row(tilingRect, "Tiling", tiling);
        offset = DrawSafeVector2Row(offsetRect, "Offset", offset);
        bool transformChanged = EditorGUI.EndChangeCheck();

        if(textureChanged)
            prop.textureValue = nextTexture;
        if(transformChanged)
            prop.textureScaleAndOffset = new Vector4(tiling.x, tiling.y, offset.x, offset.y);

        EditorGUI.showMixedValue = oldMixedValue;
    }

    private static void SyncAutomaticRenderRules(Material material)
    {
        if(material == null) return;

        material.SetOverrideTag("RenderType", "Transparent");

        if(material.HasProperty("_UseSpeedLine") && material.HasProperty("_RenderQueueValue") &&
           material.GetFloat("_UseSpeedLine") < 0.5f && Mathf.RoundToInt(material.GetFloat("_RenderQueueValue")) == 3300)
        {
            material.SetFloat("_RenderQueueValue", 3000);
            if(material.renderQueue != 3000)
            {
                material.renderQueue = 3000;
            }
            EditorUtility.SetDirty(material);
            return;
        }

        if(material.HasProperty("_RenderQueueValue"))
        {
            int desiredQueue = Mathf.Clamp(Mathf.RoundToInt(material.GetFloat("_RenderQueueValue")), 2500, 5000);
            if(material.renderQueue < 2500 || material.renderQueue > 5000)
            {
                material.renderQueue = desiredQueue;
                EditorUtility.SetDirty(material);
            }
        }
    }

    private static float Get(Material material, string property)
    {
        return material != null && material.HasProperty(property) ? material.GetFloat(property) : 0f;
    }

    private static void Set(Material material, string property, float value)
    {
        if(material != null && material.HasProperty(property))
        {
            material.SetFloat(property, value);
            EditorUtility.SetDirty(material);
        }
    }

    private static void Popup(Material material, string property, string label, string[] options)
    {
        int current = Mathf.Clamp(Mathf.RoundToInt(Get(material, property)), 0, options.Length - 1);
        int next = EditorGUILayout.Popup(label, current, options);
        if(next == current) return;

        Set(material, property, next);
        if(property == "_BlendMode")
        {
            ApplyBlendState(material, next);
        }
        else if(property == "_CullDisplayMode")
        {
            ApplyCullState(material, next);
        }
    }

    private static void ApplyBlendState(Material material, int mode)
    {
        if(material == null) return;

        if(mode == 0)
        {
            Set(material, "_SrcBlend", (float)BlendMode.SrcAlpha);
            Set(material, "_DstBlend", (float)BlendMode.OneMinusSrcAlpha);
        }
        else if(mode == 1)
        {
            Set(material, "_SrcBlend", (float)BlendMode.SrcAlpha);
            Set(material, "_DstBlend", (float)BlendMode.One);
        }
        else if(mode == 2)
        {
            Set(material, "_SrcBlend", (float)BlendMode.OneMinusDstColor);
            Set(material, "_DstBlend", (float)BlendMode.One);
        }
        else
        {
            Set(material, "_SrcBlend", (float)BlendMode.One);
            Set(material, "_DstBlend", (float)BlendMode.OneMinusSrcAlpha);
        }

        material.SetOverrideTag("RenderType", "Transparent");
        int queue = material.HasProperty("_RenderQueueValue")
            ? Mathf.Clamp(Mathf.RoundToInt(material.GetFloat("_RenderQueueValue")), 2500, 5000)
            : 3000;
        if(material.renderQueue != queue)
        {
            material.renderQueue = queue;
        }
        EditorUtility.SetDirty(material);
    }

    private static void ApplyCullState(Material material, int mode)
    {
        if(material == null) return;

        mode = Mathf.Clamp(mode, 0, 2);
        if(mode == 0)
        {
            Set(material, "_CullMode", 0f);
            Set(material, "_DoubleSided", 1f);
        }
        else if(mode == 1)
        {
            Set(material, "_CullMode", 2f);
            Set(material, "_DoubleSided", 0f);
        }
        else
        {
            Set(material, "_CullMode", 1f);
            Set(material, "_DoubleSided", 0f);
        }
        EditorUtility.SetDirty(material);
    }

}
