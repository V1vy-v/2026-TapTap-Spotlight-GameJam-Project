using Framework;
using GamePlay.Configs;
using GamePlay.Enemy;
using GamePlay.Interaction;
using System.Collections.Generic;
using UnityEngine;
using YooAsset;

namespace GamePlay.Session
{
    /// <summary>
    /// 战斗状态：进入时按配置生成敌人，全部击败后推进流程
    /// </summary>
    public sealed class GameplayBattleState : GameplayModeStateBase
    {
        public override GameplayMode StateKey => GameplayMode.Battle;

        /// <summary>敌人全灭后自己切走</summary>
        protected override bool AutoAdvanceOnFinish => true;
        private readonly List<GameObject> _spawnedEnemies = new();

        /// <summary>因可吸取而暂留在场上的尸体，等玩家用 F 吸，或阶段推进时统一回收</summary>
        private readonly List<GameObject> _pendingCorpses = new();
        private bool _hasError;

        public GameplayBattleState(GameplayRoot root, StateMachine<GameplayMode> sm)
            : base(root, sm) { }

        #region 生命周期

        public override void Enter()
        {
            base.Enter();

            _hasError = false;
            _spawnedEnemies.Clear();

            BattleConfig config = Root.Battle.CurrentBattle;
            if (config == null)
            {
                Debug.LogError("[GameplayBattleState] 当前节点没有配置 BattleConfig");
                _hasError = true;
                return;
            }

            //加载 BattleConfig
            var config = BattleConfigs.Get(battleId);
            if (config == null)
            {
                _hasError = true;
                return;
            }

            // 先全部生成，统计真正成功的数量。
            // 不能用 config.TotalCount —— 它只统计 Config != null，
            // PrefabLocation 为空或实例化失败时不会扣掉；计数高于场上敌人，
            // AliveEnemyCount 就归不了零，IsVictory 永远 false，玩家卡在战斗态出不来。
            int spawned = 0;
            foreach (var entry in config.Enemies)
            {
                if (entry.Config == null) continue;
                for (int i = 0; i < entry.Count; i++)
                    if (SpawnEnemy(entry.Config)) spawned++;
            }

            if (spawned == 0)
            {
                // 一个都没生成出来：跳过本场战斗，不要卡住
                Debug.LogWarning($"[GameplayBattleState] {battleId} 没有生成出任何敌人，跳过本场战斗");
                MarkFinished();
                return;
            }

            //通知敌人数量
            Root.Battle.StartBattle(spawned);
        }

        protected override void Tick(float deltaTime)
        {

        }

        protected override void CheckStateChange()
        {
            if (_hasError)
            {
                _hasError = false;
                MarkFinished();
            }
            else if (Root.Battle.IsVictory)
            {
                MarkFinished();
            }

            // 必须调基类：AutoAdvanceOnFinish 的推进逻辑在 GameplayModeStateBase 里。
            // 不调的话 MarkFinished() 只是把一个没人读的布尔值置真，战斗赢了也切不走模式。
            // 对照 GameplayDialogueState —— 它不重写本方法，所以继承基类逻辑、能正常推进。
            base.CheckStateChange();
        }

        public override void Exit()
        {
            base.Exit();
            ClearAllEnemies();
        }

        #endregion

        #region 敌人生成 / 清理

        /// <summary>生成一只敌人；返回是否真的生成成功</summary>
        private bool SpawnEnemy(EnemyConfig enemyConfig)
        {
            if (enemyConfig == null) return false;

            if (string.IsNullOrEmpty(enemyConfig.PrefabLocation))
            {
                Debug.LogWarning($"[GameplayBattleState] {enemyConfig.Id} 没配 PrefabLocation，跳过");
                return false;
            }

            var go = Global.InstantiateUnpooled(enemyConfig.PrefabLocation, new InstantiateOptions(true));

            if (go == null)
            {
                Debug.LogWarning($"[GameplayBattleState] {enemyConfig.Id} 生成失败：{enemyConfig.PrefabLocation}");
                return false;
            }

            var enemy = go.GetComponent<EnemyController>();
            if (enemy != null) enemy.OnDefeated += HandleEnemyDefeated;

            _spawnedEnemies.Add(go);
            return true;
        }

        private void ClearAllEnemies()
        {
            for (int i = 0; i < _spawnedEnemies.Count; i++)
            {
                var go = _spawnedEnemies[i];
                if (!go) continue;

                // 可吸取且还没被吸的尸体，留给剧情阶段 —— 玩家可能还没走过去按 F。
                // 这条分支故意不退订：尸体还活着，后面可能还会触发事件。
                var absorb = go.GetComponent<AbsorbSource>();
                if (absorb != null && absorb.IsAwaitingAbsorb)
                {
                    _pendingCorpses.Add(go);
                    continue;
                }

                ReleaseEnemy(go);
            }
            _spawnedEnemies.Clear();
        }

        /// <summary>
        /// 回收暂留的可吸取尸体。由 GameplayRoot 在推进剧情阶段和结束对局时调用。
        /// 已经被 AbsorbSource 吸掉的不在了，这里会自然跳过。
        /// </summary>
        public void ReleasePendingCorpses()
        {
            for (int i = 0; i < _pendingCorpses.Count; i++)
                ReleaseEnemy(_pendingCorpses[i]);

            _pendingCorpses.Clear();
        }

        /// <summary>
        /// 统一释放口：先退订再回收。
        /// 订阅必须成对 —— GameplayBattleState 是整个对局只 new 一次的常驻实例，
        /// 而尸体可能跨战斗存活；漏退订会让 OnDefeated 一直指着一个再也用不到的战斗态。
        /// </summary>
        private void ReleaseEnemy(GameObject go)
        {
            if (!go) return;

            var enemy = go.GetComponent<EnemyController>();
            if (enemy != null) enemy.OnDefeated -= HandleEnemyDefeated;

            Global.Release(go);
        }
        private void HandleEnemyDefeated(EnemyController e)
        {
            Root.Battle.OnEnemyDead();
        }
        #endregion
    }
}