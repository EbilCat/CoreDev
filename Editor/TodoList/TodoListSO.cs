using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEditor;
using UnityEngine;


namespace CoreDev.CustomEditors.TodoList
{
    [CreateAssetMenu(fileName = "TodoList", menuName = "ScriptableObjects/TodoList")]
    public class TodoListSO : ScriptableObject
    {
        [SerializeField]
        private List<Todo> todos = new List<Todo>();
        private event Action<Todo> todoAdded;
        private event Action<Todo> todoRemoving;


//*====================
//* CALLBACK REGISTRATION
//*====================
        public void RegisterForTodoAdded(Action<Todo> callback)
        {
            todoAdded -= callback;
            todoAdded += callback;

            foreach (Todo todo in todos)
            {
                callback(todo);
            }
        }

        public void RegisterForTodoRemoved(Action<Todo> callback)
        {
            todoRemoving -= callback;
            todoRemoving += callback;
        }

        public void UnregisterFromTodoAdded(Action<Todo> callback)
        {
            todoAdded -= callback;
        }

        public void UnregisterFromTodoRemoving(Action<Todo> callback)
        {
            todoRemoving -= callback;

            foreach (Todo todo in todos)
            {
                callback(todo);
            }
        }


//*====================
//* PUBLIC
//*====================
        public void AddTodo(string todoString)
        {
            Todo todo = new Todo(todos.Count, todoString);
            this.todos.Add(todo);
            this.todoAdded?.Invoke(todo);
            this.SaveTodosToDisk();
        }
        public void RemoveTodo(Todo todo)
        {
            if (todos.Contains(todo))
            {
                this.todoRemoving?.Invoke(todo);
                todos.Remove(todo);
                this.SaveTodosToDisk();
            }
        }

        public ReadOnlyCollection<Todo> GetTodos()
        {
            ReadOnlyCollection<Todo> readOnlyTodoList = this.todos.AsReadOnly();
            return readOnlyTodoList;
        }
        
        public void SaveTodosToDisk()
        {
            EditorUtility.SetDirty(this);
            AssetDatabase.SaveAssetIfDirty(this);
        }
    }


    [Serializable]
    public class Todo
    {
        public TodoVisualElement todoVisualElement;
        public string todoString;
        public bool isDone;

        public Todo(int index, string todoString, bool isDone = false)
        {
            this.todoString = todoString;
            this.isDone = isDone;
        }
    }
}