﻿using System.Collections;
using UnityEngine;

namespace CRI.HelloHouston.Experience
{
    /// <summary>
    /// Constructor for XpGroup scriptable object.
    /// </summary>
    [CreateAssetMenu(fileName = "New XpGroup", menuName = "Experience/New XpGroup", order = 1)]
    public class XpGroup : ScriptableObject
    {
        /// <summary>
        /// The name of the experiment.
        /// </summary>
        [Tooltip("The name of the experiment.")]
        public string name;

        /// <summary>
        /// A description of the experiment.
        /// </summary>
        [Tooltip("A description of the experiment.")]
        public string description;

        /// <summary>
        /// The type of gameplay of the experiment.
        /// </summary>
        [Tooltip("The type of gameplay of the experiment.")]
        public string type;

        /// <summary>
        /// The pedagogical content of the experiment.
        /// </summary>
        [Tooltip("The pedagogical content of the experiment.")]
        public string subject;             
    }
}