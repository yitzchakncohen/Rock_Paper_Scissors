using System.Collections.Generic;
using UnityEngine;

namespace GameDevtTV.Tasks
{
    [CreateAssetMenu(fileName = "New Task List", menuName = "GameDev.tv/Task List", order = 0)]
    public class TaskListSO : ScriptableObject 
    {
        [SerializeField] private List<Task> tasks;

        public List<Task> GetTasks()
        {
            return tasks;
        }

        public void AddTasks(List<Task> savedTasks)
        {
            tasks.Clear();
            this.tasks = savedTasks;
        }

        public void AddTask(Task savedTask)
        {
            if(!tasks.Contains(savedTask))
            {
                tasks.Add(savedTask);
            }
        }
    }
}
