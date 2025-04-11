using TEngine;
using UnityEngine;
using GameLogic;

namespace GameLogic
{
    /// <summary>
    /// 玩家攻击状态。
    /// </summary>
    public class PlayerAttackState : FsmState<PlayerEntity>
    {
        private PlayerEntity _player;
        private EnemyEntity _targetEnemy;
        private float _attackCooldownTimer = 0f;
        private const float ATTACK_COOLDOWN = 1.0f; // 攻击冷却时间（秒），可以后续配置化

        /// <summary>
        /// 状态初始化。
        /// </summary>
        /// <param name="fsm">状态机引用。</param>
        protected override void OnInit(IFsm<PlayerEntity> fsm)
        {
            base.OnInit(fsm);
            _player = fsm.Owner;
        }

        /// <summary>
        /// 进入状态。
        /// </summary>
        /// <param name="fsm">状态机引用。</param>
        protected override void OnEnter(IFsm<PlayerEntity> fsm)
        {
            base.OnEnter(fsm);
            _targetEnemy = fsm.GetData<EnemyEntity>("TargetEnemy");

            if (_targetEnemy == null || !_targetEnemy.gameObject.activeInHierarchy || _targetEnemy.Health <= 0)
            {
                Log.Warning($"{_player.gameObject.name} 进入攻击状态但目标无效，切换回待机。");
                ChangeState<PlayerIdleState>(fsm);
                return;
            }

            // Log.Debug($"{_player.gameObject.name} 进入攻击状态，目标: {_targetEnemy.gameObject.name}");
            _attackCooldownTimer = 0f; // 进入状态立即攻击或重置计时器
            PerformAttack();
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

            // 检查目标是否仍然有效
            if (_targetEnemy == null || !_targetEnemy.gameObject.activeInHierarchy || _targetEnemy.Health <= 0)
            {
                Log.Debug($"{_player.gameObject.name} 攻击目标丢失或死亡，切换回待机。");
                ChangeState<PlayerIdleState>(fsm);
                return;
            }

            _attackCooldownTimer += elapseSeconds;
            if (_attackCooldownTimer >= ATTACK_COOLDOWN)
            {
                // 冷却结束，切换回待机状态重新索敌
                ChangeState<PlayerIdleState>(fsm);
            }
        }

        /// <summary>
        /// 离开状态。
        /// </summary>
        /// <param name="fsm">状态机引用。</param>
        /// <param name="isShutdown">是否是状态机关闭。</param>
        protected override void OnLeave(IFsm<PlayerEntity> fsm, bool isShutdown)
        {
            base.OnLeave(fsm, isShutdown);
            // 清理可能的状态数据
            fsm.RemoveData("TargetEnemy");
            // Log.Debug($"{_player.gameObject.name} 离开攻击状态。");
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
        /// 执行攻击动作。
        /// </summary>
        private async void PerformAttack()
        {
            if (_targetEnemy != null && _targetEnemy.Health > 0)
            {
                var bulletObj = await PoolManager.Instance.GetGameObjectAsync("Bullet1", _player.transform.parent);
                bulletObj.transform.position = _player.transform.position;

                var bullet = bulletObj.GetOrAddComponent<Bullet>();
                bullet.Init(_targetEnemy.transform, 10);
            }
        }
    }
}
