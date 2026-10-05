using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(AruiShader10KFrameControllerURP))]
public class AruiShader10KFrameControllerURPTimelineEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        AruiShader10KFrameControllerURP child = (AruiShader10KFrameControllerURP)target;
        AruiShader10HierarchyKFrameGroupURP hierarchy = child.GetComponentInParent<AruiShader10HierarchyKFrameGroupURP>();

        EditorGUILayout.Space(8f);

        if(hierarchy != null)
        {
            EditorGUILayout.LabelField("主层级统一 Timeline K帧", EditorStyles.boldLabel);
            EditorGUILayout.HelpBox(
                "当前控制器已属于主层级 K帧组。请在根节点的主层级 K帧组中创建统一 Animation Track；不要为每个子粒子单独创建轨道。",
                MessageType.Info
            );

            if(GUILayout.Button("定位主层级 K帧组"))
            {
                Selection.activeObject = hierarchy.gameObject;
                EditorGUIUtility.PingObject(hierarchy.gameObject);
            }
            return;
        }

        EditorGUILayout.LabelField("单物体 Timeline K帧", EditorStyles.boldLabel);
        EditorGUILayout.HelpBox(
            "当前控制器不在主层级 K帧组内，可创建独立 Timeline 动画轨道。",
            MessageType.None
        );

        if(GUILayout.Button("创建并绑定 Timeline K帧动画轨道"))
        {
            AruiShader10TimelineKFrameUtilityURP.CreateAndBindTrack(child);
        }

        if(child.控制台目标动画片段 != null && GUILayout.Button("定位 K帧动画片段"))
        {
            Selection.activeObject = child.控制台目标动画片段;
            EditorGUIUtility.PingObject(child.控制台目标动画片段);
        }
    }
}
