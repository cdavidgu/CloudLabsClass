using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System;

namespace GUI
{
    public class CategoryDropHandler : MonoBehaviour, IDropHandler
    {
        Action SidePanelEmptyCallback;
        GameObject SidePanelContent;
        // Start is called before the first frame update
        private void Start()
        {
            SidePanelContent = GameObject.Find("MainPanel/SidePanel/ScrollView/Viewport/Content");
        }
        public void OnDrop(PointerEventData eventData)
        {
            Debug.Log("Dropped");
            if (eventData.pointerDrag != null)
            {
                eventData.pointerDrag.GetComponent<StudentPicMngr>().parentEndDrag = transform.Find("Content").transform;
                eventData.pointerDrag.transform.SetParent(transform.Find("Content").transform);
                if (SidePanelContent.transform.childCount == 0)
                {
                    SidePanelEmptyCallback();
                }
            }
        }
        public void SetEmptySidePanelCallback(Action onClickAction)
        {
            SidePanelEmptyCallback = onClickAction;
        }
    }
}