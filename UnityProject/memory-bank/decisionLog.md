# Decision Log

This file records architectural and implementation decisions using a list format.
2025-04-10 14:21:00 - Log of updates made.

*

## Decision

* 使用状态机模式管理玩家和敌人的行为
* 敌人将使用简单的AI逻辑向玩家移动并在适当距离攻击
* 玩家将站立在原地，自动选择并攻击最近的敌人

## Rationale

* 状态机模式适合管理游戏实体的不同行为状态
* 简单的AI逻辑便于实现敌人的基础移动和攻击行为
* 自动选择最近敌人的机制简化了玩家控制逻辑

## Implementation Details

* 为PlayerEntity实现状态类：PlayerIdleState, PlayerAttackState, PlayerDieState
* 为EnemyEntity实现移动和攻击逻辑
* 实现距离计算和目标选择算法


[2025-04-10 14:25:00] - 确认实现细节:
* 敌人攻击距离: 固定值 (具体数值待定，建议初始设为 1.5f)
* 角色目标选择: 基于直线距离选择最近的敌人
* 攻击伤害类型: 瞬时伤害
* 状态类: 需要新建 Player 和 Enemy 的状态类
* Player碰撞体: 需要为 Player 添加 ColliderType
* 使用碰撞系统检测攻击范围和实体间距离
