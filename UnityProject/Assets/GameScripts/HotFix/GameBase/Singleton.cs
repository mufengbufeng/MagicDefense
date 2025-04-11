/** docs
* Singleton<T> 类实现了通用单例模式，确保全局只有一个对象实例。
* 
* 主要功能：
* 1. 自动创建和管理单一实例
* 2. 提供全局访问点
* 3. 生命周期管理（包括初始化和释放）
* 4. 在编辑器模式下进行实例化检查
*
* 使用方式：
* - 创建单例类：public class MyManager : Singleton<MyManager> { ... }
* - 访问单例实例：MyManager.Instance.DoSomething();
* - 检查实例有效性：if (MyManager.IsValid) { ... }
* - 释放实例：MyManager.Instance.Release();
*
* 注意事项：
* - 必须通过Instance属性访问单例对象，不要直接实例化
* - 子类可以重写Init(), Active()和Release()方法实现自定义行为
*/

using System.Diagnostics;

namespace GameBase
{
    /// <summary>
    /// 全局对象必须继承于此。
    /// </summary>
    /// <typeparam name="T">子类类型。</typeparam>
    public abstract class Singleton<T> : ISingleton where T : Singleton<T>, new()
    {
        /// <summary>
        /// 单例实例的静态引用
        /// </summary>
        protected static T _instance = default(T);

        /// <summary>
        /// 获取单例实例。如果实例不存在，则创建一个新实例，初始化并注册到SingletonSystem。
        /// </summary>
        public static T Instance
        {
            get
            {
                if (null == _instance)
                {
                    _instance = new T();
                    _instance.Init();
                    SingletonSystem.Retain(_instance);
                }

                return _instance;
            }
        }

        /// <summary>
        /// 检查单例实例是否有效（是否已创建）
        /// </summary>
        public static bool IsValid => _instance != null;

        /// <summary>
        /// 受保护的构造函数，防止外部直接实例化。
        /// 在编辑器模式下检查实例化方式，确保只通过Instance属性创建实例。
        /// </summary>
        protected Singleton()
        {
#if UNITY_EDITOR
            string st = new StackTrace().ToString();
            // using const string to compare simply
            if (!st.Contains("GameBase.Singleton`1[T].get_Instance"))
            {
                UnityEngine.Debug.LogError($"请必须通过Instance方法来实例化{typeof(T).FullName}类");
            }
#endif
        }

        /// <summary>
        /// 初始化方法，在实例首次创建时调用。
        /// 子类可重写此方法以实现自定义初始化逻辑。
        /// </summary>
        protected virtual void Init()
        {
        }

        /// <summary>
        /// 激活方法，当单例需要被激活时调用。
        /// 子类可重写此方法以实现自定义激活逻辑。
        /// </summary>
        public virtual void Active()
        {
        }

        /// <summary>
        /// 释放方法，用于清理单例实例。
        /// 调用此方法将从SingletonSystem中注销实例并将引用设为null。
        /// 子类可重写此方法以实现自定义释放逻辑。
        /// </summary>
        public virtual void Release()
        {
            if (_instance != null)
            {
                SingletonSystem.Release(_instance);
                _instance = null;
            }
        }
    }
}