using System.Collections.Generic;
using Framework.UI.Core.Bind;
using UnityEngine;

namespace Framework.UI.Core
{
    [RequireComponent(typeof(CanvasGroup))]
    public abstract class View : MonoBehaviour
    {
        private List<View> _subViews;
        private CanvasGroup _canvasGroup;
        public ViewModel ViewModel { get; private set; }
        public Transform Transform { get; private set; }
        protected UIBindFactory Binding = new UIBindFactory();
        public abstract UILevel UILevel { get; }

        public void SetVm(ViewModel vm)
        {
            if (ViewModel == vm) return;
            ViewModel = vm;
            Binding.Reset();
            if (ViewModel != null)
            {
                OnVmChange();
            }
        }

        protected virtual void Awake()
        {
            _canvasGroup = GetComponent<CanvasGroup>();
            _subViews = new List<View>();
            Transform = transform;
        }

        public IAnimation EnterAnimation;

        public IAnimation ExitAnimation;

        public bool Visible => _canvasGroup.alpha == 1;

        #region 界面显示隐藏的调用和回调方法

        public void Show(bool ignoreAnimation = false)
        {
            SetCanvas(true);
            if (!ignoreAnimation)
                EnterAnimation?.Play();
            OnShow();
            _subViews.ForEach((subView) => subView.OnShow());
        }

        public void Hide(bool ignoreAnimation = false)
        {
            SetCanvas(false);
            if (!ignoreAnimation)
                ExitAnimation?.Play();
            OnHide();
            _subViews.ForEach((subView) => subView.OnHide());
        }

        protected virtual void OnShow()
        {
        }

        protected virtual void OnHide()
        {
        }

        public void Destroy()
        {
            Destroy(gameObject);
        }

        private void SetCanvas(bool visible)
        {
            _canvasGroup.interactable = visible;
            _canvasGroup.alpha = visible ? 1 : 0;
            _canvasGroup.blocksRaycasts = visible;
        }

        #endregion

        public void AddSubView(View view)
        {
            if (_subViews.Contains(view)) return;
            _subViews.Add(view);
        }

        protected abstract void OnVmChange();
    }
}