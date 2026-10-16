using System;
using UnityEngine;

namespace GamePlay.Player
{
    /// <summary>主角交互检测：找出身边最优的可交互物，供 F 键调用</summary>
    public class PlayerInteractor : MonoBehaviour
    {
        [SerializeField] private float _radius = 1.5f;
        [SerializeField] private LayerMask _interactMask;
        [SerializeField] private float _scanInterval = 0.08f;
        private PlayerLiquids _liquids;

        private readonly Collider2D[] _buffer = new Collider2D[16];

        private IInteractable _current;
        private float _elapsed;

        /// <summary>候选变化时触发，null 表示当前没有可交互目标</summary>
        public event Action<IInteractable> OnCandidateChanged;

        public IInteractable Current => _current;

        #region 生命周期
        public void Init(PlayerLiquids liquids)
        {
            _liquids = liquids;
        }
        private void Update()
        {
            _elapsed += Time.deltaTime;
            if (_elapsed < _scanInterval) return;
            _elapsed = 0f;

            Scan();
        }

        private void OnDisable()
        {
            SetCandidate(null);
        }
        #endregion

        #region 检测
        private void Scan()
        {
            int n = Physics2D.OverlapCircleNonAlloc(transform.position, _radius, _buffer, _interactMask);

            IInteractable best = null;
            int bestPriority = int.MinValue;
            float bestSqr = float.MaxValue;
            Vector2 self = transform.position;

            for (int i = 0; i < n; i++)
            {
                // 脚本可能和碰撞体不在同一物体上，所以向父级找
                var it = _buffer[i].GetComponentInParent<IInteractable>();
                if (it == null || !it.CanInteract) continue;
                if (it.PromptAnchor == null) continue;

                float sqr = ((Vector2)it.PromptAnchor.position - self).sqrMagnitude;
                if (it.Priority > bestPriority || (it.Priority == bestPriority && sqr < bestSqr))
                {
                    best = it; bestPriority = it.Priority; bestSqr = sqr;
                }
            }

            SetCandidate(best);
        }

        private void SetCandidate(IInteractable it)
        {
            if (ReferenceEquals(_current, it)) return;

            _current = it;
            OnCandidateChanged?.Invoke(_current);
        }
        #endregion

        #region 对外
        /// <summary>由 PlayerController 在 F 键按下时调用</summary>
        public bool TryInteract()
        {
            // 接口引用不触发 Unity 的 == 重载，销毁后要显式判一次
            if (_current is not UnityEngine.Object obj || obj == null) return false;
            if (!_current.CanInteract) return false;

            _current.Interact(gameObject, _liquids);

            Scan();     // 交互后立刻重扫：状态可能已变（如吸取后备用槽满）
            return true;
        }
        #endregion
    }
}