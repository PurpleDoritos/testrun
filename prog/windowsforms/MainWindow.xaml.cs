
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Win32;

/// <summary>
/// Represents the main window of the Task Manager application.
/// </summary>
namespace TaskManager
{
    public partial class TaskManager : Window
    {
        private TaskViewModel? _viewModel;

        /// <summary>
        /// Initializes a new instance of the <see cref="TaskManager"/> class.
        /// </summary>
        public TaskManager()
        {
         
            _viewModel = new TaskViewModel();
            DataContext = _viewModel;
        }

        /// <summary>
        /// Handles the Click event of the AddTaskButton control.
        /// </summary>
        private void AddTaskButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                TaskItem newTask = new TaskItem
                {
                    Title = TitleTextBox.Text,
                    Description = DescriptionTextBox.Text,
                    DueDate = DatePicker.SelectedDate ?? DateTime.Now,
                    IsCompleted = CompletedCheckBox.IsChecked ?? false,
                    Labels = LabelsTextBox.Text.Split(',').ToList(),
                    Priority = int.Parse(PriorityTextBox.Text),
                };

                _viewModel.AddTask(newTask);

                // Clear input fields
                TitleTextBox.Clear();
                DescriptionTextBox.Clear();
                DatePicker.SelectedDate = DateTime.Now;
                CompletedCheckBox.IsChecked = false;
                LabelsTextBox.Clear();
                PriorityTextBox.Clear();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error adding task: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Handles the Click event of the DeleteTaskButton control.
        /// </summary>
        private void DeleteTaskButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                TaskItem selectedTask = (TaskItem)TasksDataGrid.SelectedItem;
                if (selectedTask != null)
                {
                    _viewModel.DeleteTask(selectedTask);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error deleting task: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Handles the Click event of the SaveButton control.
        /// </summary>
        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Filter = "CSV Files (*.csv)|*.csv";

                if (saveFileDialog.ShowDialog() == true)
                {
                    string filePath = saveFileDialog.FileName;
                    _viewModel.SaveTasksToFile(filePath);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving tasks: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Handles the Click event of the LoadButton control.
        /// </summary>
        private void LoadButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.Filter = "CSV Files (*.csv)|*.csv";

                if (openFileDialog.ShowDialog() == true)
                {
                    string filePath = openFileDialog.FileName;
                    _viewModel.LoadTasksFromFile(filePath);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading tasks: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Handles the Click event of the ExportMarkdownButton control.
        /// </summary>
        private void ExportMarkdownButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog
                {
                    Filter = "Markdown Files (*.md)|*.md"
                };

                if (saveFileDialog.ShowDialog() == true)
                {
                    string filePath = saveFileDialog.FileName;

                    _viewModel.ExportTasksToMarkdown(filePath);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error exporting tasks: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Represents a task item in the Task Manager application.
        /// </summary>
        public class TaskItem
        {
            public string? Title { get; set; }
            public string? Description { get; set; }
            public DateTime DueDate { get; set; }
            public bool IsCompleted { get; set; }
            public List<string>? Labels { get; set; }
            public int Priority { get; set; }
        }

        /// <summary>
        /// Represents the view model for the Task Manager application.
        /// </summary>
        public class TaskViewModel
        {
            public ObservableCollection<TaskItem> Tasks { get; set; }

            /// <summary>
            /// Initializes a new instance of the <see cref="TaskViewModel"/> class.
            /// </summary>
            public TaskViewModel()
            {
                Tasks = new ObservableCollection<TaskItem>();
            }

            /// <summary>
            /// Adds a task to the collection of tasks.
            /// </summary>
            /// <param name="task">The task to add.</param>
            public void AddTask(TaskItem task)
            {
                Tasks.Add(task);
            }

            /// <summary>
            /// Deletes a task from the collection of tasks.
            /// </summary>
            /// <param name="task">The task to delete.</param>
            public void DeleteTask(TaskItem task)
            {
                Tasks.Remove(task);
            }

            /// <summary>
            /// Saves the tasks to a CSV file.
            /// </summary>
            /// <param name="filePath">The path of the file to save to.</param>
            public void SaveTasksToFile(string filePath)
            {
                using (StreamWriter writer = new StreamWriter(filePath))
                {
                    foreach (TaskItem task in Tasks)
                    {
                        writer.WriteLine($"{task.Title},{task.Description},{task.DueDate},{task.IsCompleted},{string.Join(",", task.Labels ?? new List<string>())},{task.Priority}");
                    }
                }
            }

            /// <summary>
            /// Loads tasks from a CSV file.
            /// </summary>
            /// <param name="filePath">The path of the file to load from.</param>
            public void LoadTasksFromFile(string filePath)
            {
                using (StreamReader reader = new StreamReader(filePath))
                {
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        string[] values = line.Split(',');
                        TaskItem task = new TaskItem
                        {
                            Title = values[0],
                            Description = values[1],
                            DueDate = DateTime.Parse(values[2]),
                            IsCompleted = bool.Parse(values[3]),
                            Labels = values.Length > 4 && values[4] != null ? values[4].Split(',').ToList() : new List<string>(),
                            Priority = int.Parse(values[5]),
                        };
                        AddTask(task);
                    }

                }
            }

            /// <summary>
            /// Exports the tasks to a Markdown file.
            /// </summary>
            /// <param name="filePath">The path of the file to export to.</param>
            public void ExportTasksToMarkdown(string filePath)
            {
                using (StreamWriter writer = new StreamWriter(filePath))
                {
                    foreach (TaskItem task in Tasks)
                    {
                        writer.WriteLine($"## {task.Title}");
                        writer.WriteLine();
                        writer.WriteLine($"{task.Description}");
                        writer.WriteLine();
                        writer.WriteLine($"**Due Date:** {task.DueDate.ToShortDateString()}");
                        writer.WriteLine();
                        writer.WriteLine($"**Priority:** {task.Priority}");
                        writer.WriteLine();
                        writer.WriteLine($"**Labels:** {string.Join(", ", task.Labels)}");
                        writer.WriteLine();
                        writer.WriteLine("---");
                        writer.WriteLine();
                    }
                }
            }
        }
    }
}




