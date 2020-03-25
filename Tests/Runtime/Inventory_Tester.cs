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
    public class 인벤토리_테스터
    {
        const int const_iTestSlotCount = 5;

        [Test]
        public void 인벤토리_간단한테스트()
        {
            GameObject pObjectInventory = new GameObject(nameof(인벤토리_간단한테스트));

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