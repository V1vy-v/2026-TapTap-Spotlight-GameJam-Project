AruiPostEffect1.0 - URP 版本

适用范围
- Unity 2020.3.21f1 + URP 10.x（主目标）
- Unity 2019.4 + URP 7.x（采用传统 CommandBuffer Blit 兼容路径）

安装步骤
1. 只将本文件夹 AruiPostEffect1.0_URP 整体放入 Assets。
2. 打开项目使用的 Universal Renderer Data（通常位于 Settings/UniversalRendererData）。
3. 点击 Add Renderer Feature，添加 AruiPostEffect1.0 URP Renderer Feature。
4. 在需要后效的相机挂 Arui / AruiPostEffect1.0 URP 屏幕后效控制器。
5. Renderer Feature 默认注入在 After Rendering Transparents，因此场景、角色、粒子和透明特效都会一起处理。

Timeline
1. Timeline 添加 AruiPostEffect1.0 URP 后效轨道。
2. 绑定相机上的 AruiPostEffect10URP 组件。
3. 添加 URP 后效片段，在 Clip Inspector 选择功能与独立参数。

说明
- URP 版与 Built-in 版的功能、预设、Timeline 参数逻辑保持同名设计。
- 这两个文件夹可共存于同一项目；组件和 Timeline 类型使用不同内部类名，不会互相冲突。
- 同一台相机只能启用其中一种管线对应的组件。
