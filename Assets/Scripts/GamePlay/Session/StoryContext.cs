using UnityEngine;

namespace GamePlay.Session
{
    public class StoryContext
    {
        public string CurrentNode { get; private set; }
        public StoryStage CurrentStage { get; private set; } = StoryStage.Fridge;

        /// <summary>
        /// 推进剧情节点
        /// </summary>
        public void AdvanceStoryNode()
        {

        }

        /// <summary>
        /// 推进剧情章节
        /// </summary>
        public void AdvanceStoryStage()
        {
            switch (CurrentStage)
            {
                case StoryStage.Fridge:
                    CurrentStage = StoryStage.Kitchen;
                    break;
                case StoryStage.Kitchen:
                    CurrentStage = StoryStage.Road;
                    break;
                case StoryStage.Road:
                    CurrentStage = StoryStage.Supermarket;
                    break;
                case StoryStage.Supermarket:
                    break;
            }
            Debug.Log("切换状态为"+CurrentStage);
        }

    }
}