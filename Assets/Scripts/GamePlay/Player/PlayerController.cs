using Framework;
using GamePlay.Configs;
using GamePlay.Player;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// 主角总控制器
/// </summary>
public class PlayerController : MonoBehaviour
{
    private LocalInputProvider _inputProvider;
    private PlayerStats _playerStats;
    private PlayerLiquids _playerLiquids;
    private PlayerCombat _playerCombat;

    private Rigidbody2D _rb;
    private Animator _animator;
    private SpriteRenderer _sr;

    private void Awake()
    {
        _inputProvider = Global.Get<LocalInputManager>().Provider;

        //初始化主角组件
        _rb = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
        _sr = GetComponent<SpriteRenderer>();

        _playerCombat = GetComponent<PlayerCombat>();
        _playerLiquids = new(); 
        _playerStats = new();
        if (ConfigManager.Instance.TryGetConfig<PlayerConfig>("Player_001", out PlayerConfig config))
            _playerStats.Init(config);
            
    }
    private void Start()
    {
        GetComponent<PlayerCombat>().Init(_playerStats, _playerLiquids);
    }
    private void OnEnable()
    {
        _inputProvider.PlayerActions.PrimaryAttack.performed += OnPrimaryAttack;
        _inputProvider.PlayerActions.Skill.performed += OnSkill;
        _inputProvider.PlayerActions.Interact.performed += OnInteract;
        _inputProvider.PlayerActions.SwitchModule.performed += OnSwitchModule;
        _inputProvider.PlayerActions.SwitchLiquid.performed += OnSwitchLiquid;
        _inputProvider.PlayerActions.Sprint.performed += OnSprint;
        _inputProvider.PlayerActions.Pour.performed += OnPour;
    }
    private void OnDisable()
    {
        _inputProvider.PlayerActions.PrimaryAttack.performed -= OnPrimaryAttack;
        _inputProvider.PlayerActions.Skill.performed -= OnSkill;
        _inputProvider.PlayerActions.Interact.performed -= OnInteract;
        _inputProvider.PlayerActions.SwitchModule.performed -= OnSwitchModule;
        _inputProvider.PlayerActions.SwitchLiquid.performed -= OnSwitchLiquid;
        _inputProvider.PlayerActions.Sprint.performed -= OnSprint;
        _inputProvider.PlayerActions.Pour.performed -= OnPour;
    }

    private void FixedUpdate()
    {
        //移动逻辑
        Vector2 dir = _inputProvider.PlayerActions.Move.ReadValue<Vector2>();
        _rb.velocity = dir * _playerStats.MoveSpeed;
        //移动动画
        if( dir == Vector2.zero )
        {
            _animator.SetBool("Walk", false);
        }
        else
        {
            _animator.SetBool("Walk", true);
            if (dir.x >= 0)
                _sr.flipX = false;
            else
                _sr.flipX = true;
        }
    }

    #region 输入操作
    private void OnPrimaryAttack(InputAction.CallbackContext ctx)
    {
        //普通攻击逻辑
        _playerCombat.PrimaryAttack();
    }
    private void OnSkill(InputAction.CallbackContext ctx)
    {
        //技能逻辑
        _playerCombat.UseSkill();
    }
    private void OnInteract(InputAction.CallbackContext ctx)
    {
        //交互逻辑
        Debug.Log("交互逻辑");
    }

    private void OnSprint(InputAction.CallbackContext context)
    {
        //喷射动画

    }
    private void OnSwitchModule(InputAction.CallbackContext ctx)
    {
        _playerCombat.SwitchModule();
    }

    private void OnSwitchLiquid(InputAction.CallbackContext ctx)
    {
        //切换动画与音效
        if (_playerLiquids.ChangeLiquid())
        {

        }
        else
        {

        }
    }

    private void OnPour(InputAction.CallbackContext context)
    {
        
    }
    #endregion
}
