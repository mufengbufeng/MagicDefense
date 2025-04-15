using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;
using TEngine;

namespace GameLogic
{
    public class SkillEntity : MonoBehaviour
    {
        protected SkillData SkillData;
        protected ISkillTargetEntity Target;
        public GameObject TargetGo;
        private CancellationTokenSource _cts;
        private AABBCollider _collider;


        public virtual void Initialize(SkillData skillData, ISkillTargetEntity target, GameObject go)
        {
            SkillData = skillData;
            Target = target;
            _cts = new CancellationTokenSource();
            TargetGo = go;
            _collider = GetComponent<AABBCollider>();
            _collider.OnCollisionEnter += OnAABBTriggerEnter;
            if (skillData.Targeting == TargetingType.LockOn)
            {
                MoveToTargetAsync().Forget();
            }
            else
            {
                ApplyEffectImmediately();
            }
        }

        private async UniTaskVoid MoveToTargetAsync()
        {
            while (!_cts.IsCancellationRequested)
            {
                if (TargetGo == null || !TargetGo.activeSelf)
                {
                    ReleaseSelf();
                    return;
                }

                Vector3 direction = (TargetGo.transform.position - transform.position).normalized;
                transform.position += direction * SkillData.EffectValue * Time.deltaTime;
                await UniTask.Yield(_cts.Token);
            }
        }

        private void ApplyEffectImmediately()
        {
            if (Target != null)
            {
                Target.TakeDamage((int)SkillData.EffectValue);
            }
            // PoolManager.Instance.ReleaseGameObject(gameObject);
            ReleaseSelf();
        }

        private void OnAABBTriggerEnter(AABBCollider other)
        {
            if (other == null || TargetGo == null)
            {
                // Log.Error($"OnTriggerEnter: other or TargetGo is null.other:{other}, TargetGo: {TargetGo}");
                ReleaseSelf();
                return;
            }

            if (Target != null && other.transform == TargetGo.transform)
            {
                _cts?.Cancel();
                Target.TakeDamage((int)SkillData.EffectValue);
                ReleaseSelf();
            }
        }

        public void ReleaseSelf()
        {
            if (_collider != null)
            {
                _collider.OnCollisionEnter -= OnAABBTriggerEnter;
            }
            PoolManager.Instance.ReleaseGameObject(gameObject);
        }
        private void OnDestroy()
        {

            _cts?.Cancel();
            _cts?.Dispose();
        }

    }


}
