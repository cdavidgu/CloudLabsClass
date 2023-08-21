using UnityEngine;
using Models;
using UnityEngine.UI;
using Controllers;
using System;
using TMPro;
using UnityEngine.SceneManagement;




namespace GUI
{
    public class DragNDropSceneMngr : MonoBehaviour
    {

        [SerializeField]
        GameObject StudentPicPrefab;
        GameObject VerifyPanel;
        GameObject StudentInfoPanel;
        GameObject ApprovedScrollView;
        GameObject NoApprovedScrollView;
        bool classVerified;


        // Start is called before the first frame update
        void Start()
        {
            /// <summary>
            /// Se inicializan todos los componentes gráficos de la escena y se definen las acciones de los botones y demás
            /// elementos con los que interactúa e usuario.  
            /// /// </summary>
            InitSidePanel();
            VerifyPanel = GameObject.Find("VerifyMssgPanel");
            VerifyPanel.transform.Find("Modal/OKButton").GetComponent<Button>().onClick.AddListener(() => VerifyOKButtonCallback());
            VerifyPanel.SetActive(false);
            StudentInfoPanel = GameObject.Find("MainPanel/StudentInfoPanel");
            ApprovedScrollView = GameObject.Find("MainPanel/ApprovedScrollView/Viewport");
            CategoryDropHandler ApprovedCategoryHandler = ApprovedScrollView.GetComponent<CategoryDropHandler>();
            ApprovedCategoryHandler.SetEmptySidePanelCallback(CheckCategories);
            NoApprovedScrollView = GameObject.Find("MainPanel/NoApprovedScrollView/Viewport");
            CategoryDropHandler NoApprovedCategoryHandler = NoApprovedScrollView.GetComponent<CategoryDropHandler>();
            NoApprovedCategoryHandler.SetEmptySidePanelCallback(CheckCategories);

        }

        /// <summary>
        /// Método que se llama cuando el botón OK del panel de alerta de verificación.
        /// Se oculta el panel de alerta y en caso de que todos los estudiantes estén en la categoría que le corresponde 
        /// (a través de la cariavle "classVerified") se guardan los datos de los estudiantes en el archivo JSON y se carga
        /// de nuevo la escena principal. 
        /// La variable AppController.Instance.classVerified es utilizada en la escena principal para validar si el profesor 
        /// puede editar o no la información de los alumnos dependiento si el curso ya fue calificado y cerrado. 
        /// </summary>
        private void VerifyOKButtonCallback()
        {
            VerifyPanel.SetActive(false);
            if (classVerified)
            {
                AppController.Instance.classVerified = true;
                AppController.Instance.WriteDataOnJsonFile();
                SceneManager.LoadScene("MainScene");
            }
        }


        /// <summary>
        /// Se inicializa el panel lateral con la imagen y nombre de los estudiantes. 
        /// Los datos de cada estudiante se obtienen de la variable sel singleton AppController.Instance.ListStudents.students
        /// y son guardados en un objeto de clase StudentPicMngr. 
        /// </summary>
        private void InitSidePanel()
        {
            GameObject ScrollViewContent = GameObject.Find("MainPanel/SidePanel/ScrollView/Viewport/Content");
            foreach (Student student in AppController.Instance.ListStudents.students)
            {
                GameObject _studentPic = Instantiate(StudentPicPrefab, ScrollViewContent.transform);
                _studentPic.transform.Find("Name").GetComponent<TMP_Text>().text = student.name;
                _studentPic.transform.Find("CheckImage").gameObject.SetActive(false);
                StudentPicMngr _studentInfoMngr = _studentPic.GetComponent<StudentPicMngr>();
                _studentInfoMngr.studentData.name = student.name;
                _studentInfoMngr.studentData.surname = student.surname;
                _studentInfoMngr.studentData.code = student.code;
                _studentInfoMngr.studentData.score = student.score;
                _studentInfoMngr.studentData.email = student.email;
                _studentInfoMngr.SetOnClickDownCallback(() => SetSidePanelInfo(student));
            }
        }

        /// <summary>
        /// Método llamado cuando se selecciona un estudiante. La información del estudiante es visualizada
        /// en el panel lateral de información
        /// </summary>
        /// <param name="_student"></param>
        void SetSidePanelInfo(Student _student)
        {
            StudentInfoPanel.transform.Find("Name/Background/Text").GetComponent<TMP_Text>().text = _student.name;
            StudentInfoPanel.transform.Find("Surname/Background/Text").GetComponent<TMP_Text>().text = _student.surname;
            StudentInfoPanel.transform.Find("Code/Background/Text").GetComponent<TMP_Text>().text = _student.code;
            StudentInfoPanel.transform.Find("Email/Background/Text").GetComponent<TMP_Text>().text = _student.email;
            StudentInfoPanel.transform.Find("Score/Background/Text").GetComponent<TMP_Text>().text = _student.score;
        }


        /// <summary>
        /// Método que es llamado cuando el usuario ya ha arrastrado todos los elementos (estudiantes) del panel lateral 
        /// (zona neutra) a los páneles de las categorías AProbado y Reprobado.
        /// Las categorías son ScrollView en donde se ordenan los elementos arrastrados. Al recorrer todos los hijos de 
        /// los scrollview se obtiene la informaciónm del puntaje de cada estudiante y se coteja con la categoría en 
        /// la que quedó asignado, 
        /// </summary>
        void CheckCategories()
        {

            string Mssg = "Felicitaciones.\nCurso calificado correctamente.";
            bool showErrorMark = false;
            bool errorInApprovedCategory = false;
            bool errorInNoApprovedCategory = false;
            classVerified = true;

            foreach (Transform child in ApprovedScrollView.transform.Find("Content").transform)
            {
                float floatScore = GetStudentScore(child);
                showErrorMark = floatScore < AppController.Instance.ThresholdScore;
                if (showErrorMark)
                    errorInApprovedCategory = true;
                child.transform.Find("CheckImage").gameObject.SetActive(showErrorMark);
            }
            foreach (Transform child in NoApprovedScrollView.transform.Find("Content").transform)
            {
                float floatScore = GetStudentScore(child);
                showErrorMark = floatScore >= AppController.Instance.ThresholdScore;
                if (showErrorMark)
                    errorInNoApprovedCategory = true;
                child.transform.Find("CheckImage").gameObject.SetActive(showErrorMark);
            }
            if (errorInApprovedCategory || errorInNoApprovedCategory)
            {
                Mssg = "Error. Algunos estudiantes han sido clasificados incorrectamente. Intenta de nuevo";
                classVerified = false;
            }
            VerifyPanel.transform.Find("Modal/Message").GetComponent<TMP_Text>().text = Mssg;
            VerifyPanel.SetActive(true);
        }


        /// <summary>
        /// Método que convierte el puntaje del estudiante de texto a flotante
        /// </summary>
        /// <param name="student"></param>
        /// <returns></returns>
        float GetStudentScore(Transform student)
        {
            StudentPicMngr _studentInfoMngr = student.GetComponent<StudentPicMngr>();
            return float.Parse(_studentInfoMngr.studentData.score);
        }
    }
}
