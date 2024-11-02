using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Avalonia.Controls;
using Avalonia.Interactivity;

namespace SurveyCreator;

public partial class MainWindow : Window
{
    public List<string> questions;
    public List<string> filteredQuestions;
    public MainWindow()
    {
        InitializeComponent();
        questions = new List<string>();
        filteredQuestions = new List<string>();
    }
    
    private void AddQuestion_OnClick(object? sender, RoutedEventArgs e)
    {
        var questionText = QuestionTitle.Text;
        var category = Personal.IsChecked == true ? "Osobiste" : "Ogólne";
        var answerType = Single.IsChecked == true ? "Pojedynczy" : "Wielokrotny";
        var options = new List<string>();
        
        if (Option1.IsChecked == true) options.Add(Option1.Content.ToString());
        if (Option2.IsChecked == true) options.Add(Option2.Content.ToString());
        if (Option3.IsChecked == true) options.Add(Option3.Content.ToString());
        
        if (string.IsNullOrWhiteSpace(questionText) || !options.Any())
        {
            var messageBox = new Window
            {
                Title = "Błąd",
                Content = new TextBlock { Text = "Wypełnij wszystkie potrzebne informacje" }
            };
            messageBox.ShowDialog(this);
            return;
        }
        
        var formattedQuestion = $"{questionText}, Kategoria: {category} (Typ: {answerType}), odpowiedzi: {string.Join(", ", options)}";
        questions.Add(formattedQuestion);
        
        QuestionListBox.ItemsSource = questions.ToList();
        ClearInputs();
    }
    
    private void ClearInputs()
    {
        QuestionTitle.Text = string.Empty;
        Common.IsChecked = false;
        Personal.IsChecked = false;
        Single.IsChecked = false;
        Multiple.IsChecked = false;
        Option1.IsChecked = false;
        Option2.IsChecked = false;
        Option3.IsChecked = false;
        QuestionComboBox.SelectedIndex = -1;
    }

    private void SaveToFileButton_OnClick(object? sender, RoutedEventArgs e)
    {
        if (questions.Count == 0)
        {
            var messageBox = new Window
            {
                Title = "Błąd",
                Content = new TextBlock { Text = "Nie ma żadnych pytań do wpisania." }
            };
            messageBox.ShowDialog(this);
            return;
        }

        var sb = new StringBuilder();
        foreach (var question in questions)
        {
            sb.AppendLine(question);
        }

        try
        {
            var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "survey.txt");
            
            File.WriteAllText(filePath, sb.ToString());

            var messageBoxSuccess = new Window
            {
                Title = "Informacja",
                Content = new TextBlock { Text = $"Pomyslnie zapisano do {filePath}" }
            };
            messageBoxSuccess.ShowDialog(this);
        }
        catch (Exception ex)
        {
            var messageBoxError = new Window
            {
                Title = "Błąd",
                Content = new TextBlock { Text = ex.Message}
            };
            messageBoxError.ShowDialog(this);
        }
        
    }

    private void QuestionListBox_OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (QuestionListBox.SelectedItem is string selectedQuestion)
        {
            questions.Remove(selectedQuestion);
            UpdateQuestionListBox();
        }
    }

    private void QuestionComboBox_OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        UpdateQuestionListBox();
    }

    private void UpdateQuestionListBox()
    {
        var selectedCategory = (QuestionComboBox.SelectedItem as ComboBoxItem)?.Content.ToString();

        filteredQuestions = string.IsNullOrWhiteSpace(selectedCategory)
            ? questions
            : questions.Where(q => q.Contains($"Kategoria: {selectedCategory}")).ToList();
        QuestionListBox.ItemsSource = filteredQuestions;
    }
}