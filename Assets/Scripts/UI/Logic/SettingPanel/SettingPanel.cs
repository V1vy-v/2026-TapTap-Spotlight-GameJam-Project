namespace UI
{
    /// <summary>
    /// SettingPanel 的界面逻辑
    /// </summary>
    public partial class SettingPanel
    {
        #region 生命周期

        protected override void Init()
        {
            // TODO: 初始化界面数据与控件状态
            // <ui-bind:add>
            _button.onClick.AddListener(OnButton);
            // </ui-bind:add>
        }

        protected override void AddListener()
        {
            // TODO: 订阅界面以外的事件
        }

        protected override void RemoveListener()
        {
            // TODO: 取消订阅，并交给基类清理控制器事件
            // <ui-bind:remove>
            _button.onClick.RemoveListener(OnButton);
            // </ui-bind:remove>
            base.RemoveListener();
        }

        protected override void UpdateView()
        {
            // TODO: 用界面属性刷新控件显示
        }

        #endregion

        #region UI回调

        private void OnButton()
        {
            // TODO: _button 点击后的逻辑
        }

        #endregion
    }
}
