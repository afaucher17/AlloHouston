﻿using CRI.HelloHouston.Experience;
using UnityEngine;

namespace CRI.HelloHouston.Calibration
{
    public enum ElementType
    {
        WallTopLeft,
        WallTopRight,
        Tablet,
        WallBottom,
        Hologram,
        Corner,
        Door,
        Unknown
    }

    public class VirtualElement : MonoBehaviour
    {
        /// <summary>
        /// Prefab of the element.
        /// </summary>
        private XPElement _elementPrefab;
        /// <summary>
        /// Instance of the element.
        /// </summary>
        public XPElement currentElement { get; protected set; }

        public XPContext xpContext { get; protected set; }
        /// <summary>
        /// The type of element.
        /// </summary>
        [Tooltip("The type of placeholder.")]
        [SerializeField]
        private ElementType _elementType = ElementType.Unknown;
        /// <summary>
        /// The type of element.
        /// </summary>
        public ElementType elementType { get { return _elementType; } }
        /// <summary>
        /// Places an experience element on a virtual element.
        /// </summary>
        /// <param name="element"></param>
        public virtual void PlaceObject(XPElement element, XPContext xpContext)
        {
            _elementPrefab = element;
            this.xpContext = xpContext;
        }
        /// <summary>
        /// Instantiate the element prefab and initializes it with the help of its xpManager
        /// </summary>
        /// <param name="xpManager"></param>
        /// <returns></returns>
        public virtual XPElement Init()
        {
            Clean();
            currentElement = Instantiate(_elementPrefab, transform);
            if (!currentElement.canvasElement)
            {
                Vector3 localScale = currentElement.transform.localScale;
                Vector3 lossyScale = gameObject.transform.lossyScale;
                currentElement.transform.localScale = new Vector3(
                    localScale.x / lossyScale.x,
                    localScale.y / lossyScale.y,
                    localScale.z / lossyScale.z
                    );
            }
            currentElement.Init(this);
            return currentElement;
        }
        /// <summary>
        /// Dismiss the current element and sets its value to null.
        /// </summary>
        public virtual void Clean()
        {
            if (currentElement != null)
            {
                currentElement.Dismiss();
                currentElement = null;
            }
        }
        /// <summary>
        /// Return the experience element stored in the virtual element.
        /// </summary>
        /// <typeparam name="T">The type of XPElement</typeparam>
        /// <returns>An instance of XPElement</returns>
        public virtual T GetObject<T>() where T : XPElement, new()
        {
            return _elementPrefab as T;
        }
    }
}