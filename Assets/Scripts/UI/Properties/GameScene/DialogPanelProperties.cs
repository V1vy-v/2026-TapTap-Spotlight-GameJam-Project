using System;
using Framework;
using UnityEngine;

namespace UI
{
    /// <summary>
    /// DialogPanel 的界面数据，打开界面时传入
    /// </summary>
    [Serializable]
    public class DialogPanelProperties : PanelProperties
    {
        // TODO: 补上该界面需要的数据字段，加上 SerializeField 即可在 UIView 上配置
        public TextAsset dialogText;
        // 供预制体上的 UIView 序列化使用
        public DialogPanelProperties() { }

        public DialogPanelProperties(PanelPriority priority) : base(priority) { }
    }
}
