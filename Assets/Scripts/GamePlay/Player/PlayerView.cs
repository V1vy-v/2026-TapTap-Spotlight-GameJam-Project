using UnityEngine;

namespace GamePlay.Player
{
    /// <summary>玩家表现：唯一持有 Animator / SpriteRenderer 的组件，纯 API，不含规则</summary>
    public class PlayerView : MonoBehaviour
    {
        private SpriteRenderer _sprite;
        private Animator _animator;
        public AnimatorOverrideController AnimatorOverride;

        private void Awake()
        {
            _sprite = GetComponent<SpriteRenderer>();
            _animator = GetComponent<Animator>();
        }

        private static readonly int AnimWalk = Animator.StringToHash("Walk");
        private static readonly int AnimAtk = Animator.StringToHash("PrimaryAtk");
        private static readonly int AnimDeath = Animator.StringToHash("Death");

        public void PlayAttack(AttackModule module) 
        { 
            //区分远程和近战攻击

            _animator.SetTrigger(AnimAtk);  
            /* 音效一起 */ 
        }
        public void PlayDeath()
        { 
            _animator.SetTrigger(AnimDeath); 
        }
        public void SetMoving(bool v) 
        { 
            _animator.SetBool(AnimWalk, v);
        }
        public void SetFacing(float x) 
        { 
            if (Mathf.Abs(x) > 0.01f) 
                _sprite.flipX = x < 0f; 
        }
        public void ApplyForm(AnimatorOverrideController ov) 
        { 
            /* 换形态动画 */ 
        }
    }
}