using System.Collections.Generic;
using GameBase; // 引入包含 SingletonBehaviour 的命名空间
using TEngine;
using UnityEngine;

namespace GameLogic
{
    /// <summary>
    /// AABB碰撞管理器，负责检测和处理碰撞事件
    /// </summary>
    public class CollisionManager : SingletonBehaviour<CollisionManager>
    {
        // 存储所有已注册的碰撞体
        private readonly List<AABBCollider> _colliders = new List<AABBCollider>();

        // 按类型分组存储碰撞体，用于优化碰撞检测
        private readonly Dictionary<ColliderType, List<AABBCollider>> _collidersByType =
            new Dictionary<ColliderType, List<AABBCollider>>();

        /// <summary>
        /// 注册一个碰撞体到管理器中
        /// </summary>
        /// <param name="collider">要注册的碰撞体</param>
        public void RegisterCollider(AABBCollider collider)
        {
            if (collider == null || _colliders.Contains(collider))
            {
                return; // 防止重复注册或注册空对象
            }

            _colliders.Add(collider);

            // 将碰撞体添加到按类型分组的列表中
            if (!_collidersByType.TryGetValue(collider.type, out var colliderList))
            {
                colliderList = new List<AABBCollider>();
                _collidersByType[collider.type] = colliderList;
            }
            colliderList.Add(collider);

            // Log.Debug($"Collider registered: {collider.gameObject.name}, Type: {collider.type}");
        }

        /// <summary>
        /// 从管理器中注销一个碰撞体
        /// </summary>
        /// <param name="collider">要注销的碰撞体</param>
        public void UnregisterCollider(AABBCollider collider)
        {
            if (collider == null)
                return;

            _colliders.Remove(collider);

            // 从按类型分组的列表中移除碰撞体
            if (_collidersByType.TryGetValue(collider.type, out var colliderList))
            {
                colliderList.Remove(collider);
            }
            // Log.Debug($"Collider unregistered: {collider.gameObject.name}, Type: {collider.type}");
        }

        /// <summary>
        /// 每帧调用，执行碰撞检测逻辑
        /// </summary>
        private void Update()
        {
            // 如果没有足够的碰撞体类型进行检测，则跳过
            if (!_collidersByType.ContainsKey(ColliderType.Bullet) ||
                !_collidersByType.ContainsKey(ColliderType.Enemy) ||
                _collidersByType[ColliderType.Bullet].Count == 0 ||
                _collidersByType[ColliderType.Enemy].Count == 0)
            {
                return;
            }

            CheckCollisions();
        }

        /// <summary>
        /// 执行碰撞检测的核心逻辑
        /// </summary>
        private void CheckCollisions()
        {
            var bullets = _collidersByType[ColliderType.Bullet];
            var enemies = _collidersByType[ColliderType.Enemy];

            // 使用反向循环，以便在碰撞回调中安全地移除对象（如果需要）
            for (int i = bullets.Count - 1; i >= 0; i--)
            {
                var bullet = bullets[i];
                // 检查子弹对象是否仍然有效（可能在之前的碰撞中被销毁或回收）
                if (bullet == null || !bullet.enabled || !bullet.gameObject.activeInHierarchy)
                    continue;

                for (int j = enemies.Count - 1; j >= 0; j--)
                {
                    var enemy = enemies[j];
                    // 检查敌人对象是否仍然有效
                    if (enemy == null || !enemy.enabled || !enemy.gameObject.activeInHierarchy)
                        continue;

                    // 执行AABB相交检测
                    if (CheckCollision(bullet, enemy))
                    {
                        // Log.Debug($"Collision detected between {bullet.gameObject.name} and {enemy.gameObject.name}");
                        // 触发碰撞双方的回调事件
                        bullet.onCollisionEnter?.Invoke(enemy);
                        enemy.onCollisionEnter?.Invoke(bullet);

                        // 检查回调后对象是否仍然有效，因为回调可能销毁了对象
                        // 如果子弹在回调中被销毁/回收，则跳出内层循环，处理下一个子弹
                        if (bullet == null || !bullet.enabled || !bullet.gameObject.activeInHierarchy)
                            break;
                    }
                }
            }
        }

        /// <summary>
        /// 检测两个AABB碰撞体是否相交
        /// </summary>
        /// <param name="a">第一个碰撞体</param>
        /// <param name="b">第二个碰撞体</param>
        /// <returns>如果相交则返回true，否则返回false</returns>
        private bool CheckCollision(AABBCollider a, AABBCollider b)
        {
            // 获取两个碰撞体的世界空间边界框
            Bounds2D boundsA = a.GetBounds();
            Bounds2D boundsB = b.GetBounds();

            // 使用Bounds2D结构中的Intersects方法进行检测
            return boundsA.Intersects(boundsB);
        }

        /// <summary>
        /// 清理所有注册的碰撞体，在场景切换或游戏结束时调用
        /// </summary>
        public void ClearColliders()
        {
            _colliders.Clear();
            _collidersByType.Clear();
            // Log.Debug("All colliders cleared from CollisionManager.");
        }

        public List<AABBCollider> GetColliders(ColliderType type)
        {
            if (_collidersByType.TryGetValue(type, out var colliders))
            {
                return colliders;
            }
            return null;
        }

        public bool TryGetColliders(ColliderType type, out List<AABBCollider> colliders)
        {
            if (_collidersByType.TryGetValue(type, out colliders))
            {
                return true;
            }
            colliders = null;
            return false;
        }

        // public override void Release()
        // {
        //     ClearColliders();
        //     base.Release(); // 调用基类的释放逻辑
        // }

        private void OnDestroy()
        {
            // 清理所有注册的碰撞体
            ClearColliders();
            Log.Debug("CollisionManager destroyed and all colliders cleared.");
        }
    }
}
