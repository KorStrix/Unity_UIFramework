using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.TestTools;
using NUnit.Framework;
using UIFramework;
using System.Linq;

namespace UIFramework_Test
{
    [Category("UIFramework")]
    public class Inventory_Tester
    {
        const int const_iTestSlotCount = 5;

        [Test]
        public void Inventory_Test()
        {
            GameObject pObjectInventory = new GameObject(nameof(Inventory_Test));

            for (int i = 0; i < const_iTestSlotCount; i++)
            {
                InventorySlot pSlot = new GameObject($"{nameof(InventorySlot)}_{i + 1}").AddComponent<InventorySlot>();
                pSlot.transform.SetParent(pObjectInventory.transform);
                pSlot.EventAwake();
            }

            Inventory pInventory = pObjectInventory.AddComponent<Inventory>();
            pInventory.EventAwake();

            Assert.AreEqual(pInventory.arrSlot.Count(), const_iTestSlotCount);
        }
    }
}