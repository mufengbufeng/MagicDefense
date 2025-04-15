using TEngine;
using UnityEngine;
using System.Collections.Generic;

using System;

namespace GameLogic
{
    /// <summary>
    /// EnemyEntity 类用于表示敌方角色的基本属性和行为。
    /// </summary>
    public class EnemyEntity : MonoBehaviour, ISkillTargetEntity
    {
        /// <summary>
        /// 敌人死亡事件
        /// </summary>
        public event Action OnDeath;

        /// <summary>
        /// 敌人生命值
        /// </summary>
        public int Health { get; set; } = 50;

        /// <summary>
        /// 敌人移动速度
        /// </summary>
        public float Speed { get; set; } = 3f;

        /// <summary>
        /// 敌人攻击力
        /// </summary>
        public int AttackPower { get; set; } = 10;

        /// <summary>
        /// 敌人攻击范围
        /// </summary>
        public float AttackRange { get; set; } = 1.5f;

        /// <summary>
        /// 碰撞体引用
        /// </summary>
        public AABBCollider Collider { get; private set; }

        /// <summary>
        /// 目标玩家
        /// </summary>
        public PlayerEntity TargetPlayer { get; set; }

        /// <summary>
        /// 是否已死亡
        /// </summary>
        public bool IsDead { get; private set; } = false;

        public IFsm<EnemyEntity> Fsm { get; set; }

        void Awake()
        {
            // 获取碰撞体
            Collider = GetComponent<AABBCollider>();
            if (Collider == null)
            {
                // 如果没有碰撞体，添加一个
                Collider = gameObject.AddComponent<AABBCollider>();
            }
            // GameModule.Fsm.HasFsm<EnemyEntity>();
            Fsm = GameModule.Fsm.CreateFsm($"{this.GetInstanceID()}", this, new List<FsmState<EnemyEntity>>
            {
                new EnemyMoveState(),
                new EnemyAttackState(),
                new EnemyDieState(),
            });
            Log.Info("创建敌人状态机成功！");
            Fsm.Start<EnemyMoveState>();

            // 设置碰撞体类型为Enemy
            Collider.type = ColliderType.Enemy;
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
        /// 敌人受到伤害
        /// </summary>
        /// <param name="damage">伤害值</param>
        public void TakeDamage(int damage)
        {
            if (Health <= 0 || IsDead)
                return; // 已经死亡，不再处理伤害

            Health -= damage;
            Log.Debug($"{gameObject.name} 受到 {damage} 点伤害，剩余生命值: {Health}");

            if (Health <= 0)
            {
                // 生命值归零，处理死亡
                Health = 0;
                Die();
            }
        }

        /// <summary>
        /// 攻击指定玩家
        /// </summary>
        /// <param name="player">目标玩家</param>
        public void Attack(PlayerEntity player)
        {
            if (player == null || player.Health <= 0 || IsDead)
                return;

            Log.Debug($"{gameObject.name} 攻击 {player.gameObject.name}，造成 {AttackPower} 点伤害");
            player.TakeDamage(AttackPower);
        }

        /// <summary>
        /// 处理敌人死亡
        /// </summary>
        private void Die()
        {
            if (IsDead)
                return;

            IsDead = true;
            Log.Debug($"{gameObject.name} 死亡");
            GameModule.Fsm.DestroyFsm(Fsm);
            Log.Info($"{gameObject.name} 销毁敌人状态机成功！");

            // 禁用碰撞体
            if (Collider != null)
            {
                Collider.enabled = false;
            }

            // 可以在这里添加死亡动画、特效等
            // 例如：播放死亡动画
            // GetComponent<Animator>().SetTrigger("Die");



            // 触发死亡事件
            OnDeath?.Invoke();
            // 延迟销毁对象（可以替换为对象池回收）
            Destroy(gameObject, 1.5f);
        }
    }
}
