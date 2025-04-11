using UnityEngine;

namespace GameLogic
{
    /// <summary>
    /// 2D边界框，用于AABB碰撞检测
    /// </summary>
    public struct Bounds2D
    {
        public Vector2 min;
        public Vector2 max;

        /// <summary>
        /// 检测是否与另一个边界框相交
        /// </summary>
        /// <param name="other">另一个边界框</param>
        /// <returns>如果相交则返回true，否则返回false</returns>
        public bool Intersects(Bounds2D other)
        {
            return !(other.max.x < min.x ||
                     other.min.x > max.x ||
                     other.max.y < min.y ||
                     other.min.y > max.y);
        }
    }
}
