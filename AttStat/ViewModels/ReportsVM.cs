using AttStat.Db.Models;
using CommunityToolkit.Mvvm.Input;
using PdfSharp.Fonts;
using MigraDoc.DocumentObjectModel;
using MigraDoc.Rendering;
using MigraDoc.DocumentObjectModel.Tables;
using System.Diagnostics;
using Microsoft.Win32;
using System.IO;

namespace AttStat.ViewModels
{
    partial class ReportsVM : BaseVM
    {
        private SystemDataContext _context;
        private AddDataWindow _currentDataForm;
        private List<Attestation> _attestations;
        private List<Group>? _groups;
        private Group _group;
        private int _facultyId = 0;
        private int _groupId = 0;
        private int _disciplineId = 0;
        private bool _onlyLowRating;
        private bool _isNotBusy = true;
        private bool _canSave;
        public bool IsNotBusy
        {
            get => _isNotBusy;
            set
            {
                _isNotBusy = value;
                OnPropertyChanged();
            }
        }
        public int FacultyId
        {
            get => _facultyId;
            set
            {
                _facultyId = value;
                if (value == 0)
                    _groups = _context.Groups.ToList();
                else
                    _groups = _context.Groups.Where(g => g.SpecializationNavigation.Faculty == value).ToList();
                Groups = _groups.OrderBy(g => g.Name).ToList();
                GroupId = Groups.FirstOrDefault()?.Id ?? 0;
                OnPropertyChanged();
            }
        }
        public int GroupId
        {
            get => _groupId;
            set
            {
                _groupId = value;
                _group = _context.Groups.Where(g=>g.Id == _groupId).First();
                OnPropertyChanged();
            }
        }
        public bool OnlyLowRating
        {
            get => _onlyLowRating;
            set
            {
                _onlyLowRating = value;
                ApplyFilters();

            }
        }
        public bool CanSave
        {
            get => _canSave;
            set
            {
                _canSave = value;
                ApplyFilters();

            }
        }
        public List<Group>? Groups
        {
            get => _groups;
            set
            {
                _groups = value;
                OnPropertyChanged();
            }
        }

        public List<Faculty>? Faculties { get; set; }
        public ReportsVM()
        {
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
            GlobalFontSettings.UseWindowsFontsUnderWindows = true;
            _context = new SystemDataContext();
            try
            {
                Faculties = _context.Faculties.ToList();
                Groups = _context.Groups.ToList();
                GroupId = Groups.FirstOrDefault()?.Id ?? 0;
            }
            catch (Exception ex) { new AlertWindow("Ошибка при работе с базой данных", ex.Message).Show(); }
        }
        void ApplyFilters()
        {
            try
            {
                _attestations = _context.Attestations
                    .Where(a => (a.StudentNavigation.GroupNavigation.SpecializationNavigation.Faculty == _facultyId || _facultyId == 0)
                    && (a.StudentNavigation.Group == _groupId || _groupId == 0) && (a.Discipline == _disciplineId || _disciplineId == 0)).ToList();
            }
            catch (Exception ex) { new AlertWindow("Ошибка при работе с базой данных", ex.Message).Show(); }
        }
        List<KeyValuePair<string, int?[]>> BuildDataStruct()
            => _attestations.GroupBy(a => a.StudentNavigation.Id).Select(a =>
                {
                    var student = a.First().StudentNavigation;
                    return KeyValuePair.Create($"{student.LastName} {student.FirstName} {student.MiddleName}", a.Select(i => i.Score).ToArray());
                }).Where(ds=>!OnlyLowRating || ds.Value.Any(v => v < 25)).ToList();
        [RelayCommand]
        public async Task GenerateReport()
        {
            IsNotBusy = false;
            try
            {
                ApplyFilters();
                var dataStruct = BuildDataStruct();
                var disciplines = _attestations.Select(a => a.DisciplineNavigation).Distinct().ToArray();
                var doc = new Document();
                var section = doc.AddSection();
                var paragraph = section.AddParagraph();
                var pdfRenderer = new PdfDocumentRenderer();
                paragraph.AddText($"Отчет {(OnlyLowRating ? "о неуспевающих студентах" : "об успеваемости студентов")} группы {_group.Name}");
                paragraph.AddLineBreak();
                paragraph.AddLineBreak();
                paragraph.AddText($"Дата генерации: {DateTime.Now.ToString("MM.dd.yyyy HH:mm")}");
                paragraph.Format.SpaceAfter = 20;
                var table = section.AddTable();
                table.Format.Alignment = ParagraphAlignment.Center;
                table.Format.Font.Size = 8;
                table.Borders.Width = 0.5;
                table.AddColumn("4cm");
                for (int i = 0; i < disciplines.Length; i++)
                    table.AddColumn("1cm");
                table.AddColumn("2cm");
                Row row = table.AddRow();
                row.VerticalAlignment = VerticalAlignment.Center;
                row.HeadingFormat = true;
                row.Format.Font.Bold = true;
                row.Cells[0].AddParagraph("");
                for (int i = 0; i < disciplines.Length; i++)
                    row.Cells[i + 1].AddParagraph(disciplines[i].Id.ToString());
                row.Cells[disciplines.Length + 1].AddParagraph("Средний рейтинг");
                for (int i = 0; i < disciplines.Length + 2; i++)
                {
                    row.Cells[i].Shading.Color = Color.Parse("#d8d8d8");
                }
                for (int i = 0; i < dataStruct.Count; i++)
                {
                    row = table.AddRow();
                    row.VerticalAlignment = VerticalAlignment.Center;
                    row.Cells[0].AddParagraph(dataStruct[i].Key);
                    row.Cells[0].Format.Alignment = ParagraphAlignment.Left;
                    for (int j = 0; j < disciplines.Length; j++)
                    {
                        row.Cells[j + 1].AddParagraph(dataStruct[i].Value[j].ToString());
                    }
                    row.Cells[disciplines.Length + 1].AddParagraph(Math.Round(dataStruct[i].Value.Aggregate((s1, s2) => s1 + s2).Value / (double)dataStruct[i].Value.Length, 0).ToString());

                }
                paragraph = section.AddParagraph();
                paragraph.Format.SpaceBefore = 20;
                for (int i = 0; i < disciplines.Length; i++)
                {
                    paragraph.AddText($"{disciplines[i].Id} - {disciplines[i].Name}");
                    paragraph.AddLineBreak();
                }
                pdfRenderer.Document = doc;
                pdfRenderer.RenderDocument();
                var ofd = new OpenFolderDialog();
                if (ofd.ShowDialog().GetValueOrDefault())
                {
                    var path = Path.Combine(ofd.FolderName, $"Отчет {(OnlyLowRating ? "о неуспевающих студентах" : "об успеваемости студентов")} {_group.Name}.pdf");
                    pdfRenderer.Save(path);
                    Process.Start(new ProcessStartInfo() { FileName = path, UseShellExecute = true });
                }
            }
            catch { new AlertWindow("Операция завершена с ошибкой", "Не удалось сформировать отчёт.").Show(); }




            IsNotBusy = true;
        }
    }
}
