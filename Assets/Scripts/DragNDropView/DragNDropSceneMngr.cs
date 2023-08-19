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


        // Start is called before the first frame update
        void Start()
        {
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

        private void VerifyOKButtonCallback()
        {
            VerifyPanel.SetActive(false);
            if (AppController.Instance.classVerified)
            {
                Debug.Log("Writing JSON...");
                AppController.Instance.WriteDataOnJsonFile();
                // SceneManager.LoadScene("DragNDropScene");
            }
        }

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

        void SetSidePanelInfo(Student _student)
        {
            StudentInfoPanel.transform.Find("Name/Background/Text").GetComponent<TMP_Text>().text = _student.name;
            StudentInfoPanel.transform.Find("Surname/Background/Text").GetComponent<TMP_Text>().text = _student.surname;
            StudentInfoPanel.transform.Find("Code/Background/Text").GetComponent<TMP_Text>().text = _student.code;
            StudentInfoPanel.transform.Find("Email/Background/Text").GetComponent<TMP_Text>().text = _student.email;
            StudentInfoPanel.transform.Find("Score/Background/Text").GetComponent<TMP_Text>().text = _student.score;
        }

        void CheckCategories()
        {

            string Mssg = "Felicitaciones.\nCurso calificado correctamente.";
            bool showCheck = false;
            bool showCheckApproved = false;
            bool showCheckNoApproved = false;
            AppController.Instance.classVerified = true;

            foreach (Transform child in ApprovedScrollView.transform.Find("Content").transform)
            {
                StudentPicMngr _studentInfoMngr = child.GetComponent<StudentPicMngr>();
                float floatScore = float.Parse(_studentInfoMngr.studentData.score);
                showCheck = floatScore < AppController.Instance.ThresholdScore;
                if (showCheck)
                    showCheckApproved = true;
                child.transform.Find("CheckImage").gameObject.SetActive(showCheck);
            }
            foreach (Transform child in NoApprovedScrollView.transform.Find("Content").transform)
            {
                StudentPicMngr _studentInfoMngr = child.GetComponent<StudentPicMngr>();
                float floatScore = float.Parse(_studentInfoMngr.studentData.score);
                showCheck = floatScore >= AppController.Instance.ThresholdScore;
                if (showCheck)
                    showCheckNoApproved = true;
                child.transform.Find("CheckImage").gameObject.SetActive(showCheck);
            }
            if (showCheckApproved || showCheckNoApproved)
            {
                Mssg = "Error. Algunos estudiantes han sido clasificados incorrectamente. Intenta de nuevo";
                AppController.Instance.classVerified = false;
            }
            VerifyPanel.transform.Find("Modal/Message").GetComponent<TMP_Text>().text = Mssg;
            VerifyPanel.SetActive(true);

        }
    }
}
