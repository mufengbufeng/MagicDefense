# EasySave本地存储工具设计方案

## 概述
EasySave是一个基于Unity PlayerPrefs的简易本地存储工具，专注于提供简单直观的API来存取各种数据类型，包括基本数据类型和复杂数据类型（数组、字典）。

## 总体设计

### 功能列表
- 基本数据类型（整数、浮点数、字符串、布尔值）的存取
- 复杂数据类型（数组、字典）的存取，通过JSON序列化实现
- 辅助功能：检查键是否存在、删除数据等

### 类设计
```
EasySave（静态类）
  - SaveInt/GetInt：整数存取
  - SaveFloat/GetFloat：浮点数存取
  - SaveString/GetString：字符串存取
  - SaveBool/GetBool：布尔值存取
  - SaveArray/GetArray：数组存取
  - SaveDictionary/GetDictionary：字典存取
  - HasKey/DeleteKey/DeleteAll：辅助方法
  - SerializeObject/DeserializeObject：序列化辅助方法
```

## 工作原理

对于基本数据类型（整数、浮点数、字符串、布尔值），EasySave直接使用PlayerPrefs的对应方法进行存取。

对于复杂数据类型（数组、字典），EasySave将它们序列化为JSON字符串，然后以字符串形式存储在PlayerPrefs中，读取时再将字符串反序列化回原始类型。

## 文件结构
```
Assets/GameScripts/HotFix/GameLogic/Common/EasySave/EasySave.cs
```

## 设计考虑

1. **命名空间**：使用GameLogic命名空间，与项目保持一致
2. **静态类**：设计为静态类，方便直接调用
3. **泛型支持**：为数组和字典操作提供泛型支持，增强灵活性
4. **异常处理**：加入适当的异常处理，确保数据存取安全
5. **默认值**：所有Get方法都支持默认值参数，当键不存在时返回默认值
6. **序列化兼容性**：确保序列化机制兼容Unity环境下常用的数据类型

## 实现框架

```csharp
using System.Collections.Generic;
using UnityEngine;
using System;

namespace GameLogic
{
    /// <summary>
    /// 简易本地存储工具，封装PlayerPrefs，支持基本数据类型和复杂数据类型的存取
    /// </summary>
    public static class EasySave
    {
        #region 基本数据类型

        // 整数、浮点数、字符串和布尔值的存取方法

        #endregion

        #region 复杂数据类型

        // 数组和字典的存取方法，通过JSON序列化实现

        #endregion

        #region 辅助方法

        // 检查键是否存在、删除数据等辅助方法

        #endregion

        #region 序列化辅助方法

        // JSON序列化/反序列化的辅助方法

        #endregion
    }
}
```

## 使用示例

```csharp
// 保存基本类型
EasySave.SaveInt("score", 100);
EasySave.SaveString("playerName", "Player1");
EasySave.SaveBool("isFirstLogin", false);

// 获取基本类型
int score = EasySave.GetInt("score", 0); // 如果不存在，返回默认值0
string name = EasySave.GetString("playerName", "Unknown");
bool isFirst = EasySave.GetBool("isFirstLogin", true);

// 保存数组
int[] highScores = new int[] { 100, 90, 80, 70, 60 };
EasySave.SaveArray("highScores", highScores);

// 获取数组
int[] scores = EasySave.GetArray("highScores", new int[0]);

// 保存字典
Dictionary<string, int> playerScores = new Dictionary<string, int>
{
    { "Player1", 100 },
    { "Player2", 90 }
};
EasySave.SaveDictionary("playerScores", playerScores);

// 获取字典
Dictionary<string, int> scores = EasySave.GetDictionary("playerScores", new Dictionary<string, int>());

// 辅助方法
if (EasySave.HasKey("playerName"))
{
    EasySave.DeleteKey("playerName");
}

// 清除所有数据
EasySave.DeleteAll();