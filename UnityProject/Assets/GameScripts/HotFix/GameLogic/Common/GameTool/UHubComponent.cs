using System.Collections.Generic;
using System.Reflection;
using TEngine;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace GameLogic
{
    public class UHubComponent : MonoBehaviour
    {


        /// <summary>
        /// 通过反射 自动获取ReferenceCollector值并附加到UI上 
        /// </summary>
        /// <param name="self"></param>
        /// <param name="Go"></param>
        public void BindUI(System.Object self, GameObject Go)
        {
            var rc = Go.GetComponent<ReferenceCollector>();
            if (rc == null)
            {
                Debug.LogError("ReferenceCollector is null");
                return;
            }

            foreach (var item in rc.data)
            {
                // 使用反射获取字段 public private instance
                var field = self.GetType().GetField(item.key, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                // 

                if (field != null)
                {
                    // 获取字段类型
                    var fieldType = field.FieldType;
                    // 获取字段值
                    var fieldValue = field.GetValue(self);

                    // 判断字段类型是否为GameObject

                    GameObject go = rc.Get<GameObject>(item.key);

                    if (go != null)
                    {
                        if (typeof(GameObject) == fieldType)
                        {
                            // 如果字段类型是GameObject，直接赋值
                            field.SetValue(self, go);
                            Debug.Log($"获取GameObject: {item.key}");
                        }
                        else if (fieldType.IsSubclassOf(typeof(Component)))
                        {
                            // 如果字段类型是Component，获取组件并赋值
                            var component = go.GetComponent(fieldType);
                            field.SetValue(self, component);
                            Debug.Log($"获取Component: {item.key}");
                        }
                        else
                        {
                            Debug.LogWarning($"无法获取GameObject: {item.key}");
                        }
                        // field.SetValue(self, go);
                    }
                    else
                    {
                        Debug.LogWarning($"无法获取GameObject: {item.key}");
                    }
                }
            }
        }

        public void BindClick(Button button, System.Action action)
        {
            Log.Info($"绑定按钮点击事件: {button.name}");
            if (button == null)
            {
                Log.Error("button is null");
                return;
            }
            if (action == null)
            {
                return;
            }
            // 绑定按钮点击事件
            Log.Info($"绑定按钮点击事件: {button.name}");
            button.onClick.AddListener(() => action.Invoke());
            RemoveList.Add(() => button.onClick.RemoveListener(() => action.Invoke()));
        }


        public List<UnityAction> RemoveList = new();
        public void BindAction<T>(UnityAction<T> callBack, UnityAction<UnityAction<T>> bindAction, UnityAction<UnityAction<T>> offAction)
        {
            if (callBack == null)
            {
                Debug.LogError("callBack is null");
                return;
            }
            if (bindAction == null)
            {
                Debug.LogError("bindAction is null");
                return;
            }
            if (offAction == null)
            {
                Debug.LogError("offAction is null");
                return;
            }

            bindAction(callBack);
            RemoveList.Add(() => offAction(callBack));
        }


        private void OnDestroy()
        {
            foreach (var item in RemoveList)
            {
                item?.Invoke();
            }

            RemoveList.Clear();
        }
    }
}
