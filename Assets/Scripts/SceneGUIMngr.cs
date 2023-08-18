using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Models;
using Controllers;
using UnityEngine.UI;
using System;
using TMPro;

namespace GUI
{
    public class SceneGUIMngr : MonoBehaviour
    {
        /// <summary>
        /// Prefab creado para visualizar los datos de cada estudiante. 
        /// Se crea un campo para asignar el prefab en Unity.
        /// </summary>
        [SerializeField]
        StudentTileGUI tileStudentPrefab;
        GameObject EditPanel;
        GameObject ScrollViewContent;
        Modal EditPanelModal;
        bool isModalToCreate = false;

        // Start is called before the first frame update
        void Start()
        {

            /// Se Inicializan los componentes gráficos de la aplicación
            EditPanel = GameObject.Find("EditPanel");
            ScrollViewContent = GameObject.Find("MainPanel/ScrollView/Viewport/Content");
            InitMainPanel(AppController.Instance.ListStudents);
            InitEditPanel();
            EditPanel.SetActive(false);
        }

        private void InitMainPanel(ListStudents _listStudents)
        {
            /// <summary>
            /// Se inicializa el panel principial. Esto consiste en la visualización de la lista de estudiantes obtenida 
            /// del archivo JSON en la vista principal. La información de cada estudiante es visualizada luego de instanciar un Prefab el cual 
            /// contiene un componente de clase StudentTileGUI en donde se almacenan los componentes gráficos que permiten
            /// visualizar los datos del estudiante y dos botones que permitirán editar y/o eliminar los datos del estudiante.   
            /// </summary>      
            foreach (Student student in _listStudents.students)
            {
                // Debug.Log($"Name: {student.name}, Age: {student.surname}, Email: {student.code}");
                StudentTileGUI _tile = CreateNewTile();
                _tile.Name = student.name;
                _tile.Surname = student.surname;
                _tile.Code = student.code;
                _tile.Email = student.email;
                _tile.Score = student.score.ToString("F1");
            }
            /// <summary>
            /// De igual manera se referencia el botón Crear Nuevo Estudiante para asignar las acciones que se 
            /// deben ejecutar cuando el profesor pulse dicho botón. En este caso se muestra en pantalla un 
            /// modal que permitirá ingresar los datos necesarios para crear un nuevo estudiante. 
            /// </summary>
            GameObject CreateStudenButton = GameObject.Find("MainPanel/CreateStudentButton").gameObject;
            Button _newStudentBttn = CreateStudenButton.GetComponent<Button>();
            _newStudentBttn.onClick.AddListener(() =>
            {
                isModalToCreate = true;
                ShowModalToCreate();
            });
            /// <summary>
            /// Por último se referencia el elemento gráfico "Dropdown" para asignar las acciones que se 
            /// deben ejecutar cuando el profesor cambie el valor del formato de notas "De 0 a 5" y "De 0 a 100"
            /// </summary>
            GameObject ScoreFormat = GameObject.Find("MainPanel/ScoreFormat").gameObject;
            TMP_Dropdown _dropdown = ScoreFormat.GetComponent<TMP_Dropdown>();
            //Add listener for when the value of the Dropdown changes, to take action
            _dropdown.onValueChanged.AddListener(delegate
            {
                ScoreFormatChanged(_dropdown);
            });
        }

        /// <summary>
        /// A través de esta función se evalúa cuál opción de formato de nota ha escogido el profesor y 
        /// se actualiza la propiedad "Score" del objeto de clase StudentTileGUI. Dicho objeto es componente 
        /// de cada Prefab instanciado a la hora de crear la lista de estutiantes que se visualizan en pantalla. 
        /// </summary>
        /// <param name="dropdown"></param>
        private void ScoreFormatChanged(TMP_Dropdown dropdown)
        {
            /// La nota, que en un principio está en formato de 0 a 5, es convertida a formato de 0 a 100
            if (dropdown.value == 1)
            {
                foreach (Transform child in ScrollViewContent.transform)
                {
                    StudentTileGUI _tile = child.gameObject.GetComponent<StudentTileGUI>();
                    float floatScore = float.Parse(_tile.Score);
                    _tile.Score = (floatScore * 100 / 5).ToString();
                }
            }

            /// La nota, que está en formato de 0 a 100, es convertida a formato de 0 a 5
            else
            {
                foreach (Transform child in ScrollViewContent.transform)
                {
                    StudentTileGUI _tile = child.gameObject.GetComponent<StudentTileGUI>();
                    int intScore = int.Parse(_tile.Score);
                    float _f = (float)intScore * 5 / 100;
                    _tile.Score = _f.ToString("F1");
                }
            }
        }

        /// <summary>
        /// Se inicializa el panel(modal) que permitirá al usuario modificar los datos de un estudiante
        /// ingresar los datos de un estudiante nuevo
        /// </summary>
        private void InitEditPanel()
        {
            /// <summary>
            /// Objeto de clase Modal el cual contiene las referencias de los componentes gráficos del modal.
            /// En este caso los campos (InputField) para los datos del estudiante, un botón para guardar los cambios
            /// y un botón para cancelar la operación.
            /// </summary>
            EditPanelModal = new Modal(EditPanel);
            /// <summary>
            /// Se asignan las acciones que se deben ejecutar cuando el botón "Guardar" del modal sea pulsado
            /// </summary>
            EditPanelModal.SetSaveButtonOnClickAction(() =>
            {
                ///
                /// En caso de que el modal se haya llamado para crear un nuevo estudiante se crea una nueva 
                /// instancia del prefab y se guarda la información de sus componentes en la variable "SelectedStudentTile"
                /// del controllodar de la aplicación. 
                /// 
                if (isModalToCreate)
                {
                    AppController.Instance.SelectedStudentTile = CreateNewTile();
                }
                AppController.Instance.SelectedStudentTile.Name = EditPanelModal.Name;
                AppController.Instance.SelectedStudentTile.Surname = EditPanelModal.Surname;
                AppController.Instance.SelectedStudentTile.Code = EditPanelModal.Code;
                AppController.Instance.SelectedStudentTile.Email = EditPanelModal.Email;
                AppController.Instance.SelectedStudentTile.Score = EditPanelModal.Score;
                EditPanel.SetActive(false);
            });
            /// <summary>
            /// Se oculta el modal si se pulsa el botón "Cancelar" del modal.
            /// </summary>
            /// <returns></returns>
            EditPanelModal.SetCancelButtonOnClickAction(() => EditPanel.SetActive(false));
        }

        /// <summary>
        /// Crea una instancia del prefab y a través del método "Init()" se referencian los componentes
        /// gráficos del mismo. para el caso, campos de texto y dos botones (para editar o eliminar).
        /// De igual forma se asignan las acciones que se llamarán cuando dichos botones sean pulsados. 
        /// /// </summary>
        /// <returns></returns>
        private StudentTileGUI CreateNewTile()
        {
            StudentTileGUI _tile = Instantiate(tileStudentPrefab, ScrollViewContent.transform);
            _tile.Init();
            /// <summary>
            /// Cuando se pulse el botón para editar, se visualiza el modal con la información del estudiante
            /// seleccionado.
            /// </summary>
            _tile.SetEditButtonOnClickAction(() =>
            {
                isModalToCreate = false;
                ShowModalToEdit(_tile);
            });
            /// <summary>
            /// Cuando se pulse el botón para eliminar se llama la función que destruye el objeto
            /// </summary>
            _tile.SetDeleteButtonOnClickAction();
            return _tile;
        }

        /// <summary>
        /// Visualización del modal con los datos del estudiante que se quiere modificar
        /// </summary>
        /// <param name="_studentTile"></param>
        private void ShowModalToEdit(StudentTileGUI _studentTile)
        {
            EditPanel.SetActive(true);
            AppController.Instance.SelectedStudentTile = _studentTile;
            EditPanelModal.Name = _studentTile.Name;
            EditPanelModal.Surname = _studentTile.Surname;
            EditPanelModal.Email = _studentTile.Email;
            EditPanelModal.Code = _studentTile.Code;
            EditPanelModal.Score = _studentTile.Score;
        }

        /// <summary>
        /// Visualización del modal con los campos en blanco para que el usuario digite los datos del 
        /// estudiante que desea crear.
        /// </summary>
        private void ShowModalToCreate()
        {
            EditPanel.SetActive(true);
            EditPanelModal.Name = "";
            EditPanelModal.Surname = "";
            EditPanelModal.Email = "";
            EditPanelModal.Code = "";
            EditPanelModal.Score = "";
        }
    }
}
