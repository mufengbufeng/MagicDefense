# 技能系统文档

## 系统概述
技能系统由三部分组成：
1. **SkillData** - 技能数据定义(SO)
2. **SkillEditorWindow** - Unity编辑器扩展
3. **SkillManager** - 运行时技能管理模块

## 数据结构 (SkillData.cs)

### 枚举定义
```csharp
public enum SkillType { Damage, Buff } // 技能类型
public enum TargetType { Self, Enemy, Ally, Point, Direction } // 目标类型
public enum CastType { Manual, Auto } // 施法类型
public enum TargetingType { Area, Collision, LockOn } // 施法方式
public enum LockConditionType { Health, Distance } // 锁定条件类型
public enum SortOrder { Ascending, Descending } // 排序方式
```

### SkillData 类
```csharp
[CreateAssetMenu(fileName = "NewSkillData", menuName = "Game Data/Skill Data")]
public class SkillData : ScriptableObject
{
    // 基础信息
    public int SkillID; // 唯一ID
    public string SkillName;
    public string Description;
    public Sprite Icon;
    public GameObject Prefab;

    // 类型与目标
    public SkillType Type;
    public TargetType Target;
    public TargetingType Targeting;

    // 锁定目标设置
    public LockConditionType LockCondition;
    public SortOrder TargetSortOrder;

    // 施放与消耗
    public CastType Cast;
    public float Cooldown;
    public float ManaCost;
    public float CastTime;

    // 效果参数
    public float Range;
    public float EffectRadius;
    public float EffectAngle;
    public float Duration;
    public float EffectValue;
    public float TickRate;
    public int MaxTargets;

    // 其他
    public bool Interruptible;
    public bool RequiresLineOfSight;
}
```

## 锁定目标使用示例
```csharp
// 创建一个锁定最近敌人的技能
skill.Targeting = TargetingType.LockOn;
skill.LockCondition = LockConditionType.Distance;
skill.TargetSortOrder = SortOrder.Ascending;

// 创建一个锁定血量最低敌人的技能
skill.Targeting = TargetingType.LockOn;
skill.LockCondition = LockConditionType.Health;
skill.TargetSortOrder = SortOrder.Ascending;
```

## 注意事项
1. 技能ID必须唯一
2. 编辑器路径(`_skillDataPath`)和运行时路径(`_skillDataLoadPath`)不同
3. 运行时加载逻辑尚未实现
4. 技能预制体需要手动关联
5. 锁定目标功能仅当TargetingType为LockOn时生效
