using System.Collections.Generic;
using UnityEngine;
using TEngine;

namespace GameLogic
{
    public class SkillManager : Module
    {
        private Dictionary<int, SkillData> _skillDatabase = new Dictionary<int, SkillData>();
        private string _skillDataLoadPath = "Assets/GameScripts/HotFix/GameLogic/DataConfigs/Skills";

        protected override void Awake()
        {
            base.Awake();
            LoadAllSkills();
        }

        void LoadAllSkills()
        {
            _skillDatabase.Clear();

#if UNITY_EDITOR
            string[] guids = UnityEditor.AssetDatabase.FindAssets("t:SkillData", new[] { _skillDataLoadPath });
            foreach (string guid in guids)
            {
                string path = UnityEditor.AssetDatabase.GUIDToAssetPath(guid);
                SkillData skill = UnityEditor.AssetDatabase.LoadAssetAtPath<SkillData>(path);
                if (skill != null && !_skillDatabase.ContainsKey(skill.SkillID))
                {
                    _skillDatabase.Add(skill.SkillID, skill);
                }
                else if (skill != null)
                {
                    Log.Error($"SkillManager: 重复的技能 ID {skill.SkillID} 在路径 {path}");
                }
            }
            Log.Info($"SkillManager: 加载了 {_skillDatabase.Count} 个技能数据。");
#else
            // TODO: 实现运行时加载逻辑 (Resources, Addressables 等)
            Log.Error("SkillManager: 运行时加载逻辑未实现!");
#endif
        }

        public SkillData GetSkillData(int skillID)
        {
            _skillDatabase.TryGetValue(skillID, out SkillData data);
            if (data == null)
            {
                Log.Warning($"SkillManager: 未找到 ID 为 {skillID} 的技能数据。");
            }
            return data;
        }

        // protected override void Update(float elapseSeconds, float realElapseSeconds) { }
        public void Shutdown() { _skillDatabase.Clear(); }

        private void OnDestroy()
        {
            Shutdown();
            Log.Info("SkillManager: 模块已销毁。");
        }
    }
}
