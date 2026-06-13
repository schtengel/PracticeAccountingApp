using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;
using Microsoft.Win32;
using PracticeAccountingApp.Models;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Windows;

namespace PracticeAccountingApp.ViewModels;

public partial class StudentReportEditViewModel : BaseViewModel
{
    private readonly int? _id;

    [ObservableProperty] private Student? selectedStudent;
    [ObservableProperty] private PracticeSheet? selectedPracticeSheet;
    [ObservableProperty] private string filePath = "";
    [ObservableProperty] private string gradeString = "";
    [ObservableProperty] private string submissionDateString = "";

    [ObservableProperty]
    private ObservableCollection<Student> students = new();

    [ObservableProperty]
    private ObservableCollection<PracticeSheet> practiceSheets = new();

    // Список допустимых оценок по российской шкале
    public IReadOnlyList<string> GradeOptions { get; } =
        new[] { "", "2", "3", "4", "5" };

    public StudentReportEditViewModel(int? id)
    {
        _id = id;

        foreach (var s in Db.Context.Students.OrderBy(s => s.FullName))
            Students.Add(s);

        foreach (var ps in Db.Context.PracticeSheets
                     .Include(p => p.PracticeType)
                     .Include(p => p.GroupNumberNavigation)
                     .OrderByDescending(p => p.PracticeSheetId))
            PracticeSheets.Add(ps);

        if (id != null)
        {
            var report = Db.Context.StudentReports
                .Include(r => r.Student)
                .Include(r => r.PracticeSheet)
                .First(r => r.ReportId == id);

            SelectedStudent = Students.FirstOrDefault(s => s.StudentId == report.StudentId);
            SelectedPracticeSheet = PracticeSheets.FirstOrDefault(ps => ps.PracticeSheetId == report.PracticeSheetId);
            FilePath = report.FilePath;
            GradeString = report.Grade?.ToString() ?? "";
            SubmissionDateString = report.SubmissionDate?.ToString("dd.MM.yyyy") ?? "";
        }
    }

    // Открыть диалог выбора файла
    [RelayCommand]
    private void BrowseFile()
    {
        var dialog = new OpenFileDialog
        {
            Title = "Выберите файл отчёта",
            Filter = "Документы (*.pdf;*.doc;*.docx)|*.pdf;*.doc;*.docx|Все файлы (*.*)|*.*"
        };

        if (dialog.ShowDialog() == true)
            FilePath = dialog.FileName;
    }

    private bool IsValid()
    {
        if (SelectedStudent == null)
        {
            MessageBox.Show("Выберите студента", "Ошибка",
                MessageBoxButton.OK, MessageBoxImage.Warning);
            return false;
        }

        if (SelectedPracticeSheet == null)
        {
            MessageBox.Show("Выберите ведомость практики", "Ошибка",
                MessageBoxButton.OK, MessageBoxImage.Warning);
            return false;
        }

        if (string.IsNullOrWhiteSpace(FilePath))
        {
            MessageBox.Show("Укажите путь к файлу отчёта", "Ошибка",
                MessageBoxButton.OK, MessageBoxImage.Warning);
            return false;
        }

        // Оценка необязательна, но если введена — должна быть 2–5
        if (!string.IsNullOrWhiteSpace(GradeString))
        {
            if (!byte.TryParse(GradeString, out var grade) || grade < 2 || grade > 5)
            {
                MessageBox.Show("Оценка должна быть от 2 до 5", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }
        }

        // Дата необязательна, но если введена — должна быть корректной
        if (!string.IsNullOrWhiteSpace(SubmissionDateString) &&
            !DateOnly.TryParseExact(SubmissionDateString, "dd.MM.yyyy",
                CultureInfo.InvariantCulture, DateTimeStyles.None, out _))
        {
            MessageBox.Show("Неверный формат даты сдачи (дд.мм.гггг)", "Ошибка",
                MessageBoxButton.OK, MessageBoxImage.Warning);
            return false;
        }

        return true;
    }

    [RelayCommand]
    private void Save(Window window)
    {
        if (!IsValid()) return;

        byte? grade = string.IsNullOrWhiteSpace(GradeString) ? null
                             : byte.Parse(GradeString);
        DateOnly? subDate = string.IsNullOrWhiteSpace(SubmissionDateString) ? null
                             : DateOnly.ParseExact(SubmissionDateString, "dd.MM.yyyy",
                                 CultureInfo.InvariantCulture);

        try
        {
            if (_id == null)
            {
                // Проверяем, нет ли уже отчёта этого студента по этой ведомости
                bool duplicate = Db.Context.StudentReports.Any(r =>
                    r.StudentId == SelectedStudent!.StudentId &&
                    r.PracticeSheetId == SelectedPracticeSheet!.PracticeSheetId);

                if (duplicate)
                {
                    MessageBox.Show("Отчёт этого студента по данной ведомости уже существует.",
                        "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                Db.Context.StudentReports.Add(new StudentReport
                {
                    StudentId = SelectedStudent!.StudentId,
                    PracticeSheetId = SelectedPracticeSheet!.PracticeSheetId,
                    FilePath = FilePath.Trim(),
                    Grade = grade,
                    SubmissionDate = subDate
                });
            }
            else
            {
                var report = Db.Context.StudentReports.First(r => r.ReportId == _id);

                report.StudentId = SelectedStudent!.StudentId;
                report.PracticeSheetId = SelectedPracticeSheet!.PracticeSheetId;
                report.FilePath = FilePath.Trim();
                report.Grade = grade;
                report.SubmissionDate = subDate;
            }

            Db.Context.SaveChanges();
            window?.Close();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Ошибка при сохранении: {ex.Message}", "Ошибка",
                MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}