using Framework;
using GamePlay.Session;
using UnityEngine;
using YooAsset;

namespace GamePlay
{
    public sealed class GameplayRoot : SubSystemBase
    {
        public override int Priority => (int)SubSystemPriority.GameplayRoot;

        private StateMachine<GameplayMode> _modeFsm;
        private StoryContext _storyContext;
        private BattleContext _battleContext;
        private DialogContext _dialogContext;
        private GameplayMode _resumeMode = GameplayMode.FreeRoam;
        private GameObject _player;

        public StoryContext Story => _storyContext;
        public DialogContext Dialog => _dialogContext;
        public BattleContext Battle => _battleContext;
        public GameplayMode CurrentMode => _modeFsm?.CurrentState ?? default;

        #region 对局生命周期
        public void StartNewGame()
        {
            _storyContext = new();
            _battleContext = new();
            _dialogContext = new();
            BuildModeFsm();
            _player = Global.InstantiateUnpooled(GlobalConstants.PlayerPrefabLocation, new InstantiateOptions(true));
            _modeFsm.ChangeState(GameplayMode.FreeRoam);
        }

        public void ContinueGame() 
        {
            /* 读档后同样 BuildModeFsm */ 
        }

        public void EndGame()
        {
            if (_modeFsm != null)
            {
                _modeFsm.OnStateChange -= OnModeChanged;
                _modeFsm.Stop();
                _modeFsm = null;
            }

            if (_player)
            { 
                Global.Release(_player);
                _player = null;
            }

            _storyContext = null;
            _battleContext = null;
            _dialogContext = null;
        }
        #endregion

        private void BuildModeFsm()
        {
            _modeFsm = new StateMachine<GameplayMode>();
            _modeFsm.OnStateChange += OnModeChanged;
            _modeFsm.RegisterState(new GameplayFreeRoamState(this, _modeFsm));
            _modeFsm.RegisterState(new GameplayDialogueState(this, _modeFsm));
            _modeFsm.RegisterState(new GameplayCutsceneState(this, _modeFsm));
            _modeFsm.RegisterState(new GameplayBattleState(this, _modeFsm));
            _modeFsm.RegisterState(new GameplayPauseState(this, _modeFsm));
            _modeFsm.RegisterState(new GameplayGameOverState(this, _modeFsm));
        }

        #region 局内切换
        public void ChangeMode(GameplayMode mode) => _modeFsm?.ChangeState(mode);

        /// <summary>暂停。只在自由行动与战斗中允许。</summary>
        public void Pause()
        {
            if (CurrentMode is GameplayMode.FreeRoam or GameplayMode.Battle)
                _modeFsm.ChangeState(GameplayMode.Pause);
        }

        /// <summary>恢复到暂停前的模式（而不是硬编码回 FreeRoam）。</summary>
        public void Resume() => _modeFsm?.ChangeState(_resumeMode);

        private void OnModeChanged(GameplayMode from, GameplayMode to)
        {
            if (to == GameplayMode.Pause && from != GameplayMode.Pause)
                _resumeMode = from;
        }
        #endregion

        #region 剧情推进（单场景，不走场景加载）
        public void AdvanceNode() => _storyContext?.AdvanceStoryNode();
        /// <summary>推进剧情阶段</summary>
        public void AdvanceStage()
        {
            _storyContext.AdvanceStoryStage();
            _dialogContext.RefreshByStage(_storyContext);   //刷新对话配置
            _battleContext.RefreshByStage(_storyContext);   //刷新怪物配置
        }
        public void EnterBattle() => ChangeMode(GameplayMode.Battle);
        public void EndBattle()
        {
            _battleContext?.Clear();           
            ChangeMode(GameplayMode.FreeRoam);
        }
        #endregion

        #region 生命周期
        public override void Update(float deltaTime) => _modeFsm?.Update(deltaTime);
        public override void FixedUpdate(float fixedDeltaTime) => _modeFsm?.FixedUpdate(fixedDeltaTime);
        public override void LateUpdate() => _modeFsm?.LateUpdate();
        #endregion
        /// <summary>根据当前阶段决定下一步该进什么模式</summary>
        public GameplayMode ResolveNextMode()
        {
            // 看剧情阶段，决定下一步干什么
            switch (_storyContext.CurrentStage)
            {
                case StoryStage.Fridge:
                    return GameplayMode.Dialogue;   

                case StoryStage.Kitchen:
                    if (CurrentMode == GameplayMode.Dialogue)
                        return GameplayMode.Battle;
                    if (CurrentMode == GameplayMode.Battle)
                        return GameplayMode.FreeRoam;
                    return GameplayMode.Dialogue;

                case StoryStage.Road:
                    return GameplayMode.Battle;

                case StoryStage.Supermarket:
                    return GameplayMode.Dialogue;

                default:
                    return GameplayMode.FreeRoam;
            }
        }
    }
}