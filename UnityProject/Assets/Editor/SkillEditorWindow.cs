using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;

public class SkillEditorWindow : EditorWindow
{
    private List<SkillData> _skillList = new List<SkillData>();
    private SkillData _selectedSkill;
    private Vector2 _listScrollPos;
    private Vector2 _detailScrollPos;
    private string _skillDataPath = "Assets/AssetRaw/Configs/SkillConfig";

    [MenuItem("Tools/游戏配置/技能编辑器")]
    public static void ShowWindow()
    {
        GetWindow<SkillEditorWindow>("技能配置编辑器");
    }

    private void OnEnable()
    {
        LoadSkillData();
    }

    void OnGUI()
    {
        EditorGUILayout.BeginHorizontal();

        DrawSkillListPanel();
        DrawSkillDetailPanel();

        EditorGUILayout.EndHorizontal();
    }

    void LoadSkillData()
    {
        _skillList.Clear();
        if (!Directory.Exists(_skillDataPath))
        {
            Directory.CreateDirectory(_skillDataPath);
        }

        string[] guids = AssetDatabase.FindAssets("t:SkillData", new[] { _skillDataPath });
        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            SkillData skill = AssetDatabase.LoadAssetAtPath<SkillData>(path);
            if (skill != null)
                _skillList.Add(skill);
        }
        _skillList.Sort((a, b) => a.SkillID.CompareTo(b.SkillID));
    }

    void DrawSkillListPanel()
    {
        EditorGUILayout.BeginVertical(EditorStyles.helpBox, GUILayout.Width(200), GUILayout.ExpandHeight(true));
        EditorGUILayout.LabelField("技能配置列表", EditorStyles.boldLabel);

        if (GUILayout.Button("新建技能"))
            CreateNewSkill();
        if (GUILayout.Button("刷新列表"))
            LoadSkillData();

        _listScrollPos = EditorGUILayout.BeginScrollView(_listScrollPos);
        foreach (SkillData skill in _skillList)
        {
            if (GUILayout.Button($"{skill.SkillID}: {skill.SkillName ?? "未命名"}"))
            {
                _selectedSkill = skill;
                GUI.FocusControl(null);
            }
        }
        EditorGUILayout.EndScrollView();
        EditorGUILayout.EndVertical();
    }

    void DrawSkillDetailPanel()
    {
        EditorGUILayout.BeginVertical(EditorStyles.helpBox, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));

        if (_selectedSkill != null)
        {
            EditorGUILayout.LabelField($"正在编辑: ID {_selectedSkill.SkillID} - {_selectedSkill.SkillName}", EditorStyles.boldLabel);
            _detailScrollPos = EditorGUILayout.BeginScrollView(_detailScrollPos);

            SerializedObject serializedSkill = new SerializedObject(_selectedSkill);
            serializedSkill.Update();

            SerializedProperty prop = serializedSkill.GetIterator();
            bool enterChildren = true;
            while (prop.NextVisible(enterChildren))
            {
                if (prop.name == "m_Script")
                    continue;

                // 锁定目标相关字段的条件显示
                if (prop.name == "LockCondition" || prop.name == "TargetSortOrder" || prop.name == "MaxLockTargets")
                {
                    var targetingProp = serializedSkill.FindProperty("Targeting");
                    if (targetingProp != null && targetingProp.enumValueIndex == (int)TargetingType.LockOn)
                    {
                        EditorGUILayout.PropertyField(prop, true);
                    }
                }
                else
                {
                    EditorGUILayout.PropertyField(prop, true);
                }

                enterChildren = false;
            }

            if (serializedSkill.ApplyModifiedProperties())
            {
                EditorUtility.SetDirty(_selectedSkill);
            }

            EditorGUILayout.Space();
            if (GUILayout.Button("保存配置"))
                AssetDatabase.SaveAssets();
            if (GUILayout.Button("删除配置", GUILayout.Width(100)))
            {
                if (EditorUtility.DisplayDialog("确认删除", $"确定要删除技能配置 {_selectedSkill.SkillID}: {_selectedSkill.SkillName} 吗？", "删除", "取消"))
                {
                    DeleteSkill(_selectedSkill);
                }
            }

            EditorGUILayout.EndScrollView();
        }
        else
        {
            EditorGUILayout.LabelField("请在左侧选择一个技能进行编辑");
        }

        EditorGUILayout.EndVertical();
    }

    void CreateNewSkill()
    {
        SkillData newSkill = CreateInstance<SkillData>();
        newSkill.SkillID = FindNextAvailableID();
        newSkill.SkillName = "未命名技能_" + newSkill.SkillID;

        string path = Path.Combine(_skillDataPath, $"Skill_{newSkill.SkillID}.asset");
        path = AssetDatabase.GenerateUniqueAssetPath(path);

        AssetDatabase.CreateAsset(newSkill, path);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        LoadSkillData();
        _selectedSkill = newSkill;
        EditorGUIUtility.PingObject(newSkill);
    }

    void DeleteSkill(SkillData skillToDelete)
    {
        string path = AssetDatabase.GetAssetPath(skillToDelete);
        AssetDatabase.DeleteAsset(path);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        _selectedSkill = null;
        LoadSkillData();
    }

    int FindNextAvailableID()
    {
        int maxID = 0;
        foreach (var skill in _skillList)
        {
            if (skill.SkillID > maxID)
                maxID = skill.SkillID;
        }
        return maxID + 1;
    }
}
