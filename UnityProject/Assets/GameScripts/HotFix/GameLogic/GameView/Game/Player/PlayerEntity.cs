
using TEngine;
using UnityEngine;
using System.Collections.Generic;

namespace GameLogic
{
    /// <summary>
    /// PlayerEntity类用于表示玩家角色的基本属性和行为
    /// </summary>
    public class PlayerEntity : MonoBehaviour
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
        /// 玩家攻击力
        /// </summary>
        public int AttackPower { get; set; } = 10;

        /// <summary>
        /// 碰撞体
        /// </summary>
        public AABBCollider Collider { get; private set; }

        /// <summary>
        /// 是否已死亡
        /// </summary>
        public bool IsDead { get; private set; } = false;
        public IFsm<PlayerEntity> Fsm { get; set; }

        private void Awake()
        {
            // 获取碰撞体
            Collider = gameObject.GetOrAddComponent<AABBCollider>();
            // 设置碰撞体类型为Player
            Collider.type = ColliderType.Player;

            Fsm = GameModule.Fsm.CreateFsm<PlayerEntity>(this, new PlayerIdleState(), new PlayerAttackState());
            Fsm.Start<PlayerIdleState>();

        }

        private void Start()
        {
            // 向碰撞管理器注册自己
            if (CollisionHelper.Instance != null)
            {
                CollisionHelper.Instance.RegisterPlayer(this);
            }
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
        /// 角色受到伤害
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
        /// 攻击指定敌人
        /// </summary>
        /// <param name="enemy">目标敌人</param>
        public void Attack(EnemyEntity enemy)
        {
            if (enemy == null || enemy.Health <= 0 || IsDead)
                return;

            // Log.Debug($"{gameObject.name} 攻击 {enemy.gameObject.name}，造成 {AttackPower} 点伤害");
            enemy.TakeDamage(AttackPower);
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

            // 禁用碰撞体
            if (Collider != null)
            {
                Collider.enabled = false;
            }
            GameModule.Fsm.DestroyFsm(Fsm);

            // 可以在这里添加死亡动画、特效等
            // 例如：播放死亡动画
            // GetComponent<Animator>().SetTrigger("Die");

            // 游戏结束处理
            // 这里可以延迟一段时间后重新开始游戏，或显示游戏结束界面等
            // Invoke("GameOver", 2f);
        }

        /// <summary>
        /// 游戏结束处理
        /// </summary>
        private void GameOver()
        {
            // 游戏结束的处理逻辑
            // 例如：显示游戏结束界面，重新加载场景等
            Log.Debug("游戏结束");

            // 这里可能需要调用GameSceneManager或其他管理器的相关方法
            // GameSceneManager.Instance.GameOver();
        }
    }
}
