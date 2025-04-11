using TEngine;
using UnityEngine;

namespace GameLogic
{
    /// <summary>
    /// 敌人死亡状态。
    /// </summary>
    public class EnemyDieState : FsmState<EnemyEntity>
    {
        private EnemyEntity _enemy;
        private float _dieAnimationTimer = 0f;
        private const float DIE_ANIMATION_TIME = 1.5f; // 死亡动画/效果持续时间，可以后续配置化

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
            Log.Debug($"{_enemy.gameObject.name} 进入死亡状态。");
            _dieAnimationTimer = 0f;
            DisableEnemy();

            // 可以在这里播放死亡动画，添加死亡特效等
            // 例如，可以添加一个爆炸效果，或者播放死亡声音
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

            _dieAnimationTimer += elapseSeconds;
            if (_dieAnimationTimer >= DIE_ANIMATION_TIME)
            {
                // 死亡动画/效果完成后销毁敌人
                DestroyEnemy();
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
            Log.Debug($"{_enemy.gameObject.name} 离开死亡状态。");
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
        /// 禁用敌人相关组件。
        /// </summary>
        private void DisableEnemy()
        {
            // 禁用碰撞体
            if (_enemy.Collider != null)
            {
                _enemy.Collider.enabled = false;
            }

            // 如果有其他需要禁用的组件，也在这里禁用
            // 例如，可以禁用敌人的移动组件，渲染组件等
        }

        /// <summary>
        /// 销毁敌人对象。
        /// </summary>
        private void DestroyEnemy()
        {
            Log.Debug($"销毁敌人: {_enemy.gameObject.name}");

            // 注销碰撞体
            if (_enemy.Collider != null && CollisionManager.Instance != null)
            {
                CollisionManager.Instance.UnregisterCollider(_enemy.Collider);
            }

            // 这里可以使用对象池进行回收，而不是直接销毁
            // 例如：ObjectPoolManager.Instance.Recycle(_enemy.gameObject);

            // 直接销毁游戏对象
            GameObject.Destroy(_enemy.gameObject);
        }
    }
}
