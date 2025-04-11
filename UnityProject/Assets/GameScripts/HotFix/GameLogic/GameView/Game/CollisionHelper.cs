using TEngine;
using UnityEngine;
using System.Collections.Generic;

namespace GameLogic
{
    /// <summary>
    /// 碰撞辅助类，用于处理玩家和敌人之间的交互。
    /// </summary>
    public class CollisionHelper : MonoBehaviour
    {
        private static CollisionHelper _instance;
        public static CollisionHelper Instance
        {
            get
            {
                if (_instance == null)
                {
                    GameObject go = new GameObject("CollisionHelper");
                    _instance = go.AddComponent<CollisionHelper>();
                    DontDestroyOnLoad(go);
                }
                return _instance;
            }
        }

        /// <summary>
        /// 玩家实体引用
        /// </summary>
        public PlayerEntity Player { get; private set; }

        /// <summary>
        /// 敌人列表
        /// </summary>
        private List<EnemyEntity> _enemies = new List<EnemyEntity>();

        /// <summary>
        /// 敌人攻击范围
        /// </summary>
        private const float ENEMY_ATTACK_RANGE = 1.5f;

        /// <summary>
        /// 攻击冷却时间
        /// </summary>
        private const float ATTACK_COOLDOWN = 1.0f;

        /// <summary>
        /// 敌人攻击计时器列表
        /// </summary>
        private Dictionary<EnemyEntity, float> _enemyAttackTimers = new Dictionary<EnemyEntity, float>();

        /// <summary>
        /// 玩家攻击计时器
        /// </summary>
        private float _playerAttackTimer = 0f;

        private void Awake()
        {
            // 确保只有一个实例
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
                return;
            }

            _instance = this;
            DontDestroyOnLoad(gameObject);
        }

        /// <summary>
        /// 注册玩家实体
        /// </summary>
        /// <param name="player">玩家实体</param>
        public void RegisterPlayer(PlayerEntity player)
        {
            Player = player;
            Log.Debug($"玩家已注册: {player.gameObject.name}");
        }

        /// <summary>
        /// 注册敌人实体
        /// </summary>
        /// <param name="enemy">敌人实体</param>
        public void RegisterEnemy(EnemyEntity enemy)
        {
            if (!_enemies.Contains(enemy))
            {
                _enemies.Add(enemy);
                _enemyAttackTimers[enemy] = 0f;
                // Log.Debug($"敌人已注册: {enemy.gameObject.name}");
            }
        }

        /// <summary>
        /// 注销敌人实体
        /// </summary>
        /// <param name="enemy">敌人实体</param>
        public void UnregisterEnemy(EnemyEntity enemy)
        {
            if (_enemies.Contains(enemy))
            {
                _enemies.Remove(enemy);
                if (_enemyAttackTimers.ContainsKey(enemy))
                {
                    _enemyAttackTimers.Remove(enemy);
                }
                Log.Debug($"敌人已注销: {enemy.gameObject.name}");
            }
        }

        private void Update()
        {
            if (Player == null || Player.Health <= 0)
                return;

            // 更新敌人行为
            UpdateEnemies();

            // 更新玩家行为
            UpdatePlayer();
        }

        /// <summary>
        /// 更新敌人行为
        /// </summary>
        private void UpdateEnemies()
        {
            // 遍历所有敌人
            for (int i = _enemies.Count - 1; i >= 0; i--)
            {
                EnemyEntity enemy = _enemies[i];
                if (enemy == null || enemy.Health <= 0)
                {
                    _enemies.RemoveAt(i);
                    continue;
                }

                // 计算与玩家的距离
                float distance = Vector3.Distance(enemy.transform.position, Player.transform.position);

                // 如果在攻击范围内
                if (distance <= ENEMY_ATTACK_RANGE)
                {
                    // 更新攻击计时器
                    if (_enemyAttackTimers.ContainsKey(enemy))
                    {
                        _enemyAttackTimers[enemy] += Time.deltaTime;

                        // 如果冷却时间已过，执行攻击
                        if (_enemyAttackTimers[enemy] >= ATTACK_COOLDOWN)
                        {
                            enemy.Attack(Player);
                            _enemyAttackTimers[enemy] = 0f;
                        }
                    }
                }
                else
                {
                    // 不在攻击范围内，向玩家移动
                    MoveEnemyTowardsPlayer(enemy);
                }
            }
        }

        /// <summary>
        /// 更新玩家行为
        /// </summary>
        private void UpdatePlayer()
        {
            // 更新玩家攻击计时器
            _playerAttackTimer += Time.deltaTime;

            // 如果冷却时间已过且有敌人
            if (_playerAttackTimer >= ATTACK_COOLDOWN && _enemies.Count > 0)
            {
                // 查找最近的敌人
                EnemyEntity nearestEnemy = FindNearestEnemy();
                if (nearestEnemy != null)
                {
                    // 执行攻击
                    Player.Attack(nearestEnemy);
                    _playerAttackTimer = 0f;
                }
            }
        }

        /// <summary>
        /// 敌人向玩家移动
        /// </summary>
        /// <param name="enemy">敌人实体</param>
        private void MoveEnemyTowardsPlayer(EnemyEntity enemy)
        {
            if (enemy == null || Player == null)
                return;

            // 计算方向
            Vector3 direction = (Player.transform.position - enemy.transform.position).normalized;

            // 移动敌人
            enemy.transform.position += direction * enemy.Speed * Time.deltaTime;
        }

        /// <summary>
        /// 查找离玩家最近的敌人
        /// </summary>
        /// <returns>最近的敌人实体</returns>
        private EnemyEntity FindNearestEnemy()
        {
            EnemyEntity nearest = null;
            float minDistance = float.MaxValue;

            foreach (var enemy in _enemies)
            {
                if (enemy != null && enemy.Health > 0)
                {
                    float distance = Vector3.Distance(Player.transform.position, enemy.transform.position);
                    if (distance < minDistance)
                    {
                        minDistance = distance;
                        nearest = enemy;
                    }
                }
            }

            return nearest;
        }
    }
}
