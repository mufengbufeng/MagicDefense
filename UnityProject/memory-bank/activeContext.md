# Active Context

This file tracks the project's current status, including recent changes, current goals, and open questions.
2025-04-10 14:20:00 - Log of updates made.

*

## Current Focus

* 实现敌人向角色移动的逻辑
* 实现角色攻击最近敌人的逻辑
* 处理敌人靠近角色时的停止和攻击行为

## Recent Changes

* 初始化Memory Bank
* 记录了项目中发现的错误：玩家实体状态类（PlayerIdleState, PlayerAttackState, PlayerDieState）未找到

## Open Questions/Issues

* 目前PlayerEntity.cs中引用的状态类未找到，需要创建这些类
* 需要确认敌人的移动方式和判断距离的具体实现方法
* 需要确定角色如何判断并选择最近的敌人


[2025-04-10 14:25:00] - 澄清需求:
* 敌人攻击距离: 固定值 (建议 1.5f)
* 角色目标选择: 直线距离最近
* 伤害类型: 瞬时
* 状态类: 新建
* Player碰撞体: 添加
* 攻击机制的具体实现方式
