# 战斗系统实施计划

**目标:** 实现一个简单的战斗场景，其中：
*   敌人持续向固定不动的玩家移动。
*   敌人到达指定攻击距离后停止移动并攻击玩家。
*   玩家自动攻击距离最近的敌人。
*   攻击造成瞬时伤害。

**计划步骤:**

1.  **修改碰撞体类型 (`ColliderType.cs`)**:
    *   在 `ColliderType` 枚举中添加 `Player` 类型，以便区分玩家碰撞体。

2.  **创建玩家状态类 (`PlayerState`)**:
    *   在 `Assets/GameScripts/HotFix/GameLogic/GameView/Game/Player/` 目录下创建以下状态类：
        *   `PlayerIdleState.cs`: 处理玩家的待机逻辑。主要负责搜索并确定最近的敌人目标。如果找到目标，切换到攻击状态。
        *   `PlayerAttackState.cs`: 处理攻击逻辑。向当前目标施加瞬时伤害。攻击后可能有冷却时间，冷却结束后返回待机状态重新索敌，或者如果目标死亡/丢失也返回待机状态。
        *   `PlayerDieState.cs`: 处理玩家死亡逻辑。

3.  **创建敌人状态类 (`EnemyState`)**:
    *   在 `Assets/GameScripts/HotFix/GameLogic/GameView/Game/Enemy/` 目录下创建以下状态类：
        *   `EnemyIdleState.cs`: 敌人的初始状态。可以简单地直接转换到移动状态。
        *   `EnemyMoveState.cs`: 处理敌人向玩家移动的逻辑。持续计算与玩家的距离，如果进入攻击范围，切换到攻击状态。
        *   `EnemyAttackState.cs`: 处理攻击逻辑。当与玩家距离小于等于攻击距离（例如 1.5f）时，停止移动并对玩家施加瞬时伤害。攻击后可能有冷却时间，冷却结束后检查距离，如果仍在范围内则继续攻击，否则切换回移动状态。
        *   `EnemyDieState.cs`: 处理敌人死亡逻辑。

4.  **更新玩家实体 (`PlayerEntity.cs`)**:
    *   添加 `AABBCollider` 组件引用，并设置其类型为 `Player`。
    *   实现查找最近敌人的逻辑。可以在 `PlayerIdleState` 的 `OnUpdate` 中实现，遍历 `CollisionManager` 中注册的 `Enemy` 类型碰撞体，计算直线距离并找到最近的一个。
    *   添加攻击方法，例如 `Attack(EnemyEntity target)`，用于在 `PlayerAttackState` 中调用。
    *   添加受伤方法，例如 `TakeDamage(int amount)`，用于被敌人攻击时调用。
    *   添加必要的属性，如攻击力、攻击范围（如果需要，虽然玩家是原地攻击，但可能需要范围来触发攻击状态）、攻击冷却时间。
    *   确保 `Awake` 方法中 `CreateFsm` 使用新建的状态类实例。

5.  **更新敌人实体 (`EnemyEntity.cs`)**:
    *   添加对玩家实体的引用。可以在 `EnemyMoveState` 或 `EnemyAttackState` 的 `OnEnter` 中通过查找场景中的 `PlayerEntity` 来获取。
    *   在 `EnemyMoveState` 中实现朝向玩家移动的逻辑 (例如 `transform.position = Vector3.MoveTowards(...)`)。
    *   在 `EnemyAttackState` 中实现攻击玩家的逻辑，调用玩家的 `TakeDamage` 方法。
    *   添加必要的属性，如攻击力、攻击范围 (固定值 1.5f)、攻击冷却时间、移动速度。
    *   添加受伤方法，例如 `TakeDamage(int amount)`，用于被玩家攻击时调用。
    *   确保 `Awake` 方法中 `CreateFsm` 使用新建的状态类实例。

6.  **伤害与生命值处理**:
    *   在 `PlayerEntity` 和 `EnemyEntity` 的 `TakeDamage` 方法中实现生命值扣减逻辑。
    *   当生命值小于等于 0 时，触发对应实体的死亡状态（切换到 `PlayerDieState` 或 `EnemyDieState`）。

7.  **状态机流程图**:

    *   **玩家状态机 (PlayerFsm)**
        ```mermaid
        stateDiagram-v2
            [*] --> PlayerIdle : 初始化
            PlayerIdle --> PlayerAttack : 发现最近敌人
            PlayerAttack --> PlayerIdle : 攻击完成/目标丢失
            PlayerIdle --> PlayerDie : 生命值 <= 0
            PlayerAttack --> PlayerDie : 生命值 <= 0
        ```

    *   **敌人状态机 (EnemyFsm)**
        ```mermaid
        stateDiagram-v2
            [*] --> EnemyIdle : 初始化
            EnemyIdle --> EnemyMove : 发现玩家
            EnemyMove --> EnemyAttack : 进入攻击范围 (<= 1.5f)
            EnemyAttack --> EnemyMove : 玩家离开攻击范围 (> 1.5f)
            EnemyAttack --> EnemyAttack : 攻击冷却结束 & 仍在范围内
            EnemyMove --> EnemyDie : 生命值 <= 0
            EnemyAttack --> EnemyDie : 生命值 <= 0
            EnemyIdle --> EnemyDie : 生命值 <= 0
        ```

**后续考虑:**

*   **对象池:** 对于敌人和子弹（如果以后玩家发射子弹），考虑使用对象池进行性能优化。
*   **攻击效果:** 目前是瞬时伤害，未来可以加入攻击动画、特效和音效。
*   **配置化:** 将攻击距离、伤害值、冷却时间等数值配置化，方便调整。
*   **更复杂的AI:** 可以引入更复杂的敌人行为，如巡逻、躲避等。
