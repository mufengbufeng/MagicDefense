# 项目知识图谱

## 工具组件

### EasySave

**路径**: `Assets/GameScripts/HotFix/GameLogic/Common/EasySave/EasySave.cs`

**功能概述**: 
基于Unity PlayerPrefs的本地存储工具，支持存储整数、浮点数、字符串、布尔值、数组和字典等数据类型。

**主要功能**:
- 基本数据类型存取: SaveInt/GetInt, SaveFloat/GetFloat, SaveString/GetString, SaveBool/GetBool
- 复杂数据类型存取: SaveArray/GetArray, SaveDictionary/GetDictionary
- 辅助功能: HasKey, DeleteKey, DeleteAll

**实现特点**:
- 使用完整的错误处理机制，防止运行时异常导致游戏崩溃
- 所有Get方法都支持默认值参数，当键不存在时返回默认值
- 使用JsonUtility进行序列化，保证与Unity环境的兼容性
- 借助包装类来解决JsonUtility无法直接序列化数组和字典的问题

**使用示例**:
```csharp
// 保存基本类型
EasySave.SaveInt("分数", 100);
EasySave.SaveString("玩家名", "玩家1");
EasySave.SaveBool("是否首次登录", false);

// 获取基本类型
int score = EasySave.GetInt("分数", 0); // 如果不存在，返回默认值0
string name = EasySave.GetString("玩家名", "未知");
bool isFirst = EasySave.GetBool("是否首次登录", true);

// 保存数组
int[] highScores = new int[] { 100, 90, 80, 70, 60 };
EasySave.SaveArray("高分列表", highScores);

// 获取数组
int[] scores = EasySave.GetArray("高分列表", new int[0]);

// 保存字典
Dictionary<string, int> playerScores = new Dictionary<string, int>
{
    { "玩家1", 100 },
    { "玩家2", 90 }
};
EasySave.SaveDictionary("玩家分数", playerScores);

// 获取字典
Dictionary<string, int> scores = EasySave.GetDictionary("玩家分数", new Dictionary<string, int>());
```

**相关设计文档**: `EasySave设计方案.md`

**创建时间**: 2025/3/26

**维护者**: Team

## 关联关系

### EasySave与PlayerPrefs
- EasySave基于Unity的PlayerPrefs API构建
- 为PlayerPrefs添加了错误处理和默认值支持
- 扩展了PlayerPrefs的功能，支持复杂数据类型

### EasySave与JsonUtility
- 使用Unity的JsonUtility进行序列化和反序列化
- 创建包装类解决JsonUtility不支持直接序列化数组和字典的限制

## 技术债务与优化方向

### 潜在改进
- 考虑添加数据加密功能，保护敏感数据
- 可以添加数据版本控制，支持数据迁移
- 可以优化序列化性能，特别是对于大型复杂数据

## 游戏存储系统

### 存储需求
- **游戏进度保存**：保存当前游戏状态、得分和游戏板状态
- **最高分记录**：存储历史最高分和记录列表
- **游戏设置**：保存玩家的个性化设置和选项
- **成就系统**：记录玩家达成的各种成就和里程碑

### 实现策略

#### 数据持久化方式
- **轻量级数据**：使用EasySave（PlayerPrefs）存储基本游戏状态和设置
- **数据存储位置**：在不同平台上，PlayerPrefs存储位置有所不同
  - Windows: `HKCU\Software\[公司名称]\[产品名称]`注册表项
  - macOS: `~/Library/Preferences/[bundle identifier].plist`
  - Linux: `~/.config/unity3d/[公司名称]/[产品名称]`
  - WebGL: 使用浏览器的IndexedDB
  - iOS/Android: 使用设备的应用程序偏好设置存储

#### 2048游戏板存储结构
```csharp
// 游戏核心数据结构
[Serializable]
public class GameBoardState
{
    public int[,] board;  // 游戏板二维数组
    public int score;     // 当前分数
    public int bestScore; // 历史最高分
    public bool gameOver; // 游戏是否结束
}

// 使用EasySave保存游戏状态
public void SaveGameState(GameBoardState state)
{
    string json = JsonUtility.ToJson(state);
    EasySave.SaveString("GameState", json);
    EasySave.SaveInt("BestScore", state.bestScore);
}

// 读取游戏状态
public GameBoardState LoadGameState()
{
    if (EasySave.HasKey("GameState"))
    {
        string json = EasySave.GetString("GameState");
        return JsonUtility.FromJson<GameBoardState>(json);
    }
    return InitializeNewGame(); // 返回新游戏状态
}
```

### 最佳实践
- **定期保存**：在关键操作后即时保存（如移动结束、游戏结束）
- **安全性考虑**：避免将敏感或作弊相关的数据直接保存
- **错误处理**：EasySave已内置错误处理，确保数据加载失败不会导致游戏崩溃
- **默认值**：始终为所有获取操作提供合理的默认值
- **版本兼容**：考虑未来数据结构变更的兼容性问题

### 扩展应用
- **排行榜系统**：使用有序数组存储并展示最高分记录
- **游戏设置存储**：音量、主题、游戏难度等玩家偏好设置
- **教程进度**：记录玩家已完成的教程步骤