using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;

namespace GameLogic
{
    public class Bullet : MonoBehaviour
    {
        private Transform _target;
        private float _speed = 10f;
        private int _damage = 10;
        private CancellationTokenSource _cts;

        public void Init(Transform target, int damage)
        {
            _target = target;
            _damage = damage;
            _cts = new CancellationTokenSource();
            MoveBulletAsync().Forget();
        }

        private async UniTaskVoid MoveBulletAsync()
        {
            while (!_cts.IsCancellationRequested)
            {
                if (_target == null || !_target.gameObject.activeSelf)
                {
                    PoolManager.Instance.PushGameObject(gameObject);
                    return;
                }

                Vector3 direction = (_target.position - transform.position).normalized;
                transform.position += direction * _speed * Time.deltaTime;
                await UniTask.Yield(_cts.Token);
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.transform == _target)
            {
                _cts?.Cancel();
                var enemy = other.GetComponent<EnemyEntity>();
                if (enemy != null)
                {
                    enemy.TakeDamage(_damage);
                }
                PoolManager.Instance.PushGameObject(gameObject);
            }
        }

        private void OnDestroy()
        {
            _cts?.Cancel();
            _cts?.Dispose();
        }
    }
}
