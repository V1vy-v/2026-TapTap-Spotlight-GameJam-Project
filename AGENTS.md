# AGENTS.md

Unity 2022.3.62f3 (LTS) 2D 游戏项目（TapTap「聚光灯」GameJam 参赛项目，Product Name: `SpotLight`，Company: `DefaultCompany`，`bundleVersion: 0.1.0`）。日常开发通过 Unity Editor 进行；仓库内**没有** CLI 构建 / 测试 / lint 命令。

## 工具链

- **渲染**：URP 14.0.12，2D Renderer，Linear 色彩空间（`ProjectSettings/ProjectSettings.asset: m_ActiveColorSpace: 1`）；管线资产在 `Assets/Settings/`（`URP-Balanced` / `URP-HighFidelity` / `URP-Performant` 各带同名 `-Renderer`，2D 用 `URP-2D-Renderer.asset`），全局设置在 `Assets/UniversalRenderPipelineGlobalSettings.asset`。
- **输入**：Input System 1.14.2，`activeInputHandler: 2`（新旧输入系统同时启用）；`Assets/InputActions/PlayerInputActions.inputactions` 与其生成的 `PlayerInputActions.cs`（**生成物，不要手改**）。
- **相机**：Cinemachine 3.1.3，预设 `Assets/Prefabs/Cameara/Third Person Camera.prefab`、`Third Person Aim Camera.prefab`（注意目录名就是拼错的 `Cameara`）。
- **资源与打包**：YooAsset 3.0.5（`Assets/Scripts/Framework/SubSystems/Resource/Provider/YooAsset/YooAssetProvider.cs`）。采集配置 `Assets/Resources/BundleCollectorSetting.asset`，三组均为 `CollectorType: 0`（MainAsset）+ `PackRule: PackDirectory` + `AddressRule: AddressByGroupAndFileName`：
  - `UI` ← `Assets/Prefabs/UI`
  - `Prefab` ← `Assets/Prefabs/Player`、`Assets/Prefabs/Enemy`
  - `Config` ← `Assets/Configs`

  **地址规则是 `<组名>_<文件名>`**（如 `Prefab_Player`、`UI_BeginPanel`）。**场景不走 YooAsset**，只走 Build Settings；Wwise SoundBank 走 StreamingAssets。
- **寻路**：NavMeshPlus（`com.h8man.2d.navmeshplus`，2D 版 NavMesh）；烘焙数据在 `Assets/Scenes/GameScene/NavMesh-Navigation*.asset`。
- **音频**：Wwise 2022.1.19.8584 深度集成；作者工程在**本仓库内** `Taptap_Wwise/Taptap_Wwise.wproj`（与 `Assets/` 同级，所以 `WwiseProjectPath=../Taptap_Wwise/Taptap_Wwise.wproj` 才解析得到）。两个场景都有 `WwiseGlobal`。
- **UI 文本**：TextMeshPro 3.0.7；中文字体源文件 `Assets/Art/Fonts/AaHouDiHei-2.ttf`（详见末尾「中文文本」）。
- **编辑器内自动化**：Unity MCP（`com.coplaydev.unity-mcp`，HTTP `http://127.0.0.1:8080/mcp`）；`.cursor/mcp.json` 与 `~/.codex/config.toml` 的 `[mcp_servers.unityMCP]` 都指向该地址。**只有 Unity Editor 打开本工程且 MCP 桥接启动后该端口才可用**，否则工具调用直接连接失败。

## 项目结构

- `Assets/Scripts/` —— 项目自有代码（147 个 `.cs`），全部编译进 `Assembly-CSharp`。目录分四层，但**目录名与 namespace 并不一一对应，以代码为准**：
  - `Core/`（`namespace Core`）—— `AppCore`（`MonoSingleton<AppCore>`，全局子系统宿主）、`AppCoreConfig`、`AppModel`、`AppModelProxy`。启动顺序是：**先**注册 `ResourceManager` 并 `SetProvider(new YooAssetProvider(...))`，在 `OnResourceReady` 回调里才 `InitSubSystems()`（`StorageManager` → `InstantiateManager` → `PoolManager` → `TimerManager` → `DataProxyManager` → `UIManager` → `SceneLoader` → `LocalInputManager` → `AudioManager` → `CameraManager` → `GameplayRoot`），再 `InitDataProxy()`（`AppModelProxy`、`GameSaveDataProxy`），最后置 `IsReady` 并触发 `OnAppReady`。**订阅 `OnAppReady` 不必关心 Awake 顺序：已就绪后再订阅会立刻收到一次回调。**
  - `Debug/`（`namespace Core`）—— `ConsoleToScreen.cs`，把 Runtime 日志显示到屏幕。
  - `Framework/`（`namespace Framework`；少数为 `Framework.SubSystems`，编辑器侧为 `Framework.Editor`）—— 与玩法无关的通用框架：
    - `Global.cs` —— 服务定位器。业务层通过 `Global.Get<T>()` / `Global.Register<T>()` / `Global.RegisterDataProxy<T>()` 取子系统，**不要直接引用 `AppCore`**。
    - `GlobalConstants.cs` —— 场景名与预制体地址：`MENU_SCENE_NAME = "MainScene"`、`GAME_SCENE_NAME = "GameScene"`、`PlayerPrefabLocation = "Prefab_Player"`。
    - `Common/` —— `Singleton/`（`MonoSingleton`；`ScriptableObjectSingleton` 用 `Resources.Load<T>(typeof(T).Name)` 取 `Resources/<类名>.asset`）、`StateMachine/`、`SubSystemManager/`（`ISubSystem`、`SubSystemBase`、`SubSystemPriority`、`SystemManager`）。
    - `Event/EventBus.cs`；`Input/`（`LocalInputManager`、`LocalInputProvider`、`InputState`、`IInputStateProvider`、`LocalInputType`；`LocalInputManager.SwitchMap(LocalInputType)` 切映射，内部 `GameObject.Find("[GameRoot]")`）。
    - `SubSystems/` —— 各子系统实现：
      - `Audio/`（`AudioManager`、`AudioConfig`，`namespace Framework.SubSystems`，接 Wwise）
      - `Camera/`（`CameraManager`、`CameraConfig`、`CameraState`）
      - `DataProxy/`（`DataProxyManager`、`BaseDataProxy`、`IDataProxy`）
      - `Instantiation/`（`InstantiateManager`，partial 拆分：`.Async` / `.Debug` / `.Entries` / `.Pool` / `.Recycle`）
      - `ObjectPool/`（`PoolManager`、`ObjectPool`、`IPoolable`；`ResGroup/` 下 `ResGroup`、`ResGroupPolicy`）
      - `Resource/`（`ResourceManager`、`IResourceLoader`、`Provider/YooAsset/YooAssetProvider`）
      - `SceneControl/`（`SceneLoader`）
      - `Storage/`（`StorageManager`：JSON 存档，`Load<T>` / `SaveJson<T>` / `DeleteFile`，配 `UnityVectorJsonConverter`）
      - `Timer/`（`TimerManager`、`Timer`）
      - `UI/`（`UIManager`；`Core/`：`UIView`、`UIController`、`UIBaseLayer`、`IUIController`、`IUIProperties`；`Panel/`、`Window/`、`Scene/` 三层各有 `*Controller` / `*Layer` / `*Properties` / `*Priority`；`UIAnimation/`：`AnimationView`、`AnimComponent`、`FadeAnimation`）。`UIManager` 对外是 `ShowPanel` / `HidePanel`、`ShowWindow` / `HideWindow` / `CloseCurrentWindow`、`ShowScene` / `HideScene`、`ShowUI<T>`，带实例缓存与延迟隐藏。
  - `GamePlay/` —— 玩法层。**namespace 沿用现状（若干目录仍是 `Framework`），不要按目录名想当然**：
    - `Procedure/`（`namespace GamePlay.Procedure`）—— **会话层**流程状态机。`ProcedureCore`（`MonoSingleton`，挂在 MainScene 的 `[GameRoot]` 上）持有 `StateMachine<ProcedureState>`：`Menu` / `Loading` / `Playing`；`States/` 三个状态 + `ProcedureStateBase`，`Task/`（`ILoadTask`、`ProcedureTransition`、`SceneLoadTask`）。对外：`StartUp()`、`RequestPlay()`、`RequestMenu()`、`RequestState(ProcedureState, ILoadTask)`、`RequestScene(...)`、`ClearNewGameTag()`、`FailTransition(...)`。**换场景只能走 `RequestPlay()` / `RequestMenu()`，绝不要直接调 `Global.LoadScene`** —— 否则状态机与场景不同步，表现为「旧面板不消失、主角不生成」。
    - `Session/`（`namespace GamePlay.Session`，惟 `GameplayRoot.cs` 是 `namespace GamePlay`）—— 一局对局的上下文：`GameplayRoot`（`SubSystemBase`）、`StoryContext`、`StoryStage`、`BattleContext`、`GameplayState.cs`（内含 **`GameplayMode`** 枚举：FreeRoam / Dialogue / Cutscene / Battle / Pause / GameOver；注意文件名与类型名不一致）、`States/` 六个局内状态 + `GameplayStateBase`。`GameplayRoot` 对外：`StartNewGame()`、`ContinueGame()`、`EndGame()`、`ChangeMode(GameplayMode)`、`Pause()` / `Resume()`、`AdvanceNode()` / `AdvanceStage()`、`EnterBattle()` / `EndBattle()`，并负责主角的实例化与释放。
    - `Configs/`（`namespace GamePlay.Configs`）—— `ConfigManager`（`ScriptableObjectSingleton<ConfigManager>`，`OnEnable` 按 `ConfigBase.Id` 建字典，重复 Id 会 `LogError`）+ `ConfigBase`（`Id`、`Name`）+ `Content/`：`BuffConfig`、`EnemyConfig`、`ItemConfig`、`LiquidConfig`、`PlayerConfig`、`StoryConfig`。读取用 `ConfigManager.Instance.TryGetConfig<T>(id, out var cfg)`。实例资产在 `Assets/Configs/`。**配置里只放数值或地址字符串，不放资源引用** —— `ConfigManager` 由 `Resources.Load` 加载并**硬引用** `_configs` 列表，塞进 Sprite/Prefab 会把所有依赖拖进启动并让 YooAsset bundle 互相交叉。
    - `Player/`（`namespace GamePlay.Player`）—— 主角：`PlayerController`（输入入口 + 编排）、`PlayerCombat`、`PlayerLiquids`、`PlayerStats`、`PlayerInteractor`、`PlayerView`（**唯一**持有 Animator / SpriteRenderer 的表现层）。
    - `Liquid/`（`namespace GamePlay.Liquid`）—— 液体形态：`LiquidBase`（纯 C# 抽象基类，管技能时序）、`LiquidFactory`（`Create(LiquidConfig, PlayerStats)`，按 `LiquidConfig.Type` 造子类）、`Liquids/`（`Cola`、`GuoJiao`）。`LiquidConfig` 里另有 `LiquidKind` 枚举（`Active` / `MainPassive` / `StandbyPassive`）。
    - `Enemy/`（`namespace GamePlay.Enemy`）—— 敌人状态机与导航：`EnemyController`、`EnemyMoveController`、`EnemyDetectZone`、`EnemyBaseState` / `EnemyRealState` / `EnemyStates`、`EnemyStateType`。
    - `Buff/`、`Obstacle/` —— **注意这两个目录的 namespace 不是 `GamePlay.*`**：`Buff/` 是 `Framework`，`Obstacle/` 是 `GamePlay.Liquid`。内容为 `BuffController`、`BuffEffect`、`BuffInstance`、`SpeedBuffEffect`；`ObstacleBase`、`Obstacle_Breakable` / `Obstacle_Pushable` / `Obstacle_UnBreakable`、`ObstacleType`。
    - `Dialog/`（`namespace GamePlay.DataProxy`）—— 对话系统：`CharacterDatabase`（`GetIcon` / `Contains`）、`CharacterEntry`、`DialogController`。
    - `GameSave/`（`namespace GamePlay.GameSave`）—— `SaveData/GameSaveData.cs`，局内存档数据实体。
    - `DataProxy/`（`namespace GamePlay.DataProxy`）—— `GameSave/GameSaveDataProxy.cs`。
    - `TestScript.cs` —— 临时脚本（`namespace Framework`）。
  - `UI/`（`namespace UI`）—— 界面按 `Binding/`（自动生成的控件引用）、`Logic/`（界面逻辑，`partial class`）、`Properties/`（对外参数）三层拆分，并按场景分子目录（`MainScene/`、`GameScene/`）。绑定代码由 `Assets/Scripts/Editor/UIFramework/UIViewBindingEditor.cs` 在 Inspector 中生成；新增界面请沿用该拆分并保留 `// <ui-bind:add>` 标记块。
  - `Editor/`（`namespace Framework.Editor`）—— 编辑器扩展：`Instantiation/InstantiateManagerDebugWindow.cs`、`Protobuf/ProtoCompilerWindow.cs`、`UIFramework/UIViewBindingEditor.cs`。
- `Assets/Scenes/` —— `MainScene.unity`（菜单，**启动场景**：根下有 `[GameRoot]`、`WwiseGlobal`、`Global Volume`、`Light 2D`，以及一个名为「========== 全局组件 ==========」的分隔节点）与 `GameScene.unity`（对局：根下有 `[SceneRoot]`、`WwiseGlobal`、`Ground`、`BK`、`Navigation`、`Items`、`[Chicken]`、`Light 2D` 及各场景道具）。两个场景**都已加入 Build Settings**（`ProjectSettings/EditorBuildSettings.asset`，均 enabled）且不进 YooAsset。`[GameRoot]` 是运行期经 `GameObject.Find("[GameRoot]")` 定位的，**改名会断掉 `LocalInputManager` 与 `InstantiateManager`**；`[SceneRoot]` 目前无脚本引用，只是 GameScene 的组织节点。
- `Assets/Configs/` —— `ConfigBase` 实例资产：`Player/PlayerConfig`、`Enemy/EnemyConfig`、`Liquid/`（`Cola`、`CookingWine`、`Leibi`、`ManbaTea`、`Vinegar`）、`BuffEffect/SpeedBuffEffect`；`Item/`、`Story/` 目前是空目录。
- `Assets/Prefabs/` —— `Cameara/`（第三人称相机）、`Player/Player.prefab`、`Enemy/`（`Enemy_ManbaTea.prefab`、`Enemy_ManbaTea_防丢失新加版.prefab`）、`GameScene/`（`ScenePalette.prefab` + `Tiles/`）、`Scene/`、`Text/`、`UI/`（`UIManager.prefab`；`MainScene/`：`BeginPanel`、`SettingPanel`；`GameScene/`：`HUD`、`MenuPanel`、`DialogPanel`；`Loading/LoadingPanel`）。
- `Assets/Art/`、`Assets/Animations/`（`Animations/` + `Animation Controller/`）、`Assets/ThirdParty/`（`AruiPostEffect1.0_URP`、`AruiShader_URP`、`PandaShaderSamPleURPV1.1`、`TextMesh Pro` —— **视为只读**）、`Assets/Plugins/`（**当前为空目录**）、`Assets/Audio/`（**当前为空占位目录**）。
- `Assets/Editor/SpineSettings.asset` —— Spine 残留资产：工程内没有 Spine 运行时（`Assets/Plugins/` 为空可佐证），不要被它误导。
- `Assets/Resources/` —— 7 个 `ScriptableObject` 单例配置：`AppCoreConfig`、`AudioConfig`、`CameraConfig`、`ConfigManager`、`ResGroupPolicy`、`YooAssetSettings`、`BundleCollectorSetting`。**注意：Resources 下的内容会被无条件打进包，且不经 YooAsset 管理。**
- `Assets/StreamingAssets/` —— Wwise SoundBank 产物（`Audio/GeneratedSoundBanks/**`、`Init.bnk`、`Wwise_IDs.h`）。
- `Taptap_Wwise/` —— Wwise 作者工程；`Assets/Wwise/` 是 Wwise Unity Integration 插件本体（自带 8 个 asmdef），由安装器生成，**不要手改**。
- `docs/` —— 策划文档 `01流程` … `07数值`（7 个 `.docx`）+ `开发任务线.md`、`开发者日志02.md`。
- `Library/`、`Temp/`、`obj/`、`Logs/`、`UserSettings/`、`Bundles/`、`Build/` 是 Unity/构建生成目录，已被根目录 `.gitignore` 忽略。

## 已知现状 / 待办

以下是盘查到的**真实工程状态**，动手前先确认，别踩坑：

- **`LiquidFactory` 只实现了 `Cola` 与 `GuoJiao`**；`LiquidType` 里其余四个（`LeiBi`、`LiaoJiu`、`BingMeiShi`、`SanQuan`）走 `_ => null`。
- **`ConfigManager.asset` 只登记了 3 个配置**：`Enemy/EnemyConfig`、`Liquid/ManbaTea`、`Player/PlayerConfig`。`BuffEffect/SpeedBuffEffect`、`Liquid/Cola`、`Liquid/CookingWine`、`Liquid/Leibi`、`Liquid/Vinegar` **都不在** `_configs` 里，`TryGetConfig` 取不到。
- **5 个 Liquid 资产是复制出来的占位数据**：`CookingWine` / `Leibi` / `ManbaTea` / `Vinegar` 的 `Id` 全是 `Liquid_001`、`Name` 全是「国窖」，且资产里**没有序列化 `Type` 字段**（`Cola` 的 `Id` 是 `Liquid_002`）。当前只有 `ManbaTea` 被登记，所以重复 Id 的 `LogError` 暂时不会触发。
- **`GameSaveData` 只有 `PlayerPos` 一个字段并带 TODO**，「存档功能」还在早期。
- **只有 `BeginPanel` / `MenuPanel` 三件套齐全**；`HUD`、`SettingPanel`、`DialogPanel`、`LoadingPanel` 目前只有预制体，没有 Binding / Logic / Properties。

## 注意事项

- **没有项目自有的 `.asmdef`**：`Assets/Scripts/` 下代码全部编译进单个 `Assembly-CSharp`（`/Editor/` 目录进 `Assembly-CSharp-Editor`）；`Assets/Wwise/**` 自带 8 个 asmdef 除外。不要随意新增 asmdef，那会改变整个项目的编译方式与程序集引用关系。
- **编码与换行**：`Assets/` 下 **461 个 `.cs` 全部**是 UTF-8 无 BOM + CRLF（0 个 BOM、0 个 LF），这是硬约定 —— 一旦出现 GBK（cp936），中文注释在 UTF-8 工具链下会乱码；编辑器里请关闭「UTF-8 带签名 / BOM」的保存选项。但**非 `.cs` 资产并未统一**：`.meta` 144 个、`.asset` 14 个、`.prefab` 8 个、`.unity` 1 个（`MainScene.unity`）、`.inputactions` 1 个、`.json` 2 个仍是 LF。改动这些文件时**不要顺手做全文件 EOL 转换**，否则会产生巨大的无意义 diff。
- `*.sln` / `*.csproj`（`SpotLight.sln`、`Assembly-CSharp*.csproj`、各 `AK.Wwise.*.csproj`）由 Unity 生成，**且已被 `.gitignore` 的 `*.sln` / `*.csproj` 规则忽略**（未跟踪），只能由编辑器重新生成，**绝不手改**。注意 `.vscode/settings.json` 的 `dotnet.defaultSolution` 仍指向旧仓库名 `2026-TapTap-Spotlight-GameJam-Project.sln`，已失效。
- `.meta` 必须与资产一起提交；删除/重命名资产时必须带上对应 `.meta`。**不要手改 `.asset` / `.prefab` / `.unity` 中的 GUID**。
- **Wwise 配置与本机强绑定**：`Assets/WwiseSettings.xml` 里 `WwiseInstallationPathWindows` 是机器绝对路径（当前值 `D:\LRF\Wwise\Wwise_2022.1.19.8584` —— **该路径在本机不存在，换机后必须重设**，否则 Wwise 面板与预构建 SoundBank 步骤不可用）；`WwiseProjectPath=../Taptap_Wwise/Taptap_Wwise.wproj`（相对 `Assets/` 解析，即作者工程就在本仓库内，**不是**仓库同级）；WAAPI 端口 `8081`；`CopySoundBanksAsPreBuildStep=true`（SoundBank 已提交在 StreamingAssets，所以运行期音频不受上面的路径失效影响）。
- 代码注释与文档沿用中文（多为中文 XML 注释 + `#region` 分区），项目自有代码请保持这一惯例。
- **这是一个 git 仓库** —— **两个远程**：`origin` = https://github.com/V1vy-v/2026-TapTap-Spotlight-GameJam-Project.git，`gitee` = https://gitee.com/hyyye/2026-TapTap-Spotlight-GameJam-Project.git；默认分支 `main`。没有 CI、没有测试基础设施。破坏性 git 操作（reset、强推、回退、丢弃未提交改动）必须先经用户同意。本地目录名已从 `2026-TapTap-Spotlight-GameJam-Project` 改为 `SpotLight`，远程仓库名未变。
- `docs/` 下会出现 Word 打开文档时产生的 `~$*.docx` 锁文件（**已有 6 个被误提交**：`~$01流程`、`~$02敌人`、`~$03道具`、`~$04系统规则`、`~$07数值`、`~$UI与HUD`），**不要提交新的锁文件**。
- `.gitignore` 里的 `.curosr/` 是 `.cursor/` 的拼写错误，**`.cursor/` 实际未被忽略**，改动 `.cursor/mcp.json` 时留意不要误提交。

## 中文文本

- 字体源文件 `Assets/Art/Fonts/AaHouDiHei-2.ttf`：Dynamic 导入、`Include Font Data` 已开，覆盖 GB2312 全集 6763 汉字 —— 项目现有文案零缺字；GB2312 之外的字（生僻字、繁体）会缺。
- 制作 TMP 字体资产：右键 `.ttf` → `Create > TextMeshPro > Font Asset` 得到 **Dynamic** 资产，务必勾上 `Multi Atlas Textures`（中文必需，否则字形一多就报 atlas 满）；`Sampling Point Size` ≥ 40（约 60）、`Padding` ≈ 点大小 × 0.1、`Render Mode` 用 `SDFAA`。
- 字体资产在 UI 预制体上引用即可（作为依赖随 `UI` 组 bundle 走）；**不要**轻易设成 `TMP Settings` 的默认字体 —— 那会让它走 `Resources` 无条件进包。

## AI 协作环境

- 当前通过 **DeepSeek Harness（DSH）** 在本工程下工作，本文件即工作区指令。
- 该工程同时注册在 Codex 桌面端：`~/.codex/config.toml` 中 `model_provider = "opencode-go"`，`[model_providers.opencode-go]` 指向 `base_url = "https://opencode.ai/zen/go/v1"`（`wire_api = "responses"`），凭据取自环境变量 `OPENCODE_GO_API_KEY`；`model = "deepseek-flash"`（DeepSeek V4 Flash），`model_reasoning_effort = "high"`；`[mcp_servers.unityMCP]` 注册了 Unity MCP。`trust_level = "trusted"`。
- Unity Editor 安装位置（手动注册在 Unity Hub）：`D:\Programdata\UnityHub\Editor\Unity2022.3\Editor\Unity.exe`（2022.3.62f3）。
