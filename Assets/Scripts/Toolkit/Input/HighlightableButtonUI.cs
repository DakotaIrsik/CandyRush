using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using VContainer;
using OctoberStudio.Input;

namespace OctoberStudio.UI
{
    [RequireComponent(typeof(Button))]
    public class HighlightableButtonUI : MonoBehaviour, ISelectHandler, IDeselectHandler
    {
        public bool IsHighlighted { get; set; }

        protected Button button;

        private Vector3 savedPosition = Vector3.zero;

        // Injected dependencies
        private IInputManager inputManager;

        [Inject]
        public void Construct(IInputManager inputManager)
        {
            this.inputManager = inputManager;
        }

        protected virtual void Awake()
        {
            button = GetComponent<Button>();
        }

        private void Update()
        {
            if (IsHighlighted)
            {
                if(transform.position != savedPosition)
                {
                    savedPosition = transform.position;

                    inputManager.Highlights?.RefreshHighlight();
                }
            }
        }

        public virtual void Highlight()
        {
            if (button.enabled && inputManager?.Highlights != null)
            {
                inputManager.Highlights.Highlight(this);
            }

            savedPosition = transform.position;
        }

        public virtual void StopHighlighting()
        {
            if (IsHighlighted && inputManager?.Highlights != null)
            {
                inputManager.Highlights.StopHighlighting(this);
            }

            IsHighlighted = false;
            transform.position = savedPosition;
        }

        private void OnDisable()
        {
            if (IsHighlighted && inputManager?.Highlights != null)
            {
                inputManager.Highlights.StopHighlighting(this);
            }
        }

        public virtual void OnSelect(BaseEventData eventData)
        {
            Highlight();
        }

        public virtual void OnDeselect(BaseEventData eventData)
        {
            StopHighlighting();
        }
    }
}