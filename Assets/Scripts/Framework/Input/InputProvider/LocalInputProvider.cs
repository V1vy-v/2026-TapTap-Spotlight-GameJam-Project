using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace Framework
{
    /// <summary>
    /// 本地原生输入封装
    /// </summary>
    public class LocalInputProvider : MonoBehaviour, IInputStateProvider
    {
        private PlayerInputActions _inputActions;
        public PlayerInputActions.PlayerActions PlayerActions;
        public PlayerInputActions.UIActions UiActions;

        /// <summary>
        /// 当前启用的ActionMap
        /// </summary>
        public LocalInputType currentInputMap = LocalInputType.Player;
        private Dictionary<LocalInputType, InputActionMap> _actionMap;

        private void Init()
        {
            _actionMap = new Dictionary<LocalInputType, InputActionMap>();
            _inputActions = new PlayerInputActions();
            PlayerActions = _inputActions.Player;
            UiActions = _inputActions.UI;
            
            RegisterInputAction(LocalInputType.Player, PlayerActions);
            RegisterInputAction(LocalInputType.UI, UiActions);
        }

        public void RegisterInputAction(LocalInputType type, InputActionMap action)
        {
            if(action != null)
                _actionMap[type] = action;
        }

        public void UnregisterInputAction(LocalInputType type)
        {
            _actionMap.Remove(type);
        }

        /// <summary>
        /// 切换ActionMap
        /// </summary>
        /// <param name="type">ActionMap类型</param>
        public void SwitchInputMap(LocalInputType type)
        {
            if (_actionMap.ContainsKey(type))
            {
                _actionMap[currentInputMap].Disable();
                _actionMap[type].Enable();
                currentInputMap = type;
            }
        }

        /// <summary>
        /// 禁用某一ActionMap一段时间
        /// </summary>
        /// <param name="type">需要禁用的ActionMap类型</param>
        /// <param name="sec">时间</param>
        public void DisableActionForSec(LocalInputType type, float sec)
        {
            DisableActionForSec(_actionMap[type], sec);
        }

        /// <summary>
        /// 禁用某一ActionMap一段时间
        /// </summary>
        /// <param name="actionMap">需要禁用的行为</param>
        /// <param name="sec">时间</param>
        public void DisableActionForSec(InputActionMap actionMap, float sec)
        {
            StartCoroutine(DisableAction(actionMap, sec));
        }

        private IEnumerator DisableAction(InputActionMap actionMap, float sec)
        {
            actionMap.Disable();
            yield return new WaitForSeconds(sec);
            actionMap.Enable();
        }

        #region 网络同步输入接口
        public InputState GetInputState()
        {
            return new InputState
            {

            };
        }
        #endregion

        #region 生命周期

        private void Awake()
        {
            Init();
        }

        private void OnEnable()
        {
            _actionMap[currentInputMap].Enable();
        }

        private void OnDisable()
        {
            _actionMap[currentInputMap].Disable();
        }
        #endregion
    }
}
