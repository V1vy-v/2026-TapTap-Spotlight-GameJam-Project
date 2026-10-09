using Framework;
using UnityEngine;

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

    private void Update()
    {
        if(inputProvider == null && Global.Get<LocalInputManager>().Provider != null)
            inputProvider = Global.Get<LocalInputManager>().Provider;
    }
    private void FixedUpdate()
    {
        if (inputProvider == null) return;
        rb.velocity = inputProvider.GetInputState().MoveInput * speed;
    }
}
