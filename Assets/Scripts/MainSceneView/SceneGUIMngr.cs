using UnityEngine;
using Models;
using Controllers;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
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
        GameObject VerifyPanel;
        GameObject ScrollViewContent;
        Modal EditPanelModal;
        bool isModalToCreate;
        bool closeScene;
        const string CourseClosedMessage = "El curso ya ha sido calificado, deseas volver a calificar?";

        // Start is called before the first frame update
        void Start()
        {

            /// Se Inicializan los componentes gráficos de la aplicación
            EditPanel = GameObject.Find("EditPanel");
            VerifyPanel = GameObject.Find("VerifyMssgPanel");
            VerifyPanel.transform.Find("Modal/OKButton").GetComponent<Button>().onClick.AddListener(() => VerifyOKButtonCallback());
            VerifyPanel.transform.Find("Modal/CancelButton").GetComponent<Button>().onClick.AddListener(() => { VerifyPanel.SetActive(false); });
            if (AppController.Instance.classVerified)
                VerifyPanel.transform.Find("Modal/CancelButton").gameObject.SetActive(true);
            else
                VerifyPanel.transform.Find("Modal/CancelButton").gameObject.SetActive(false);

            ScrollViewContent = GameObject.Find("MainPanel/ScrollView/Viewport/Content");
            InitMainPanel(AppController.Instance.ListStudents);
            InitEditPanel();
            EditPanel.SetActive(false);
            VerifyPanel.SetActive(false);
        }

        /// <summary>
        /// EL panel que alerta al profesor con algún mensaje. Función que llama el botón OK del panel de alertas
        /// </summary>
        private void VerifyOKButtonCallback()
        {
            VerifyPanel.SetActive(false);
            if (AppController.Instance.classVerified)
            {
                AppController.Instance.classVerified = false;
                VerifyPanel.transform.Find("Modal/CancelButton").gameObject.SetActive(false);
            }
            if (closeScene)
            {
                SaveDataScene();
                SceneManager.LoadScene("DragNDropScene");
            }
        }

        void ShowAlertPanel(string mssg)
        {
            VerifyPanel.transform.Find("Modal/Message").GetComponent<TMP_Text>().text = mssg;
            VerifyPanel.SetActive(true);
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
                _tile.Score = student.score;
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
                if (AppController.Instance.classVerified)
                {
                    ShowAlertPanel(CourseClosedMessage);
                }
                else
                {
                    ShowModalToCreate();
                    isModalToCreate = true;
                }
            });


            /// <summary>
            /// Se referencia el elemento gráfico "Dropdown" para asignar las acciones que se 
            /// deben ejecutar cuando el profesor cambie el valor del formato de notas "De 0 a 5" y "De 0 a 100"
            /// </summary>
            GameObject ScoreFormat = GameObject.Find("MainPanel/ScoreFormat").gameObject;
            TMP_Dropdown _dropdown = ScoreFormat.GetComponent<TMP_Dropdown>();
            _dropdown.value = AppController.Instance.ListStudents.scoreFormat;
            _dropdown.onValueChanged.AddListener(delegate
            {
                AppController.Instance.ThresholdScore = _dropdown.value == 0 ? 3.0f : 60.0f;
                AppController.Instance.ListStudents.scoreFormat = _dropdown.value;
                ScoreFormatChanged(_dropdown.value);
            });


            /// <summary>
            /// Se referencia el botón "Verificar Clase" y se asigna el método que éste llamará cuando sea pulsado
            /// </summary>
            /// <returns></returns>
            GameObject VerifyClassButton = GameObject.Find("MainPanel/VerifyClassButton").gameObject;
            Button _verifyButton = VerifyClassButton.GetComponent<Button>();
            _verifyButton.onClick.AddListener(() => VerifyClass());
        }


        /// <summary>
        /// Para verficiar la clase se recorren cada uno de los elemntos que componen la tabla de estudiantes.
        /// Para ello se obtienen todos los objetos hijos del elemento contenedor de la tabla y se obtien el componente
        /// StudentTileGUI el cual almacena los datos de cada estudiante incluyendo la nota que se compara para verificar
        /// la clase. 
        /// </summary>
        private void VerifyClass()
        {
            float threshold;
            bool verified;
            verified = true;//local
            string Mssg;
            threshold = AppController.Instance.ThresholdScore;
            foreach (Transform child in ScrollViewContent.transform)
            {
                StudentTileGUI _tile = child.gameObject.GetComponent<StudentTileGUI>();
                float floatScore = float.Parse(_tile.Score);
                if ((floatScore < threshold && _tile.passCheck) || (floatScore >= threshold && !_tile.passCheck))
                {
                    _tile.ScoreColor = Color.red;
                    verified = false;//local
                }
                else
                    _tile.ScoreColor = Color.black;
            }
            if (verified)
            {
                Mssg = "Curso Verificado Correctamente";
                closeScene = true;
            }
            else
            {
                Mssg = "Error al verificar la clase. \nPor favor verifica las notas de los estudiantes que aparecen en rojo";
            }
            ShowAlertPanel(Mssg);
        }


        /// <summary>
        /// A través de esta función se evalúa cuál opción de formato de nota ha escogido el profesor y 
        /// se actualiza la propiedad "Score" del objeto de clase StudentTileGUI. Dicho objeto es componente 
        /// de cada Prefab instanciado a la hora de crear la lista de estutiantes que se visualizan en pantalla. 
        /// </summary>
        /// <param name="dropdownValue"></param>
        private void ScoreFormatChanged(int dropdownValue)
        {
            /// La nota, que en un principio está en formato de 0 a 5, es convertida a formato de 0 a 100
            if (dropdownValue == 1)
            {
                foreach (Transform child in ScrollViewContent.transform)
                {
                    StudentTileGUI _tile = child.gameObject.GetComponent<StudentTileGUI>();
                    _tile.Score = ConvertFormatToZeroHundred(_tile.Score);

                }
            }

            /// La nota, que está en formato de 0 a 100, es convertida a formato de 0 a 5
            else
            {
                foreach (Transform child in ScrollViewContent.transform)
                {
                    StudentTileGUI _tile = child.gameObject.GetComponent<StudentTileGUI>();
                    _tile.Score = ConvertFormatToZeroFive(_tile.Score);
                }
            }
        }


        /// <summary>
        /// Convierte _score de un string con valor de 0 - 5 a un string de 0 -100
        /// </summary>
        /// <param name="_score"></param>
        /// <returns></returns>
        private string ConvertFormatToZeroHundred(string _score)
        {
            float floatScore = float.Parse(_score);
            return ((int)(floatScore * 100 / 5)).ToString();
        }


        /// <summary>
        /// Convierte _score de un string con valor de 0 - 100 a un string de 0 - 5
        /// </summary>
        /// <param name="_score"></param>
        /// <returns></returns>
        private string ConvertFormatToZeroFive(string _score)
        {
            int intScore = int.Parse(_score);
            float _f = (float)intScore * 5 / 100;
            return _f.ToString("F2");
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
                ///TODO:Verificar si todos los campos están llenos y la calificación está en el valor que deb
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
                if (AppController.Instance.classVerified) //ok
                {
                    ShowAlertPanel(CourseClosedMessage);
                }
                else
                {
                    isModalToCreate = false;
                    ShowModalToEdit(_tile);
                }

            });
            /// <summary>
            /// Cuando se pulse el botón para eliminar se llama la función que destruye el objeto
            /// </summary>
            _tile.SetDeleteButtonOnClickAction(() =>
            {
                if (AppController.Instance.classVerified) //ok 
                {
                    ShowAlertPanel(CourseClosedMessage);
                }
                else
                {
                    Destroy(_tile.gameObject);
                }
            });
            /// <summary>
            /// Cuando se cambie el valor del checkbox de aprobación se verifica si la acción se debe ejcutar 
            /// o si se debe alertar al profesor que el curso ya fue calificado y cerrado. 
            /// </summary>
            _tile.SetPassCheckButtonValueChanged(() =>
            {
                if (AppController.Instance.classVerified)
                {
                    ShowAlertPanel(CourseClosedMessage);
                    _tile.SetPashCheckWithoutNotify(!_tile.passCheck);
                }
            });


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

        /// <summary>
        /// Se guardans los datos de los estudiantes 
        /// </summary>
        private void SaveDataScene()
        {
            int childCount = ScrollViewContent.transform.childCount;
            AppController.Instance.ListStudents.students = new Student[childCount];
            for (int i = 0; i < childCount; i++)
            {
                StudentTileGUI _tile = ScrollViewContent.transform.GetChild(i).gameObject.GetComponent<StudentTileGUI>(); ;
                AppController.Instance.ListStudents.students[i] = new Student
                {
                    name = _tile.Name,
                    surname = _tile.Surname,
                    code = _tile.Code,
                    email = _tile.Email,
                    score = _tile.Score
                };
            }
        }
    }
}
