using System;
using Framework;

namespace UI
{
    /// <summary>
    /// MenuPanel 的界面数据，打开界面时传入
    /// </summary>
    [Serializable]
    public class MenuPanelProperties : PanelProperties
    {
        // TODO: 补上该界面需要的数据字段，加上 SerializeField 即可在 UIView 上配置

        // 供预制体上的 UIView 序列化使用
        public MenuPanelProperties() { }

        public MenuPanelProperties(PanelPriority priority) : base(priority) { }
    }
}
