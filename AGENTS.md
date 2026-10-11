# AGENTS.md

Unity 2022.3.62f3 (LTS) 2D 游戏项目（TapTap「聚光灯」GameJam 参赛项目，Product Name: `SpotLight`，Company: `DefaultCompany`）。日常开发通过 Unity Editor 进行；仓库内**没有** CLI 构建 / 测试 / lint 命令。

## 工具链

- **渲染**：URP 14.0.12，2D Renderer，Linear 色彩空间（`ProjectSettings/ProjectSettings.asset: m_ActiveColorSpace: 1`）；管线资产在 `Assets/Settings/`。
- **输入**：Input System 1.14.2，`activeInputHandler: 2`（新旧输入系统同时启用）；`Assets/InputActions/PlayerInputActions.inputactions` 与其生成的 `PlayerInputActions.cs`。
- **相机**：Cinemachine 3.1.3（`Assets/Prefabs/Cameara/` 下有第三人称相机预设，注意目录名就是拼错的 `Cameara`）。
- **资源与打包**：YooAsset 3.0.5（`Assets/Scripts/Framework/SubSystems/Resource/Provider/YooAsset/YooAssetProvider.cs`）；采集配置 `Assets/Resources/BundleCollectorSetting.asset`，忽略规则 `Assets/Resources/YooAssetSettings.asset`。
- **寻路**：NavMeshPlus（`com.h8man.2d.navmeshplus`，2D 版 NavMesh）。
- **音频**：Wwise 2022.1.19.8584 深度集成，作者工程在**仓库同级**的 `Taptap_Wwise/Taptap_Wwise.wproj`。
- **UI 文本**：TextMeshPro 3.0.7。
- **编辑器内自动化**：Unity MCP（`com.coplaydev.unity-mcp`，HTTP `http://127.0.0.1:8080/mcp`）。Codex 侧已在 `~/.codex/config.toml` 的 `[mcp_servers.unityMCP]` 注册，`.cursor/mcp.json` 指向同一地址。**只有 Unity Editor 打开本工程且 MCP 桥接启动后该端口才可用**，否则工具调用直接连接失败。

## 项目结构

- `Assets/Scripts/` —— 项目自有代码，按命名空间分四层：
  - `Core/`（`namespace Core`）—— `AppCore`（`MonoSingleton<AppCore>`，全局子系统宿主，对外提供 `IsReady` / `OnAppReady`）、`AppCoreConfig`、`AppModel`、`GlobalConst`。
  - `Framework/`（`namespace Framework`）—— 与具体玩法无关的通用框架：
    - `Global.cs` —— 服务定位器。业务层通过 `Global.Get<T>()` / `Global.Register<T>()` 取子系统，**不要直接引用 `AppCore`**。
    - `Common/Singleton/`（`MonoSingleton`、`ScriptableObjectSingleton`）、`Common/StateMachine/`、`Common/SubSystemManager/`（`ISubSystem`、`SubSystemBase`、`SubSystemPriority`、`SystemManager`）。
    - `Event/EventBus.cs`；`Input/`（`LocalInputManager`、`LocalInputProvider`、`InputState`）。
    - `SubSystems/` —— 各子系统实现：`Audio/`（接 Wwise）、`Camera/`、`DataProxy/`、`Instantiation/`、`ObjectPool/`、`Resource/`、`SceneControl/`、`Timer/`、`UI/`（Panel / Window / Scene 三层 + `UIManager`）。
  - `GamePlay/`（`namespace GamePlay` / `GamePlay.Configs` / `GamePlay.Procedure`）—— 玩法层：
    - `Bootstrap/GameplayBootstrap.cs` —— 每局入口：订阅 `AppCore.OnAppReady`，就绪后注册 `GameplayRoot`。
    - `Procedure/` —— 流程状态机（`ProcedureCore` 持有 `StateMachine<ProcedureState>`，`States/` 各状态，`Task/` 切换任务）。
    - `Session/` —— 一局对局的上下文（`GameplayRoot`、`LevelContext`、`BattleContext`、`StroyContext`）。
    - `Configs/` —— `ConfigManager`（`ScriptableObjectSingleton`，按 `ConfigBase.Id` 建索引）+ `Content/*Config.cs`；实例资产在 `Assets/Configs/`。
    - `Player/`、`Enemy/`、`Dialog/` —— 角色控制、敌人状态机与导航、对话系统（`CharacterDatabase` + `DiaLogController`，注意类名就是 `DiaLogController`）。
  - `UI/`（`namespace UI`）—— 界面按 `Binding/`（自动生成的控件引用）、`Logic/`（界面逻辑，`partial class`）、`Properties/`（对外参数）三层拆分。绑定代码由 `Scripts/Editor/UIFramework/UIViewBindingEditor.cs` 在 Inspector 中生成；新增界面请沿用该拆分并保留 `// <ui-bind:add>` 标记块。
  - `Editor/` —— 编辑器扩展：`Instantiation/`、`Protobuf/`、`UIFramework/`。`Debug/ConsoleToScreen.cs` 用于把 Runtime 日志显示到屏幕。
- `Assets/Scenes/` —— `MainScene.unity`（**唯一**加入 Build Settings 的场景）+ `GameScene.unity`。
- `Assets/Prefabs/`、`Assets/Art/`、`Assets/Animations/`、`Assets/ThirdParty/`（AruiShader_URP、AruiPostEffect、PandaShader 等第三方 URP 包，**视为只读**）、`Assets/Plugins/`、`Assets/Audio/`（当前为空占位目录）。
- `Assets/Resources/` —— 各 `ScriptableObject` 单例配置：`AppCoreConfig`、`AudioConfig`、`CameraConfig`、`ConfigManager`、`ResGroupPolicy`、`YooAssetSettings`、`BundleCollectorSetting`。
- `Assets/StreamingAssets/` —— Wwise SoundBank 产物（`Audio/GeneratedSoundBanks/**`、`Init.bnk`、`Wwise_IDs.h`）。
- `Taptap_Wwise/` —— Wwise 作者工程；`Assets/Wwise/` 是 Wwise Unity Integration 插件本体（自带 asmdef），由安装器生成，**不要手改**。
- `Library/`、`Temp/`、`obj/`、`Logs/`、`UserSettings/`、`Bundles/` 是 Unity/构建生成目录，已被根目录 `.gitignore` 忽略。

## 注意事项

- **没有项目自有的 `.asmdef`**：`Assets/Scripts/` 下全部代码编译进单个 `Assembly-CSharp`（`Assets/Wwise/**` 自带 asmdef 除外）。不要随意新增 asmdef，那会改变整个项目的编译方式与程序集引用关系。
- **源文件编码不统一（已知问题）**：多数是 UTF-8，但下面 8 个是 **GBK（cp936）**，在 UTF-8 工具链下会显示为乱码。修改这些文件时必须保持原编码，或连同内容一起整体转为 UTF-8，否则中文注释会损坏：
  - `Assets/Scripts/GamePlay/Dialog/CharacterDatabase_Dialog.cs`、`Assets/Scripts/GamePlay/Dialog/DialogController.cs`
  - `Assets/Scripts/GamePlay/Enemy/` 下的 `EnemyBaseState.cs`、`EnemyController.cs`、`EnemyDetectZone.cs`、`EnemyMoveController.cs`、`EnemyStates.cs`、`EnemyStateType.cs`
- **`Assets/Scripts/GamePlay/Dialog/CharacterDatabase_Dialog.asset` 的脚本引用已损坏**：工作区中为 `m_Script: {fileID: 0}`（HEAD 中是正确 GUID），Inspector 会显示为 Missing Script。若非有意为之，应还原该文件而不是把它提交上去。
- `*.sln` / `*.csproj`（`2026-TapTap-Spotlight-GameJam-Project.sln`、`Assembly-CSharp*.csproj`、各 `AK.Wwise.*.csproj`）由 Unity 生成，只能由编辑器重新生成，**绝不手改**。
- `.meta` 必须与资产一起提交；删除/重命名资产时必须带上对应 `.meta`。**不要手改 `.asset` / `.prefab` / `.unity` 中的 GUID**。
- **Wwise 配置与本机强绑定**：`Assets/WwiseSettings.xml` 里 `WwiseInstallationPathWindows` 是机器绝对路径，`WwiseProjectPath=../Taptap_Wwise/Taptap_Wwise.wproj`（要求仓库与 Wwise 工程位于同一父目录），WAAPI 端口 `8081`，`CopySoundBanksAsPreBuildStep=true`。换机器时需重新设置。
- 代码注释与文档沿用中文（多为中文 XML 注释 + `#region` 分区），项目自有代码请保持这一惯例。
- **这是一个 git 仓库** —— 远程 `origin` 为 https://github.com/V1vy-v/2026-TapTap-Spotlight-GameJam-Project，默认分支 `main`。没有 CI、没有测试基础设施。破坏性 git 操作（reset、强推、回退、丢弃未提交改动）必须先经用户同意。

## AI 协作环境

- 默认模型提供方为 **OpenCode Go**：`~/.codex/config.toml` 中 `model_provider = "opencode-go"`，`[model_providers.opencode-go]` 指向 `base_url = "https://opencode.ai/zen/go/v1"`（`wire_api = "responses"`），凭据取自环境变量 `OPENCODE_GO_API_KEY`；当前 `model = "deepseek-flash"`（DeepSeek V4 Flash），`model_reasoning_effort = "high"`。
- 本工程已在 Codex 桌面端注册为本地项目，`trust_level = "trusted"`。