using TEngine;
using UnityEngine;

namespace GameLogic
{
    /// <summary>
    /// 敌人攻击状态。
    /// </summary>
    public class EnemyAttackState : FsmState<EnemyEntity>
    {
        private EnemyEntity _enemy;
        private PlayerEntity _targetPlayer;
        private float _attackCooldownTimer = 0f;
        private const float ATTACK_COOLDOWN = 1.0f; // 攻击冷却时间（秒）
        private const float ATTACK_RANGE = 1.5f; // 攻击范围，固定值
        private const int ATTACK_DAMAGE = 10; // 攻击伤害，可以后续改为从敌人实体获取

        /// <summary>
        /// 状态初始化。
        /// </summary>
        /// <param name="fsm">状态机引用。</param>
        protected override void OnInit(IFsm<EnemyEntity> fsm)
        {
            base.OnInit(fsm);
            _enemy = fsm.Owner;
        }

        /// <summary>
        /// 进入状态。
        /// </summary>
        /// <param name="fsm">状态机引用。</param>
        protected override void OnEnter(IFsm<EnemyEntity> fsm)
        {
            base.OnEnter(fsm);

            // 获取目标玩家
            _targetPlayer = fsm.GetData<PlayerEntity>("TargetPlayer");
            if (_targetPlayer == null)
            {
                _targetPlayer = GameObject.FindObjectOfType<PlayerEntity>();
                if (_targetPlayer != null)
                {
                    fsm.SetData("TargetPlayer", _targetPlayer);
                }
                else
                {
                    Log.Warning($"{_enemy.gameObject.name} 无法找到目标玩家，返回待机状态。");
                    ChangeState<EnemyMoveState>(fsm);
                    return;
                }
            }

            Log.Debug($"{_enemy.gameObject.name} 进入攻击状态，目标: {_targetPlayer.gameObject.name}");
            _attackCooldownTimer = 0f; // 进入状态时重置攻击冷却计时器
            PerformAttack(); // 进入状态立即执行一次攻击
        }

        /// <summary>
        /// 状态轮询。
        /// </summary>
        /// <param name="fsm">状态机引用。</param>
        /// <param name="elapseSeconds">逻辑流逝时间。</param>
        /// <param name="realElapseSeconds">真实流逝时间。</param>
        protected override void OnUpdate(IFsm<EnemyEntity> fsm, float elapseSeconds, float realElapseSeconds)
        {
            base.OnUpdate(fsm, elapseSeconds, realElapseSeconds);

            // 检查目标玩家是否有效
            if (_targetPlayer == null || !_targetPlayer.gameObject.activeInHierarchy || _targetPlayer.Health <= 0)
            {
                Log.Debug($"{_enemy.gameObject.name} 目标玩家无效，返回待机状态。");
                ChangeState<EnemyMoveState>(fsm);
                return;
            }

            // 计算与玩家的距离
            float distanceToPlayer = Vector3.Distance(_enemy.transform.position, _targetPlayer.transform.position);

            // 如果超出攻击范围，切换回移动状态
            if (distanceToPlayer > ATTACK_RANGE)
            {
                Log.Debug($"{_enemy.gameObject.name} 超出攻击范围，切换回移动状态。");
                ChangeState<EnemyMoveState>(fsm);
                return;
            }

            // 攻击冷却计时
            _attackCooldownTimer += elapseSeconds;
            if (_attackCooldownTimer >= ATTACK_COOLDOWN)
            {
                // 冷却结束，执行攻击
                _attackCooldownTimer = 0f;
                PerformAttack();
            }
        }

        /// <summary>
        /// 离开状态。
        /// </summary>
        /// <param name="fsm">状态机引用。</param>
        /// <param name="isShutdown">是否是状态机关闭。</param>
        protected override void OnLeave(IFsm<EnemyEntity> fsm, bool isShutdown)
        {
            base.OnLeave(fsm, isShutdown);
            Log.Debug($"{_enemy.gameObject.name} 离开攻击状态。");
        }

        /// <summary>
        /// 状态销毁。
        /// </summary>
        /// <param name="fsm">状态机引用。</param>
        protected override void OnDestroy(IFsm<EnemyEntity> fsm)
        {
            base.OnDestroy(fsm);
        }

        /// <summary>
        /// 执行攻击动作。
        /// </summary>
        private void PerformAttack()
        {
            if (_targetPlayer == null || _targetPlayer.Health <= 0)
                return;

            Log.Debug($"{_enemy.gameObject.name} 攻击 {_targetPlayer.gameObject.name}。");

            // 调用玩家的受伤方法
            // 暂时直接在这里调用TakeDamage，后续可以添加更复杂的伤害计算逻辑
            _targetPlayer.TakeDamage(ATTACK_DAMAGE);
        }
    }
}
