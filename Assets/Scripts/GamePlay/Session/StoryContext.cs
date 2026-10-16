using GamePlay.Story;

namespace GamePlay.Session
{
    public sealed class StoryContext
    {
        /// <summary>当前所在节点</summary>
        public NodeConfig CurrentNode { get; private set; }

        /// <summary>当前所在的剧情阶段。随节点变化自动更新。</summary>
        public StoryStage CurrentStage => CurrentNode != null ? CurrentNode.currentStage : default;

        /// <summary>用起始节点初始化</summary>
        public void Reset(NodeConfig startNode)
        {
            CurrentNode = startNode;
        }

        /// <summary>
        /// 尝试前进到下一个节点。
        /// 返回 false 表示流程已到末端（nextNode 为空）。
        /// </summary>
        public bool TryMoveNext(out NodeConfig next)
        {
            next = CurrentNode != null ? CurrentNode.nextNode : null;
            if (next == null) return false;

            CurrentNode = next;
            return true;
        }

        /// <summary>
        /// 直接跳到指定节点
        /// </summary>
        public void JumpTo(NodeConfig node)
        {
            if (node == null) return;
            CurrentNode = node;
        }
    }
}