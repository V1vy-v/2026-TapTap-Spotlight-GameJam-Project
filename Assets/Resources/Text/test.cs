using Framework;
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
        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            var root = Global.Get<GameplayRoot>();
            root.AdvanceStage();
            //var props = new DialogPanelProperties
            //{
            //    DialogueId = "111",
            //};

            //Global.ShowUI("DialogPanel", props);
        }
        if (Keyboard.current.bKey.wasPressedThisFrame)
        {
            var root = Global.Get<GameplayRoot>();
            root.ChangeMode(root.ResolveNextMode());
            //var props = new DialogPanelProperties
            //{
            //    DialogueId = "111",
            //};

            //Global.ShowUI("DialogPanel", props);
        }
    }
}
