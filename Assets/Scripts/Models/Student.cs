using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Models
{
    [System.Serializable]
    public class ListStudents
    {
        public int scoreFormat;
        public Student[] students;
    }
    [System.Serializable]
    public class Student
    {
        public string name;
        public string surname;
        public string code;
        public string email;
        public string score;
    }
}