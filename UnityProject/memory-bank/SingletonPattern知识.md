# Unity项目中单例模式最佳实践

## 单例模式重构案例：PoolManager

本文档记录了将 PoolManager 类从自定义单例模式重构为使用标准化 SingletonBehaviour<T> 实现的过程和经验总结。

## 单例模式类型对比

### 1. 自定义单例实现

**优点：**
- 完全控制单例的创建和销毁过程
- 可以根据项目需求量身定制

**缺点：**
- 每个单例类都需要重复编写相似代码
- 容易出现不一致的实现方式
- 难以统一管理单例对象的生命周期
- DontDestroyOnLoad 处理不集中

```csharp
// 自定义单例模式示例
public class MyManager : MonoBehaviour
{
    private static MyManager _instance;
    
    public static MyManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<MyManager>();
                
                if (_instance == null)
                {
                    GameObject gameObject = new GameObject();
                    gameObject.name = nameof(MyManager);
                    _instance = gameObject.AddComponent<MyManager>();
                    DontDestroyOnLoad(_instance);
                }
            }
            return _instance;
        }
    }
}
```

### 2. 通用基类单例实现

**优点：**
- 代码复用，减少重复编写
- 统一管理单例对象
- 集中处理 DontDestroyOnLoad 逻辑
- 统一的生命周期管理
- 更好的错误处理和调试支持

**缺点：**
- 需要额外学习基类的使用方法
- 对基类的依赖性

```csharp
// 通用基类单例模式示例
public class MyManager : SingletonBehaviour<MyManager>
{
    protected override void OnLoad()
    {
        base.OnLoad();
        // 初始化逻辑
    }
}
```

## 重构过程中的经验教训

1. **保持接口一致性**
   - 重构前后，外部代码访问 PoolManager 的方式保持一致（通过 Instance 静态属性）
   - 确保了重构不会影响其他依赖 PoolManager 的代码

2. **生命周期事件处理**
   - 使用 SingletonBehaviour<T> 后，初始化逻辑移到了 OnLoad 方法中
   - 这种方式比在 Instance getter 中初始化更加清晰和可控

3. **资源管理优化**
   - 通过 SingletonSystem，实现了统一管理单例对象，更好地控制了资源的创建和释放
   - 避免了潜在的内存泄漏问题

## SingletonBehaviour<T> 使用指南

### 基本用法

1. 继承 SingletonBehaviour<T>
   ```csharp
   public class MyManager : SingletonBehaviour<MyManager>
   {
   }
   ```

2. 重写 OnLoad 方法执行初始化
   ```csharp
   protected override void OnLoad()
   {
       base.OnLoad();
       // 在这里进行初始化操作
   }
   ```

3. 访问单例实例
   ```csharp
   MyManager manager = MyManager.Instance;
   ```

### 生命周期管理

- **创建**: 首次访问 Instance 属性时自动创建
- **初始化**: 在 OnLoad 方法中进行
- **销毁**: 通过 Release 方法或 SingletonSystem.Release() 方法

### 最佳实践

1. **尽量使用统一的单例实现**
   - 项目中统一使用 SingletonBehaviour<T> 或其他单例基类
   - 避免混合使用多种单例实现方式

2. **适当拆分单例对象职责**
   - 单例不应承担过多责任，应遵循单一职责原则
   - 考虑使用组合而非继承来扩展功能

3. **注意初始化顺序**
   - 单例间可能存在依赖关系，注意初始化顺序
   - 使用延迟初始化或依赖注入解决依赖问题

## 结论

通过将 PoolManager 从自定义单例实现改造为使用 SingletonBehaviour<T>，我们获得了更统一、更易维护的代码结构。这种做法不仅减少了重复代码，还提高了系统的健壮性和可维护性。

对于任何 Unity 项目，推荐采用统一的单例实现方式，以确保代码质量和开发效率。