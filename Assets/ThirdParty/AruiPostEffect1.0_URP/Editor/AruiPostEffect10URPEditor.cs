#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

/// <summary>
/// AruiPostEffect1.0 URP 主控制器中文 Inspector。
/// 每个功能只保留一层折叠；展开后直接显示参数，不再出现空白二级折叠。
/// </summary>
[CustomEditor(typeof(AruiPostEffect10URP))]
public sealed class AruiPostEffect10URPEditor : Editor
{
    private SerializedProperty enableEffect;
    private SerializedProperty totalStrength;
    private SerializedProperty editorPreview;
    private SerializedProperty shader;
    private SerializedProperty shake;
    private SerializedProperty colorSplit;
    private SerializedProperty fullScreenBlur;
    private SerializedProperty letterbox;
    private SerializedProperty radialRays;
    private SerializedProperty wideAngle;
    private SerializedProperty vignette;
    private SerializedProperty skillBlackWhite;
    private SerializedProperty manga;

    private bool showTutorial = true;
    private bool openShake = true;
    private bool openColorSplit;
    private bool openBlur;
    private bool openLetterbox;
    private bool openRadialRays;
    private bool openWideAngle;
    private bool openVignette;
    private bool openSkillBlackWhite;
    private bool openManga;

    private void OnEnable()
    {
        enableEffect = serializedObject.FindProperty("开启后效");
        totalStrength = serializedObject.FindProperty("总强度");
        editorPreview = serializedObject.FindProperty("编辑器实时预览");
        shader = serializedObject.FindProperty("后效Shader");
        shake = serializedObject.FindProperty("震屏");
        colorSplit = serializedObject.FindProperty("色相分离");
        fullScreenBlur = serializedObject.FindProperty("全屏模糊");
        letterbox = serializedObject.FindProperty("电影黑边框");
        radialRays = serializedObject.FindProperty("全屏放射线");
        wideAngle = serializedObject.FindProperty("广角");
        vignette = serializedObject.FindProperty("暗角");
        skillBlackWhite = serializedObject.FindProperty("技能黑白闪");
        manga = serializedObject.FindProperty("漫画滤镜");
    }

    public override void OnInspectorGUI()
    {
        AruiPostEffect10URP controller = target as AruiPostEffect10URP;
        if (controller == null)
        {
            return;
        }

        serializedObject.Update();

        EditorGUILayout.Space(4f);
        EditorGUILayout.LabelField("AruiPostEffect1.0 URP", EditorStyles.boldLabel);
        EditorGUILayout.HelpBox(
            "Built-in 默认管线 · 相机最终画面后效。\n" +
            "每个功能展开后直接显示参数。手动预览与动画/Timeline影响均为开关；Timeline Clip 播放时会自动关闭该功能的手动预览，并切换到 Timeline 控制模式。",
            MessageType.Info);

        EditorGUILayout.BeginHorizontal();
        showTutorial = GUILayout.Toggle(showTutorial, "显示简易教程", "Button");
        if (GUILayout.Button("关闭所有手动预览"))
        {
            serializedObject.ApplyModifiedProperties();
            Undo.RecordObject(controller, "关闭 AruiPostEffect1.0 URP 手动预览");
            controller.关闭所有手动预览();
            EditorUtility.SetDirty(controller);
            serializedObject.Update();
        }
        if (GUILayout.Button("补齐后效 Shader"))
        {
            serializedObject.ApplyModifiedProperties();
            Undo.RecordObject(controller, "补齐 AruiPostEffect1.0 URP Shader");
            controller.后效Shader = Shader.Find(AruiPostEffect10URP.ShaderName);
            EditorUtility.SetDirty(controller);
            serializedObject.Update();
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        EditorGUILayout.LabelField("全局控制", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(enableEffect, new GUIContent("开启屏幕后效"));
        EditorGUILayout.PropertyField(totalStrength, new GUIContent("后效总强度"));
        EditorGUILayout.PropertyField(editorPreview, new GUIContent("编辑器实时预览"));
        EditorGUILayout.PropertyField(shader, new GUIContent("后效 Shader"));
        if (controller.后效Shader == null)
        {
            EditorGUILayout.HelpBox("找不到后效 Shader。请确认 Runtime/AruiPostEffect1_0_2020.shader 已导入，并且 Console 没有 Shader 报错。", MessageType.Error);
        }
        EditorGUILayout.EndVertical();

        DrawEffectSection(
            "震屏", shake, ref openShake, controller, AruiPostEffect10URP.后效功能.震屏,
            controller.应用震屏预设,
            new[] { "位移强度", "横向与纵向振幅", "震动频率", "画面旋转角度", "震屏中心" },
            "适合命中、重击、爆炸。此版本是屏幕 UV 震屏，不移动相机 Transform，不会和相机动画或 Cinemachine 位移打架。");

        DrawEffectSection(
            "色相分离 · RGB", colorSplit, ref openColorSplit, controller, AruiPostEffect10URP.后效功能.色相分离,
            controller.应用色相分离预设,
            new[] { "分离方式", "分离强度", "分离中心", "固定方向角度", "边缘增强", "红蓝偏移比例" },
            "适合空间撕裂、能量爆发、科技故障。普通命中建议“分离强度”约 0.003 到 0.007；数值过高会让画面发脏。");

        DrawEffectSection(
            "技能黑白闪 · 全屏", skillBlackWhite, ref openSkillBlackWhite, controller, AruiPostEffect10URP.后效功能.技能黑白闪,
            controller.应用技能黑白闪预设,
            new[] { "闪屏模式", "暗部颜色", "亮部颜色", "黑白阈值", "黑白边缘柔和度", "黑白对比度", "黑白往返次数", "手动闪屏进度", "动画或Timeline进度", "轮廓边缘强度", "轮廓边缘宽度", "轮廓边缘阈值" },
            "直接处理主相机最终画面，所以场景、角色、武器与粒子会一起产生技能黑白闪；不使用 Layer 或第二相机。手动预览可调“手动闪屏进度”；Timeline 会自动读取 Clip 播放进度。黑白反相往返中：黑 → 白 → 黑 = 1 次往返；2 次为黑 → 白 → 黑 → 白 → 黑。需要在 Timeline 单独调整黑白颜色时，勾选 Clip 的“Timeline 独立控制”，即可使用 Clip 内的“暗部颜色 / 亮部颜色”。建议 Clip 长度 0.08 到 0.18 秒。 ");

        DrawEffectSection(
            "漫画滤镜 · 黑白放射", manga, ref openManga, controller, AruiPostEffect10URP.后效功能.漫画滤镜,
            controller.应用漫画滤镜预设,
            new[] { "漫画模式", "墨色", "纸张色", "黑白阈值", "黑白柔和度", "黑白对比度", "爆发中心", "放射笔刷数量", "笔刷宽度", "笔刷长度", "中心留白范围", "笔刷不规则度", "笔刷断裂", "笔刷强度", "轮廓边缘强度", "轮廓边缘宽度", "轮廓边缘阈值" },
            "参考黑白速度线漫画构图：先用“黑白放射漫画”预设，再把“爆发中心”移动到角色或爆点附近。想让主体更清楚，先减小“中心留白范围”；想让笔刷更像墨迹，增加“笔刷不规则度”和“笔刷断裂”。Timeline 可直接作为独立 Clip 使用。 ");

        DrawEffectSection(
            "全屏模糊", fullScreenBlur, ref openBlur, controller, AruiPostEffect10URP.后效功能.全屏模糊,
            controller.应用全屏模糊预设,
            new[] { "模糊方式", "模糊中心", "模糊半径像素", "模糊强化倍数", "模糊混合", "径向采样次数" },
            "单相机最终画面模糊：场景、角色、特效都会一起模糊，不使用第二相机。“爆发拉伸”建议只用 0.08 到 0.2 秒。 ");

        DrawEffectSection(
            "电影黑边框", letterbox, ref openLetterbox, controller, AruiPostEffect10URP.后效功能.电影黑边框,
            controller.应用电影黑边框预设,
            new[] { "黑边颜色", "上下黑边高度", "左右黑边宽度", "黑边边缘柔和度", "黑边不透明度" },
            "电影黑边框用于镜头演出与大招收束。只做上下宽银幕时，把“左右黑边宽度”保持为 0。Timeline Clip 可用强度曲线控制黑边出现和收回。");

        DrawEffectSection(
            "全屏放射线", radialRays, ref openRadialRays, controller, AruiPostEffect10URP.后效功能.全屏放射线,
            controller.应用全屏放射线预设,
            new[] { "放射模式", "放射线颜色", "放射中心", "放射线数量", "放射线宽度", "放射线发光强度", "放射线旋转角度", "放射线流动速度", "放射线闪烁", "背景压暗" },
            "独立功能：全屏放射或中心爆发放射。适合大招爆发、角色切入、终结技冲击。Timeline 可作为独立 Clip 使用。 ");

        DrawEffectSection(
            "广角", wideAngle, ref openWideAngle, controller, AruiPostEffect10URP.后效功能.广角,
            controller.应用广角预设,
            new[] { "广角方式", "目标相机视野角", "屏幕广角畸变强度", "广角中心" },
            "真实相机视野角会临时修改本相机 Field Of View，效果结束后自动恢复；相机本身有 FOV 动画时，推荐使用“屏幕广角畸变”。");

        DrawEffectSection(
            "暗角 · 形状可选", vignette, ref openVignette, controller, AruiPostEffect10URP.后效功能.暗角,
            controller.应用暗角预设,
            new[] { "暗角中心", "暗角颜色", "暗角强度", "暗角范围", "暗角柔和度", "形状选择", "横向拉伸", "纵向拉伸", "圆角矩形圆角" },
            "形状选择：圆形适合角色聚焦；矩形适合电影式边缘压暗；菱形适合斩击卡帧；圆角矩形适合大招收束。先调强度，再调范围与柔和度。 ");

        serializedObject.ApplyModifiedProperties();

        EditorGUILayout.Space(5f);
        EditorGUILayout.HelpBox(
            "Timeline 使用：添加“AruiPostEffect1.0 URP 后效轨道” → 将相机上的 AruiPostEffect1.0 URP 控制器组件拖到轨道绑定槽 → 右键轨道添加后效片段。\n" +
            "Clip 左边缘 = 触发时间；Clip 长度 = 持续时间。Clip 播放时会自动关闭对应功能的“手动预览”，并开启“受动画或Timeline影响”。技能黑白闪会自动读取 Clip 的播放进度，不需要额外 K 黑白往返进度。",
            MessageType.None);
    }

    private void DrawEffectSection(
        string title,
        SerializedProperty property,
        ref bool foldout,
        AruiPostEffect10URP controller,
        AruiPostEffect10URP.后效功能 effect,
        System.Action applyPreset,
        string[] parameterNames,
        string tutorial)
    {
        EditorGUILayout.Space(3f);
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        foldout = EditorGUILayout.Foldout(foldout, title, true);
        if (foldout)
        {
            DrawControlMode(property);

            SerializedProperty preset = property.FindPropertyRelative("常用预设");
            EditorGUILayout.Space(2f);
            EditorGUILayout.LabelField("预设与参数", EditorStyles.boldLabel);
            EditorGUILayout.BeginHorizontal();
            if (preset != null)
            {
                EditorGUILayout.PropertyField(preset, new GUIContent("常用预设"));
            }
            if (GUILayout.Button("应用当前预设", GUILayout.Width(112f)))
            {
                serializedObject.ApplyModifiedProperties();
                Undo.RecordObject(controller, "应用 AruiPostEffect1.0 URP 预设");
                applyPreset();
                EditorUtility.SetDirty(controller);
                serializedObject.Update();
            }
            EditorGUILayout.EndHorizontal();

            DrawParameterFields(property, parameterNames);

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("只预览本功能"))
            {
                serializedObject.ApplyModifiedProperties();
                Undo.RecordObject(controller, "预览 AruiPostEffect1.0 URP 功能");
                controller.只预览功能(effect);
                EditorUtility.SetDirty(controller);
                serializedObject.Update();
            }
            EditorGUILayout.EndHorizontal();

            if (showTutorial)
            {
                EditorGUILayout.HelpBox(tutorial, MessageType.None);
            }
        }
        EditorGUILayout.EndVertical();
    }

    private void DrawControlMode(SerializedProperty property)
    {
        SerializedProperty manualPreview = property.FindPropertyRelative("手动预览");
        SerializedProperty timelineControl = property.FindPropertyRelative("受动画或Timeline影响");
        SerializedProperty manualStrength = property.FindPropertyRelative("手动预览强度");
        SerializedProperty animationStrength = property.FindPropertyRelative("动画或Timeline强度");

        EditorGUILayout.LabelField("播放控制", EditorStyles.boldLabel);

        bool manualChanged = false;
        bool timelineChanged = false;

        if (manualPreview != null)
        {
            EditorGUI.BeginChangeCheck();
            bool value = EditorGUILayout.ToggleLeft("手动预览（默认开启）", manualPreview.boolValue);
            if (EditorGUI.EndChangeCheck())
            {
                manualPreview.boolValue = value;
                manualChanged = true;
            }
        }

        if (timelineControl != null)
        {
            EditorGUI.BeginChangeCheck();
            bool value = EditorGUILayout.ToggleLeft("受动画或 Timeline 影响", timelineControl.boolValue);
            if (EditorGUI.EndChangeCheck())
            {
                timelineControl.boolValue = value;
                timelineChanged = true;
            }
        }

        // 两种控制方式互斥：手动开启时关闭 Timeline；Timeline 开启时关闭手动预览。
        if (manualPreview != null && timelineControl != null)
        {
            if (timelineChanged && timelineControl.boolValue)
            {
                manualPreview.boolValue = false;
            }
            else if (manualChanged && manualPreview.boolValue)
            {
                timelineControl.boolValue = false;
            }
        }

        if (manualPreview != null && manualPreview.boolValue && manualStrength != null)
        {
            EditorGUILayout.PropertyField(manualStrength, new GUIContent("手动预览强度"));
        }

        if (timelineControl != null && timelineControl.boolValue && animationStrength != null)
        {
            EditorGUILayout.PropertyField(animationStrength, new GUIContent("动画控制强度"));
            EditorGUILayout.HelpBox("Animation 可直接 K“动画控制强度”。Timeline Clip 播放时会自动使用 Clip 曲线，并自动关闭手动预览。", MessageType.None);
        }
    }

    private static void DrawParameterFields(SerializedProperty property, string[] parameterNames)
    {
        if (parameterNames == null)
        {
            return;
        }

        for (int index = 0; index < parameterNames.Length; index++)
        {
            SerializedProperty parameter = property.FindPropertyRelative(parameterNames[index]);
            if (parameter != null)
            {
                if (parameterNames[index] == "黑白往返次数")
                {
                    EditorGUILayout.PropertyField(parameter, new GUIContent("黑白往返次数（黑→白→黑 = 1 次）"), true);
                }
                else
                {
                    EditorGUILayout.PropertyField(parameter, true);
                }
            }
        }
    }
}
#endif
