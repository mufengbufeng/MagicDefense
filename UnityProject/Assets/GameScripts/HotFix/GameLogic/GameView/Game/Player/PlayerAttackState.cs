using TEngine;
using UnityEngine;
using GameLogic;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;

namespace GameLogic
{
    /// <summary>
    /// 玩家攻击状态。
    /// </summary>
    public class PlayerAttackState : FsmState<PlayerEntity>
    {
        private PlayerEntity _player;
        private EnemyEntity _targetEnemy;

        protected override void OnInit(IFsm<PlayerEntity> fsm)
        {
            base.OnInit(fsm);
            _player = fsm.Owner;
        }

        protected override void OnEnter(IFsm<PlayerEntity> fsm)
        {
            base.OnEnter(fsm);
            _targetEnemy = fsm.GetData<EnemyEntity>("TargetEnemy");

            // Log.Info("进入攻击状态");
            if (_targetEnemy == null || !_targetEnemy.gameObject.activeInHierarchy || _targetEnemy.Health <= 0)
            {
                Log.Warning($"{_player.gameObject.name} 进入攻击状态但目标无效，切换回待机。");
                ChangeState<PlayerIdleState>(fsm);
                return;
            }

            TryExecuteHighestPriorityReadySkill(fsm).Forget();
        }

        protected override void OnUpdate(IFsm<PlayerEntity> fsm, float elapseSeconds, float realElapseSeconds)
        {
            base.OnUpdate(fsm, elapseSeconds, realElapseSeconds);


        }

        protected override void OnLeave(IFsm<PlayerEntity> fsm, bool isShutdown)
        {
            base.OnLeave(fsm, isShutdown);
            fsm.RemoveData("TargetEnemy");
        }

        private async UniTaskVoid TryExecuteHighestPriorityReadySkill(IFsm<PlayerEntity> fsm)
        {
            if (_targetEnemy == null || _player == null)
            {
                ChangeState<PlayerIdleState>(fsm);
                Log.Warning($"{_player.gameObject.name} 进入攻击状态但目标无效，切换回待机。");
                return;
            }
            while (true)
            {
                List<SkillData> readySkills = await _player.GetReadySkills();
                // Log.Info($"可用技能数量: {readySkills.Count}");

                if (readySkills.Count == 0)
                {
                    Log.Warning($"{_player.gameObject.name} 没有可用的技能，切换回待机状态。");
                    ChangeState<PlayerIdleState>(fsm);
                    return;
                }

                // 按优先级排序(数值越小优先级越高)
                readySkills.Sort((a, b) => a.Priority.CompareTo(b.Priority));
                var skillData = readySkills.FirstOrDefault(s => _player.CheckSkillRange(s, _player.transform.position, _targetEnemy.transform.position));

                if (skillData == null)
                {
                    Log.Warning($"{_player.gameObject.name} 没有可用的技能，切换回待机状态。");
                    ChangeState<PlayerIdleState>(fsm);
                    return;
                }
                _player.StartSkillCooldown(skillData);
                ExecuteSkillEffect(skillData, _targetEnemy).Forget();

            }
        }

        private async UniTask ExecuteSkillEffect(SkillData skillData, EnemyEntity target)
        {
            // Log.Info($"ttarget is null {target == null} || target.Health <= 0 {target.Health <= 0}");
            if (target == null || target.Health <= 0)
                return;
            // Log.Info($"目标: {target.gameObject.name}，生命值: {target.Health}");
            // Log.Info($"{_player.gameObject.name} 使用技能 {skillData.SkillName}，目标: {target.gameObject.name}");
            if (skillData.Prefab != null)
            {
                await SkillManager.Instance.LoadSkill(skillData, target, target.transform, _player.transform);
            }
            else
            {
                // 没有预制体的直接伤害技能
                target.TakeDamage((int)skillData.EffectValue);
            }
        }
    }
}
