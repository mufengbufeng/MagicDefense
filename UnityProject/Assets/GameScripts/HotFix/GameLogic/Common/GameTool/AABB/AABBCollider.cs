using UnityEngine;

namespace GameLogic
{
    /// <summary>
    /// AABB碰撞组件
    /// </summary>
    public class AABBCollider : MonoBehaviour
    {
        [Tooltip("碰撞体大小")]
        public Vector2 size = Vector2.one;
        [Tooltip("碰撞体相对于GameObject中心的偏移量")]
        public Vector2 offset = Vector2.zero;
        [Tooltip("碰撞体类型")]
        public ColliderType type;

        /// <summary>
        /// 碰撞发生时的回调委托
        /// </summary>
        public System.Action<AABBCollider> onCollisionEnter;

        private void Start()
        {
            // 向碰撞管理器注册自身
            CollisionManager.Instance.RegisterCollider(this);
        }

        private void OnDestroy()
        {
            // 从碰撞管理器注销自身
            // 需要检查Instance是否仍然有效，因为对象销毁顺序可能导致CollisionManager先被销毁
            if (CollisionManager.IsValid)
            {
                CollisionManager.Instance.UnregisterCollider(this);
            }
        }

        /// <summary>
        /// 获取当前碰撞体在世界空间中的边界框
        /// </summary>
        /// <returns>世界空间边界框</returns>
        public Bounds2D GetBounds()
        {
            // 计算世界空间中的中心点
            Vector2 worldCenter = (Vector2)transform.position + offset;
            // 计算半尺寸
            Vector2 halfSize = size * 0.5f;

            return new Bounds2D
            {
                min = worldCenter - halfSize,
                max = worldCenter + halfSize
            };
        }

        // 可选：在编辑器中绘制碰撞框辅助线，方便调试
#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            Bounds2D bounds = GetBounds();
            Vector2 center = (bounds.min + bounds.max) * 0.5f;
            Vector2 gizmoSize = bounds.max - bounds.min;

            Gizmos.color = Color.green; // 设置Gizmo颜色
            Gizmos.DrawWireCube(center, gizmoSize);
        }
#endif
    }
}
