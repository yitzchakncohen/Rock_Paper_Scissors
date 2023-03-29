using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameDevtTV.Tasks
{
    [System.Serializable]
    public class Task
    {
        public Task(string text, bool completed)
        {
            Text = text;
            Completed = completed;
        }

        public string Text;

        public bool Completed; 
    }

}
