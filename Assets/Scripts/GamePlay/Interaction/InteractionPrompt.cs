using GamePlay.Player;
using TMPro;
using UnityEngine;

namespace GamePlay.Interaction
{
    /// <summary>
    /// 世界空间交互提示：跟着当前候选交互物的 PromptAnchor 显示按键文案。
    /// 挂在主角预制体的提示节点上。
    /// </summary>
    public class InteractionPrompt : MonoBehaviour
    {
        [SerializeField] private PlayerInteractor _interactor;
        [SerializeField] private GameObject _root;      // 真正显示/隐藏的节点
        [SerializeField] private TMP_Text _label;
        [SerializeField] private Vector3 _offset = new Vector3(0f, 1.2f, 0f);

        private void OnEnable()
        {
            if (_interactor == null) return;
            _interactor.OnPromptChanged += OnPromptChanged;
            OnPromptChanged(_interactor.CurrentPrompt);
        }

        private void OnDisable()
        {
            if (_interactor != null) _interactor.OnPromptChanged -= OnPromptChanged;
        }

        private void LateUpdate()
        {
            if (_root == null || !_root.activeSelf) return;

            var anchor = _interactor != null ? _interactor.Current?.PromptAnchor : null;
            if (anchor != null) _root.transform.position = anchor.position + _offset;
        }

        private void OnPromptChanged(string text)
        {
            bool show = !string.IsNullOrEmpty(text);
            if (_root != null) _root.SetActive(show);
            if (show && _label != null) _label.text = text;
        }
    }
}