using TEngine;
using UnityEngine;

namespace GameLogic
{
    /// <summary>
    /// 敌人移动状态。
    /// </summary>
    public class EnemyMoveState : FsmState<EnemyEntity>
    {
        private EnemyEntity _enemy;
        private PlayerEntity _targetPlayer;
        private const float ATTACK_RANGE = 1.5f; // 攻击范围，固定值

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
                // 尝试查找玩家
                // _targetPlayer = GameObject.FindObjectOfType<PlayerEntity>();
                _targetPlayer = GameSceneManager.Instance.PlayerEntity;
                if (_targetPlayer != null)
                {
                    fsm.SetData("TargetPlayer", _targetPlayer);
                }
                else
                {
                    Log.Warning($"{_enemy.gameObject.name} 无法找到目标玩家，返回待机状态。");
                    return;
                }
            }

            Log.Debug($"{_enemy.gameObject.name} 进入移动状态，目标: {_targetPlayer.gameObject.name}");
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
                // Log.Debug($"{_enemy.gameObject.name} 目标玩家无效，返回待机状态。");
                return;
            }

            // 计算与玩家的距离
            float distanceToPlayer = Vector3.Distance(_enemy.transform.position, _targetPlayer.transform.position);

            // 如果进入攻击范围，切换到攻击状态
            if (distanceToPlayer <= ATTACK_RANGE)
            {
                // Log.Debug($"{_enemy.gameObject.name} 进入攻击范围，切换到攻击状态。");
                ChangeState<EnemyAttackState>(fsm);
                return;
            }

            // 朝玩家移动
            MoveTowardsPlayer(elapseSeconds);
        }

        /// <summary>
        /// 离开状态。
        /// </summary>
        /// <param name="fsm">状态机引用。</param>
        /// <param name="isShutdown">是否是状态机关闭。</param>
        protected override void OnLeave(IFsm<EnemyEntity> fsm, bool isShutdown)
        {
            base.OnLeave(fsm, isShutdown);
            Log.Debug($"{_enemy.gameObject.name} 离开移动状态。");
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
        /// 朝玩家移动。
        /// </summary>
        /// <param name="elapseSeconds">逻辑流逝时间。</param>
        private void MoveTowardsPlayer(float elapseSeconds)
        {
            if (_targetPlayer == null || _enemy == null)
                return;

            // 计算移动方向
            Vector3 direction = (_targetPlayer.transform.position - _enemy.transform.position).normalized;

            // 根据速度和时间计算移动距离
            float moveDistance = _enemy.Speed * elapseSeconds;

            // 更新敌人位置
            _enemy.transform.position += direction * moveDistance;

            // 面向玩家（如果游戏中有朝向的概念）
            // _enemy.transform.forward = direction;
        }
    }
}
