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
        public enum EItemName
        { 
            A, B, C, D, E,
            MAX,
        }

        public class InventoryData_Dummy : IInventoryData
        {
            public string IInventoryData_Key => eItemName.ToString();

            public int IInventoryData_Count => iCount;

            public EItemName eItemName;
            public int iCount;

            public InventoryData_Dummy(EItemName eItemName, int iCount = 1)
            {
                this.eItemName = eItemName; this.iCount = iCount;
            }

            public IInventoryData IInventoryData_AddOrMinusCount(int iCount)
            {
                return this;
            }
        }

        const int const_iTestSlotCount = 5;

        [Test]
        public void 인벤토리_간단한테스트()
        {
            // Arrange
            Inventory pInventory = Create_TestInventory(nameof(인벤토리_간단한테스트));


            // Act


            // Assert
            Assert.AreEqual(pInventory.arrSlot.Count(), const_iTestSlotCount);
        }

        [Test]
        public void 인벤토리에_데이터를넣으면_슬롯에_데이터가들어갑니다()
        {
            // Arrange
            Inventory pInventory = Create_TestInventory(nameof(인벤토리에_데이터를넣으면_슬롯에_데이터가들어갑니다));


            // Act
            for (int i = 0; i < (int)EItemName.MAX; i++)
                pInventory.DoAddData(new InventoryData_Dummy((EItemName)i));


            // Assert
            Assert.AreEqual(pInventory.arrData.Count(), (int)EItemName.MAX);
            Assert.AreEqual(pInventory.arrData.First().IInventoryData_Key, EItemName.A.ToString());
            Assert.AreEqual(pInventory.arrData.Last().IInventoryData_Key, (EItemName.MAX - 1).ToString());
        }

        [Test]
        public void 인벤토리에_데이터를넣었다가_클리어를하면_인벤토리는_비워집니다()
        {
            // Arrange
            Inventory pInventory = Create_TestInventory(nameof(인벤토리에_데이터를넣었다가_클리어를하면_인벤토리는_비워집니다));


            // Act
            for (int i = 0; i < (int)EItemName.MAX; i++)
                pInventory.DoAddData(new InventoryData_Dummy((EItemName)i));
            pInventory.DoClearData();


            // Assert
            Assert.AreEqual(pInventory.arrData.Count(), 0);
        }



        // ============================================================================================

        private Inventory Create_TestInventory(string strObjectName)
        {
            GameObject pObjectInventory = new GameObject(strObjectName);

            for (int i = 0; i < const_iTestSlotCount; i++)
            {
                InventorySlot pSlot = new GameObject($"{nameof(InventorySlot)}_{i + 1}").AddComponent<InventorySlot>();
                pSlot.transform.SetParent(pObjectInventory.transform);
                pSlot.EventAwake();
            }

            Inventory pInventory = pObjectInventory.AddComponent<Inventory>();
            pInventory.EventAwake();
            return pInventory;
        }
    }
}