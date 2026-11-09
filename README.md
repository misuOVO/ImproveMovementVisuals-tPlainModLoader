# 更好的移动视觉 / Improved Movement Visuals

tPlainModLoader (Terraria 1.4.5) 纯视觉模组。

参考 Workshop 模组「Improved Movement Visuals」（id 2816557864，2024.9 版本）的动作视觉思路，在 tPlainModLoader 上重新实现，**不影响任何游戏数值**。

## 功能

| 功能 | 说明 |
| --- | --- |
| 头部看向鼠标 | 玩家头部跟随鼠标方向转动，左右朝向自动切换 |
| 身体朝向鼠标 | 未移动时身体跟随鼠标转向 |
| 移动时身体倾斜 | 前倾 / 后仰，幅度可调 |
| NPC 倾斜 | 史莱姆跳跃倾斜、僵尸 AI 行走倾斜、小动物 AI 倾斜 |

## 设置

模组设置界面「移动视觉设置」：

- 看向鼠标 / 身体朝向鼠标 —— 开关
- 头部转动幅度 —— 0.00 ~ 1.00
- 移动时身体倾斜 —— 开关
- 倾斜幅度 —— 0.00 ~ 2.00
- NPC 倾斜 —— 开关

配置文件：`Mods/ImproveMovementVisuals/setting.json`

## 安装

把 `ImproveMovementVisuals` 文件夹放到 `Terraria/Mods/` 下，启动 tPlainModLoader 即可。

## 说明

- 仅本地玩家生效（`Main.myPlayer`），不处理多人同步。
- 乘坐坐骑、睡觉、坐着、摆件假人时不介入，避免冲突。
- 与原版 IMV 的 IMVConfigClient 配置体系、多人包同步无关，这里只做单机视觉。

## 致谢

- 原模组：Improved Movement Visuals（Workshop 2816557864）
