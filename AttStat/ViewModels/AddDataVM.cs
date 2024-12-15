using AttStat.Db.Models;
using System.Text.RegularExpressions;
using AttStat.Models;
using CommunityToolkit.Mvvm.Input;

namespace AttStat.ViewModels
{
    partial class AddDataVM : BaseVM
    {
        private List<GenericStudent>? _students;
        private List<Discipline>? _disciplines;
        private List<Db.Models.Group>? _groups;
        private Student _student;
        private int _facultyId = 0;
        private int _groupId = 0;
        private int _studentId = 0;
        private int _disciplineId = 0;
        private int _changesCount;
        private bool _canSaveChanges;
        private string _score;
        private SystemDataContext _context;
        private Action _closeAction;
        public int FacultyId
        {
            get => _facultyId;
            set
            {
                _facultyId = value;
                if (value == 0)
                    Groups = [new Db.Models.Group() { Id = 0, Name = "Все" }, .. _context.Groups];
                else
                    Groups = [new Db.Models.Group() { Id = 0, Name = "Все" }, .. _context.Groups.Where(g => g.SpecializationNavigation.Faculty == value)];
                GroupId = 0;
                OnPropertyChanged();
            }
        }
        public int GroupId
        {
            get => _groupId;
            set
            {
                _groupId = value;
                Students = (value == 0 ? _context.Students.Where(s=> _facultyId == 0 || s.GroupNavigation.SpecializationNavigation.Faculty == _facultyId).Select(s => new GenericStudent()
                {
                    Id = s.Id,
                    Fio = $"{s.LastName} {s.FirstName} {s.MiddleName}",
                    Course = s.GroupNavigation.Course
                }) :
                _context.Students.Where(s=>s.Group == value && (_facultyId == 0 || s.GroupNavigation.SpecializationNavigation.Faculty == FacultyId)).Select(s => new GenericStudent()
                {
                    Id = s.Id,
                    Fio = $"{s.LastName} {s.FirstName} {s.MiddleName}",
                    Course = s.GroupNavigation.Course
                })).ToList();
                StudentId = Students.Count > 0 ? Students[0].Id : 0;
                OnPropertyChanged();
            }
        }
        public int StudentId
        {
            get => _studentId;
            set
            {
                _studentId = value;
                if (value != 0)
                {
                    _student = _context.Students.Where(s => s.Id == value).First();
                    Disciplines = _context.Disciplines.ToList().Where(d => d.Specialization == _student.GroupNavigation.Specialization && d.Course.Split().ToList().Contains(_student.GroupNavigation.Course.ToString())).ToList();
                    DisciplineId = Disciplines[0].Id;
                }
                else DisciplineId = 0;
                OnPropertyChanged();
            }
        }
        public int DisciplineId
        {
            get => _disciplineId;
            set
            {
                _disciplineId = value;
                Score = "";
                OnPropertyChanged();
            }
        }
        public List<GenericStudent>? Students
        {
            get => _students;
            set
            {
                _students = value;
                OnPropertyChanged();
            }
        }
        public List<Discipline>? Disciplines
        {
            get => _disciplines;
            set
            {
                _disciplines = value;
                OnPropertyChanged();
            }
        }
        public string Score
        {
            get => _score;
            set
            {
                if(!Regex.IsMatch(value, "[^0-9]+") && string.IsNullOrEmpty(value) || int.Parse(value) <= 100)
                {
                    _score = value;
                    CanSaveChanges = !string.IsNullOrEmpty(value);
                    WriteDataCommand.NotifyCanExecuteChanged();
                }
                OnPropertyChanged();

            }
        }
        public List<Db.Models.Group>? Groups
        {
            get => _groups;
            set
            {
                _groups = value;
                OnPropertyChanged();
            }
        }
        public bool CanSaveChanges
        {
            get => _canSaveChanges;
            set
            {
                _canSaveChanges = value;
                OnPropertyChanged();
            }
        }
        public List<Faculty>? Faculties { get; set; }
        
        public AddDataVM(Action closeAction)
        {
            _closeAction = closeAction;
            _context = new SystemDataContext();
            try
            {
                Faculties = [new Faculty() { Id = 0, Name = "Все" }, .. _context.Faculties];
                Groups = [new Db.Models.Group() { Id = 0, Name = "Все" }, .. _context.Groups];
                Students = _context.Students.Select(s => new GenericStudent()
                {
                    Id = s.Id,
                    Fio = $"{s.LastName} {s.FirstName} {s.MiddleName}",
                    Course = s.GroupNavigation.Course
                }).ToList();
            }
            catch (Exception ex) { new AlertWindow("Ошибка при работе с базой данных", ex.Message).Show(); }
        }
        [RelayCommand]
        public void SaveData()
        {
            _context.Attestations.Where(a => a.Student == StudentId && a.Discipline == DisciplineId).First().Score = int.Parse(Score);
            _changesCount++;
            WriteDataCommand.NotifyCanExecuteChanged();
            CanSaveChanges = false;
        }
        [RelayCommand(CanExecute = "CheckChanges")]
        public async Task WriteData()
        {
            if(CanSaveChanges)
                SaveData();
            await _context.SaveChangesAsync();
            new AlertWindow("Операция успешно завершена", $"Сохранено изменений: {_changesCount}").Show();
            _closeAction?.Invoke();
        }
        public bool CheckChanges()
            => _changesCount > 0 || CanSaveChanges;
    }
}
