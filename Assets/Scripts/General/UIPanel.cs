﻿using System;
using UnityEngine;

namespace CRI.HelloHouston
{
    [RequireComponent(typeof(CanvasGroup))]
    public abstract class UIPanel : MonoBehaviour
    {
        /// <summary>
        /// If true, this panel is hidden.
        /// </summary>
        protected bool _hidden = false;
        /// <summary>
        /// The CanvasGroup component.
        /// </summary>
        protected CanvasGroup _canvasGroup;
        /// <summary>
        /// If true, hides the UIPanel on start.
        /// </summary>
        [Tooltip("If true, hides the UIPanel on strt.")]
        public bool hideOnStart;

        protected virtual void Awake()
        {
            _canvasGroup = GetComponent<CanvasGroup>();
            if (hideOnStart)
                Hide();
            else
                Show();
        }

        public void Hide()
        {
            _hidden = true;
            _canvasGroup.alpha = 0;
            _canvasGroup.interactable = false;
            _canvasGroup.blocksRaycasts = false;
        }

        public void Show()
        {
            _hidden = false;
            _canvasGroup.alpha = 1;
            _canvasGroup.interactable = true;
            _canvasGroup.blocksRaycasts = true;
        }

        public virtual void Init(object obj)
        {

        }
    }
}