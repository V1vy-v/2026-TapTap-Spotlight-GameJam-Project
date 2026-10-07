using UnityEngine;
using UnityEditor;

public class AruiShader10ConsoleWindowV10URP : EditorWindow
{
    private const float MinimumWindowWidth = 360f;
    private const float MinimumWindowHeight = 420f;
    private const float CompactWidth = 500f;
    private const float NarrowWidth = 420f;

    private readonly string[] moduleTabs =
    {
        "概览 / 快捷工具",
        "1. 渲染",
        "2. 材质类型 / Alpha / UV",
        "3. 主贴图 / 颜色 / 主贴图功能",
        "4. 遮罩 / 二级遮罩",
        "5. 溶解",
        "6. 扭曲 / 热浪",
        "7. 边缘与菲尼尔",
        "8. 顶点偏移",
        "9. 速度线 / 屏幕特效",
        "10. Custom Data / 粒子控制"
    };

    private readonly string[] compactModuleTabs =
    {
        "概览",
        "渲染",
        "材质 / Alpha / UV",
        "主贴图 / 颜色",
        "遮罩",
        "溶解",
        "扭曲 / 热浪",
        "边缘 / 菲尼尔",
        "顶点偏移",
        "速度线 / 屏幕",
        "Custom Data"
    };

    private Material targetMaterial;
    private Material cachedMaterial;
    private MaterialEditor materialEditor;
    private Vector2 scrollPosition;
    private bool lockMaterial;
    private int selectedModule;

    private readonly AruiShader10DualModeFullRendererV10URP renderer = new AruiShader10DualModeFullRendererV10URP();

    [MenuItem("Tools/AruiShader1.0 URP/打开完整控制台 V1.0")]
    public static void OpenFromMenu()
    {
        Open(ResolveMaterialFromCurrentSelection());
    }

    public static void Open(Material material)
    {
        AruiShader10ConsoleWindowV10URP window = GetWindow<AruiShader10ConsoleWindowV10URP>("AruiShader1.0 URP V1.0");
        window.minSize = new Vector2(MinimumWindowWidth, MinimumWindowHeight);
        window.targetMaterial = material;
        window.EnsureMaterialEditor();
        window.Show();
        window.Focus();
    }

    private void OnEnable()
    {
        minSize = new Vector2(MinimumWindowWidth, MinimumWindowHeight);
        TryUseSelection();
    }

    private void OnDisable()
    {
        DestroyMaterialEditor();
    }

    private void OnSelectionChange()
    {
        if(!lockMaterial)
        {
            TryUseSelection();
        }
        Repaint();
    }

    private void TryUseSelection()
    {
        Material selected = ResolveMaterialFromCurrentSelection();

        // 选中没有材质的物体时保持当前材质，避免控制台突然变空。
        if(selected == null || selected == targetMaterial) return;

        targetMaterial = selected;
        scrollPosition = Vector2.zero;
        EnsureMaterialEditor();
    }

    private static Material ResolveMaterialFromCurrentSelection()
    {
        Material selectedMaterial = Selection.activeObject as Material;
        if(selectedMaterial != null) return selectedMaterial;

        GameObject selectedGameObject = Selection.activeGameObject;
        if(selectedGameObject == null)
        {
            Component selectedComponent = Selection.activeObject as Component;
            if(selectedComponent != null)
            {
                selectedGameObject = selectedComponent.gameObject;
            }
        }

        if(selectedGameObject == null) return null;

        // 优先读取当前选中粒子系统自身的 ParticleSystemRenderer。
        ParticleSystemRenderer particleRenderer = selectedGameObject.GetComponent<ParticleSystemRenderer>();
        Material particleMaterial = GetFirstMaterial(particleRenderer);
        if(particleMaterial != null) return particleMaterial;

        // 粒子系统启用 Trails 时，如主材质为空则读取轨迹材质。
        if(particleRenderer != null && particleRenderer.trailMaterial != null)
        {
            return particleRenderer.trailMaterial;
        }

        // 兼容直接选中普通 Renderer（MeshRenderer、SkinnedMeshRenderer、TrailRenderer 等）。
        Renderer currentRenderer = selectedGameObject.GetComponent<Renderer>();
        Material rendererMaterial = GetFirstMaterial(currentRenderer);
        if(rendererMaterial != null) return rendererMaterial;

        // 选中粒子系统父节点时，自动寻找第一个子粒子系统材质。
        ParticleSystemRenderer childParticleRenderer =
            selectedGameObject.GetComponentInChildren<ParticleSystemRenderer>(true);
        Material childParticleMaterial = GetFirstMaterial(childParticleRenderer);
        if(childParticleMaterial != null) return childParticleMaterial;

        if(childParticleRenderer != null && childParticleRenderer.trailMaterial != null)
        {
            return childParticleRenderer.trailMaterial;
        }

        // 最后兼容子节点中的普通 Renderer。
        Renderer childRenderer = selectedGameObject.GetComponentInChildren<Renderer>(true);
        return GetFirstMaterial(childRenderer);
    }

    private static Material GetFirstMaterial(Renderer renderer)
    {
        if(renderer == null) return null;

        if(renderer.sharedMaterial != null)
        {
            return renderer.sharedMaterial;
        }

        Material[] materials = renderer.sharedMaterials;
        if(materials == null) return null;

        for(int index = 0; index < materials.Length; index++)
        {
            if(materials[index] != null)
            {
                return materials[index];
            }
        }

        return null;
    }

    private void EnsureMaterialEditor()
    {
        if(targetMaterial == cachedMaterial && materialEditor != null) return;

        DestroyMaterialEditor();
        cachedMaterial = targetMaterial;

        if(targetMaterial != null)
        {
            materialEditor = Editor.CreateEditor(targetMaterial, typeof(MaterialEditor)) as MaterialEditor;
        }
    }

    private void DestroyMaterialEditor()
    {
        if(materialEditor != null)
        {
            DestroyImmediate(materialEditor);
            materialEditor = null;
        }
        cachedMaterial = null;
    }

    private void OnGUI()
    {
        minSize = new Vector2(MinimumWindowWidth, MinimumWindowHeight);

        if(!lockMaterial)
        {
            TryUseSelection();
        }

        DrawHeader();
        DrawMaterialSelector();
        DrawModuleNavigation();

        if(targetMaterial == null)
        {
            EditorGUILayout.Space(8f);
            EditorGUILayout.HelpBox("请选择一个使用 VFX/AruiShader1.0_URP 的材质球，再打开完整控制台。", MessageType.Info);
            return;
        }

        EnsureMaterialEditor();
        if(materialEditor == null) return;

        MaterialProperty[] properties = MaterialEditor.GetMaterialProperties(new Object[] { targetMaterial });

        // Unity 2020 的 EditorGUILayout.BeginScrollView 没有 bool + GUIStyle 的组合重载。
        // 使用最稳定的基础重载，控件宽度已在 Renderer 中预留滚动条安全边距，因此不会再产生横向溢出。
        scrollPosition = EditorGUILayout.BeginScrollView(
            scrollPosition,
            GUILayout.ExpandWidth(true),
            GUILayout.ExpandHeight(true)
        );

        float oldLabelWidth = EditorGUIUtility.labelWidth;
        float oldFieldWidth = EditorGUIUtility.fieldWidth;
        int oldIndentLevel = EditorGUI.indentLevel;

        try
        {
            float usableWidth = Mathf.Max(300f, position.width - 42f);
            EditorGUIUtility.labelWidth = Mathf.Clamp(usableWidth * 0.22f, 96f, 170f);
            EditorGUIUtility.fieldWidth = 50f;
            EditorGUI.indentLevel = 0;

            renderer.DrawConsoleContent(materialEditor, properties, selectedModule);
        }
        finally
        {
            EditorGUIUtility.labelWidth = oldLabelWidth;
            EditorGUIUtility.fieldWidth = oldFieldWidth;
            EditorGUI.indentLevel = oldIndentLevel;
        }

        EditorGUILayout.EndScrollView();
    }

    private void DrawHeader()
    {
        bool compact = position.width < CompactWidth;
        float headerHeight = compact ? 72f : 52f;
        Rect row = EditorGUILayout.GetControlRect(false, headerHeight);

        EditorGUI.DrawRect(row, new Color(0.105f, 0.105f, 0.105f, 1f));
        EditorGUI.DrawRect(new Rect(row.x, row.y, 4f, row.height), new Color(0.36f, 0.68f, 1f, 1f));

        if(compact)
        {
            Rect titleRect = new Rect(row.x + 12f, row.y + 7f, Mathf.Max(1f, row.width - 24f), 20f);
            Rect moduleRect = new Rect(row.x + 12f, row.y + 29f, Mathf.Max(1f, row.width - 24f), 17f);
            Rect statusRect = new Rect(row.x + 12f, row.y + 50f, Mathf.Max(1f, row.width - 24f), 15f);

            EditorGUI.LabelField(titleRect, "AruiShader1.0 URP · 完整控制台 V1.0", EditorStyles.boldLabel);
            EditorGUI.LabelField(
                moduleRect,
                new GUIContent("当前模块：" + moduleTabs[Mathf.Clamp(selectedModule, 0, moduleTabs.Length - 1)]),
                EditorStyles.miniLabel
            );
            EditorGUI.LabelField(statusRect, "自适应窄窗口模式", EditorStyles.centeredGreyMiniLabel);
            return;
        }

        Rect wideTitleRect = new Rect(row.x + 12f, row.y + 7f, Mathf.Max(1f, row.width - 150f), 20f);
        Rect wideModuleRect = new Rect(row.x + 12f, row.y + 28f, Mathf.Max(1f, row.width - 150f), 15f);
        Rect modeRect = new Rect(row.xMax - 118f, row.y + 16f, 106f, 20f);

        EditorGUI.LabelField(wideTitleRect, "AruiShader1.0 URP · 完整控制台 V1.0", EditorStyles.boldLabel);
        EditorGUI.LabelField(
            wideModuleRect,
            new GUIContent("当前模块：" + moduleTabs[Mathf.Clamp(selectedModule, 0, moduleTabs.Length - 1)]),
            EditorStyles.miniLabel
        );
        GUI.Label(modeRect, "URP 完整模式 V1.0", EditorStyles.centeredGreyMiniLabel);
    }

    private void DrawMaterialSelector()
    {
        bool narrow = position.width < NarrowWidth;
        float selectorHeight = narrow ? 56f : 28f;
        Rect row = EditorGUILayout.GetControlRect(false, selectorHeight);

        if(narrow)
        {
            Rect lockRect = new Rect(row.x, row.y + 2f, 56f, 20f);
            Rect labelRect = new Rect(row.x + 62f, row.y + 4f, 42f, 17f);
            Rect fieldRect = new Rect(row.x, row.y + 28f, Mathf.Max(1f, row.width), 22f);

            lockMaterial = GUI.Toggle(lockRect, lockMaterial, "锁定", EditorStyles.miniButton);
            EditorGUI.LabelField(labelRect, "当前材质", EditorStyles.miniLabel);

            EditorGUI.BeginChangeCheck();
            Material nextNarrow = (Material)EditorGUI.ObjectField(fieldRect, targetMaterial, typeof(Material), false);
            if(EditorGUI.EndChangeCheck())
            {
                targetMaterial = nextNarrow;
                EnsureMaterialEditor();
            }
            return;
        }

        Rect wideLockRect = new Rect(row.x, row.y + 3f, 56f, row.height - 6f);
        Rect wideLabelRect = new Rect(row.x + 62f, row.y + 5f, 36f, row.height - 10f);
        Rect wideFieldRect = new Rect(row.x + 102f, row.y + 2f, Mathf.Max(1f, row.width - 102f), row.height - 4f);

        lockMaterial = GUI.Toggle(wideLockRect, lockMaterial, "锁定", EditorStyles.miniButton);
        EditorGUI.LabelField(wideLabelRect, "材质", EditorStyles.miniLabel);

        EditorGUI.BeginChangeCheck();
        Material next = (Material)EditorGUI.ObjectField(wideFieldRect, targetMaterial, typeof(Material), false);
        if(EditorGUI.EndChangeCheck())
        {
            targetMaterial = next;
            EnsureMaterialEditor();
        }
    }

    private void DrawModuleNavigation()
    {
        float availableWidth = Mathf.Max(1f, position.width - 18f);
        int perRow;

        if(availableWidth < 470f)
            perRow = 1;
        else if(availableWidth < 730f)
            perRow = 2;
        else if(availableWidth < 970f)
            perRow = 3;
        else
            perRow = 4;

        float padding = 4f;
        float gap = 3f;
        float buttonHeight = perRow == 1 ? 23f : 24f;
        int rowCount = Mathf.CeilToInt(moduleTabs.Length / (float)perRow);
        float navigationHeight = padding * 2f + rowCount * buttonHeight + Mathf.Max(0, rowCount - 1) * gap;

        Rect row = EditorGUILayout.GetControlRect(false, navigationHeight);
        EditorGUI.DrawRect(row, new Color(0.14f, 0.14f, 0.14f, 1f));

        float buttonWidth = Mathf.Max(1f, (row.width - padding * 2f - gap * (perRow - 1)) / perRow);
        GUIStyle style = new GUIStyle(EditorStyles.miniButton)
        {
            fontSize = perRow == 1 ? 11 : 10,
            alignment = TextAnchor.MiddleCenter,
            clipping = TextClipping.Clip,
            padding = new RectOffset(3, 3, 1, 1)
        };

        for(int index = 0; index < moduleTabs.Length; index++)
        {
            int column = index % perRow;
            int line = index / perRow;
            Rect buttonRect = new Rect(
                row.x + padding + column * (buttonWidth + gap),
                row.y + padding + line * (buttonHeight + gap),
                buttonWidth,
                buttonHeight
            );

            if(selectedModule == index)
            {
                EditorGUI.DrawRect(buttonRect, new Color(0.22f, 0.45f, 0.70f, 1f));
            }

            string displayName = perRow == 1 ? moduleTabs[index] : compactModuleTabs[index];
            GUIContent content = new GUIContent(displayName, moduleTabs[index]);

            if(GUI.Button(buttonRect, content, style))
            {
                selectedModule = index;
                scrollPosition = Vector2.zero;
                GUI.FocusControl(null);
                Repaint();
            }
        }
    }
}
