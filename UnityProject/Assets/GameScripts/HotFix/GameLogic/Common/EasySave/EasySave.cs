using System;
using System.Collections.Generic;
using UnityEngine;

namespace GameLogic
{
    /// <summary>
    /// 简易本地存储工具，封装PlayerPrefs，支持基本数据类型和复杂数据类型的存取
    /// </summary>
    public static class EasySave
    {
        #region 基本数据类型

        /// <summary>
        /// 保存整数值
        /// </summary>
        /// <param name="key">键名</param>
        /// <param name="value">要保存的整数值</param>
        public static void SaveInt(string key, int value)
        {
            try
            {
                PlayerPrefs.SetInt(key, value);
                PlayerPrefs.Save();
            }
            catch (Exception e)
            {
                Debug.LogError($"保存整数值时出错: {e.Message}");
            }
        }

        /// <summary>
        /// 获取整数值
        /// </summary>
        /// <param name="key">键名</param>
        /// <param name="defaultValue">如果键不存在时返回的默认值</param>
        /// <returns>获取的整数值，如果键不存在则返回默认值</returns>
        public static int GetInt(string key, int defaultValue = 0)
        {
            try
            {
                return PlayerPrefs.GetInt(key, defaultValue);
            }
            catch (Exception e)
            {
                Debug.LogError($"获取整数值时出错: {e.Message}");
                return defaultValue;
            }
        }

        /// <summary>
        /// 保存浮点数值
        /// </summary>
        /// <param name="key">键名</param>
        /// <param name="value">要保存的浮点数值</param>
        public static void SaveFloat(string key, float value)
        {
            try
            {
                PlayerPrefs.SetFloat(key, value);
                PlayerPrefs.Save();
            }
            catch (Exception e)
            {
                Debug.LogError($"保存浮点数值时出错: {e.Message}");
            }
        }

        /// <summary>
        /// 获取浮点数值
        /// </summary>
        /// <param name="key">键名</param>
        /// <param name="defaultValue">如果键不存在时返回的默认值</param>
        /// <returns>获取的浮点数值，如果键不存在则返回默认值</returns>
        public static float GetFloat(string key, float defaultValue = 0f)
        {
            try
            {
                return PlayerPrefs.GetFloat(key, defaultValue);
            }
            catch (Exception e)
            {
                Debug.LogError($"获取浮点数值时出错: {e.Message}");
                return defaultValue;
            }
        }

        /// <summary>
        /// 保存字符串值
        /// </summary>
        /// <param name="key">键名</param>
        /// <param name="value">要保存的字符串值</param>
        public static void SaveString(string key, string value)
        {
            try
            {
                PlayerPrefs.SetString(key, value);
                PlayerPrefs.Save();
            }
            catch (Exception e)
            {
                Debug.LogError($"保存字符串值时出错: {e.Message}");
            }
        }

        /// <summary>
        /// 获取字符串值
        /// </summary>
        /// <param name="key">键名</param>
        /// <param name="defaultValue">如果键不存在时返回的默认值</param>
        /// <returns>获取的字符串值，如果键不存在则返回默认值</returns>
        public static string GetString(string key, string defaultValue = "")
        {
            try
            {
                return PlayerPrefs.GetString(key, defaultValue);
            }
            catch (Exception e)
            {
                Debug.LogError($"获取字符串值时出错: {e.Message}");
                return defaultValue;
            }
        }

        /// <summary>
        /// 保存布尔值
        /// </summary>
        /// <param name="key">键名</param>
        /// <param name="value">要保存的布尔值</param>
        public static void SaveBool(string key, bool value)
        {
            try
            {
                // PlayerPrefs不直接支持布尔值，将其转换为整数存储
                PlayerPrefs.SetInt(key, value ? 1 : 0);
                PlayerPrefs.Save();
            }
            catch (Exception e)
            {
                Debug.LogError($"保存布尔值时出错: {e.Message}");
            }
        }

        /// <summary>
        /// 获取布尔值
        /// </summary>
        /// <param name="key">键名</param>
        /// <param name="defaultValue">如果键不存在时返回的默认值</param>
        /// <returns>获取的布尔值，如果键不存在则返回默认值</returns>
        public static bool GetBool(string key, bool defaultValue = false)
        {
            try
            {
                // PlayerPrefs不直接支持布尔值，将整数转换为布尔值
                int value = PlayerPrefs.GetInt(key, defaultValue ? 1 : 0);
                return value != 0;
            }
            catch (Exception e)
            {
                Debug.LogError($"获取布尔值时出错: {e.Message}");
                return defaultValue;
            }
        }

        #endregion

        #region 复杂数据类型

        /// <summary>
        /// 保存数组
        /// </summary>
        /// <typeparam name="T">数组元素类型</typeparam>
        /// <param name="key">键名</param>
        /// <param name="array">要保存的数组</param>
        public static void SaveArray<T>(string key, T[] array)
        {
            try
            {
                // 创建一个包装类来序列化数组，因为JsonUtility不能直接序列化数组
                var wrapper = new ArrayWrapper<T> { array = array };
                string json = JsonUtility.ToJson(wrapper);
                PlayerPrefs.SetString(key, json);
                PlayerPrefs.Save();
            }
            catch (Exception e)
            {
                Debug.LogError($"保存数组时出错: {e.Message}");
            }
        }

        /// <summary>
        /// 获取数组
        /// </summary>
        /// <typeparam name="T">数组元素类型</typeparam>
        /// <param name="key">键名</param>
        /// <param name="defaultValue">如果键不存在时返回的默认数组</param>
        /// <returns>获取的数组，如果键不存在则返回默认数组</returns>
        public static T[] GetArray<T>(string key, T[] defaultValue = null)
        {
            try
            {
                if (!PlayerPrefs.HasKey(key))
                {
                    return defaultValue ?? new T[0];
                }

                string json = PlayerPrefs.GetString(key);
                ArrayWrapper<T> wrapper = JsonUtility.FromJson<ArrayWrapper<T>>(json);
                return wrapper.array;
            }
            catch (Exception e)
            {
                Debug.LogError($"获取数组时出错: {e.Message}");
                return defaultValue ?? new T[0];
            }
        }

        /// <summary>
        /// 保存二维数组
        /// </summary>
        /// <typeparam name="T">数组元素类型</typeparam>
        /// <param name="key">键名</param>
        /// <param name="array2D">要保存的二维数组</param>
        public static void SaveArray2D<T>(string key, T[,] array2D)
        {
            try
            {
                int rows = array2D.GetLength(0);
                int cols = array2D.GetLength(1);
                T[] flatArray = new T[rows * cols];

                // 将二维数组扁平化为一维数组
                for (int i = 0; i < rows; i++)
                {
                    for (int j = 0; j < cols; j++)
                    {
                        flatArray[i * cols + j] = array2D[i, j];
                    }
                }

                // 创建包装类并序列化
                var wrapper = new Array2DWrapper<T>
                {
                    rows = rows,
                    cols = cols,
                    data = flatArray
                };

                string json = JsonUtility.ToJson(wrapper);
                PlayerPrefs.SetString(key, json);
                PlayerPrefs.Save();
            }
            catch (Exception e)
            {
                Debug.LogError($"保存二维数组时出错: {e.Message}");
            }
        }

        /// <summary>
        /// 获取二维数组
        /// </summary>
        /// <typeparam name="T">数组元素类型</typeparam>
        /// <param name="key">键名</param>
        /// <param name="defaultValue">如果键不存在时返回的默认二维数组</param>
        /// <returns>获取的二维数组，如果键不存在则返回默认二维数组</returns>
        public static T[,] GetArray2D<T>(string key, T[,] defaultValue = null)
        {
            try
            {
                if (!PlayerPrefs.HasKey(key))
                {
                    return defaultValue;
                }

                string json = PlayerPrefs.GetString(key);
                Array2DWrapper<T> wrapper = JsonUtility.FromJson<Array2DWrapper<T>>(json);

                // 从扁平数组重建二维数组
                T[,] result = new T[wrapper.rows, wrapper.cols];
                for (int i = 0; i < wrapper.rows; i++)
                {
                    for (int j = 0; j < wrapper.cols; j++)
                    {
                        result[i, j] = wrapper.data[i * wrapper.cols + j];
                    }
                }

                return result;
            }
            catch (Exception e)
            {
                Debug.LogError($"获取二维数组时出错: {e.Message}");
                return defaultValue;
            }
        }

        /// <summary>
        /// 保存字典
        /// </summary>
        /// <typeparam name="TKey">字典键类型</typeparam>
        /// <typeparam name="TValue">字典值类型</typeparam>
        /// <param name="key">键名</param>
        /// <param name="dictionary">要保存的字典</param>
        public static void SaveDictionary<TKey, TValue>(string key, Dictionary<TKey, TValue> dictionary)
        {
            try
            {
                // 将字典转换为可序列化的键值对列表
                List<DictionaryEntry<TKey, TValue>> entries = new List<DictionaryEntry<TKey, TValue>>();
                foreach (var kvp in dictionary)
                {
                    entries.Add(new DictionaryEntry<TKey, TValue> { key = kvp.Key, value = kvp.Value });
                }

                // 创建一个包装类来序列化列表
                var wrapper = new DictionaryWrapper<TKey, TValue> { entries = entries };
                string json = JsonUtility.ToJson(wrapper);
                PlayerPrefs.SetString(key, json);
                PlayerPrefs.Save();
            }
            catch (Exception e)
            {
                Debug.LogError($"保存字典时出错: {e.Message}");
            }
        }

        /// <summary>
        /// 获取字典
        /// </summary>
        /// <typeparam name="TKey">字典键类型</typeparam>
        /// <typeparam name="TValue">字典值类型</typeparam>
        /// <param name="key">键名</param>
        /// <param name="defaultValue">如果键不存在时返回的默认字典</param>
        /// <returns>获取的字典，如果键不存在则返回默认字典</returns>
        public static Dictionary<TKey, TValue> GetDictionary<TKey, TValue>(string key, Dictionary<TKey, TValue> defaultValue = null)
        {
            try
            {
                if (!PlayerPrefs.HasKey(key))
                {
                    return defaultValue ?? new Dictionary<TKey, TValue>();
                }

                string json = PlayerPrefs.GetString(key);
                DictionaryWrapper<TKey, TValue> wrapper = JsonUtility.FromJson<DictionaryWrapper<TKey, TValue>>(json);
                
                Dictionary<TKey, TValue> result = new Dictionary<TKey, TValue>();
                foreach (var entry in wrapper.entries)
                {
                    result[entry.key] = entry.value;
                }
                
                return result;
            }
            catch (Exception e)
            {
                Debug.LogError($"获取字典时出错: {e.Message}");
                return defaultValue ?? new Dictionary<TKey, TValue>();
            }
        }

        #endregion

        #region 辅助方法

        /// <summary>
        /// 检查是否存在指定键
        /// </summary>
        /// <param name="key">键名</param>
        /// <returns>如果存在返回true，否则返回false</returns>
        public static bool HasKey(string key)
        {
            try
            {
                return PlayerPrefs.HasKey(key);
            }
            catch (Exception e)
            {
                Debug.LogError($"检查键是否存在时出错: {e.Message}");
                return false;
            }
        }

        /// <summary>
        /// 删除指定键的数据
        /// </summary>
        /// <param name="key">键名</param>
        public static void DeleteKey(string key)
        {
            try
            {
                PlayerPrefs.DeleteKey(key);
                PlayerPrefs.Save();
            }
            catch (Exception e)
            {
                Debug.LogError($"删除键时出错: {e.Message}");
            }
        }

        /// <summary>
        /// 删除所有数据
        /// </summary>
        public static void DeleteAll()
        {
            try
            {
                PlayerPrefs.DeleteAll();
                PlayerPrefs.Save();
            }
            catch (Exception e)
            {
                Debug.LogError($"删除所有数据时出错: {e.Message}");
            }
        }

        #endregion

        #region 序列化辅助类

        // 用于JSON序列化数组的包装类
        [Serializable]
        private class ArrayWrapper<T>
        {
            public T[] array;
        }

        // 用于JSON序列化二维数组的包装类
        [Serializable]
        private class Array2DWrapper<T>
        {
            public int rows;
            public int cols;
            public T[] data;
        }

        // 用于JSON序列化字典的单个键值对
        [Serializable]
        private class DictionaryEntry<TKey, TValue>
        {
            public TKey key;
            public TValue value;
        }

        // 用于JSON序列化字典的包装类
        [Serializable]
        private class DictionaryWrapper<TKey, TValue>
        {
            public List<DictionaryEntry<TKey, TValue>> entries;
        }

        #endregion
    }
}