using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Models;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// Los onjetos de clase StudentPicMngr almacenan la información de cada estudiante dentro de un objketo de clase Student
/// y además se encarga de detectar las acciones del usuario: CLick sobre Imagen de estudiante, "Drag" de una Imagen de 
/// estudiante y finalización de "Drag" del estudiante. 
/// La detección del clik y del drag se logra a través del sistema de eventos de unity y se implementan los métodos OnBeginDragg
/// OnDrag, OnEndDrag y OnPointerDown de las interfaces IBeginDragHandler, IEndDragHandler, IDragHandler, IPointerDownHandler de Unity.
/// 
/// </summary>
namespace GUI
{
    public class StudentPicMngr : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler, IPointerDownHandler
    {
        // Start is called before the first frame update
        Action OnClickDownCallback;
        [HideInInspector] public Student studentData;
        [HideInInspector] public Transform parentEndDrag;



        public void OnBeginDrag(PointerEventData eventData)
        {
            Debug.Log("BeggingDragging");
            parentEndDrag = transform.parent;
            transform.SetParent(transform.root);
            transform.SetAsLastSibling();
            transform.GetComponent<Image>().raycastTarget = false;
        }

        public void OnDrag(PointerEventData eventData)
        {
            // Debug.Log("Dragging");
            transform.position = Input.mousePosition;
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            Debug.Log("EndDrag");
            transform.SetParent(parentEndDrag);
            transform.GetComponent<Image>().raycastTarget = true;
        }


        public void OnPointerDown(PointerEventData eventData)
        {
            // Debug.Log("clickDown");
            OnClickDownCallback();
        }


        public void SetOnClickDownCallback(Action onClickAction)
        {
            OnClickDownCallback = onClickAction;
        }
    }
}