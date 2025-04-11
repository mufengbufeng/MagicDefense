# AABB碰撞系统使用示例

本文档提供了如何在游戏中实际使用AABB碰撞系统的详细示例。这个系统专为2D游戏中的子弹与敌人碰撞检测而设计，是一个轻量级且独立于Unity物理系统的解决方案。

## 1. 系统初始化

在使用AABB碰撞系统前，需要确保场景中有`CollisionManager`的实例。由于它继承自`SingletonBehaviour<T>`，我们可以通过以下方式初始化：

```csharp
using GameLogic;
using UnityEngine;

public class GameInitializer : MonoBehaviour
{
    private void Start()
    {
        // 确保CollisionManager实例已经创建
        var collisionManager = CollisionManager.Instance;
        Debug.Log("AABB碰撞系统已初始化");
    }
}
```

## 2. 子弹类实现示例

下面是一个简单的子弹类实现，展示了如何为子弹添加AABB碰撞检测功能：

```csharp
using GameLogic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 10f;          // 子弹速度
    public int damage = 10;            // 子弹伤害值
    private AABBCollider aabbCollider; // AABB碰撞组件引用

    private void Awake()
    {
        // 添加AABB碰撞组件
        aabbCollider = gameObject.AddComponent<AABBCollider>();

        // 配置碰撞组件
        aabbCollider.type = ColliderType.BULLET;
        aabbCollider.size = new Vector2(0.5f, 0.5f); // 根据子弹实际大小调整

        // 注册碰撞回调
        aabbCollider.onCollisionEnter += OnCollision;
    }

    private void Update()
    {
        // 子弹移动逻辑
        transform.Translate(Vector3.right * speed * Time.deltaTime);

        // 超出屏幕范围自动销毁
        if (transform.position.x > 15f)
        {
            RecycleBullet();
        }
    }

    private void OnCollision(AABBCollider other)
    {
        // 检查碰撞对象类型
        if (other.type == ColliderType.ENEMY)
        {
            // 尝试获取敌人组件并造成伤害
            var enemy = other.gameObject.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
            }

            // 子弹碰到敌人后回收
            RecycleBullet();
        }
    }

    private void RecycleBullet()
    {
        // 使用对象池回收子弹
        PoolManager.Instance.PushGameObject(gameObject);
    }

    private void OnDestroy()
    {
        // 移除事件监听，防止内存泄漏
        if (aabbCollider != null)
        {
            aabbCollider.onCollisionEnter -= OnCollision;
        }
    }
}
```

## 3. 敌人类实现示例

下面是一个简单的敌人类实现，展示了如何为敌人添加AABB碰撞检测功能：

```csharp
using GameLogic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public int maxHealth = 100;        // 最大生命值
    public float moveSpeed = 3f;       // 移动速度
    private int currentHealth;         // 当前生命值
    private AABBCollider aabbCollider; // AABB碰撞组件引用

    private void Awake()
    {
        // 初始化生命值
        currentHealth = maxHealth;

        // 添加AABB碰撞组件
        aabbCollider = gameObject.AddComponent<AABBCollider>();

        // 配置碰撞组件
        aabbCollider.type = ColliderType.ENEMY;
        aabbCollider.size = new Vector2(1f, 1f); // 根据敌人实际大小调整

        // 注册碰撞回调
        aabbCollider.onCollisionEnter += OnCollision;
    }

    private void Update()
    {
        // 敌人移动逻辑
        transform.Translate(Vector3.left * moveSpeed * Time.deltaTime);

        // 超出屏幕范围自动销毁
        if (transform.position.x < -15f)
        {
            Destroy(gameObject);
        }
    }

    private void OnCollision(AABBCollider other)
    {
        // 可以在这里处理与其他物体的碰撞
        // 当敌人与子弹碰撞时，伤害逻辑已在子弹中处理
    }

    // 受到伤害的方法，由子弹调用
    public void TakeDamage(int damage)
    {
        currentHealth -= damage;

        // 检查是否死亡
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        // 这里可以添加死亡效果，如粒子、音效等
        Debug.Log($"敌人 {gameObject.name} 被击败");

        // 销毁敌人对象
        Destroy(gameObject);
    }

    private void OnDestroy()
    {
        // 移除事件监听，防止内存泄漏
        if (aabbCollider != null)
        {
            aabbCollider.onCollisionEnter -= OnCollision;
        }
    }
}
```

## 4. 生成器示例

下面是一个简单的敌人和子弹生成器示例，用于测试AABB碰撞系统：

```csharp
using System.Collections;
using GameLogic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public GameObject bulletPrefab;   // 子弹预制体
    public GameObject enemyPrefab;    // 敌人预制体
    public Transform playerPosition;  // 玩家位置（子弹生成点）

    private void Start()
    {
        // 确保CollisionManager已初始化
        var collisionManager = CollisionManager.Instance;

        // 开始生成敌人
        StartCoroutine(SpawnEnemies());
    }

    // 生成子弹的方法，可由玩家输入触发
    public void FireBullet()
    {
        // 从对象池获取子弹对象
        GameObject bullet = PoolManager.Instance.GetGameObject(bulletPrefab.name);

        // 设置子弹位置
        bullet.transform.position = playerPosition.position;
        bullet.SetActive(true);
    }

    // 定期生成敌人的协程
    private IEnumerator SpawnEnemies()
    {
        while (true)
        {
            // 随机等待时间
            yield return new WaitForSeconds(Random.Range(1f, 3f));

            // 创建敌人
            Vector3 spawnPosition = new Vector3(15f, Random.Range(-5f, 5f), 0f);
            GameObject enemy = Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);
        }
    }
}
```

## 5. 在编辑器中可视化AABB碰撞框

AABB碰撞组件内置了Gizmo绘制功能，可以在Unity编辑器中直观地显示碰撞边界。这在调试和开发阶段非常有用。

当你在Unity编辑器中选中带有AABBCollider组件的对象时，场景视图中会显示绿色的线框，表示该对象的AABB碰撞区域。你可以直接在Inspector中调整size和offset属性，立即看到碰撞区域的变化。

## 6. 性能优化提示

1. **空间分割**：当场景中有大量对象需要进行碰撞检测时，可以考虑实现空间分割算法（如四叉树）以减少检测次数。

2. **对象池化**：对于频繁创建和销毁的对象（如子弹），请确保使用对象池技术来减少GC压力。

3. **碰撞优先级**：如果有多种类型的碰撞体，可以根据优先级或重要性来决定碰撞检测的顺序。

4. **禁用不需要的碰撞检测**：对于不在屏幕内或不需要碰撞检测的对象，可以临时禁用其AABBCollider组件。

## 7. 注意事项

1. **单例管理**：CollisionManager使用单例模式，确保在场景中只有一个实例。

2. **事件注册和注销**：在使用onCollisionEnter事件时，务必在对象销毁前注销事件，避免内存泄漏。

3. **坐标系**：AABB碰撞检测基于世界坐标系，确保在使用本地坐标时正确转换。

4. **组件依赖**：AABBCollider组件依赖于CollisionManager，确保在使用AABBCollider之前已经初始化CollisionManager。
