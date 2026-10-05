using UnityEngine;
using UnityEditor;
using UnityEngine.Rendering;
using System.Collections.Generic;

public class AruiShader10DualModeFullRendererV10URP : ShaderGUI
{
    private const float TextureBoxSize = 80f; // 64的1.25倍，所有贴图槽使用正方形预览框
    private const float ScrollbarSafetyWidth = 22f; // Console ScrollView 的右侧安全边距，防止控件压到滚动条下方
    private static bool g0=false,g1=false,g2=true,g4=false,g8=false,g9=false,g10=false,g11=false,g13=false,g14=false,g15=false,g12=false;
    private static bool alphaFold=false, uvFold=false, subFold=false, colorFold=true, noiseFold=false, diagnosticFold=false, presetFold=false;

    private readonly string[] blendNames = {"Alpha透明","Add叠加","Soft Add柔和叠加","Premultiply预乘Alpha"};
    private readonly string[] cullDisplayNames = {"双面显示","显示正面","显示背面"};
    private readonly string[] outputNames = {"正常颜色","只输出Alpha","只输出扭曲","只输出遮罩"};
    private readonly string[] debugNames = {"正常显示","主贴图RGB","主贴图Alpha","主贴图形体","副贴图形体","遮罩","溶解噪波","溶解边缘","最终Alpha","边缘亮边","菲尼尔","扭曲量","顶点贴图"};
    private readonly string[] materialTypeNames = {"爆炸火焰","烟雾云团","能量球","刀光轨迹","光束激光","冲击波","地面法阵","护盾球","传送门","水波扭曲","黑白闪","自定义"};
    private readonly string[] performanceNames = {"高品质","标准","移动端","极简"};
    private readonly string[] panelModeNames = {"简洁模式","高级模式","调试模式"};
    private readonly string[] randomNames = {"全部随机优化","随机主贴图单色-全色调","随机主贴图三段色-全色调","随机溶解Tiling","随机溶解参数","随机溶解边缘颜色","随机主贴图流动","随机边缘/菲尼尔","随机扭曲/热浪","随机顶点"};
        private readonly string[] subBlendNames = {"加法","乘法","覆盖","取最大"};
    private readonly string[] rampSourceNames = {"主帖图/形体","溶解噪波","遮罩"};
    private readonly string[] rampBlendNames = {"替换","加法叠加","乘法","取最大"};
    private readonly string[] wrapNames = {"使用贴图设置","Repeat重复","Clamp夹紧","Mirror镜像"};
    private readonly string[] zTestNames = {"Disabled关闭","Never永不通过","Less小于","Equal等于","LEqual小于等于","Greater大于","NotEqual不等于","GEqual大于等于","Always总是通过"};
    private readonly int[] zTestValues = {0,1,2,3,4,5,6,7,8};
    private readonly string[] dirNames = {"下到上","上到下","左到右","右到左","中心到外","外到中心"};
    private readonly string[] curveNames = {"线性","缓入缓出","前快后慢","前慢后快","中段停留"};
    private readonly string[] vertexModeNames = {"关闭","法线膨胀","方向漂浮","径向扩散","波浪摆动","呼吸缩放","冲击波扩张"};
    private readonly string[] shapeSourceNames = {"RGB灰度","Alpha通道","RGB乘Alpha","RGB和Alpha取最大"};
    private readonly string[] modelUVModeNames = {"原始UV","交换UV","U反向","V反向","UV反向","横向方向UV","纵向方向UV"};
    private readonly string[] dissolveOffsetAxisNames = {"控制X Offset","控制Y Offset"};
    private readonly string[] auxMaskChannelNames = {"RGB灰度","R通道","G通道","B通道","A通道","RGB最大值"};
    private readonly string[] spatialGradientNames = {"UV横向U","UV纵向V","中心到外","世界Y","模型Y","模型X","模型Z"};
    private readonly string[] parallaxModeNames = {"黑白高度图","法线RG"};
    private readonly string[] speedLineModeNames = {"屏幕UV","径向速度线"};
    private readonly string[] speedLineHoleShapeNames = {"圆形/椭圆","横向胶囊","纵向胶囊","矩形","菱形"};
    private readonly string[] vatModeNames = {"RGB=XYZ偏移","黑白沿法线偏移"};
    private readonly string[] mainBlurModeNames = {"均值模糊","动感模糊","高斯模糊","径向模糊"};
    private readonly string[] depthIntersectionModeNames = {"软粒子淡出","反向软粒子","深度边缘发光"};
private readonly string[] stencilCompNames = {"Always 总是通过","Never 永不通过","Less 小于","Equal 等于","LessEqual 小于等于","Greater 大于","NotEqual 不等于","GreaterEqual 大于等于"};
    private readonly int[] stencilCompValues = {8,1,2,3,4,5,6,7};
    private readonly string[] stencilPassNames = {"Keep 保持","Zero 清零","Replace 替换","IncrSat 加1饱和","DecrSat 减1饱和","Invert 反转","IncrWrap 加1循环","DecrWrap 减1循环"};
    private readonly int[] stencilPassValues = {0,1,2,3,4,5,6,7};

    public override void OnGUI(MaterialEditor editor, MaterialProperty[] props)
    {
        DrawConsoleContent(editor, props, 0);
    }

    // moduleIndex = 0 为概览；1~10 对应完整控制台的模块导航。
    public void DrawConsoleContent(MaterialEditor editor, MaterialProperty[] props, int moduleIndex)
    {
        if(editor == null || editor.target == null) return;

        Material mat = editor.target as Material;
        if(mat == null) return;

        AruiShader10KFrameConsoleBridgeURP.Begin(mat);
        AruiShader10KFrameConsoleBridgeURP.DrawTimelineRecorderPanel();
        SyncAutomaticRenderRules(mat);
        SyncRenderState(mat);
        SyncBackFaceColorState(mat);

        switch(moduleIndex)
        {
            case 0:
                DrawConsoleOverview(mat);
                DrawDiagnostics(mat);
                break;
            case 1:
                DrawRender(editor, props, mat);
                break;
            case 2:
                DrawMaterialTypeAlphaUV(editor, props, mat);
                break;
            case 3:
                DrawMainSubColor(editor, props, mat);
                break;
            case 4:
                DrawMaskLayer(editor, props, mat);
                break;
            case 5:
                DrawDissolve(editor, props, mat);
                break;
            case 6:
                DrawDistort(editor, props, mat);
                break;
            case 7:
                DrawEdge(editor, props, mat);
                break;
            case 8:
                DrawVertex(editor, props, mat);
                break;
            case 9:
                DrawSpeedLineModule(editor, props, mat);
                break;
            case 10:
                DrawCustomDataInfo(editor, props, mat);
                break;
        }

        DrawRenderQueueBottom(editor, props, mat);
    }

    // moduleIndex = 0 为概览；1~10 对应控制台导航中的完整模块名称。
    // 独立控制台调用入口：showQuickTools=false 时，窗口使用自己的顶部栏，
    // 材质Inspector轻量模式不需要画完整面板。
    public void DrawFullPanel(MaterialEditor editor, MaterialProperty[] props, bool showQuickTools)
    {
        if(editor == null || editor.target == null) return;

        Material mat = editor.target as Material;
        if(mat == null) return;

        AruiShader10KFrameConsoleBridgeURP.Begin(mat);
        AruiShader10KFrameConsoleBridgeURP.DrawTimelineRecorderPanel();
        SyncRenderState(mat);
        SyncBackFaceColorState(mat);

        if(showQuickTools)
        {
            DrawTopBar(mat);
        }

        DrawDiagnostics(mat);
        DrawRender(editor, props, mat);
        int panelMode = Mathf.Clamp(Mathf.RoundToInt(Get(mat,"_PanelMode")), 0, 2);

        if(panelMode == 0)
        {
            DrawMaterialTypeAlphaUV(editor, props, mat);
            DrawMainSubColor(editor, props, mat);
            DrawDissolve(editor, props, mat);
            DrawDistort(editor, props, mat);
            DrawEdge(editor, props, mat);
        }
        else if(panelMode == 2)
        {
            DrawDebugQuick(mat);

            DrawMaterialTypeAlphaUV(editor, props, mat);
            DrawMainSubColor(editor, props, mat);
            DrawMaskLayer(editor, props, mat);
            DrawDissolve(editor, props, mat);
            DrawDistort(editor, props, mat);
            DrawEdge(editor, props, mat);
            DrawVertex(editor, props, mat);
            DrawSpeedLineModule(editor, props, mat);
            DrawCustomDataInfo(editor, props, mat);
        }
        else
        {
            DrawMaterialTypeAlphaUV(editor, props, mat);
            DrawMainSubColor(editor, props, mat);
            DrawMaskLayer(editor, props, mat);
            DrawDissolve(editor, props, mat);
            DrawDistort(editor, props, mat);
            DrawEdge(editor, props, mat);
            DrawVertex(editor, props, mat);
            DrawSpeedLineModule(editor, props, mat);
            DrawCustomDataInfo(editor, props, mat);
        }

        DrawRenderQueueBottom(editor, props, mat);
    }

    private void DrawConsoleOverview(Material mat)
    {
        DrawConsoleBanner(mat);

        DrawQuickSectionLabel("性能标准");
        DrawQuickPerformanceRow(mat);
        EditorGUILayout.LabelField(
            GetPerformanceDesc(Mathf.RoundToInt(Get(mat, "_PerformanceMode"))),
            EditorStyles.miniLabel
        );

        DrawQuickSectionLabel("模块展开");
        DrawThreeButtons(
            "展开全部", ()=>SetAllFolds(true),
            "折叠全部", ()=>SetAllFolds(false),
            "只展开基础", ()=>OpenBasicFolds()
        );

        DrawQuickSectionLabel("材质操作");
        DrawThreeButtons(
            "清零参数", ()=>ClearParamsKeepTextures(mat),
            "关闭功能", ()=>CloseAllFeaturesKeepMain(mat),
            "随机参数", ()=>RandomizeSelected(mat)
        );
        DrawQuickPopupRow(mat, "_RandomMode", "随机范围", randomNames);
        DrawTwoButtons(
            "清空贴图(保留主贴图)", ()=>ClearAllTexturesExceptMain(mat),
            "恢复默认值", ()=>ResetAllParamsDefault(mat)
        );

        EditorGUILayout.Space(5);
        DrawOverviewStats(mat);
    }

    private void DrawConsoleBanner(Material mat)
    {
        Rect row = EditorGUILayout.GetControlRect(false, 38f);
        EditorGUI.DrawRect(row, new Color(0.115f, 0.115f, 0.115f, 1f));

        string materialName = mat != null ? mat.name : "未选择材质";
        int moduleCount = EnabledModuleCount(mat);

        EditorGUI.LabelField(
            new Rect(row.x + 10f, row.y + 6f, row.width - 180f, 18f),
            "概览 / 当前材质：" + materialName,
            EditorStyles.boldLabel
        );
        EditorGUI.LabelField(
            new Rect(row.x + 10f, row.y + 23f, row.width - 180f, 12f),
            "已启用功能：" + moduleCount + " 个 · 使用上方导航切换到单独模块编辑",
            EditorStyles.miniLabel
        );

        Rect tagRect = new Rect(row.xMax - 112f, row.y + 9f, 102f, 20f);
        GUI.Label(tagRect, "完整控制台 V2", EditorStyles.centeredGreyMiniLabel);
    }

    private void DrawOverviewStats(Material mat)
    {
        Rect row = EditorGUILayout.GetControlRect(false, 54f);
        float gap = 4f;
        float width = (row.width - gap * 2f) / 3f;

        DrawStatCard(
            new Rect(row.x, row.y, width, row.height),
            "混合模式",
            GetBlendModeName(Mathf.RoundToInt(Get(mat, "_BlendMode")))
        );

        DrawStatCard(
            new Rect(row.x + width + gap, row.y, width, row.height),
            "显示面模式",
            GetCullModeName(Mathf.RoundToInt(Get(mat, "_CullDisplayMode")))
        );

        string queue = Mathf.RoundToInt(Get(mat, "_RenderQueueValue")).ToString();
        DrawStatCard(
            new Rect(row.x + (width + gap) * 2f, row.y, width, row.height),
            "Render Queue",
            queue
        );
    }

    private void DrawStatCard(Rect rect, string label, string value)
    {
        EditorGUI.DrawRect(rect, new Color(0.15f, 0.15f, 0.15f, 1f));
        EditorGUI.LabelField(
            new Rect(rect.x + 7f, rect.y + 6f, rect.width - 14f, 14f),
            label,
            EditorStyles.miniLabel
        );
        EditorGUI.LabelField(
            new Rect(rect.x + 7f, rect.y + 25f, rect.width - 14f, 20f),
            value,
            EditorStyles.boldLabel
        );
    }

    private string GetBlendModeName(int mode)
    {
        mode = Mathf.Clamp(mode, 0, blendNames.Length - 1);
        return blendNames[mode];
    }

    private string GetCullModeName(int mode)
    {
        mode = Mathf.Clamp(mode, 0, cullDisplayNames.Length - 1);
        return cullDisplayNames[mode];
    }

    private void DrawRenderQueueBottom(MaterialEditor e, MaterialProperty[] p, Material mat)
    {
        EditorGUILayout.Space(8);
        EditorGUILayout.LabelField("材质排序 / Render Queue（固定在最下方）", EditorStyles.boldLabel);
        DrawProp(e,p,"_RenderQueueValue");
        SyncRenderState(mat);
        EditorGUILayout.HelpBox("默认透明队列为3000。数值越大越靠后绘制，常用于解决透明特效前后排序。", MessageType.None);
    }

    private void DrawPresetTools(Material mat)
    {
        EditorGUILayout.Space(4);
        presetFold = EditorGUILayout.Foldout(presetFold, "一键颜色模板", true, new GUIStyle(EditorStyles.foldout){fontStyle = FontStyle.Bold});
        if(presetFold)
        {
            EditorGUILayout.HelpBox("颜色模板只修改颜色体系，不开启/关闭功能，不修改颜色强度。", MessageType.None);

            EditorGUILayout.Space(1);
            if(GUILayout.Button("火焰")) ApplyTemplate(mat,0);
            if(GUILayout.Button("冰霜")) ApplyTemplate(mat,1);
            if(GUILayout.Button("水")) ApplyTemplate(mat,2);
            // LayoutSafe：已移除EndHorizontal，避免EndLayoutGroup报错。

            EditorGUILayout.Space(1);
            if(GUILayout.Button("绿毒气")) ApplyTemplate(mat,3);
            if(GUILayout.Button("紫毒气")) ApplyTemplate(mat,4);
            if(GUILayout.Button("治疗")) ApplyTemplate(mat,5);
            // LayoutSafe：已移除EndHorizontal，避免EndLayoutGroup报错。

            EditorGUILayout.Space(1);
            if(GUILayout.Button("物理")) ApplyTemplate(mat,6);
            if(GUILayout.Button("圣光")) ApplyTemplate(mat,7);
            if(GUILayout.Button("血液")) ApplyTemplate(mat,8);
            // LayoutSafe：已移除EndHorizontal，避免EndLayoutGroup报错。
        }
        // LayoutSafe：已移除EndVertical，避免EndLayoutGroup报错。
    }

    private void AddDiag(ref string msg, ref int count, string text)
    {
        msg += "• " + text + "\n";
        count++;
    }

    private int EnabledModuleCount(Material m)
    {
        if(m == null) return 0;
        int c = 0;
        if(Get(m,"_UseSubLayer") > 0.5f) c++;
        if(Get(m,"_UseMaskLayer") > 0.5f) c++;
        if(Get(m,"_UseRampModule") > 0.5f) c++;
        if(Get(m,"_UseUVModule") > 0.5f) c++;
        if(Get(m,"_UseDissolveModule") > 0.5f) c++;
        if(Get(m,"_UseEdgeModule") > 0.5f) c++;
        if(Get(m,"_UseFresnelModule") > 0.5f) c++;
        if(Get(m,"_UseDistortModule") > 0.5f) c++;
        if(Get(m,"_UseVertexModule") > 0.5f) c++;
        if(Get(m,"_UseSoftParticles") > 0.5f) c++;
        if(Get(m,"_UseStencil") > 0.5f) c++;
        return c;
    }

    private void DrawDiagnostics(Material mat)
    {
        diagnosticFold = DrawCardFoldout("材质体检 / 一键修复", diagnosticFold, "检查");
        if(diagnosticFold)
        {
            EditorGUILayout.HelpBox("先检查材质问题，再调整渲染状态。默认收起，不占主界面空间。", MessageType.None);
            if(GUILayout.Button("材质体检一键自动修复", GUILayout.Height(24))) AutoRepair(mat);

            string msg = "";
            int warn = 0;

            if(Get(mat,"_UseMainLayer") > 0.5f && mat.GetTexture("_MainTex") == null)
                AddDiag(ref msg, ref warn, "主贴图为空，材质可能没有形体。");

            if(Get(mat,"_Opacity") <= 0.001f)
                AddDiag(ref msg, ref warn, "整体透明度为0，材质不可见。");

            if(Get(mat,"_UseSubLayer") > 0.5f && mat.GetTexture("_SubTex") == null)
                AddDiag(ref msg, ref warn, "副贴图层开启，但副贴图为空。");

            if(Get(mat,"_UseRampModule") > 0.5f && mat.GetTexture("_RampTex") == null)
                AddDiag(ref msg, ref warn, "Ramp开启，但Ramp贴图为空。");

            if(Get(mat,"_UseEdgeRamp") > 0.5f && mat.GetTexture("_EdgeRampTex") == null)
                AddDiag(ref msg, ref warn, "边缘Ramp开启，但边缘Ramp贴图为空。");

            if(Get(mat,"_UseFresnelRamp") > 0.5f && mat.GetTexture("_FresnelRampTex") == null)
                AddDiag(ref msg, ref warn, "菲尼尔Ramp开启，但菲尼尔Ramp贴图为空。");

            if(Get(mat,"_UseDissolveModule") > 0.5f && mat.GetTexture("_DissolveTex") == null)
                AddDiag(ref msg, ref warn, "溶解开启，但溶解贴图为空。");

            if(Get(mat,"_UseDistortModule") > 0.5f && mat.GetTexture("_DistortTex") == null)
                AddDiag(ref msg, ref warn, "扭曲/热浪开启，但扭曲贴图为空。");

            if(Get(mat,"_UseVertexModule") > 0.5f && Get(mat,"_VertexUseAnimMode") < 0.5f && mat.GetTexture("_VertexTex") == null)
                AddDiag(ref msg, ref warn, "顶点贴图模式开启，但顶点偏移黑白贴图为空。");

            if(Get(mat,"_UseStencil") > 0.5f && Get(mat,"_StencilRef") <= 0)
                AddDiag(ref msg, ref warn, "模板测试开启但模板Ref为0，通常不会得到预期遮罩。");

            if(Get(mat,"_BlendMode") < 0.5f && Get(mat,"_ZWriteToggle") > 0.5f)
                AddDiag(ref msg, ref warn, "Alpha透明模式下开启ZWrite，可能导致透明排序异常。");

            if(Get(mat,"_RenderQueueValue") < 2500 || Get(mat,"_RenderQueueValue") > 5000)
                AddDiag(ref msg, ref warn, "Render Queue建议保持在2500到5000之间。");

            if(Get(mat,"_ZTestToggle") < 0.5f)
                AddDiag(ref msg, ref warn, "ZTest深度测试未勾选，材质会无视深度，可能穿透场景显示。");

            if(Get(mat,"_UseCameraDistanceFade") > 0.5f && Get(mat,"_CameraFarFadeEnd") <= Get(mat,"_CameraFarFadeStart"))
                AddDiag(ref msg, ref warn, "摄像机远距离淡出结束值需要大于开始值，否则远距离淡出不会生效。");

            if(Get(mat,"_UseCameraDistanceFade") > 0.5f && Get(mat,"_CameraNearFadeEnd") < Get(mat,"_CameraNearFadeStart"))
                AddDiag(ref msg, ref warn, "摄像机近距离淡出结束值不能小于开始值。");
            if(Get(mat,"_UseSoftParticles") > 0.5f && Get(mat,"_SoftParticleDistance") <= 0.001f)
                AddDiag(ref msg, ref warn, "深度交界距离过小，可能看不到交界效果。");
            if(Get(mat,"_BlendMode") > 0.5f && Get(mat,"_BlendMode") < 1.5f && Get(mat,"_Opacity") < 0.05f)
                AddDiag(ref msg, ref warn, "Add叠加模式下整体透明度过低，可能看起来没有效果。");

            if(Get(mat,"_AlphaClipEnable") > 0.5f && Get(mat,"_AlphaClip") > 0.25f)
                AddDiag(ref msg, ref warn, "Alpha硬裁剪过高，可能导致边缘锯齿或形体缺失。");

            if(Get(mat,"_EmissionIntensity") > 8.0f)
                AddDiag(ref msg, ref warn, "颜色强度过高，可能导致过曝。");

            if(Get(mat,"_DistortStrength") > 1.0f)
                AddDiag(ref msg, ref warn, "扭曲强度偏高，可能导致贴图撕裂。");

            if(Mathf.Abs(Get(mat,"_VertexStrength")) > 1.5f)
                AddDiag(ref msg, ref warn, "顶点偏移强度偏高，可能导致模型穿插或破面。");

            if(Get(mat,"_DebugMode") > 0.5f)
                AddDiag(ref msg, ref warn, "当前处于调试显示模式，最终效果会被替换成调试图。");

            int modules = EnabledModuleCount(mat);
            string perf = modules <= 3 ? "低" : (modules <= 7 ? "中" : "高");
            EditorGUILayout.HelpBox("当前开启模块数量：" + modules + "，性能预估：" + perf, modules > 7 ? MessageType.Warning : MessageType.Info);

            if(warn == 0)
                EditorGUILayout.HelpBox("当前材质状态：正常。", MessageType.Info);
            else
                EditorGUILayout.HelpBox(msg, MessageType.Warning);

            EditorGUILayout.Space(1);
            if(GUILayout.Button("一键修复显示")) FixVisible(mat);
            if(GUILayout.Button("一键修复透明PNG")) FixAlpha(mat);
            if(GUILayout.Button("一键修复溶解")) FixDissolve(mat);
            // LayoutSafe：已移除EndHorizontal，避免EndLayoutGroup报错。

            EditorGUILayout.Space(1);
            if(GUILayout.Button("一键修复菲尼尔")) FixFresnel(mat);
            if(GUILayout.Button("一键修复扭曲/热浪")) FixDistort(mat);
            if(GUILayout.Button("一键修复Stencil")) FixStencil(mat);
            // LayoutSafe：已移除EndHorizontal，避免EndLayoutGroup报错。

            EditorGUILayout.Space(1);
            if(GUILayout.Button("修复Alpha模式")) FixAlphaBlendMode(mat);
            if(GUILayout.Button("修复Add模式")) FixAddBlendMode(mat);
            if(GUILayout.Button("关闭高级功能")) CloseAllFeaturesKeepMain(mat);
            // LayoutSafe：已移除EndHorizontal，避免EndLayoutGroup报错。

            EditorGUILayout.Space(1);
            if(GUILayout.Button("重置UV")) ResetUVParams(mat);
            if(GUILayout.Button("重置颜色")) ResetColorParams(mat);
            if(GUILayout.Button("基础可见")) BasicVisible(mat);
            // LayoutSafe：已移除EndHorizontal，避免EndLayoutGroup报错。
        }
        // LayoutSafe：已移除EndVertical，避免EndLayoutGroup报错。
    }

    private void DrawDebugQuick(Material mat)
    {
        EditorGUILayout.Space(4);
        EditorGUILayout.LabelField("调试模式快捷查看", EditorStyles.boldLabel);
        EditorGUILayout.Space(1);
        if(GUILayout.Button("正常显示")) Set(mat,"_DebugMode",0);
        if(GUILayout.Button("主贴图")) Set(mat,"_DebugMode",1);
        if(GUILayout.Button("遮罩")) Set(mat,"_DebugMode",2);
        if(GUILayout.Button("溶解")) Set(mat,"_DebugMode",3);
        // LayoutSafe：已移除EndHorizontal，避免EndLayoutGroup报错。

        EditorGUILayout.Space(1);
        if(GUILayout.Button("Alpha")) Set(mat,"_DebugMode",4);
        if(GUILayout.Button("边缘")) Set(mat,"_DebugMode",5);
        if(GUILayout.Button("菲尼尔")) Set(mat,"_DebugMode",6);
        if(GUILayout.Button("Noise")) Set(mat,"_DebugMode",7);
        // LayoutSafe：已移除EndHorizontal，避免EndLayoutGroup报错。
        // LayoutSafe：已移除EndVertical，避免EndLayoutGroup报错。
    }

    private void SetAllFolds(bool open)
    {
        g0 = g1 = g2 = g4 = g8 = g9 = g10 = g11 = g12 = g13 = g14 = g15 = open;
        alphaFold = uvFold = subFold = colorFold = noiseFold = presetFold = open;
    }

    private void OpenBasicFolds()
    {
        SetAllFolds(false);
        g0 = g1 = g2 = true;
        alphaFold = colorFold = true;
    }

    private void OpenOnlyDissolve()
    {
        SetAllFolds(false);
        g8 = true;
    }

    private void OpenOnlyFresnel()
    {
        SetAllFolds(false);
        g9 = true;
    }

    private void DrawTopBar(Material mat)
    {
        DrawConsoleOverview(mat);
    }

    private void DrawMainHeader(Material mat)
    {
        Rect row = EditorGUILayout.GetControlRect(false, 28f);
        EditorGUI.DrawRect(row, new Color(0.16f, 0.16f, 0.16f, 1f));

        Rect titleRect = new Rect(row.x + 8f, row.y + 5f, 130f, row.height - 8f);
        Rect modeLabelRect = new Rect(row.xMax - 198f, row.y + 6f, 50f, row.height - 10f);
        Rect popupRect = new Rect(row.xMax - 144f, row.y + 4f, 118f, row.height - 8f);
        Rect resetRect = new Rect(row.xMax - 22f, row.y + 5f, 18f, row.height - 10f);

        EditorGUI.LabelField(titleRect, "AruiShader1.0", EditorStyles.boldLabel);
        EditorGUI.LabelField(modeLabelRect, "面板模式", EditorStyles.miniLabel);

        int current = Mathf.Clamp(Mathf.RoundToInt(Get(mat,"_PanelMode")), 0, panelModeNames.Length - 1);
        int next = EditorGUI.Popup(popupRect, current, panelModeNames);
        if(next != current)
        {
            Set(mat,"_PanelMode",next);
            EditorUtility.SetDirty(mat);
        }

        if(GUI.Button(resetRect, new GUIContent("↺", "恢复面板模式默认值"), EditorStyles.miniButton))
        {
            ResetFloatPropertyToDefaultMaterial(mat, "_PanelMode");
        }
    }

    private void DrawQuickSectionLabel(string title)
    {
        EditorGUILayout.Space(3);
        Rect row = EditorGUILayout.GetControlRect(false, 19f);
        EditorGUI.DrawRect(row, new Color(0.20f, 0.20f, 0.20f, 1f));
        EditorGUI.LabelField(new Rect(row.x + 6f, row.y + 2f, row.width - 12f, row.height - 3f), title, EditorStyles.miniBoldLabel);
    }

    private void DrawQuickPerformanceRow(Material mat)
    {
        Rect row = GetSafeControlRect(false, 25f);
        float labelWidth = Mathf.Clamp(row.width * 0.18f, 48f, 60f);
        float buttonWidth = Mathf.Clamp(row.width * 0.20f, 56f, 74f);
        float gap = 4f;

        Rect labelRect = new Rect(row.x, row.y + 4f, labelWidth, row.height - 8f);
        Rect popupRect = new Rect(
            row.x + labelWidth + 2f,
            row.y,
            Mathf.Max(72f, row.width - labelWidth - buttonWidth - gap - 2f),
            row.height
        );
        Rect buttonRect = new Rect(popupRect.xMax + gap, row.y, buttonWidth, row.height);

        EditorGUI.LabelField(labelRect, "性能标准", EditorStyles.miniLabel);
        int current = Mathf.Clamp(Mathf.RoundToInt(Get(mat, "_PerformanceMode")), 0, performanceNames.Length - 1);
        int next = EditorGUI.Popup(popupRect, current, performanceNames);
        if(next != current)
        {
            Set(mat, "_PerformanceMode", next);
            EditorUtility.SetDirty(mat);
        }

        if(GUI.Button(buttonRect, "应用", EditorStyles.miniButton))
        {
            ApplyPerformanceMode(mat);
        }
    }

    private void DrawQuickPopupRow(Material mat, string prop, string label, string[] names)
    {
        Rect row = GetSafeControlRect(false, 24f);
        float labelWidth = Mathf.Clamp(row.width * 0.18f, 48f, 60f);
        float resetWidth = 20f;

        Rect labelRect = new Rect(row.x, row.y + 4f, labelWidth, row.height - 8f);
        Rect popupRect = new Rect(
            row.x + labelWidth + 2f,
            row.y,
            Mathf.Max(72f, row.width - labelWidth - resetWidth - 4f),
            row.height
        );
        Rect resetRect = new Rect(popupRect.xMax + 2f, row.y + 2f, resetWidth - 1f, row.height - 4f);

        EditorGUI.LabelField(labelRect, label, EditorStyles.miniLabel);
        int current = Mathf.Clamp(Mathf.RoundToInt(Get(mat, prop)), 0, names.Length - 1);
        int next = EditorGUI.Popup(popupRect, current, names);
        if(next != current)
        {
            Set(mat, prop, next);
            EditorUtility.SetDirty(mat);
        }

        if(GUI.Button(resetRect, new GUIContent("↺", "恢复默认值"), EditorStyles.miniButton))
        {
            ResetFloatPropertyToDefaultMaterial(mat, prop);
        }
    }

    private bool DrawCardFoldout(string title, bool fold, string status)
    {
        EditorGUILayout.Space(6);
        Rect row = EditorGUILayout.GetControlRect(false, 30f);
        EditorGUI.DrawRect(row, new Color(0.125f, 0.125f, 0.125f, 1f));
        EditorGUI.DrawRect(
            new Rect(row.x, row.y, 3f, row.height),
            fold ? new Color(0.36f, 0.72f, 1.0f, 1f) : new Color(0.28f, 0.28f, 0.28f, 1f)
        );

        Rect foldRect = new Rect(row.x + 10f, row.y + 5f, Mathf.Max(80f, row.width - 84f), row.height - 10f);
        Rect statusRect = new Rect(row.xMax - 72f, row.y + 7f, 64f, row.height - 14f);

        fold = EditorGUI.Foldout(
            foldRect,
            fold,
            title,
            true,
            new GUIStyle(EditorStyles.foldout) { fontStyle = FontStyle.Bold, fontSize = 12 }
        );

        GUI.Label(statusRect, status, EditorStyles.centeredGreyMiniLabel);
        return fold;
    }

    private void DrawThreeButtons(string a, System.Action aAction, string b, System.Action bAction, string c, System.Action cAction)
    {
        Rect row = EditorGUILayout.GetControlRect(false, 24f);
        float gap = 2f;
        float width = (row.width - gap * 2f) / 3f;

        if(GUI.Button(new Rect(row.x, row.y, width, row.height), a, EditorStyles.miniButtonLeft)) aAction?.Invoke();
        if(GUI.Button(new Rect(row.x + width + gap, row.y, width, row.height), b, EditorStyles.miniButtonMid)) bAction?.Invoke();
        if(GUI.Button(new Rect(row.x + (width + gap) * 2f, row.y, width, row.height), c, EditorStyles.miniButtonRight)) cAction?.Invoke();
    }

    private void DrawTwoButtons(string a, System.Action aAction, string b, System.Action bAction)
    {
        Rect row = EditorGUILayout.GetControlRect(false, 24f);
        float gap = 2f;
        float width = (row.width - gap) * 0.5f;

        if(GUI.Button(new Rect(row.x, row.y, width, row.height), a, EditorStyles.miniButtonLeft)) aAction?.Invoke();
        if(GUI.Button(new Rect(row.x + width + gap, row.y, width, row.height), b, EditorStyles.miniButtonRight)) bAction?.Invoke();
    }

    private bool BeginModule(MaterialEditor editor, MaterialProperty[] props, string title, ref bool fold, string toggleProp)
    {
        MaterialProperty p = FindProperty(toggleProp, props, false);
        bool active = p == null || p.floatValue > 0.5f;

        // SwitchBeforeName：
        // 不使用BeginHorizontal/EndHorizontal，避免IMGUI布局栈报错。
        // 开关在标题前面，标题和开关保持同一行平行对齐。
        EditorGUILayout.Space(3);
        Rect row = EditorGUILayout.GetControlRect(false, 24f);
        EditorGUI.DrawRect(row, new Color(0.18f, 0.18f, 0.18f, 1f));

        Rect toggleRect = new Rect(row.x + 5f, row.y + 3f, 18f, row.height - 6f);
        Rect foldRect = new Rect(row.x + 28f, row.y + 2f, Mathf.Max(80f, row.width - 96f), row.height - 4f);
        Rect statusRect = new Rect(row.xMax - 62f, row.y + 5f, 56f, row.height - 8f);

        if (p != null)
        {
            bool nv = EditorGUI.Toggle(toggleRect, active);
            if (nv != active)
            {
                p.floatValue = nv ? 1f : 0f;
                active = nv;
            }
        }

        fold = EditorGUI.Foldout(foldRect, fold, title, true, new GUIStyle(EditorStyles.foldout){fontStyle = FontStyle.Bold});
        GUI.Label(statusRect, active ? "已启用" : "未启用", EditorStyles.miniLabel);

        if (!active)
        {
            EditorGUILayout.HelpBox("模块已关闭。勾选标题前方开关后显示参数。", MessageType.None);
        }

        if(active && fold) EditorGUILayout.Space(2);
        return active && fold;
    }

    private void EndModule(){ }

    private void DrawRender(MaterialEditor e, MaterialProperty[] p, Material m)
    {
        g0 = DrawCardFoldout("1. 渲染", g0, "渲染");
        if(g0)
        {
            Popup(m,"_BlendMode","混合模式",blendNames, v=>ApplyBlend(m,v));
            Popup(m,"_OutputMode","输出模式",outputNames,null);
            Popup(m,"_DebugMode","调试显示模式",debugNames,null);

            DrawProp(e,p,"_ZTestToggle");
            ApplyZTestState(m);

            Popup(m,"_CullDisplayMode","显示面模式",cullDisplayNames, v=>ApplyDoubleSided(m));
            ApplyDoubleSided(m);

            DrawProp(e,p,"_ZWriteToggle");
            ApplyZWrite(m);

            DrawProp(e,p,"_UseStencil");
            ApplyStencilState(m);
            if(Get(m,"_UseStencil")>0.5f)
            {
                EditorGUILayout.LabelField("模板预设", EditorStyles.boldLabel);
                EditorGUILayout.Space(1);
                if(GUILayout.Button("关闭模板")) StencilPresetOff(m);
                if(GUILayout.Button("写入遮罩")) StencilPresetWriteMask(m);
                if(GUILayout.Button("只显示遮罩内")) StencilPresetReadInside(m);
                // LayoutSafe：已移除EndHorizontal，避免EndLayoutGroup报错。

                EditorGUILayout.Space(1);
                if(GUILayout.Button("反向遮罩")) StencilPresetReadOutside(m);
                if(GUILayout.Button("清除遮罩")) StencilPresetEraseMask(m);
                if(GUILayout.Button("叠加写入")) StencilPresetIncrementMask(m);
                // LayoutSafe：已移除EndHorizontal，避免EndLayoutGroup报错。

                EditorGUILayout.Space(4);
                EditorGUILayout.LabelField("模板高级参数", EditorStyles.boldLabel);
                DrawProp(e,p,"_StencilRef");
                PopupMapped(m,"_StencilComp","模板比较",stencilCompNames,stencilCompValues);
                PopupMapped(m,"_StencilPass","模板通过操作",stencilPassNames,stencilPassValues);
                PopupMapped(m,"_StencilFail","模板失败操作",stencilPassNames,stencilPassValues);
                PopupMapped(m,"_StencilZFail","模板深度失败操作",stencilPassNames,stencilPassValues);
                EditorGUILayout.HelpBox("常用情况直接点预设即可。高级参数说明：通过=模板测试和深度测试都通过时执行；失败=模板测试失败时执行；深度失败=模板通过但深度测试失败时执行。", MessageType.None);
                DrawProp(e,p,"_StencilReadMask");
                DrawProp(e,p,"_StencilWriteMask");
            }

            DrawProp(e,p,"_UseSoftParticles");
            EditorGUILayout.HelpBox("软粒子：交界处逐渐透明，用来消除特效插进地面/模型时的硬边。\n反向软粒子：只显示交界边缘，适合贴地气浪、落地尘、接触光。", MessageType.None);
            if(Get(m,"_UseSoftParticles")>0.5f)
            {
                Popup(m,"_SoftParticleMode","接触边缘模式",depthIntersectionModeNames,null);
                DrawProp(e,p,"_SoftParticleDistance");
                DrawProp(e,p,"_SoftParticleSoftness");

                if(Mathf.RoundToInt(Get(m,"_SoftParticleMode")) == 2)
                {
                    DrawProp(e,p,"_DepthEdgeColor");
                    DrawProp(e,p,"_DepthEdgeIntensity");
                    DrawProp(e,p,"_DepthEdgeAffectColor");
                    DrawProp(e,p,"_DepthEdgeAffectAlpha");
                }

                EditorGUILayout.HelpBox("软粒子：淡出硬边。\n反向软粒子：保留接触边缘。\n深度边缘发光：在反向软粒子基础上加颜色高亮。", MessageType.None);
            }

            DrawProp(e,p,"_UseCameraDistanceFade");
            if(Get(m,"_UseCameraDistanceFade")>0.5f)
            {
                DrawProp(e,p,"_CameraNearFadeStart");
                DrawProp(e,p,"_CameraNearFadeEnd");
                DrawProp(e,p,"_CameraFarFadeStart");
                DrawProp(e,p,"_CameraFarFadeEnd");
            }

        }
        // LayoutSafe：已移除EndVertical，避免EndLayoutGroup报错。
    }

    private void DrawMaterialTypeAlphaUV(MaterialEditor e, MaterialProperty[] p, Material m)
    {
        g1 = DrawCardFoldout("2. 材质类型 / Alpha / UV", g1, "基础");
        if(!g1) return;

        SectionTitle("2.1 Alpha / 面片 / 模型模式");
        DrawProp(e,p,"_UseAlphaModule");
        if(Get(m,"_UseAlphaModule")>0.5f)
        {
            EditorGUILayout.HelpBox("AlphaFix：主贴图A通道会作为最终透明遮罩，同时裁切边缘/菲尼尔附加Alpha；整体透明度和粒子颜色A会控制全部Alpha。", MessageType.None);
            DrawProp(e,p,"_UseMainAlpha");
            DrawProp(e,p,"_AlphaPower");
            DrawProp(e,p,"_AlphaClipEnable");
            if(Get(m,"_AlphaClipEnable")>0.5f) DrawProp(e,p,"_AlphaClip");
            DrawProp(e,p,"_QuadMaskRadius");
            DrawProp(e,p,"_QuadMaskSoftness");
            DrawProp(e,p,"_QuadMaskScaleX");
            DrawProp(e,p,"_QuadMaskScaleY");
        }
        else
        {
            EditorGUILayout.LabelField("勾选后显示 Alpha 与面片模式参数。", EditorStyles.miniLabel);
        }

        SectionTitle("2.2 UV变形");
        DrawProp(e,p,"_UseUVModule");
        if(Get(m,"_UseUVModule")>0.5f)
        {
            Popup(m,"_ModelUVMode","模型UV选择",modelUVModeNames,null);
            EditorGUILayout.HelpBox("模型UV选择会影响主/副贴图采样，也会影响模型方向溶解方向。用于快速修正模型UV方向。", MessageType.None);
            DrawProp(e,p,"_UVSwirlStrength");
            DrawProp(e,p,"_UVWaveStrength");
            DrawProp(e,p,"_UVWaveScale");
            DrawProp(e,p,"_UVWaveSpeed");
            DrawProp(e,p,"_UVRadialStrength");
            DrawProp(e,p,"_UVKaleidoscope");
            DrawProp(e,p,"_UVCenterX");
            DrawProp(e,p,"_UVCenterY");
        }
        else
        {
            EditorGUILayout.LabelField("勾选后显示 UV 变形参数。", EditorStyles.miniLabel);
        }
    }

    private void DrawMainSubColor(MaterialEditor e, MaterialProperty[] p, Material m)
    {
        g2 = DrawCardFoldout("3. 主贴图 / 颜色 / 主贴图功能", g2, "核心");
        if(!g2) return;

        SectionTitle("3.1 主贴图");
        DrawProp(e,p,"_UseMainLayer");
        if(Get(m,"_UseMainLayer")>0.5f)
        {
            DrawProp(e,p,"_UseCDMainOffset");

            DrawProp(e,p,"_MainUseFlipbook");
            if(Get(m,"_MainUseFlipbook")>0.5f)
            {
                DrawProp(e,p,"_MainFlipbookColumns");
                DrawProp(e,p,"_MainFlipbookRows");
                DrawProp(e,p,"_MainFlipbookSpeed");
                DrawProp(e,p,"_MainFlipbookFrame");
                DrawProp(e,p,"_MainFlipbookLoop");
            }

            DrawProp(e,p,"_MainTex");
            DrawProp(e,p,"_MainAlphaStrength");
            DrawTextureUVOptions(e,p,m,"_Main","主贴图");
            DrawTextureFlow(e,p,m,"_Main","主贴图");

            Popup(m,"_MainShapeSource","主贴图形体来源",shapeSourceNames,null);
            DrawProp(e,p,"_MainContrast");
            DrawProp(e,p,"_MainPower");
            DrawProp(e,p,"_MainColorMix");

            DrawProp(e,p,"_MainUseBlur");
            if(Get(m,"_MainUseBlur")>0.5f)
            {
                Popup(m,"_MainBlurMode","主贴图模糊方式",mainBlurModeNames,null);
                DrawProp(e,p,"_MainBlurStrength");
                if(Mathf.RoundToInt(Get(m,"_MainBlurMode")) == 1)
                {
                    DrawProp(e,p,"_MainMotionBlurAngle");
                }
                EditorGUILayout.HelpBox("主贴图模糊作用在主贴图采样阶段：均值=整体柔化，动感=按方向拖影，高斯=更柔和，径向=从主贴图UV中心向外拖影。", MessageType.None);
            }

            DrawProp(e,p,"_MainUseHueSplit");
            if(Get(m,"_MainUseHueSplit")>0.5f)
            {
                EditorGUILayout.HelpBox("色相分离默认强度为0，建议从0.002慢慢提高。", MessageType.None);
                DrawProp(e,p,"_MainHueSplitAmount");
                DrawProp(e,p,"_MainHueSplitAngle");
                DrawProp(e,p,"_MainHueSplitSpeed");
            }
        }

        EditorGUILayout.Space(8);
        colorFold = EditorGUILayout.Foldout(colorFold, "3.2 颜色 / Ramp / 颜色后处理", true);
        if(colorFold)
        {
            DrawPresetTools(m);
            EditorGUILayout.Space(4);
            DrawProp(e,p,"_UseThreeColor");
            if(Get(m,"_UseThreeColor")>0.5f)
            {
                Set(m,"_UseBackFaceColor",0f);
                DrawProp(e,p,"_ColorA");
                DrawProp(e,p,"_ColorB");
                DrawProp(e,p,"_ColorC");
                DrawProp(e,p,"_ColorSplit1");
                DrawProp(e,p,"_ColorSplit2");
                DrawProp(e,p,"_ColorSoftness");
                EditorGUILayout.HelpBox("启用三段色后，背面颜色控制会自动关闭。", MessageType.None);
            }
            else
            {
                DrawProp(e,p,"_SingleColor");
                DrawProp(e,p,"_UseBackFaceColor");
                if(Get(m,"_UseBackFaceColor")>0.5f)
                {
                    DrawProp(e,p,"_BackFaceColor");
                    DrawProp(e,p,"_BackFaceColorSoftness");
                    EditorGUILayout.HelpBox("背面颜色只在单色模式下生效。切边柔和度越高，正面和背面颜色过渡越柔。", MessageType.None);
                }
            }

            DrawProp(e,p,"_Opacity");
            DrawProp(e,p,"_EmissionIntensity");

            EditorGUILayout.Space(4);
            DrawProp(e,p,"_UseColorAdjust");
            if(Get(m,"_UseColorAdjust")>0.5f)
            {
                DrawProp(e,p,"_HueShift");
                DrawProp(e,p,"_Saturation");
                DrawProp(e,p,"_Value");
                DrawProp(e,p,"_ColorContrast");
                DrawProp(e,p,"_InvertColor");
                DrawProp(e,p,"_UseBlackWhiteFlash");
                if(Get(m,"_UseBlackWhiteFlash")>0.5f) DrawProp(e,p,"_BlackWhiteFlashValue");
                DrawProp(e,p,"_OverExposureClamp");
            }

            EditorGUILayout.Space(4);
            DrawProp(e,p,"_UseRampModule");
            if(Get(m,"_UseRampModule")>0.5f)
            {
                DrawProp(e,p,"_RampTex");
                DrawTextureUVOptions(e,p,m,"_Ramp","Ramp");
                Popup(m,"_RampBlendMode","Ramp叠加模式",rampBlendNames,null);
                DrawTextureFlow(e,p,m,"_Ramp","Ramp");
                DrawProp(e,p,"_RampOffset");
                DrawProp(e,p,"_RampContrast");
                DrawProp(e,p,"_RampStrength");
            }
        }

        EditorGUILayout.Space(8);
        subFold = EditorGUILayout.Foldout(subFold, "3.3 副贴图层", true);
        if(subFold)
        {
            DrawProp(e,p,"_UseSubLayer");
            if(Get(m,"_UseSubLayer")>0.5f)
            {
                DrawProp(e,p,"_SubTex");
                DrawTextureUVOptions(e,p,m,"_Sub","副贴图");
                DrawTextureFlow(e,p,m,"_Sub","副贴图");
                Popup(m,"_SubBlendMode","副贴图混合模式",subBlendNames,null);
                DrawProp(e,p,"_SubContrast");
                DrawProp(e,p,"_SubPower");
                DrawProp(e,p,"_SubStrength");
                DrawProp(e,p,"_SubRemoveBlack");
                DrawProp(e,p,"_SubColor");
                DrawProp(e,p,"_SubColorStrength");
                DrawProp(e,p,"_SubColorIntensity");
                DrawProp(e,p,"_SubUseHueSplit");
                if(Get(m,"_SubUseHueSplit")>0.5f)
                {
                    DrawProp(e,p,"_SubHueSplitAmount");
                    DrawProp(e,p,"_SubHueSplitAngle");
                    DrawProp(e,p,"_SubHueSplitSpeed");
                }
            }
        }

        EditorGUILayout.Space(8);
        SectionTitle("3.4 径向波纹 / 冲击波");
        DrawProp(e,p,"_UseRadialWave");
        if(Get(m,"_UseRadialWave")>0.5f)
        {
            DrawProp(e,p,"_RadialWaveCenterX");
            DrawProp(e,p,"_RadialWaveCenterY");
            DrawProp(e,p,"_RadialWaveRadius");
            DrawProp(e,p,"_RadialWaveWidth");
            DrawProp(e,p,"_RadialWaveSoftness");
            DrawProp(e,p,"_RadialWaveSpeed");
            DrawProp(e,p,"_RadialWaveLoop");
            DrawProp(e,p,"_RadialWaveColor");
            DrawProp(e,p,"_RadialWaveIntensity");
            DrawProp(e,p,"_RadialWaveAffectAlpha");
            DrawProp(e,p,"_RadialWaveDistortStrength");
            EditorGUILayout.HelpBox("可用于冲击波、水波扩散、能量环、护盾受击。", MessageType.None);
        }

        DrawParallaxModule(e,p,m);
    }

    private void DrawMaskLayer(MaterialEditor e, MaterialProperty[] p, Material m)
    {
        g4 = DrawCardFoldout("4. 遮罩 / 二级遮罩", g4, "可选");
        if(g4)
        {
            SectionTitle("4.1 基础遮罩");
            DrawProp(e,p,"_UseMaskLayer");
            if(Get(m,"_UseMaskLayer")>0.5f)
            {
                DrawProp(e,p,"_UseCDMaskOffset");
                DrawProp(e,p,"_MaskTex");
                DrawTextureUVOptions(e,p,m,"_Mask","遮罩");
                DrawTextureFlow(e,p,m,"_Mask","遮罩");
                DrawProp(e,p,"_MaskInvert");
                DrawProp(e,p,"_MaskContrast");
                DrawProp(e,p,"_MaskPower");
                DrawProp(e,p,"_MaskAffectAlpha");
                DrawProp(e,p,"_MaskAffectColor");
                DrawProp(e,p,"_MaskAffectDissolve");
                DrawProp(e,p,"_MaskAffectDistort");
                DrawProp(e,p,"_MaskAffectFresnel");
            }

            EditorGUILayout.Space(8);
            SectionTitle("4.2 辅助遮罩 / 二级遮罩");
            DrawProp(e,p,"_UseAuxMaskLayer");
            if(Get(m,"_UseAuxMaskLayer")>0.5f)
            {
                DrawProp(e,p,"_AuxMaskTex");
                DrawTextureUVOptions(e,p,m,"_AuxMask","辅助遮罩");
                DrawTextureFlow(e,p,m,"_AuxMask","辅助遮罩");
                Popup(m,"_AuxMaskChannel","辅助遮罩通道",auxMaskChannelNames,null);
                DrawProp(e,p,"_AuxMaskInvert");
                DrawProp(e,p,"_AuxMaskContrast");
                DrawProp(e,p,"_AuxMaskPower");
                DrawProp(e,p,"_AuxMaskAffectAlpha");
                DrawProp(e,p,"_AuxMaskAffectColor");
                DrawProp(e,p,"_AuxMaskAffectDissolve");
                DrawProp(e,p,"_AuxMaskAffectDistort");
                DrawProp(e,p,"_AuxMaskAffectFresnel");
                EditorGUILayout.HelpBox("二级遮罩独立于基础遮罩，可单独影响Alpha、颜色、溶解、扭曲和菲尼尔。", MessageType.None);
            }
        }
        // LayoutSafe：已移除EndVertical，避免EndLayoutGroup报错。
    }

    private void DrawDissolve(MaterialEditor e, MaterialProperty[] p, Material m)
    {
        g8 = DrawCardFoldout("5. 溶解", g8, "可选");
        if(g8)
        {
            SectionTitle("5.1 基础溶解");
            DrawProp(e,p,"_UseDissolveModule");
            if(Get(m,"_UseDissolveModule")>0.5f)
            {
                DrawProp(e,p,"_UseCDDissolve");
                DrawProp(e,p,"_UseCDDissolveOffset");
                if(Get(m,"_UseCDDissolveOffset")>0.5f)
                {
                    Popup(m,"_CDDissolveOffsetAxis","Custom1.W控制轴",dissolveOffsetAxisNames,null);
                }
                DrawProp(e,p,"_DissolveAmount");
                EditorGUILayout.HelpBox("溶解：进度0完全显示，进度1完全溶解；边缘宽度只影响中间过渡。", MessageType.None);
                DrawProp(e,p,"_DissolveTex");
                DrawTextureUVOptions(e,p,m,"_Dissolve","溶解");
                DrawTextureFlow(e,p,m,"_Dissolve","溶解");

                EditorGUILayout.Space(4);
                EditorGUILayout.LabelField("溶解方向图 / 溶解遮罩", EditorStyles.boldLabel);
                Popup(m,"_DissolveExtraMode","溶解方向图模式",new string[]{"关闭","控制溶解方向","作为溶解遮罩"},null);
                if(Get(m,"_DissolveExtraMode") > 0.5f)
                {
                    DrawProp(e,p,"_DissolveExtraTex");
                    DrawTextureUVOptions(e,p,m,"_DissolveExtra","辅助溶解");
                    DrawTextureFlow(e,p,m,"_DissolveExtra","辅助溶解");
                    DrawProp(e,p,"_DissolveExtraStrength");
                    DrawProp(e,p,"_DissolveExtraInvert");
                    if(Get(m,"_DissolveExtraMode") < 1.5f)
                        EditorGUILayout.HelpBox("控制溶解方向：辅助图与主溶解图按强度混合。", MessageType.None);
                    else
                        EditorGUILayout.HelpBox("作为溶解遮罩：白色区域溶解，黑色区域保留。", MessageType.None);
                }
                DrawProp(e,p,"_DissolveHardEdge");
                if(Get(m,"_DissolveHardEdge") < 0.5f)
                {
                    DrawProp(e,p,"_DissolveSoftness");
                    DrawProp(e,p,"_DissolveSoftPreserveSolid");
                    if(Get(m,"_DissolveSoftPreserveSolid") > 0.5f)
                    {
                        EditorGUILayout.HelpBox("只软化溶解交界，未溶解主体保持实心，避免整个特效发虚。", MessageType.None);
                    }
                }
                DrawProp(e,p,"_DissolveContrast");
                DrawProp(e,p,"_DissolveEdgeWidth");
                DrawProp(e,p,"_DissolveEdgeColor");
                DrawProp(e,p,"_DissolveEdgeIntensity");
                DrawProp(e,p,"_DissolveOuterEdgeColor");
                DrawProp(e,p,"_DissolveOuterEdgeWidth");
                DrawProp(e,p,"_DissolveOuterEdgeIntensity");
                DrawProp(e,p,"_DissolveEdgeAffectAlpha");

                DrawProp(e,p,"_UseDirectionDissolve");
                if(Get(m,"_UseDirectionDissolve")>0.5f)
                {
                    Popup(m,"_DirectionMode","方向溶解模式",dirNames,null);
                    DrawProp(e,p,"_DirectionStrength");
                    DrawProp(e,p,"_DirectionNoiseBlend");
                    DrawProp(e,p,"_DirectionPower");
                }

                DrawProp(e,p,"_UseLayerDissolve");
                if(Get(m,"_UseLayerDissolve")>0.5f)
                {
                    DrawProp(e,p,"_Layer1End");
                    DrawProp(e,p,"_Layer2End");
                    DrawProp(e,p,"_Layer3Start");
                    DrawProp(e,p,"_LayerDissolveSoftness");
                }

                Popup(m,"_DissolveCurveMode","溶解曲线模式",curveNames,null);
                DrawProp(e,p,"_DissolveCurvePower");
                DrawProp(e,p,"_DissolveDelay");
                DrawProp(e,p,"_DissolveEndBoost");
            }
        }
        // LayoutSafe：已移除EndVertical，避免EndLayoutGroup报错。
    }

    private void DrawEdge(MaterialEditor e, MaterialProperty[] p, Material m)
    {
        g9 = DrawCardFoldout("7. 边缘与菲尼尔", g9, "轮廓");
        if(!g9) return;

        SectionTitle("7.1 边缘亮边");
        DrawProp(e,p,"_UseEdgeModule");
        if(Get(m,"_UseEdgeModule")>0.5f)
        {
            DrawProp(e,p,"_EdgeOnlyColor");
            DrawProp(e,p,"_EdgeWidth");
            DrawProp(e,p,"_EdgeBrightness");
            DrawProp(e,p,"_EdgeIntensity");
            DrawProp(e,p,"_EdgeSoftness");
            DrawProp(e,p,"_EdgeAffectAlpha");

            EditorGUILayout.Space(4);
            DrawProp(e,p,"_UseEdgeRamp");
            if(Get(m,"_UseEdgeRamp")>0.5f)
            {
                DrawProp(e,p,"_EdgeRampTex");
                DrawTextureUVOptions(e,p,m,"_EdgeRamp","边缘Ramp");
                DrawProp(e,p,"_EdgeRampStrength");
            }
        }

        EditorGUILayout.Space(8);
        SectionTitle("7.2 菲尼尔");
        DrawProp(e,p,"_UseFresnelModule");
        if(Get(m,"_UseFresnelModule")>0.5f)
        {
            DrawProp(e,p,"_FresnelColor");
            DrawProp(e,p,"_UseFresnelInnerColor");
            if(Get(m,"_UseFresnelInnerColor") > 0.5f)
            {
                DrawProp(e,p,"_FresnelInnerColor");
                DrawProp(e,p,"_FresnelInnerWidth");
                DrawProp(e,p,"_FresnelInnerIntensity");
                EditorGUILayout.HelpBox("内圈颜色控制菲尼尔光环靠近透明中心的内侧边缘。仅在非反向菲尼尔时生效。", MessageType.None);
            }
            DrawProp(e,p,"_FresnelInvert");
            EditorGUILayout.HelpBox("菲尼尔纯外壳模式：非反向=只显示随摄像机旋转变化的模型边缘；反向=中心显示、外圈隐藏。", MessageType.None);
            DrawProp(e,p,"_DissolveAffectFresnel");
            EditorGUILayout.HelpBox("开启后，溶解会同时裁切菲尼尔颜色、菲尼尔Alpha、内圈、双层菲尼尔和强制纯外壳。默认开启。", MessageType.None);
            DrawProp(e,p,"_DissolveDistortAffectFresnel");
            EditorGUILayout.HelpBox("此项只让溶解噪波扰动菲尼尔强度，不负责裁切。", MessageType.None);
            DrawProp(e,p,"_FresnelThickness");
            DrawProp(e,p,"_FresnelIntensity");
            DrawProp(e,p,"_FresnelBrightness");
            DrawProp(e,p,"_FresnelPower");
            DrawProp(e,p,"_FresnelBias");
            DrawProp(e,p,"_FresnelAffectAlpha");
            DrawProp(e,p,"_FresnelOutlineExpand");
            DrawProp(e,p,"_FresnelForceHollow");
            if(Get(m,"_FresnelForceHollow")>0.5f)
            {
                DrawProp(e,p,"_FresnelHollowPower");
                DrawProp(e,p,"_FresnelHollowMin");
                DrawProp(e,p,"_FresnelHideBackface");
            }

            EditorGUILayout.Space(4);
            DrawProp(e,p,"_FresnelUseNoise");
            if(Get(m,"_FresnelUseNoise")>0.5f)
            {
                DrawProp(e,p,"_FresnelNoiseStrength");
                DrawProp(e,p,"_FresnelNoiseScale");
                DrawProp(e,p,"_FresnelNoiseSpeed");
            }

            EditorGUILayout.Space(4);
            DrawProp(e,p,"_UseSecondFresnel");
            if(Get(m,"_UseSecondFresnel")>0.5f)
            {
                DrawProp(e,p,"_FresnelSecondColor");
                DrawProp(e,p,"_FresnelSecondThickness");
                DrawProp(e,p,"_FresnelSecondIntensity");
                DrawProp(e,p,"_FresnelSecondBrightness");
                DrawProp(e,p,"_FresnelSecondPower");
                DrawProp(e,p,"_FresnelSecondAffectAlpha");
            }

            EditorGUILayout.Space(4);
            DrawProp(e,p,"_UseFresnelRamp");
            if(Get(m,"_UseFresnelRamp")>0.5f)
            {
                DrawProp(e,p,"_FresnelRampTex");
                DrawTextureUVOptions(e,p,m,"_FresnelRamp","菲尼尔Ramp");
                DrawProp(e,p,"_FresnelRampStrength");
            }
        }
    }

    private void DrawDistort(MaterialEditor e, MaterialProperty[] p, Material m)
    {
        g10 = DrawCardFoldout("6. 扭曲 / 热浪", g10, "可选");
        if(!g10) return;

        SectionTitle("6.1 扭曲");
        DrawProp(e,p,"_UseDistortModule");
        if(Get(m,"_UseDistortModule")<=0.5f)
        {
            EditorGUILayout.LabelField("勾选后显示扭曲与热浪参数。", EditorStyles.miniLabel);
            return;
        }

        DrawProp(e,p,"_UseCDDistort");
        DrawProp(e,p,"_UseFlowMap");
        if(Get(m,"_UseFlowMap")>0.5f)
        {
            DrawProp(e,p,"_FlowMapTex");
            DrawProp(e,p,"_FlowMapStrength");
            DrawProp(e,p,"_FlowMapSpeed");
            DrawProp(e,p,"_FlowMapTiling");
            DrawProp(e,p,"_FlowMapAffectMain");
            DrawProp(e,p,"_FlowMapAffectSub");
            DrawProp(e,p,"_FlowMapAffectDissolve");
            DrawProp(e,p,"_FlowMapAffectDistort");
        }

        DrawProp(e,p,"_DistortTex");
        DrawTextureUVOptions(e,p,m,"_Distort","扭曲");
        DrawTextureFlow(e,p,m,"_Distort","扭曲");

        EditorGUILayout.Space(3);
        EditorGUILayout.LabelField("扭曲遮罩", EditorStyles.boldLabel);
        DrawProp(e,p,"_DistortMaskTex");
        DrawTextureUVOptions(e,p,m,"_DistortMask","扭曲遮罩");
        DrawTextureFlow(e,p,m,"_DistortMask","扭曲遮罩");
        DrawProp(e,p,"_DistortMaskStrength");
        DrawProp(e,p,"_DistortMaskInvert");
        EditorGUILayout.HelpBox("白色保留扭曲，黑色关闭扭曲；默认白图不改变旧效果。", MessageType.None);

        EditorGUILayout.Space(4);
        EditorGUILayout.LabelField("扭曲影响对象", EditorStyles.boldLabel);
        DrawProp(e,p,"_DistortAffectMain");
        DrawProp(e,p,"_DistortAffectMask");
        DrawProp(e,p,"_DistortAffectDissolve");
        DrawProp(e,p,"_DistortAffectFresnel");

        DrawProp(e,p,"_DistortStrength");
        DrawProp(e,p,"_DistortGradientStep");

        SectionTitle("6.2 热浪");
        DrawProp(e,p,"_UseEdgeHeat");
        if(Get(m,"_UseEdgeHeat")>0.5f)
        {
            DrawProp(e,p,"_HeatAffectMain");
            DrawProp(e,p,"_EdgeHeatStrength");
            DrawProp(e,p,"_EdgeHeatScaleX");
            DrawProp(e,p,"_EdgeHeatScaleY");
            DrawProp(e,p,"_EdgeHeatSpeed");
        }
        else
        {
            EditorGUILayout.LabelField("勾选后显示热浪参数。", EditorStyles.miniLabel);
        }
    }

    private void DrawVertex(MaterialEditor e, MaterialProperty[] p, Material m)
    {
        g11 = DrawCardFoldout("8. 顶点偏移", g11, "可选");
        if(!g11) return;

        SectionTitle("8.1 顶点偏移");
        DrawProp(e,p,"_UseVertexModule");
        if(Get(m,"_UseVertexModule") > 0.5f)
        {
            DrawProp(e,p,"_UseCDVertex");
            DrawProp(e,p,"_VertexUseFlow");
            if(Get(m,"_VertexUseFlow") > 0.5f)
            {
                DrawProp(e,p,"_VertexFlowX");
                DrawProp(e,p,"_VertexFlowY");
                DrawProp(e,p,"_VertexFlowSpeed");
            }

            DrawProp(e,p,"_VertexUseAnimMode");
            if(Get(m,"_VertexUseAnimMode") < 0.5f)
            {
                EditorGUILayout.HelpBox("顶点控制图：黑色=0偏移，白色=完整偏移，灰色=按比例偏移；方向由强度正负决定。", MessageType.None);
                DrawProp(e,p,"_VertexTex");
                DrawTextureUVOptions(e,p,m,"_Vertex","顶点贴图");

                EditorGUILayout.LabelField("顶点偏移黑白遮罩图", EditorStyles.boldLabel);
                DrawProp(e,p,"_VertexOffsetMaskTex");
                DrawTextureUVOptions(e,p,m,"_VertexOffsetMask","顶点偏移黑白遮罩");
                DrawTextureFlow(e,p,m,"_VertexOffsetMask","顶点偏移黑白遮罩");
                EditorGUILayout.HelpBox("黑白遮罩图控制最终顶点偏移：黑色=0强度，白色=1强度，灰色按黑白过渡计算。", MessageType.None);

                DrawProp(e,p,"_VertexStrength");
                DrawProp(e,p,"_VertexTexContrast");
                DrawProp(e,p,"_VertexTexOffset");
                DrawProp(e,p,"_VertexTexPower");
            }
            else
            {
                Popup(m,"_VertexMode","顶点动画模式",vertexModeNames,null);
                EditorGUILayout.LabelField("顶点偏移黑白遮罩图", EditorStyles.boldLabel);
                DrawProp(e,p,"_VertexOffsetMaskTex");
                DrawTextureUVOptions(e,p,m,"_VertexOffsetMask","顶点偏移黑白遮罩");
                DrawTextureFlow(e,p,m,"_VertexOffsetMask","顶点偏移黑白遮罩");
                DrawProp(e,p,"_VertexStrength");
                DrawProp(e,p,"_VertexSpeed");
                DrawProp(e,p,"_VertexScale");
                DrawProp(e,p,"_VertexDirectionX");
                DrawProp(e,p,"_VertexDirectionY");
                DrawProp(e,p,"_VertexDirectionZ");
            }
        }
        else
        {
            EditorGUILayout.LabelField("勾选后显示顶点偏移参数。", EditorStyles.miniLabel);
        }

        SectionTitle("8.2 VAT 顶点动画");
        DrawProp(e,p,"_UseVAT");
        if(Get(m,"_UseVAT") > 0.5f)
        {
            DrawProp(e,p,"_VATTex");
            Popup(m,"_VATMode","VAT模式",vatModeNames,null);
            DrawProp(e,p,"_VATStrength");
            DrawProp(e,p,"_VATColumns");
            DrawProp(e,p,"_VATRows");
            DrawProp(e,p,"_VATSpeed");
            DrawProp(e,p,"_VATFrame");
            DrawProp(e,p,"_VATLoop");
            EditorGUILayout.HelpBox("VAT基础版：RGB=XYZ偏移时0.5为不偏移；黑白沿法线模式适合简单模型鼓动/变形。", MessageType.None);
        }
        else
        {
            EditorGUILayout.LabelField("勾选后显示VAT顶点动画参数。", EditorStyles.miniLabel);
        }
    }

    private void DrawParallaxModule(MaterialEditor e, MaterialProperty[] p, Material m)
    {
        SectionTitle("3.5 视差 / 黑白图或法线图");
        DrawProp(e,p,"_UseParallax");
        if(Get(m,"_UseParallax") <= 0.5f)
        {
            EditorGUILayout.LabelField("勾选后显示视差参数。", EditorStyles.miniLabel);
            return;
        }

        DrawProp(e,p,"_ParallaxTex");
        Popup(m,"_ParallaxMode","视差模式",parallaxModeNames,null);
        DrawProp(e,p,"_ParallaxStrength");
        DrawProp(e,p,"_ParallaxCenter");
        DrawProp(e,p,"_ParallaxAffectMain");
        DrawProp(e,p,"_ParallaxAffectSub");
        DrawProp(e,p,"_ParallaxAffectMask");
        DrawProp(e,p,"_ParallaxAffectDissolve");
        EditorGUILayout.HelpBox("视差使用黑白高度图或法线RG，主要给主/副贴图、遮罩、溶解制造表面层次。", MessageType.None);
    }

    private void DrawSpeedLineModule(MaterialEditor e, MaterialProperty[] p, Material m)
    {
        g14 = DrawCardFoldout("9. 速度线 / 屏幕特效", g14, "可选");
        if(!g14) return;

        SectionTitle("9.1 屏幕速度线 / 冲锋边缘线");
        DrawSpeedLineToggle(e,p,m);
        if(Get(m,"_UseSpeedLine") <= 0.5f)
        {
            EditorGUILayout.LabelField("勾选后显示速度线参数。", EditorStyles.miniLabel);
            return;
        }

        if(GUILayout.Button("一键设置参考图冲锋速度线", GUILayout.Height(24))) SetupSpeedLineRunStyle(m);

        Popup(m,"_SpeedLineMode","速度线UV模式",speedLineModeNames,null);
        EditorGUILayout.HelpBox("速度线读取主贴图颜色，并受单色/三段色/Ramp控制。", MessageType.None);
        DrawProp(e,p,"_SpeedLineIntensity");
        DrawProp(e,p,"_SpeedLineAlpha");

        SectionTitle("9.2 中心掏空形状 / 边缘范围");
        DrawProp(e,p,"_SpeedLineCenterX");
        DrawProp(e,p,"_SpeedLineCenterY");
        DrawProp(e,p,"_SpeedLineEdgeStart");
        DrawProp(e,p,"_SpeedLineEdgeSoftness");
        Popup(m,"_SpeedLineHoleShape","中心掏空形状",speedLineHoleShapeNames,null);
        DrawProp(e,p,"_SpeedLineCenterClear");
        DrawProp(e,p,"_SpeedLineCenterSoftness");
        DrawProp(e,p,"_SpeedLineHoleScaleX");
        DrawProp(e,p,"_SpeedLineHoleScaleY");
        DrawProp(e,p,"_SpeedLineAspectFix");
        DrawProp(e,p,"_SpeedLineVignette");

        SectionTitle("9.3 线条密度 / 拉伸");
        DrawProp(e,p,"_SpeedLineAngleTiling");
        DrawProp(e,p,"_SpeedLineRadialTiling");
        DrawProp(e,p,"_SpeedLineLength");
        DrawProp(e,p,"_SpeedLinePower");
        DrawProp(e,p,"_SpeedLineInvert");

        SectionTitle("9.4 联动 / 流动");
        DrawProp(e,p,"_SpeedLineAffectDistort");
        DrawProp(e,p,"_SpeedLineAffectDissolve");
        DrawProp(e,p,"_SpeedLineFlowX");
        DrawProp(e,p,"_SpeedLineFlowY");
        DrawProp(e,p,"_SpeedLineSpeed");
        DrawProp(e,p,"_SpeedLineRotate");
        DrawProp(e,p,"_SpeedLineRotateSpeed");
    }

    private void DrawCustomDataInfo(MaterialEditor e, MaterialProperty[] p, Material m)
    {
        g12 = DrawCardFoldout("10. Custom Data / 粒子控制", g12, "不变");
        if(!g12) return;

        EditorGUILayout.HelpBox(
            "Custom Vertex Streams配置说明：\n" +
            "Custom1.X = 主贴图Offset X\n" +
            "Custom1.Y = 主贴图Offset Y\n" +
            "Custom1.Z = 扭曲强度\n" +
            "Custom1.W = 溶解贴图Offset\n" +
            "Custom2.X = 遮罩Offset X\n" +
            "Custom2.Y = 遮罩Offset Y\n" +
            "Custom2.Z = 溶解进度\n" +
            "Custom2.W = 顶点偏移强度",
            MessageType.Info
        );

        if(GUILayout.Button("一键配置选中粒子的Custom Vertex Streams", GUILayout.Height(24)))
        {
            SetupParticleCustomVertexStreams();
        }
    }

    private void SectionTitle(string title)
    {
        EditorGUILayout.Space(8);
        Rect row = EditorGUILayout.GetControlRect(false, 22f);
        EditorGUI.DrawRect(row, new Color(0.17f, 0.17f, 0.17f, 1f));
        EditorGUI.LabelField(
            new Rect(row.x + 8f, row.y + 3f, row.width - 16f, row.height - 4f),
            title,
            EditorStyles.miniBoldLabel
        );
    }

    private void DrawTextureFlow(MaterialEditor e, MaterialProperty[] p, Material m, string prefix, string label)
    {
        DrawProp(e,p,prefix+"UseFlow");
        if(Get(m, prefix+"UseFlow")>0.5f)
        {
            DrawProp(e,p,prefix+"FlowX");
            DrawProp(e,p,prefix+"FlowY");
            DrawProp(e,p,prefix+"FlowSpeed");
        }
    }

    private void DrawTextureUVOptions(MaterialEditor e, MaterialProperty[] p, Material m, string prefix, string label)
    {
        EditorGUILayout.Space(2);
        EditorGUILayout.LabelField(label + " UV扩展", EditorStyles.boldLabel);
        Popup(m, prefix+"WrapModeX", "Wrap X轴", wrapNames, null);
        Popup(m, prefix+"WrapModeY", "Wrap Y轴", wrapNames, null);

        DrawProp(e,p,prefix+"UsePolar");
        DrawProp(e,p,prefix+"UseRotate");

        if(Get(m, prefix+"UsePolar") > 0.5f || Get(m, prefix+"UseRotate") > 0.5f)
        {
            DrawProp(e,p,prefix+"UVCenterX");
            DrawProp(e,p,prefix+"UVCenterY");
        }

        if(Get(m, prefix+"UseRotate") > 0.5f)
        {
            DrawProp(e,p,prefix+"RotateAngle");
            DrawProp(e,p,prefix+"RotateSpeed");
        }
    }

    private Rect GetSafeControlRect(bool hasLabel, float height)
    {
        Rect row = EditorGUILayout.GetControlRect(hasLabel, height);

        // EditorWindow 内部的 ScrollView 会在右侧占用垂直滚动条宽度。
        // 给所有可编辑控件预留固定安全边距，避免数值框、贴图框、重置按钮被压到滚动条下或触发横向溢出。
        row.width = Mathf.Max(1f, row.width - ScrollbarSafetyWidth);
        return row;
    }

    private void DrawShaderPropertySafe(MaterialEditor editor, Rect rect, MaterialProperty property, string label)
    {
        float oldLabelWidth = EditorGUIUtility.labelWidth;
        float oldFieldWidth = EditorGUIUtility.fieldWidth;

        EditorGUIUtility.labelWidth = Mathf.Clamp(rect.width * 0.24f, 96f, 170f);
        EditorGUIUtility.fieldWidth = 50f;

        editor.ShaderProperty(rect, property, label);

        EditorGUIUtility.labelWidth = oldLabelWidth;
        EditorGUIUtility.fieldWidth = oldFieldWidth;
    }

    private void DrawProp(MaterialEditor e, MaterialProperty[] p, string name)
    {
        MaterialProperty prop = FindProperty(name, p, false);
        if(prop == null) return;

        if(prop.type == MaterialProperty.PropType.Texture)
        {
            DrawTexturePropSquare(prop);
            return;
        }

        float rowHeight = EditorGUIUtility.singleLineHeight + 2f;

        if(prop.type == MaterialProperty.PropType.Float || prop.type == MaterialProperty.PropType.Range)
        {
            Rect row = GetSafeControlRect(true, rowHeight);
            float resetWidth = Mathf.Min(20f, Mathf.Max(16f, row.width * 0.08f));
            Rect valueRect = new Rect(row.x, row.y, Mathf.Max(72f, row.width - resetWidth - 4f), row.height);
            Rect resetRect = new Rect(valueRect.xMax + 4f, row.y + 1f, resetWidth, row.height - 2f);

            bool kFrameHandled = AruiShader10KFrameConsoleBridgeURP.TryDrawProperty(prop, valueRect, prop.displayName);
            if(!kFrameHandled)
            {
                DrawShaderPropertySafe(e, valueRect, prop, prop.displayName);
            }

            if(GUI.Button(resetRect, new GUIContent("↺", "恢复默认值"), EditorStyles.miniButton))
            {
                if(kFrameHandled)
                {
                    AruiShader10KFrameConsoleBridgeURP.ResetToMaterialBase(prop.name, prop.type, prop.floatValue);
                }
                else
                {
                    ResetFloatPropertyToDefault(e, prop.name);
                }
            }
            return;
        }

        Rect normalRow = GetSafeControlRect(true, rowHeight);
        if(!AruiShader10KFrameConsoleBridgeURP.TryDrawProperty(prop, normalRow, prop.displayName))
        {
            DrawShaderPropertySafe(e, normalRow, prop, prop.displayName);
        }
    }

    private Vector2 DrawSafeVector2Row(Rect rect, string label, Vector2 value)
    {
        float gap = rect.width < 260f ? 2f : 4f;
        float mainLabelWidth = Mathf.Clamp(rect.width * 0.16f, 38f, 52f);
        float axisLabelWidth = rect.width < 260f ? 12f : 16f;

        float available = rect.width - mainLabelWidth - gap * 3f;
        float axisFieldWidth = Mathf.Max(46f, available * 0.5f);

        Rect mainLabelRect = new Rect(rect.x, rect.y + 1f, mainLabelWidth, rect.height - 2f);
        Rect xRect = new Rect(mainLabelRect.xMax + gap, rect.y, axisFieldWidth, rect.height);
        Rect yRect = new Rect(xRect.xMax + gap, rect.y, axisFieldWidth, rect.height);

        EditorGUI.LabelField(mainLabelRect, label, EditorStyles.miniLabel);

        float oldLabelWidth = EditorGUIUtility.labelWidth;
        float oldFieldWidth = EditorGUIUtility.fieldWidth;
        EditorGUIUtility.labelWidth = axisLabelWidth;
        EditorGUIUtility.fieldWidth = Mathf.Max(24f, axisFieldWidth - axisLabelWidth - 2f);

        // FloatField 自带可拖拽标签：直接拖动 X / Y 可以连续调整数值。
        value.x = EditorGUI.FloatField(xRect, new GUIContent("X", "拖动 X 标签可连续调整数值"), value.x);
        value.y = EditorGUI.FloatField(yRect, new GUIContent("Y", "拖动 Y 标签可连续调整数值"), value.y);

        EditorGUIUtility.labelWidth = oldLabelWidth;
        EditorGUIUtility.fieldWidth = oldFieldWidth;
        return value;
    }

    private void DrawTexturePropSquare(MaterialProperty prop)
    {
        float line = EditorGUIUtility.singleLineHeight;
        float previewSize = 58f;
        float height = 116f;
        Rect row = GetSafeControlRect(false, height);

        bool oldMixedValue = EditorGUI.showMixedValue;
        EditorGUI.showMixedValue = prop.hasMixedValue;

        float rightX = row.x + previewSize + 8f;
        float rightWidth = Mathf.Max(120f, row.xMax - rightX);

        Rect titleRect = new Rect(row.x, row.y + 2f, row.width, line);
        Rect previewRect = new Rect(row.x, row.y + 26f, previewSize, previewSize);
        Rect objectRect = new Rect(rightX, row.y + 26f, rightWidth, line);
        Rect tilingRect = new Rect(rightX, row.y + 51f, rightWidth, line);
        Rect offsetRect = new Rect(rightX, row.y + 77f, rightWidth, line);

        EditorGUI.LabelField(titleRect, prop.displayName);

        Texture currentTexture;
        bool kFrameTexture = AruiShader10KFrameConsoleBridgeURP.TryGetTextureValue(prop.name, prop.textureValue, out currentTexture);

        EditorGUI.BeginChangeCheck();
        Texture nextTexture = (Texture)EditorGUI.ObjectField(previewRect, currentTexture, typeof(Texture), false);
        bool textureChanged = EditorGUI.EndChangeCheck();

        EditorGUI.ObjectField(
            objectRect,
            new GUIContent(currentTexture != null ? currentTexture.name : "未设置贴图"),
            currentTexture,
            typeof(Texture),
            false
        );

        Vector4 currentST;
        bool kFrameVector = AruiShader10KFrameConsoleBridgeURP.TryGetVectorValue(prop.name + "_ST", prop.textureScaleAndOffset, out currentST);
        Vector2 tiling = new Vector2(currentST.x, currentST.y);
        Vector2 offset = new Vector2(currentST.z, currentST.w);

        EditorGUI.BeginChangeCheck();
        tiling = DrawSafeVector2Row(tilingRect, "Tiling", tiling);
        offset = DrawSafeVector2Row(offsetRect, "Offset", offset);
        bool transformChanged = EditorGUI.EndChangeCheck();

        if(textureChanged)
        {
            if(kFrameTexture)
            {
                AruiShader10KFrameConsoleBridgeURP.SetTextureTrackValue(prop.name, nextTexture, "控制台 Timeline K帧替换 " + prop.displayName);
            }
            else
            {
                prop.textureValue = nextTexture;
            }
        }

        if(transformChanged)
        {
            Vector4 nextST = new Vector4(tiling.x, tiling.y, offset.x, offset.y);
            if(kFrameVector)
            {
                AruiShader10KFrameConsoleBridgeURP.SetVectorTrackValue(prop.name + "_ST", nextST, "控制台 Timeline K帧调整 " + prop.displayName + " Tiling Offset");
            }
            else
            {
                prop.textureScaleAndOffset = nextST;
            }
        }

        EditorGUI.showMixedValue = oldMixedValue;
    }

    private void ResetFloatPropertyToDefault(MaterialEditor e, string propName)
    {
        if(e == null || e.targets == null) return;
        foreach(Object obj in e.targets)
        {
            Material mat = obj as Material;
            if(mat == null || mat.shader == null || !mat.HasProperty(propName)) continue;

            Material defaultMat = new Material(mat.shader);
            if(defaultMat.HasProperty(propName))
            {
                mat.SetFloat(propName, defaultMat.GetFloat(propName));
                EditorUtility.SetDirty(mat);
            }
            Object.DestroyImmediate(defaultMat);
        }
    }

    private float Get(Material m, string n){ return m != null && m.HasProperty(n) ? m.GetFloat(n) : 0f; }
    private void Set(Material m, string n, float v){ if(m != null && m.HasProperty(n)) m.SetFloat(n,v); }
    private void SetColor(Material m, string n, Color c){ if(m != null && m.HasProperty(n)) m.SetColor(n,c); }

    private void ResetFloatPropertyToDefaultMaterial(Material m, string propName)
    {
        if(m == null || m.shader == null || !m.HasProperty(propName)) return;
        Material defaultMat = new Material(m.shader);
        if(defaultMat.HasProperty(propName))
        {
            m.SetFloat(propName, defaultMat.GetFloat(propName));
            EditorUtility.SetDirty(m);
        }
        Object.DestroyImmediate(defaultMat);
    }

    private void Popup(Material m, string prop, string label, string[] names, System.Action<int> onChanged)
    {
        int v = Mathf.Clamp(Mathf.RoundToInt(Get(m, prop)), 0, names.Length - 1);

        Rect row = GetSafeControlRect(true, EditorGUIUtility.singleLineHeight + 2f);
        float resetWidth = Mathf.Min(20f, Mathf.Max(16f, row.width * 0.08f));
        Rect valueRect = new Rect(row.x, row.y, Mathf.Max(72f, row.width - resetWidth - 4f), row.height);
        Rect resetRect = new Rect(valueRect.xMax + 4f, row.y + 1f, resetWidth, row.height - 2f);

        int nv = EditorGUI.Popup(valueRect, label, v, names);
        if(nv != v)
        {
            Set(m, prop, nv);
            if(onChanged != null) onChanged(nv);
        }

        if(GUI.Button(resetRect, new GUIContent("↺", "恢复默认值"), EditorStyles.miniButton))
        {
            ResetFloatPropertyToDefaultMaterial(m, prop);
            if(onChanged != null) onChanged(Mathf.RoundToInt(Get(m, prop)));
        }
    }

    private void PopupMapped(Material m, string prop, string label, string[] names, int[] values)
    {
        int current = Mathf.RoundToInt(Get(m, prop));
        int index = 0;
        for(int i = 0; i < values.Length; i++)
        {
            if(values[i] == current)
            {
                index = i;
                break;
            }
        }

        Rect row = GetSafeControlRect(true, EditorGUIUtility.singleLineHeight + 2f);
        int newIndex = EditorGUI.Popup(row, label, index, names);
        if(newIndex != index)
        {
            Set(m, prop, values[newIndex]);
        }
    }

    private void SyncBackFaceColorState(Material m)
    {
        if(m == null) return;

        // 启动三段色后自动关闭背面颜色控制，避免三段色与背面单色互相冲突。
        if(Get(m,"_UseThreeColor") > 0.5f && Get(m,"_UseBackFaceColor") > 0.5f)
        {
            Set(m,"_UseBackFaceColor",0f);
            EditorUtility.SetDirty(m);
        }
    }

    private void SetRenderQueue(Material m, int queue)
    {
        if(m == null) return;

        int safeQueue = Mathf.Clamp(queue, 2500, 5000);
        if(m.HasProperty("_RenderQueueValue"))
        {
            Set(m, "_RenderQueueValue", safeQueue);
        }

        m.SetOverrideTag("RenderType", "Transparent");
        if(m.renderQueue != safeQueue)
        {
            m.renderQueue = safeQueue;
        }

        EditorUtility.SetDirty(m);
    }

    private void ApplyDefaultRenderSettings(Material m)
    {
        if(m == null) return;

        // AruiShader1.0 稳定默认：Alpha透明、ZTest开启、Queue 3000、主贴图颜色混合=1。
        Set(m, "_BlendMode", 0);
        Set(m, "_ZTestToggle", 1);
        Set(m, "_ZTestMode", 4);
        Set(m, "_MainColorMix", 1);
        ApplyBlend(m, 0);
        ApplyZTestState(m);
        SetRenderQueue(m, 3000);
        SyncRenderState(m);
    }

    private void SyncAutomaticRenderRules(Material m)
    {
        if(m == null) return;

        // 速度线一键预设使用3300。关闭速度线后，仅当队列仍为速度线专用3300时还原默认3000。
        if(Get(m, "_UseSpeedLine") < 0.5f && Mathf.RoundToInt(Get(m, "_RenderQueueValue")) == 3300)
        {
            SetRenderQueue(m, 3000);
        }
    }

    private void DrawSpeedLineToggle(MaterialEditor e, MaterialProperty[] p, Material m)
    {
        MaterialProperty prop = FindProperty("_UseSpeedLine", p, false);
        if(prop == null) return;

        EditorGUI.BeginChangeCheck();
        e.ShaderProperty(prop, prop.displayName);
        if(EditorGUI.EndChangeCheck())
        {
            if(prop.floatValue < 0.5f)
            {
                // 关闭速度线后，将速度线预设使用的3300恢复到默认队列3000。
                if(Mathf.RoundToInt(Get(m, "_RenderQueueValue")) == 3300)
                {
                    SetRenderQueue(m, 3000);
                }
            }
            EditorUtility.SetDirty(m);
        }
    }

    private void SyncRenderState(Material m)
    {
        if(m == null) return;

        int mode = Mathf.Clamp(Mathf.RoundToInt(Get(m,"_BlendMode")), 0, 3);
        if(mode == 0)
        {
            Set(m,"_SrcBlend",(float)BlendMode.SrcAlpha);
            Set(m,"_DstBlend",(float)BlendMode.OneMinusSrcAlpha);
        }
        else if(mode == 1)
        {
            Set(m,"_SrcBlend",(float)BlendMode.SrcAlpha);
            Set(m,"_DstBlend",(float)BlendMode.One);
        }
        else if(mode == 2)
        {
            Set(m,"_SrcBlend",(float)BlendMode.OneMinusDstColor);
            Set(m,"_DstBlend",(float)BlendMode.One);
        }
        else
        {
            Set(m,"_SrcBlend",(float)BlendMode.One);
            Set(m,"_DstBlend",(float)BlendMode.OneMinusSrcAlpha);
        }

        ApplyZWrite(m);
        ApplyZTestState(m);
        ApplyDoubleSided(m);
        ApplyStencilState(m);

        // 稳定版不再每次OnGUI无条件重写真实队列，只在当前队列无效时修复一次。
        m.SetOverrideTag("RenderType", "Transparent");
        int desiredQueue = Mathf.Clamp(Mathf.RoundToInt(Get(m,"_RenderQueueValue")), 2500, 5000);
        if(m.renderQueue < 2500 || m.renderQueue > 5000)
        {
            SetRenderQueue(m, desiredQueue);
        }
    }

    private void ApplyBlend(Material m, int mode)
    {
        if(m == null) return;

        Set(m,"_BlendMode",mode);
        if(mode == 0)
        {
            Set(m,"_SrcBlend",(float)BlendMode.SrcAlpha);
            Set(m,"_DstBlend",(float)BlendMode.OneMinusSrcAlpha);
        }
        else if(mode == 1)
        {
            Set(m,"_SrcBlend",(float)BlendMode.SrcAlpha);
            Set(m,"_DstBlend",(float)BlendMode.One);
        }
        else if(mode == 2)
        {
            Set(m,"_SrcBlend",(float)BlendMode.OneMinusDstColor);
            Set(m,"_DstBlend",(float)BlendMode.One);
        }
        else
        {
            Set(m,"_SrcBlend",(float)BlendMode.One);
            Set(m,"_DstBlend",(float)BlendMode.OneMinusSrcAlpha);
        }

        ApplyZWrite(m);
        ApplyDoubleSided(m);
        ApplyStencilState(m);
        m.SetOverrideTag("RenderType", "Transparent");

        int desiredQueue = Mathf.Clamp(Mathf.RoundToInt(Get(m,"_RenderQueueValue")), 2500, 5000);
        SetRenderQueue(m, desiredQueue);
        EditorUtility.SetDirty(m);
    }

    private void ApplyDoubleSided(Material m)
    {
        if(m == null) return;

        // 显示面模式：0=双面显示(Cull Off)，1=显示正面(Cull Back)，2=显示背面(Cull Front)
        int mode = Mathf.Clamp(Mathf.RoundToInt(Get(m,"_CullDisplayMode")), 0, 2);
        if(mode == 0)
        {
            Set(m,"_CullMode",0f);
            Set(m,"_DoubleSided",1f);
        }
        else if(mode == 1)
        {
            Set(m,"_CullMode",2f);
            Set(m,"_DoubleSided",0f);
        }
        else
        {
            Set(m,"_CullMode",1f);
            Set(m,"_DoubleSided",0f);
        }
    }

    private void ApplyZWrite(Material m)
    {
        if(m == null) return;
        Set(m,"_ZWrite", Get(m,"_ZWriteToggle") > 0.5f ? 1f : 0f);
    }

    private void ApplyZTestState(Material m)
    {
        if(m == null) return;
        // 勾选 = LEqual正常深度测试；不勾选 = Always无视深度显示
        Set(m,"_ZTestMode", Get(m,"_ZTestToggle") > 0.5f ? 4f : 8f);
    }

    private void ApplyStencilState(Material m)
    {
        if(m == null) return;
        if(Get(m,"_UseStencil") < 0.5f)
        {
            Set(m,"_StencilComp",8f);
            Set(m,"_StencilPass",0f);
            Set(m,"_StencilFail",0f);
            Set(m,"_StencilZFail",0f);
            Set(m,"_StencilRef",0f);
            Set(m,"_StencilReadMask",255f);
            Set(m,"_StencilWriteMask",255f);
        }
    }

    private void FixVisible(Material m)
    {
        if(m == null) return;
        Set(m,"_UseMainLayer",1);
        Set(m,"_UseAlphaModule",1);
        Set(m,"_UseMainAlpha",1);
        Set(m,"_MainShapeSource",2);
        Set(m,"_Opacity",1);
        Set(m,"_EmissionIntensity",0);
        Set(m,"_MainContrast",1);
        Set(m,"_MainPower",1);
        Set(m,"_MainColorMix",1);
        Set(m,"_UseCDMainOffset",0);
        Set(m,"_OutputMode",0);
        ApplyDefaultRenderSettings(m);
        Set(m,"_QuadMaskRadius",1.2f);
        Set(m,"_QuadMaskSoftness",0.08f);
        Set(m,"_QuadMaskScaleX",1);
        Set(m,"_QuadMaskScaleY",1);
        Set(m,"_DebugMode",0);
        EditorUtility.SetDirty(m);
    }

    private void FixAlpha(Material m)
    {
        if(m == null) return;
        Set(m,"_UseAlphaModule",1);
        Set(m,"_UseMainAlpha",1);
        Set(m,"_MainShapeSource",2);
        Set(m,"_AlphaPower",1);
        Set(m,"_Opacity",1);
        Set(m,"_AlphaClipEnable",1);
        if(Get(m,"_AlphaClip") < 0.005f) Set(m,"_AlphaClip",0.015f);
        EditorUtility.SetDirty(m);
    }

    private void FixDissolve(Material m)
    {
        if(m == null) return;
        Set(m,"_UseDissolveModule",1);
        if(Get(m,"_DissolveContrast") < 0.5f) Set(m,"_DissolveContrast",1f);
        if(Get(m,"_DissolveSoftness") < 0.01f) Set(m,"_DissolveSoftness",0.08f);
        if(Get(m,"_DissolveEdgeWidth") <= 0.001f) Set(m,"_DissolveEdgeWidth",0.06f);
        if(Get(m,"_DissolveEdgeIntensity") <= 0.001f) Set(m,"_DissolveEdgeIntensity",1f);
        EditorUtility.SetDirty(m);
    }

    private void FixFresnel(Material m)
    {
        if(m == null) return;
        Set(m,"_UseFresnelModule",1);
        if(Get(m,"_FresnelIntensity") <= 0.001f) Set(m,"_FresnelIntensity",1f);
        if(Get(m,"_FresnelBrightness") <= 0.001f) Set(m,"_FresnelBrightness",1f);
        if(Get(m,"_FresnelThickness") < 0.05f) Set(m,"_FresnelThickness",0.45f);
        if(Get(m,"_FresnelPower") < 0.5f) Set(m,"_FresnelPower",3f);
        EditorUtility.SetDirty(m);
    }

    private void FixDistort(Material m)
    {
        if(m == null) return;
        Set(m,"_UseDistortModule",1);
        if(Get(m,"_DistortStrength") <= 0.001f) Set(m,"_DistortStrength",0.08f);
        if(Get(m,"_DistortGradientStep") <= 0.0005f) Set(m,"_DistortGradientStep",0.01f);
        Set(m,"_UseEdgeHeat",1);
        if(Get(m,"_HeatAffectMain") <= 0.001f) Set(m,"_HeatAffectMain",1);
        if(Get(m,"_EdgeHeatStrength") <= 0.001f) Set(m,"_EdgeHeatStrength",0.35f);
        EditorUtility.SetDirty(m);
    }

    private void FixStencil(Material m)
    {
        if(m == null) return;
        Set(m,"_UseStencil",1);
        if(Get(m,"_StencilRef") <= 0) Set(m,"_StencilRef",1);
        Set(m,"_StencilComp",8);
        Set(m,"_StencilPass",0);
        Set(m,"_StencilFail",0);
        Set(m,"_StencilZFail",0);
        Set(m,"_StencilReadMask",255);
        Set(m,"_StencilWriteMask",255);
        EditorUtility.SetDirty(m);
    }

    private void SetStencilRefIfZero(Material m)
    {
        if(m == null) return;
        if(Get(m,"_StencilRef") <= 0) Set(m,"_StencilRef",1);
    }

    private void StencilPresetOff(Material m)
    {
        if(m == null) return;
        Set(m,"_UseStencil",0);
        Set(m,"_StencilComp",8);      // Always
        Set(m,"_StencilPass",0);      // Keep
        Set(m,"_StencilFail",0);      // Keep
        Set(m,"_StencilZFail",0);     // Keep
        Set(m,"_StencilRef",0);
        Set(m,"_StencilReadMask",255);
        Set(m,"_StencilWriteMask",255);
        ApplyStencilState(m);
        EditorUtility.SetDirty(m);
    }

    private void StencilPresetWriteMask(Material m)
    {
        if(m == null) return;
        Set(m,"_UseStencil",1);
        SetStencilRefIfZero(m);
        Set(m,"_StencilComp",8);      // Always
        Set(m,"_StencilPass",2);      // Replace
        Set(m,"_StencilFail",0);      // Keep
        Set(m,"_StencilZFail",0);     // Keep
        Set(m,"_StencilReadMask",255);
        Set(m,"_StencilWriteMask",255);
        EditorUtility.SetDirty(m);
    }

    private void StencilPresetReadInside(Material m)
    {
        if(m == null) return;
        Set(m,"_UseStencil",1);
        SetStencilRefIfZero(m);
        Set(m,"_StencilComp",3);      // Equal
        Set(m,"_StencilPass",0);      // Keep
        Set(m,"_StencilFail",0);      // Keep
        Set(m,"_StencilZFail",0);     // Keep
        Set(m,"_StencilReadMask",255);
        Set(m,"_StencilWriteMask",0);

        // 只显示遮罩内通常需要压在遮罩写入层之后绘制。
        // 因此自动使用透明队列3001，避免与写入遮罩的材质排序冲突。
        SetRenderQueue(m, 3001);
        EditorUtility.SetDirty(m);
    }

    private void StencilPresetReadOutside(Material m)
    {
        if(m == null) return;
        Set(m,"_UseStencil",1);
        SetStencilRefIfZero(m);
        Set(m,"_StencilComp",6);      // NotEqual
        Set(m,"_StencilPass",0);      // Keep
        Set(m,"_StencilFail",0);      // Keep
        Set(m,"_StencilZFail",0);     // Keep
        Set(m,"_StencilReadMask",255);
        Set(m,"_StencilWriteMask",0);
        EditorUtility.SetDirty(m);
    }

    private void StencilPresetEraseMask(Material m)
    {
        if(m == null) return;
        Set(m,"_UseStencil",1);
        SetStencilRefIfZero(m);
        Set(m,"_StencilComp",8);      // Always
        Set(m,"_StencilPass",1);      // Zero
        Set(m,"_StencilFail",0);      // Keep
        Set(m,"_StencilZFail",0);     // Keep
        Set(m,"_StencilReadMask",255);
        Set(m,"_StencilWriteMask",255);
        EditorUtility.SetDirty(m);
    }

    private void StencilPresetIncrementMask(Material m)
    {
        if(m == null) return;
        Set(m,"_UseStencil",1);
        SetStencilRefIfZero(m);
        Set(m,"_StencilComp",8);      // Always
        Set(m,"_StencilPass",3);      // IncrSat
        Set(m,"_StencilFail",0);      // Keep
        Set(m,"_StencilZFail",0);     // Keep
        Set(m,"_StencilReadMask",255);
        Set(m,"_StencilWriteMask",255);
        EditorUtility.SetDirty(m);
    }

    private void FixAlphaBlendMode(Material m)
    {
        if(m == null) return;
        ApplyBlend(m, 0);
        Set(m,"_Opacity",1);
        Set(m,"_ZWriteToggle",0);
        Set(m,"_ZTestToggle",1); Set(m,"_ZTestMode",4);
        ApplyZWrite(m);
        Set(m,"_UseAlphaModule",1);
        Set(m,"_UseMainAlpha",1);
        Set(m,"_MainShapeSource",2);
        Set(m,"_DebugMode",0);
        EditorUtility.SetDirty(m);
    }

    private void FixAddBlendMode(Material m)
    {
        if(m == null) return;
        ApplyBlend(m, 1);
        Set(m,"_Opacity",1);
        Set(m,"_ZWriteToggle",0);
        Set(m,"_ZTestToggle",1); Set(m,"_ZTestMode",4);
        ApplyZWrite(m);
        Set(m,"_AlphaClipEnable",0);
        Set(m,"_DebugMode",0);
        EditorUtility.SetDirty(m);
    }

    private void ResetUVParams(Material m)
    {
        if(m == null) return;
        string[] prefixes = {"_Main","_Sub","_Mask","_Ramp","_EdgeRamp","_FresnelRamp","_Dissolve","_Distort","_Vertex"};
        foreach(string p in prefixes)
        {
            Set(m,p+"UseFlow",0);
            Set(m,p+"FlowX",0);
            Set(m,p+"FlowY",0);
            Set(m,p+"FlowSpeed",1);
            Set(m,p+"WrapModeX",0);
            Set(m,p+"WrapModeY",0);
            Set(m,p+"UsePolar",0);
            Set(m,p+"UseRotate",0);
            Set(m,p+"RotateAngle",0);
            Set(m,p+"RotateSpeed",0);
            Set(m,p+"UVCenterX",0.5f);
            Set(m,p+"UVCenterY",0.5f);
        }

        Set(m,"_UVSwirlStrength",0);
        Set(m,"_UVWaveStrength",0);
        Set(m,"_UVRadialStrength",0);
        Set(m,"_UVKaleidoscope",0);
        Set(m,"_UVCenterX",0.5f);
        Set(m,"_UVCenterY",0.5f);
        Set(m,"_MainUseHueSplit",0);
        Set(m,"_SubUseHueSplit",0);
        EditorUtility.SetDirty(m);
    }

    private void ResetColorParams(Material m)
    {
        if(m == null) return;
        Set(m,"_UseThreeColor",0);
        SetColor(m,"_ColorA",Color.white);
        SetColor(m,"_ColorB",Color.white);
        SetColor(m,"_ColorC",Color.white);
        SetColor(m,"_SingleColor",Color.white);
        SetColor(m,"_SubColor",Color.white);
        SetColor(m,"_EdgeOnlyColor",Color.white);
        SetColor(m,"_DissolveEdgeColor",Color.white);
        SetColor(m,"_DissolveOuterEdgeColor",Color.white);
        SetColor(m,"_FresnelColor",Color.white);
        SetColor(m,"_FresnelSecondColor",Color.white);
        Set(m,"_EmissionIntensity",0);
        Set(m,"_SubColorStrength",0);
        Set(m,"_SubColorIntensity",0);
        EditorUtility.SetDirty(m);
    }

    private void BasicVisible(Material m)
    {
        if(m == null) return;
        FixVisible(m);
        ApplyBlend(m,0);
        Set(m,"_UseMainLayer",1);
        Set(m,"_UseAlphaModule",1);
        Set(m,"_UseMainAlpha",1);
        Set(m,"_Opacity",1);
        Set(m,"_DebugMode",0);
        Set(m,"_OutputMode",0);
        EditorUtility.SetDirty(m);
    }

    private void ApplyTemplate(Material m, int type)
    {
        if(m == null) return;

        // 颜色模板只修改颜色，不关闭功能，不改变混合模式，不改变颜色强度。
        Set(m,"_UseThreeColor",1);

        Color a = Color.white;
        Color b = Color.white;
        Color c = Color.white;
        Color edge = Color.white;
        Color fresnel = Color.white;

        // 0 火焰
        if(type == 0)
        {
            a = new Color(0.55f,0.05f,0.01f,1);
            b = new Color(1.00f,0.32f,0.02f,1);
            c = new Color(1.00f,0.85f,0.12f,1);
            edge = new Color(1.00f,0.55f,0.04f,1);
            fresnel = new Color(1.00f,0.35f,0.02f,1);
        }
        // 1 冰霜
        else if(type == 1)
        {
            a = new Color(0.04f,0.25f,0.55f,1);
            b = new Color(0.20f,0.80f,1.00f,1);
            c = new Color(0.85f,1.00f,1.00f,1);
            edge = new Color(0.30f,0.95f,1.00f,1);
            fresnel = new Color(0.45f,0.95f,1.00f,1);
        }
        // 2 水
        else if(type == 2)
        {
            a = new Color(0.02f,0.18f,0.45f,1);
            b = new Color(0.05f,0.55f,1.00f,1);
            c = new Color(0.45f,0.95f,1.00f,1);
            edge = new Color(0.10f,0.75f,1.00f,1);
            fresnel = new Color(0.20f,0.80f,1.00f,1);
        }
        // 3 绿毒气
        else if(type == 3)
        {
            a = new Color(0.04f,0.18f,0.02f,1);
            b = new Color(0.25f,0.80f,0.06f,1);
            c = new Color(0.85f,1.00f,0.15f,1);
            edge = new Color(0.45f,1.00f,0.08f,1);
            fresnel = new Color(0.35f,0.95f,0.10f,1);
        }
        // 4 紫毒气
        else if(type == 4)
        {
            a = new Color(0.12f,0.02f,0.20f,1);
            b = new Color(0.55f,0.08f,0.85f,1);
            c = new Color(1.00f,0.25f,0.95f,1);
            edge = new Color(0.85f,0.15f,1.00f,1);
            fresnel = new Color(0.80f,0.12f,1.00f,1);
        }
        // 5 治疗
        else if(type == 5)
        {
            a = new Color(0.02f,0.22f,0.12f,1);
            b = new Color(0.20f,1.00f,0.55f,1);
            c = new Color(0.85f,1.00f,0.65f,1);
            edge = new Color(0.35f,1.00f,0.55f,1);
            fresnel = new Color(0.35f,1.00f,0.50f,1);
        }
        // 6 物理
        else if(type == 6)
        {
            a = new Color(0.18f,0.12f,0.07f,1);
            b = new Color(0.70f,0.48f,0.25f,1);
            c = new Color(1.00f,0.86f,0.55f,1);
            edge = new Color(1.00f,0.72f,0.35f,1);
            fresnel = new Color(1.00f,0.65f,0.28f,1);
        }
        // 7 圣光
        else if(type == 7)
        {
            a = new Color(0.42f,0.30f,0.06f,1);
            b = new Color(1.00f,0.88f,0.22f,1);
            c = new Color(1.00f,1.00f,0.82f,1);
            edge = new Color(1.00f,0.95f,0.45f,1);
            fresnel = new Color(1.00f,0.90f,0.35f,1);
        }
        // 8 血液
        else
        {
            a = new Color(0.16f,0.00f,0.00f,1);
            b = new Color(0.65f,0.02f,0.02f,1);
            c = new Color(1.00f,0.10f,0.05f,1);
            edge = new Color(0.95f,0.05f,0.03f,1);
            fresnel = new Color(0.90f,0.04f,0.04f,1);
        }

        SetColor(m,"_ColorA",a);
        SetColor(m,"_ColorB",b);
        SetColor(m,"_ColorC",c);
        SetColor(m,"_SingleColor",b);
        SetColor(m,"_DissolveEdgeColor",edge);
        SetColor(m,"_DissolveOuterEdgeColor",c);
        SetColor(m,"_EdgeOnlyColor",edge);
        SetColor(m,"_FresnelColor",fresnel);
        SetColor(m,"_FresnelSecondColor",c);

        Set(m,"_ColorSplit1",0.35f);
        Set(m,"_ColorSplit2",0.68f);
        Set(m,"_ColorSoftness",0.08f);

        Set(m,"_OutputMode",0);
        Set(m,"_DebugMode",0);
        SyncRenderState(m);
        EditorUtility.SetDirty(m);
    }

    private void ApplyMaterialType(Material m)
    {
        if(m == null) return;
        int keepType = Mathf.RoundToInt(Get(m,"_MaterialType"));
        int keepPerf = Mathf.RoundToInt(Get(m,"_PerformanceMode"));

        Set(m,"_UseMainLayer",1); Set(m,"_UseAlphaModule",1);
        Set(m,"_UseSubLayer",0); Set(m,"_MainUseBlur",0); Set(m,"_UseMaskLayer",0); Set(m,"_UseRampModule",0); Set(m,"_UseUVModule",0);
        Set(m,"_UseDissolveModule",0); Set(m,"_UseEdgeModule",0); Set(m,"_UseFresnelModule",0); Set(m,"_UseDistortModule",0); Set(m,"_UseVertexModule",0); Set(m,"_UseVAT",0); Set(m,"_VertexUseAnimMode",0);
if(keepType == 0)
        {
            ApplyBlend(m,0); Set(m,"_UseDissolveModule",1); Set(m,"_UseLayerDissolve",1); Set(m,"_UseDirectionDissolve",1); Set(m,"_UseVertexModule",1); 
        }
        else if(keepType == 1)
        {
            ApplyBlend(m,0); Set(m,"_UseSoftParticles",1); Set(m,"_UseVertexModule",1); Set(m,"_EmissionIntensity",0); 
        }
        else if(keepType == 2)
        {
            ApplyBlend(m,1); Set(m,"_UseUVModule",1); Set(m,"_UseFresnelModule",1); Set(m,"_UseEdgeModule",1); Set(m,"_UseSubLayer",1); Set(m,"_UseRampModule",1);
        }
        else if(keepType == 3 || keepType == 4)
        {
            ApplyBlend(m,1); Set(m,"_ShapeMode",1); Set(m,"_UseDissolveModule",1); Set(m,"_UseDirectionDissolve",1); Set(m,"_UseEdgeModule",1);
        }
        else if(keepType == 5 || keepType == 6)
        {
            ApplyBlend(m,1); Set(m,"_ShapeMode",1); Set(m,"_UseUVModule",1); Set(m,"_UseDissolveModule",1); Set(m,"_UseRampModule",1);
        }
        else if(keepType == 7)
        {
            ApplyBlend(m,0); Set(m,"_UseEdgeModule",1); Set(m,"_UseFresnelModule",1); Set(m,"_UseSubLayer",1);
        }
        else if(keepType == 8)
        {
            ApplyBlend(m,1); Set(m,"_UseUVModule",1); Set(m,"_UVSwirlStrength",2.5f); Set(m,"_UseSubLayer",1); Set(m,"_UseRampModule",1);
        }
        else if(keepType == 9)
        {
            ApplyBlend(m,0); Set(m,"_OutputMode",2); Set(m,"_UseDistortModule",1);
        }
        else if(keepType == 10)
        {
            ApplyBlend(m,1); Set(m,"_UseThreeColor",0); SetColor(m,"_SingleColor",Color.white); Set(m,"_EmissionIntensity",3);
        }

        Set(m,"_MaterialType",keepType);
        Set(m,"_PerformanceMode",keepPerf);
        EditorUtility.SetDirty(m);
    }

    private string GetPerformanceDesc(int mode)
    {
        if(mode == 0)
            return "高品质：允许使用全部功能。适合PC、主角技能、作品集展示。切到此模式不会强制关闭功能。";
        if(mode == 1)
            return "标准：保留主贴图、Alpha、遮罩、Ramp、溶解、边缘、菲尼尔、基础扭曲；自动关闭顶点偏移、双层菲尼尔、复杂Ramp和FlowMap。";
        if(mode == 2)
            return "移动端：保留主贴图、Alpha、基础溶解、基础边缘；自动关闭副贴图、Ramp、复杂UV、FlowMap、扭曲、菲尼尔、顶点偏移、软粒子。";
        return "极简：只保留主贴图、颜色、Alpha。自动关闭所有高级模块，用于大量低成本粒子。";
    }

    private void ApplyPerformanceMode(Material m)
    {
        if(m == null) return;
        int keepPerf = Mathf.Clamp(Mathf.RoundToInt(Get(m,"_PerformanceMode")),0,3);

        if(keepPerf == 0)
        {
            // 高品质：不强制关闭，允许全部功能继续使用
        }
        else if(keepPerf == 1)
        {
            // 标准：关闭重功能，保留常用视觉模块
            Set(m,"_UseVertexModule",0);
        Set(m,"_UseVAT",0);
        Set(m,"_VertexUseAnimMode",0);
Set(m,"_UseSpeedLine",0);
        Set(m,"_UseParallax",0);
            Set(m,"_UseSecondFresnel",0);
            Set(m,"_UseEdgeRamp",0);
            Set(m,"_UseFresnelRamp",0);
            Set(m,"_UseFlowMap",0);
            Set(m,"_UseSoftParticles",0);
        }
        else if(keepPerf == 2)
        {
            // 移动端：关闭中高消耗功能
            Set(m,"_UseSubLayer",0);
            Set(m,"_UseRampModule",0);
            Set(m,"_UseUVModule",0);
            Set(m,"_UseDistortModule",0);
            Set(m,"_UseFlowMap",0);
            Set(m,"_UseFresnelModule",0);
            Set(m,"_UseEdgeRamp",0);
            Set(m,"_UseFresnelRamp",0);
            Set(m,"_UseSecondFresnel",0);
            Set(m,"_UseVertexModule",0);
            Set(m,"_UseSoftParticles",0);
            Set(m,"_UseCameraDistanceFade",0);
}
        else
        {
            // 极简：只保留主贴图、颜色、Alpha
            Set(m,"_UseSubLayer",0);
            Set(m,"_UseMaskLayer",0);
            Set(m,"_UseRampModule",0);
            Set(m,"_UseUVModule",0);
            Set(m,"_UseDissolveModule",0);
            Set(m,"_UseDirectionDissolve",0);
            Set(m,"_UseLayerDissolve",0);
            Set(m,"_UseEdgeModule",0);
            Set(m,"_UseFresnelModule",0);
            Set(m,"_UseDistortModule",0);
            Set(m,"_UseFlowMap",0);
            Set(m,"_UseVertexModule",0);
            Set(m,"_UseSoftParticles",0);
            Set(m,"_UseCameraDistanceFade",0);
}

        Set(m,"_PerformanceMode",keepPerf);
        EditorUtility.SetDirty(m);
    }

    private void AutoRepair(Material m)
    {
        if(m == null) return;
        ApplyBlend(m, Mathf.Clamp(Mathf.RoundToInt(Get(m,"_BlendMode")),0,3));
        ApplyDoubleSided(m);
        ApplyZWrite(m);
        ApplyStencilState(m);
        Set(m,"_DebugMode",0);
        Set(m,"_UseMainLayer",1);
        Set(m,"_UseAlphaModule",1);
        Set(m,"_MainShapeSource",2);
        Set(m,"_ShapeMode",1);
        Set(m,"_ShapeMode",1);
        Set(m,"_UseMainAlpha",1);
        Set(m,"_AlphaClipEnable",1);
        if(Get(m,"_AlphaClip")<0.005f) Set(m,"_AlphaClip",0.015f);
        EditorUtility.SetDirty(m);
        EditorUtility.DisplayDialog("体检完成","已修复：主贴图/透明PNG显示、混合模式、双面/深度/模板状态、面片Alpha裁剪、调试模式。","好的");
    }

    private class TextureState
    {
        public string name;
        public Texture texture;
        public Vector2 scale;
        public Vector2 offset;
    }

    private List<string> GetTexturePropertyNames(Material m)
    {
        List<string> result = new List<string>();
        if(m == null || m.shader == null) return result;

        int count = ShaderUtil.GetPropertyCount(m.shader);
        for(int i=0;i<count;i++)
        {
            if(ShaderUtil.GetPropertyType(m.shader, i) == ShaderUtil.ShaderPropertyType.TexEnv)
            {
                result.Add(ShaderUtil.GetPropertyName(m.shader, i));
            }
        }
        return result;
    }

    private List<TextureState> SaveAllTextureStates(Material m)
    {
        List<TextureState> list = new List<TextureState>();
        if(m == null) return list;

        foreach(string texName in GetTexturePropertyNames(m))
        {
            list.Add(SaveOneTextureState(m, texName));
        }
        return list;
    }

    private TextureState SaveOneTextureState(Material m, string texName)
    {
        TextureState state = new TextureState();
        state.name = texName;
        state.texture = null;
        state.scale = Vector2.one;
        state.offset = Vector2.zero;

        if(m != null && m.HasProperty(texName))
        {
            state.texture = m.GetTexture(texName);
            state.scale = m.GetTextureScale(texName);
            state.offset = m.GetTextureOffset(texName);
        }
        return state;
    }

    private void RestoreTextureStates(Material m, List<TextureState> list)
    {
        if(m == null || list == null) return;
        foreach(TextureState state in list)
        {
            RestoreOneTextureState(m, state);
        }
    }

    private void RestoreOneTextureState(Material m, TextureState state)
    {
        if(m == null || state == null || string.IsNullOrEmpty(state.name) || !m.HasProperty(state.name)) return;
        m.SetTexture(state.name, state.texture);
        m.SetTextureScale(state.name, state.scale);
        m.SetTextureOffset(state.name, state.offset);
    }

    private void ResetMaterialToShaderDefault(Material m)
    {
        if(m == null || m.shader == null) return;

        Material defaultMat = new Material(m.shader);
        m.CopyPropertiesFromMaterial(defaultMat);
        Object.DestroyImmediate(defaultMat);
    }

    private bool IsFeatureToggleName(string propName)
    {
        if(string.IsNullOrEmpty(propName)) return false;
        if(propName.StartsWith("_Use")) return true;
        if(propName.Contains("Use")) return true;
        return false;
    }

    private void ClearAllTexturesExceptMain(Material m)
    {
        if(m == null || m.shader == null) return;

        Texture mainTex = m.HasProperty("_MainTex") ? m.GetTexture("_MainTex") : null;
        Vector2 mainScale = m.HasProperty("_MainTex") ? m.GetTextureScale("_MainTex") : Vector2.one;
        Vector2 mainOffset = m.HasProperty("_MainTex") ? m.GetTextureOffset("_MainTex") : Vector2.zero;

        foreach(string texName in GetTexturePropertyNames(m))
        {
            if(texName == "_MainTex") continue;
            m.SetTexture(texName, null);
            m.SetTextureScale(texName, Vector2.one);
            m.SetTextureOffset(texName, Vector2.zero);
        }

        if(m.HasProperty("_MainTex"))
        {
            m.SetTexture("_MainTex", mainTex);
            m.SetTextureScale("_MainTex", mainScale);
            m.SetTextureOffset("_MainTex", mainOffset);
        }

        EditorUtility.SetDirty(m);
    }

    private void ClearParamsKeepTextures(Material m)
    {
        if(m == null || m.shader == null) return;

        // 覆盖所有功能：所有非贴图参数恢复Shader默认值，所有贴图与贴图Tiling/Offset保留。
        List<TextureState> textures = SaveAllTextureStates(m);
        ResetMaterialToShaderDefault(m);
        RestoreTextureStates(m, textures);
        ApplyDefaultRenderSettings(m);
        EditorUtility.SetDirty(m);
    }

    private void CloseAllFeaturesKeepMain(Material m)
    {
        if(m == null || m.shader == null) return;

        // 覆盖所有功能：先恢复默认，再关闭所有包含Use的功能开关，只保留主贴图和基础Alpha显示。
        TextureState mainState = SaveOneTextureState(m, "_MainTex");

        ResetMaterialToShaderDefault(m);
        ClearAllTexturesExceptMain(m);
        RestoreOneTextureState(m, mainState);

        int count = ShaderUtil.GetPropertyCount(m.shader);
        for(int i=0;i<count;i++)
        {
            string propName = ShaderUtil.GetPropertyName(m.shader, i);
            ShaderUtil.ShaderPropertyType type = ShaderUtil.GetPropertyType(m.shader, i);
            if((type == ShaderUtil.ShaderPropertyType.Float || type == ShaderUtil.ShaderPropertyType.Range) && IsFeatureToggleName(propName))
            {
                Set(m, propName, 0f);
            }
        }

        Set(m,"_UseMainLayer",1);
        Set(m,"_UseAlphaModule",1);
        Set(m,"_UseMainAlpha",1);
        Set(m,"_MainShapeSource",2);
        Set(m,"_Opacity",1);
        Set(m,"_EmissionIntensity",0);
        Set(m,"_OutputMode",0);
        Set(m,"_DebugMode",0);
        Set(m,"_CullDisplayMode",0);
        Set(m,"_DoubleSided",1);
        ApplyDefaultRenderSettings(m);
        ApplyStencilState(m);
        SyncRenderState(m);

        EditorUtility.SetDirty(m);
    }

    private void SetupParticleCustomVertexStreams()
    {
        GameObject go = Selection.activeGameObject;
        if(go == null)
        {
            EditorUtility.DisplayDialog("没有选中对象", "请先选中一个带有ParticleSystem的对象。", "好的");
            return;
        }

        ParticleSystemRenderer renderer = go.GetComponent<ParticleSystemRenderer>();
        if(renderer == null)
        {
            ParticleSystem ps = go.GetComponent<ParticleSystem>();
            if(ps != null) renderer = ps.GetComponent<ParticleSystemRenderer>();
        }

        if(renderer == null)
        {
            EditorUtility.DisplayDialog("未找到ParticleSystemRenderer", "选中对象上没有ParticleSystemRenderer。", "好的");
            return;
        }

        List<ParticleSystemVertexStream> streams = new List<ParticleSystemVertexStream>();
        streams.Add(ParticleSystemVertexStream.Position);
        streams.Add(ParticleSystemVertexStream.Normal);
        streams.Add(ParticleSystemVertexStream.Color);
        streams.Add(ParticleSystemVertexStream.UV);
        streams.Add(ParticleSystemVertexStream.UV2);
        streams.Add(ParticleSystemVertexStream.Custom1XYZW);
        streams.Add(ParticleSystemVertexStream.Custom2XYZW);

        renderer.SetActiveVertexStreams(streams);
        EditorUtility.SetDirty(renderer);
        EditorUtility.DisplayDialog("配置完成", "已为选中粒子加入：Position / Normal / Color / UV / UV2 / Custom1.xyzw / Custom2.xyzw。", "好的");
    }




    private void ResetAllParamsDefault(Material m)
    {
        if(m == null || m.shader == null) return;

        // 覆盖所有功能：恢复Shader默认材质状态，包含全部数值、颜色、贴图、Tiling/Offset和渲染状态。
        ResetMaterialToShaderDefault(m);
        ApplyDefaultRenderSettings(m);
        EditorUtility.SetDirty(m);
    }

    private void SetupSpeedLineRunStyle(Material m)
    {
        if(m == null) return;
        Set(m,"_UseSpeedLine",1);
        Set(m,"_SpeedLineMode",1);
        SetColor(m,"_SpeedLineColor",Color.white);
        Set(m,"_SpeedLineIntensity",3.5f);
        Set(m,"_SpeedLineAlpha",0.45f);
        Set(m,"_SpeedLineCenterX",0.5f);
        Set(m,"_SpeedLineCenterY",0.48f);
        Set(m,"_SpeedLineFlowX",0);
        Set(m,"_SpeedLineFlowY",-1);
        Set(m,"_SpeedLineSpeed",6);
        Set(m,"_SpeedLineRotate",0);
        Set(m,"_SpeedLineRotateSpeed",0);
        Set(m,"_SpeedLineEdgeStart",0.28f);
        Set(m,"_SpeedLineEdgeSoftness",0.18f);
        Set(m,"_SpeedLinePower",1.2f);
        Set(m,"_SpeedLineInvert",0);
        Set(m,"_SpeedLineAngleTiling",9);
        Set(m,"_SpeedLineRadialTiling",2.2f);
        Set(m,"_SpeedLineLength",2.4f);
        Set(m,"_SpeedLineCenterClear",0.24f);
        Set(m,"_SpeedLineCenterSoftness",0.22f);
        Set(m,"_SpeedLineAspectFix",1);
        Set(m,"_SpeedLineVignette",0.55f);
        Set(m,"_SpeedLineHoleShape",0);
        Set(m,"_SpeedLineHoleScaleX",1);
        Set(m,"_SpeedLineHoleScaleY",1);
        Set(m,"_SpeedLineAffectDistort",0);
        Set(m,"_SpeedLineAffectDissolve",0);
        Set(m,"_BlendMode",1);
        ApplyBlend(m,1);
        Set(m,"_ZTestToggle",0);
        Set(m,"_ZTestMode",8);
        Set(m,"_RenderQueueValue",3300);
        SyncRenderState(m);
        EditorUtility.SetDirty(m);
    }

    private Color RandomColorKeepIntensity(Material m, string propName)
    {
        Color old = (m != null && m.HasProperty(propName)) ? m.GetColor(propName) : Color.white;
        float keepIntensity = Mathf.Max(Mathf.Max(old.r, old.g), old.b);
        if(keepIntensity < 0.001f) keepIntensity = 1f;

        Color c = Color.HSVToRGB(Random.value, Random.Range(0.75f, 1.0f), 1f);
        c.r *= keepIntensity;
        c.g *= keepIntensity;
        c.b *= keepIntensity;
        c.a = old.a;
        return c;
    }


    private void RandomizeSelected(Material m)
    {
        if(m == null) return;
        int mode = Mathf.Clamp(Mathf.RoundToInt(Get(m,"_RandomMode")),0,9);

        // 0 全部随机优化：只随机常用可见项，不随机危险渲染状态。
        if(mode == 0)
        {
            RandomMainSingleColor(m);
            RandomMainThreeColor(m);
            RandomDissolveTiling(m);
            RandomDissolveParams(m);
            RandomDissolveEdgeColors(m);
            RandomMainFlow(m);
            RandomEdgeFresnel(m);
            RandomDistortHeat(m);
            RandomVertex(m);
        }
        else if(mode == 1) RandomMainSingleColor(m);
        else if(mode == 2) RandomMainThreeColor(m);
        else if(mode == 3) RandomDissolveTiling(m);
        else if(mode == 4) RandomDissolveParams(m);
        else if(mode == 5) RandomDissolveEdgeColors(m);
        else if(mode == 6) RandomMainFlow(m);
        else if(mode == 7) RandomEdgeFresnel(m);
        else if(mode == 8) RandomDistortHeat(m);
        else if(mode == 9) RandomVertex(m);

        EditorUtility.SetDirty(m);
    }

    private void RandomMainSingleColor(Material m)
    {
        Set(m,"_UseThreeColor",0);
        SetColor(m,"_SingleColor",RandomColorKeepIntensity(m,"_SingleColor"));
        Set(m,"_MainColorMix",Random.Range(0.4f,1f));
        Set(m,"_UseColorAdjust",Random.Range(0,2));
        Set(m,"_HueShift",Random.Range(-0.15f,0.15f));
        Set(m,"_Saturation",Random.Range(0.8f,1.8f));
        Set(m,"_Value",Random.Range(0.8f,1.6f));
    }

    private void RandomMainThreeColor(Material m)
    {
        Set(m,"_UseThreeColor",1);
        SetColor(m,"_ColorA",RandomColorKeepIntensity(m,"_ColorA"));
        SetColor(m,"_ColorB",RandomColorKeepIntensity(m,"_ColorB"));
        SetColor(m,"_ColorC",RandomColorKeepIntensity(m,"_ColorC"));
        Set(m,"_ColorSplit1",Random.Range(0.22f,0.48f));
        Set(m,"_ColorSplit2",Random.Range(0.55f,0.86f));
        Set(m,"_ColorSoftness",Random.Range(0.03f,0.22f));
    }

    private void RandomDissolveTiling(Material m)
    {
        Vector2 scale = new Vector2(Random.Range(0.4f,4.5f), Random.Range(0.4f,4.5f));
        Vector2 offset = new Vector2(Random.Range(-0.5f,0.5f), Random.Range(-0.5f,0.5f));
        if(m.HasProperty("_DissolveTex"))
        {
            m.SetTextureScale("_DissolveTex", scale);
            m.SetTextureOffset("_DissolveTex", offset);
        }
        Set(m,"_DissolveWrapModeX",1);
        Set(m,"_DissolveWrapModeY",1);
        Set(m,"_DissolveUseRotate",Random.Range(0,2));
        Set(m,"_DissolveRotateAngle",Random.Range(-180f,180f));
        Set(m,"_DissolveRotateSpeed",Random.Range(-45f,45f));
    }

    private void RandomDissolveParams(Material m)
    {
        Set(m,"_UseDissolveModule",1);
        Set(m,"_DissolveAmount",Random.Range(0.15f,0.75f));
        Set(m,"_DissolveContrast",Random.Range(0.8f,3.2f));
        Set(m,"_DissolveHardEdge",Random.Range(0,2));
        Set(m,"_DissolveSoftness",Random.Range(0.02f,0.22f));
        Set(m,"_DissolveEdgeWidth",Random.Range(0.025f,0.22f));
        Set(m,"_DissolveOuterEdgeWidth",Random.Range(0.04f,0.35f));
        Set(m,"_DissolveEdgeIntensity",Random.Range(0.8f,5.5f));
        Set(m,"_DissolveOuterEdgeIntensity",Random.Range(0f,3.5f));
        Set(m,"_DissolveEdgeAffectAlpha",Random.Range(0f,0.75f));
        Set(m,"_DissolveCurveMode",Random.Range(0,5));
        Set(m,"_DissolveCurvePower",Random.Range(0.7f,2.4f));
        Set(m,"_DissolveEndBoost",Random.Range(0f,0.7f));
    }

    private void RandomDissolveEdgeColors(Material m)
    {
        SetColor(m,"_DissolveEdgeColor",RandomColorKeepIntensity(m,"_DissolveEdgeColor"));
        SetColor(m,"_DissolveOuterEdgeColor",RandomColorKeepIntensity(m,"_DissolveOuterEdgeColor"));
        Set(m,"_DissolveEdgeIntensity",Random.Range(1.0f,6.0f));
        Set(m,"_DissolveOuterEdgeIntensity",Random.Range(0.5f,4.0f));
    }

    private void RandomMainFlow(Material m)
    {
        Set(m,"_MainUseFlow",1);
        Set(m,"_MainFlowX",Random.Range(-1f,1f));
        Set(m,"_MainFlowY",Random.Range(-1f,1f));
        Set(m,"_MainFlowSpeed",Random.Range(0.2f,3f));
        Set(m,"_MainUseRotate",Random.Range(0,2));
        Set(m,"_MainRotateSpeed",Random.Range(-90f,90f));
    }

    private void RandomEdgeFresnel(Material m)
    {
        Set(m,"_UseEdgeModule",1);
        Set(m,"_UseFresnelModule",1);
        SetColor(m,"_EdgeOnlyColor",RandomColorKeepIntensity(m,"_EdgeOnlyColor"));
        SetColor(m,"_FresnelColor",RandomColorKeepIntensity(m,"_FresnelColor"));
        Set(m,"_EdgeIntensity",Random.Range(0.25f,3.0f));
        Set(m,"_EdgeWidth",Random.Range(0.01f,0.25f));
        Set(m,"_FresnelForceHollow",1);
        Set(m,"_FresnelHollowPower",1);
        Set(m,"_FresnelHollowMin",0);
        Set(m,"_FresnelIntensity",Random.Range(0.3f,3.0f));
        Set(m,"_FresnelThickness",Random.Range(0.08f,0.55f));
        Set(m,"_FresnelBrightness",Random.Range(0.8f,4.0f));
    }

    private void RandomDistortHeat(Material m)
    {
        Set(m,"_UseDistortModule",1);
        Set(m,"_DistortStrength",Random.Range(0.03f,0.32f));
        Set(m,"_DistortAffectMain",Random.Range(0.3f,1f));
        Set(m,"_UseEdgeHeat",Random.Range(0,2));
        Set(m,"_HeatAffectMain",Random.Range(0.2f,1f));
        Set(m,"_EdgeHeatStrength",Random.Range(0.3f,2f));
        Set(m,"_EdgeHeatScaleX",Random.Range(2f,25f));
        Set(m,"_EdgeHeatScaleY",Random.Range(2f,25f));
        Set(m,"_EdgeHeatSpeed",Random.Range(0.5f,5f));
    }

    private void RandomVertex(Material m)
    {
        Set(m,"_UseVertexModule",1);
        Set(m,"_VertexUseAnimMode",0);
        Set(m,"_VertexMode",Random.Range(1,7));
        Set(m,"_VertexStrength",Random.Range(0.03f,0.45f));
        Set(m,"_VertexSpeed",Random.Range(0.5f,3.5f));
        Set(m,"_VertexScale",Random.Range(1.5f,14f));
        Set(m,"_VertexTexContrast",Random.Range(0.8f,2.5f));
        Set(m,"_VertexTexPower",Random.Range(0.8f,2.0f));
    }

}
