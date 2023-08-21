using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Models;
using GUI;
using System.IO;


namespace Controllers
{
    public sealed class AppController : MonoBehaviour
    {
        private static AppController _instance;

        // Variable que almacena los datos de los estudiantes que se leen desde el archivo JSON 
        [HideInInspector] public ListStudents ListStudents;

        /// variable que permite a la aplicación conocer cuál es el estudiante seleccionado por el profesor
        [HideInInspector] public StudentTileGUI SelectedStudentTile;
        public bool classVerified = false;
        [HideInInspector] public float ThresholdScore;
        const string jsonPath = "Assets/StreamingAssets/students.json";


        private AppController() //
        {

        }

        /// <summary>
        /// Se emplea el patron Singleton para almacenar y preservar de manera estática los datos de 
        /// los estudiantes y demas datos relevantes entre las escenas de la aplicación.
        /// </summary>
        /// <returns></returns>
        public static AppController Instance
        {
            get { return _instance; }
        }

        private void Awake()
        {
            if (_instance == null)
            {
                _instance = this;
                DontDestroyOnLoad(this);
                LoadStudents();

            }
            else
            {
                Destroy(this.gameObject);
            }
        }

        /// <summary>
        /// Se cargan los datos de los estudiantes guardados en el archivo JSON y son almacenados en una 
        /// lista de objetos de tipo Student
        /// </summary>
        private void LoadStudents()
        {
            string jsonText = File.ReadAllText(jsonPath);
            ListStudents = JsonUtility.FromJson<ListStudents>(jsonText);
            print(ListStudents.scoreFormat);
            ThresholdScore = ListStudents.scoreFormat == 0 ? 3.0f : 60.0f;
        }

        // Start is called before the first frame update
        void Start()
        {

        }

        public void WriteDataOnJsonFile()
        {
            string jsonText = JsonUtility.ToJson(ListStudents, true);
            File.WriteAllText(jsonPath, jsonText);
            Debug.Log("Saving Student Data to JSON file.");
        }
    }
}