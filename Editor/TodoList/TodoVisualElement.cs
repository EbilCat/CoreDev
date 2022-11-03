using UnityEngine;
using UnityEngine.UIElements;

namespace CoreDev.CustomEditors.TodoList
{
    public class TodoVisualElement : VisualElement
    {
        private TodoListSO todoListSO;
        private Todo todo;
        public Toggle todoToggle;
        public Label todoLabel;
        private Button removeButton;

        public TodoVisualElement(TodoListSO todoListSO, Todo todo)
        {
            this.todoListSO = todoListSO;
            this.todo = todo;

            VisualTreeAsset visualTreeAsset = AssetDatabaseHelper.LoadAsset<VisualTreeAsset>("TodoVisualElement");
            if (visualTreeAsset != null)
            {
                this.Add(visualTreeAsset.Instantiate());

                this.todoToggle = this.Q<Toggle>("todoToggle");
                this.todoLabel = this.Q<Label>("todoLabel");
                this.removeButton = this.Q<Button>("removeTodoButton");
                this.todoLabel.text = todo.todoString;
                this.todoToggle.value = todo.isDone;

                todoToggle.RegisterValueChangedCallback(OnToggleValueChanged);
                removeButton.clicked += OnRemoveButtonClicked;
            }
            else
            {
                Debug.LogError("Error loading/locating TodoVisualElement.uxml");
            }
        }

        public void Dispose()
        {
            todoToggle.UnregisterValueChangedCallback(OnToggleValueChanged);
            removeButton.clicked -= OnRemoveButtonClicked;
        }

        private void OnToggleValueChanged(ChangeEvent<bool> evt)
        {
            this.todo.isDone = evt.newValue;
            todoListSO.SaveTodosToDisk();
        }

        private void OnRemoveButtonClicked()
        {
            int index = this.todoListSO.GetTodos().IndexOf(todo);
            todoListSO.RemoveTodo(todo);
        }
    }
}