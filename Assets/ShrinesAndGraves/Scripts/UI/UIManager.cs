using UnityEngine;
using System.Collections;

namespace Shrines {
    public class UIManager : MonoBehaviour {
        public DialogChoice dialogChoice;
        public DialogWindow defaultWindow;
        public DialogTree testDialog;

        public static UIManager Instance;

        public IEnumerator ShowDialog(DialogTree dialog, System.Action onEndDialog = null)
        {
            defaultWindow.gameObject.SetActive(true);
            var node = dialog.Root;
            while (true)
            {
                for (int i = 0; i < node.lines.Length; i++)
                {
                    defaultWindow.ShowText(node.lines[i].line);
                    yield return new WaitForSeconds(1);
                }
                if (node.branches.Length > 0)
                {
                    switch (node.branchType)
                    {
                        case DialogTree.BranchType.DialogChoice:
                            {
                                dialogChoice.gameObject.SetActive(true);
                                dialogChoice.ShowChoices(0, node.keys);
                                while (!dialogChoice.selected)
                                {
                                    yield return 1;
                                }
                                dialogChoice.gameObject.SetActive(false);
                                node = dialog[dialogChoice.current];
                            }
                            break;
                        case DialogTree.BranchType.FlagSet:
                            {
                                for (int i = 0; i < node.keys.Length; i++)
                                {
                                    if (DialogTree.flags.Contains(node.keys[i]))
                                    {
                                        node = dialog[node.branches[i]];
                                    }
                                }
                                if (node == dialog.Root) break;
                            }
                            break;
                        default: break;
                    }
                }
                else
                {
                    break;
                }
            }
            defaultWindow.gameObject.SetActive(false);
            if (onEndDialog != null)
            {
                onEndDialog.Invoke();
            }
        }

        void Awake()
        {
            if (this.SetInstanceOrKill(ref Instance))
            {

            }
        }

        public void DoTestDialog()
        {
            if (testDialog != null)
            {
                StartCoroutine(ShowDialog(testDialog));
            }
        }
    }
}