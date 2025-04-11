using TEngine;
using UnityEngine;
using System.Collections.Generic; // 需要引入List

namespace GameLogic
{
    /// <summary>
    /// 玩家待机状态。
    /// </summary>
    public class PlayerIdleState : FsmState<PlayerEntity>
    {
        private PlayerEntity _player;

        /// <summary>
        /// 状态初始化。
        /// </summary>
        /// <param name="fsm">状态机引用。</param>
        protected override void OnInit(IFsm<PlayerEntity> fsm)

        {
            base.OnInit(fsm);
            _player = fsm.Owner; // 获取状态机持有者
        }

        /// <summary>
        /// 进入状态。
        /// </summary>
        /// <param name="fsm">状态机引用。</param>
        protected override void OnEnter(IFsm<PlayerEntity> fsm)
        {
            base.OnEnter(fsm);
            // Log.Debug($"{_player.gameObject.name}进入待机状态。");
            // 可以在这里停止玩家动画等
        }

        /// <summary>
        /// 状态轮询。
        /// </summary>
        /// <param name="fsm">状态机引用。</param>
        /// <param name="elapseSeconds">逻辑流逝时间。</param>
        /// <param name="realElapseSeconds">真实流逝时间。</param>
        protected override void OnUpdate(IFsm<PlayerEntity> fsm, float elapseSeconds, float realElapseSeconds)
        {
            base.OnUpdate(fsm, elapseSeconds, realElapseSeconds);

            // 查找最近的敌人
            EnemyEntity nearestEnemy = FindNearestEnemy();

            if (nearestEnemy != null)
            {
                // 如果找到敌人，切换到攻击状态
                // 将目标敌人信息传递给攻击状态
                fsm.SetData("TargetEnemy", nearestEnemy);
                ChangeState<PlayerAttackState>(fsm);
            }
            // else 保持待机
        }

        /// <summary>
        /// 离开状态。
        /// </summary>
        /// <param name="fsm">状态机引用。</param>
        /// <param name="isShutdown">是否是状态机关闭。</param>
        protected override void OnLeave(IFsm<PlayerEntity> fsm, bool isShutdown)
        {
            base.OnLeave(fsm, isShutdown);
            // Log.Debug($"{_player.gameObject.name}离开待机状态。");
        }

        /// <summary>
        /// 状态销毁。
        /// </summary>
        /// <param name="fsm">状态机引用。</param>
        protected override void OnDestroy(IFsm<PlayerEntity> fsm)
        {
            base.OnDestroy(fsm);
        }

        /// <summary>
        /// 查找最近的敌人。
        /// </summary>
        /// <returns>最近的敌人实体，如果找不到则返回null。</returns>
        private EnemyEntity FindNearestEnemy()
        {
            EnemyEntity nearest = null;
            float minDistanceSqr = float.MaxValue;
            Vector3 playerPos = _player.transform.position;

            // 从CollisionManager获取所有敌人碰撞体
            // 注意：这里假设CollisionManager有方法可以获取特定类型的碰撞体列表
            // 如果没有，需要修改CollisionManager或采用其他方式获取敌人列表
            if (CollisionManager.Instance != null && CollisionManager.Instance.TryGetColliders(ColliderType.Enemy, out List<AABBCollider> enemyColliders))
            {
                foreach (var enemyCollider in enemyColliders)
                {
                    if (enemyCollider != null && enemyCollider.enabled && enemyCollider.gameObject.activeInHierarchy)
                    {
                        EnemyEntity enemy = enemyCollider.GetComponent<EnemyEntity>();
                        if (enemy != null && enemy.Health > 0) // 确保敌人存活
                        {
                            float distanceSqr = (enemy.transform.position - playerPos).sqrMagnitude;
                            if (distanceSqr < minDistanceSqr)
                            {
                                minDistanceSqr = distanceSqr;
                                nearest = enemy;
                            }
                        }
                    }
                }
            }
            else
            {
                Log.Warning("CollisionManager实例不存在或无法获取敌人列表。");
            }


            return nearest;
        }
    }
}
