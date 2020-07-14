using System.Collections.Generic;
using NUnit.Framework;

namespace UIFramework_Test
{
    [Category("UIFramework")]
    public class 심플풀_테스터
    {
        public class TestObject
        {
            public enum EState
            {
                Init,
                Used,
                Unused,
                Destroyed,
            }


            public int iID { get; private set; } = -1;
            public EState eState { get; private set; } = EState.Init;

            public TestObject(int iID)
            {
                this.iID = iID;
                eState = EState.Init;
            }

            public void Used()
            {
                eState = EState.Used;
            }

            public void UnUsed()
            {
                eState = EState.Unused;
            }

            public void OnDestroy()
            {
                eState = EState.Destroyed;
            }
        }

        [Test]
        public void 심플풀_동작테스트()
        {
            int iMaxPoolCount = 10;


            // Init & PrePooling (초기화 및 미리 생성) 테스트
            SimplePool<TestObject> pPool = new SimplePool<TestObject>(
                OnCreateItem: iCount => new TestObject(iCount),
                OnDestroyItem: (pTestObject) => pTestObject.OnDestroy(),
                iPrePoolCount: iMaxPoolCount);


            Assert.AreEqual(pPool.iAllInstanceCount, iMaxPoolCount);
            Assert.AreEqual(pPool.iUsedInstanceCount, 0);
            Assert.AreEqual(pPool.iUnUsedInstanceCount, iMaxPoolCount);


            // Pop(사용 안하는 오브젝트 요청) 테스트
            System.Random pRandom = new System.Random();
            int iRandomPop = pRandom.Next(5, iMaxPoolCount);
            List<TestObject> listUsed = new List<TestObject>();
            for (int i = 0; i < iRandomPop; i++)
            {
                TestObject pTestObject = pPool.DoPop();
                Assert.AreEqual(pTestObject.eState, TestObject.EState.Init);

                pTestObject.Used();
                Assert.AreEqual(pTestObject.eState, TestObject.EState.Used);

                listUsed.Add(pTestObject);
            }
            Assert.AreEqual(pPool.iAllInstanceCount, iMaxPoolCount);
            Assert.AreEqual(pPool.iUsedInstanceCount, iRandomPop);
            Assert.AreEqual(pPool.iUnUsedInstanceCount, iMaxPoolCount - iRandomPop);


            // Push(리턴) 테스트
            for (int i = 0; i < listUsed.Count; i++)
            {
                TestObject pTestObject = listUsed[i];
                pTestObject.UnUsed();
                Assert.AreEqual(pTestObject.eState, TestObject.EState.Unused);

                pPool.DoPush(pTestObject);
            }
            listUsed.Clear();
            Assert.AreEqual(pPool.iAllInstanceCount, iMaxPoolCount);
            Assert.AreEqual(pPool.iUsedInstanceCount, 0);
            Assert.AreEqual(pPool.iUnUsedInstanceCount, iMaxPoolCount);


            // MaxOver Pop 테스트
            int iNewMaxPoolCount = iRandomPop + iMaxPoolCount;
            for (int i = 0; i < iNewMaxPoolCount; i++)
            {
                TestObject pTestObject = pPool.DoPop();
                pTestObject.Used();
                Assert.AreEqual(pTestObject.eState, TestObject.EState.Used);

                listUsed.Add(pTestObject);
            }
            Assert.AreEqual(pPool.iAllInstanceCount, iNewMaxPoolCount);
            Assert.AreEqual(pPool.iUsedInstanceCount, iNewMaxPoolCount);
            Assert.AreEqual(pPool.iUnUsedInstanceCount, 0);


            // PushAll 테스트
            pPool.DoPushAll();
            Assert.AreEqual(pPool.iAllInstanceCount, iNewMaxPoolCount);
            Assert.AreEqual(pPool.iUsedInstanceCount, 0);
            Assert.AreEqual(pPool.iUnUsedInstanceCount, iNewMaxPoolCount);


            // Destroy 테스트
            pPool.DoDestroyPool(true);
            for (int i = 0; i < listUsed.Count; i++)
                Assert.AreEqual(listUsed[i].eState, TestObject.EState.Destroyed);
            Assert.AreEqual(pPool.iAllInstanceCount, 0);
            Assert.AreEqual(pPool.iUsedInstanceCount, 0);
            Assert.AreEqual(pPool.iUnUsedInstanceCount, 0);
        }


    }
}
