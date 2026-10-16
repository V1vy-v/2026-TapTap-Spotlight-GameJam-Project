using Framework;
using GamePlay;
using GamePlay.Session;
using System;
using System.Diagnostics;
using UI;

public sealed class GameplayDialogueState : GameplayModeStateBase
{
    public override GameplayMode StateKey => GameplayMode.Dialogue;
    protected override LocalInputType InputMap => LocalInputType.UI;

    //完成后自动推进
    protected override bool AutoAdvanceOnFinish => true;

    public GameplayDialogueState(GameplayRoot root, StateMachine<GameplayMode> sm)
        : base(root, sm) { }

    public override void Enter()
    {
        base.Enter();

        DialogPanel.OnDialogueCompleted += OnCompleted;

        string id = Root.Dialog.CurrentDialogueId;
        Global.ShowUI("DialogPanel", new DialogPanelProperties { DialogueId = id });
    }

    public override void Exit()
    {
        base.Exit();
        DialogPanel.OnDialogueCompleted -= OnCompleted;
        Global.HideUI("DialogPanel");
    }

    private void OnCompleted()
    {
        MarkFinished();
    }
}