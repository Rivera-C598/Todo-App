using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.CompilerServices;
using System.ComponentModel;
using System.Collections.ObjectModel;
using Xamarin.Forms;
using System.Threading.Tasks;

namespace CRUD_app
{
    public class TodoListViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private readonly DBConnection database;
        private ObservableCollection<TodoItem> todoItems;
        public ObservableCollection<TodoItem> TodoItems
        {
            get { return todoItems; }
            set
            {
                todoItems = value;
                OnPropertyChanged();
            }
        }

        private TodoItem selectedItem;
        public TodoItem SelectedItem
        {
            get { return selectedItem; }
            set
            {
                selectedItem = value;
                OnPropertyChanged();
            }
        }

        public Command AddTodoCommand { get; }
        public Command EditTodoCommand { get; }
        public Command DeleteTodoCommand { get; }
        public Command DeleteAllTodoCommand { get; }

        public TodoListViewModel()
        {
            database = new DBConnection(DependencyService.Get<IFileHelper>().GetLocalFilePath("database.db3"));
            TodoItems = new ObservableCollection<TodoItem>();
            AddTodoCommand = new Command(AddTodo);
            EditTodoCommand = new Command(EditTodo);
            DeleteTodoCommand = new Command(DeleteTodo);
            DeleteAllTodoCommand = new Command(DeleteAllTodo);
            LoadItems();
        }

        private async void LoadItems()
        {
            var items = await database.GetItemsAsync();
            TodoItems = new ObservableCollection<TodoItem>(items);
        }

        private void AddTodo()
        {
            TodoItem newItem = new TodoItem { Title = "New Todo", IsDone = false };
            TodoItems.Insert(0, newItem);
            SaveTodoItem(newItem);
            SelectedItem = newItem;
        }

        private async void EditTodo()
        {
            if (SelectedItem == null)
            {
                await Application.Current.MainPage.DisplayAlert("Error", "Please select an item.", "OK");
                return;
            }

            string newTitle = await Application.Current.MainPage.DisplayPromptAsync("Edit Todo", "Enter a new title for the Todo:", initialValue: SelectedItem.Title);
            if (newTitle != null)
            {
                SelectedItem.Title = newTitle;
                await SaveTodoItem(SelectedItem);

                // Update the TodoItems collection after editing
                var updatedItems = await database.GetItemsAsync();
                TodoItems = new ObservableCollection<TodoItem>(updatedItems);
            }
        }



        private void DeleteTodo()
        {
            if (SelectedItem != null)
            {
                TodoItems.Remove(SelectedItem);
                DeleteTodoItem(SelectedItem);
                SelectedItem = null;
            }
        }

        private async void DeleteAllTodo()
        {
            TodoItems.Clear();
            await database.DeleteAllItemsAsync();
        }
        
        private async void DeleteAllTodoItems()
        {
            await database.DeleteAllItemsAsync();
        }

        private async Task SaveTodoItem(TodoItem item)
        {
            await database.SaveItemAsync(item);
        }

        private async void DeleteTodoItem(TodoItem item)
        {
            await database.DeleteItemAsync(item);
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
