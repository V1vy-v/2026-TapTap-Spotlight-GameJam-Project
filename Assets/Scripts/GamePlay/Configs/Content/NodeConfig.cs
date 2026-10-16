using GamePlay.Session;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace GamePlay.Story
{
    /// <summary>
    /// 剧情流程中的一个节点。每个节点代表"一步"。
    /// </summary>
    [CreateAssetMenu(fileName = "NodeConfig", menuName = "Gameplay/NodeConfig")]
    public sealed class NodeConfig : ScriptableObject
    {
        public string nodeId;

        [Header("进入此节点时切换到的模式")]
        public GameplayMode mode;

        [Header("是否需要刷新Context")]
        public bool refreshDialog;
        public bool refreshBattle;

        [Header("数据配置(不需要就不填)")]
        public TextAsset dialogConfig;
        public BattleConfig battleConfig;

        [Header("下一个结点")]
        public NodeConfig nextNode;

        [Header("当前流程")]
        public StoryStage currentStage;
    }
}