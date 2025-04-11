# PoolManager 代码修改对比

## 修改前

```csharp
using System;
using System.Collections.Generic;
using TEngine;
using UnityEngine;

namespace GameLogic
{
    public class PoolManager : MonoBehaviour
    {
        private static PoolManager _instance;

        public static PoolManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<PoolManager>();
                }

                if (_instance == null)
                {
                    GameObject gameObject = new GameObject();
                    gameObject.name = nameof(PoolManager);
                    _instance = gameObject.AddComponent<PoolManager>();
                    _instance.poolRootObj = gameObject;
                    DontDestroyOnLoad(_instance);
                }

                return _instance;
            }
        }

        [SerializeField] private GameObject poolRootObj;
        // 其余代码...
    }
}
```

## 修改后

```csharp
using System;
using System.Collections.Generic;
using TEngine;
using UnityEngine;
using GameBase; // 添加引用

namespace GameLogic
{
    public class PoolManager : SingletonBehaviour<PoolManager>
    {
        [SerializeField] private GameObject poolRootObj;
        
        // 重写OnLoad方法初始化poolRootObj
        protected override void OnLoad()
        {
            base.OnLoad();
            poolRootObj = gameObject;
        }
        
        // 其余代码...
    }
}
```

## 主要变更

1. **添加命名空间引用**
   - 添加了 `using GameBase;` 引用以使用 SingletonBehaviour 类

2. **修改继承关系**
   - 从 `MonoBehaviour` 改为 `SingletonBehaviour<PoolManager>`

3. **移除自定义单例实现**
   - 删除了私有静态字段 `private static PoolManager _instance;`
   - 删除了自定义的 `public static PoolManager Instance { get {...} }` 属性
   - 删除了 `DontDestroyOnLoad(_instance);` 代码，因为这已在 SingletonBehaviour 中处理

4. **添加 OnLoad 方法**
   - 重写 `OnLoad` 方法来初始化 poolRootObj

## 改造收益

1. **代码统一性**：使用项目标准的单例实现
2. **可维护性**：减少重复代码，集中管理单例生命周期
3. **功能完善**：利用 SingletonSystem 提供的功能