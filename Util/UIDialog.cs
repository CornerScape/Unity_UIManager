using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Szn.Framework.UI
{
    public class UIDialog : MonoBehaviour
    {
        private Text titleText;

        private Text contentText;

        private GameObject confirmGameObj;
        private Button confirmBtn;
        private Text confirmText;
        private UnityAction confirmBtnAction;

        private GameObject cancelGameObj;
        private Button cancelBtn;
        private Text cancelText;
        private UnityAction cancelBtnAction;

        private CanvasGroup canvasGroup;
        private WaitForSeconds animWait;

        private bool isDialogPlaying;

        private void Start()
        {
            Transform trans = transform;

            Transform contentTrans = trans.Find("Panel/Content");

            titleText = contentTrans.Find("Title").GetComponent<Text>();

            contentText = contentTrans.Find("Content").GetComponent<Text>();

            Transform confirmTrans = contentTrans.Find("OperatingArea/Confirm");
            confirmGameObj = confirmTrans.gameObject;
            confirmBtn = confirmGameObj.GetComponent<Button>();
            confirmBtn.onClick.AddListener(() =>
            {
                confirmBtnAction?.Invoke();
                StartCoroutine(Close());
            });
            confirmText = confirmTrans.Find("Text").GetComponent<Text>();

            Transform cancelTrans = contentTrans.Find("OperatingArea/Cancel");
            cancelGameObj = cancelTrans.gameObject;
            cancelBtn = cancelGameObj.GetComponent<Button>();
            cancelBtn.onClick.AddListener(() =>
            {
                cancelBtnAction?.Invoke();
                StartCoroutine(Close());
            });
            cancelText = cancelTrans.Find("Text").GetComponent<Text>();

            canvasGroup = trans.GetComponent<CanvasGroup>();
            animWait = new WaitForSeconds(.1f);

            isDialogPlaying = false;
        }

        public void Show(string InTitle, string InContent, string InConfirmText, UnityAction InConfirmAction = null)
        {
            Show(InTitle, InContent, InConfirmText, null, InConfirmAction);
        }

        public void Show(string InTitle, string InContent, string InConfirmText, string InCancelText = null,
            UnityAction InConfirmAction = null, UnityAction InCancelAction = null)
        {
            if (string.IsNullOrEmpty(InTitle))
            {
                Debug.LogError("Dialog title can not be empty or null.");
                return;
            }

            if (string.IsNullOrEmpty(InContent))
            {
                Debug.LogError("Dialog content can not be empty or null.");
                return;
            }

            if (isDialogPlaying)
            {
                Debug.LogError("Dialog is showing.");
                return;
            }

            isDialogPlaying = true;

            titleText.text = InTitle;
            contentText.text = InContent;

            if (!confirmGameObj.activeSelf) confirmGameObj.SetActive(true);
            confirmText.text = InConfirmText;
            confirmBtnAction = InConfirmAction;

            bool needCancelBtn = !string.IsNullOrEmpty(InCancelText);
            if (cancelGameObj.activeSelf != needCancelBtn) cancelGameObj.SetActive(needCancelBtn);
            if (needCancelBtn)
            {
                cancelText.text = InCancelText;
                cancelBtnAction = InCancelAction;
            }

            StartCoroutine(Open());
        }

        private IEnumerator Open()
        {
            for (int i = 0; i < 5; i++)
            {
                canvasGroup.alpha += .2f;
                yield return animWait;
            }
        }

        private IEnumerator Close()
        {
            for (int i = 0; i < 5; i++)
            {
                canvasGroup.alpha -= .2f;
                yield return animWait;
            }

            isDialogPlaying = false;
        }
    }
}