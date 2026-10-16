using GamePlay.Story;
using UnityEngine;

namespace GamePlay.Session
{
    public class DialogContext
    {
        /// <summary>当前对话配置</summary>
        public TextAsset CurrentDialogue { get; private set; }

        /// <summary>根据节点刷新对话配置（不加载）</summary>
        public void RefreshByNode(NodeConfig node)
        {
            if (node == null)
            {
                CurrentDialogue = null;
                return;
            }

            CurrentDialogue = node.dialogConfig;
            if (CurrentDialogue != null)
                Debug.Log($"[DialogContext] 切换对话: {node.nodeId}");
        }
    }
}