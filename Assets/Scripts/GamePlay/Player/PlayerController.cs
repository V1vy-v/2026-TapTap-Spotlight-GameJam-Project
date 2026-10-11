using Framework;
using GamePlay.Player;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// 主角总控制器
/// </summary>
public class PlayerController : MonoBehaviour
{
    private LocalInputProvider inputProvider;
    private Rigidbody2D _rb;
    private PlayerLiquids _playerLiquids;
    private PlayerCombat _playerCombat;
    private float speed = 10f;

    private void Start()
    {
        //待修改
        StartCoroutine(GetProvider());

        //初始化主角组件
        _rb = GetComponent<Rigidbody2D>();
        _playerLiquids = new();
        _playerCombat = new();
    }
    IEnumerator GetProvider()
    {
        yield return new WaitForSeconds(3);
        inputProvider = Global.Get<LocalInputManager>().Provider;
    }

    private void OnEnable()
    {
        inputProvider.PlayerActions.PrimaryAttack.performed += OnPrimaryAttack;
        inputProvider.PlayerActions.Interact.performed += OnInteract;
        inputProvider.PlayerActions.Skill.performed += OnSkill;
        inputProvider.PlayerActions.Consumables.performed += OnConsumable;
        inputProvider.PlayerActions.Jump.performed += OnJump;
        inputProvider.PlayerActions.Pause.performed += OnPause;
    }
    private void OnDisable()
    {
        inputProvider.PlayerActions.PrimaryAttack.performed -= OnPrimaryAttack;
        inputProvider.PlayerActions.Interact.performed -= OnInteract;
        inputProvider.PlayerActions.Skill.performed -= OnSkill;
        inputProvider.PlayerActions.Consumables.performed -= OnConsumable;
        inputProvider.PlayerActions.Jump.performed -= OnJump;
        inputProvider.PlayerActions.Pause.performed -= OnPause;
    }
    private void FixedUpdate()
    {
        //移动逻辑
        _rb.velocity = inputProvider.PlayerActions.Move.ReadValue<Vector2>() * speed;
    }

    private void OnPrimaryAttack(InputAction.CallbackContext ctx)
    {
        //普通攻击逻辑
        Debug.Log("普通攻击"); 
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
    private void OnConsumable(InputAction.CallbackContext ctx)
    {
        //使用消耗品逻辑
        Debug.Log("使用消耗品逻辑");
    }
    private void OnJump(InputAction.CallbackContext ctx)
    {
        //冲刺逻辑
        Debug.Log("冲刺逻辑");
    }
    private void OnPause(InputAction.CallbackContext ctx)
    {
        //暂停逻辑
        Debug.Log("暂停逻辑");
    }
}
