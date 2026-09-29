using System;
using UnityEngine;
using UnityEngine.Serialization;

/// <summary>
/// AruiPostEffect1.0
/// Unity 2020.3 URP 屏幕后效控制器。
/// 支持：相机面板手动预览、Animation 参数动画、Timeline 独立片段控制。
/// </summary>
[ExecuteAlways]
[DisallowMultipleComponent]
[RequireComponent(typeof(Camera))]
[AddComponentMenu("Arui/AruiPostEffect1.0 URP 屏幕后效控制器")]
public sealed class AruiPostEffect10URP : MonoBehaviour
{
    public const string ShaderName = "Hidden/Arui/AruiPostEffect1_0_URP";

    public enum 后效功能
    {
        震屏 = 0,
        色相分离 = 1,
        全屏模糊 = 2,
        电影黑边框 = 3,
        全屏放射线 = 4,
        广角 = 5,
        暗角 = 6,
        技能黑白闪 = 7,
        漫画滤镜 = 8
    }

    public enum 震屏预设 { 自定义, 轻微命中, 重击, 爆炸, 大招爆发 }
    public enum 色相分离预设 { 自定义, 轻微命中色散, 空间撕裂, 能量爆发, 科技故障 }
    public enum 色相分离方式 { 固定方向, 从中心向外 }
    public enum 全屏模糊预设 { 自定义, 轻微失焦, 大招蓄力, 爆发拉伸, 眩晕模糊 }
    public enum 模糊模式 { 全屏均匀模糊, 向中心拉伸, 向外扩散 }
    public enum 电影黑边框预设 { 自定义, 轻微宽银幕, 标准宽银幕, 史诗宽银幕, 四周收束 }
    public enum 全屏放射线预设 { 自定义, 紫色爆发放射, 蓝色能量放射, 金色大招放射, 黑白冲击放射 }
    public enum 放射线模式 { 全屏放射, 中心爆发 }
    public enum 广角预设 { 自定义, 轻微广角, 动作广角, 大招广角, 极限鱼眼 }
    public enum 广角模式 { 屏幕广角畸变, 真实相机视野角, 真实视野角加畸变 }
    public enum 暗角预设 { 自定义, 轻微圆形聚焦, 战斗矩形压迫, 大招菱形聚焦, 圆角矩形收束 }
    public enum 暗角形状 { 圆形, 矩形, 菱形, 圆角矩形 }
    public enum 技能黑白闪预设 { 自定义, 高对比斩击卡帧, 强命中黑白卡帧, 黑白反相往返, 爆发白闪, 压迫黑闪 }
    public enum 技能黑白闪模式 { 高对比黑白卡帧, 黑白反相往返, 全屏白闪, 全屏黑闪 }
    public enum 漫画滤镜预设 { 自定义, 黑白放射漫画, 斩击速度线, 强烈冲击漫画, 仅放射线叠加 }
    public enum 漫画滤镜模式 { 高对比放射漫画, 高对比剪影, 放射线叠加 }

    [Serializable]
    public sealed class 震屏设置
    {
        [Header("控制方式")]
        [Tooltip("开启时使用“动画或Timeline强度”。Animation 可直接 K 此强度；Timeline 片段开始播放时会自动切换到此模式。")]
        [FormerlySerializedAs("是否受动画或Timeline控制")]
        public bool 受动画或Timeline影响;
        [Tooltip("开启时使用“手动预览强度”。默认开启；Timeline 片段开始播放时会自动关闭，避免和 Timeline 叠加。")]
        [FormerlySerializedAs("开启")]
        public bool 手动预览 = true;
        [Range(0f, 1f)] public float 手动预览强度;
        [Range(0f, 1f)] public float 动画或Timeline强度;
        public 震屏预设 常用预设 = 震屏预设.轻微命中;

        [Header("震屏参数")]
        [Range(0f, 0.08f)] public float 位移强度 = 0.012f;
        public Vector2 横向与纵向振幅 = new Vector2(1f, 0.7f);
        [Range(0f, 80f)] public float 震动频率 = 24f;
        [Range(0f, 8f)] public float 画面旋转角度 = 0.4f;
        public Vector2 震屏中心 = new Vector2(0.5f, 0.5f);
    }

    [Serializable]
    public sealed class 色相分离设置
    {
        [Header("控制方式")]
        [Tooltip("开启时使用“动画或Timeline强度”。Animation 可直接 K 此强度；Timeline 片段开始播放时会自动切换到此模式。")]
        [FormerlySerializedAs("是否受动画或Timeline控制")]
        public bool 受动画或Timeline影响;
        [Tooltip("开启时使用“手动预览强度”。默认开启；Timeline 片段开始播放时会自动关闭，避免和 Timeline 叠加。")]
        [FormerlySerializedAs("开启")]
        public bool 手动预览 = true;
        [Range(0f, 1f)] public float 手动预览强度;
        [Range(0f, 1f)] public float 动画或Timeline强度;
        public 色相分离预设 常用预设 = 色相分离预设.轻微命中色散;

        [Header("RGB 色相分离参数")]
        public 色相分离方式 分离方式 = 色相分离方式.从中心向外;
        [Range(0f, 0.05f)] public float 分离强度 = 0.006f;
        public Vector2 分离中心 = new Vector2(0.5f, 0.5f);
        [Range(0f, 360f)] public float 固定方向角度;
        [Range(0f, 3f)] public float 边缘增强 = 0.5f;
        [Range(0f, 2f)] public float 红蓝偏移比例 = 1f;
    }

    [Serializable]
    public sealed class 全屏模糊设置
    {
        [Header("控制方式")]
        [Tooltip("开启时使用“动画或Timeline强度”。Animation 可直接 K 此强度；Timeline 片段开始播放时会自动切换到此模式。")]
        [FormerlySerializedAs("是否受动画或Timeline控制")]
        public bool 受动画或Timeline影响;
        [Tooltip("开启时使用“手动预览强度”。默认开启；Timeline 片段开始播放时会自动关闭，避免和 Timeline 叠加。")]
        [FormerlySerializedAs("开启")]
        public bool 手动预览 = true;
        [Range(0f, 1f)] public float 手动预览强度;
        [Range(0f, 1f)] public float 动画或Timeline强度;
        public 全屏模糊预设 常用预设 = 全屏模糊预设.轻微失焦;

        [Header("单相机全局模糊")]
        [Tooltip("此模糊直接处理当前相机最终画面：场景、角色、特效都会一起模糊；不使用第二相机。")]
        public 模糊模式 模糊方式 = 模糊模式.全屏均匀模糊;
        public Vector2 模糊中心 = new Vector2(0.5f, 0.5f);
        [Range(0f, 96f)] public float 模糊半径像素 = 8f;
        [Tooltip("最终模糊半径的额外倍数。1 = 原始强度，2 = 双倍强度。")]
        [Range(0.25f, 4f)] public float 模糊强化倍数 = 1.5f;
        [Range(0f, 1f)] public float 模糊混合 = 0.8f;
        [Range(4, 24)] public int 径向采样次数 = 12;
    }

    [Serializable]
    public sealed class 电影黑边框设置
    {
        [Header("控制方式")]
        [Tooltip("开启时使用“动画或Timeline强度”。Animation 可直接 K 此强度；Timeline 片段开始播放时会自动切换到此模式。")]
        [FormerlySerializedAs("是否受动画或Timeline控制")]
        public bool 受动画或Timeline影响;
        [Tooltip("开启时使用“手动预览强度”。默认开启；Timeline 片段开始播放时会自动关闭，避免和 Timeline 叠加。")]
        [FormerlySerializedAs("开启")]
        public bool 手动预览 = true;
        [Range(0f, 1f)] public float 手动预览强度;
        [Range(0f, 1f)] public float 动画或Timeline强度;
        public 电影黑边框预设 常用预设 = 电影黑边框预设.标准宽银幕;

        [Header("电影黑边框参数")]
        [ColorUsage(true, true)] public Color 黑边颜色 = Color.black;
        [Tooltip("上下黑边占屏幕高度的比例。0.12 大约是标准宽银幕。")]
        [Range(0f, 0.48f)] public float 上下黑边高度 = 0.12f;
        [Tooltip("左右黑边占屏幕宽度的比例。0 表示只显示上下黑边。")]
        [Range(0f, 0.48f)] public float 左右黑边宽度;
        [Tooltip("黑边与画面的过渡柔和度。0.001 接近硬边。")]
        [Range(0.001f, 0.20f)] public float 黑边边缘柔和度 = 0.008f;
        [Range(0f, 1f)] public float 黑边不透明度 = 1f;
    }

    [Serializable]
    public sealed class 全屏放射线设置
    {
        [Header("控制方式")]
        [Tooltip("开启时使用“动画或Timeline强度”。Animation 可直接 K 此强度；Timeline 片段开始播放时会自动切换到此模式。")]
        [FormerlySerializedAs("是否受动画或Timeline控制")]
        public bool 受动画或Timeline影响;
        [Tooltip("开启时使用“手动预览强度”。默认开启；Timeline 片段开始播放时会自动关闭，避免和 Timeline 叠加。")]
        [FormerlySerializedAs("开启")]
        public bool 手动预览 = true;
        [Range(0f, 1f)] public float 手动预览强度;
        [Range(0f, 1f)] public float 动画或Timeline强度;
        public 全屏放射线预设 常用预设 = 全屏放射线预设.紫色爆发放射;

        [Header("放射线参数")]
        public 放射线模式 放射模式 = 放射线模式.全屏放射;
        [ColorUsage(true, true)] public Color 放射线颜色 = new Color(0.55f, 0.2f, 2f, 1f);
        public Vector2 放射中心 = new Vector2(0.5f, 0.5f);
        [Range(4f, 160f)] public float 放射线数量 = 40f;
        [Range(0.001f, 0.5f)] public float 放射线宽度 = 0.065f;
        [Range(0f, 8f)] public float 放射线发光强度 = 2.4f;
        [Range(0f, 360f)] public float 放射线旋转角度;
        [Range(-5f, 5f)] public float 放射线流动速度 = 0.45f;
        [Range(0f, 1f)] public float 放射线闪烁 = 0.32f;
        [Range(0f, 1f)] public float 背景压暗 = 0.18f;
    }

    [Serializable]
    public sealed class 广角设置
    {
        [Header("控制方式")]
        [Tooltip("开启时使用“动画或Timeline强度”。Animation 可直接 K 此强度；Timeline 片段开始播放时会自动切换到此模式。")]
        [FormerlySerializedAs("是否受动画或Timeline控制")]
        public bool 受动画或Timeline影响;
        [Tooltip("开启时使用“手动预览强度”。默认开启；Timeline 片段开始播放时会自动关闭，避免和 Timeline 叠加。")]
        [FormerlySerializedAs("开启")]
        public bool 手动预览 = true;
        [Range(0f, 1f)] public float 手动预览强度;
        [Range(0f, 1f)] public float 动画或Timeline强度;
        public 广角预设 常用预设 = 广角预设.轻微广角;

        [Header("广角参数")]
        [Tooltip("屏幕广角畸变不修改 Camera FOV；真实相机视野角会修改本相机 Field Of View；组合模式同时使用两者。")]
        public 广角模式 广角方式 = 广角模式.真实视野角加畸变;
        [Range(20f, 150f)] public float 目标相机视野角 = 78f;
        [Range(-0.8f, 0.8f)] public float 屏幕广角畸变强度 = 0.14f;
        public Vector2 广角中心 = new Vector2(0.5f, 0.5f);
    }

    [Serializable]
    public sealed class 暗角设置
    {
        [Header("控制方式")]
        [Tooltip("开启时使用“动画或Timeline强度”。Animation 可直接 K 此强度；Timeline 片段开始播放时会自动切换到此模式。")]
        [FormerlySerializedAs("是否受动画或Timeline控制")]
        public bool 受动画或Timeline影响;
        [Tooltip("开启时使用“手动预览强度”。默认开启；Timeline 片段开始播放时会自动关闭，避免和 Timeline 叠加。")]
        [FormerlySerializedAs("开启")]
        public bool 手动预览 = true;
        [Range(0f, 1f)] public float 手动预览强度;
        [Range(0f, 1f)] public float 动画或Timeline强度;
        public 暗角预设 常用预设 = 暗角预设.战斗矩形压迫;

        [Header("暗角基础参数")]
        [Tooltip("暗角中心。默认 0.5 / 0.5 为屏幕中心。")]
        public Vector2 暗角中心 = new Vector2(0.5f, 0.5f);
        [ColorUsage(true, true)] public Color 暗角颜色 = Color.black;
        [Tooltip("暗角总体压暗强度。")]
        [Range(0f, 1f)] public float 暗角强度 = 0.55f;
        [Tooltip("暗角开始出现的位置。数值越小，越早向画面中心收缩。")]
        [Range(0f, 1.5f)] public float 暗角范围 = 0.65f;
        [Tooltip("暗角边缘柔和度。")]
        [Range(0.001f, 1f)] public float 暗角柔和度 = 0.32f;

        [Header("暗角形状")]
        [Tooltip("圆形适合角色聚焦；矩形适合电影画面边缘压暗；菱形适合斩击卡帧；圆角矩形适合大招收束。")]
        public 暗角形状 形状选择 = 暗角形状.圆形;
        [Tooltip("1 为原始比例。小于 1 会让暗角更早从该方向收缩；大于 1 会拉宽该方向。")]
        [Range(0.25f, 3f)] public float 横向拉伸 = 1f;
        [Range(0.25f, 3f)] public float 纵向拉伸 = 1f;
        [Tooltip("仅对圆角矩形生效。0 接近矩形；1 接近圆形。")]
        [Range(0f, 1f)] public float 圆角矩形圆角 = 0.35f;
    }

    [Serializable]
    public sealed class 技能黑白闪设置
    {
        [Header("控制方式")]
        [Tooltip("开启时使用“动画或Timeline强度”。Animation 可直接 K 此强度；Timeline 片段开始播放时会自动切换到此模式。")]
        [FormerlySerializedAs("是否受动画或Timeline控制")]
        public bool 受动画或Timeline影响;
        [Tooltip("开启时使用“手动预览强度”。默认开启；Timeline 片段开始播放时会自动关闭，避免和 Timeline 叠加。")]
        [FormerlySerializedAs("开启")]
        public bool 手动预览 = true;
        [Range(0f, 1f)] public float 手动预览强度;
        [Range(0f, 1f)] public float 动画或Timeline强度;
        public 技能黑白闪预设 常用预设 = 技能黑白闪预设.高对比斩击卡帧;

        [Header("技能黑白闪参数")]
        [Tooltip("高对比黑白卡帧：全画面黑白分色；黑白反相往返：按播放进度黑白翻转；全屏白闪 / 黑闪：直接进行纯色闪屏。")]
        public 技能黑白闪模式 闪屏模式 = 技能黑白闪模式.高对比黑白卡帧;
        [ColorUsage(true, true)] public Color 暗部颜色 = Color.black;
        [ColorUsage(true, true)] public Color 亮部颜色 = Color.white;
        [Tooltip("亮度落在阈值两侧时分别使用暗部颜色与亮部颜色。")]
        [Range(0f, 1f)] public float 黑白阈值 = 0.46f;
        [Tooltip("分色边缘柔和度。数值越小，黑白切换越硬。")]
        [Range(0.001f, 0.45f)] public float 黑白边缘柔和度 = 0.035f;
        [Range(0f, 8f)] public float 黑白对比度 = 4.5f;
        [Tooltip("仅在“黑白反相往返”模式生效。黑 → 白 → 黑计为 1 次往返；2 次为黑 → 白 → 黑 → 白 → 黑。每次往返都会在 Clip 结尾前回到初始黑白状态。")]
        [Range(1, 4)] public int 黑白往返次数 = 1;
        [Tooltip("手动预览时控制反相阶段；0 到 1 对应一个 Timeline Clip 的播放进度。")]
        [Range(0f, 1f)] public float 手动闪屏进度;
        [Tooltip("动画模式时可直接 K 此进度；Timeline Clip 会自动使用 Clip 内的归一化播放进度。")]
        [Range(0f, 1f)] public float 动画或Timeline进度;

        [Header("轮廓强化")]
        [Tooltip("基于最终画面亮度边缘增强黑白轮廓。0 表示关闭。")]
        [Range(0f, 8f)] public float 轮廓边缘强度 = 1.8f;
        [Tooltip("轮廓采样范围，单位为像素。")]
        [Range(0.25f, 4f)] public float 轮廓边缘宽度 = 1.2f;
        [Range(0f, 1f)] public float 轮廓边缘阈值 = 0.08f;
    }


    [Serializable]
    public sealed class 漫画滤镜设置
    {
        [Header("控制方式")]
        [Tooltip("开启时使用“动画或Timeline强度”。Animation 可直接 K 此强度；Timeline 片段开始播放时会自动切换到此模式。")]
        public bool 受动画或Timeline影响;
        [Tooltip("开启时使用“手动预览强度”。默认开启；Timeline 片段开始播放时会自动关闭，避免和 Timeline 叠加。")]
        public bool 手动预览 = true;
        [Range(0f, 1f)] public float 手动预览强度;
        [Range(0f, 1f)] public float 动画或Timeline强度;
        public 漫画滤镜预设 常用预设 = 漫画滤镜预设.黑白放射漫画;

        [Header("漫画黑白分色")]
        [Tooltip("高对比放射漫画最接近参考图；高对比剪影只保留黑白剪影；放射线叠加保留原画面颜色。")]
        public 漫画滤镜模式 漫画模式 = 漫画滤镜模式.高对比放射漫画;
        [ColorUsage(true, true)] public Color 墨色 = Color.black;
        [ColorUsage(true, true)] public Color 纸张色 = Color.white;
        [Tooltip("亮度落在阈值两侧时，分别使用墨色与纸张色。")]
        [Range(0f, 1f)] public float 黑白阈值 = 0.48f;
        [Range(0.001f, 0.35f)] public float 黑白柔和度 = 0.018f;
        [Range(0f, 8f)] public float 黑白对比度 = 6.5f;

        [Header("漫画放射笔刷")]
        [Tooltip("放射线向外扩散的中心。可设在角色、武器或爆点附近。")]
        public Vector2 爆发中心 = new Vector2(0.5f, 0.5f);
        [Range(4f, 180f)] public float 放射笔刷数量 = 56f;
        [Tooltip("单条笔刷的角度宽度。数值越大，黑色笔刷越粗。")]
        [Range(0.001f, 0.35f)] public float 笔刷宽度 = 0.060f;
        [Tooltip("笔刷由中心向外延伸的长度。1 表示几乎贯穿整屏。")]
        [Range(0.05f, 1.35f)] public float 笔刷长度 = 1.05f;
        [Tooltip("中心预留空白的范围，避免笔刷完全盖住角色或爆点。")]
        [Range(0f, 0.75f)] public float 中心留白范围 = 0.10f;
        [Tooltip("每条笔刷的长度、宽度会产生随机变化。")]
        [Range(0f, 1f)] public float 笔刷不规则度 = 0.55f;
        [Tooltip("笔刷沿半径方向的断裂程度。0 = 连续笔刷。")]
        [Range(0f, 0.90f)] public float 笔刷断裂 = 0.30f;
        [Range(0f, 2f)] public float 笔刷强度 = 1f;

        [Header("剪影轮廓强化")]
        [Range(0f, 8f)] public float 轮廓边缘强度 = 1.8f;
        [Range(0.25f, 4f)] public float 轮廓边缘宽度 = 1.15f;
        [Range(0f, 1f)] public float 轮廓边缘阈值 = 0.08f;
    }

    private sealed class Timeline输入
    {
        public bool 激活;
        public float 强度;
        public float 进度;
        public AruiPostEffect10URPTimelineClip 片段;
    }

    [Header("AruiPostEffect1.0 - 全局控制")]
    [Tooltip("关闭后直接输出原始画面。")]
    public bool 开启后效 = true;
    [Range(0f, 1f)] public float 总强度 = 1f;
    [Tooltip("仅影响编辑器 Game 视图预览；Play Mode 中始终可用。")]
    public bool 编辑器实时预览 = true;
    [Tooltip("通常自动查找，无需手动指定。")]
    public Shader 后效Shader;

    [Header("功能参数")]
    public 震屏设置 震屏 = new 震屏设置();
    public 色相分离设置 色相分离 = new 色相分离设置();
    public 全屏模糊设置 全屏模糊 = new 全屏模糊设置();
    [FormerlySerializedAs("屏幕线条")]
    public 电影黑边框设置 电影黑边框 = new 电影黑边框设置();
    public 全屏放射线设置 全屏放射线 = new 全屏放射线设置();
    public 广角设置 广角 = new 广角设置();
    public 暗角设置 暗角 = new 暗角设置();
    public 技能黑白闪设置 技能黑白闪 = new 技能黑白闪设置();
    public 漫画滤镜设置 漫画滤镜 = new 漫画滤镜设置();

    private readonly Timeline输入[] timelineInputs = new Timeline输入[(int)后效功能.漫画滤镜 + 1];
    private Material runtimeMaterial;
    private Camera targetCamera;
    private float 初始相机视野角;
    private bool 正在控制相机视野角;
    private bool 已接收Timeline时间;
    private double 上次Timeline时间;

    private void Reset()
    {
        后效Shader = Shader.Find(ShaderName);
        获取相机并记录初始视野角();
        确保Timeline输入数组();
    }

    private void OnEnable()
    {
        if (后效Shader == null)
        {
            后效Shader = Shader.Find(ShaderName);
        }
        获取相机并记录初始视野角();
        确保Timeline输入数组();
    }

    private void OnValidate()
    {
        总强度 = Mathf.Clamp01(总强度);
        if (全屏模糊 != null)
        {
            全屏模糊.径向采样次数 = Mathf.Clamp(全屏模糊.径向采样次数, 4, 24);
            全屏模糊.模糊强化倍数 = Mathf.Clamp(全屏模糊.模糊强化倍数, 0.25f, 4f);
        }
        if (技能黑白闪 != null)
        {
            技能黑白闪.黑白往返次数 = Mathf.Clamp(技能黑白闪.黑白往返次数, 1, 4);
        }
        if (漫画滤镜 != null)
        {
            漫画滤镜.放射笔刷数量 = Mathf.Clamp(漫画滤镜.放射笔刷数量, 4f, 180f);
            漫画滤镜.笔刷长度 = Mathf.Clamp(漫画滤镜.笔刷长度, 0.05f, 1.35f);
        }
        if (后效Shader == null)
        {
            后效Shader = Shader.Find(ShaderName);
        }
        确保Timeline输入数组();
    }

    private void Update()
    {
        更新广角相机视野角();
    }

    private void OnDisable()
    {
        清空Timeline输入();
        恢复初始相机视野角();
        释放运行材质();
    }

    private void OnDestroy()
    {
        恢复初始相机视野角();
        释放运行材质();
    }

    /// <summary>由 URP Renderer Feature 在相机最终颜色缓冲执行前调用。</summary>
    public bool 准备URP渲染()
    {

        if (!开启后效 || (!Application.isPlaying && !编辑器实时预览) || !确保运行材质())
        {
            return false;
        }

        float globalWeight = Mathf.Clamp01(总强度);
        震屏设置 shakeSettings = 获取震屏参数();
        色相分离设置 splitSettings = 获取色相分离参数();
        全屏模糊设置 blurSettings = 获取全屏模糊参数();
        电影黑边框设置 letterboxSettings = 获取电影黑边框参数();
        全屏放射线设置 raySettings = 获取全屏放射线参数();
        广角设置 wideSettings = 获取广角参数();
        暗角设置 vignetteSettings = 获取暗角参数();
        技能黑白闪设置 skillBlackWhiteSettings = 获取技能黑白闪参数();
        漫画滤镜设置 mangaSettings = 获取漫画滤镜参数();

        float shakeWeight = 获取最终强度(后效功能.震屏, 震屏.手动预览, 震屏.受动画或Timeline影响, 震屏.手动预览强度, 震屏.动画或Timeline强度) * globalWeight;
        float splitWeight = 获取最终强度(后效功能.色相分离, 色相分离.手动预览, 色相分离.受动画或Timeline影响, 色相分离.手动预览强度, 色相分离.动画或Timeline强度) * globalWeight;
        float blurWeight = 获取最终强度(后效功能.全屏模糊, 全屏模糊.手动预览, 全屏模糊.受动画或Timeline影响, 全屏模糊.手动预览强度, 全屏模糊.动画或Timeline强度) * globalWeight;
        float letterboxWeight = 获取最终强度(后效功能.电影黑边框, 电影黑边框.手动预览, 电影黑边框.受动画或Timeline影响, 电影黑边框.手动预览强度, 电影黑边框.动画或Timeline强度) * globalWeight;
        float rayWeight = 获取最终强度(后效功能.全屏放射线, 全屏放射线.手动预览, 全屏放射线.受动画或Timeline影响, 全屏放射线.手动预览强度, 全屏放射线.动画或Timeline强度) * globalWeight;
        float wideWeight = 获取最终强度(后效功能.广角, 广角.手动预览, 广角.受动画或Timeline影响, 广角.手动预览强度, 广角.动画或Timeline强度) * globalWeight;
        float vignetteWeight = 获取最终强度(后效功能.暗角, 暗角.手动预览, 暗角.受动画或Timeline影响, 暗角.手动预览强度, 暗角.动画或Timeline强度) * globalWeight;
        float skillBlackWhiteWeight = 获取最终强度(后效功能.技能黑白闪, 技能黑白闪.手动预览, 技能黑白闪.受动画或Timeline影响, 技能黑白闪.手动预览强度, 技能黑白闪.动画或Timeline强度) * globalWeight;
        float mangaWeight = 获取最终强度(后效功能.漫画滤镜, 漫画滤镜.手动预览, 漫画滤镜.受动画或Timeline影响, 漫画滤镜.手动预览强度, 漫画滤镜.动画或Timeline强度) * globalWeight;
        float skillBlackWhiteProgress = 获取技能黑白闪进度();

        设置震屏材质参数(shakeWeight, shakeSettings);
        设置色相分离材质参数(splitWeight, splitSettings);
        设置全屏模糊材质参数(blurWeight, blurSettings);
        // 电影黑边框在 Timeline Clip 中，曲线控制的是黑边“宽度 / 高度”，不是透明度淡入。
        // 其他控制方式（手动预览 / Animation）仍按原逻辑用强度控制整体不透明度。
        设置电影黑边框材质参数(letterboxWeight, letterboxSettings, 获取Timeline激活状态(后效功能.电影黑边框));
        设置全屏放射线材质参数(rayWeight, raySettings);
        设置广角材质参数(wideWeight, wideSettings);
        设置暗角材质参数(vignetteWeight, vignetteSettings);
        设置技能黑白闪材质参数(skillBlackWhiteWeight, skillBlackWhiteProgress, skillBlackWhiteSettings);
        设置漫画滤镜材质参数(mangaWeight, mangaSettings);
        runtimeMaterial.SetFloat("_AruiUnscaledTime", Application.isPlaying ? Time.unscaledTime : Time.realtimeSinceStartup);

        
    
        return true;
    }

    /// <summary>仅供 AruiPostEffect1.0 URP Renderer Feature 获取。不要在外部销毁此材质。</summary>
    public Material 获取URP运行材质()
    {
        return runtimeMaterial;
    }


    private void 获取相机并记录初始视野角()
    {
        if (targetCamera == null)
        {
            targetCamera = GetComponent<Camera>();
        }
        if (targetCamera != null && !正在控制相机视野角)
        {
            初始相机视野角 = targetCamera.fieldOfView;
        }
    }

    private void 更新广角相机视野角()
    {
        获取相机并记录初始视野角();
        if (targetCamera == null || 广角 == null)
        {
            return;
        }

        if (!开启后效 || (!Application.isPlaying && !编辑器实时预览))
        {
            恢复初始相机视野角();
            return;
        }

        广角设置 settings = 获取广角参数();
        float weight = 获取最终强度(后效功能.广角, 广角.手动预览, 广角.受动画或Timeline影响, 广角.手动预览强度, 广角.动画或Timeline强度) * Mathf.Clamp01(总强度);
        bool useCameraFov = settings != null && (settings.广角方式 == 广角模式.真实相机视野角 || settings.广角方式 == 广角模式.真实视野角加畸变);

        if (useCameraFov && weight > 0.0001f)
        {
            if (!正在控制相机视野角)
            {
                初始相机视野角 = targetCamera.fieldOfView;
            }
            targetCamera.fieldOfView = Mathf.Lerp(初始相机视野角, settings.目标相机视野角, weight);
            正在控制相机视野角 = true;
        }
        else
        {
            恢复初始相机视野角();
        }
    }

    private void 恢复初始相机视野角()
    {
        if (targetCamera != null && 正在控制相机视野角)
        {
            targetCamera.fieldOfView = 初始相机视野角;
        }
        正在控制相机视野角 = false;
    }

    private void 设置震屏材质参数(float weight, 震屏设置 settings)
    {
        if (settings == null) { runtimeMaterial.SetFloat("_ShakeWeight", 0f); return; }
        runtimeMaterial.SetFloat("_ShakeWeight", weight);
        runtimeMaterial.SetFloat("_ShakeStrength", settings.位移强度);
        runtimeMaterial.SetVector("_ShakeAmplitude", new Vector4(settings.横向与纵向振幅.x, settings.横向与纵向振幅.y, 0f, 0f));
        runtimeMaterial.SetFloat("_ShakeFrequency", settings.震动频率);
        runtimeMaterial.SetFloat("_ShakeRotation", settings.画面旋转角度 * Mathf.Deg2Rad);
        runtimeMaterial.SetVector("_ShakeCenter", new Vector4(settings.震屏中心.x, settings.震屏中心.y, 0f, 0f));
    }

    private void 设置色相分离材质参数(float weight, 色相分离设置 settings)
    {
        if (settings == null) { runtimeMaterial.SetFloat("_ColorSplitWeight", 0f); return; }
        runtimeMaterial.SetFloat("_ColorSplitWeight", weight);
        runtimeMaterial.SetFloat("_ColorSplitMode", (float)settings.分离方式);
        runtimeMaterial.SetFloat("_ColorSplitStrength", settings.分离强度);
        runtimeMaterial.SetVector("_ColorSplitCenter", new Vector4(settings.分离中心.x, settings.分离中心.y, 0f, 0f));
        runtimeMaterial.SetFloat("_ColorSplitDirection", settings.固定方向角度 * Mathf.Deg2Rad);
        runtimeMaterial.SetFloat("_ColorSplitEdgeBoost", settings.边缘增强);
        runtimeMaterial.SetFloat("_ColorSplitRBScale", settings.红蓝偏移比例);
    }

    private void 设置全屏模糊材质参数(float weight, 全屏模糊设置 settings)
    {
        if (settings == null) { runtimeMaterial.SetFloat("_BlurWeight", 0f); return; }
        runtimeMaterial.SetFloat("_BlurWeight", weight);
        runtimeMaterial.SetFloat("_BlurMode", (float)settings.模糊方式);
        runtimeMaterial.SetVector("_BlurCenter", new Vector4(settings.模糊中心.x, settings.模糊中心.y, 0f, 0f));
        runtimeMaterial.SetFloat("_BlurRadius", settings.模糊半径像素 * settings.模糊强化倍数);
        runtimeMaterial.SetFloat("_BlurMix", settings.模糊混合);
        runtimeMaterial.SetFloat("_BlurSamples", Mathf.Clamp(settings.径向采样次数, 4, 24));
    }

    /// <summary>
    /// 设置电影黑边框参数。
    /// Timeline 片段激活时：播放强度曲线控制上下 / 左右黑边的尺寸从 0 到目标值；
    /// 黑边不透明度保持为面板设置值，不会被曲线额外淡入。
    /// </summary>
    private void 设置电影黑边框材质参数(float weight, 电影黑边框设置 settings, bool timelineControlsWidth)
    {
        if (settings == null)
        {
            runtimeMaterial.SetFloat("_LetterboxWeight", 0f);
            return;
        }

        float curveValue = Mathf.Clamp01(weight);
        float topBottomSize = settings.上下黑边高度;
        float leftRightSize = settings.左右黑边宽度;
        float opacityWeight = curveValue;

        if (timelineControlsWidth)
        {
            // Timeline 曲线 = 黑边尺寸曲线。曲线 0 时黑边高度 / 宽度为 0，曲线 1 时到达面板目标尺寸。
            topBottomSize *= curveValue;
            leftRightSize *= curveValue;
            // 曲线不参与黑边透明度淡入，保持黑边颜色和不透明度稳定。
            opacityWeight = curveValue > 0.0001f ? 1f : 0f;
        }

        runtimeMaterial.SetFloat("_LetterboxWeight", opacityWeight);
        runtimeMaterial.SetColor("_LetterboxColor", settings.黑边颜色);
        runtimeMaterial.SetFloat("_LetterboxTopBottom", Mathf.Clamp01(topBottomSize));
        runtimeMaterial.SetFloat("_LetterboxLeftRight", Mathf.Clamp01(leftRightSize));
        runtimeMaterial.SetFloat("_LetterboxSoftness", settings.黑边边缘柔和度);
        runtimeMaterial.SetFloat("_LetterboxOpacity", settings.黑边不透明度);
    }

    private void 设置全屏放射线材质参数(float weight, 全屏放射线设置 settings)
    {
        if (settings == null) { runtimeMaterial.SetFloat("_RadialRayWeight", 0f); return; }
        runtimeMaterial.SetFloat("_RadialRayWeight", weight);
        runtimeMaterial.SetFloat("_RadialRayMode", (float)settings.放射模式);
        runtimeMaterial.SetColor("_RadialRayColor", settings.放射线颜色);
        runtimeMaterial.SetVector("_RadialRayCenter", new Vector4(settings.放射中心.x, settings.放射中心.y, 0f, 0f));
        runtimeMaterial.SetFloat("_RadialRayCount", settings.放射线数量);
        runtimeMaterial.SetFloat("_RadialRayWidth", settings.放射线宽度);
        runtimeMaterial.SetFloat("_RadialRayGlow", settings.放射线发光强度);
        runtimeMaterial.SetFloat("_RadialRayRotation", settings.放射线旋转角度 * Mathf.Deg2Rad);
        runtimeMaterial.SetFloat("_RadialRayFlowSpeed", settings.放射线流动速度);
        runtimeMaterial.SetFloat("_RadialRayFlicker", settings.放射线闪烁);
        runtimeMaterial.SetFloat("_RadialRayDarken", settings.背景压暗);
    }

    private void 设置广角材质参数(float weight, 广角设置 settings)
    {
        if (settings == null) { runtimeMaterial.SetFloat("_WideWeight", 0f); return; }
        bool useScreenDistortion = settings.广角方式 == 广角模式.屏幕广角畸变 || settings.广角方式 == 广角模式.真实视野角加畸变;
        runtimeMaterial.SetFloat("_WideWeight", useScreenDistortion ? weight : 0f);
        runtimeMaterial.SetFloat("_WideDistortion", settings.屏幕广角畸变强度);
        runtimeMaterial.SetVector("_WideCenter", new Vector4(settings.广角中心.x, settings.广角中心.y, 0f, 0f));
    }

    private void 设置暗角材质参数(float weight, 暗角设置 settings)
    {
        if (settings == null) { runtimeMaterial.SetFloat("_VignetteWeight", 0f); return; }
        runtimeMaterial.SetFloat("_VignetteWeight", weight);
        runtimeMaterial.SetVector("_VignetteCenter", new Vector4(settings.暗角中心.x, settings.暗角中心.y, 0f, 0f));
        runtimeMaterial.SetColor("_VignetteColor", settings.暗角颜色);
        runtimeMaterial.SetFloat("_VignetteIntensity", settings.暗角强度);
        runtimeMaterial.SetFloat("_VignetteRange", settings.暗角范围);
        runtimeMaterial.SetFloat("_VignetteSoftness", settings.暗角柔和度);
        runtimeMaterial.SetFloat("_VignetteShapeMode", (float)settings.形状选择);
        runtimeMaterial.SetVector("_VignetteScale", new Vector4(settings.横向拉伸, settings.纵向拉伸, 0f, 0f));
        runtimeMaterial.SetFloat("_VignetteCornerRoundness", settings.圆角矩形圆角);
    }

    private void 设置技能黑白闪材质参数(float weight, float progress, 技能黑白闪设置 settings)
    {
        if (settings == null) { runtimeMaterial.SetFloat("_SkillBWWeight", 0f); return; }
        runtimeMaterial.SetFloat("_SkillBWWeight", weight);
        runtimeMaterial.SetFloat("_SkillBWMode", (float)settings.闪屏模式);
        runtimeMaterial.SetColor("_SkillBWBlackColor", settings.暗部颜色);
        runtimeMaterial.SetColor("_SkillBWWhiteColor", settings.亮部颜色);
        runtimeMaterial.SetFloat("_SkillBWThreshold", settings.黑白阈值);
        runtimeMaterial.SetFloat("_SkillBWSoftness", settings.黑白边缘柔和度);
        runtimeMaterial.SetFloat("_SkillBWContrast", settings.黑白对比度);
        runtimeMaterial.SetFloat("_SkillBWSwitchCount", Mathf.Clamp(settings.黑白往返次数, 1, 4));
        runtimeMaterial.SetFloat("_SkillBWProgress", Mathf.Clamp01(progress));
        runtimeMaterial.SetFloat("_SkillBWEdgeStrength", settings.轮廓边缘强度);
        runtimeMaterial.SetFloat("_SkillBWEdgeWidth", settings.轮廓边缘宽度);
        runtimeMaterial.SetFloat("_SkillBWEdgeThreshold", settings.轮廓边缘阈值);
    }


    private void 设置漫画滤镜材质参数(float weight, 漫画滤镜设置 settings)
    {
        if (settings == null)
        {
            runtimeMaterial.SetFloat("_MangaWeight", 0f);
            return;
        }

        runtimeMaterial.SetFloat("_MangaWeight", weight);
        runtimeMaterial.SetFloat("_MangaMode", (float)settings.漫画模式);
        runtimeMaterial.SetColor("_MangaInkColor", settings.墨色);
        runtimeMaterial.SetColor("_MangaPaperColor", settings.纸张色);
        runtimeMaterial.SetFloat("_MangaThreshold", settings.黑白阈值);
        runtimeMaterial.SetFloat("_MangaSoftness", settings.黑白柔和度);
        runtimeMaterial.SetFloat("_MangaContrast", settings.黑白对比度);
        runtimeMaterial.SetVector("_MangaCenter", new Vector4(settings.爆发中心.x, settings.爆发中心.y, 0f, 0f));
        runtimeMaterial.SetFloat("_MangaBrushCount", settings.放射笔刷数量);
        runtimeMaterial.SetFloat("_MangaBrushWidth", settings.笔刷宽度);
        runtimeMaterial.SetFloat("_MangaBrushLength", settings.笔刷长度);
        runtimeMaterial.SetFloat("_MangaInnerBlank", settings.中心留白范围);
        runtimeMaterial.SetFloat("_MangaIrregularity", settings.笔刷不规则度);
        runtimeMaterial.SetFloat("_MangaBreakup", settings.笔刷断裂);
        runtimeMaterial.SetFloat("_MangaBrushStrength", settings.笔刷强度);
        runtimeMaterial.SetFloat("_MangaEdgeStrength", settings.轮廓边缘强度);
        runtimeMaterial.SetFloat("_MangaEdgeWidth", settings.轮廓边缘宽度);
        runtimeMaterial.SetFloat("_MangaEdgeThreshold", settings.轮廓边缘阈值);
    }

    private float 获取最终强度(后效功能 功能, bool 开启, bool 受动画控制, float 手动强度, float 动画强度)
    {
        Timeline输入 input = 获取Timeline输入(功能);
        if (input != null && input.激活)
        {
            return Mathf.Clamp01(input.强度);
        }
        if (!开启)
        {
            return 0f;
        }
        return Mathf.Clamp01(受动画控制 ? 动画强度 : 手动强度);
    }

    private Timeline输入 获取Timeline输入(后效功能 功能)
    {
        确保Timeline输入数组();
        int index = (int)功能;
        return index >= 0 && index < timelineInputs.Length ? timelineInputs[index] : null;
    }

    private bool 获取Timeline激活状态(后效功能 功能)
    {
        Timeline输入 input = 获取Timeline输入(功能);
        return input != null && input.激活;
    }

    private AruiPostEffect10URPTimelineClip 获取Timeline参数(后效功能 功能)
    {
        Timeline输入 input = 获取Timeline输入(功能);
        return input != null && input.激活 ? input.片段 : null;
    }

    private 震屏设置 获取震屏参数() { AruiPostEffect10URPTimelineClip clip = 获取Timeline参数(后效功能.震屏); return clip != null && clip.使用Timeline独立参数 ? clip.震屏参数 : 震屏; }
    private 色相分离设置 获取色相分离参数() { AruiPostEffect10URPTimelineClip clip = 获取Timeline参数(后效功能.色相分离); return clip != null && clip.使用Timeline独立参数 ? clip.色相分离参数 : 色相分离; }
    private 全屏模糊设置 获取全屏模糊参数() { AruiPostEffect10URPTimelineClip clip = 获取Timeline参数(后效功能.全屏模糊); return clip != null && clip.使用Timeline独立参数 ? clip.模糊参数 : 全屏模糊; }
    private 电影黑边框设置 获取电影黑边框参数() { AruiPostEffect10URPTimelineClip clip = 获取Timeline参数(后效功能.电影黑边框); return clip != null && clip.使用Timeline独立参数 ? clip.电影黑边框参数 : 电影黑边框; }
    private 全屏放射线设置 获取全屏放射线参数() { AruiPostEffect10URPTimelineClip clip = 获取Timeline参数(后效功能.全屏放射线); return clip != null && clip.使用Timeline独立参数 ? clip.放射线参数 : 全屏放射线; }
    private 广角设置 获取广角参数() { AruiPostEffect10URPTimelineClip clip = 获取Timeline参数(后效功能.广角); return clip != null && clip.使用Timeline独立参数 ? clip.广角参数 : 广角; }
    private 暗角设置 获取暗角参数() { AruiPostEffect10URPTimelineClip clip = 获取Timeline参数(后效功能.暗角); return clip != null && clip.使用Timeline独立参数 ? clip.暗角参数 : 暗角; }
    private 技能黑白闪设置 获取技能黑白闪参数() { AruiPostEffect10URPTimelineClip clip = 获取Timeline参数(后效功能.技能黑白闪); return clip != null && clip.使用Timeline独立参数 ? clip.技能黑白闪参数 : 技能黑白闪; }
    private 漫画滤镜设置 获取漫画滤镜参数() { AruiPostEffect10URPTimelineClip clip = 获取Timeline参数(后效功能.漫画滤镜); return clip != null && clip.使用Timeline独立参数 ? clip.漫画滤镜参数 : 漫画滤镜; }

    private float 获取技能黑白闪进度()
    {
        Timeline输入 input = 获取Timeline输入(后效功能.技能黑白闪);
        if (input != null && input.激活)
        {
            return Mathf.Clamp01(input.进度);
        }
        技能黑白闪设置 settings = 获取技能黑白闪参数();
        if (settings == null)
        {
            return 0f;
        }
        return Mathf.Clamp01(settings.受动画或Timeline影响 ? settings.动画或Timeline进度 : settings.手动闪屏进度);
    }

    public void 开始Timeline帧(double timelineTime)
    {
        确保Timeline输入数组();
        if (!已接收Timeline时间 || Math.Abs(timelineTime - 上次Timeline时间) > 0.000001d)
        {
            for (int index = 0; index < timelineInputs.Length; index++)
            {
                timelineInputs[index].激活 = false;
                timelineInputs[index].强度 = 0f;
                timelineInputs[index].进度 = 0f;
                timelineInputs[index].片段 = null;
            }
            上次Timeline时间 = timelineTime;
            已接收Timeline时间 = true;
        }
    }

    public void 设置Timeline功能(后效功能 功能, float 强度, float 归一化进度, AruiPostEffect10URPTimelineClip 片段)
    {
        Timeline输入 input = 获取Timeline输入(功能);
        if (input == null || 片段 == null)
        {
            return;
        }

        // Timeline 首次接管某功能时，自动关闭面板手动预览并切换到动画/Timeline 模式。
        // Clip 自身的曲线仍是实际播放强度；该状态切换用于避免预览强度与 Timeline 同时叠加。
        切换功能到Timeline控制(功能);

        float clampedStrength = Mathf.Clamp01(强度);
        if (!input.激活 || clampedStrength >= input.强度)
        {
            input.激活 = true;
            input.强度 = clampedStrength;
            input.进度 = Mathf.Clamp01(归一化进度);
            input.片段 = 片段;
        }
    }

    public void 清空Timeline输入()
    {
        确保Timeline输入数组();
        for (int index = 0; index < timelineInputs.Length; index++)
        {
            timelineInputs[index].激活 = false;
            timelineInputs[index].强度 = 0f;
            timelineInputs[index].进度 = 0f;
            timelineInputs[index].片段 = null;
        }
        已接收Timeline时间 = false;
        上次Timeline时间 = 0d;
    }

    private void 确保Timeline输入数组()
    {
        for (int index = 0; index < timelineInputs.Length; index++)
        {
            if (timelineInputs[index] == null)
            {
                timelineInputs[index] = new Timeline输入();
            }
        }
    }

    private void 切换功能到Timeline控制(后效功能 功能)
    {
        bool changed = false;
        switch (功能)
        {
            case 后效功能.震屏: changed = 切换到Timeline控制模式(震屏); break;
            case 后效功能.色相分离: changed = 切换到Timeline控制模式(色相分离); break;
            case 后效功能.全屏模糊: changed = 切换到Timeline控制模式(全屏模糊); break;
            case 后效功能.电影黑边框: changed = 切换到Timeline控制模式(电影黑边框); break;
            case 后效功能.全屏放射线: changed = 切换到Timeline控制模式(全屏放射线); break;
            case 后效功能.广角: changed = 切换到Timeline控制模式(广角); break;
            case 后效功能.暗角: changed = 切换到Timeline控制模式(暗角); break;
            case 后效功能.技能黑白闪: changed = 切换到Timeline控制模式(技能黑白闪); break;
            case 后效功能.漫画滤镜: changed = 切换到Timeline控制模式(漫画滤镜); break;
        }
#if UNITY_EDITOR
        if (changed && !Application.isPlaying)
        {
            UnityEditor.EditorUtility.SetDirty(this);
        }
#endif
    }

    private static bool 切换到Timeline控制模式(震屏设置 settings) { bool changed = settings.手动预览 || !settings.受动画或Timeline影响; settings.手动预览 = false; settings.受动画或Timeline影响 = true; return changed; }
    private static bool 切换到Timeline控制模式(色相分离设置 settings) { bool changed = settings.手动预览 || !settings.受动画或Timeline影响; settings.手动预览 = false; settings.受动画或Timeline影响 = true; return changed; }
    private static bool 切换到Timeline控制模式(全屏模糊设置 settings) { bool changed = settings.手动预览 || !settings.受动画或Timeline影响; settings.手动预览 = false; settings.受动画或Timeline影响 = true; return changed; }
    private static bool 切换到Timeline控制模式(电影黑边框设置 settings) { bool changed = settings.手动预览 || !settings.受动画或Timeline影响; settings.手动预览 = false; settings.受动画或Timeline影响 = true; return changed; }
    private static bool 切换到Timeline控制模式(全屏放射线设置 settings) { bool changed = settings.手动预览 || !settings.受动画或Timeline影响; settings.手动预览 = false; settings.受动画或Timeline影响 = true; return changed; }
    private static bool 切换到Timeline控制模式(广角设置 settings) { bool changed = settings.手动预览 || !settings.受动画或Timeline影响; settings.手动预览 = false; settings.受动画或Timeline影响 = true; return changed; }
    private static bool 切换到Timeline控制模式(暗角设置 settings) { bool changed = settings.手动预览 || !settings.受动画或Timeline影响; settings.手动预览 = false; settings.受动画或Timeline影响 = true; return changed; }
    private static bool 切换到Timeline控制模式(技能黑白闪设置 settings) { bool changed = settings.手动预览 || !settings.受动画或Timeline影响; settings.手动预览 = false; settings.受动画或Timeline影响 = true; return changed; }
    private static bool 切换到Timeline控制模式(漫画滤镜设置 settings) { bool changed = settings.手动预览 || !settings.受动画或Timeline影响; settings.手动预览 = false; settings.受动画或Timeline影响 = true; return changed; }

    public void 关闭所有手动预览()
    {
        震屏.手动预览 = false;
        色相分离.手动预览 = false;
        全屏模糊.手动预览 = false;
        电影黑边框.手动预览 = false;
        全屏放射线.手动预览 = false;
        广角.手动预览 = false;
        暗角.手动预览 = false;
        技能黑白闪.手动预览 = false;
        漫画滤镜.手动预览 = false;
    }

    public void 只预览功能(后效功能 功能)
    {
        关闭所有手动预览();
        switch (功能)
        {
            case 后效功能.震屏: 启用功能手动预览(震屏); break;
            case 后效功能.色相分离: 启用功能手动预览(色相分离); break;
            case 后效功能.全屏模糊: 启用功能手动预览(全屏模糊); break;
            case 后效功能.电影黑边框: 启用功能手动预览(电影黑边框); break;
            case 后效功能.全屏放射线: 启用功能手动预览(全屏放射线); break;
            case 后效功能.广角: 启用功能手动预览(广角); break;
            case 后效功能.暗角: 启用功能手动预览(暗角); break;
            case 后效功能.技能黑白闪: 启用功能手动预览(技能黑白闪); break;
            case 后效功能.漫画滤镜: 启用功能手动预览(漫画滤镜); break;
        }
    }

    private static void 启用功能手动预览(震屏设置 settings) { settings.手动预览 = true; settings.受动画或Timeline影响 = false; settings.手动预览强度 = 1f; }
    private static void 启用功能手动预览(色相分离设置 settings) { settings.手动预览 = true; settings.受动画或Timeline影响 = false; settings.手动预览强度 = 1f; }
    private static void 启用功能手动预览(全屏模糊设置 settings) { settings.手动预览 = true; settings.受动画或Timeline影响 = false; settings.手动预览强度 = 1f; }
    private static void 启用功能手动预览(电影黑边框设置 settings) { settings.手动预览 = true; settings.受动画或Timeline影响 = false; settings.手动预览强度 = 1f; }
    private static void 启用功能手动预览(全屏放射线设置 settings) { settings.手动预览 = true; settings.受动画或Timeline影响 = false; settings.手动预览强度 = 1f; }
    private static void 启用功能手动预览(广角设置 settings) { settings.手动预览 = true; settings.受动画或Timeline影响 = false; settings.手动预览强度 = 1f; }
    private static void 启用功能手动预览(暗角设置 settings) { settings.手动预览 = true; settings.受动画或Timeline影响 = false; settings.手动预览强度 = 1f; }
    private static void 启用功能手动预览(技能黑白闪设置 settings) { settings.手动预览 = true; settings.受动画或Timeline影响 = false; settings.手动预览强度 = 1f; }
    private static void 启用功能手动预览(漫画滤镜设置 settings) { settings.手动预览 = true; settings.受动画或Timeline影响 = false; settings.手动预览强度 = 1f; }

    public void 应用震屏预设() { 应用震屏预设(震屏); 启用功能手动预览(震屏); }
    public void 应用色相分离预设() { 应用色相分离预设(色相分离); 启用功能手动预览(色相分离); }
    public void 应用全屏模糊预设() { 应用全屏模糊预设(全屏模糊); 启用功能手动预览(全屏模糊); }
    public void 应用电影黑边框预设() { 应用电影黑边框预设(电影黑边框); 启用功能手动预览(电影黑边框); }
    public void 应用全屏放射线预设() { 应用全屏放射线预设(全屏放射线); 启用功能手动预览(全屏放射线); }
    public void 应用广角预设() { 应用广角预设(广角); 启用功能手动预览(广角); }
    public void 应用暗角预设() { 应用暗角预设(暗角); 启用功能手动预览(暗角); }
    public void 应用技能黑白闪预设() { 应用技能黑白闪预设(技能黑白闪); 启用功能手动预览(技能黑白闪); }
    public void 应用漫画滤镜预设() { 应用漫画滤镜预设(漫画滤镜); 启用功能手动预览(漫画滤镜); }

    public static void 应用震屏预设(震屏设置 settings)
    {
        if (settings == null) return;
        switch (settings.常用预设)
        {
            case 震屏预设.轻微命中: settings.位移强度 = 0.007f; settings.横向与纵向振幅 = new Vector2(1f, 0.55f); settings.震动频率 = 28f; settings.画面旋转角度 = 0.15f; break;
            case 震屏预设.重击: settings.位移强度 = 0.018f; settings.横向与纵向振幅 = new Vector2(1f, 0.62f); settings.震动频率 = 20f; settings.画面旋转角度 = 0.65f; break;
            case 震屏预设.爆炸: settings.位移强度 = 0.035f; settings.横向与纵向振幅 = new Vector2(1.1f, 0.85f); settings.震动频率 = 16f; settings.画面旋转角度 = 1.2f; break;
            case 震屏预设.大招爆发: settings.位移强度 = 0.052f; settings.横向与纵向振幅 = new Vector2(1.25f, 1f); settings.震动频率 = 12f; settings.画面旋转角度 = 2.1f; break;
        }
    }

    public static void 应用色相分离预设(色相分离设置 settings)
    {
        if (settings == null) return;
        switch (settings.常用预设)
        {
            case 色相分离预设.轻微命中色散: settings.分离方式 = 色相分离方式.从中心向外; settings.分离强度 = 0.0045f; settings.边缘增强 = 0.25f; settings.红蓝偏移比例 = 0.9f; break;
            case 色相分离预设.空间撕裂: settings.分离方式 = 色相分离方式.从中心向外; settings.分离强度 = 0.016f; settings.边缘增强 = 1.25f; settings.红蓝偏移比例 = 1.25f; break;
            case 色相分离预设.能量爆发: settings.分离方式 = 色相分离方式.从中心向外; settings.分离强度 = 0.009f; settings.边缘增强 = 0.75f; settings.红蓝偏移比例 = 1.05f; break;
            case 色相分离预设.科技故障: settings.分离方式 = 色相分离方式.固定方向; settings.固定方向角度 = 0f; settings.分离强度 = 0.018f; settings.边缘增强 = 0.18f; settings.红蓝偏移比例 = 1.5f; break;
        }
    }

    public static void 应用全屏模糊预设(全屏模糊设置 settings)
    {
        if (settings == null) return;
        switch (settings.常用预设)
        {
            case 全屏模糊预设.轻微失焦: settings.模糊方式 = 模糊模式.全屏均匀模糊; settings.模糊半径像素 = 8f; settings.模糊强化倍数 = 1.25f; settings.模糊混合 = 0.60f; settings.径向采样次数 = 12; break;
            case 全屏模糊预设.大招蓄力: settings.模糊方式 = 模糊模式.向中心拉伸; settings.模糊半径像素 = 18f; settings.模糊强化倍数 = 1.6f; settings.模糊混合 = 0.78f; settings.径向采样次数 = 16; break;
            case 全屏模糊预设.爆发拉伸: settings.模糊方式 = 模糊模式.向外扩散; settings.模糊半径像素 = 30f; settings.模糊强化倍数 = 2.1f; settings.模糊混合 = 0.95f; settings.径向采样次数 = 24; break;
            case 全屏模糊预设.眩晕模糊: settings.模糊方式 = 模糊模式.全屏均匀模糊; settings.模糊半径像素 = 52f; settings.模糊强化倍数 = 2.2f; settings.模糊混合 = 1f; settings.径向采样次数 = 24; break;
        }
    }

    public static void 应用电影黑边框预设(电影黑边框设置 settings)
    {
        if (settings == null) return;
        switch (settings.常用预设)
        {
            case 电影黑边框预设.轻微宽银幕:
                settings.黑边颜色 = Color.black; settings.上下黑边高度 = 0.07f; settings.左右黑边宽度 = 0f; settings.黑边边缘柔和度 = 0.008f; settings.黑边不透明度 = 1f; break;
            case 电影黑边框预设.标准宽银幕:
                settings.黑边颜色 = Color.black; settings.上下黑边高度 = 0.12f; settings.左右黑边宽度 = 0f; settings.黑边边缘柔和度 = 0.008f; settings.黑边不透明度 = 1f; break;
            case 电影黑边框预设.史诗宽银幕:
                settings.黑边颜色 = Color.black; settings.上下黑边高度 = 0.18f; settings.左右黑边宽度 = 0f; settings.黑边边缘柔和度 = 0.004f; settings.黑边不透明度 = 1f; break;
            case 电影黑边框预设.四周收束:
                settings.黑边颜色 = Color.black; settings.上下黑边高度 = 0.10f; settings.左右黑边宽度 = 0.08f; settings.黑边边缘柔和度 = 0.02f; settings.黑边不透明度 = 1f; break;
        }
    }

    public static void 应用全屏放射线预设(全屏放射线设置 settings)
    {
        if (settings == null) return;
        switch (settings.常用预设)
        {
            case 全屏放射线预设.紫色爆发放射:
                settings.放射模式 = 放射线模式.全屏放射; settings.放射线颜色 = new Color(0.55f, 0.2f, 2f, 1f); settings.放射线数量 = 40f; settings.放射线宽度 = 0.065f; settings.放射线发光强度 = 2.4f; settings.放射线流动速度 = 0.45f; settings.放射线闪烁 = 0.32f; settings.背景压暗 = 0.18f; break;
            case 全屏放射线预设.蓝色能量放射:
                settings.放射模式 = 放射线模式.全屏放射; settings.放射线颜色 = new Color(0.2f, 1.1f, 2f, 1f); settings.放射线数量 = 54f; settings.放射线宽度 = 0.045f; settings.放射线发光强度 = 1.8f; settings.放射线流动速度 = 0.7f; settings.放射线闪烁 = 0.2f; settings.背景压暗 = 0.12f; break;
            case 全屏放射线预设.金色大招放射:
                settings.放射模式 = 放射线模式.中心爆发; settings.放射线颜色 = new Color(2f, 1.25f, 0.28f, 1f); settings.放射线数量 = 28f; settings.放射线宽度 = 0.10f; settings.放射线发光强度 = 3.4f; settings.放射线流动速度 = 0.2f; settings.放射线闪烁 = 0.42f; settings.背景压暗 = 0.28f; break;
            case 全屏放射线预设.黑白冲击放射:
                settings.放射模式 = 放射线模式.全屏放射; settings.放射线颜色 = Color.white; settings.放射线数量 = 64f; settings.放射线宽度 = 0.028f; settings.放射线发光强度 = 1.5f; settings.放射线流动速度 = 1f; settings.放射线闪烁 = 0.5f; settings.背景压暗 = 0.55f; break;
        }
    }

    public static void 应用广角预设(广角设置 settings)
    {
        if (settings == null) return;
        switch (settings.常用预设)
        {
            case 广角预设.轻微广角: settings.广角方式 = 广角模式.真实视野角加畸变; settings.目标相机视野角 = 72f; settings.屏幕广角畸变强度 = 0.08f; break;
            case 广角预设.动作广角: settings.广角方式 = 广角模式.真实视野角加畸变; settings.目标相机视野角 = 82f; settings.屏幕广角畸变强度 = 0.16f; break;
            case 广角预设.大招广角: settings.广角方式 = 广角模式.真实视野角加畸变; settings.目标相机视野角 = 92f; settings.屏幕广角畸变强度 = 0.23f; break;
            case 广角预设.极限鱼眼: settings.广角方式 = 广角模式.屏幕广角畸变; settings.目标相机视野角 = 100f; settings.屏幕广角畸变强度 = 0.42f; break;
        }
    }

    public static void 应用暗角预设(暗角设置 settings)
    {
        if (settings == null) return;
        switch (settings.常用预设)
        {
            case 暗角预设.轻微圆形聚焦:
                settings.暗角颜色 = Color.black; settings.暗角强度 = 0.28f; settings.暗角范围 = 0.78f; settings.暗角柔和度 = 0.38f; settings.形状选择 = 暗角形状.圆形; settings.横向拉伸 = 1f; settings.纵向拉伸 = 1f; settings.圆角矩形圆角 = 0.35f; break;
            case 暗角预设.战斗矩形压迫:
                settings.暗角颜色 = Color.black; settings.暗角强度 = 0.58f; settings.暗角范围 = 0.62f; settings.暗角柔和度 = 0.30f; settings.形状选择 = 暗角形状.矩形; settings.横向拉伸 = 1.05f; settings.纵向拉伸 = 0.92f; settings.圆角矩形圆角 = 0f; break;
            case 暗角预设.大招菱形聚焦:
                settings.暗角颜色 = new Color(0.01f, 0.015f, 0.035f, 1f); settings.暗角强度 = 0.76f; settings.暗角范围 = 0.72f; settings.暗角柔和度 = 0.22f; settings.形状选择 = 暗角形状.菱形; settings.横向拉伸 = 1.25f; settings.纵向拉伸 = 0.95f; settings.圆角矩形圆角 = 0.35f; break;
            case 暗角预设.圆角矩形收束:
                settings.暗角颜色 = Color.black; settings.暗角强度 = 0.72f; settings.暗角范围 = 0.56f; settings.暗角柔和度 = 0.22f; settings.形状选择 = 暗角形状.圆角矩形; settings.横向拉伸 = 1.1f; settings.纵向拉伸 = 0.95f; settings.圆角矩形圆角 = 0.45f; break;
        }
    }

    public static void 应用技能黑白闪预设(技能黑白闪设置 settings)
    {
        if (settings == null) return;
        switch (settings.常用预设)
        {
            case 技能黑白闪预设.高对比斩击卡帧:
                settings.闪屏模式 = 技能黑白闪模式.高对比黑白卡帧; settings.暗部颜色 = Color.black; settings.亮部颜色 = Color.white; settings.黑白阈值 = 0.44f; settings.黑白边缘柔和度 = 0.018f; settings.黑白对比度 = 6.0f; settings.黑白往返次数 = 1; settings.轮廓边缘强度 = 2.6f; settings.轮廓边缘宽度 = 1.25f; settings.轮廓边缘阈值 = 0.06f; break;
            case 技能黑白闪预设.强命中黑白卡帧:
                settings.闪屏模式 = 技能黑白闪模式.高对比黑白卡帧; settings.暗部颜色 = new Color(0.01f, 0.01f, 0.01f, 1f); settings.亮部颜色 = Color.white; settings.黑白阈值 = 0.50f; settings.黑白边缘柔和度 = 0.008f; settings.黑白对比度 = 7.5f; settings.黑白往返次数 = 1; settings.轮廓边缘强度 = 3.2f; settings.轮廓边缘宽度 = 1.5f; settings.轮廓边缘阈值 = 0.045f; break;
            case 技能黑白闪预设.黑白反相往返:
                settings.闪屏模式 = 技能黑白闪模式.黑白反相往返; settings.暗部颜色 = Color.black; settings.亮部颜色 = Color.white; settings.黑白阈值 = 0.46f; settings.黑白边缘柔和度 = 0.020f; settings.黑白对比度 = 5.5f; settings.黑白往返次数 = 1; settings.轮廓边缘强度 = 2.0f; settings.轮廓边缘宽度 = 1.1f; settings.轮廓边缘阈值 = 0.07f; break;
            case 技能黑白闪预设.爆发白闪:
                settings.闪屏模式 = 技能黑白闪模式.全屏白闪; settings.暗部颜色 = Color.black; settings.亮部颜色 = Color.white; settings.黑白阈值 = 0.5f; settings.黑白边缘柔和度 = 0.02f; settings.黑白对比度 = 1f; settings.黑白往返次数 = 1; settings.轮廓边缘强度 = 0f; settings.轮廓边缘宽度 = 1f; settings.轮廓边缘阈值 = 0.1f; break;
            case 技能黑白闪预设.压迫黑闪:
                settings.闪屏模式 = 技能黑白闪模式.全屏黑闪; settings.暗部颜色 = Color.black; settings.亮部颜色 = Color.white; settings.黑白阈值 = 0.5f; settings.黑白边缘柔和度 = 0.02f; settings.黑白对比度 = 1f; settings.黑白往返次数 = 1; settings.轮廓边缘强度 = 0f; settings.轮廓边缘宽度 = 1f; settings.轮廓边缘阈值 = 0.1f; break;
        }
    }


    public static void 应用漫画滤镜预设(漫画滤镜设置 settings)
    {
        if (settings == null) return;
        switch (settings.常用预设)
        {
            case 漫画滤镜预设.黑白放射漫画:
                settings.漫画模式 = 漫画滤镜模式.高对比放射漫画;
                settings.墨色 = Color.black;
                settings.纸张色 = Color.white;
                settings.黑白阈值 = 0.48f;
                settings.黑白柔和度 = 0.012f;
                settings.黑白对比度 = 7.0f;
                settings.爆发中心 = new Vector2(0.52f, 0.50f);
                settings.放射笔刷数量 = 58f;
                settings.笔刷宽度 = 0.055f;
                settings.笔刷长度 = 1.10f;
                settings.中心留白范围 = 0.10f;
                settings.笔刷不规则度 = 0.62f;
                settings.笔刷断裂 = 0.28f;
                settings.笔刷强度 = 1.0f;
                settings.轮廓边缘强度 = 2.2f;
                settings.轮廓边缘宽度 = 1.2f;
                settings.轮廓边缘阈值 = 0.07f;
                break;
            case 漫画滤镜预设.斩击速度线:
                settings.漫画模式 = 漫画滤镜模式.高对比放射漫画;
                settings.墨色 = Color.black;
                settings.纸张色 = Color.white;
                settings.黑白阈值 = 0.44f;
                settings.黑白柔和度 = 0.008f;
                settings.黑白对比度 = 7.5f;
                settings.爆发中心 = new Vector2(0.46f, 0.52f);
                settings.放射笔刷数量 = 76f;
                settings.笔刷宽度 = 0.038f;
                settings.笔刷长度 = 1.22f;
                settings.中心留白范围 = 0.06f;
                settings.笔刷不规则度 = 0.72f;
                settings.笔刷断裂 = 0.42f;
                settings.笔刷强度 = 1.25f;
                settings.轮廓边缘强度 = 2.8f;
                settings.轮廓边缘宽度 = 1.35f;
                settings.轮廓边缘阈值 = 0.06f;
                break;
            case 漫画滤镜预设.强烈冲击漫画:
                settings.漫画模式 = 漫画滤镜模式.高对比放射漫画;
                settings.墨色 = Color.black;
                settings.纸张色 = Color.white;
                settings.黑白阈值 = 0.52f;
                settings.黑白柔和度 = 0.006f;
                settings.黑白对比度 = 8.0f;
                settings.爆发中心 = new Vector2(0.50f, 0.50f);
                settings.放射笔刷数量 = 42f;
                settings.笔刷宽度 = 0.090f;
                settings.笔刷长度 = 1.28f;
                settings.中心留白范围 = 0.08f;
                settings.笔刷不规则度 = 0.80f;
                settings.笔刷断裂 = 0.18f;
                settings.笔刷强度 = 1.5f;
                settings.轮廓边缘强度 = 3.8f;
                settings.轮廓边缘宽度 = 1.55f;
                settings.轮廓边缘阈值 = 0.05f;
                break;
            case 漫画滤镜预设.仅放射线叠加:
                settings.漫画模式 = 漫画滤镜模式.放射线叠加;
                settings.墨色 = Color.black;
                settings.纸张色 = Color.white;
                settings.黑白阈值 = 0.50f;
                settings.黑白柔和度 = 0.020f;
                settings.黑白对比度 = 1.0f;
                settings.爆发中心 = new Vector2(0.5f, 0.5f);
                settings.放射笔刷数量 = 60f;
                settings.笔刷宽度 = 0.045f;
                settings.笔刷长度 = 1.10f;
                settings.中心留白范围 = 0.12f;
                settings.笔刷不规则度 = 0.55f;
                settings.笔刷断裂 = 0.30f;
                settings.笔刷强度 = 0.85f;
                settings.轮廓边缘强度 = 0f;
                settings.轮廓边缘宽度 = 1f;
                settings.轮廓边缘阈值 = 0.1f;
                break;
        }
    }

    private bool 确保运行材质()
    {
        if (后效Shader == null)
        {
            后效Shader = Shader.Find(ShaderName);
        }
        if (后效Shader == null || !后效Shader.isSupported)
        {
            return false;
        }
        if (runtimeMaterial == null || runtimeMaterial.shader != 后效Shader)
        {
            释放运行材质();
            runtimeMaterial = new Material(后效Shader);
            runtimeMaterial.hideFlags = HideFlags.HideAndDontSave;
        }
        return true;
    }

    private void 释放运行材质()
    {
        if (runtimeMaterial == null)
        {
            return;
        }
        if (Application.isPlaying)
        {
            Destroy(runtimeMaterial);
        }
        else
        {
            DestroyImmediate(runtimeMaterial);
        }
        runtimeMaterial = null;
    }
}
