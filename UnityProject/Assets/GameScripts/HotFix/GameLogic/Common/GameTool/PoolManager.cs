/** docs
* PoolManager类是游戏中的对象池管理器，实现了对象复用以提高性能。
* 主要功能：
* 1. 管理GameObject对象池：包括获取、回收游戏对象
* 2. 管理C#普通对象池：包括获取、回收C#对象
* 3. 提供清理对象池的各种方法
*
* 使用方式：
* - 获取GameObject: PoolManager.Instance.GetGameObject("prefabName")
* - 回收GameObject: PoolManager.Instance.PushGameObject(gameObject)
* - 获取C#对象: PoolManager.Instance.GetObject<T>()
* - 回收C#对象: PoolManager.Instance.PushObject(obj)
*
* 注意事项：
* - 对象回收后会自动禁用并改变父级
* - 获取对象时如不指定父级，会移动到当前活动场景
*/

using System;
using System.Collections.Generic;
using TEngine;
using UnityEngine;
using GameBase;
using Cysharp.Threading.Tasks;
using System.Threading;


namespace GameLogic
{
    /// <summary>
    /// 对象池管理器，继承自SingletonBehaviour实现单例模式
    /// </summary>
    public class PoolManager : SingletonBehaviour<PoolManager>
    {
        /// <summary>
        /// 对象池根节点对象
        /// </summary>
        [SerializeField] private GameObject _poolRootObj;

        /// <summary>
        /// 初始化方法，设置对象池根节点
        /// </summary>
        protected override void OnLoad()
        {
            base.OnLoad();
            _poolRootObj = gameObject;
        }

        /// <summary>
        /// GameObject对象池字典，键为预制体名称，值为对应的对象池数据
        /// </summary>
        public Dictionary<string, GameObjectPoolData> GameObjectPoolDic = new Dictionary<string, GameObjectPoolData>();

        /// <summary>
        /// C#对象池字典，键为类型全名，值为对应的对象池数据
        /// </summary>
        public Dictionary<string, ObjectPoolData> ObjectPoolDic = new Dictionary<string, ObjectPoolData>();

        /// <summary>
        /// 从对象池获取GameObject，如池中没有则加载新对象
        /// </summary>
        /// <param name="assetName">资源名称（预制体名）</param>
        /// <param name="parent">指定父节点，默认为null</param>
        /// <returns>获取或新建的GameObject对象</returns>
        public GameObject GetGameObject(string assetName, Transform parent = null)
        {
            GameObject obj = null;
            if (GameObjectPoolDic.TryGetValue(assetName, out var gameObjectPoolData) && gameObjectPoolData.PoolQueue.Count > 0)
            {
                obj = gameObjectPoolData.GetObj(parent);
            }

            if (obj == null)
            {
                obj = GameModule.Resource.LoadGameObject(assetName, parent: parent);
                obj.name = assetName;
            }
            return obj;
        }

        public async UniTask<GameObject> GetGameObjectAsync(string assetName, Transform parent = null, CancellationToken cancellationToken = default)
        {
            GameObject obj = null;
            if (GameObjectPoolDic.TryGetValue(assetName, out var gameObjectPoolData) && gameObjectPoolData.PoolQueue.Count > 0)
            {
                obj = gameObjectPoolData.GetObj(parent);
            }

            if (obj == null)
            {
                obj = await GameModule.Resource.LoadGameObjectAsync(assetName, parent: parent, cancellationToken: cancellationToken);
                obj.name = assetName;
            }

            return obj;
        }

        /// <summary>
        /// 将GameObject放入对象池中
        /// </summary>
        /// <param name="obj">需要放入池中的对象</param>
        public void PushGameObject(GameObject obj)
        {
            string objName = obj.name;
            if (GameObjectPoolDic.TryGetValue(objName, out var gameObjectPoolData))
            {
                gameObjectPoolData.PushObj(obj);
            }
            else
            {
                GameObjectPoolDic.Add(objName, new GameObjectPoolData(obj, _poolRootObj));
            }
        }

        public void ReleaseGameObject(GameObject obj)
        {
            string objName = obj.name;
            if (GameObjectPoolDic.TryGetValue(objName, out var gameObjectPoolData))
            {
                gameObjectPoolData.PushObj(obj);
            }
            else
            {
                GameObjectPoolDic.Add(objName, new GameObjectPoolData(obj, _poolRootObj));
            }
        }

        /// <summary>
        /// 从对象池获取指定类型的C#对象，如池中没有则创建新对象
        /// </summary>
        /// <typeparam name="T">要获取的对象类型，必须有无参构造函数</typeparam>
        /// <returns>池中获取或新建的对象</returns>
        public T GetObject<T>() where T : class, new()
        {
            return CheckObjectCache<T>() ? (T)ObjectPoolDic[typeof(T).FullName].GetObj() : new T();
        }

        /// <summary>
        /// 将C#对象放入对象池中
        /// </summary>
        /// <param name="obj">需要放入池中的对象</param>
        public void PushObject(object obj)
        {
            string fullName = obj.GetType().FullName;
            if (ObjectPoolDic.ContainsKey(fullName))
            {
                ObjectPoolDic[fullName].PushObj(obj);
            }
            else
            {
                ObjectPoolDic.Add(fullName, new ObjectPoolData(obj));
            }
        }

        /// <summary>
        /// 检查指定类型的对象池中是否有可用对象
        /// </summary>
        /// <typeparam name="T">要检查的对象类型</typeparam>
        /// <returns>如果对象池中有可用对象则返回true，否则返回false</returns>
        private bool CheckObjectCache<T>()
        {
            string fullName = typeof(T).FullName;
            return fullName != null && ObjectPoolDic.ContainsKey(fullName) && ObjectPoolDic[fullName].PoolQueue.Count > 0;
        }

        /// <summary>
        /// 清空指定类型的对象池
        /// </summary>
        /// <param name="clearGameObject">是否清空GameObject对象池</param>
        /// <param name="clearCObject">是否清空C#对象池</param>
        public void Clear(bool clearGameObject = true, bool clearCObject = true)
        {
            if (clearGameObject)
            {
                for (int index = 0; index < _poolRootObj.transform.childCount; ++index)
                {
                    Destroy(_poolRootObj.transform.GetChild(index).gameObject);
                }
                GameObjectPoolDic.Clear();
            }

            if (!clearCObject)
            {
                return;
            }
            ObjectPoolDic.Clear();
        }

        /// <summary>
        /// 清空所有GameObject对象池
        /// </summary>
        public void ClearAllGameObject() => Clear(clearCObject: false);

        /// <summary>
        /// 清空指定名称的GameObject对象池
        /// </summary>
        /// <param name="prefabName">预制体名称</param>
        public void ClearGameObject(string prefabName)
        {
            GameObject obj = _poolRootObj.transform.Find(prefabName).gameObject;
            if (obj == null)
            {
                return;
            }

            Destroy(obj);
            GameObjectPoolDic.Remove(prefabName);
        }

        /// <summary>
        /// 清空指定GameObject的对象池
        /// </summary>
        /// <param name="prefab">预制体对象</param>
        public void ClearGameObject(GameObject prefab) => ClearGameObject(prefab.name);

        /// <summary>
        /// 清空所有C#对象池
        /// </summary>
        public void ClearAllObject() => Clear(false);

        /// <summary>
        /// 清空指定类型的C#对象池
        /// </summary>
        /// <typeparam name="T">要清空的对象类型</typeparam>
        public void ClearObject<T>() => ObjectPoolDic.Remove(typeof(T).FullName);

        /// <summary>
        /// 清空指定类型的C#对象池
        /// </summary>
        /// <param name="type">要清空的对象类型</param>
        public void ClearObject(Type type) => ObjectPoolDic.Remove(type.FullName);
    }

    /// <summary>
    /// C#对象池数据类，管理普通C#对象的缓存队列
    /// </summary>
    public class ObjectPoolData
    {
        /// <summary>
        /// 对象缓存队列
        /// </summary>
        public readonly Queue<object> PoolQueue = new Queue<object>();

        /// <summary>
        /// 构造函数，创建对象池并添加初始对象
        /// </summary>
        /// <param name="obj">初始对象</param>
        public ObjectPoolData(object obj) => PushObj(obj);

        /// <summary>
        /// 将对象放入对象池
        /// </summary>
        /// <param name="obj">需要放入池中的对象</param>
        public void PushObj(object obj) => PoolQueue.Enqueue(obj);

        /// <summary>
        /// 从对象池获取对象
        /// </summary>
        /// <returns>池中的对象</returns>
        public object GetObj() => PoolQueue.Dequeue();
    }

    /// <summary>
    /// GameObject对象池数据类，管理Unity游戏对象的缓存队列
    /// </summary>
    public class GameObjectPoolData
    {
        /// <summary>
        /// 对象池父节点
        /// </summary>
        public readonly GameObject FatherObj;

        /// <summary>
        /// 游戏对象缓存队列
        /// </summary>
        public readonly Queue<GameObject> PoolQueue;

        /// <summary>
        /// 构造函数，创建对象池并添加初始对象
        /// </summary>
        /// <param name="obj">初始对象</param>
        /// <param name="poolRootObj">对象池根节点</param>
        public GameObjectPoolData(GameObject obj, GameObject poolRootObj)
        {
            FatherObj = new GameObject(obj.name);
            FatherObj.transform.SetParent(poolRootObj.transform);
            PoolQueue = new Queue<GameObject>();
            PushObj(obj);
        }

        /// <summary>
        /// 构造函数，使用现有父节点创建对象池
        /// </summary>
        /// <param name="fatherObj">父节点对象</param>
        public GameObjectPoolData(GameObject fatherObj)
        {
            this.FatherObj = fatherObj;
        }

        /// <summary>
        /// 将游戏对象放入对象池
        /// </summary>
        /// <param name="obj">需要放入池中的对象</param>
        public void PushObj(GameObject obj)
        {
            PoolQueue.Enqueue(obj);
            obj.transform.SetParent(FatherObj.transform);
            obj.SetActive(false);
        }

        /// <summary>
        /// 从对象池获取游戏对象
        /// </summary>
        /// <param name="parent">指定父节点，默认为null</param>
        /// <returns>池中的游戏对象</returns>
        public GameObject GetObj(Transform parent = null)
        {
            GameObject go = PoolQueue.Dequeue();
            go.SetActive(true);
            go.transform.SetParent(parent);
            if (parent == null)
            {
                UnityEngine.SceneManagement.SceneManager.MoveGameObjectToScene(go, UnityEngine.SceneManagement.SceneManager.GetActiveScene());
            }

            return go;
        }
    }
}
