using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using UnityEngine;
using System;

namespace CoreDev.CustomEditors.TodoList
{
    public class TodoListEditor : EditorWindow
    {
        private VisualElement root;
        private ToolbarSearchField searchTextField;
        private TextField todoTextField;
        private Button addTodoButton;
        private ScrollView todoListScrollView;
        private ObjectField savedTodosObjectField;
        private TodoListSO todoListSO;
        private Button newTodoListButton;

        [MenuItem("CoreDevUtils/Todos")]
        public static void ShowWindow()
        {
            TodoListEditor todoListEditor = GetWindow<TodoListEditor>();
            todoListEditor.minSize = Vector2.one * 400;
            todoListEditor.titleContent.text = "ToDo List";
        }


//*====================
//* UNITY
//*====================
        private void CreateGUI()
        {
            VisualTreeAsset visualTreeAsset = AssetDatabaseHelper.LoadAsset<VisualTreeAsset>("TodoListEditor");
            if (visualTreeAsset != null)
            {
                this.root = rootVisualElement;
                TemplateContainer templateContainer = visualTreeAsset.Instantiate();
                templateContainer.style.flexGrow = 1;
                this.root.Add(templateContainer);

                StyleSheet styleSheet = AssetDatabaseHelper.LoadAsset<StyleSheet>("TodoListEditor");
                this.root.styleSheets.Add(styleSheet);

                this.searchTextField = this.root.Q<ToolbarSearchField>("searchTextField");
                this.todoTextField = this.root.Q<TextField>("todoText");
                this.addTodoButton = this.root.Q<Button>("addTodoButton");
                this.todoListScrollView = this.root.Q<ScrollView>("todoList");
                this.savedTodosObjectField = this.root.Q<ObjectField>("savedTodosObjectField");
                this.savedTodosObjectField.objectType = typeof(TodoListSO);
                this.newTodoListButton = this.root.Q<Button>("newTodoList");

                this.searchTextField.RegisterValueChangedCallback(OnSearchTextFieldValueChanged);
                this.todoTextField.RegisterCallback<KeyDownEvent>(OnKeyDownOnTodoField);
                this.addTodoButton.clicked += OnAddTodoButtonClicked;
                this.savedTodosObjectField.RegisterValueChangedCallback(OnSavedTodosObjectFieldChanged);
                this.newTodoListButton.clicked += OnNewTodoListButtonClicked;
            }
            else
            {
                Debug.LogError("Error loading/locating TodoListEditor.uxml");
            }
        }

        private void OnDestroy()
        {
            this.searchTextField.UnregisterValueChangedCallback(OnSearchTextFieldValueChanged);
            this.todoTextField.UnregisterCallback<KeyDownEvent>(OnKeyDownOnTodoField);
            this.addTodoButton.clicked -= OnAddTodoButtonClicked;
            this.savedTodosObjectField.UnregisterValueChangedCallback(OnSavedTodosObjectFieldChanged);
            this.newTodoListButton.clicked -= OnNewTodoListButtonClicked;

            foreach (TodoVisualElement todoVisualElement in todoListScrollView.Children())
            {
                todoVisualElement.Dispose();
            }
            todoListScrollView.Clear();
        }


//*====================
//* UI - CALLBACKS
//*====================
        private void OnNewTodoListButtonClicked()
        {
            string path = EditorUtility.SaveFilePanel("Save New Todo list", "Assets/", "TodoList", "asset");
            if (string.IsNullOrEmpty(path)) return;

            path = FileUtil.GetProjectRelativePath(path);

            TodoListSO todoListSO = ScriptableObject.CreateInstance<TodoListSO>();
            AssetDatabase.CreateAsset(todoListSO, path);
            AssetDatabase.SaveAssets();

            this.savedTodosObjectField.value = todoListSO;
        }


        private void OnSearchTextFieldValueChanged(ChangeEvent<string> evt)
        {
            foreach (TodoVisualElement todoVisualElement in todoListScrollView.Children())
            {
                bool isVisible = todoVisualElement.todoLabel.text.Contains(evt.newValue, StringComparison.CurrentCultureIgnoreCase);
                todoVisualElement.style.display = (isVisible) ? DisplayStyle.Flex : DisplayStyle.None;
            }
        }

        private void OnKeyDownOnTodoField(KeyDownEvent evt)
        {
            if (Event.current.Equals(Event.KeyboardEvent("Return")))
            {
                this.OnAddTodoButtonClicked();
            }
        }

        private void OnSavedTodosObjectFieldChanged(ChangeEvent<UnityEngine.Object> evt)
        {
            if (this.todoListSO != null)
            {
                this.todoListSO.UnregisterFromTodoAdded(OnTodoAdded);
                this.todoListSO.UnregisterFromTodoRemoving(OnTodoRemoving);
            }

            this.todoListSO = evt.newValue as TodoListSO;
            this.todoListSO?.RegisterForTodoAdded(OnTodoAdded);
            this.todoListSO?.RegisterForTodoRemoved(OnTodoRemoving);
        }

        private void OnAddTodoButtonClicked()
        {
            if (todoListSO != null)
            {
                this.todoListSO.AddTodo(this.todoTextField.value);
                this.todoTextField.value = "";
                this.todoTextField.Focus();
            }
        }


//*====================
//* TodoListSO - CALLBACKS
//*====================
        private void OnTodoAdded(Todo todo)
        {
            TodoVisualElement todoVisualElement = new TodoVisualElement(this.todoListSO, todo);
            todo.todoVisualElement = todoVisualElement;
            this.todoListScrollView.Add(todoVisualElement);
        }

        private void OnTodoRemoving(Todo todo)
        {
            TodoVisualElement todoVisualElemnt = todo?.todoVisualElement;

            if (todoVisualElemnt != null)
            {
                this.todoListScrollView.Remove(todo.todoVisualElement);
            }
        }
    }
}