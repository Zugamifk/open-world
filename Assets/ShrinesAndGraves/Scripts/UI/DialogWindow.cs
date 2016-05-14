using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace Shrines
{
    public class DialogWindow : MonoBehaviour
    {
        public RectTransform textWindow;
        public Text dialogText;

        public void ShowText(string text)
        {
            dialogText.text = text;
        }
    }
}