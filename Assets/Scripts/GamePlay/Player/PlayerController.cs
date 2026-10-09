using Framework;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private Rigidbody2D rb;
    private LocalInputProvider inputProvider;
    private float speed = 10f;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        //inputProvider = Global.Get<LocalInputManager>().Provider;
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
    private void Update()
    {
        //测试用，待删除
        if(inputProvider == null && Global.Get<LocalInputManager>().Provider != null)
            inputProvider = Global.Get<LocalInputManager>().Provider;
    }
    private void FixedUpdate()
    {
        //测试用，待删除
        if (inputProvider == null) return;

        //移动逻辑
        rb.velocity = inputProvider.PlayerActions.Move.ReadValue<Vector2>() * speed;
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
