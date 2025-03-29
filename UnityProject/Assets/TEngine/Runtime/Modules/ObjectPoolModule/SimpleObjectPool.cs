using System.Collections.Generic;
using UnityEngine;

namespace TEngine
{
    /// <summary>
    /// 简单对象池接口
    /// </summary>
    /// <typeparam name="T">池化对象类型</typeparam>
    public interface ISimpleObjectPool<T> where T : class
    {
        /// <summary>
        /// 获取对象
        /// </summary>
        /// <returns>池中对象</returns>
        T Get();
        
        /// <summary>
        /// 回收对象
        /// </summary>
        /// <param name="obj">要回收的对象</param>
        void Release(T obj);
        
        /// <summary>
        /// 清理对象池
        /// </summary>
        void Clear();
        
        /// <summary>
        /// 获取池中闲置对象数量
        /// </summary>
        int CountInactive { get; }
    }
    
    /// <summary>
    /// 简单的对象池实现，用于轻量级对象池管理
    /// </summary>
    /// <typeparam name="T">池化对象类型</typeparam>
    public class SimpleObjectPool<T> : ISimpleObjectPool<T> where T : class
    {
        private readonly Stack<T> _pool = new Stack<T>();
        private readonly System.Func<T> _createFunc;
        private readonly System.Action<T> _onGet;
        private readonly System.Action<T> _onRelease;
        private readonly System.Action<T> _onDestroy;
        private readonly int _maxSize;
        private readonly bool _collectionCheck;
        
        /// <summary>
        /// 获取池中闲置对象数量
        /// </summary>
        public int CountInactive => _pool.Count;
        
        /// <summary>
        /// 创建简单对象池
        /// </summary>
        /// <param name="createFunc">创建对象的函数</param>
        /// <param name="onGet">获取对象时的回调</param>
        /// <param name="onRelease">回收对象时的回调</param>
        /// <param name="onDestroy">销毁对象时的回调</param>
        /// <param name="collectionCheck">是否检查重复回收</param>
        /// <param name="maxSize">对象池最大容量</param>
        public SimpleObjectPool(System.Func<T> createFunc, 
            System.Action<T> onGet = null,
            System.Action<T> onRelease = null,
            System.Action<T> onDestroy = null,
            bool collectionCheck = true,
            int maxSize = 10)
        {
            _createFunc = createFunc;
            _onGet = onGet;
            _onRelease = onRelease;
            _onDestroy = onDestroy;
            _collectionCheck = collectionCheck;
            _maxSize = maxSize;
        }
        
        /// <summary>
        /// 从池中获取对象
        /// </summary>
        /// <returns>池中对象</returns>
        public T Get()
        {
            T obj;
            if (_pool.Count == 0)
            {
                obj = _createFunc();
            }
            else
            {
                obj = _pool.Pop();
            }
            
            _onGet?.Invoke(obj);
            return obj;
        }
        
        /// <summary>
        /// 回收对象到池中
        /// </summary>
        /// <param name="obj">要回收的对象</param>
        public void Release(T obj)
        {
            if (obj == null)
            {
                Log.Error("尝试释放空对象到对象池");
                return;
            }
            
            if (_collectionCheck && _pool.Contains(obj))
            {
                Log.Error("尝试释放已在池中的对象");
                return;
            }
            
            _onRelease?.Invoke(obj);
            
            if (_pool.Count < _maxSize)
            {
                _pool.Push(obj);
            }
            else
            {
                _onDestroy?.Invoke(obj);
            }
        }
        
        /// <summary>
        /// 清理对象池中的所有对象
        /// </summary>
        public void Clear()
        {
            if (_onDestroy != null)
            {
                foreach (var item in _pool)
                {
                    _onDestroy(item);
                }
            }
            
            _pool.Clear();
        }
    }
} 