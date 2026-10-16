using Framework;
using GamePlay.Configs;
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
        private bool _hasError;

        public GameplayBattleState(GameplayRoot root, StateMachine<GameplayMode> sm)
            : base(root, sm) { }

        #region 生命周期

        public override void Enter()
        {
            base.Enter();

            _hasError = false;
            _spawnedEnemies.Clear();

            string battleId = Root.Battle.CurrentBattleId;
            if (string.IsNullOrEmpty(battleId))
            {
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
            //通知敌人数量
            Root.Battle.StartBattle(config.TotalCount);
            //生成敌人
            foreach (var entry in config.Enemies)
            {
                if (entry.Config == null) continue;
                for (int i = 0; i < entry.Count; i++)
                    SpawnEnemy(entry.Config);
            }
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
                return;
            }

            if (Root.Battle.IsVictory)
            {
                MarkFinished();
            }
        }

        public override void Exit()
        {
            base.Exit();
            ClearAllEnemies();
        }

        #endregion

        #region 敌人生成 / 清理

        private void SpawnEnemy(EnemyConfig enemyConfig)
        {
            if (string.IsNullOrEmpty(enemyConfig.PrefabLocation))
            {
                return;
            }

            var go = Global.InstantiateUnpooled(
                enemyConfig.PrefabLocation,
                new InstantiateOptions(true));

            if (go == null)
            {
                return;
            }

            _spawnedEnemies.Add(go);
        }

        private void ClearAllEnemies()
        {
            for (int i = 0; i < _spawnedEnemies.Count; i++)
            {
                var go = _spawnedEnemies[i];
                if (go) Global.Release(go);
            }
            _spawnedEnemies.Clear();
        }

        #endregion
    }
}