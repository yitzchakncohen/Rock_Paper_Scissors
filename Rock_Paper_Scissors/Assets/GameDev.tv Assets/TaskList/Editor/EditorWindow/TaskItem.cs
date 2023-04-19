using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

namespace GameDevtTV.Tasks
{
    [System.Serializable]
    public class TaskItem : VisualElement
    {
        private Toggle taskToggle;
        private Label taskLabel;
        private Button removeButton;

        public TaskItem(string taskText)
        {
            VisualTreeAsset original = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(TaskListEditor.PATH + "TaskItem.uxml");
            this.Add(original.Instantiate());

            taskToggle = this.Q<Toggle>("taskToggle");

            taskLabel = this.Q<Label>("taskLabel"); 
            taskLabel.text = taskText;

            removeButton = this.Q<Button>("removeButton");
        }

        public Toggle GetTaskToggle()
        {
            return taskToggle;
        }

        public Label GetTaskLabel()
        {
            return taskLabel;
        }
        
        public Button GetRemoveButton()
        {
            return removeButton;
        }
    }
}
