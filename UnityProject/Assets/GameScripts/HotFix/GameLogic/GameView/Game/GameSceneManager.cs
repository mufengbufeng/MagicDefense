/** docs
* GameSceneView类用于管理游戏场景视图和核心游戏元素。
*
* 主要功能：
* 1. 初始化游戏场景视图
* 2. 管理场景中的关键游戏对象
* 3. 处理玩家加载逻辑
*
* 使用方式：
* - 获取实例：GameSceneView.Instance
* - 初始化场景：GameSceneView.Instance.Init()
* - 加载玩家：GameSceneView.Instance.LoadPlayer()
*
* 注意事项：
* - 该类是一个部分类(partial class)，可能在多个文件中定义
* - 使用SingletonBehaviour实现单例模式
*/

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using GameBase;
using GameConfig;
using JetBrains.Annotations;
using TEngine;
using UnityEngine;

namespace GameLogic
{
    /// <summary>
    /// 游戏场景视图类，定义场景关键元素
    /// </summary>
    public partial class GameSceneManager
    {
        /// <summary>
        /// 游戏根节点对象
        /// </summary>
        public GameObject GameRoot;

        /// <summary>
        /// 引用收集器组件，用于管理场景中的引用
        /// </summary>
        public ReferenceCollector Rc;

        /// <summary>
        /// 地图对象
        /// </summary>
        public GameObject Map;

        /// <summary>
        /// 单位对象
        /// </summary>
        public GameObject Unit;

        public GameObject PlayerRoot;
        public GameObject EnemyRoot;

        public List<EnemyEntity> EnemyEntities = new List<EnemyEntity>();
        /// <summary>
        /// 敌人生成时间间隔（秒）
        /// </summaryfsm.Owner
        private const int ENEMY_SPAWN_INTERVAL = 2000;

        /// <summary>
        /// 敌人最大数量
        /// </summary>
        private const int MAX_ENEMY_COUNT = 2;

        private CancellationTokenSource _EnemyCancel;

        public PlayerEntity PlayerEntity { get; set; }

    }

    /// <summary>
    /// 游戏场景视图类的实现部分，继承自SingletonBehaviour实现单例模式
    /// </summary>
    public partial class GameSceneManager : SingletonBehaviour<GameSceneManager>
    {


        /// <summary>
        /// 初始化游戏场景视图
        /// </summary>
        /// <remarks>
        /// 查找并设置GameRoot，获取ReferenceCollector组件
        /// 添加并绑定UHubComponent组件
        /// 加载玩家
        /// </remarks>
        public void Init()
        {
            GameRoot = GameObject.Find("GameRoot");
            Rc = GameRoot.GetComponent<ReferenceCollector>();

            if (Rc != null)
            {
                var uHubComponent = transform.GetOrAddComponent<UHubComponent>();
                uHubComponent.BindUI(this, GameRoot);
            }
            Log.Info("GameSceneView Init");
            LoadPlayer().Forget();

            _EnemyCancel = new CancellationTokenSource();
            // 开始生成敌人
            LoadEnemyAsync(_EnemyCancel).Forget();
        }



        private async UniTask LoadEnemyAsync(CancellationTokenSource token)
        {
            if (token.IsCancellationRequested)
            {
                await UniTask.CompletedTask;
            }
            if (EnemyEntities.Count < MAX_ENEMY_COUNT)
            {
                SpawnEnemy();
            }
            await UniTask.Delay(ENEMY_SPAWN_INTERVAL, cancellationToken: token.Token);
            await LoadEnemyAsync(token);
        }

        /// <summary>
        /// 生成敌人
        /// </summary>
        private void SpawnEnemy()
        {
            // 生成敌人，使用"Enemy"作为默认名称
            LoadEnemy("Enemy1").Forget();
            // Log.Debug("生成了一个敌人");
        }

        public async Task<GameObject> LoadObj(string name)
        {
            GameObject obj = await PoolManager.Instance.GetGameObjectAsync(name, Unit.transform);
            return obj;
        }

        /// <summary>
        /// 加载玩家相关资源和设置
        /// </summary>
        /// <remarks>
        /// 使用对象池管理器加载和初始化玩家资源
        /// 注意：此方法尚未完成实现
        /// </remarks>
        private async UniTask LoadPlayer()
        {
            var player = await PoolManager.Instance.GetGameObjectAsync("Player", Unit.transform);
            player.transform.localPosition = new Vector3(PlayerRoot.transform.position.x, PlayerRoot.transform.position.y, 0);
            PlayerEntity = player.GetOrAddComponent<PlayerEntity>();
        }

        private async UniTask LoadEnemy(string name)
        {
            var enemy = await PoolManager.Instance.GetGameObjectAsync(name, EnemyRoot.transform);

            var pos = new Vector3(Random.Range(-5, 5), Random.Range(-5, 5), 0);
            var collider = enemy.GetOrAddComponent<AABBCollider>();
            collider.type = ColliderType.Enemy;
            var enemyEntity = enemy.AddComponent<EnemyEntity>();

            // 将新生成的敌人添加到列表中以便跟踪
            EnemyEntities.Add(enemyEntity);

            // 设置敌人位置
            enemy.transform.localPosition = pos;

            // 注册敌人死亡事件监听，在敌人死亡时从列表中移除
            enemyEntity.OnDeath += () =>
            {
                EnemyEntities.Remove(enemyEntity);
                Log.Debug($"敌人 {enemy.name} 已死亡，当前敌人数量: {EnemyEntities.Count}");
            };
        }

    }
}
