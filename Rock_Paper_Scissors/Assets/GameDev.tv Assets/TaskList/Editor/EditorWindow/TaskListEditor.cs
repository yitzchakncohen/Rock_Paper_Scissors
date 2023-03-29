using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using System;
using System.Collections.Generic;

namespace GameDevtTV.Tasks
{
    public class TaskListEditor : EditorWindow 
    {
        public const string PATH = "Assets/GameDev.tv Assets/TaskList/Editor/EditorWindow/";
        private VisualElement container;
        private ObjectField savedTasksObjectField;
        private Button loadTasksButton;
        private TextField taskText;
        private Button addTaskButton;
        private ScrollView taskListScrollView;
        private TaskListSO taskListSO;
        private Button saveProgressButton;
        private ProgressBar taskProgressBar;
        private ToolbarSearchField searchBox;
        private Label notificationLabel;

        [MenuItem("GameDev.tv/Task List")]
        public static void ShowWindow() 
        {
            TaskListEditor window = GetWindow<TaskListEditor>();
            window.titleContent = new GUIContent("Task List");
            window.Show();
        }

        public void CreateGUI()
        {
            container = rootVisualElement;
            VisualTreeAsset visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(PATH + "TaskListEditor.uxml");
            container.Add(visualTree.Instantiate());

            StyleSheet styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>(PATH + "TaskListEditor.uss");
            container.styleSheets.Add(styleSheet);

            taskText = container.Q<TextField>("taskText");
            taskText.RegisterCallback<KeyDownEvent>(AddTask);

            addTaskButton = container.Q<Button>("addTaskButton");
            addTaskButton.clicked += AddTask;

            taskListScrollView = container.Q<ScrollView>("taskList");

            savedTasksObjectField = container.Q<ObjectField>("savedTasksObjectField");
            savedTasksObjectField.objectType = typeof(TaskListSO);

            loadTasksButton = container.Q<Button>("loadTasksButton");
            loadTasksButton.clicked += LoadTasks;

            saveProgressButton = container.Q<Button>("saveProgressButton");
            saveProgressButton.clicked += SaveProgress;

            taskProgressBar = container.Q<ProgressBar>("taskProgressBar");

            searchBox = container.Q<ToolbarSearchField>("searchBox");
            searchBox.RegisterValueChangedCallback(OnSearchTextChange);

            notificationLabel = container.Q<Label>("notificationLabel");
        }

        private void AddTask()
        {
            if(!string.IsNullOrEmpty(taskText.value))
            {
                Task taskObject = new Task(taskText.text, false);
                TaskItem taskItem = CreateTask(taskObject);
                taskListScrollView.Add(taskItem);
                taskItem.GetRemoveButton().RegisterCallback<MouseUpEvent>(OnRemoveButtonPressed);
                SaveTask(taskObject);
                taskText.value = "";
                taskText.Focus();
                UpdateProgress();
                UpdatenNotifications("Task added succesfully!");
            }
            else
            {
                UpdatenNotifications("No text entered for task.");
            }
        }

        private void AddTask(KeyDownEvent e)
        {
            if(Event.current.Equals(Event.KeyboardEvent("Return")))
            {
                AddTask();
            }
        }

        private void LoadTasks()
        {
            taskListSO = savedTasksObjectField.value as TaskListSO;
            if(taskListSO != null)
            {
                taskListScrollView.Clear();
                List<Task> tasks = taskListSO.GetTasks();
                
            
                foreach (Task task in tasks)
                {
                    TaskItem taskItem = CreateTask(task);
                    taskItem.GetRemoveButton().RegisterCallback<MouseUpEvent>(OnRemoveButtonPressed);
                    taskListScrollView.Add(taskItem);
                    if(task.Completed)
                    {
                        foreach (VisualElement child in taskItem.Children())
                        {
                            child.AddToClassList("completed");                            
                        }
                    } 
                    else
                    {
                        foreach (VisualElement child in taskItem.Children())
                        {
                            child.RemoveFromClassList("completed");                          
                        }
                    }
                }

                UpdateProgress();
                UpdatenNotifications("Task list loaded.");
            }
            else
            {
                UpdatenNotifications("Failed to load task list.");
            }

            SortTaskList();
        }

        private TaskItem CreateTask(Task task)
        {
            TaskItem taskItem = new TaskItem(task.Text);
            taskItem.GetTaskToggle().RegisterValueChangedCallback(OnToggleValueChanged);
            taskItem.GetTaskToggle().value = task.Completed;
            return taskItem;
        }

        private void SaveTask(Task task)
        {
            taskListSO.AddTask(task);
            EditorUtility.SetDirty(taskListSO);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        private void SaveProgress()
        {
            if(taskListSO !=null)
            {
                List<Task> tasks = new List<Task>();

                foreach (TaskItem task in taskListScrollView.Children())
                {
                    Task taskObject = new Task(task.GetTaskLabel().text, task.GetTaskToggle().value);
                    tasks.Add(taskObject);
                }

                taskListSO.AddTasks(tasks);
                EditorUtility.SetDirty(taskListSO);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
                LoadTasks();

                UpdatenNotifications("Progress saved.");
            }
        }

        private void UpdateProgress()
        {
            int completedTasks = 0;
            foreach (TaskItem task in taskListScrollView.Children())
            {
                if(task.GetTaskToggle().value)
                {
                    completedTasks++;
                }
            }

            if(taskListScrollView.childCount > 0)
            {
                taskProgressBar.value = (float)completedTasks/taskListScrollView.childCount;
            }
            else
            {
                taskProgressBar.value = 1;
            }
            taskProgressBar.title = (taskProgressBar.value*100).ToString("F1") + " %";
        }

        private void OnToggleValueChanged(ChangeEvent<bool> changeEvent)
        {
            Toggle changedToggled = changeEvent.currentTarget as Toggle;
            VisualElement taskItem = changedToggled.parent.parent as VisualElement;
            if(changeEvent.newValue)
            {
                taskItem.AddToClassList("completed");
            } 
            else
            {
                taskItem.RemoveFromClassList("completed");
            }
            UpdateProgress();
            SortTaskList();
        }

        private void OnSearchTextChange(ChangeEvent<string> changeEvent)
        {
            string searchText = changeEvent.newValue.ToUpper();

            foreach (TaskItem task in taskListScrollView.Children())
            {
                string taskText = task.GetTaskLabel().text.ToUpper();

                if(!string.IsNullOrEmpty(searchText) && taskText.Contains(searchText))
                {
                    task.GetTaskLabel().AddToClassList("highlight");
                }
                else
                {
                    task.GetTaskLabel().RemoveFromClassList("highlight");
                }
            }

            //TODO Sort task list to only show search results.
        }

        private void UpdatenNotifications(string notification)
        {
            if(!string.IsNullOrEmpty(notification))
            {
                notificationLabel.text = notification;
            }
        }

        private void OnRemoveButtonPressed(MouseUpEvent mouseUpEvent)
        {
            Button pressedButton = mouseUpEvent.currentTarget as Button;
            if(pressedButton != null)
            {
                TaskItem taskItem = pressedButton.parent.parent.parent as TaskItem;
                if(taskItem != null)
                {
                    taskListScrollView.Remove(taskItem);
                }
            }
        }

        private void SortTaskList()
        {
            List<TaskItem> taskItems = new List<TaskItem>();
            foreach (TaskItem task in taskListScrollView.Children())
            {
                taskItems.Add(task);
            }
            taskListScrollView.Clear();
            
            taskItems.Sort(SortTasksByCompleted);

            foreach (TaskItem task in taskItems)
            {
                taskListScrollView.Add(task);
            }
        }

        private static int SortTasksByCompleted(TaskItem taskItemA, TaskItem taskItemB)
        {
            if(taskItemA.GetTaskToggle().value)
            {
                return 1;
            }
            else if(!taskItemA.GetTaskToggle().value && !taskItemB.GetTaskToggle().value)
            {
                return 1;
            }
            return -1;
        }
    }
}