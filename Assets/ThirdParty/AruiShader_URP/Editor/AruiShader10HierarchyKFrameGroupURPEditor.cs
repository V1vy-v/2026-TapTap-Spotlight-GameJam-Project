using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(AruiShader10HierarchyKFrameGroupURP))]
public class AruiShader10HierarchyKFrameGroupURPEditor : Editor
{
    public override void OnInspectorGUI()
    {
        AruiShader10HierarchyKFrameGroupURP hierarchy = (AruiShader10HierarchyKFrameGroupURP)target;

        DrawDefaultInspector();

        EditorGUILayout.Space(8f);
        EditorGUILayout.LabelField("主层级统一 K帧工作流", EditorStyles.boldLabel);
        EditorGUILayout.HelpBox(
            "主层级只创建一条 Animation Track 和一份 .anim。所有子粒子上的 K帧控制器都由这份动画片段统一驱动。",
            MessageType.Info
        );

        if(GUILayout.Button("1. 自动补齐全部子粒子 K帧控制器"))
        {
            AruiShader10HierarchyTimelineUtilityURP.自动补齐子粒子K帧控制器(hierarchy);
        }

        if(GUILayout.Button("2. 创建并绑定主层级统一 Timeline 动画轨道"))
        {
            AruiShader10HierarchyTimelineUtilityURP.创建或绑定主层级动画轨道(hierarchy);
        }

        if(GUILayout.Button("3. 扫描并同步全部子层级控制器"))
        {
            Undo.RecordObject(hierarchy, "同步主层级 K帧控制器");
            hierarchy.扫描子层级K帧控制器();
            hierarchy.同步配置到全部子层级();
            EditorUtility.SetDirty(hierarchy);
        }

        EditorGUILayout.Space(4f);
        EditorGUILayout.LabelField("当前扫描到的子层级控制器：" + (hierarchy.子层级K帧控制器 == null ? 0 : hierarchy.子层级K帧控制器.Count), EditorStyles.miniBoldLabel);

        if(hierarchy.子层级K帧控制器 != null)
        {
            for(int index = 0; index < hierarchy.子层级K帧控制器.Count; index++)
            {
                Object controller = hierarchy.子层级K帧控制器[index];
                EditorGUILayout.ObjectField("子层级 " + (index + 1), controller, typeof(Object), true);
            }
        }
    }
}
