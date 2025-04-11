/** docs
* SingletonSystem 是一个单例管理系统，用于统一管理游戏中的全局对象和DontDestroyOnLoad对象。
* 
* 主要功能：
* 1. 管理ISingleton接口实现的单例对象
* 2. 管理需要DontDestroyOnLoad的GameObject
* 3. 提供统一的对象注册和释放机制
* 4. 支持游戏重启时的资源清理
*
* 使用方式：
* - 注册单例：SingletonSystem.Retain(singletonInstance)
* - 注册GameObject：SingletonSystem.Retain(gameObject)
* - 释放单例：SingletonSystem.Release(singletonInstance)
* - 释放GameObject：SingletonSystem.Release(gameObject)
* - 释放所有对象：SingletonSystem.Release()
* - 重启游戏：SingletonSystem.Restart()
* - 获取GameObject：SingletonSystem.GetGameObject(name)
*
* 注意事项：
* - 单例对象应实现ISingleton接口
* - 系统会自动对注册的GameObject调用DontDestroyOnLoad
* - 释放资源时会按照注册的相反顺序进行处理
*/

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GameBase
{
    /// <summary>
    /// 单例接口，定义单例对象的基本行为
    /// </summary>
    public interface ISingleton
    {
        /// <summary>
        /// 激活接口，通常用于在某个时机手动实例化
        /// </summary>
        void Active();

        /// <summary>
        /// 释放接口
        /// </summary>
        void Release();
    }

    /// <summary>
    /// 框架中的全局对象与Unity场景依赖相关的DontDestroyOnLoad需要统一管理，方便重启游戏时清除工作
    /// </summary>
    public static class SingletonSystem
    {
        /// <summary>
        /// 存储所有注册的单例对象列表
        /// </summary>
        private static List<ISingleton> _singletons;
        
        /// <summary>
        /// 存储所有注册的GameObject字典，键为GameObject的名称
        /// </summary>
        private static Dictionary<string, GameObject> _gameObjects;

        /// <summary>
        /// 注册一个单例对象到系统中
        /// </summary>
        /// <param name="go">实现了ISingleton接口的单例对象</param>
        public static void Retain(ISingleton go)
        {
            if (_singletons == null)
            {
                _singletons = new List<ISingleton>();
            }

            _singletons.Add(go);
        }

        /// <summary>
        /// 注册一个GameObject到系统中，并设置为DontDestroyOnLoad
        /// </summary>
        /// <param name="go">需要注册的GameObject</param>
        public static void Retain(GameObject go)
        {
            if (_gameObjects == null)
            {
                _gameObjects = new Dictionary<string, GameObject>();
            }

            if (_gameObjects.TryAdd(go.name, go))
            {
                if (Application.isPlaying)
                {
                    Object.DontDestroyOnLoad(go);
                }
            }
        }

        /// <summary>
        /// 释放一个已注册的GameObject
        /// </summary>
        /// <param name="go">需要释放的GameObject</param>
        public static void Release(GameObject go)
        {
            if (_gameObjects != null && _gameObjects.ContainsKey(go.name))
            {
                _gameObjects.Remove(go.name);
                Object.Destroy(go);
            }
        }

        /// <summary>
        /// 释放一个已注册的单例对象
        /// </summary>
        /// <param name="go">需要释放的单例对象</param>
        public static void Release(ISingleton go)
        {
            if (_singletons != null && _singletons.Contains(go))
            {
                _singletons.Remove(go);
            }
        }

        /// <summary>
        /// 释放所有注册的GameObject和单例对象，并卸载未使用的资源
        /// </summary>
        public static void Release()
        {
            if (_gameObjects != null)
            {
                foreach (var item in _gameObjects)
                {
                    Object.Destroy(item.Value);
                }

                _gameObjects.Clear();
            }

            if (_singletons != null)
            {
                for (int i = _singletons.Count -1; i >= 0; i--)
                {
                    _singletons[i].Release();
                }

                _singletons.Clear();
            }

            Resources.UnloadUnusedAssets();
        }

        /// <summary>
        /// 通过名称获取已注册的GameObject
        /// </summary>
        /// <param name="name">GameObject的名称</param>
        /// <returns>找到的GameObject，如不存在则返回null</returns>
        public static GameObject GetGameObject(string name)
        {
            GameObject go = null;
            if (_gameObjects != null)
            {
                _gameObjects.TryGetValue(name, out go);
            }
            
            return go;
        }

        /// <summary>
        /// 检查是否包含指定名称的GameObject
        /// </summary>
        /// <param name="name">要检查的GameObject名称</param>
        /// <returns>如果存在则返回true，否则返回false</returns>
        internal static bool ContainsKey(string name)
        {
            if (_gameObjects != null)
            {
                return _gameObjects.ContainsKey(name);
            }

            return false;
        }

        /// <summary>
        /// 重启游戏，释放所有资源并重新加载初始场景
        /// </summary>
        public static void Restart()
        {
            if (Camera.main != null)
            {
                Camera.main.gameObject.SetActive(false);
            }

            Release();
            SceneManager.LoadScene(0);
        }

        /// <summary>
        /// 通过名称获取单例对象
        /// </summary>
        /// <param name="name">单例对象的名称（ToString()的值）</param>
        /// <returns>找到的单例对象，如不存在则返回null</returns>
        internal static ISingleton GetSingleton(string name)
        {
            for (int i = 0; i < _singletons.Count; ++i)
            {
                if (_singletons[i].ToString() == name)
                {
                    return _singletons[i];
                }
            }

            return null;
        }
    }
}