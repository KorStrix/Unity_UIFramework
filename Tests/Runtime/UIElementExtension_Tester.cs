using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.UI;

namespace UIFramework_Test
{
    [Category("UIFramework")]
    public class UIElementExtension_Tester
    {
        [UnityTest]
        public IEnumerator Text_SeekTween_Int()
        {
            GameObject pObjectTestTarget = new GameObject(nameof(Text_SeekTween_Int));
            Text pText = pObjectTestTarget.AddComponent<Text>();

            int iStartNumber = 0;
            int iDestNumber = int.MaxValue;

            pText.text = iStartNumber.ToString();

            for (int i = 0; i < 10; i++)
            {
                float fSeek_0_1 = Random.Range(0.01f, 0.9f);
                pText.DoSeekTween(iStartNumber, iDestNumber, fSeek_0_1);
                Assert.AreEqual(pText.text, (Mathf.RoundToInt(iDestNumber * fSeek_0_1)).ToString());

                pText.DoSeekTween(iStartNumber, iDestNumber, fSeek_0_1, "n1");
                Assert.AreEqual(pText.text, (Mathf.RoundToInt(iDestNumber * fSeek_0_1)).ToString("n1"));
            }

            pText.DoSeekTween(iStartNumber, iDestNumber, 0f);
            Assert.AreEqual(pText.text, (iStartNumber).ToString());

            pText.DoPlayTween(pText, iStartNumber, iDestNumber, 0.01f);
            yield return null;
            yield return null;
            Assert.AreEqual(pText.text, (iDestNumber).ToString());
        }

        [UnityTest]
        public IEnumerator Text_SeekTween_Float()
        {
            GameObject pObjectTestTarget = new GameObject(nameof(Text_SeekTween_Float));
            Text pText = pObjectTestTarget.AddComponent<Text>();

            float fStartNumber = 0f;
            float fDestNumber = float.MaxValue;

            pText.text = fStartNumber.ToString();

            for (int i = 0; i < 10; i++)
            {
                float fSeek_0_1 = Random.Range(0.01f, 0.9f);
                pText.DoSeekTween(fStartNumber, fDestNumber, fSeek_0_1);
                Assert.AreEqual(pText.text, (fDestNumber * fSeek_0_1).ToString());

                pText.DoSeekTween(fStartNumber, fDestNumber, fSeek_0_1, "n1");
                Assert.AreEqual(pText.text, (fDestNumber * fSeek_0_1).ToString("n1"));
            }

            pText.DoSeekTween(fStartNumber, fDestNumber, 0f);
            Assert.AreEqual(pText.text, (fStartNumber).ToString());

            pText.DoPlayTween(pText, fStartNumber, fDestNumber, 0.01f);
            yield return null;
            yield return null;
            Assert.AreEqual(pText.text, (fDestNumber).ToString());
        }

        [UnityTest]
        public IEnumerator Text_SeekTween_Long()
        {
            GameObject pObjectTestTarget = new GameObject(nameof(Text_SeekTween_Long));
            Text pText = pObjectTestTarget.AddComponent<Text>();

            long iStartNumber = 0;
            long iDestNumber = long.MaxValue;

            pText.text = iStartNumber.ToString();

            for (int i = 0; i < 10; i++)
            {
                float fSeek_0_1 = Random.Range(0.01f, 0.9f);
                pText.DoSeekTween(iStartNumber, iDestNumber, fSeek_0_1);
                Assert.AreEqual(pText.text, (iDestNumber * fSeek_0_1).ToString());

                pText.DoSeekTween(iStartNumber, iDestNumber, fSeek_0_1, "n1");
                Assert.AreEqual(pText.text, (iDestNumber * fSeek_0_1).ToString("n1"));
            }

            pText.DoPlayTween(pText, iStartNumber, iDestNumber, 0.01f);
            yield return null;
            yield return null;
            Assert.AreEqual(pText.text, (iDestNumber).ToString());
        }
    }
}