using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.UI;

namespace StrixLibrary_Test
{
    public class UIElementExtension_Tester
    {
        [Test]
        public void Text_SeekTween_Int()
        {
            GameObject pObjectTestTarget = new GameObject(nameof(Text_SeekTween_Int));
            Text pText = pObjectTestTarget.AddComponent<Text>();

            int iStartNumber = 0;
            int iDestNumber = 100;

            pText.text = iStartNumber.ToString();

            for(int i = 0; i < 10; i++)
            {
                float fSeek_0_1 = Random.Range(0.01f, 0.9f);
                pText.DoSeekTween(pText, iStartNumber, iDestNumber, fSeek_0_1);
                Assert.AreEqual(pText.text, (Mathf.RoundToInt(iDestNumber * fSeek_0_1)).ToString());
            }

            pText.DoSeekTween(pText, iStartNumber, iDestNumber, 1f);
            Assert.AreEqual(pText.text, (iDestNumber).ToString());

            pText.DoSeekTween(pText, iStartNumber, iDestNumber, 2f);
            Assert.AreEqual(pText.text, (iDestNumber).ToString());
        }

        [Test]
        public void Text_SeekTween_Float()
        {
            GameObject pObjectTestTarget = new GameObject(nameof(Text_SeekTween_Float));
            Text pText = pObjectTestTarget.AddComponent<Text>();

            float fStartNumber = 0f;
            float fDestNumber = 100f;

            pText.text = fStartNumber.ToString();

            for (int i = 0; i < 10; i++)
            {
                float fSeek_0_1 = Random.Range(0.01f, 0.9f);
                pText.DoSeekTween(pText, fStartNumber, fDestNumber, fSeek_0_1);
                Assert.AreEqual(pText.text, (fDestNumber * fSeek_0_1).ToString());
            }

            pText.DoSeekTween(pText, fStartNumber, fDestNumber, 1f);
            Assert.AreEqual(pText.text, (fDestNumber).ToString());

            pText.DoSeekTween(pText, fStartNumber, fDestNumber, 2f);
            Assert.AreEqual(pText.text, (fDestNumber).ToString());
        }
    }
}