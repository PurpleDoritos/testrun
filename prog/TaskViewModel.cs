public class TaskViewModel : INotifyPropertyChanged
{
    public ObservableCollection<TaskItem> Tasks { get; set; }
    public ObservableCollection<string> Categories { get; set; }  // Added for categories

    // Constructor
    public TaskViewModel()
    {
        Tasks = new ObservableCollection<TaskItem>();
        Categories = new ObservableCollection<string>();
    }

    // Implement INotifyPropertyChanged

    // Add methods to add, edit, delete tasks, add/remove categories, etc.

    public void AddTask(TaskItem task)
    {
        Tasks.Add(task);
    }

    public void EditTask(TaskItem task)
    {
        Tasks.edit(task);
    }

    public void DeleteTask(TaskItem task)
    {
        Tasks.Remove(task);
    }

    public void SaveTasksToFile(string filePath)
    {
        var options = new JsonSerializerOptions
        {
            WriteIndented = true
        };

        var json = JsonSerializer.Serialize(Tasks, options);

        File.WriteAllText(filePath, json);
    }
}
