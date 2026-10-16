using Framework;
using GamePlay.Procedure;
using UnityEngine;
using UI;
using UnityEngine.InputSystem;
using GamePlay;

public class test : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Keyboard.current.bKey.wasPressedThisFrame)
        {
            var root = Global.Get<GameplayRoot>();
            root.AdvanceNode(); ;
        }
    }
}
