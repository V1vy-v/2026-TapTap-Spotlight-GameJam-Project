# AGENTS.md

Unity 2022.3.62f3 (LTS) 2D 游戏项目（TapTap「聚光灯」GameJam 参赛项目，Product Name: `SpotLight`，Company: `DefaultCompany`）。日常开发通过 Unity Editor 进行；仓库内**没有** CLI 构建 / 测试 / lint 命令。

## 工具链

- **渲染**：URP 14.0.12，2D Renderer，Linear 色彩空间（`ProjectSettings/ProjectSettings.asset: m_ActiveColorSpace: 1`）；管线资产在 `Assets/Settings/`。
- **输入**：Input System 1.14.2，`activeInputHandler: 2`（新旧输入系统同时启用）；`Assets/InputActions/PlayerInputActions.inputactions` 与其生成的 `PlayerInputActions.cs`（**生成物，不要手改**）。
- **相机**：Cinemachine 3.1.3（`Assets/Prefabs/Cameara/` 下有第三人称相机预设，注意目录名就是拼错的 `Cameara`）。
- **资源与打包**：YooAsset 3.0.5（`Assets/Scripts/Framework/SubSystems/Resource/Provider/YooAsset/YooAssetProvider.cs`）；采集配置 `Assets/Resources/BundleCollectorSetting.asset`（当前三组：`UI` / `Prefab` / `Config`），忽略规则 `Assets/Resources/YooAssetSettings.asset`。**地址规则是 `<组名>_<文件名>`**（如 `Prefab_Player`、`UI_BeginPanel`）。**场景不走 YooAsset**，只走 Build Settings；Wwise SoundBank 走 StreamingAssets。
- **寻路**：NavMeshPlus（`com.h8man.2d.navmeshplus`，2D 版 NavMesh）。
- **音频**：Wwise 2022.1.19.8584 深度集成，作者工程在**仓库同级**的 `Taptap_Wwise/Taptap_Wwise.wproj`。
- **UI 文本**：TextMeshPro 3.0.7；中文字体源文件 `Assets/Art/Fonts/AaHouDiHei-2.ttf`（详见末尾「中文文本」）。
- **编辑器内自动化**：Unity MCP（`com.coplaydev.unity-mcp`，HTTP `http://127.0.0.1:8080/mcp`）。Codex 侧已在 `~/.codex/config.toml` 的 `[mcp_servers.unityMCP]` 注册，`.cursor/mcp.json` 指向同一地址。**只有 Unity Editor 打开本工程且 MCP 桥接启动后该端口才可用**，否则工具调用直接连接失败。

## 项目结构

- `Assets/Scripts/` —— 项目自有代码，按命名空间分四层：
  - `Core/`（`namespace Core`）—— `AppCore`（`MonoSingleton<AppCore>`，全局子系统宿主，对外提供 `IsReady` / `OnAppReady`；在 `Init` 里经 `SystemManager` 注册各子系统，其中包含 `GameplayRoot`）、`AppCoreConfig`、`AppModel`。
  - `Framework/`（`namespace Framework`）—— 与具体玩法无关的通用框架：
    - `Global.cs` —— 服务定位器。业务层通过 `Global.Get<T>()` / `Global.Register<T>()` 取子系统，**不要直接引用 `AppCore`**。
    - `GlobalConstants.cs` —— 场景名、预制体地址等全局常量。
    - `Common/Singleton/`（`MonoSingleton`、`ScriptableObjectSingleton`）、`Common/StateMachine/`、`Common/SubSystemManager/`（`ISubSystem`、`SubSystemBase`、`SubSystemPriority`、`SystemManager`）。
    - `Event/EventBus.cs`；`Input/`（`LocalInputManager`、`LocalInputProvider`、`InputState`）。
    - `SubSystems/` —— 各子系统实现：`Audio/`（接 Wwise）、`Camera/`、`DataProxy/`、`Instantiation/`、`ObjectPool/`、`Resource/`、`SceneControl/`、`Timer/`、`UI/`（Panel / Window / Scene 三层 + `UIManager`）。
  - `GamePlay/`（`namespace GamePlay` / `GamePlay.Configs` / `GamePlay.Procedure` / `GamePlay.Player` / `GamePlay.Liquid`）—— 玩法层：
    - `Procedure/` —— **会话层**流程状态机。`ProcedureCore`（`MonoSingleton`，挂在 `[GameRoot]` 上并 `DontDestroyOnLoad`）持有 `StateMachine<ProcedureState>`：`Menu` / `Loading` / `Playing`；`States/` 各状态，`Task/` 场景切换任务。**换场景只能走 `ProcedureCore.RequestPlay()` / `RequestMenu()`，绝不要直接调 `Global.LoadScene`** —— 否则状态机与场景不同步，表现为「旧面板不消失、主角不生成」。
    - `Session/` —— 一局对局的上下文：`GameplayRoot`（`SubSystemBase`）、`StoryContext`、`StoryStage`、`BattleContext`。`GameplayRoot` 另持一个**局内**状态机 `GameplayMode`（FreeRoam / Dialogue / Cutscene / Battle / Pause / GameOver），并负责主角的实例化与释放。
    - `Configs/` —— `ConfigManager`（`ScriptableObjectSingleton`，按 `ConfigBase.Id` 建索引）+ `Content/*Config.cs`；实例资产在 `Assets/Configs/`。**配置里只放数值，不放资源引用** —— `ConfigManager` 由 `Resources.Load` 加载并硬引用全部配置，塞进 Sprite/Prefab 会把所有依赖拖进启动并让 YooAsset bundle 互相交叉。
    - `Player/` —— 主角：`PlayerController`（输入入口 + 编排）、`PlayerCombat`、`PlayerLiquids`、`PlayerStats`、`PlayerView`（**唯一**持有 Animator / SpriteRenderer 的表现层）。
    - `Liquid/` —— 液体形态：`LiquidBase`（纯 C# 抽象基类，管技能时序）、`LiquidFactory`（按 `LiquidConfig.Type` 造子类）、`Liquids/` 各形态子类。
    - `Enemy/`、`Buff/`、`Obstacle/`、`Dialog/` —— 敌人状态机与导航、Buff、障碍物、对话系统（`CharacterDatabase` + `DialogController`）。
  - `UI/`（`namespace UI`）—— 界面按 `Binding/`（自动生成的控件引用）、`Logic/`（界面逻辑，`partial class`）、`Properties/`（对外参数）三层拆分。绑定代码由 `Scripts/Editor/UIFramework/UIViewBindingEditor.cs` 在 Inspector 中生成；新增界面请沿用该拆分并保留 `// <ui-bind:add>` 标记块。
  - `Editor/` —— 编辑器扩展：`Instantiation/`、`Protobuf/`、`UIFramework/`。`Debug/ConsoleToScreen.cs` 用于把 Runtime 日志显示到屏幕。
- `Assets/Scenes/` —— `MainScene.unity`（菜单）与 `GameScene.unity`（对局），**两个都已加入 Build Settings**（`ProjectSettings/EditorBuildSettings.asset`）。场景不进 YooAsset。
- `Assets/Configs/` —— 各 `ConfigBase` 资产的实例（`Player/`、`Liquid/`、`Enemy/` …）。
- `Assets/Prefabs/`、`Assets/Art/`、`Assets/Animations/`、`Assets/ThirdParty/`（AruiShader_URP、AruiPostEffect、PandaShader 等第三方 URP 包，**视为只读**）、`Assets/Plugins/`、`Assets/Audio/`（当前为空占位目录）。
- `Assets/Resources/` —— 各 `ScriptableObject` 单例配置：`AppCoreConfig`、`AudioConfig`、`CameraConfig`、`ConfigManager`、`ResGroupPolicy`、`YooAssetSettings`、`BundleCollectorSetting`。**注意：Resources 下的内容会被无条件打进包，且不经 YooAsset 管理。**
- `Assets/StreamingAssets/` —— Wwise SoundBank 产物（`Audio/GeneratedSoundBanks/**`、`Init.bnk`、`Wwise_IDs.h`）。
- `Taptap_Wwise/` —— Wwise 作者工程；`Assets/Wwise/` 是 Wwise Unity Integration 插件本体（自带 asmdef），由安装器生成，**不要手改**。
- `Library/`、`Temp/`、`obj/`、`Logs/`、`UserSettings/`、`Bundles/` 是 Unity/构建生成目录，已被根目录 `.gitignore` 忽略。

## 注意事项

- **没有项目自有的 `.asmdef`**：`Assets/Scripts/` 下全部代码编译进单个 `Assembly-CSharp`（`Assets/Wwise/**` 自带 asmdef 除外）。不要随意新增 asmdef，那会改变整个项目的编译方式与程序集引用关系。
- **源文件编码已统一为 UTF-8 无 BOM + CRLF**（2026-10 完成：原 21 个 GBK 文件转 UTF-8、18 个带 BOM 的去掉 BOM、5 个 LF 文件规整为 CRLF）。新增 / 修改文件请保持 **UTF-8 无 BOM**；一旦出现 GBK（cp936），中文注释在 UTF-8 工具链下会乱码。编辑器里请关闭「UTF-8 带签名 / BOM」的保存选项。
- `*.sln` / `*.csproj`（`SpotLight.sln`、`Assembly-CSharp*.csproj`、各 `AK.Wwise.*.csproj`）由 Unity 生成，只能由编辑器重新生成，**绝不手改**。
- `.meta` 必须与资产一起提交；删除/重命名资产时必须带上对应 `.meta`。**不要手改 `.asset` / `.prefab` / `.unity` 中的 GUID**。
- **Wwise 配置与本机强绑定**：`Assets/WwiseSettings.xml` 里 `WwiseInstallationPathWindows` 是机器绝对路径，`WwiseProjectPath=../Taptap_Wwise/Taptap_Wwise.wproj`（要求仓库与 Wwise 工程位于同一父目录），WAAPI 端口 `8081`，`CopySoundBanksAsPreBuildStep=true`。换机器时需重新设置。
- 代码注释与文档沿用中文（多为中文 XML 注释 + `#region` 分区），项目自有代码请保持这一惯例。
- **这是一个 git 仓库** —— **两个远程**：`origin` = https://github.com/V1vy-v/2026-TapTap-Spotlight-GameJam-Project.git，`gitee` = https://gitee.com/hyyye/2026-TapTap-Spotlight-GameJam-Project.git；默认分支 `main`。没有 CI、没有测试基础设施。破坏性 git 操作（reset、强推、回退、丢弃未提交改动）必须先经用户同意。
- `docs/` 下会出现 Word 打开文档时产生的 `~$*.docx` 锁文件（已有 5 个被误提交），**不要提交新的锁文件**。

## 中文文本

- 字体源文件 `Assets/Art/Fonts/AaHouDiHei-2.ttf`：Dynamic 导入、`Include Font Data` 已开，覆盖 GB2312 全集 6763 汉字 —— 项目现有文案零缺字；GB2312 之外的字（生僻字、繁体）会缺。
- 制作 TMP 字体资产：右键 `.ttf` → `Create > TextMeshPro > Font Asset` 得到 **Dynamic** 资产，务必勾上 `Multi Atlas Textures`（中文必需，否则字形一多就报 atlas 满）；`Sampling Point Size` ≥ 40（约 60）、`Padding` ≈ 点大小 × 0.1、`Render Mode` 用 `SDFAA`。
- 字体资产在 UI 预制体上引用即可（作为依赖随 `UI` 组 bundle 走）；**不要**轻易设成 `TMP Settings` 的默认字体 —— 那会让它走 `Resources` 无条件进包。

## AI 协作环境

- 默认模型提供方为 **OpenCode Go**：`~/.codex/config.toml` 中 `model_provider = "opencode-go"`，`[model_providers.opencode-go]` 指向 `base_url = "https://opencode.ai/zen/go/v1"`（`wire_api = "responses"`），凭据取自环境变量 `OPENCODE_GO_API_KEY`；当前 `model = "deepseek-flash"`（DeepSeek V4 Flash），`model_reasoning_effort = "high"`。
- 本工程已在 Codex 桌面端注册为本地项目，`trust_level = "trusted"`。
