using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace Akila.FPSFramework
{
    /// <summary>
    /// Base class for all InventoryItems and all PickableItems
    /// </summary>
    public class Item : MonoBehaviour
    {
        [Header("Base")]
        public string Name = "Default";
        public Sprite gunImage;
        public Color grade;

        public bool isActive { get; set; } = true;
    }
}