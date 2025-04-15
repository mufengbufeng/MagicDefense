using System.Collections.Generic;
using UnityEngine;
using TEngine;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using GameBase;

namespace GameLogic
{
    public class SkillManager : SingletonBehaviour<SkillManager>
    {


        public async UniTask<SkillData> GetSkillData(int skillID)
        {
            var skill = await GameModule.Resource.LoadAssetAsync<SkillData>($"Skill_{skillID}");
            return skill;
        }

        public async UniTask LoadSkill(SkillData skillData, EnemyEntity target, Transform targetTrans, Transform selfTrans)
        {
            // var effectObj = await PoolManager.Instance.GetGameObjectAsync(
            //     skillData.Prefab.name,
            //     GameSceneManager.Instance.unit);
            var effectObj = await GameSceneManager.Instance.LoadObj(skillData.Prefab.name);

            effectObj.transform.position = selfTrans.position;


            Log.Info($"{selfTrans.name} 使用技能 {skillData.SkillName}，特效位置: {effectObj.transform.position}");

            var skillEntity = effectObj.GetOrAddComponent<SkillEntity>();
            effectObj.GetOrAddComponent<AABBCollider>();
            skillEntity.Initialize(skillData, target, target.gameObject);
        }


        private void OnDestroy()
        {
            Log.Info("SkillManager: 模块已销毁。");
        }
    }
}
