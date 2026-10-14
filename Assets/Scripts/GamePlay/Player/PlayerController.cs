using Framework;
using GamePlay.Configs;
using GamePlay.Player;
using System;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// 主角总控制器
/// </summary>
public class PlayerController : MonoBehaviour
{
    private LocalInputProvider _inputProvider;
    private Rigidbody2D _rb;
    private PlayerStats _playerStats;
    private PlayerLiquids _playerLiquids;
    private PlayerCombat _playerCombat;

    private void Awake()
    {
        _inputProvider = Global.Get<LocalInputManager>().Provider;

        //初始化主角组件
        _rb = GetComponent<Rigidbody2D>();
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
        _inputProvider.PlayerActions.Spray.performed += OnSpray;
        _inputProvider.PlayerActions.Pour.performed += OnPour;
    }
    private void OnDisable()
    {
        _inputProvider.PlayerActions.PrimaryAttack.performed -= OnPrimaryAttack;
        _inputProvider.PlayerActions.Skill.performed -= OnSkill;
        _inputProvider.PlayerActions.Interact.performed -= OnInteract;
        _inputProvider.PlayerActions.SwitchModule.performed -= OnSwitchModule;
        _inputProvider.PlayerActions.SwitchLiquid.performed -= OnSwitchLiquid;
        _inputProvider.PlayerActions.Spray.performed -= OnSpray;
        _inputProvider.PlayerActions.Pour.performed -= OnPour;
    }

    private void FixedUpdate()
    {
        //移动逻辑
        _rb.velocity = _inputProvider.PlayerActions.Move.ReadValue<Vector2>() * _playerStats.MoveSpeed;
    }

    #region 输入操作
    private void OnPrimaryAttack(InputAction.CallbackContext ctx)
    {
        //普通攻击逻辑
        _playerCombat.RangeAttack(Vector2.zero, 2f);
    }
    private void OnSkill(InputAction.CallbackContext ctx)
    {
        //技能逻辑
        Debug.Log("技能逻辑");
    }
    private void OnInteract(InputAction.CallbackContext ctx)
    {
        //交互逻辑
        Debug.Log("交互逻辑");
    }
    private void OnSwitchModule(InputAction.CallbackContext ctx)
    {

    }

    private void OnSpray(InputAction.CallbackContext context)
    {

    }

    private void OnSwitchLiquid(InputAction.CallbackContext ctx)
    {

    }

    private void OnPour(InputAction.CallbackContext context)
    {
        
    }
    #endregion
}
