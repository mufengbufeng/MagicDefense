using TEngine;
using UnityEngine;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;

namespace GameLogic
{
    /// <summary>
    /// PlayerEntity类用于表示玩家角色的基本属性和行为
    /// </summary>
    public class PlayerEntity : MonoBehaviour, ISkillTargetEntity
    {
        /// <summary>
        /// 玩家生命值
        /// </summary>
        public int Health { get; set; } = 100;

        /// <summary>
        /// 玩家移动速度
        /// </summary>
        public float Speed { get; set; } = 0f;  // 初始速度为0 角色不移动

        /// <summary>
        /// 装备的技能ID列表
        /// </summary>
        public List<int> EquippedSkillIDs { get; private set; } = new List<int>();

        private Dictionary<int, long> _skillCooldownTimers = new Dictionary<int, long>();
        // private SkillManager _skillManager;

        /// <summary>
        /// 基础攻击力(无技能时使用)
        /// </summary>
        public int BaseAttackPower { get; set; } = 10;

        /// <summary>
        /// 碰撞体
        /// </summary>
        public AABBCollider Collider { get; private set; }

        /// <summary>
        /// 是否已死亡
        /// </summary>
        public bool IsDead { get; private set; } = false;
        public IFsm<PlayerEntity> Fsm { get; set; }

        void Awake()
        {
            EquipSkill(1);
            // 获取碰撞体
            Collider = gameObject.GetOrAddComponent<AABBCollider>();
            // 设置碰撞体类型为Player
            Collider.type = ColliderType.Player;
            Fsm = GameModule.Fsm.CreateFsm(this, new PlayerIdleState(), new PlayerAttackState());
            Fsm.Start<PlayerIdleState>();
            // _skillManager = gameObject.GetOrAddComponent<SkillManager>();
            // 启动技能冷却计时器
            UpdateTimer().Forget();
        }

        // private void Update()
        // {
        //     UpdateCooldowns(Time.deltaTime);
        // }

        private async UniTaskVoid UpdateTimer()
        {
            while (true)
            {
                await UniTask.Delay(1000); // 每秒更新一次
                UpdateCooldowns(1000);
            }
        }

        private void Start()
        {

        }

        private void OnEnable()
        {
            // 当对象启用时，注册碰撞体
            if (Collider != null && CollisionManager.Instance != null)
            {
                CollisionManager.Instance.RegisterCollider(Collider);
            }

            // 重置状态
            IsDead = false;
        }

        private void OnDisable()
        {
            // 当对象禁用时，注销碰撞体
            if (Collider != null && CollisionManager.Instance != null)
            {
                CollisionManager.Instance.UnregisterCollider(Collider);
            }
        }

        /// <summary>
        /// 装备技能
        /// </summary>
        public void EquipSkill(int skillID)
        {
            if (!EquippedSkillIDs.Contains(skillID))
            {
                EquippedSkillIDs.Add(skillID);
                _skillCooldownTimers[skillID] = 0;
            }
        }

        /// <summary>
        /// 卸载技能
        /// </summary>
        public void UnequipSkill(int skillID)
        {
            if (EquippedSkillIDs.Contains(skillID))
            {
                EquippedSkillIDs.Remove(skillID);
                _skillCooldownTimers.Remove(skillID);
            }
        }

        /// <summary>
        /// 更新所有技能冷却时间
        /// </summary>
        public void UpdateCooldowns(int deltaTime)
        {
            var keys = new List<int>(_skillCooldownTimers.Keys);
            foreach (var skillID in keys)
            {
                if (_skillCooldownTimers[skillID] > 0)
                {
                    _skillCooldownTimers[skillID] -= deltaTime;
                }
            }
        }

        /// <summary>
        /// 检查技能是否冷却完成
        /// </summary>
        public bool IsSkillReady(int skillID)
        {
            bool isReady = _skillCooldownTimers.TryGetValue(skillID, out long timer) && timer <= 0;
            // Log.Info($"技能 {skillID} 冷却状态: {(isReady ? "可用" : "冷却中")} timer: {timer}");
            return isReady;
        }

        /// <summary>
        /// 开始技能冷却
        /// </summary>
        public void StartSkillCooldown(SkillData data)
        {
            if (data != null && _skillCooldownTimers.ContainsKey(data.SkillID))
            {
                Log.Info($"开始技能 {data.SkillID} 冷却，冷却时间: {data.Cooldown} 毫秒");
                _skillCooldownTimers[data.SkillID] = data.Cooldown;
            }
        }

        public async UniTask<List<SkillData>> GetReadySkills()
        {
            List<SkillData> readySkills = new List<SkillData>();
            foreach (var skillID in EquippedSkillIDs)
            {
                // Log.Info($"检查技能 {skillID} 是否可用");
                if (IsSkillReady(skillID))
                {
                    var skillData = await SkillManager.Instance.GetSkillData(skillID);
                    if (skillData != null)
                    {
                        readySkills.Add(skillData);
                    }
                }
            }
            return readySkills;
        }

        public bool CheckSkillRange(SkillData data,Vector3 pos1 , Vector3 pos2)
        {
            if (data == null)
                return false;

            float distance = Vector3.Distance(pos1, pos2);
            return distance <= data.Range;
        }
        /// <summary>
        /// 角色受到伤害
        /// </summary>
        public void TakeDamage(int damage)
        {
            if (Health <= 0 || IsDead)
                return;

            Health -= damage;
            Log.Debug($"{gameObject.name} 受到 {damage} 点伤害，剩余生命值: {Health}");

            if (Health <= 0)
            {
                Health = 0;
                Die();
            }
        }

        /// <summary>
        /// 处理角色死亡
        /// </summary>
        private void Die()
        {
            if (IsDead)
                return;

            IsDead = true;
            Log.Debug($"{gameObject.name} 死亡");

            if (Collider != null)
            {
                Collider.enabled = false;
            }
            GameModule.Fsm.DestroyFsm(Fsm);
        }
    }
}
