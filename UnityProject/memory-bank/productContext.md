# Product Context

This file provides a high-level overview of the project and the expected product that will be created. Initially it is based upon projectBrief.md (if provided) and all other available project-related information in the working directory. This file is intended to be updated as the project evolves, and should be used to inform all other modes of the project's goals and context.
2025-04-10 14:20:00 - Log of updates made will be appended as footnotes to the end of this file.

*

## Project Goal

* 开发一个简单的战斗游戏，角色站立在原地，敌人不断向角色靠近并攻击，角色则攻击距离最近的敌人

## Key Features

* 角色站立在原地不动，自动攻击最近的敌人
* 敌人向角色移动，到达攻击距离后停止并攻击角色
* 碰撞检测系统处理实体间的交互
* 战斗逻辑（攻击、伤害计算等）

## Overall Architecture

* 基于Unity引擎开发
* 使用AABB碰撞系统
* 实体系统（玩家、敌人）
* 状态机系统管理实体行为
