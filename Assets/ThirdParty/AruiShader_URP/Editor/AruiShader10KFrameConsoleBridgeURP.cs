using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

// 控制台 K帧桥接。
// 单个粒子可独立记录；若其属于主层级 K帧组，则自动记录到根节点的一条统一 Animation Track。
public static class AruiShader10KFrameConsoleBridgeURP
{
    private static AruiShader10KFrameControllerURP currentController;
    private static AruiShader10HierarchyKFrameGroupURP currentHierarchy;
    private static Material currentMaterial;

    public static bool IsActive
    {
        get { return currentController != null && currentController.启用K帧控制; }
    }

    public static bool IsHierarchyMode
    {
        get
        {
            return currentHierarchy != null &&
                   currentHierarchy.控制台直接记录K帧 &&
                   currentHierarchy.主层级动画片段 != null;
        }
    }

    public static bool CanDirectRecord
    {
        get
        {
            if(!IsActive) return false;

            if(IsHierarchyMode) return true;

            return currentController.控制台直接记录K帧 &&
                   currentController.控制台目标动画片段 != null;
        }
    }

    public static void Begin(Material material)
    {
        currentMaterial = material;
        currentController = ResolveController(material);
        currentHierarchy = ResolveHierarchy(currentController);
    }

    public static void DrawTimelineRecorderPanel()
    {
        if(!IsActive) return;

        EditorGUILayout.Space(3f);
        EditorGUILayout.LabelField(
            IsHierarchyMode ? "主层级统一 Timeline K帧录制" : "单物体 Timeline K帧录制",
            EditorStyles.boldLabel
        );

        if(currentHierarchy != null)
        {
            EditorGUILayout.HelpBox(
                "当前子粒子已接入根节点统一动画系统。控制台改值会写到主层级的一条 Animation Track，路径会自动定位到对应子层级。",
                MessageType.Info
            );

            EditorGUI.BeginChangeCheck();
            currentHierarchy.控制台直接记录K帧 = EditorGUILayout.ToggleLeft(
                "控制台拖动参数时直接写入主层级动画片段",
                currentHierarchy.控制台直接记录K帧
            );
            currentHierarchy.优先使用Timeline播放头 = EditorGUILayout.ToggleLeft(
                "优先使用 Timeline 播放头时间",
                currentHierarchy.优先使用Timeline播放头
            );
            currentHierarchy.备用K帧时间 = Mathf.Max(
                0f,
                EditorGUILayout.FloatField("备用K帧时间（秒）", currentHierarchy.备用K帧时间)
            );
            currentHierarchy.K帧帧率 = Mathf.Max(
                1f,
                EditorGUILayout.FloatField("K帧帧率", currentHierarchy.K帧帧率)
            );

            EditorGUILayout.ObjectField(
                "主层级动画片段",
                currentHierarchy.主层级动画片段,
                typeof(AnimationClip),
                false
            );
            EditorGUILayout.ObjectField(
                "Timeline导演",
                currentHierarchy.Timeline导演,
                typeof(PlayableDirector),
                true
            );

            if(EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(currentHierarchy, "调整主层级 Timeline K帧设置");
                currentHierarchy.同步配置到全部子层级();
                EditorUtility.SetDirty(currentHierarchy);
            }

            if(GUILayout.Button("定位主层级 K帧组"))
            {
                Selection.activeObject = currentHierarchy.gameObject;
                EditorGUIUtility.PingObject(currentHierarchy.gameObject);
            }

            float time = GetRecordTime();
            string state = CanDirectRecord
                ? "已就绪：控制台修改会写入主层级动画片段。当前 Timeline 写帧时间：" + time.ToString("F3") + " 秒。"
                : "主层级动画系统尚未创建。请选中根节点，点击“创建并绑定主层级统一 Timeline 动画轨道”。";

            EditorGUILayout.HelpBox(state, CanDirectRecord ? MessageType.Info : MessageType.Warning);
            return;
        }

        EditorGUI.BeginChangeCheck();
        currentController.控制台直接记录K帧 = EditorGUILayout.ToggleLeft(
            "控制台拖动参数时直接写入动画片段",
            currentController.控制台直接记录K帧
        );
        currentController.优先使用Timeline播放头 = EditorGUILayout.ToggleLeft(
            "优先使用 Timeline 播放头时间",
            currentController.优先使用Timeline播放头
        );
        currentController.控制台K帧时间 = Mathf.Max(
            0f,
            EditorGUILayout.FloatField("备用K帧时间（秒）", currentController.控制台K帧时间)
        );
        currentController.控制台K帧帧率 = Mathf.Max(
            1f,
            EditorGUILayout.FloatField("K帧帧率", currentController.控制台K帧帧率)
        );

        EditorGUILayout.ObjectField(
            "目标动画片段",
            currentController.控制台目标动画片段,
            typeof(AnimationClip),
            false
        );

        if(EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(currentController, "调整单物体 Timeline K帧设置");
            EditorUtility.SetDirty(currentController);
        }

        if(GUILayout.Button("创建并绑定单物体 Timeline K帧轨道"))
        {
            AruiShader10TimelineKFrameUtilityURP.CreateAndBindTrack(currentController);
        }

        string standaloneState = CanDirectRecord
            ? "已就绪：控制台会写入当前单物体动画片段。"
            : "请指定目标动画片段或点击创建单物体 Timeline K帧轨道。";

        EditorGUILayout.HelpBox(standaloneState, CanDirectRecord ? MessageType.Info : MessageType.Warning);
    }

    public static bool TryDrawProperty(MaterialProperty property, Rect rect, string label)
    {
        if(!IsActive || property == null || IsRenderStateProperty(property.name)) return false;

        if(property.type == MaterialProperty.PropType.Float || property.type == MaterialProperty.PropType.Range)
        {
            float current = GetFloatTrackValue(property.name, property.floatValue);
            EditorGUI.BeginChangeCheck();

            float next;
            if(IsToggleLikeProperty(property.name))
            {
                bool value = EditorGUI.Toggle(rect, new GUIContent(label, "主层级统一 Timeline K帧"), current > 0.5f);
                next = value ? 1f : 0f;
            }
            else if(property.type == MaterialProperty.PropType.Range)
            {
                next = EditorGUI.Slider(rect, new GUIContent(label, "拖动会写入当前 Timeline 播放头位置。"), current, property.rangeLimits.x, property.rangeLimits.y);
            }
            else
            {
                next = EditorGUI.FloatField(rect, new GUIContent(label, "拖动标签或输入会写入当前 Timeline 播放头位置。"), current);
            }

            if(EditorGUI.EndChangeCheck())
            {
                SetFloatTrackValue(property.name, next, "控制台 Timeline K帧：" + label);
            }

            return true;
        }

        if(property.type == MaterialProperty.PropType.Color)
        {
            Color current = GetColorTrackValue(property.name, property.colorValue);
            EditorGUI.BeginChangeCheck();
            Color next = EditorGUI.ColorField(
                rect,
                new GUIContent(label, "颜色会写入当前 Timeline 播放头位置。"),
                current,
                true,
                true,
                true
            );

            if(EditorGUI.EndChangeCheck())
            {
                SetColorTrackValue(property.name, next, "控制台 Timeline K帧：" + label);
            }

            return true;
        }

        return false;
    }

    public static bool TryGetTextureValue(string propertyName, Texture fallback, out Texture value)
    {
        value = fallback;
        if(!IsActive) return false;

        SerializedObject serialized = new SerializedObject(currentController);
        SerializedProperty track = FindTrack(serialized, "贴图轨道", propertyName, false);
        if(track == null) return true;

        SerializedProperty enabled = track.FindPropertyRelative("启用");
        SerializedProperty texture = track.FindPropertyRelative("贴图");
        if(enabled != null && enabled.boolValue && texture != null)
        {
            value = texture.objectReferenceValue as Texture;
        }

        return true;
    }

    public static bool TryGetVectorValue(string propertyName, Vector4 fallback, out Vector4 value)
    {
        value = fallback;
        if(!IsActive) return false;

        SerializedObject serialized = new SerializedObject(currentController);
        SerializedProperty track = FindTrack(serialized, "向量轨道", propertyName, false);
        if(track == null) return true;

        SerializedProperty enabled = track.FindPropertyRelative("启用");
        SerializedProperty vector = track.FindPropertyRelative("值");
        if(enabled != null && enabled.boolValue && vector != null)
        {
            value = vector.vector4Value;
        }

        return true;
    }

    public static void SetTextureTrackValue(string propertyName, Texture value, string undoName)
    {
        if(!IsActive) return;

        SerializedObject serialized = new SerializedObject(currentController);
        SerializedProperty track = FindTrack(serialized, "贴图轨道", propertyName, true);
        if(track == null) return;

        SerializedProperty enabled = track.FindPropertyRelative("启用");
        SerializedProperty texture = track.FindPropertyRelative("贴图");
        if(enabled == null || texture == null) return;

        Undo.RecordObject(currentController, undoName);
        enabled.boolValue = true;
        texture.objectReferenceValue = value;
        string path = texture.propertyPath;
        serialized.ApplyModifiedProperties();

        RecordObjectReferenceKey(path, value);
        currentController.应用当前K帧值();
        EditorUtility.SetDirty(currentController);
    }

    public static void SetVectorTrackValue(string propertyName, Vector4 value, string undoName)
    {
        if(!IsActive) return;

        SerializedObject serialized = new SerializedObject(currentController);
        SerializedProperty track = FindTrack(serialized, "向量轨道", propertyName, true);
        if(track == null) return;

        SerializedProperty enabled = track.FindPropertyRelative("启用");
        SerializedProperty vector = track.FindPropertyRelative("值");
        if(enabled == null || vector == null) return;

        Undo.RecordObject(currentController, undoName);
        enabled.boolValue = true;
        vector.vector4Value = value;
        string path = vector.propertyPath;
        serialized.ApplyModifiedProperties();

        RecordVectorKeys(path, value);
        currentController.应用当前K帧值();
        EditorUtility.SetDirty(currentController);
    }

    public static void ResetToMaterialBase(string propertyName, MaterialProperty.PropType type, object fallback)
    {
        if(!IsActive || currentMaterial == null) return;

        if(type == MaterialProperty.PropType.Texture)
        {
            Texture texture = currentMaterial.HasProperty(propertyName)
                ? currentMaterial.GetTexture(propertyName)
                : fallback as Texture;
            SetTextureTrackValue(propertyName, texture, "控制台恢复贴图基础值");
            return;
        }

        if(type == MaterialProperty.PropType.Color)
        {
            Color color = currentMaterial.HasProperty(propertyName)
                ? currentMaterial.GetColor(propertyName)
                : (Color)fallback;
            SetColorTrackValue(propertyName, color, "控制台恢复颜色基础值");
            return;
        }

        float number = currentMaterial.HasProperty(propertyName)
            ? currentMaterial.GetFloat(propertyName)
            : (float)fallback;
        SetFloatTrackValue(propertyName, number, "控制台恢复数值基础值");
    }

    private static float GetFloatTrackValue(string propertyName, float fallback)
    {
        SerializedObject serialized = new SerializedObject(currentController);
        SerializedProperty track = FindTrack(serialized, "浮点轨道", propertyName, false);
        if(track == null) return fallback;

        SerializedProperty enabled = track.FindPropertyRelative("启用");
        SerializedProperty value = track.FindPropertyRelative("值");
        return enabled != null && enabled.boolValue && value != null ? value.floatValue : fallback;
    }

    private static Color GetColorTrackValue(string propertyName, Color fallback)
    {
        SerializedObject serialized = new SerializedObject(currentController);
        SerializedProperty track = FindTrack(serialized, "颜色轨道", propertyName, false);
        if(track == null) return fallback;

        SerializedProperty enabled = track.FindPropertyRelative("启用");
        SerializedProperty color = track.FindPropertyRelative("颜色");
        return enabled != null && enabled.boolValue && color != null ? color.colorValue : fallback;
    }

    private static void SetFloatTrackValue(string propertyName, float value, string undoName)
    {
        if(!IsActive) return;

        SerializedObject serialized = new SerializedObject(currentController);
        SerializedProperty track = FindTrack(serialized, "浮点轨道", propertyName, true);
        if(track == null) return;

        SerializedProperty enabled = track.FindPropertyRelative("启用");
        SerializedProperty number = track.FindPropertyRelative("值");
        if(enabled == null || number == null) return;

        Undo.RecordObject(currentController, undoName);
        enabled.boolValue = true;
        number.floatValue = value;
        string path = number.propertyPath;
        serialized.ApplyModifiedProperties();

        RecordFloatKey(path, value);
        currentController.应用当前K帧值();
        EditorUtility.SetDirty(currentController);
    }

    private static void SetColorTrackValue(string propertyName, Color value, string undoName)
    {
        if(!IsActive) return;

        SerializedObject serialized = new SerializedObject(currentController);
        SerializedProperty track = FindTrack(serialized, "颜色轨道", propertyName, true);
        if(track == null) return;

        SerializedProperty enabled = track.FindPropertyRelative("启用");
        SerializedProperty color = track.FindPropertyRelative("颜色");
        if(enabled == null || color == null) return;

        Undo.RecordObject(currentController, undoName);
        enabled.boolValue = true;
        color.colorValue = value;
        string path = color.propertyPath;
        serialized.ApplyModifiedProperties();

        RecordColorKeys(path, value);
        currentController.应用当前K帧值();
        EditorUtility.SetDirty(currentController);
    }

    private static SerializedProperty FindTrack(SerializedObject serialized, string listName, string shaderPropertyName, bool create)
    {
        SerializedProperty list = serialized.FindProperty(listName);
        if(list == null || !list.isArray) return null;

        for(int index = 0; index < list.arraySize; index++)
        {
            SerializedProperty item = list.GetArrayElementAtIndex(index);
            SerializedProperty shaderNameProperty = item.FindPropertyRelative("Shader属性名");
            if(shaderNameProperty != null && shaderNameProperty.stringValue == shaderPropertyName)
            {
                return item;
            }
        }

        if(!create) return null;

        list.arraySize++;
        SerializedProperty created = list.GetArrayElementAtIndex(list.arraySize - 1);
        SerializedProperty enabledProperty = created.FindPropertyRelative("启用");
        SerializedProperty createdShaderNameProperty = created.FindPropertyRelative("Shader属性名");
        if(enabledProperty != null) enabledProperty.boolValue = true;
        if(createdShaderNameProperty != null) createdShaderNameProperty.stringValue = shaderPropertyName;
        return created;
    }

    private static AruiShader10KFrameControllerURP ResolveController(Material material)
    {
        if(material == null) return null;

        GameObject selected = Selection.activeGameObject;
        if(selected != null)
        {
            AruiShader10KFrameControllerURP result = FindMatchingController(selected.GetComponentsInParent<AruiShader10KFrameControllerURP>(true), material);
            if(result != null) return result;

            result = FindMatchingController(selected.GetComponentsInChildren<AruiShader10KFrameControllerURP>(true), material);
            if(result != null) return result;
        }

        return FindMatchingController(Resources.FindObjectsOfTypeAll<AruiShader10KFrameControllerURP>(), material);
    }

    private static AruiShader10HierarchyKFrameGroupURP ResolveHierarchy(AruiShader10KFrameControllerURP controller)
    {
        if(controller == null) return null;

        // Unity 2020 没有 GetComponentInParent<T>(bool includeInactive) 重载。
        AruiShader10HierarchyKFrameGroupURP[] parentGroups = controller.GetComponentsInParent<AruiShader10HierarchyKFrameGroupURP>(true);
        if(parentGroups != null)
        {
            for(int parentIndex = 0; parentIndex < parentGroups.Length; parentIndex++)
            {
                AruiShader10HierarchyKFrameGroupURP group = parentGroups[parentIndex];
                if(group == null) continue;
                group.扫描子层级K帧控制器();
                if(group.包含控制器(controller)) return group;
            }
        }

        AruiShader10HierarchyKFrameGroupURP[] groups = Resources.FindObjectsOfTypeAll<AruiShader10HierarchyKFrameGroupURP>();
        for(int index = 0; index < groups.Length; index++)
        {
            AruiShader10HierarchyKFrameGroupURP candidate = groups[index];
            if(candidate == null || !candidate.gameObject.scene.IsValid()) continue;
            candidate.扫描子层级K帧控制器();
            if(candidate.包含控制器(controller)) return candidate;
        }

        return null;
    }

    private static AruiShader10KFrameControllerURP FindMatchingController(AruiShader10KFrameControllerURP[] controllers, Material material)
    {
        if(controllers == null) return null;

        for(int index = 0; index < controllers.Length; index++)
        {
            AruiShader10KFrameControllerURP controller = controllers[index];
            if(controller == null || !controller.启用K帧控制) continue;
            if(!controller.gameObject.scene.IsValid()) continue;
            if(controller.获取当前材质() == material) return controller;
        }

        return null;
    }

    private static bool IsToggleLikeProperty(string propertyName)
    {
        return propertyName.StartsWith("_Use") ||
               propertyName.StartsWith("_Enable") ||
               propertyName.Contains("Toggle") ||
               propertyName.Contains("Invert") ||
               propertyName.Contains("DoubleSided") ||
               propertyName.Contains("ClipEnable") ||
               propertyName.Contains("Affect") ||
               propertyName.Contains("Loop") ||
               propertyName.Contains("RemoveBlack") ||
               propertyName.Contains("ForceHollow") ||
               propertyName.Contains("HideBackface");
    }

    private static bool IsRenderStateProperty(string propertyName)
    {
        return propertyName == "_BlendMode" ||
               propertyName == "_SrcBlend" ||
               propertyName == "_DstBlend" ||
               propertyName == "_ZWrite" ||
               propertyName == "_ZTestMode" ||
               propertyName == "_ZTestToggle" ||
               propertyName == "_CullMode" ||
               propertyName == "_CullDisplayMode" ||
               propertyName == "_RenderQueueValue" ||
               propertyName.StartsWith("_Stencil");
    }

    private static AnimationClip GetRecordClip()
    {
        if(IsHierarchyMode) return currentHierarchy.主层级动画片段;
        return CanDirectRecord ? currentController.控制台目标动画片段 : null;
    }

    private static Transform GetBindingRoot()
    {
        if(IsHierarchyMode) return currentHierarchy.transform;
        return currentController != null ? currentController.transform : null;
    }

    private static string GetBindingPath()
    {
        Transform root = GetBindingRoot();
        if(root == null || currentController == null) return string.Empty;
        if(currentController.transform == root) return string.Empty;

        List<string> names = new List<string>();
        Transform cursor = currentController.transform;
        while(cursor != null && cursor != root)
        {
            names.Add(cursor.name);
            cursor = cursor.parent;
        }

        if(cursor != root) return string.Empty;

        names.Reverse();
        return string.Join("/", names.ToArray());
    }

    private static PlayableDirector GetRecordDirector()
    {
        if(IsHierarchyMode) return currentHierarchy.Timeline导演;
        return currentController != null ? currentController.Timeline导演 : null;
    }

    private static bool ShouldUseTimelineTime()
    {
        if(IsHierarchyMode)
        {
            return currentHierarchy.优先使用Timeline播放头 &&
                   currentHierarchy.Timeline导演 != null;
        }

        return currentController != null &&
               currentController.优先使用Timeline播放头 &&
               currentController.Timeline导演 != null;
    }

    private static float GetRecordTime()
    {
        float fallback = IsHierarchyMode
            ? currentHierarchy.备用K帧时间
            : (currentController != null ? currentController.控制台K帧时间 : 0f);

        AnimationClip clip = GetRecordClip();
        PlayableDirector director = GetRecordDirector();

        if(!ShouldUseTimelineTime() || clip == null || director == null)
        {
            return QuantizeTime(fallback);
        }

        TimelineClip timelineClip = IsHierarchyMode
            ? AruiShader10HierarchyTimelineUtilityURP.FindTimelineClip(director, clip)
            : AruiShader10TimelineKFrameUtilityURP.FindTimelineClip(director, clip);

        if(timelineClip == null) return QuantizeTime(fallback);

        double localTime = director.time - timelineClip.start;
        if(localTime < 0d) localTime = 0d;
        if(timelineClip.duration > 0d && localTime > timelineClip.duration) localTime = timelineClip.duration;

        return QuantizeTime((float)localTime);
    }

    private static float QuantizeTime(float time)
    {
        float frameRate = IsHierarchyMode
            ? Mathf.Max(1f, currentHierarchy.K帧帧率)
            : Mathf.Max(1f, currentController.控制台K帧帧率);

        return Mathf.Round(Mathf.Max(0f, time) * frameRate) / frameRate;
    }

    private static void RecordFloatKey(string propertyPath, float value)
    {
        AnimationClip clip = GetRecordClip();
        if(clip == null) return;

        Undo.RegisterCompleteObjectUndo(clip, "控制台记录 Timeline 浮点关键帧");

        EditorCurveBinding binding = EditorCurveBinding.FloatCurve(
            GetBindingPath(),
            currentController.GetType(),
            propertyPath
        );

        AnimationCurve curve = AnimationUtility.GetEditorCurve(clip, binding) ?? new AnimationCurve();
        SetCurveKey(curve, GetRecordTime(), value);
        AnimationUtility.SetEditorCurve(clip, binding, curve);

        EditorUtility.SetDirty(clip);
        EvaluateTimeline();
    }

    private static void RecordColorKeys(string propertyPath, Color value)
    {
        RecordFloatKey(propertyPath + ".r", value.r);
        RecordFloatKey(propertyPath + ".g", value.g);
        RecordFloatKey(propertyPath + ".b", value.b);
        RecordFloatKey(propertyPath + ".a", value.a);
    }

    private static void RecordVectorKeys(string propertyPath, Vector4 value)
    {
        RecordFloatKey(propertyPath + ".x", value.x);
        RecordFloatKey(propertyPath + ".y", value.y);
        RecordFloatKey(propertyPath + ".z", value.z);
        RecordFloatKey(propertyPath + ".w", value.w);
    }

    private static void RecordObjectReferenceKey(string propertyPath, UnityEngine.Object value)
    {
        AnimationClip clip = GetRecordClip();
        if(clip == null) return;

        Undo.RegisterCompleteObjectUndo(clip, "控制台记录 Timeline 贴图关键帧");

        EditorCurveBinding binding = EditorCurveBinding.PPtrCurve(
            GetBindingPath(),
            currentController.GetType(),
            propertyPath
        );

        List<ObjectReferenceKeyframe> keys = new List<ObjectReferenceKeyframe>();
        ObjectReferenceKeyframe[] oldKeys = AnimationUtility.GetObjectReferenceCurve(clip, binding);
        if(oldKeys != null) keys.AddRange(oldKeys);

        float time = GetRecordTime();
        bool replaced = false;

        for(int index = 0; index < keys.Count; index++)
        {
            if(Mathf.Abs(keys[index].time - time) < 0.0001f)
            {
                ObjectReferenceKeyframe key = keys[index];
                key.value = value;
                keys[index] = key;
                replaced = true;
                break;
            }
        }

        if(!replaced)
        {
            keys.Add(new ObjectReferenceKeyframe { time = time, value = value });
        }

        keys.Sort((left, right) => left.time.CompareTo(right.time));
        AnimationUtility.SetObjectReferenceCurve(clip, binding, keys.ToArray());

        EditorUtility.SetDirty(clip);
        EvaluateTimeline();
    }

    private static void SetCurveKey(AnimationCurve curve, float time, float value)
    {
        for(int index = 0; index < curve.length; index++)
        {
            if(Mathf.Abs(curve.keys[index].time - time) < 0.0001f)
            {
                Keyframe key = curve.keys[index];
                key.value = value;
                curve.MoveKey(index, key);
                return;
            }
        }

        curve.AddKey(new Keyframe(time, value));
    }

    private static void EvaluateTimeline()
    {
        PlayableDirector director = GetRecordDirector();
        if(director != null) director.Evaluate();
    }
}
