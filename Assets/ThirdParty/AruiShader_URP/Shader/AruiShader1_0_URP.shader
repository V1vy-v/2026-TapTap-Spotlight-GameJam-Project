Shader "VFX/AruiShader1.0_URP"
{
    Properties
    {
        _MainTex ("主帖图 / 黑白形体", 2D) = "white" {}
        _MainAlphaStrength ("主贴图Alpha强度", Range(0,10)) = 1
        _SubTex ("副帖图 / 黑白细节", 2D) = "gray" {}
        _MaskTex ("遮罩贴图 / 黑白遮罩", 2D) = "white" {}
        _AuxMaskTex ("辅助遮罩贴图", 2D) = "white" {}
        _RampTex ("Ramp渐变贴图", 2D) = "white" {}
        _EdgeRampTex ("边缘Ramp贴图", 2D) = "white" {}
        _FresnelRampTex ("菲尼尔Ramp贴图", 2D) = "white" {}
        _DistortTex ("扭曲黑白图", 2D) = "gray" {}
        _DistortMaskTex ("扭曲遮罩黑白图", 2D) = "white" {}
        _ParallaxTex ("视差黑白/法线图", 2D) = "gray" {}
        _FlowMapTex ("FlowMap流向图 RG", 2D) = "gray" {}
        _VertexTex ("顶点偏移控制黑白图", 2D) = "black" {}
        _VertexOffsetMaskTex ("顶点偏移黑白遮罩图", 2D) = "white" {}
        _VATTex ("VAT顶点动画贴图", 2D) = "gray" {}
        _DissolveTex ("溶解噪波 / 黑白图", 2D) = "white" {}
        _DissolveExtraTex ("溶解方向图 / 溶解遮罩", 2D) = "white" {}

        [HideInInspector]_SrcBlend ("SrcBlend", Float) = 5
        [HideInInspector]_DstBlend ("DstBlend", Float) = 10
        [HideInInspector]_ZWrite ("ZWrite", Float) = 0
        [HideInInspector]_CullMode ("Cull", Float) = 0
        _RenderQueueValue ("Render Queue", Range(2500,5000)) = 3000
        [HideInInspector]_ZTestMode ("ZTest 深度测试", Float) = 4
        [Toggle]_ZTestToggle ("启用ZTest深度测试（默认开启）", Float) = 1

        _BlendMode ("混合模式（默认Alpha透明）", Float) = 0
        _OutputMode ("输出模式", Float) = 0
        _DebugMode ("调试显示模式", Float) = 0
        _MaterialType ("材质类型", Float) = 0
        _PerformanceMode ("性能标准", Float) = 0
        _RandomMode ("随机范围", Float) = 0
        _PanelMode ("面板模式", Float) = 1

        _CullDisplayMode ("显示面模式", Float) = 0
        [HideInInspector]_DoubleSided ("双面显示旧开关", Float) = 1
        [Toggle]_ZWriteToggle ("写入深度", Float) = 0
        [Toggle]_UseStencil ("启用模板测试", Float) = 0
        _StencilRef ("模板Ref", Range(0,255)) = 0
        _StencilComp ("模板比较", Float) = 8
        _StencilPass ("模板通过操作", Float) = 0
        _StencilFail ("模板失败操作", Float) = 0
        _StencilZFail ("模板深度失败操作", Float) = 0
        _StencilReadMask ("模板读取Mask", Range(0,255)) = 255
        _StencilWriteMask ("模板写入Mask", Range(0,255)) = 255

        [Toggle]_UseMainLayer ("启用主帖图层", Float) = 1
        [Toggle]_UseSubLayer ("启用副帖图层", Float) = 0
        [Toggle]_UseMaskLayer ("启用遮罩层", Float) = 0
        [Toggle]_UseAuxMaskLayer ("启用辅助遮罩", Float) = 0
                [Toggle]_UseRampModule ("启用Ramp渐变", Float) = 0
        [Toggle]_UseAlphaModule ("启用Alpha/模式", Float) = 1
        [Toggle]_UseUVModule ("启用UV变形", Float) = 0
        [Toggle]_UseDissolveModule ("启用溶解", Float) = 0
        [Toggle]_UseDirectionDissolve ("启用方向溶解", Float) = 0
        [Toggle]_UseLayerDissolve ("启用三层溶解", Float) = 0
        [Toggle]_UseEdgeModule ("启用边缘亮边", Float) = 0
        [Toggle]_UseFresnelModule ("启用菲尼尔", Float) = 0
        [Toggle]_UseDistortModule ("启用扭曲/热浪", Float) = 0
        [Toggle]_UseFlowMap ("启用FlowMap", Float) = 0
        [Toggle]_UseVertexModule ("启用顶点动画", Float) = 0
        [Toggle]_UseSoftParticles ("启用接触边缘效果", Float) = 0
        [Toggle]_UseCameraDistanceFade ("启用摄像机距离淡出", Float) = 0
        _CameraNearFadeStart ("近距离淡出开始", Range(0,100)) = 0
        _CameraNearFadeEnd ("近距离淡出结束", Range(0,100)) = 0
        _CameraFarFadeStart ("远距离淡出开始", Range(0,500)) = 100
        _CameraFarFadeEnd ("远距离淡出结束", Range(0,500)) = 120
[Toggle]_UseCDMainOffset ("Custom1 XY控制主帖图Offset", Float) = 0
        [Toggle]_UseCDMaskOffset ("Custom2 XY控制遮罩Offset", Float) = 0
        [Toggle]_UseCDDissolve ("Custom2 Z控制溶解进度", Float) = 0
[Toggle]_UseCDDistort ("Custom1 Z控制扭曲强度", Float) = 0
        [HideInInspector]_UseCDFresnel ("Custom1 W控制菲尼尔强度", Float) = 0
        [Toggle]_UseCDDissolveOffset ("Custom1 W控制溶解Offset", Float) = 0
        _CDDissolveOffsetAxis ("Custom1W控制轴", Float) = 0
        [Toggle]_UseCDVertex ("Custom2 W控制顶点偏移强度", Float) = 0

        [Toggle]_MainUseFlow ("主帖图自流动", Float) = 0
        _MainFlowX ("主帖图流动方向X", Range(-5,5)) = 0
        _MainFlowY ("主帖图流动方向Y", Range(-5,5)) = 0
        _MainFlowSpeed ("主帖图流动倍数", Range(0,10)) = 1
        _MainWrapModeX ("主贴图Wrap X轴", Float) = 0
        _MainWrapModeY ("主贴图Wrap Y轴", Float) = 0
        [Toggle]_MainUsePolar ("主贴图极坐标", Float) = 0
        [Toggle]_MainUseRotate ("主贴图自旋转", Float) = 0
        _MainRotateAngle ("主贴图旋转角度", Range(-360,360)) = 0
        _MainRotateSpeed ("主贴图旋转速度", Range(-720,720)) = 0
        _MainUVCenterX ("主贴图UV中心X", Range(0,1)) = 0.5
        _MainUVCenterY ("主贴图UV中心Y", Range(0,1)) = 0.5
        _MainContrast ("主帖图对比度", Range(0.1,8)) = 1
        _MainPower ("主帖图明暗强度", Range(0.1,8)) = 1
        _MainShapeSource ("主帖图形体来源", Float) = 2
        _MainColorMix ("主贴图颜色混合", Range(0,1)) = 1
        [Toggle]_MainUseBlur ("主贴图启用模糊", Float) = 0
        _MainBlurMode ("主贴图模糊方式", Float) = 0
        _MainBlurStrength ("主贴图模糊强度", Range(0,12)) = 1
        _MainMotionBlurAngle ("主贴图动感模糊方向", Range(-360,360)) = 0
        [Toggle]_MainUseFlipbook ("主贴图Flipbook序列图", Float) = 0
        _MainFlipbookColumns ("Flipbook横向帧数", Range(1,16)) = 4
        _MainFlipbookRows ("Flipbook纵向帧数", Range(1,16)) = 4
        _MainFlipbookSpeed ("Flipbook播放速度", Range(-60,60)) = 12
        _MainFlipbookFrame ("Flipbook当前帧", Range(0,255)) = 0
        [Toggle]_MainFlipbookLoop ("Flipbook循环", Float) = 1
        [Toggle]_MainUseHueSplit ("主贴图色相分离", Float) = 0
        _MainHueSplitAmount ("主贴图分离强度", Range(0,0.1)) = 0
        _MainHueSplitAngle ("主贴图分离角度", Range(-360,360)) = 0
        _MainHueSplitSpeed ("主贴图分离旋转速度", Range(-720,720)) = 0

        [Toggle]_SubUseFlow ("副帖图自流动", Float) = 0
        _SubFlowX ("副帖图流动方向X", Range(-5,5)) = 0
        _SubFlowY ("副帖图流动方向Y", Range(-5,5)) = 0
        _SubFlowSpeed ("副帖图流动倍数", Range(0,10)) = 1
        _SubWrapModeX ("副贴图Wrap X轴", Float) = 0
        _SubWrapModeY ("副贴图Wrap Y轴", Float) = 0
        [Toggle]_SubUsePolar ("副贴图极坐标", Float) = 0
        [Toggle]_SubUseRotate ("副贴图自旋转", Float) = 0
        _SubRotateAngle ("副贴图旋转角度", Range(-360,360)) = 0
        _SubRotateSpeed ("副贴图旋转速度", Range(-720,720)) = 0
        _SubUVCenterX ("副贴图UV中心X", Range(0,1)) = 0.5
        _SubUVCenterY ("副贴图UV中心Y", Range(0,1)) = 0.5
        _SubBlendMode ("副帖图混合模式", Float) = 0
        _SubContrast ("副帖图对比度", Range(0.1,8)) = 1
        _SubPower ("副帖图明暗强度", Range(0.1,8)) = 1
        _SubStrength ("副帖图强度", Range(0,3)) = 1
        [Toggle]_SubRemoveBlack ("副贴图去黑底", Float) = 0
        [HDR]_SubColor ("副贴图颜色 HDR", Color) = (1,1,1,1)
        _SubColorStrength ("副贴图颜色强度", Range(0,5)) = 0
        _SubColorIntensity ("副贴图HDR增强", Range(0,10)) = 0
        [Toggle]_SubUseHueSplit ("副贴图色相分离", Float) = 0
        _SubHueSplitAmount ("副贴图分离强度", Range(0,0.1)) = 0
        _SubHueSplitAngle ("副贴图分离角度", Range(-360,360)) = 0
        _SubHueSplitSpeed ("副贴图分离旋转速度", Range(-720,720)) = 0

        [Toggle]_MaskUseFlow ("遮罩自流动", Float) = 0
        _MaskFlowX ("遮罩流动方向X", Range(-5,5)) = 0
        _MaskFlowY ("遮罩流动方向Y", Range(-5,5)) = 0
        _MaskFlowSpeed ("遮罩流动倍数", Range(0,10)) = 1
        _MaskWrapModeX ("遮罩Wrap X轴", Float) = 0
        _MaskWrapModeY ("遮罩Wrap Y轴", Float) = 0
        [Toggle]_MaskUsePolar ("遮罩极坐标", Float) = 0
        [Toggle]_MaskUseRotate ("遮罩自旋转", Float) = 0
        _MaskRotateAngle ("遮罩旋转角度", Range(-360,360)) = 0
        _MaskRotateSpeed ("遮罩旋转速度", Range(-720,720)) = 0
        _MaskUVCenterX ("遮罩UV中心X", Range(0,1)) = 0.5
        _MaskUVCenterY ("遮罩UV中心Y", Range(0,1)) = 0.5
        [Toggle]_MaskInvert ("遮罩反转", Float) = 0
        _MaskContrast ("遮罩对比度", Range(0.1,8)) = 1
        _MaskPower ("遮罩明暗强度", Range(0.1,8)) = 1
        _MaskAffectAlpha ("遮罩影响Alpha", Range(0,1)) = 1
        _MaskAffectColor ("遮罩影响颜色", Range(0,1)) = 0
        _MaskAffectDissolve ("遮罩影响溶解", Range(0,1)) = 0
        _MaskAffectDistort ("遮罩影响扭曲", Range(0,1)) = 0
        _MaskAffectFresnel ("遮罩影响菲尼尔", Range(0,1)) = 0

        _AuxMaskWrapModeX ("辅助遮罩Wrap X轴", Float) = 0
        _AuxMaskWrapModeY ("辅助遮罩Wrap Y轴", Float) = 0
        [Toggle]_AuxMaskUsePolar ("辅助遮罩极坐标", Float) = 0
        [Toggle]_AuxMaskUseRotate ("辅助遮罩自旋转", Float) = 0
        _AuxMaskRotateAngle ("辅助遮罩旋转角度", Range(-360,360)) = 0
        _AuxMaskRotateSpeed ("辅助遮罩旋转速度", Range(-720,720)) = 0
        _AuxMaskUVCenterX ("辅助遮罩UV中心X", Range(0,1)) = 0.5
        _AuxMaskUVCenterY ("辅助遮罩UV中心Y", Range(0,1)) = 0.5
        [Toggle]_AuxMaskUseFlow ("辅助遮罩自流动", Float) = 0
        _AuxMaskFlowX ("辅助遮罩流动方向X", Range(-5,5)) = 0
        _AuxMaskFlowY ("辅助遮罩流动方向Y", Range(-5,5)) = 0
        _AuxMaskFlowSpeed ("辅助遮罩流动倍数", Range(0,10)) = 1
        _AuxMaskChannel ("辅助遮罩通道", Float) = 0
        [Toggle]_AuxMaskInvert ("辅助遮罩反转", Float) = 0
        _AuxMaskContrast ("辅助遮罩对比度", Range(0.1,8)) = 1
        _AuxMaskPower ("辅助遮罩明暗强度", Range(0.1,8)) = 1
        _AuxMaskAffectAlpha ("辅助遮罩影响Alpha", Range(0,1)) = 1
        _AuxMaskAffectColor ("辅助遮罩影响颜色", Range(0,1)) = 0
        _AuxMaskAffectDissolve ("辅助遮罩影响溶解", Range(0,1)) = 0
        _AuxMaskAffectDistort ("辅助遮罩影响扭曲", Range(0,1)) = 0
        _AuxMaskAffectFresnel ("辅助遮罩影响菲尼尔", Range(0,1)) = 0


        [Toggle]_UseThreeColor ("启用三段色", Float) = 0
        [HDR]_SingleColor ("单色板颜色 HDR", Color) = (1,1,1,1)
        [Toggle]_UseBackFaceColor ("开启背面颜色控制", Float) = 0
        [HDR]_BackFaceColor ("背面颜色 HDR", Color) = (1,1,1,1)
        _BackFaceColorSoftness ("切边柔和度", Range(0,1)) = 0.15
        [HDR]_ColorA ("三段色A / 暗部", Color) = (1,1,1,1)
        [HDR]_ColorB ("三段色B / 主体", Color) = (1,1,1,1)
        [HDR]_ColorC ("三段色C / 高亮", Color) = (1,1,1,1)
        [HDR]_FresnelColor ("菲尼尔颜色 HDR", Color) = (1,1,1,1)
        _Opacity ("整体透明度", Range(0,1)) = 1
        _EmissionIntensity ("颜色强度", Range(0,20)) = 0
        [Toggle]_UseColorAdjust ("启用颜色后处理", Float) = 0
        _HueShift ("色相偏移", Range(-1,1)) = 0
        _Saturation ("饱和度", Range(0,3)) = 1
        _Value ("明度", Range(0,3)) = 1
        _ColorContrast ("颜色对比度", Range(0,3)) = 1
        [Toggle]_InvertColor ("反相颜色", Float) = 0
        [Toggle]_UseBlackWhiteFlash ("黑白闪", Float) = 0
        _BlackWhiteFlashValue ("黑白闪强度", Range(0,1)) = 0
        _OverExposureClamp ("过曝保护", Range(0,20)) = 10
        [HideInInspector]_UseParticleColor ("粒子控制颜色", Float) = 1
        [HideInInspector]_ParticleColorStrength ("粒子颜色强度", Float) = 1
        [HideInInspector]_UseParticleAlpha ("粒子控制透明度", Float) = 1
        [HideInInspector]_ParticleAlphaStrength ("粒子透明度强度", Float) = 1
        [HideInInspector]_ParticleColorAffectEdge ("粒子颜色影响边缘", Float) = 0
        [HideInInspector]_ParticleColorAffectFresnel ("粒子颜色影响菲尼尔", Float) = 0
        _ColorSplit1 ("三段色分层1", Range(0,1)) = 0.35
        _ColorSplit2 ("三段色分层2", Range(0,1)) = 0.68
        _ColorSoftness ("颜色过渡软度", Range(0.001,0.4)) = 0.06
        [HideInInspector]_RampSource ("Ramp来源", Float) = 0
        _RampBlendMode ("Ramp叠加模式", Float) = 0
        [Toggle]_RampUseFlow ("Ramp自流动", Float) = 0
        _RampFlowX ("Ramp流动方向X", Range(-5,5)) = 0
        _RampFlowY ("Ramp流动方向Y", Range(-5,5)) = 0
        _RampFlowSpeed ("Ramp流动倍数", Range(0,10)) = 1
        _RampWrapModeX ("RampWrap X轴", Float) = 0
        _RampWrapModeY ("RampWrap Y轴", Float) = 0
        [Toggle]_RampUsePolar ("Ramp极坐标", Float) = 0
        [Toggle]_RampUseRotate ("Ramp自旋转", Float) = 0
        _RampRotateAngle ("Ramp旋转角度", Range(-360,360)) = 0
        _RampRotateSpeed ("Ramp旋转速度", Range(-720,720)) = 0
        _RampUVCenterX ("RampUV中心X", Range(0,1)) = 0.5
        _RampUVCenterY ("RampUV中心Y", Range(0,1)) = 0.5
        _RampOffset ("Ramp偏移", Range(-1,1)) = 0
        _RampContrast ("Ramp对比度", Range(0.1,8)) = 1
        _RampStrength ("Ramp强度", Range(0,1)) = 1
        [Toggle]_UseEdgeRamp ("启用边缘Ramp", Float) = 0
        _EdgeRampStrength ("边缘Ramp强度", Range(0,1)) = 0
        _EdgeRampWrapModeX ("边缘RampWrap X轴", Float) = 0
        _EdgeRampWrapModeY ("边缘RampWrap Y轴", Float) = 0
        [Toggle]_EdgeRampUsePolar ("边缘Ramp极坐标", Float) = 0
        [Toggle]_EdgeRampUseRotate ("边缘Ramp自旋转", Float) = 0
        _EdgeRampRotateAngle ("边缘Ramp旋转角度", Range(-360,360)) = 0
        _EdgeRampRotateSpeed ("边缘Ramp旋转速度", Range(-720,720)) = 0
        _EdgeRampUVCenterX ("边缘RampUV中心X", Range(0,1)) = 0.5
        _EdgeRampUVCenterY ("边缘RampUV中心Y", Range(0,1)) = 0.5
        [Toggle]_UseFresnelRamp ("启用菲尼尔Ramp", Float) = 0
        _FresnelRampStrength ("菲尼尔Ramp强度", Range(0,1)) = 0
        _FresnelRampWrapModeX ("菲尼尔RampWrap X轴", Float) = 0
        _FresnelRampWrapModeY ("菲尼尔RampWrap Y轴", Float) = 0
        [Toggle]_FresnelRampUsePolar ("菲尼尔Ramp极坐标", Float) = 0
        [Toggle]_FresnelRampUseRotate ("菲尼尔Ramp自旋转", Float) = 0
        _FresnelRampRotateAngle ("菲尼尔Ramp旋转角度", Range(-360,360)) = 0
        _FresnelRampRotateSpeed ("菲尼尔Ramp旋转速度", Range(-720,720)) = 0
        _FresnelRampUVCenterX ("菲尼尔RampUV中心X", Range(0,1)) = 0.5
        _FresnelRampUVCenterY ("菲尼尔RampUV中心Y", Range(0,1)) = 0.5

        [HideInInspector]_ShapeMode ("使用模式", Float) = 1
        _UseMainAlpha ("使用主帖图Alpha", Range(0,1)) = 1
        _AlphaPower ("Alpha强度", Range(0.1,8)) = 1
        [Toggle]_AlphaClipEnable ("开启Alpha硬裁剪", Float) = 0
        _AlphaClip ("Alpha硬裁剪", Range(0,0.5)) = 0.01
        _QuadMaskRadius ("面片圆形遮罩半径", Range(0.05,1.5)) = 0.72
        _QuadMaskSoftness ("面片圆形遮罩软边", Range(0.001,0.5)) = 0.08
        _QuadMaskScaleX ("面片遮罩宽度比例", Range(0.1,3)) = 1
        _QuadMaskScaleY ("面片遮罩高度比例", Range(0.1,3)) = 1

        _UVSwirlStrength ("UV漩涡强度", Range(-10,10)) = 0
        _UVWaveStrength ("UV波浪强度", Range(0,2)) = 0
        _UVWaveScale ("UV波浪密度", Range(0.1,80)) = 10
        _UVWaveSpeed ("UV波浪速度", Range(-10,10)) = 1
        _UVRadialStrength ("UV径向扩散", Range(-5,5)) = 0
        _UVKaleidoscope ("UV万花筒数量", Range(0,12)) = 0
        _UVCenterX ("UV中心X", Range(0,1)) = 0.5
        _UVCenterY ("UV中心Y", Range(0,1)) = 0.5
        _ModelUVMode ("模型UV选择", Float) = 0

        _DissolveAmount ("溶解进度", Range(0,1)) = 0
        [Toggle]_DissolveUseFlow ("溶解自流动", Float) = 0
        _DissolveFlowX ("溶解流动方向X", Range(-5,5)) = 0
        _DissolveFlowY ("溶解流动方向Y", Range(-5,5)) = 0
        _DissolveFlowSpeed ("溶解流动倍数", Range(0,10)) = 1
        _DissolveExtraMode ("溶解方向图模式", Float) = 0
        _DissolveExtraStrength ("溶解方向/遮罩强度", Range(0,1)) = 1
        [Toggle]_DissolveExtraInvert ("溶解方向图反转", Float) = 0
        [Toggle]_DissolveExtraUseFlow ("溶解方向图自流动", Float) = 0
        _DissolveExtraFlowX ("溶解方向图流动X", Range(-5,5)) = 0
        _DissolveExtraFlowY ("溶解方向图流动Y", Range(-5,5)) = 0
        _DissolveExtraFlowSpeed ("溶解方向图流动倍数", Range(0,10)) = 1
        _DissolveExtraWrapModeX ("溶解方向图Wrap X轴", Float) = 0
        _DissolveExtraWrapModeY ("溶解方向图Wrap Y轴", Float) = 0
        [Toggle]_DissolveExtraUsePolar ("溶解方向图极坐标", Float) = 0
        [Toggle]_DissolveExtraUseRotate ("溶解方向图自旋转", Float) = 0
        _DissolveExtraRotateAngle ("溶解方向图旋转角度", Range(-360,360)) = 0
        _DissolveExtraRotateSpeed ("溶解方向图旋转速度", Range(-720,720)) = 0
        _DissolveExtraUVCenterX ("溶解方向图UV中心X", Range(0,1)) = 0.5
        _DissolveExtraUVCenterY ("溶解方向图UV中心Y", Range(0,1)) = 0.5
        _DissolveWrapModeX ("溶解Wrap X轴", Float) = 0
        _DissolveWrapModeY ("溶解Wrap Y轴", Float) = 0
        [Toggle]_DissolveUsePolar ("溶解极坐标", Float) = 0
        [Toggle]_DissolveUseRotate ("溶解自旋转", Float) = 0
        _DissolveRotateAngle ("溶解旋转角度", Range(-360,360)) = 0
        _DissolveRotateSpeed ("溶解旋转速度", Range(-720,720)) = 0
        _DissolveUVCenterX ("溶解UV中心X", Range(0,1)) = 0.5
        _DissolveUVCenterY ("溶解UV中心Y", Range(0,1)) = 0.5
        [Toggle]_DissolveHardEdge ("溶解硬边模式", Float) = 0
        _DissolveSoftness ("溶解软边", Range(0.001,0.4)) = 0.08
        [Toggle]_DissolveSoftPreserveSolid ("软溶解保持实体", Float) = 1
        _DissolveContrast ("溶解对比度", Range(0.1,8)) = 1.4
        _DissolveEdgeWidth ("溶解边缘宽度", Range(0,0.5)) = 0.08
        [HDR]_DissolveEdgeColor ("溶解边缘颜色 HDR", Color) = (1,1,1,1)
        _DissolveEdgeIntensity ("溶解内边缘强度", Range(0,10)) = 2.2
        [HDR]_DissolveOuterEdgeColor ("溶解外边缘颜色 HDR", Color) = (1,1,1,1)
        _DissolveOuterEdgeWidth ("溶解外边缘宽度", Range(0,0.8)) = 0.16
        _DissolveOuterEdgeIntensity ("溶解外边缘强度", Range(0,10)) = 0
        _DissolveEdgeAffectAlpha ("溶解边缘影响Alpha", Range(0,1)) = 0
        _DirectionMode ("方向溶解模式", Float) = 0
        _DirectionStrength ("方向溶解强度", Range(0,1)) = 0.45
        _DirectionNoiseBlend ("方向与噪波融合", Range(0,1)) = 0.75
        _DirectionPower ("方向硬度", Range(0.1,5)) = 1
_Layer1End ("高亮层结束", Range(0,1)) = 0.32
        _Layer2End ("主体层结束", Range(0,1)) = 0.58
        _Layer3Start ("暗部层开始", Range(0,1)) = 0.55
        _LayerDissolveSoftness ("三层溶解边缘软化", Range(0.001,0.5)) = 0.08
        _DissolveCurveMode ("溶解曲线模式", Float) = 0
        _DissolveCurvePower ("溶解曲线幂次", Range(0.1,5)) = 1
        _DissolveDelay ("溶解前段延迟", Range(0,0.9)) = 0
        _DissolveEndBoost ("溶解后段加速", Range(0,1)) = 0

        [HDR]_EdgeOnlyColor ("边缘亮边颜色 HDR", Color) = (1,1,1,1)
        _EdgeWidth ("边缘宽度", Range(0,1)) = 0.08
        _EdgeBrightness ("边缘亮度", Range(0,10)) = 1.2
        _EdgeIntensity ("边缘强度", Range(0,10)) = 1.2
        _EdgeSoftness ("边缘柔度", Range(0.001,0.4)) = 0.08
        _EdgeAffectAlpha ("边缘影响Alpha", Range(0,1)) = 0.15

        [Toggle]_FresnelInvert ("反向菲尼尔", Float) = 0
        [Toggle]_UseFresnelInnerColor ("启用菲尼尔内圈颜色", Float) = 0
        [HDR]_FresnelInnerColor ("菲尼尔内圈颜色 HDR", Color) = (1,1,1,1)
        _FresnelInnerWidth ("菲尼尔内圈过渡范围", Range(0.01,1)) = 0.18
        _FresnelInnerIntensity ("菲尼尔内圈颜色强度", Range(0,10)) = 1
        [Toggle]_DissolveAffectFresnel ("溶解裁切菲尼尔", Float) = 1
        [Toggle]_DissolveDistortAffectFresnel ("溶解噪波扰动菲尼尔", Float) = 0
        _FresnelThickness ("菲尼尔厚度", Range(0.01,1)) = 0.55
        _FresnelIntensity ("菲尼尔强度", Range(0,8)) = 1.2
        _FresnelBrightness ("菲尼尔亮度", Range(0,10)) = 1.0
        _FresnelPower ("菲尼尔锐度", Range(0.1,10)) = 3.0
        _FresnelBias ("菲尼尔底值", Range(0,1)) = 0.02
        _FresnelAffectAlpha ("菲尼尔影响Alpha", Range(0,1)) = 0.45
        _FresnelOutlineExpand ("菲尼尔外轮廓扩张", Range(0,0.5)) = 0
        [Toggle]_FresnelForceHollow ("菲尼尔强制掏空中心", Float) = 1
        _FresnelHollowPower ("菲尼尔掏空锐度", Range(0.1,8)) = 1
        _FresnelHollowMin ("菲尼尔中心透明阈值", Range(0,1)) = 0
        [Toggle]_FresnelHideBackface ("菲尼尔外壳隐藏背面", Float) = 1
        [Toggle]_UseSecondFresnel ("启用双层菲尼尔", Float) = 0
        [HDR]_FresnelSecondColor ("第二层菲尼尔颜色 HDR", Color) = (1,1,1,1)
        _FresnelSecondThickness ("第二层菲尼尔厚度", Range(0.01,1)) = 0.25
        _FresnelSecondIntensity ("第二层菲尼尔强度", Range(0,8)) = 0
        _FresnelSecondBrightness ("第二层菲尼尔亮度", Range(0,10)) = 1
        _FresnelSecondPower ("第二层菲尼尔锐度", Range(0.1,10)) = 2
        _FresnelSecondAffectAlpha ("第二层菲尼尔影响Alpha", Range(0,1)) = 0
        [Toggle]_FresnelUseNoise ("菲尼尔噪波扰动", Float) = 0
        _FresnelNoiseStrength ("菲尼尔噪波强度", Range(0,2)) = 0
        _FresnelNoiseScale ("菲尼尔噪波密度", Range(0.1,80)) = 12
        _FresnelNoiseSpeed ("菲尼尔噪波速度", Range(-10,10)) = 1

        [Toggle]_DistortAffectMain ("扭曲影响主贴图", Float) = 1
        [Toggle]_DistortAffectMask ("扭曲影响遮罩", Float) = 0
        [Toggle]_DistortAffectDissolve ("扭曲影响溶解", Float) = 1
        [Toggle]_DistortAffectFresnel ("扭曲影响菲尼尔", Float) = 0
        [Toggle]_DistortUseFlow ("扭曲自流动", Float) = 0
        _DistortFlowX ("扭曲流动方向X", Range(-5,5)) = 0
        _DistortFlowY ("扭曲流动方向Y", Range(-5,5)) = 0
        _DistortFlowSpeed ("扭曲流动倍数", Range(0,10)) = 1
        _DistortMaskStrength ("扭曲遮罩强度", Range(0,1)) = 1
        [Toggle]_DistortMaskInvert ("扭曲遮罩反转", Float) = 0
        [Toggle]_DistortMaskUseFlow ("扭曲遮罩自流动", Float) = 0
        _DistortMaskFlowX ("扭曲遮罩流动方向X", Range(-5,5)) = 0
        _DistortMaskFlowY ("扭曲遮罩流动方向Y", Range(-5,5)) = 0
        _DistortMaskFlowSpeed ("扭曲遮罩流动倍数", Range(0,10)) = 1
        _DistortMaskWrapModeX ("扭曲遮罩Wrap X轴", Float) = 0
        _DistortMaskWrapModeY ("扭曲遮罩Wrap Y轴", Float) = 0
        [Toggle]_DistortMaskUsePolar ("扭曲遮罩极坐标", Float) = 0
        [Toggle]_DistortMaskUseRotate ("扭曲遮罩自旋转", Float) = 0
        _DistortMaskRotateAngle ("扭曲遮罩旋转角度", Range(-360,360)) = 0
        _DistortMaskRotateSpeed ("扭曲遮罩旋转速度", Range(-720,720)) = 0
        _DistortMaskUVCenterX ("扭曲遮罩UV中心X", Range(0,1)) = 0.5
        _DistortMaskUVCenterY ("扭曲遮罩UV中心Y", Range(0,1)) = 0.5
        _DistortWrapModeX ("扭曲Wrap X轴", Float) = 0
        _DistortWrapModeY ("扭曲Wrap Y轴", Float) = 0
        [Toggle]_DistortUsePolar ("扭曲极坐标", Float) = 0
        [Toggle]_DistortUseRotate ("扭曲自旋转", Float) = 0
        _DistortRotateAngle ("扭曲旋转角度", Range(-360,360)) = 0
        _DistortRotateSpeed ("扭曲旋转速度", Range(-720,720)) = 0
        _DistortUVCenterX ("扭曲UV中心X", Range(0,1)) = 0.5
        _DistortUVCenterY ("扭曲UV中心Y", Range(0,1)) = 0.5
        _DistortStrength ("扭曲强度", Range(0,2)) = 0.12
        _FlowMapStrength ("FlowMap强度", Range(0,1)) = 0.1
        _FlowMapSpeed ("FlowMap速度", Range(-10,10)) = 1
        _FlowMapTiling ("FlowMap平铺", Range(0.01,20)) = 1
        _FlowMapAffectMain ("FlowMap影响主贴图", Range(0,1)) = 1
        _FlowMapAffectSub ("FlowMap影响副贴图", Range(0,1)) = 1
        _FlowMapAffectDissolve ("FlowMap影响溶解", Range(0,1)) = 1
        _FlowMapAffectDistort ("FlowMap影响扭曲", Range(0,1)) = 1
        _DistortGradientStep ("黑白扭曲采样间距", Range(0.0005,0.05)) = 0.01
        [Toggle]_UseParallax ("启用视差偏移", Float) = 0
[Toggle]_UseSpeedLine ("启用屏幕速度线", Float) = 0
_ParallaxMode ("视差模式", Float) = 0
        _ParallaxStrength ("视差强度", Range(-0.2,0.2)) = 0.02
        _ParallaxCenter ("视差黑白中心", Range(0,1)) = 0.5
        _ParallaxAffectMain ("视差影响主贴图", Range(0,1)) = 1
        _ParallaxAffectSub ("视差影响副贴图", Range(0,1)) = 1
        _ParallaxAffectMask ("视差影响遮罩", Range(0,1)) = 0
        _ParallaxAffectDissolve ("视差影响溶解", Range(0,1)) = 1
        [Toggle]_UseEdgeHeat ("启用热浪", Float) = 0
        _HeatAffectMain ("热浪影响主贴图", Range(0,1)) = 1
        _EdgeHeatStrength ("边缘热浪强度", Range(0,10)) = 0.35
        _EdgeHeatScaleX ("边缘热浪密度X", Range(1,80)) = 26
        _EdgeHeatScaleY ("边缘热浪密度Y", Range(1,80)) = 26
        _EdgeHeatSpeed ("边缘热浪速度", Range(0,10)) = 2
        [Toggle]_UseRadialWave ("启用径向波纹/冲击波", Float) = 0
        _RadialWaveCenterX ("波纹中心X", Range(0,1)) = 0.5
        _RadialWaveCenterY ("波纹中心Y", Range(0,1)) = 0.5
        _RadialWaveRadius ("波纹半径", Range(0,2)) = 0.25
        _RadialWaveWidth ("波纹宽度", Range(0.001,1)) = 0.08
        _RadialWaveSoftness ("波纹软边", Range(0.001,1)) = 0.08
        _RadialWaveSpeed ("波纹速度", Range(-5,5)) = 0
        [Toggle]_RadialWaveLoop ("波纹循环", Float) = 0
        [HDR]_RadialWaveColor ("波纹颜色 HDR", Color) = (1,1,1,1)
        _RadialWaveIntensity ("波纹颜色强度", Range(0,10)) = 0
        _RadialWaveAffectAlpha ("波纹影响Alpha", Range(0,1)) = 0
        _RadialWaveDistortStrength ("波纹影响扭曲", Range(0,1)) = 0
        _SpeedLineMode ("速度线UV模式", Float) = 1
        [HDR]_SpeedLineColor ("速度线颜色 HDR", Color) = (1,1,1,1)
        _SpeedLineIntensity ("速度线颜色强度", Range(0,10)) = 0
        _SpeedLineAlpha ("速度线Alpha增强", Range(0,1)) = 0
        _SpeedLineCenterX ("速度线中心X", Range(0,1)) = 0.5
        _SpeedLineCenterY ("速度线中心Y", Range(0,1)) = 0.5
        _SpeedLineFlowX ("速度线流动X", Range(-5,5)) = 0
        _SpeedLineFlowY ("速度线流动Y", Range(-5,5)) = -1
        _SpeedLineSpeed ("速度线速度", Range(-20,20)) = 4
        _SpeedLineRotate ("速度线旋转角度", Range(-360,360)) = 0
        _SpeedLineRotateSpeed ("速度线旋转速度", Range(-720,720)) = 0
        _SpeedLineEdgeStart ("速度线边缘起点", Range(0,1.5)) = 0.35
        _SpeedLineEdgeSoftness ("速度线边缘软度", Range(0.001,1)) = 0.25
        _SpeedLinePower ("速度线明暗强度", Range(0.1,8)) = 1
        [Toggle]_SpeedLineInvert ("速度线反转", Float) = 0
        _SpeedLineAngleTiling ("速度线角向密度", Range(0.1,20)) = 6
        _SpeedLineRadialTiling ("速度线径向密度", Range(0.1,20)) = 3
        _SpeedLineLength ("速度线拉伸长度", Range(0.05,5)) = 1.6
        _SpeedLineCenterClear ("中心留白半径", Range(0,1)) = 0.22
        _SpeedLineCenterSoftness ("中心留白软度", Range(0.001,1)) = 0.25
        _SpeedLineAspectFix ("屏幕比例修正", Range(0,1)) = 1
        _SpeedLineVignette ("边缘压暗/聚焦", Range(0,1)) = 0.35
        [Toggle]_SpeedLineOneClickStyle ("使用冲锋速度线增强", Float) = 1
[Toggle]_SpeedLineAffectDistort ("速度线受扭曲影响", Float) = 0
        [Toggle]_SpeedLineAffectDissolve ("速度线受溶解影响", Float) = 0
        _SpeedLineHoleShape ("速度线掏空形状", Float) = 0
        _SpeedLineHoleScaleX ("掏空形状宽度", Range(0.1,3)) = 1
        _SpeedLineHoleScaleY ("掏空形状高度", Range(0.1,3)) = 1
[Toggle]_VertexUseAnimMode ("顶点使用动画模式", Float) = 0
        [Toggle]_VertexUseFlow ("顶点偏移自流动", Float) = 0
        _VertexFlowX ("顶点流动方向X", Range(-5,5)) = 0
        _VertexFlowY ("顶点流动方向Y", Range(-5,5)) = 0
        _VertexFlowSpeed ("顶点流动倍数", Range(0,10)) = 1
        _VertexMode ("顶点动画模式", Float) = 0
        _VertexStrength ("顶点偏移强度", Range(-3,3)) = 0.2
        _VertexSpeed ("顶点动画速度", Range(-10,10)) = 1
        _VertexScale ("顶点动画密度", Range(0.1,40)) = 4
        _VertexDirectionX ("顶点方向X", Range(-1,1)) = 0
        _VertexDirectionY ("顶点方向Y", Range(-1,1)) = 1
        _VertexDirectionZ ("顶点方向Z", Range(-1,1)) = 0
        _VertexTexContrast ("顶点贴图对比度", Range(0.1,8)) = 1
        _VertexTexOffset ("顶点控制图起始阈值", Range(0,0.99)) = 0
        _VertexTexPower ("顶点贴图影响强度", Range(0.1,8)) = 1
        [Toggle]_VertexOffsetMaskUseFlow ("顶点偏移黑白遮罩自流动", Float) = 0
        _VertexOffsetMaskFlowX ("顶点遮罩流动方向X", Range(-5,5)) = 0
        _VertexOffsetMaskFlowY ("顶点遮罩流动方向Y", Range(-5,5)) = 0
        _VertexOffsetMaskFlowSpeed ("顶点遮罩流动倍数", Range(0,10)) = 1
        _VertexOffsetMaskWrapModeX ("顶点遮罩Wrap X轴", Float) = 0
        _VertexOffsetMaskWrapModeY ("顶点遮罩Wrap Y轴", Float) = 0
        [Toggle]_VertexOffsetMaskUsePolar ("顶点遮罩极坐标", Float) = 0
        [Toggle]_VertexOffsetMaskUseRotate ("顶点遮罩自旋转", Float) = 0
        _VertexOffsetMaskRotateAngle ("顶点遮罩旋转角度", Range(-360,360)) = 0
        _VertexOffsetMaskRotateSpeed ("顶点遮罩旋转速度", Range(-720,720)) = 0
        _VertexOffsetMaskUVCenterX ("顶点遮罩UV中心X", Range(0,1)) = 0.5
        _VertexOffsetMaskUVCenterY ("顶点遮罩UV中心Y", Range(0,1)) = 0.5
        [Toggle]_UseVAT ("启用VAT顶点动画", Float) = 0
        _VATMode ("VAT模式", Float) = 0
        _VATStrength ("VAT强度", Range(-10,10)) = 1
        _VATColumns ("VAT横向帧数", Range(1,32)) = 4
        _VATRows ("VAT纵向帧数", Range(1,32)) = 4
        _VATSpeed ("VAT播放速度", Range(-60,60)) = 12
        _VATFrame ("VAT当前帧", Range(0,1024)) = 0
        [Toggle]_VATLoop ("VAT循环播放", Float) = 1
        _VertexWrapModeX ("顶点贴图Wrap X轴", Float) = 0
        _VertexWrapModeY ("顶点贴图Wrap Y轴", Float) = 0
        [Toggle]_VertexUsePolar ("顶点贴图极坐标", Float) = 0
        [Toggle]_VertexUseRotate ("顶点贴图自旋转", Float) = 0
        _VertexRotateAngle ("顶点贴图旋转角度", Range(-360,360)) = 0
        _VertexRotateSpeed ("顶点贴图旋转速度", Range(-720,720)) = 0
        _VertexUVCenterX ("顶点贴图UV中心X", Range(0,1)) = 0.5
        _VertexUVCenterY ("顶点贴图UV中心Y", Range(0,1)) = 0.5

                _SoftParticleMode ("深度交界模式", Float) = 0
        _SoftParticleDistance ("深度交界距离", Range(0.01,10)) = 1
        _SoftParticleSoftness ("深度交界软度", Range(0.001,1)) = 0.35
        [HDR]_DepthEdgeColor ("深度边缘颜色 HDR", Color) = (1,1,1,1)
        _DepthEdgeIntensity ("深度边缘强度", Range(0,20)) = 2
        _DepthEdgeAffectColor ("深度边缘影响颜色", Range(0,1)) = 1
        _DepthEdgeAffectAlpha ("深度边缘影响Alpha", Range(0,1)) = 0.35
    }


    SubShader
    {
        Tags
        {
            "RenderPipeline"="UniversalPipeline"
            "Queue"="Transparent"
            "RenderType"="Transparent"
            "IgnoreProjector"="True"
        }

        Pass
        {
            Name "AruiShaderURPForward"
            Tags { "LightMode"="UniversalForward" }

            Blend [_SrcBlend] [_DstBlend]
            Cull [_CullMode]
            ZWrite [_ZWrite]
            ZTest [_ZTestMode]

            Stencil
            {
                Ref [_StencilRef]
                ReadMask [_StencilReadMask]
                WriteMask [_StencilWriteMask]
                Comp [_StencilComp]
                Pass [_StencilPass]
                Fail [_StencilFail]
                ZFail [_StencilZFail]
            }

            HLSLPROGRAM
            #pragma target 3.0
            #pragma vertex Vert
            #pragma fragment Frag
            #pragma multi_compile_instancing

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareDepthTexture.hlsl"

            TEXTURE2D(_MainTex);              SAMPLER(sampler_MainTex);
            TEXTURE2D(_SubTex);               SAMPLER(sampler_SubTex);
            TEXTURE2D(_MaskTex);              SAMPLER(sampler_MaskTex);
            TEXTURE2D(_AuxMaskTex);           SAMPLER(sampler_AuxMaskTex);
            TEXTURE2D(_RampTex);              SAMPLER(sampler_RampTex);
            TEXTURE2D(_EdgeRampTex);          SAMPLER(sampler_EdgeRampTex);
            TEXTURE2D(_FresnelRampTex);       SAMPLER(sampler_FresnelRampTex);
            TEXTURE2D(_DistortTex);           SAMPLER(sampler_DistortTex);
            TEXTURE2D(_DistortMaskTex);       SAMPLER(sampler_DistortMaskTex);
            TEXTURE2D(_ParallaxTex);          SAMPLER(sampler_ParallaxTex);
            TEXTURE2D(_FlowMapTex);           SAMPLER(sampler_FlowMapTex);
            TEXTURE2D(_VertexTex);            SAMPLER(sampler_VertexTex);
            TEXTURE2D(_VertexOffsetMaskTex);  SAMPLER(sampler_VertexOffsetMaskTex);
            TEXTURE2D(_VATTex);               SAMPLER(sampler_VATTex);
            TEXTURE2D(_DissolveTex);          SAMPLER(sampler_DissolveTex);
            TEXTURE2D(_DissolveExtraTex);     SAMPLER(sampler_DissolveExtraTex);

            CBUFFER_START(UnityPerMaterial)
                float4 _MainTex_ST, _SubTex_ST, _MaskTex_ST, _AuxMaskTex_ST;
                float4 _RampTex_ST, _EdgeRampTex_ST, _FresnelRampTex_ST;
                float4 _DistortTex_ST, _DistortMaskTex_ST, _ParallaxTex_ST, _FlowMapTex_ST;
                float4 _VertexTex_ST, _VertexOffsetMaskTex_ST, _VATTex_ST, _DissolveTex_ST, _DissolveExtraTex_ST;
                float4 _MainTex_TexelSize;

                float _BlendMode, _OutputMode, _DebugMode;
                float _UseMainLayer, _UseSubLayer, _UseMaskLayer, _UseAuxMaskLayer, _UseRampModule;
                float _UseAlphaModule, _UseUVModule, _UseDissolveModule, _UseDirectionDissolve;
                float _UseLayerDissolve, _UseEdgeModule, _UseFresnelModule, _UseDistortModule;
                float _UseFlowMap, _UseRadialWave, _UseParallax, _UseSpeedLine, _UseVertexModule;
                float _UseSoftParticles, _UseCameraDistanceFade;

                float _UseCDMainOffset, _UseCDMaskOffset, _UseCDDissolve, _UseCDDistort;
                float _UseCDDissolveOffset, _CDDissolveOffsetAxis, _UseCDVertex;

                float _MainUseFlow, _MainFlowX, _MainFlowY, _MainFlowSpeed;
                float _MainWrapModeX, _MainWrapModeY, _MainUsePolar, _MainUseRotate;
                float _MainRotateAngle, _MainRotateSpeed, _MainUVCenterX, _MainUVCenterY;
                float _MainContrast, _MainPower, _MainShapeSource, _MainColorMix;
                float _MainAlphaStrength, _MainUseBlur, _MainBlurMode, _MainBlurStrength, _MainMotionBlurAngle;
                float _MainUseFlipbook, _MainFlipbookColumns, _MainFlipbookRows, _MainFlipbookSpeed, _MainFlipbookFrame, _MainFlipbookLoop;
                float _MainUseHueSplit, _MainHueSplitAmount, _MainHueSplitAngle, _MainHueSplitSpeed;

                float _SubUseFlow, _SubFlowX, _SubFlowY, _SubFlowSpeed;
                float _SubWrapModeX, _SubWrapModeY, _SubUsePolar, _SubUseRotate;
                float _SubRotateAngle, _SubRotateSpeed, _SubUVCenterX, _SubUVCenterY;
                float _SubBlendMode, _SubContrast, _SubPower, _SubStrength, _SubRemoveBlack, _SubColorStrength, _SubColorIntensity;
                float _SubUseHueSplit, _SubHueSplitAmount, _SubHueSplitAngle, _SubHueSplitSpeed;

                float _MaskUseFlow, _MaskFlowX, _MaskFlowY, _MaskFlowSpeed;
                float _MaskWrapModeX, _MaskWrapModeY, _MaskUsePolar, _MaskUseRotate;
                float _MaskRotateAngle, _MaskRotateSpeed, _MaskUVCenterX, _MaskUVCenterY;
                float _MaskInvert, _MaskContrast, _MaskPower, _MaskAffectAlpha, _MaskAffectColor, _MaskAffectDissolve, _MaskAffectDistort, _MaskAffectFresnel;

                float _AuxMaskUseFlow, _AuxMaskFlowX, _AuxMaskFlowY, _AuxMaskFlowSpeed;
                float _AuxMaskWrapModeX, _AuxMaskWrapModeY, _AuxMaskUsePolar, _AuxMaskUseRotate;
                float _AuxMaskRotateAngle, _AuxMaskRotateSpeed, _AuxMaskUVCenterX, _AuxMaskUVCenterY;
                float _AuxMaskChannel, _AuxMaskInvert, _AuxMaskContrast, _AuxMaskPower;
                float _AuxMaskAffectAlpha, _AuxMaskAffectColor, _AuxMaskAffectDissolve, _AuxMaskAffectDistort, _AuxMaskAffectFresnel;

                float _UseThreeColor, _UseBackFaceColor, _BackFaceColorSoftness;
                float4 _SingleColor, _BackFaceColor, _ColorA, _ColorB, _ColorC;
                float4 _SubColor, _FresnelColor, _FresnelInnerColor, _FresnelSecondColor;
                float4 _DissolveEdgeColor, _DissolveOuterEdgeColor, _EdgeOnlyColor;
                float4 _RadialWaveColor, _SpeedLineColor, _DepthEdgeColor;
                float _Opacity, _EmissionIntensity;
                float _UseParticleColor, _ParticleColorStrength, _UseParticleAlpha, _ParticleAlphaStrength;
                float _ParticleColorAffectEdge, _ParticleColorAffectFresnel;
                float _UseColorAdjust, _HueShift, _Saturation, _Value, _ColorContrast, _InvertColor;
                float _UseBlackWhiteFlash, _BlackWhiteFlashValue, _OverExposureClamp;
                float _ColorSplit1, _ColorSplit2, _ColorSoftness;

                float _RampBlendMode, _RampUseFlow, _RampFlowX, _RampFlowY, _RampFlowSpeed;
                float _RampWrapModeX, _RampWrapModeY, _RampUsePolar, _RampUseRotate;
                float _RampRotateAngle, _RampRotateSpeed, _RampUVCenterX, _RampUVCenterY;
                float _RampOffset, _RampContrast, _RampStrength;

                float _UseEdgeRamp, _EdgeRampStrength;
                float _EdgeRampWrapModeX, _EdgeRampWrapModeY, _EdgeRampUsePolar, _EdgeRampUseRotate;
                float _EdgeRampRotateAngle, _EdgeRampRotateSpeed, _EdgeRampUVCenterX, _EdgeRampUVCenterY;
                float _UseFresnelRamp, _FresnelRampStrength;
                float _FresnelRampWrapModeX, _FresnelRampWrapModeY, _FresnelRampUsePolar, _FresnelRampUseRotate;
                float _FresnelRampRotateAngle, _FresnelRampRotateSpeed, _FresnelRampUVCenterX, _FresnelRampUVCenterY;

                float _UseMainAlpha, _AlphaPower, _AlphaClipEnable, _AlphaClip;
                float _QuadMaskRadius, _QuadMaskSoftness, _QuadMaskScaleX, _QuadMaskScaleY;
                float _UVSwirlStrength, _UVWaveStrength, _UVWaveScale, _UVWaveSpeed, _UVRadialStrength, _UVKaleidoscope, _UVCenterX, _UVCenterY, _ModelUVMode;

                float _DissolveAmount, _DissolveUseFlow, _DissolveFlowX, _DissolveFlowY, _DissolveFlowSpeed;
                float _DissolveWrapModeX, _DissolveWrapModeY, _DissolveUsePolar, _DissolveUseRotate;
                float _DissolveRotateAngle, _DissolveRotateSpeed, _DissolveUVCenterX, _DissolveUVCenterY;
                float _DissolveExtraWrapModeX, _DissolveExtraWrapModeY, _DissolveExtraUsePolar, _DissolveExtraUseRotate;
                float _DissolveExtraRotateAngle, _DissolveExtraRotateSpeed, _DissolveExtraUVCenterX, _DissolveExtraUVCenterY;
                float _DissolveHardEdge, _DissolveSoftness, _DissolveSoftPreserveSolid, _DissolveContrast, _DissolveExtraMode, _DissolveExtraStrength, _DissolveExtraInvert, _DissolveExtraUseFlow, _DissolveExtraFlowX, _DissolveExtraFlowY, _DissolveExtraFlowSpeed, _DissolveEdgeWidth, _DissolveEdgeIntensity;
                float _DissolveOuterEdgeWidth, _DissolveOuterEdgeIntensity, _DissolveEdgeAffectAlpha;
                float _DirectionMode, _DirectionStrength, _DirectionNoiseBlend, _DirectionPower;
                float _Layer1End, _Layer2End, _Layer3Start, _LayerDissolveSoftness, _DissolveCurveMode, _DissolveCurvePower, _DissolveDelay, _DissolveEndBoost;

                float _EdgeWidth, _EdgeBrightness, _EdgeIntensity, _EdgeSoftness, _EdgeAffectAlpha;

                float _FresnelInvert, _UseFresnelInnerColor, _FresnelInnerWidth, _FresnelInnerIntensity;
                float _DissolveAffectFresnel, _DissolveDistortAffectFresnel, _FresnelThickness, _FresnelIntensity, _FresnelBrightness, _FresnelPower, _FresnelBias, _FresnelAffectAlpha;
                float _FresnelOutlineExpand, _FresnelForceHollow, _FresnelHollowPower, _FresnelHollowMin, _FresnelHideBackface;
                float _UseSecondFresnel, _FresnelSecondThickness, _FresnelSecondIntensity, _FresnelSecondBrightness, _FresnelSecondPower, _FresnelSecondAffectAlpha;
                float _FresnelUseNoise, _FresnelNoiseStrength, _FresnelNoiseScale, _FresnelNoiseSpeed;

                float _DistortAffectMain, _DistortAffectMask, _DistortAffectDissolve, _DistortAffectFresnel;
                float _DistortUseFlow, _DistortFlowX, _DistortFlowY, _DistortFlowSpeed;
                float _DistortWrapModeX, _DistortWrapModeY, _DistortUsePolar, _DistortUseRotate;
                float _DistortRotateAngle, _DistortRotateSpeed, _DistortUVCenterX, _DistortUVCenterY;
                float _DistortStrength, _DistortGradientStep;
                float _DistortMaskStrength, _DistortMaskInvert, _DistortMaskUseFlow, _DistortMaskFlowX, _DistortMaskFlowY, _DistortMaskFlowSpeed;
                float _DistortMaskWrapModeX, _DistortMaskWrapModeY, _DistortMaskUsePolar, _DistortMaskUseRotate;
                float _DistortMaskRotateAngle, _DistortMaskRotateSpeed, _DistortMaskUVCenterX, _DistortMaskUVCenterY;
                float _FlowMapStrength, _FlowMapSpeed, _FlowMapTiling, _FlowMapAffectMain, _FlowMapAffectSub, _FlowMapAffectDissolve, _FlowMapAffectDistort;
                float _UseEdgeHeat, _HeatAffectMain, _EdgeHeatStrength, _EdgeHeatScaleX, _EdgeHeatScaleY, _EdgeHeatSpeed;

                float _ParallaxMode, _ParallaxStrength, _ParallaxCenter, _ParallaxAffectMain, _ParallaxAffectSub, _ParallaxAffectMask, _ParallaxAffectDissolve;

                float _RadialWaveCenterX, _RadialWaveCenterY, _RadialWaveRadius, _RadialWaveWidth, _RadialWaveSoftness, _RadialWaveSpeed, _RadialWaveLoop;
                float _RadialWaveIntensity, _RadialWaveAffectAlpha, _RadialWaveDistortStrength;

                float _SpeedLineMode, _SpeedLineIntensity, _SpeedLineAlpha, _SpeedLineCenterX, _SpeedLineCenterY;
                float _SpeedLineFlowX, _SpeedLineFlowY, _SpeedLineSpeed, _SpeedLineRotate, _SpeedLineRotateSpeed;
                float _SpeedLineEdgeStart, _SpeedLineEdgeSoftness, _SpeedLinePower, _SpeedLineInvert;
                float _SpeedLineAngleTiling, _SpeedLineRadialTiling, _SpeedLineLength, _SpeedLineCenterClear, _SpeedLineCenterSoftness;
                float _SpeedLineAspectFix, _SpeedLineVignette, _SpeedLineHoleShape, _SpeedLineHoleScaleX, _SpeedLineHoleScaleY;
                float _SpeedLineAffectDistort, _SpeedLineAffectDissolve;

                float _VertexUseAnimMode, _VertexUseFlow, _VertexFlowX, _VertexFlowY, _VertexFlowSpeed;
                float _VertexMode, _VertexStrength, _VertexSpeed, _VertexScale, _VertexDirectionX, _VertexDirectionY, _VertexDirectionZ;
                float _VertexTexContrast, _VertexTexOffset, _VertexTexPower;
                float _VertexOffsetMaskUseFlow, _VertexOffsetMaskFlowX, _VertexOffsetMaskFlowY, _VertexOffsetMaskFlowSpeed;
                float _VertexOffsetMaskWrapModeX, _VertexOffsetMaskWrapModeY, _VertexOffsetMaskUsePolar, _VertexOffsetMaskUseRotate;
                float _VertexOffsetMaskRotateAngle, _VertexOffsetMaskRotateSpeed, _VertexOffsetMaskUVCenterX, _VertexOffsetMaskUVCenterY;
                float _UseVAT, _VATMode, _VATStrength, _VATColumns, _VATRows, _VATSpeed, _VATFrame, _VATLoop;
                float _VertexWrapModeX, _VertexWrapModeY, _VertexUsePolar, _VertexUseRotate, _VertexRotateAngle, _VertexRotateSpeed, _VertexUVCenterX, _VertexUVCenterY;

                float _SoftParticleMode, _SoftParticleDistance, _SoftParticleSoftness, _DepthEdgeIntensity, _DepthEdgeAffectColor, _DepthEdgeAffectAlpha;
                float _CameraNearFadeStart, _CameraNearFadeEnd, _CameraFarFadeStart, _CameraFarFadeEnd;
            CBUFFER_END

            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS : NORMAL;
                float4 texcoord : TEXCOORD0;
                float4 texcoord1 : TEXCOORD1;
                float4 texcoord2 : TEXCOORD2;
                float4 color : COLOR;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float2 uv : TEXCOORD0;
                float3 positionWS : TEXCOORD1;
                float3 normalWS : TEXCOORD2;
                float4 color : COLOR;
                float4 custom1 : TEXCOORD3;
                float4 custom2 : TEXCOORD4;
                float eyeDepth : TEXCOORD5;
                float3 positionOS : TEXCOORD6;
                UNITY_VERTEX_INPUT_INSTANCE_ID
                UNITY_VERTEX_OUTPUT_STEREO
            };

            float GetBW(float4 c)
            {
                return dot(c.rgb, float3(0.333333, 0.333333, 0.333333));
            }

            float Contrast01(float value, float contrastValue)
            {
                return saturate((value - 0.5) * contrastValue + 0.5);
            }

            float Hash21(float2 p)
            {
                p = frac(p * float2(123.34, 456.21));
                p += dot(p, p + 45.32);
                return frac(p.x * p.y);
            }

            float Noise2D(float2 p)
            {
                float2 i = floor(p);
                float2 f = frac(p);
                float a = Hash21(i);
                float b = Hash21(i + float2(1.0, 0.0));
                float c = Hash21(i + float2(0.0, 1.0));
                float d = Hash21(i + float2(1.0, 1.0));
                float2 u = f * f * (3.0 - 2.0 * f);
                return lerp(lerp(a, b, u.x), lerp(c, d, u.x), u.y);
            }

            float FBM2D(float2 p)
            {
                float value = 0.0;
                float amplitude = 0.5;
                value += Noise2D(p) * amplitude; p *= 2.03; amplitude *= 0.5;
                value += Noise2D(p) * amplitude; p *= 2.01; amplitude *= 0.5;
                value += Noise2D(p) * amplitude;
                return saturate(value / 0.875);
            }

            float2 RotateUV(float2 uv, float angleDegrees, float2 center)
            {
                float radiansValue = radians(angleDegrees);
                float s = sin(radiansValue);
                float c = cos(radiansValue);
                float2 p = uv - center;
                return center + float2(p.x * c - p.y * s, p.x * s + p.y * c);
            }

            float2 PolarUV(float2 uv, float2 center)
            {
                float2 p = uv - center;
                return float2(atan2(p.y, p.x) / 6.2831853 + 0.5, length(p) * 2.0);
            }

            float WrapAxis(float value, float mode)
            {
                if(mode < 0.5) return value;
                if(mode < 1.5) return frac(value);
                if(mode < 2.5) return saturate(value);
                return 1.0 - abs(frac(value * 0.5) * 2.0 - 1.0);
            }

            float2 WrapUV(float2 uv, float modeX, float modeY)
            {
                return float2(WrapAxis(uv.x, modeX), WrapAxis(uv.y, modeY));
            }

            float2 BuildUV(float2 uv, float4 st, float usePolar, float useRotate, float rotateAngle, float rotateSpeed, float centerX, float centerY, float timeValue)
            {
                float2 center = float2(centerX, centerY);
                if(useRotate > 0.5)
                {
                    uv = RotateUV(uv, rotateAngle + rotateSpeed * timeValue, center);
                }
                if(usePolar > 0.5)
                {
                    uv = PolarUV(uv, center);
                }
                return uv * st.xy + st.zw;
            }

            float2 ApplyFlipbook(float2 uv, float enabled, float columns, float rows, float speed, float frame, float loopEnabled, float timeValue)
            {
                if(enabled < 0.5) return uv;

                columns = max(floor(columns + 0.5), 1.0);
                rows = max(floor(rows + 0.5), 1.0);
                float total = max(columns * rows, 1.0);

                float current = frame + speed * timeValue;
                current = loopEnabled > 0.5 ? fmod(current, total) : clamp(current, 0.0, total - 1.0);
                current = floor(current);
                if(current < 0.0) current += total;

                float column = fmod(current, columns);
                float row = floor(current / columns);
                float2 local = frac(uv);
                local.x = (local.x + column) / columns;
                local.y = (local.y + (rows - 1.0 - row)) / rows;
                return local;
            }

            float2 BaseUVDeform(float2 uv, float timeValue)
            {
                if(_UseUVModule < 0.5) return uv;

                float2 center = float2(_UVCenterX, _UVCenterY);
                float2 p = uv - center;
                float radiusValue = length(p);
                float angleValue = atan2(p.y, p.x);

                if(_UVKaleidoscope > 1.0)
                {
                    float count = max(floor(_UVKaleidoscope + 0.5), 2.0);
                    float segment = 6.2831853 / count;
                    float localAngle = fmod(angleValue + 6.2831853 + segment * 0.5, segment) - segment * 0.5;
                    angleValue = abs(localAngle);
                }

                angleValue += _UVSwirlStrength * radiusValue;
                radiusValue *= max(0.0001, 1.0 + _UVRadialStrength * 0.2);
                uv = center + float2(cos(angleValue), sin(angleValue)) * radiusValue;

                float wave = sin((uv.x + uv.y) * _UVWaveScale + timeValue * _UVWaveSpeed) * _UVWaveStrength * 0.01;
                return uv + wave.xx;
            }

            float2 ModelUVSelect(float2 uv)
            {
                if(_ModelUVMode > 0.5 && _ModelUVMode < 1.5) return uv.yx;
                if(_ModelUVMode > 1.5 && _ModelUVMode < 2.5) return float2(1.0 - uv.x, uv.y);
                if(_ModelUVMode > 2.5 && _ModelUVMode < 3.5) return float2(uv.x, 1.0 - uv.y);
                if(_ModelUVMode > 3.5 && _ModelUVMode < 4.5) return 1.0 - uv;
                if(_ModelUVMode > 4.5 && _ModelUVMode < 5.5) return float2(uv.x, uv.x);
                if(_ModelUVMode > 5.5 && _ModelUVMode < 6.5) return float2(uv.y, uv.y);
                return uv;
            }

            float SourceShape(float rgb, float alpha)
            {
                if(_MainShapeSource < 0.5) return rgb;
                if(_MainShapeSource < 1.5) return alpha;
                if(_MainShapeSource < 2.5) return rgb * alpha;
                return max(rgb, alpha);
            }

            float DirectionValue(float2 uv)
            {
                float value = uv.y;
                if(_DirectionMode < 0.5) value = uv.y;
                else if(_DirectionMode < 1.5) value = 1.0 - uv.y;
                else if(_DirectionMode < 2.5) value = uv.x;
                else if(_DirectionMode < 3.5) value = 1.0 - uv.x;
                else if(_DirectionMode < 4.5) value = saturate(length(uv - 0.5) * 1.4142);
                else value = 1.0 - saturate(length(uv - 0.5) * 1.4142);
                return saturate(pow(max(value, 0.0001), max(_DirectionPower, 0.0001)));
            }

            float ApplyDissolveCurve(float value)
            {
                value = saturate((value - _DissolveDelay) / max(1.0 - _DissolveDelay, 0.001));
                if(_DissolveCurveMode > 0.5 && _DissolveCurveMode < 1.5) value = value * value * (3.0 - 2.0 * value);
                else if(_DissolveCurveMode > 1.5 && _DissolveCurveMode < 2.5) value = 1.0 - pow(1.0 - value, 2.0);
                else if(_DissolveCurveMode > 2.5 && _DissolveCurveMode < 3.5) value = value * value;
                else if(_DissolveCurveMode > 3.5) value = lerp(value * 0.35, value, smoothstep(0.2, 0.75, value));
                value = pow(max(value, 0.0001), max(_DissolveCurvePower, 0.0001));
                return saturate(lerp(value, 1.0 - pow(1.0 - value, 2.0), _DissolveEndBoost));
            }

            float2 FlowMapOffset(float2 uv, float timeValue)
            {
                if(_UseFlowMap < 0.5) return 0.0.xx;
                float2 flowUV = uv * _FlowMapTiling + _FlowMapTex_ST.zw + timeValue * _FlowMapSpeed * 0.05;
                float2 flow = SAMPLE_TEXTURE2D(_FlowMapTex, sampler_FlowMapTex, flowUV).rg * 2.0 - 1.0;
                return flow * _FlowMapStrength;
            }

            float2 ParallaxOffset(float2 uv)
            {
                if(_UseParallax < 0.5) return 0.0.xx;
                float2 parallaxUV = uv * _ParallaxTex_ST.xy + _ParallaxTex_ST.zw;
                float4 sampleValue = SAMPLE_TEXTURE2D(_ParallaxTex, sampler_ParallaxTex, parallaxUV);

                if(_ParallaxMode < 0.5)
                {
                    float height = GetBW(sampleValue);
                    float2 direction = normalize(uv - 0.5 + 0.0001);
                    return direction * (height - _ParallaxCenter) * _ParallaxStrength;
                }
                return (sampleValue.rg * 2.0 - 1.0) * _ParallaxStrength;
            }

            float2 DistortOffset(float2 uv, float timeValue, float customStrength)
            {
                if(_UseDistortModule < 0.5) return 0.0.xx;

                float2 duv = BuildUV(uv, _DistortTex_ST, _DistortUsePolar, _DistortUseRotate, _DistortRotateAngle, _DistortRotateSpeed, _DistortUVCenterX, _DistortUVCenterY, timeValue);
                if(_DistortUseFlow > 0.5) duv += float2(_DistortFlowX, _DistortFlowY) * _DistortFlowSpeed * timeValue;
                duv = WrapUV(duv, _DistortWrapModeX, _DistortWrapModeY);

                float stepValue = max(_DistortGradientStep, 0.0001);
                float dx = GetBW(SAMPLE_TEXTURE2D(_DistortTex, sampler_DistortTex, duv + float2(stepValue, 0.0))) -
                           GetBW(SAMPLE_TEXTURE2D(_DistortTex, sampler_DistortTex, duv - float2(stepValue, 0.0)));
                float dy = GetBW(SAMPLE_TEXTURE2D(_DistortTex, sampler_DistortTex, duv + float2(0.0, stepValue))) -
                           GetBW(SAMPLE_TEXTURE2D(_DistortTex, sampler_DistortTex, duv - float2(0.0, stepValue)));

                float2 distortMaskUV = BuildUV(uv, _DistortMaskTex_ST, _DistortMaskUsePolar, _DistortMaskUseRotate, _DistortMaskRotateAngle, _DistortMaskRotateSpeed, _DistortMaskUVCenterX, _DistortMaskUVCenterY, timeValue);
                if(_DistortMaskUseFlow > 0.5)
                    distortMaskUV += float2(_DistortMaskFlowX, _DistortMaskFlowY) * _DistortMaskFlowSpeed * timeValue;
                distortMaskUV = WrapUV(distortMaskUV, _DistortMaskWrapModeX, _DistortMaskWrapModeY);
                float distortMaskValue = GetBW(SAMPLE_TEXTURE2D(_DistortMaskTex, sampler_DistortMaskTex, distortMaskUV));
                distortMaskValue = lerp(distortMaskValue, 1.0 - distortMaskValue, _DistortMaskInvert);
                distortMaskValue = lerp(1.0, distortMaskValue, _DistortMaskStrength);

                float2 heat = 0.0.xx;
                if(_UseEdgeHeat > 0.5)
                {
                    float2 heatDensityXY = max(float2(_EdgeHeatScaleX, _EdgeHeatScaleY), float2(0.001, 0.001));
                    float2 heatUV = uv * heatDensityXY;
                    float h1 = FBM2D(heatUV + float2(timeValue * _EdgeHeatSpeed, -timeValue * 0.31 * _EdgeHeatSpeed));
                    float h2 = FBM2D(heatUV + float2(11.7 - timeValue * 0.43 * _EdgeHeatSpeed, 7.1 + timeValue * 0.27 * _EdgeHeatSpeed));
                    heat = (float2(h1, h2) - 0.5) * _EdgeHeatStrength * 0.035;
                }

                return (float2(dx, dy) * _DistortStrength * customStrength * 2.0 * distortMaskValue) + heat;
            }

            float RadialWave(float2 uv, float timeValue)
            {
                if(_UseRadialWave < 0.5) return 0.0;
                float radiusValue = _RadialWaveRadius + timeValue * _RadialWaveSpeed;
                radiusValue = _RadialWaveLoop > 0.5 ? frac(radiusValue) : radiusValue;
                float distanceValue = distance(uv, float2(_RadialWaveCenterX, _RadialWaveCenterY));
                float delta = abs(distanceValue - radiusValue);
                return 1.0 - smoothstep(max(_RadialWaveWidth, 0.0001), max(_RadialWaveWidth + _RadialWaveSoftness, 0.0002), delta);
            }

            float SpeedLineHoleDistance(float2 _point)
            {
                float2 p = _point / max(float2(_SpeedLineHoleScaleX, _SpeedLineHoleScaleY), 0.0001);
                if(_SpeedLineHoleShape < 0.5) return length(p);
                if(_SpeedLineHoleShape < 1.5) return max(abs(p.y), length(float2(p.x * 0.65, p.y)));
                if(_SpeedLineHoleShape < 2.5) return max(abs(p.x), length(float2(p.x, p.y * 0.65)));
                if(_SpeedLineHoleShape < 3.5) return max(abs(p.x), abs(p.y));
                return abs(p.x) + abs(p.y);
            }

            float SpeedLineMask(float2 screenUV, float timeValue, out float2 lineUV)
            {
                lineUV = screenUV;
                if(_UseSpeedLine < 0.5) return 0.0;

                float2 center = float2(_SpeedLineCenterX, _SpeedLineCenterY);
                float2 p = screenUV - center;
                float aspect = _ScreenParams.x / max(_ScreenParams.y, 0.0001);
                float2 corrected = lerp(p, float2(p.x * aspect, p.y), saturate(_SpeedLineAspectFix));
                float radiusValue = length(corrected);
                float angleValue = atan2(corrected.y, corrected.x) / 6.2831853 + 0.5;

                float outerMask = smoothstep(_SpeedLineEdgeStart, _SpeedLineEdgeStart + max(_SpeedLineEdgeSoftness, 0.0001), radiusValue);
                float holeMask = smoothstep(_SpeedLineCenterClear, _SpeedLineCenterClear + max(_SpeedLineCenterSoftness, 0.0001), SpeedLineHoleDistance(corrected));
                float maskValue = saturate(outerMask * holeMask);
                maskValue = lerp(maskValue, 1.0 - maskValue, _SpeedLineInvert);

                if(_SpeedLineMode > 0.5)
                {
                    float radial = radiusValue / max(_SpeedLineLength, 0.0001);
                    lineUV = float2(angleValue * _SpeedLineAngleTiling, radial * _SpeedLineRadialTiling);
                }

                lineUV = RotateUV(lineUV, _SpeedLineRotate + _SpeedLineRotateSpeed * timeValue, 0.5);
                lineUV = lineUV * _MainTex_ST.xy + _MainTex_ST.zw;
                lineUV += float2(_SpeedLineFlowX, _SpeedLineFlowY) * _SpeedLineSpeed * timeValue * 0.1;

                return maskValue;
            }

            float3 ApplyColorAdjust(float3 color)
            {
                if(_UseColorAdjust < 0.5) return color;

                float maxColor = max(max(color.r, color.g), color.b);
                float minColor = min(min(color.r, color.g), color.b);
                float delta = maxColor - minColor;
                float hue = 0.0;
                if(delta > 0.00001)
                {
                    if(maxColor == color.r) hue = frac((color.g - color.b) / delta / 6.0);
                    else if(maxColor == color.g) hue = (color.b - color.r) / delta / 6.0 + 1.0 / 3.0;
                    else hue = (color.r - color.g) / delta / 6.0 + 2.0 / 3.0;
                }

                float saturationValue = maxColor <= 0.00001 ? 0.0 : delta / maxColor;
                float value = maxColor;
                hue = frac(hue + _HueShift);
                saturationValue *= _Saturation;
                value *= _Value;

                float3 k = frac(hue.xxx + float3(0.0, 2.0 / 3.0, 1.0 / 3.0));
                float3 rgb = value * lerp(1.0, saturate(abs(k * 6.0 - 3.0) - 1.0), saturationValue);
                rgb = (rgb - 0.5) * _ColorContrast + 0.5;
                rgb = lerp(rgb, 1.0 - rgb, _InvertColor);

                if(_UseBlackWhiteFlash > 0.5)
                {
                    float gray = GetBW(float4(rgb, 1.0));
                    rgb = lerp(rgb, gray.xxx, _BlackWhiteFlashValue);
                }

                if(_OverExposureClamp > 0.001)
                    rgb = min(rgb, _OverExposureClamp.xxx);
                return max(rgb, 0.0);
            }

            float4 SampleMainBlur(float2 uv)
            {
                float4 center = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv);
                if(_MainUseBlur < 0.5 || _MainBlurStrength <= 0.001) return center;

                float2 pixel = _MainTex_TexelSize.xy * _MainBlurStrength;

                if(_MainBlurMode < 0.5)
                {
                    float4 sum = 0.0;
                    sum += SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv + pixel * float2(-1,-1));
                    sum += SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv + pixel * float2( 0,-1));
                    sum += SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv + pixel * float2( 1,-1));
                    sum += SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv + pixel * float2(-1, 0));
                    sum += center;
                    sum += SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv + pixel * float2( 1, 0));
                    sum += SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv + pixel * float2(-1, 1));
                    sum += SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv + pixel * float2( 0, 1));
                    sum += SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv + pixel * float2( 1, 1));
                    return sum / 9.0;
                }

                if(_MainBlurMode < 1.5)
                {
                    float radiansValue = radians(_MainMotionBlurAngle);
                    float2 direction = normalize(float2(cos(radiansValue), sin(radiansValue)) + 0.0001) * max(pixel.x, pixel.y);
                    float4 sum = center * 2.0;
                    sum += SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv + direction);
                    sum += SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv - direction);
                    sum += SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv + direction * 2.0);
                    sum += SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv - direction * 2.0);
                    sum += SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv + direction * 3.0);
                    sum += SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv - direction * 3.0);
                    return sum / 8.0;
                }

                if(_MainBlurMode < 2.5)
                {
                    float4 sum = center * 4.0;
                    sum += SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv + float2(pixel.x, 0.0)) * 2.0;
                    sum += SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv + float2(-pixel.x, 0.0)) * 2.0;
                    sum += SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv + float2(0.0, pixel.y)) * 2.0;
                    sum += SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv + float2(0.0, -pixel.y)) * 2.0;
                    sum += SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv + pixel * float2(1,1));
                    sum += SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv + pixel * float2(-1,1));
                    sum += SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv + pixel * float2(1,-1));
                    sum += SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv + pixel * float2(-1,-1));
                    return sum / 16.0;
                }

                float2 direction = normalize(uv - float2(_MainUVCenterX, _MainUVCenterY) + 0.0001) * max(pixel.x, pixel.y);
                float4 radialSum = center * 2.0;
                radialSum += SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv + direction);
                radialSum += SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv - direction);
                radialSum += SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv + direction * 2.0);
                radialSum += SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv - direction * 2.0);
                return radialSum / 6.0;
            }

            Varyings Vert(Attributes input)
            {
                Varyings output = (Varyings)0;
                UNITY_SETUP_INSTANCE_ID(input);
                UNITY_TRANSFER_INSTANCE_ID(input, output);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);

                float timeValue = _Time.y;
                float3 positionOS = input.positionOS.xyz;
                float3 normalOS = normalize(input.normalOS);
                float vertexCustom = lerp(1.0, input.texcoord2.w, _UseCDVertex);

                if(_UseFresnelModule > 0.5 && _FresnelInvert < 0.5 && _FresnelOutlineExpand > 0.0001)
                {
                    positionOS += normalOS * _FresnelOutlineExpand;
                }

                if(_UseVertexModule > 0.5)
                {
                    float strength = _VertexStrength * vertexCustom;

                    float2 vertexOffsetMaskUV = input.texcoord.xy;
                    if(_VertexOffsetMaskUseFlow > 0.5)
                        vertexOffsetMaskUV += float2(_VertexOffsetMaskFlowX, _VertexOffsetMaskFlowY) * _VertexOffsetMaskFlowSpeed * timeValue;
                    vertexOffsetMaskUV = BuildUV(vertexOffsetMaskUV, _VertexOffsetMaskTex_ST, _VertexOffsetMaskUsePolar, _VertexOffsetMaskUseRotate, _VertexOffsetMaskRotateAngle, _VertexOffsetMaskRotateSpeed, _VertexOffsetMaskUVCenterX, _VertexOffsetMaskUVCenterY, timeValue);
                    vertexOffsetMaskUV = WrapUV(vertexOffsetMaskUV, _VertexOffsetMaskWrapModeX, _VertexOffsetMaskWrapModeY);
                    float vertexOffsetMask = GetBW(SAMPLE_TEXTURE2D_LOD(_VertexOffsetMaskTex, sampler_VertexOffsetMaskTex, vertexOffsetMaskUV, 0));
                    strength *= saturate(vertexOffsetMask);

                    if(_VertexUseAnimMode < 0.5)
                    {
                        float2 vertexUV = input.texcoord.xy;
                        if(_VertexUseFlow > 0.5)
                            vertexUV += float2(_VertexFlowX, _VertexFlowY) * _VertexFlowSpeed * timeValue;

                        vertexUV = BuildUV(vertexUV, _VertexTex_ST, _VertexUsePolar, _VertexUseRotate, _VertexRotateAngle, _VertexRotateSpeed, _VertexUVCenterX, _VertexUVCenterY, timeValue);
                        vertexUV = WrapUV(vertexUV, _VertexWrapModeX, _VertexWrapModeY);

                        float vertexMask = GetBW(SAMPLE_TEXTURE2D_LOD(_VertexTex, sampler_VertexTex, vertexUV, 0));
                        vertexMask = pow(max(Contrast01(vertexMask, _VertexTexContrast), 0.0001), max(_VertexTexPower, 0.0001));

                        // AruiShader1.0：控制图黑色=0偏移，白色=完整偏移。
                        float vertexControl = saturate((vertexMask - _VertexTexOffset) / max(1.0 - _VertexTexOffset, 0.0001));
                        positionOS += normalOS * vertexControl * strength;
                    }
                    else if(_VertexMode > 0.5)
                    {
                        float animTime = timeValue * _VertexSpeed;
                        float3 direction = normalize(float3(_VertexDirectionX, _VertexDirectionY, _VertexDirectionZ) + 0.0001);
                        float wave01 = sin(animTime) * 0.5 + 0.5;
                        float wave11 = sin(animTime);
                        float noise = FBM2D(input.texcoord.xy * _VertexScale + animTime) * 2.0 - 1.0;
                        float3 radial = normalize(positionOS + 0.0001);

                        if(_VertexMode < 1.5) positionOS += normalOS * strength * wave01;
                        else if(_VertexMode < 2.5) positionOS += direction * strength * wave11;
                        else if(_VertexMode < 3.5) positionOS += radial * strength * wave01;
                        else if(_VertexMode < 4.5) positionOS += normalOS * noise * strength;
                        else if(_VertexMode < 5.5) positionOS *= 1.0 + wave11 * strength * 0.1;
                        else positionOS += radial * (1.0 - smoothstep(0.25, 1.0, frac(animTime))) * strength;
                    }
                }

                if(_UseVAT > 0.5)
                {
                    float2 vatUV = input.texcoord.xy * _VATTex_ST.xy + _VATTex_ST.zw;
                    vatUV = ApplyFlipbook(vatUV, 1.0, _VATColumns, _VATRows, _VATSpeed, _VATFrame, _VATLoop, timeValue);
                    float3 vatOffset = SAMPLE_TEXTURE2D_LOD(_VATTex, sampler_VATTex, vatUV, 0).rgb * 2.0 - 1.0;
                    if(_VATMode < 0.5) positionOS += vatOffset * _VATStrength;
                    else positionOS += normalOS * GetBW(float4(vatOffset * 0.5 + 0.5, 1.0)) * _VATStrength;
                }

                float3 positionWS = TransformObjectToWorld(positionOS);
                output.positionWS = positionWS;
                output.normalWS = TransformObjectToWorldNormal(normalOS);
                output.positionCS = TransformWorldToHClip(positionWS);
                output.uv = input.texcoord.xy;
                output.color = input.color;
                output.custom1 = input.texcoord1;
                output.custom2 = input.texcoord2;
                output.eyeDepth = max(0.0, -TransformWorldToView(positionWS).z);
                output.positionOS = positionOS;
                return output;
            }

            half4 Frag(Varyings input) : SV_Target
            {
                UNITY_SETUP_INSTANCE_ID(input);
                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);

                float timeValue = _Time.y;
                float2 uv = BaseUVDeform(ModelUVSelect(input.uv), timeValue);
                float2 flowOffset = FlowMapOffset(uv, timeValue);
                float2 parallaxOffset = ParallaxOffset(uv);
                float radialWave = RadialWave(uv, timeValue);
                float2 radialDirection = normalize(uv - float2(_RadialWaveCenterX, _RadialWaveCenterY) + 0.0001);
                float2 radialOffset = radialDirection * radialWave * _RadialWaveDistortStrength;
                float2 distortion = DistortOffset(uv, timeValue, lerp(1.0, input.custom1.z, _UseCDDistort)) + radialOffset;
                // DistortJaggedAutoFix：自动限制流动扭曲的UV尖峰，无需额外开关。
                float automaticMaxOffset = lerp(0.055, 0.16, saturate(_DistortStrength * 0.5));
                float distortionLength = length(distortion);
                float distortionScale = min(1.0, automaticMaxOffset / max(distortionLength, 0.0001));
                distortion *= distortionScale;

                float2 mainUVUndistorted = BuildUV(
                    uv + flowOffset * _FlowMapAffectMain + parallaxOffset * _ParallaxAffectMain,
                    _MainTex_ST, _MainUsePolar, _MainUseRotate, _MainRotateAngle, _MainRotateSpeed,
                    _MainUVCenterX, _MainUVCenterY, timeValue
                );
                if(_MainUseFlow > 0.5)
                    mainUVUndistorted += float2(_MainFlowX, _MainFlowY) * _MainFlowSpeed * timeValue;
                mainUVUndistorted += input.custom1.xy * _UseCDMainOffset;
                mainUVUndistorted = WrapUV(mainUVUndistorted, _MainWrapModeX, _MainWrapModeY);

                float2 mainUVBeforeFlipbook = WrapUV(mainUVUndistorted + distortion * _DistortAffectMain, _MainWrapModeX, _MainWrapModeY);
                float2 mainUV = ApplyFlipbook(mainUVBeforeFlipbook, _MainUseFlipbook, _MainFlipbookColumns, _MainFlipbookRows, _MainFlipbookSpeed, _MainFlipbookFrame, _MainFlipbookLoop, timeValue);
                float2 mainUVGuard = ApplyFlipbook(mainUVUndistorted, _MainUseFlipbook, _MainFlipbookColumns, _MainFlipbookRows, _MainFlipbookSpeed, _MainFlipbookFrame, _MainFlipbookLoop, timeValue);

                float4 mainSample = SampleMainBlur(mainUV);
                // 自动使用未扭曲轮廓保护扭曲后的透明边缘。
                if(_UseDistortModule > 0.5 && _DistortAffectMain > 0.0001)
                {
                    float4 guardSample = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, mainUVGuard);
                    float guardShape = SourceShape(GetBW(guardSample), guardSample.a);
                    float guardAA = max(0.035, fwidth(guardShape) * 1.75);
                    float guardMask = smoothstep(0.012, 0.012 + guardAA, guardShape);
                    mainSample.rgb *= guardMask;
                    mainSample.a *= guardMask;
                }
                if(_MainUseHueSplit > 0.5 && _MainHueSplitAmount > 0.00001)
                {
                    float radiansValue = radians(_MainHueSplitAngle + _MainHueSplitSpeed * timeValue);
                    float2 split = float2(cos(radiansValue), sin(radiansValue)) * _MainHueSplitAmount;
                    float2 mainUR = ApplyFlipbook(WrapUV(mainUVBeforeFlipbook + split, _MainWrapModeX, _MainWrapModeY), _MainUseFlipbook, _MainFlipbookColumns, _MainFlipbookRows, _MainFlipbookSpeed, _MainFlipbookFrame, _MainFlipbookLoop, timeValue);
                    float2 mainUB = ApplyFlipbook(WrapUV(mainUVBeforeFlipbook - split, _MainWrapModeX, _MainWrapModeY), _MainUseFlipbook, _MainFlipbookColumns, _MainFlipbookRows, _MainFlipbookSpeed, _MainFlipbookFrame, _MainFlipbookLoop, timeValue);
                    mainSample.r = SampleMainBlur(mainUR).r;
                    mainSample.b = SampleMainBlur(mainUB).b;
                }

                float mainAlpha = saturate(pow(saturate(mainSample.a), max(_AlphaPower, 0.0001)) * _MainAlphaStrength);
                float mainShape = SourceShape(GetBW(mainSample), mainAlpha);
                mainShape = pow(max(Contrast01(mainShape, _MainContrast), 0.0001), max(_MainPower, 0.0001));
                float shape = lerp(1.0, mainShape, _UseMainLayer);

                float3 viewDirection = normalize(_WorldSpaceCameraPos - input.positionWS);
                float normalView = dot(normalize(input.normalWS), viewDirection);
                float backMask = 1.0 - smoothstep(-max(_BackFaceColorSoftness, 0.0001), max(_BackFaceColorSoftness, 0.0001), normalView);

                // StablePaletteAlphaFix：色板透明度与色板RGB采用同一套混合规则。
                float3 color = _SingleColor.rgb;
                float paletteAlpha = saturate(_SingleColor.a);
                if(_UseThreeColor > 0.5)
                {
                    float middle = smoothstep(_ColorSplit1 - _ColorSoftness, _ColorSplit1 + _ColorSoftness, shape);
                    float hot = smoothstep(_ColorSplit2 - _ColorSoftness, _ColorSplit2 + _ColorSoftness, shape);
                    color = lerp(_ColorA.rgb, _ColorB.rgb, middle);
                    color = lerp(color, _ColorC.rgb, hot);
                    paletteAlpha = lerp(_ColorA.a, _ColorB.a, middle);
                    paletteAlpha = lerp(paletteAlpha, _ColorC.a, hot);
                    paletteAlpha = saturate(paletteAlpha);
                }
                else
                {
                    float backColorMix = saturate(backMask * _UseBackFaceColor);
                    color = lerp(color, _BackFaceColor.rgb, backColorMix);
                    paletteAlpha = saturate(lerp(paletteAlpha, _BackFaceColor.a, backColorMix));
                }
                color = lerp(color, color * mainSample.rgb, _MainColorMix);

                float3 subColorAdd = 0.0;
                float subShape = 0.0;
                if(_UseSubLayer > 0.5)
                {
                    float2 subUV = BuildUV(
                        uv + flowOffset * _FlowMapAffectSub + parallaxOffset * _ParallaxAffectSub + distortion,
                        _SubTex_ST, _SubUsePolar, _SubUseRotate, _SubRotateAngle, _SubRotateSpeed,
                        _SubUVCenterX, _SubUVCenterY, timeValue
                    );
                    if(_SubUseFlow > 0.5)
                        subUV += float2(_SubFlowX, _SubFlowY) * _SubFlowSpeed * timeValue;
                    subUV = WrapUV(subUV, _SubWrapModeX, _SubWrapModeY);

                    float4 subSample = SAMPLE_TEXTURE2D(_SubTex, sampler_SubTex, subUV);
                    float subBW = pow(max(Contrast01(GetBW(subSample), _SubContrast), 0.0001), max(_SubPower, 0.0001)) * _SubStrength;
                    subShape = subBW;

                    float subAlphaMask = lerp(subSample.a, saturate(max(max(subSample.r, subSample.g), subSample.b)) * subSample.a, _SubRemoveBlack);
                    subColorAdd = subSample.rgb * _SubColor.rgb * _SubColorStrength * (1.0 + _SubColorIntensity) * subAlphaMask;

                    if(_SubBlendMode < 0.5) shape = saturate(shape + subBW);
                    else if(_SubBlendMode < 1.5) shape = saturate(shape * subBW);
                    else if(_SubBlendMode < 2.5) shape = lerp(shape, subBW, saturate(subBW));
                    else shape = max(shape, subBW);
                }

                float maskValue = 1.0;
                float auxMaskValue = 1.0;
                float maskAffectColor = 1.0;
                if(_UseMaskLayer > 0.5)
                {
                    float2 maskUV = BuildUV(
                        uv + parallaxOffset * _ParallaxAffectMask + distortion * _DistortAffectMask,
                        _MaskTex_ST, _MaskUsePolar, _MaskUseRotate, _MaskRotateAngle, _MaskRotateSpeed,
                        _MaskUVCenterX, _MaskUVCenterY, timeValue
                    );
                    if(_MaskUseFlow > 0.5)
                        maskUV += float2(_MaskFlowX, _MaskFlowY) * _MaskFlowSpeed * timeValue;
                    maskUV += input.custom2.xy * _UseCDMaskOffset;
                    maskUV = WrapUV(maskUV, _MaskWrapModeX, _MaskWrapModeY);

                    maskValue = pow(max(Contrast01(GetBW(SAMPLE_TEXTURE2D(_MaskTex, sampler_MaskTex, maskUV)), _MaskContrast), 0.0001), max(_MaskPower, 0.0001));
                    maskValue = lerp(maskValue, 1.0 - maskValue, _MaskInvert);
                    maskAffectColor = lerp(1.0, maskValue, _MaskAffectColor);
                }

                if(_UseAuxMaskLayer > 0.5)
                {
                    float2 auxUV = BuildUV(
                        uv, _AuxMaskTex_ST, _AuxMaskUsePolar, _AuxMaskUseRotate, _AuxMaskRotateAngle, _AuxMaskRotateSpeed,
                        _AuxMaskUVCenterX, _AuxMaskUVCenterY, timeValue
                    );
                    if(_AuxMaskUseFlow > 0.5)
                        auxUV += float2(_AuxMaskFlowX, _AuxMaskFlowY) * _AuxMaskFlowSpeed * timeValue;
                    auxUV = WrapUV(auxUV, _AuxMaskWrapModeX, _AuxMaskWrapModeY);

                    float4 auxSample = SAMPLE_TEXTURE2D(_AuxMaskTex, sampler_AuxMaskTex, auxUV);
                    float auxChannel = GetBW(auxSample);
                    if(_AuxMaskChannel > 0.5 && _AuxMaskChannel < 1.5) auxChannel = auxSample.r;
                    else if(_AuxMaskChannel > 1.5 && _AuxMaskChannel < 2.5) auxChannel = auxSample.g;
                    else if(_AuxMaskChannel > 2.5 && _AuxMaskChannel < 3.5) auxChannel = auxSample.b;
                    else if(_AuxMaskChannel > 3.5 && _AuxMaskChannel < 4.5) auxChannel = auxSample.a;
                    else if(_AuxMaskChannel > 4.5) auxChannel = max(max(auxSample.r, auxSample.g), auxSample.b);

                    auxMaskValue = pow(max(Contrast01(auxChannel, _AuxMaskContrast), 0.0001), max(_AuxMaskPower, 0.0001));
                    auxMaskValue = lerp(auxMaskValue, 1.0 - auxMaskValue, _AuxMaskInvert);
                }

                float dissolveNoise = 1.0;
                float dissolveVisible = 1.0;
                float dissolveEdge = 0.0;
                float dissolveOuterEdge = 0.0;
                if(_UseDissolveModule > 0.5)
                {
                    float2 dissolveUV = BuildUV(
                        uv + flowOffset * _FlowMapAffectDissolve + parallaxOffset * _ParallaxAffectDissolve + distortion * _DistortAffectDissolve,
                        _DissolveTex_ST, _DissolveUsePolar, _DissolveUseRotate, _DissolveRotateAngle, _DissolveRotateSpeed,
                        _DissolveUVCenterX, _DissolveUVCenterY, timeValue
                    );
                    if(_DissolveUseFlow > 0.5)
                        dissolveUV += float2(_DissolveFlowX, _DissolveFlowY) * _DissolveFlowSpeed * timeValue;
                    if(_UseCDDissolveOffset > 0.5)
                    {
                        dissolveUV += _CDDissolveOffsetAxis < 0.5 ? float2(input.custom1.w, 0.0) : float2(0.0, input.custom1.w);
                    }
                    dissolveUV = WrapUV(dissolveUV, _DissolveWrapModeX, _DissolveWrapModeY);

                    dissolveNoise = Contrast01(GetBW(SAMPLE_TEXTURE2D(_DissolveTex, sampler_DissolveTex, dissolveUV)), _DissolveContrast);
                    float dissolveAmount = lerp(_DissolveAmount, input.custom2.z, _UseCDDissolve);
                    dissolveAmount = ApplyDissolveCurve(dissolveAmount);

                    float2 dissolveExtraUV = BuildUV(uv, _DissolveExtraTex_ST, _DissolveExtraUsePolar, _DissolveExtraUseRotate, _DissolveExtraRotateAngle, _DissolveExtraRotateSpeed, _DissolveExtraUVCenterX, _DissolveExtraUVCenterY, timeValue);
                    if(_DissolveExtraUseFlow > 0.5)
                        dissolveExtraUV += float2(_DissolveExtraFlowX, _DissolveExtraFlowY) * _DissolveExtraFlowSpeed * timeValue;
                    dissolveExtraUV = WrapUV(dissolveExtraUV, _DissolveExtraWrapModeX, _DissolveExtraWrapModeY);
                    float dissolveExtraValue = GetBW(SAMPLE_TEXTURE2D(_DissolveExtraTex, sampler_DissolveExtraTex, dissolveExtraUV));
                    dissolveExtraValue = lerp(dissolveExtraValue, 1.0 - dissolveExtraValue, _DissolveExtraInvert);

                    float dissolveExtraDirectionMode = step(0.5, _DissolveExtraMode) * (1.0 - step(1.5, _DissolveExtraMode));
                    float dissolveExtraMaskMode = step(1.5, _DissolveExtraMode);
                    dissolveNoise = lerp(dissolveNoise, lerp(dissolveNoise, dissolveExtraValue, _DissolveExtraStrength), dissolveExtraDirectionMode);
                    float dissolveAreaMask = lerp(1.0, dissolveExtraValue, _DissolveExtraStrength);
                    float effectiveDissolveAmount = lerp(dissolveAmount, dissolveAmount * dissolveAreaMask, dissolveExtraMaskMode);

                    if(_UseDirectionDissolve > 0.5)
                    {
                        float direction = DirectionValue(uv);
                        dissolveNoise = lerp(dissolveNoise, direction, _DirectionNoiseBlend * _DirectionStrength);
                    }

                    if(_DissolveHardEdge > 0.5)
                    {
                        dissolveVisible = step(effectiveDissolveAmount, dissolveNoise);
                    }
                    else
                    {
                        float legacySoftVisible = smoothstep(
                            effectiveDissolveAmount - _DissolveSoftness,
                            effectiveDissolveAmount + _DissolveSoftness,
                            dissolveNoise
                        );
                        float solidSoftVisible = smoothstep(
                            effectiveDissolveAmount,
                            effectiveDissolveAmount + _DissolveSoftness,
                            dissolveNoise
                        );
                        dissolveVisible = lerp(legacySoftVisible, solidSoftVisible, saturate(_DissolveSoftPreserveSolid));
                    }

                    dissolveEdge = saturate(
                        smoothstep(effectiveDissolveAmount, effectiveDissolveAmount + max(_DissolveEdgeWidth, 0.0001), dissolveNoise) -
                        smoothstep(effectiveDissolveAmount + _DissolveEdgeWidth, effectiveDissolveAmount + _DissolveEdgeWidth + max(_DissolveSoftness, 0.0001), dissolveNoise)
                    );

                    dissolveOuterEdge = saturate(
                        smoothstep(effectiveDissolveAmount - _DissolveOuterEdgeWidth, effectiveDissolveAmount, dissolveNoise) -
                        smoothstep(effectiveDissolveAmount, effectiveDissolveAmount + max(_DissolveSoftness, 0.0001), dissolveNoise)
                    );

                    if(dissolveAmount <= 0.001)
                    {
                        dissolveVisible = 1.0;
                        dissolveEdge = 0.0;
                        dissolveOuterEdge = 0.0;
                    }
                    else if(dissolveAmount >= 0.999)
                    {
                        dissolveVisible = lerp(0.0, dissolveVisible * (1.0 - step(0.999, dissolveAreaMask)), dissolveExtraMaskMode);
                        dissolveEdge *= dissolveExtraMaskMode;
                        dissolveOuterEdge *= dissolveExtraMaskMode;
                    }

                    if(_UseLayerDissolve > 0.5)
                    {
                        float layerHigh = 1.0 - smoothstep(_Layer1End - _LayerDissolveSoftness, _Layer1End + _LayerDissolveSoftness, dissolveNoise);
                        float layerDark = smoothstep(_Layer3Start - _LayerDissolveSoftness, _Layer3Start + _LayerDissolveSoftness, dissolveNoise);
                        color = lerp(color, _ColorC.rgb, layerHigh);
                        color = lerp(color, _ColorA.rgb, layerDark);
                    }
                }

                radialWave = RadialWave(uv, timeValue);
                float edge = 0.0;
                if(_UseEdgeModule > 0.5)
                {
                    edge = saturate(1.0 - smoothstep(_EdgeWidth, _EdgeWidth + max(_EdgeSoftness, 0.0001), shape));
                    edge *= dissolveVisible;
                }

                float fresnel = 0.0;
                float fresnelSecond = 0.0;
                float fresnelInnerMask = 0.0;
                float fresnelShellMask = 1.0;
                // 溶解裁切菲尼尔：控制普通菲尼尔、双层菲尼尔、内圈和纯外壳。
                float fresnelDissolveMask = lerp(1.0, dissolveVisible, saturate(_DissolveAffectFresnel * _UseDissolveModule));
                if(_UseFresnelModule > 0.5)
                {
                    float fresnelBase = 1.0 - saturate(abs(normalView));
                    if(_FresnelUseNoise > 0.5)
                    {
                        fresnelBase = saturate(fresnelBase + (FBM2D(uv * _FresnelNoiseScale + timeValue * _FresnelNoiseSpeed) - 0.5) * _FresnelNoiseStrength);
                    }

                    if(_FresnelInvert > 0.5)
                    {
                        fresnelBase = 1.0 - fresnelBase;
                    }

                    float threshold = saturate(1.0 - _FresnelThickness);
                    float band = smoothstep(threshold, 1.0, fresnelBase);
                    fresnel = pow(max(band + _FresnelBias, 0.0001), max(_FresnelPower, 0.0001)) * _FresnelIntensity * _FresnelBrightness;

                    if(_FresnelInvert < 0.5)
                    {
                        float innerStart = max(0.0, threshold - _FresnelInnerWidth);
                        fresnelInnerMask = smoothstep(innerStart, threshold, fresnelBase) *
                                           (1.0 - smoothstep(threshold, threshold + 0.03, fresnelBase)) *
                                           _UseFresnelInnerColor;
                    }

                    if(_UseSecondFresnel > 0.5)
                    {
                        float secondThreshold = saturate(1.0 - _FresnelSecondThickness);
                        float secondBand = smoothstep(secondThreshold, 1.0, fresnelBase);
                        fresnelSecond = pow(max(secondBand, 0.0001), max(_FresnelSecondPower, 0.0001)) *
                                        _FresnelSecondIntensity * _FresnelSecondBrightness;
                    }

                    if(_FresnelForceHollow > 0.5 && _FresnelInvert < 0.5)
                    {
                        fresnelShellMask = saturate(band);
                    }

                    // 保留旧参数作为“噪波强度扰动”，与真正的溶解裁切分离。
                    if(_DissolveDistortAffectFresnel > 0.5)
                    {
                        float dissolveInfluence = lerp(0.45, 1.45, dissolveNoise);
                        fresnel *= dissolveInfluence;
                        fresnelSecond *= dissolveInfluence;
                    }

                    // FresnelDissolveFix：所有菲尼尔输出统一受溶解可见区域裁切。
                    fresnel *= fresnelDissolveMask;
                    fresnelSecond *= fresnelDissolveMask;
                    fresnelInnerMask *= fresnelDissolveMask;
                    fresnelShellMask *= fresnelDissolveMask;
                }

                float2 screenUV = GetNormalizedScreenSpaceUV(input.positionCS);
                float2 speedLineUV;
                float speedLine = SpeedLineMask(screenUV + distortion * _SpeedLineAffectDistort, timeValue, speedLineUV);
                float4 speedLineTex = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, speedLineUV);

                float3 speedLineColor = _SpeedLineColor.rgb;
                if(_SpeedLineColor.a <= 0.0001)
                {
                    speedLineColor = speedLineTex.rgb;
                }
                if(_UseThreeColor > 0.5)
                {
                    float speedLum = GetBW(speedLineTex);
                    float mid = smoothstep(_ColorSplit1 - _ColorSoftness, _ColorSplit1 + _ColorSoftness, speedLum);
                    float hot = smoothstep(_ColorSplit2 - _ColorSoftness, _ColorSplit2 + _ColorSoftness, speedLum);
                    speedLineColor = lerp(_ColorA.rgb, _ColorB.rgb, mid);
                    speedLineColor = lerp(speedLineColor, _ColorC.rgb, hot);
                }

                if(_UseRampModule > 0.5)
                {
                    float2 rampUV = BuildUV(float2(saturate(shape + _RampOffset), 0.5), _RampTex_ST, _RampUsePolar, _RampUseRotate, _RampRotateAngle, _RampRotateSpeed, _RampUVCenterX, _RampUVCenterY, timeValue);
                    if(_RampUseFlow > 0.5)
                        rampUV += float2(_RampFlowX, _RampFlowY) * _RampFlowSpeed * timeValue;
                    rampUV = WrapUV(rampUV, _RampWrapModeX, _RampWrapModeY);

                    float3 rampColor = SAMPLE_TEXTURE2D(_RampTex, sampler_RampTex, rampUV).rgb;
                    if(_RampBlendMode < 0.5) color = lerp(color, rampColor, _RampStrength);
                    else if(_RampBlendMode < 1.5) color = lerp(color, color + rampColor, _RampStrength);
                    else if(_RampBlendMode < 2.5) color = lerp(color, color * rampColor, _RampStrength);
                    else color = lerp(color, max(color, rampColor), _RampStrength);
                }

                color += subColorAdd;
                color *= maskAffectColor;
                color *= lerp(1.0, auxMaskValue, _AuxMaskAffectColor * _UseAuxMaskLayer);
                color += _RadialWaveColor.rgb * radialWave * _RadialWaveIntensity * _UseRadialWave;

                float3 edgeColor = _EdgeOnlyColor.rgb;
                if(_UseEdgeRamp > 0.5)
                {
                    float2 edgeRampUV = BuildUV(float2(saturate(edge), 0.5), _EdgeRampTex_ST, _EdgeRampUsePolar, _EdgeRampUseRotate, _EdgeRampRotateAngle, _EdgeRampRotateSpeed, _EdgeRampUVCenterX, _EdgeRampUVCenterY, timeValue);
                    edgeRampUV = WrapUV(edgeRampUV, _EdgeRampWrapModeX, _EdgeRampWrapModeY);
                    edgeColor = lerp(edgeColor, SAMPLE_TEXTURE2D(_EdgeRampTex, sampler_EdgeRampTex, edgeRampUV).rgb, _EdgeRampStrength);
                }

                float3 fresnelColor = _FresnelColor.rgb;
                if(_UseFresnelRamp > 0.5)
                {
                    float2 fresnelRampUV = BuildUV(float2(saturate(fresnel + fresnelSecond), 0.5), _FresnelRampTex_ST, _FresnelRampUsePolar, _FresnelRampUseRotate, _FresnelRampRotateAngle, _FresnelRampRotateSpeed, _FresnelRampUVCenterX, _FresnelRampUVCenterY, timeValue);
                    fresnelRampUV = WrapUV(fresnelRampUV, _FresnelRampWrapModeX, _FresnelRampWrapModeY);
                    fresnelColor = lerp(fresnelColor, SAMPLE_TEXTURE2D(_FresnelRampTex, sampler_FresnelRampTex, fresnelRampUV).rgb, _FresnelRampStrength);
                }

                fresnelColor = lerp(fresnelColor, _FresnelInnerColor.rgb * _FresnelInnerIntensity, saturate(fresnelInnerMask));
                color += fresnelColor * fresnel;
                color += _FresnelSecondColor.rgb * fresnelSecond;

                float alphaMask = dissolveVisible;
                alphaMask *= lerp(1.0, maskValue, _MaskAffectAlpha * _UseMaskLayer);
                alphaMask *= lerp(1.0, auxMaskValue, _AuxMaskAffectAlpha * _UseAuxMaskLayer);
                alphaMask *= lerp(1.0, radialWave, _RadialWaveAffectAlpha * _UseRadialWave);

                float particleAlpha = lerp(1.0, input.color.a, saturate(_UseParticleAlpha * _ParticleAlphaStrength));
                float3 particleColor = lerp(1.0, input.color.rgb, saturate(_UseParticleColor * _ParticleColorStrength));

                float alpha = saturate(shape * alphaMask);
                alpha += edge * _EdgeAffectAlpha;
                alpha += dissolveEdge * _DissolveEdgeAffectAlpha * _DissolveEdgeColor.a;
                alpha += dissolveOuterEdge * _DissolveEdgeAffectAlpha * _DissolveOuterEdgeColor.a;
                alpha += fresnel * _FresnelAffectAlpha + fresnelSecond * _FresnelSecondAffectAlpha;
                alpha = saturate(alpha * _Opacity * particleAlpha * paletteAlpha);

                if(_UseFresnelModule > 0.5 && _FresnelForceHollow > 0.5 && _FresnelInvert < 0.5)
                {
                    alpha = saturate(fresnelShellMask * _Opacity * particleAlpha * paletteAlpha);
                    color = fresnelColor * fresnel + _FresnelSecondColor.rgb * fresnelSecond;
                }

                if(_UseSpeedLine > 0.5)
                {
                    color = speedLineColor * speedLine * _SpeedLineIntensity;
                    alpha = saturate(speedLine * _SpeedLineAlpha * _Opacity * particleAlpha * paletteAlpha);
                }

                if(_UseCameraDistanceFade > 0.5)
                {
                    float nearFade = _CameraNearFadeEnd <= _CameraNearFadeStart ? 1.0 :
                        saturate((distance(_WorldSpaceCameraPos, input.positionWS) - _CameraNearFadeStart) / max(_CameraNearFadeEnd - _CameraNearFadeStart, 0.0001));
                    float farFade = _CameraFarFadeEnd <= _CameraFarFadeStart ? 1.0 :
                        saturate(1.0 - (distance(_WorldSpaceCameraPos, input.positionWS) - _CameraFarFadeStart) / max(_CameraFarFadeEnd - _CameraFarFadeStart, 0.0001));
                    alpha *= nearFade * farFade;
                }

                float depthEdge = 0.0;
                if(_UseSoftParticles > 0.5)
                {
                    float sceneRawDepth = SampleSceneDepth(screenUV);
                    float sceneDepth = LinearEyeDepth(sceneRawDepth, _ZBufferParams);
                    float depthDifference = max(sceneDepth - input.eyeDepth, 0.0);
                    float depth01 = saturate(depthDifference / max(_SoftParticleDistance, 0.0001));
                    float contactMask = 1.0 - smoothstep(0.0, max(_SoftParticleSoftness, 0.0001), depth01);

                    if(_SoftParticleMode < 0.5)
                    {
                        alpha *= depth01;
                    }
                    else if(_SoftParticleMode < 1.5)
                    {
                        alpha *= contactMask;
                    }
                    else
                    {
                        depthEdge = contactMask * _DepthEdgeIntensity;
                        alpha = lerp(alpha, max(alpha, contactMask * _DepthEdgeColor.a), _DepthEdgeAffectAlpha);
                    }
                }

                color += edgeColor * edge * _EdgeBrightness * _EdgeIntensity;
                color = lerp(color, _DissolveOuterEdgeColor.rgb, saturate(dissolveOuterEdge * _DissolveOuterEdgeIntensity * _DissolveOuterEdgeColor.a));
                color = lerp(color, _DissolveEdgeColor.rgb, saturate(dissolveEdge * _DissolveEdgeIntensity * _DissolveEdgeColor.a));
                color += _DepthEdgeColor.rgb * depthEdge * _DepthEdgeAffectColor;
                color *= (1.0 + _EmissionIntensity) * particleColor;
                color = ApplyColorAdjust(color);

                if(_DebugMode > 0.5 && _DebugMode < 1.5) return half4(mainSample.rgb, 1.0);
                if(_DebugMode > 1.5 && _DebugMode < 2.5) return half4(mainAlpha.xxx, 1.0);
                if(_DebugMode > 2.5 && _DebugMode < 3.5) return half4(mainShape.xxx, 1.0);
                if(_DebugMode > 3.5 && _DebugMode < 4.5) return half4(subShape.xxx, 1.0);
                if(_DebugMode > 4.5 && _DebugMode < 5.5) return half4(maskValue.xxx, 1.0);
                if(_DebugMode > 5.5 && _DebugMode < 6.5) return half4(dissolveNoise.xxx, 1.0);
                if(_DebugMode > 6.5 && _DebugMode < 7.5) return half4(saturate(dissolveEdge + dissolveOuterEdge).xxx, 1.0);
                if(_DebugMode > 7.5 && _DebugMode < 8.5) return half4(alpha.xxx, 1.0);
                if(_DebugMode > 8.5 && _DebugMode < 9.5) return half4(edge.xxx, 1.0);
                if(_DebugMode > 9.5 && _DebugMode < 10.5) return half4(saturate(fresnel + fresnelSecond).xxx, 1.0);
                if(_DebugMode > 10.5 && _DebugMode < 11.5) return half4(saturate(length(distortion) * 10.0).xxx, 1.0);

                if(_AlphaClipEnable > 0.5 && _UseAlphaModule > 0.5)
                    clip(alpha - _AlphaClip);

                if(_OutputMode > 0.5 && _OutputMode < 1.5) return half4(alpha.xxx, 1.0);
                if(_OutputMode > 1.5 && _OutputMode < 2.5) return half4(distortion * 0.5 + 0.5, 0.0, 1.0);
                if(_OutputMode > 2.5 && _OutputMode < 3.5) return half4(maskValue.xxx, 1.0);

                return half4(color, alpha);
            }
            ENDHLSL
        }
    }

    CustomEditor "AruiShader10DualModeLiteV10URPGUI"
    Fallback Off
}
