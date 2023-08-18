using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;

namespace GUI
{
    public class Modal
    {
        GameObject TextInputName, TextInputSurname, TextInputCode, TextInputEmail, TextInputScore, SaveButton, CancelButton, uiEditPanel;
        public StudentTileGUI SelectedStudentTile;
        public String Name
        {
            get { return TextInputName.GetComponent<TMP_InputField>().text; }
            set { TextInputName.GetComponent<TMP_InputField>().text = value; }
        }
        public String Surname
        {
            get { return TextInputSurname.GetComponent<TMP_InputField>().text; }
            set { TextInputSurname.GetComponent<TMP_InputField>().text = value; }
        }

        public String Code
        {
            get { return TextInputCode.GetComponent<TMP_InputField>().text; }
            set { TextInputCode.GetComponent<TMP_InputField>().text = value; }
        }

        public String Email
        {
            get { return TextInputEmail.GetComponent<TMP_InputField>().text; }
            set { TextInputEmail.GetComponent<TMP_InputField>().text = value; }
        }
        public String Score
        {
            get { return TextInputScore.GetComponent<TMP_InputField>().text; }
            set { TextInputScore.GetComponent<TMP_InputField>().text = value; }
        }

        public GameObject UIModal
        {
            get { return uiEditPanel; }
        }

        public Modal(GameObject _uieditPanel)
        {
            uiEditPanel = _uieditPanel;
            TextInputName = _uieditPanel.transform.Find("Modal/InputName").gameObject;
            TextInputSurname = _uieditPanel.transform.Find("Modal/InputSurname").gameObject;
            TextInputCode = _uieditPanel.transform.Find("Modal/InputCode").gameObject;
            TextInputEmail = _uieditPanel.transform.Find("Modal/InputEmail").gameObject;
            TextInputScore = _uieditPanel.transform.Find("Modal/InputScore").gameObject;
            SaveButton = _uieditPanel.transform.Find("Modal/SaveButton").gameObject;
            CancelButton = _uieditPanel.transform.Find("Modal/CancelButton").gameObject;
        }
        public void SetSaveButtonOnClickAction(Action onclickAction)
        {
            Button saveButton = SaveButton.GetComponent<Button>();
            saveButton.onClick.AddListener(() => onclickAction());
        }
        public void SetCancelButtonOnClickAction(Action onclickAction)
        {
            Button cancelButton = CancelButton.GetComponent<Button>();
            cancelButton.onClick.AddListener(() => onclickAction());
        }

    }
}
