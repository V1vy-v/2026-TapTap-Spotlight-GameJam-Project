using System.Collections.Generic;
using UnityEngine;

namespace GamePlay.Session
{
    public class DialogContext
    {
        private static readonly Dictionary<StoryStage, string> StageDialogueMap = new()
        {
            { StoryStage.Fridge,      "111" },
            { StoryStage.Kitchen,     "dialogue_kitchen" },
            { StoryStage.Road,        "dialogue_road" },
            { StoryStage.Supermarket, "dialogue_supermarket" },
        };

        /// <summary>当前对话 Id</summary>
        public string CurrentDialogueId { get; private set; }

        /// <summary>根据剧情阶段刷新对话 Id（不加载）</summary>
        public void RefreshByStage(StoryContext story)
        {
            if (story == null)
            {
                CurrentDialogueId = null;
                return;
            }

            if (StageDialogueMap.TryGetValue(story.CurrentStage, out var dialogueId))
            {
                CurrentDialogueId = dialogueId;
            }
            else
            {
                CurrentDialogueId = null;
            }
        }
    }
}