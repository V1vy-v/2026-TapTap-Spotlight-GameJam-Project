using Framework;
using UnityEngine;

namespace UI
{
    /// <summary>
    /// DialogPanel 的界面逻辑
    /// </summary>
    public partial class DialogPanel
    {
        // 对话控制器（挂在本物体或子物体上）
        private DiaLogController _dialogController;

        #region 生命周期

        protected override void Init()
        {
            _dialogController = GetComponentInChildren<DiaLogController>();
            if (_dialogController == null)
            {
                Debug.LogError($"[{name}] 找不到 DiaLogController 组件！");
                return;
            }

            // 传全部引用
            _dialogController.SetupUI(
                _roleA, _roleB,
                _nameText, _dialogText,
                _bK
            );

            // <ui-bind:add>
            _bK.onClick.AddListener(OnBK);
            // </ui-bind:add>
        }

        protected override void AddListener()
        {
            // 订阅界面以外的事件（比如 GameManager 的事件），暂时没有
        }

        protected override void RemoveListener()
        {
            // <ui-bind:remove>
            _bK.onClick.RemoveListener(OnBK);
            // </ui-bind:remove>
            base.RemoveListener();
        }

        protected override void UpdateView()
        {
            if (_dialogController == null) return;
            var dialogProps = Properties as DialogPanelProperties;

            if (Properties != null && !string.IsNullOrEmpty(dialogProps.DialogPath))
            {
                _dialogController.SetDialogFile(dialogProps.DialogPath);
            }
            else
            {
                Debug.LogWarning("DialogPath为空");
            }
        }

        #endregion

        #region UI回调

        private void OnBK()
        {
            // 点击继续按钮切换下一句对话
            _dialogController?.OnClickNext();
        }

        #endregion
    }
}