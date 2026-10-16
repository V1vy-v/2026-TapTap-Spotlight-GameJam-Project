using System.Collections.Generic;
using UnityEngine;

namespace GamePlay.Session
{
    /// <summary>
    /// 战斗上下文：根据剧情阶段加载对应战斗，并管理战斗运行时状态
    /// </summary>
    public class BattleContext
    {
        /// <summary>战斗配置 ID 的映射</summary>
        private static readonly Dictionary<StoryStage, string> StageBattleMap = new()
        {
            { StoryStage.Fridge,      "battle_fridge" },
            { StoryStage.Kitchen,     "battle_kitchen" },
            { StoryStage.Road,        "battle_road" },
            { StoryStage.Supermarket, "battle_supermarket" },
        };

        /// <summary>当前战斗配置 ID</summary>
        public string CurrentBattleId { get; private set; }

        /// <summary>还活着的敌人数</summary>
        public int AliveEnemyCount { get; private set; }

        /// <summary>本场战斗是否已胜利</summary>
        public bool IsVictory { get; private set; }

        /// <summary>当前是否处于战斗中</summary>
        public bool IsInBattle => AliveEnemyCount > 0 && !IsVictory;

        /// <summary>根据剧情阶段刷新战斗配置</summary>
        public void RefreshByStage(StoryContext story)
        {
            if (story == null)
            {
                Debug.LogWarning("StoryContext 为空");
                CurrentBattleId = null;
                return;
            }

            if (StageBattleMap.TryGetValue(story.CurrentStage, out var battleId))
            {
                CurrentBattleId = battleId;
                Debug.Log($"战斗切换为：{battleId}");
            }
            else
            {
                Debug.LogWarning($"阶段 {story.CurrentStage} 没有配置战斗");
                CurrentBattleId = null;
            }
        }

        /// <summary>开始战斗（进入战斗模式时调用）</summary>
        public void StartBattle(int enemyCount)
        {
            if (enemyCount <= 0)
            {
                return;
            }

            AliveEnemyCount = enemyCount;
            IsVictory = false;
        }

        /// <summary>一只敌人死亡</summary>
        public void OnEnemyDead()
        {
            if (AliveEnemyCount <= 0) return;

            AliveEnemyCount = Mathf.Max(0, AliveEnemyCount - 1);

            if (AliveEnemyCount <= 0)
                MarkVictory();
        }

        /// <summary>战斗胜利</summary>
        public void MarkVictory()
        {
            if (IsVictory) return;

            IsVictory = true;
        }

        /// <summary>清理战斗状态（离开战斗模式时调用）</summary>
        public void Clear()
        {
            AliveEnemyCount = 0;
            IsVictory = false;
        }
    }
}