using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;
/// <summary>
/// Los objetos de esta clase almacenan las referencias de los componentes gráficos que permiten visualizar
/// en pantalla los datos de los estudiantes y las referencias de los botones que pertmiten al usuario 
/// modificar o eliminar dichos datos. 
/// Estos componentes gráficos son los objetos que posee el Prefab creado para visualizar los datos de 
/// cada estudiante. 
/// </summary> 

public class StudentTileGUI : MonoBehaviour
{
    GameObject TextName, TextSurname, TextCode, TextEmail, TextScore, EditButton, DeleteButton, PassCheck;

    /* ------------------------------- Propiedades ------------------------------ */
    public String Name
    {
        get { return TextName.GetComponent<TMP_Text>().text; }
        set { TextName.GetComponent<TMP_Text>().text = value; }
    }
    public String Surname
    {
        get { return TextSurname.GetComponent<TMP_Text>().text; }
        set { TextSurname.GetComponent<TMP_Text>().text = value; }
    }

    public String Code
    {
        get { return TextCode.GetComponent<TMP_Text>().text; }
        set { TextCode.GetComponent<TMP_Text>().text = value; }
    }

    public String Email
    {
        get { return TextEmail.GetComponent<TMP_Text>().text; }
        set { TextEmail.GetComponent<TMP_Text>().text = value; }
    }
    public String Score
    {
        get { return TextScore.GetComponent<TMP_Text>().text; }
        set { TextScore.GetComponent<TMP_Text>().text = value; }
    }
    public Color ScoreColor
    {
        get { return TextScore.GetComponent<TMP_Text>().color; }
        set { TextScore.GetComponent<TMP_Text>().color = value; }
    }
    public bool passCheck
    {
        get { return PassCheck.GetComponent<Toggle>().isOn; }
        set { PassCheck.GetComponent<Toggle>().isOn = value; }
    }


    /// <summary>
    /// Referencias de los objetos (hijos) del prefab.
    /// </summary>
    public void Init()
    {
        TextName = transform.Find("Name").gameObject;
        TextSurname = transform.Find("Surname").gameObject;
        TextCode = transform.Find("Code").gameObject;
        TextEmail = transform.Find("Email").gameObject;
        TextScore = transform.Find("Score").gameObject;
        DeleteButton = transform.Find("DeleteButton").gameObject;
        EditButton = transform.Find("EditButton").gameObject;
        PassCheck = transform.Find("PassCheck").gameObject;
    }
    /// <summary>
    /// Función para Eliminar datos del estudiante. En este caso se elimina el prefab anteriormente 
    /// instanciado
    /// </summary>
    public void SetDeleteButtonOnClickAction(Action onclickAction)
    {
        Button deleteButton = DeleteButton.GetComponent<Button>();
        deleteButton.onClick.AddListener(() => onclickAction());
    }
    /// <summary>
    /// Función para asignar la acción a ejecutar cuando se pulse el botón "Editar" del prefab
    /// </summary>
    /// <param name="onclickAction"></param>
    public void SetEditButtonOnClickAction(Action onclickAction)
    {
        Button editButton = EditButton.GetComponent<Button>();
        editButton.onClick.AddListener(() => onclickAction());
    }
    public void SetPassCheckButtonValueChanged(Action onclickAction)
    {
        Toggle passCheck = PassCheck.GetComponent<Toggle>();
        passCheck.onValueChanged.AddListener((value) => onclickAction());
    }
    public void SetPashCheckWithoutNotify(bool value)
    {
        PassCheck.GetComponent<Toggle>().SetIsOnWithoutNotify(value);
    }
}
