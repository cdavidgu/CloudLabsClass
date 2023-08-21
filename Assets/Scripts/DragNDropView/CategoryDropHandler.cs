using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System;


/// <summary>
/// Los objetos de clase CategoryDropHandler verifican, a través del sistema de eventos de Unity, si el usuario ha realizado
/// un "Drop", para ello se implemeta el método OnDrop de la interface IDropHandler.
/// </summary>
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
                /// Se asigna la jerarquía del objeto que que representa la imagen del estudiante. En este caso 
                /// la imagen del estudiante pasa a ser hijo del objeto que representa la categoría. 
                eventData.pointerDrag.GetComponent<StudentPicMngr>().parentEndDrag = transform.Find("Content").transform;
                eventData.pointerDrag.transform.SetParent(transform.Find("Content").transform);
                /// Cuando el profesor haya arrastrado todos los estudiantes desde el panel lateral hacia los pàneles 
                /// de aprobados y reprobados se ejecuta esete métdodo. para el caso la verificación de cada estudiante
                /// en cada categoría. 
                /// 
                if (SidePanelContent.transform.childCount == 0)
                {
                    SidePanelEmptyCallback();
                }
            }
        }
        /// <summary>
        /// Método que permite asignar una acción cuando el pánel lateral esté vacío
        /// </summary>
        /// <param name="onEmptyAction"></param>
        public void SetEmptySidePanelCallback(Action onEmptyAction)
        {
            SidePanelEmptyCallback = onEmptyAction;
        }
    }
}