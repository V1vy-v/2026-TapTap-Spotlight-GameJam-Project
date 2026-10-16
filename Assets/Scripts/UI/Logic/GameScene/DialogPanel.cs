using Framework;
using UnityEngine;

namespace UI
{
    public partial class DialogPanel
    {
        public static System.Action OnDialogueCompleted;   

        private DiaLogController _dialogController;

        protected override void Init()
        {
            _dialogController = GetComponent<DiaLogController>();
            _dialogController.SetupUI(_roleA, _roleB, _nameText, _dialogText, null);
            _dialogController.OnDialogueEnd += () => OnDialogueCompleted?.Invoke();
            _bK.onClick.AddListener(OnBK);
        }

        protected override void AddListener() { }

        protected override void RemoveListener()
        {
            _bK.onClick.RemoveListener(OnBK);
            base.RemoveListener();
        }

        protected override void UpdateView() { }

        protected override void OnShow()
        {
            if (Properties is DialogPanelProperties props &&
                !string.IsNullOrEmpty(props.DialogueId))
            {
                _dialogController?.SetDialogFile(props.DialogueId);
            }
        }

        private void OnBK() => _dialogController?.OnClickNext();
    }
}