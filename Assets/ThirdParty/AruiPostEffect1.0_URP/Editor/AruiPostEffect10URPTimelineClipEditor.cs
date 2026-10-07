#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

/// <summary>
/// AruiPostEffect1.0 URP Timeline 片段中文 Inspector。
/// 每个 Clip 只显示一种后效的参数，不显示无意义的手动预览二级折叠。
/// </summary>
[CustomEditor(typeof(AruiPostEffect10URPTimelineClip))]
public sealed class AruiPostEffect10URPTimelineClipEditor : Editor
{
    private SerializedProperty effectType;
    private SerializedProperty independent;
    private SerializedProperty intensityCurve;
    private SerializedProperty shake;
    private SerializedProperty colorSplit;
    private SerializedProperty blur;
    private SerializedProperty letterbox;
    private SerializedProperty radialRays;
    private SerializedProperty wideAngle;
    private SerializedProperty vignette;
    private SerializedProperty skillBlackWhite;
    private SerializedProperty manga;

    private void OnEnable()
    {
        effectType = serializedObject.FindProperty("后效类型");
        independent = serializedObject.FindProperty("使用Timeline独立参数");
        intensityCurve = serializedObject.FindProperty("播放强度曲线");
        shake = serializedObject.FindProperty("震屏参数");
        colorSplit = serializedObject.FindProperty("色相分离参数");
        blur = serializedObject.FindProperty("模糊参数");
        letterbox = serializedObject.FindProperty("电影黑边框参数");
        radialRays = serializedObject.FindProperty("放射线参数");
        wideAngle = serializedObject.FindProperty("广角参数");
        vignette = serializedObject.FindProperty("暗角参数");
        skillBlackWhite = serializedObject.FindProperty("技能黑白闪参数");
        manga = serializedObject.FindProperty("漫画滤镜参数");
    }

    public override void OnInspectorGUI()
    {
        AruiPostEffect10URPTimelineClip clip = target as AruiPostEffect10URPTimelineClip;
        if (clip == null)
        {
            return;
        }

        serializedObject.Update();
        EditorGUILayout.Space(4f);
        EditorGUILayout.LabelField("AruiPostEffect1.0 URP - Timeline 后效片段", EditorStyles.boldLabel);
        EditorGUILayout.HelpBox(
            "一个 Clip = 一种后效。Clip 起点是触发时间，Clip 长度是持续时间。\n" +
            "Clip 播放时，会自动关闭相机对应功能的“手动预览”，并开启“受动画或 Timeline 影响”。",
            MessageType.Info);

        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        EditorGUILayout.PropertyField(effectType, new GUIContent("选择添加的后效"));
        EditorGUILayout.PropertyField(independent, new GUIContent("Timeline 独立控制"));

        // 电影黑边框例外：曲线直接驱动黑边宽度 / 高度，不驱动透明度淡入。
        string curveLabel = clip.后效类型 == AruiPostEffect10URP.后效功能.电影黑边框
            ? "黑边宽度曲线（0=无黑边，1=目标宽度）"
            : "片段播放强度曲线";
        EditorGUILayout.PropertyField(intensityCurve, new GUIContent(curveLabel));
        EditorGUILayout.EndVertical();

        if (clip.使用Timeline独立参数)
        {
            EditorGUILayout.Space(4f);
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.LabelField("当前片段独立参数", EditorStyles.boldLabel);
            DrawCurrentSettings(clip);
            EditorGUILayout.EndVertical();
        }
        else
        {
            EditorGUILayout.HelpBox("当前 Clip 读取相机上的 AruiPostEffect1.0 URP 同名功能参数。Timeline 只负责触发时间、持续时间与播放强度。", MessageType.Warning);
        }

        serializedObject.ApplyModifiedProperties();
    }

    private void DrawCurrentSettings(AruiPostEffect10URPTimelineClip clip)
    {
        SerializedProperty currentProperty = null;
        string[] parameterNames = null;
        string tutorial = string.Empty;

        switch (clip.后效类型)
        {
            case AruiPostEffect10URP.后效功能.震屏:
                currentProperty = shake;
                parameterNames = new[] { "位移强度", "横向与纵向振幅", "震动频率", "画面旋转角度", "震屏中心" };
                tutorial = "命中、重击、爆炸。Clip 长度通常 0.08 到 0.25 秒。";
                break;
            case AruiPostEffect10URP.后效功能.色相分离:
                currentProperty = colorSplit;
                parameterNames = new[] { "分离方式", "分离强度", "分离中心", "固定方向角度", "边缘增强", "红蓝偏移比例" };
                tutorial = "普通命中建议使用“轻微命中色散”，空间技能可用“空间撕裂”。";
                break;
            case AruiPostEffect10URP.后效功能.全屏模糊:
                currentProperty = blur;
                parameterNames = new[] { "模糊方式", "模糊中心", "模糊半径像素", "模糊强化倍数", "模糊混合", "径向采样次数" };
                tutorial = "单相机全局模糊，场景、角色、特效都会一起模糊。爆发拉伸适合短 Clip。";
                break;
            case AruiPostEffect10URP.后效功能.电影黑边框:
                currentProperty = letterbox;
                parameterNames = new[] { "黑边颜色", "上下黑边高度", "左右黑边宽度", "黑边边缘柔和度", "黑边不透明度" };
                tutorial = "电影黑边框：Timeline 曲线控制黑边高度 / 宽度（0=无黑边，1=面板目标尺寸），不会做透明度淡入。";
                break;
            case AruiPostEffect10URP.后效功能.全屏放射线:
                currentProperty = radialRays;
                parameterNames = new[] { "放射模式", "放射线颜色", "放射中心", "放射线数量", "放射线宽度", "放射线发光强度", "放射线旋转角度", "放射线流动速度", "放射线闪烁", "背景压暗" };
                tutorial = "可单独调颜色、数量、线宽、发光与旋转。适合大招爆发和切入。";
                break;
            case AruiPostEffect10URP.后效功能.广角:
                currentProperty = wideAngle;
                parameterNames = new[] { "广角方式", "目标相机视野角", "屏幕广角畸变强度", "广角中心" };
                tutorial = "真实相机视野角会临时修改 Camera FOV；相机有 FOV 动画时建议使用屏幕广角畸变模式。";
                break;
            case AruiPostEffect10URP.后效功能.暗角:
                currentProperty = vignette;
                parameterNames = new[] { "暗角中心", "暗角颜色", "暗角强度", "暗角范围", "暗角柔和度", "形状选择", "横向拉伸", "纵向拉伸", "圆角矩形圆角" };
                tutorial = "暗角可选圆形、矩形、菱形、圆角矩形。先调强度，再调范围、柔和度和形状拉伸。";
                break;
            case AruiPostEffect10URP.后效功能.技能黑白闪:
                currentProperty = skillBlackWhite;
                parameterNames = new[] { "闪屏模式", "黑白阈值", "黑白边缘柔和度", "黑白对比度", "黑白往返次数", "轮廓边缘强度", "轮廓边缘宽度", "轮廓边缘阈值" };
                tutorial = "技能黑白闪直接作用于相机最终画面，不需要 Layer。Timeline 会自动使用当前 Clip 进度；黑白反相往返中，黑 → 白 → 黑 = 1 次往返，2 次为黑 → 白 → 黑 → 白 → 黑。开启 Timeline 独立控制后，可在下方直接调整暗部颜色和亮部颜色。";
                break;
            case AruiPostEffect10URP.后效功能.漫画滤镜:
                currentProperty = manga;
                parameterNames = new[] { "漫画模式", "墨色", "纸张色", "黑白阈值", "黑白柔和度", "黑白对比度", "爆发中心", "放射笔刷数量", "笔刷宽度", "笔刷长度", "中心留白范围", "笔刷不规则度", "笔刷断裂", "笔刷强度", "轮廓边缘强度", "轮廓边缘宽度", "轮廓边缘阈值" };
                tutorial = "参考图同类的黑白放射漫画：主画面会压成墨色与纸张色，围绕爆发中心生成不规则的黑色笔刷速度线。Clip 曲线控制整体出现与收回；爆发中心建议设置在角色或武器命中点。";
                break;
        }

        if (currentProperty == null)
        {
            return;
        }

        SerializedProperty preset = currentProperty.FindPropertyRelative("常用预设");
        EditorGUILayout.BeginHorizontal();
        if (preset != null)
        {
            EditorGUILayout.PropertyField(preset, new GUIContent("常用预设"));
        }
        if (GUILayout.Button("应用此预设", GUILayout.Width(96f)))
        {
            serializedObject.ApplyModifiedProperties();
            Undo.RecordObject(clip, "应用 AruiPostEffect1.0 URP Timeline 预设");
            clip.应用当前功能预设();
            EditorUtility.SetDirty(clip);
            serializedObject.Update();
        }
        EditorGUILayout.EndHorizontal();

        // 技能黑白闪的颜色单独放在最前面，避免在 Timeline 面板中被其它参数淹没。
        if (clip.后效类型 == AruiPostEffect10URP.后效功能.技能黑白闪)
        {
            SerializedProperty darkColor = currentProperty.FindPropertyRelative("暗部颜色");
            SerializedProperty lightColor = currentProperty.FindPropertyRelative("亮部颜色");
            EditorGUILayout.Space(2f);
            EditorGUILayout.LabelField("黑白颜色控制（Timeline）", EditorStyles.boldLabel);
            if (darkColor != null)
            {
                EditorGUILayout.PropertyField(darkColor, new GUIContent("暗部颜色 / 黑场颜色"));
            }
            if (lightColor != null)
            {
                EditorGUILayout.PropertyField(lightColor, new GUIContent("亮部颜色 / 白场颜色"));
            }
            EditorGUILayout.HelpBox("这两项仅作用于当前 Timeline Clip。请保持“Timeline 独立控制”开启。", MessageType.None);
        }

        if (parameterNames != null)
        {
            for (int index = 0; index < parameterNames.Length; index++)
            {
                SerializedProperty parameter = currentProperty.FindPropertyRelative(parameterNames[index]);
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

        EditorGUILayout.HelpBox(tutorial, MessageType.None);
    }
}
#endif
