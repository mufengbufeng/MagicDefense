using TEngine;
using UnityEngine;

namespace GameLogic
{
    /// <summary>
    /// 玩家死亡状态。
    /// </summary>
    public class PlayerDieState : FsmState<PlayerEntity>
    {
        private PlayerEntity _player;
        private float _dieAnimationTimer = 0f;
        private const float DIE_ANIMATION_TIME = 2.0f; // 死亡动画/效果持续时间，可以后续配置化

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
            Log.Debug($"{_player.gameObject.name} 进入死亡状态。");
            _dieAnimationTimer = 0f;
            // 可以在这里播放死亡动画，禁用碰撞体等
            DisablePlayer();
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

            _dieAnimationTimer += elapseSeconds;
            if (_dieAnimationTimer >= DIE_ANIMATION_TIME)
            {
                // 死亡动画/效果完成后的处理
                // 例如：触发游戏结束事件，重新开始游戏等
                Log.Debug($"{_player.gameObject.name} 死亡效果结束，触发游戏结束事件。");
                GameOver();
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
            Log.Debug($"{_player.gameObject.name} 离开死亡状态。");
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
        /// 禁用玩家相关组件。
        /// </summary>
        private void DisablePlayer()
        {
            // 禁用碰撞体
            if (_player.Collider != null)
            {
                _player.Collider.enabled = false;
            }

            // 可以在这里播放死亡动画，禁用其他组件等
            // ...
        }

        /// <summary>
        /// 游戏结束处理。
        /// </summary>
        private void GameOver()
        {

            // 游戏结束的处理逻辑
            // 例如：显示游戏结束界面，重新加载场景等
            // ...

            // 这里可能需要调用GameSceneManager或其他管理器的相关方法
            // GameSceneManager.Instance.GameOver();
        }
    }
}
