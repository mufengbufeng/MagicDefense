using UnityEngine;

/// <summary>
/// 技能施法类型
/// </summary>
public enum SkillType
{
    Damage, // 伤害技能
    Buff,   // 增益技能
}
/// <summary>
/// 技能目标类型
/// </summary>
public enum TargetType
{
    Self,   // 自身
    Enemy,  // 敌人
    Ally,   // 友方
    Point,  // 指定点
    Direction, // 指定方向
}

/// <summary>
/// 技能施法类型
/// </summary>
public enum CastType
{
    Manual, // 手动施法
    Auto,   // 自动施法
}

/// <summary>
/// 技能施法方式
/// </summary>
public enum TargetingType
{
    Area,// 范围施法类型,范围内的目标
    Collision, // 碰撞施法类型,碰撞到的目标 子弹
    LockOn, // 锁定目标
}

/// <summary>
/// 锁定条件类型
/// </summary>
public enum LockConditionType
{
    Health,  // 血量
    Distance // 距离
}

/// <summary>
/// 排序方式
/// </summary>
public enum SortOrder
{
    Ascending,  // 正序
    Descending  // 倒序
}

[CreateAssetMenu(fileName = "NewSkillData", menuName = "Game Data/Skill Data")]
public class SkillData : ScriptableObject
{
    [Header("基础信息")]
    [Tooltip("技能唯一标识符")]
    public int SkillID;
    [Tooltip("技能名称")]
    public string SkillName;
    [Tooltip("技能描述")]
    [TextArea] public string Description;
    [Tooltip("技能图标")]
    public Sprite Icon;
    [Tooltip("技能预制体")]
    public GameObject Prefab; // 预制体引用


    [Header("类型与目标")]
    [Tooltip("技能类型")]
    public SkillType Type = SkillType.Damage;
    [Tooltip("技能目标类型")]
    public TargetType Target = TargetType.Enemy;
    [Tooltip("技能施法类型")]
    public TargetingType Targeting = TargetingType.LockOn;

    [Header("锁定目标设置")]
    [Tooltip("锁定条件类型")]
    public LockConditionType LockCondition = LockConditionType.Distance;

    [Tooltip("排序方式")]
    public SortOrder TargetSortOrder = SortOrder.Ascending;

    [Header("施放与消耗")]
    [Tooltip("施法方式")]
    public CastType Cast = CastType.Auto;
    [Tooltip("施法冷却时间 ms")]
    public int Cooldown = 1000;
    [Tooltip("施法消耗的魔法值")]
    public int ManaCost = 0;
    [Tooltip("施法时间，单位ms")]
    public int CastTime = 1000;

    [Header("效果参数")]
    [Tooltip("技能施放范围")]
    public float Range = 5.0f;
    [Tooltip("技能效果半径")]
    public float EffectRadius = 0f;

    [Tooltip("技能施放角度")]
    public float EffectAngle = 360f;

    [Tooltip("技能施放持续时间")]
    public float Duration = 0f;
    [Tooltip("技能施放效果值(伤害值)")]
    public float EffectValue = 10f;

    [Tooltip("增加buffId - 所有类型都能增加buff")]
    public int[] SelfBuff = new int[0];

    [Tooltip("触发其他技能 - 技能出发子技能")]
    public int[] TriggerSkills = new int[0];

    [Header("优先级")]
    [Tooltip("技能释放优先级，数值越小优先级越高")]
    public int Priority = 0;

    [Header("其他")]
    [Tooltip("技能施放时是否打断其他技能")]
    public bool Interruptible = false;

    [Tooltip("技能施放时是否需要目标")]
    public bool RequiresLineOfSight = true;
}
