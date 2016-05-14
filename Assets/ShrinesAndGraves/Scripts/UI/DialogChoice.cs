using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;

namespace Shrines
{
    public class DialogChoice : MonoBehaviour
    {
        [System.Serializable]
        public class Choice
        {
            public RectTransform root;
            
            Text text;

            public void Init()
            {
                text = root.gameObject.GetComponentInChildren<Text>();
            }

            public void SetText(string str)
            {
                text.text = str;
            }
        }

        public Choice[] choices;
        public RectTransform choiceArrow;

        public int current { get; private set; }
        
        public bool selected {get; private set;}

        float arrowStep;

        void Awake()
        {
            arrowStep = choiceArrow.sizeDelta.y;
            for (int i = 0; i < choices.Length; i++)
            {
                choices[i].Init();
            }
        }

        public void ShowChoices(int def = 0, params string[] strings)
        {
            Clear();
            for (int i = 0; i < strings.Length; i++)
            {
                choices[i].root.gameObject.SetActive(true);
                choices[i].SetText(strings[i]);
            }
            current = def;
        }

        public void Clear()
        {
            for (int i = 0; i < choices.Length; i++)
            {
                choices[i].SetText(null);
                choices[i].root.gameObject.SetActive(false);
            }
            current = 0;
            selected = false;
        }

        public void Choose(int i)
        {
            selected = true;
        }

        public void Select(int i)
        {
            current = i;
            EventSystem.current.SetSelectedGameObject(choices[i].root.gameObject);
            choiceArrow.anchoredPosition = new Vector2(0, i * arrowStep);
        }
    }
}