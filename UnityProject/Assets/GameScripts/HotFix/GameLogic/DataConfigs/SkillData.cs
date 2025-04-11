using UnityEngine;

public enum SkillType { Damage, Buff, Heal, Movement }
public enum TargetType { Self, Enemy, Ally, Point, Direction }
public enum CastType { Manual, Auto, Continuous }
public enum TargetingType { Collision, LockOn, Directional, Area }
public enum TargetFaction { Enemy, Ally, Both, Neutral }

[CreateAssetMenu(fileName = "NewSkillData", menuName = "Game Data/Skill Data")]
public class SkillData : ScriptableObject
{
    [Header("基础信息")]
    public int SkillID;
    public string SkillName;
    [TextArea] public string Description;
    public Sprite Icon;

    [Header("类型与目标")]
    public SkillType Type = SkillType.Damage;
    public TargetType Target = TargetType.Enemy;
    public TargetFaction Faction = TargetFaction.Enemy;
    public TargetingType Targeting = TargetingType.LockOn;

    [Header("施放与消耗")]
    public CastType Cast = CastType.Auto;
    public float Cooldown = 1.0f;
    public float ManaCost = 0f;
    public float CastTime = 0f;

    [Header("效果参数")]
    public float Range = 5.0f;
    public float EffectRadius = 0f;
    public float EffectAngle = 360f;
    public float Duration = 0f;
    public float EffectValue = 10f;
    public float TickRate = 0f;
    public int MaxTargets = 1;

    [Header("关联资源")]
    public GameObject AssociatedPrefab;
    public string HitEffectPrefabName;
    public string CastSoundName;
    public string HitSoundName;

    [Header("其他")]
    public bool Interruptible = true;
    public bool RequiresLineOfSight = true;
}
