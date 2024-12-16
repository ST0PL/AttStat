using AttStat.Db.Models;
using AttStat.Models;
using CommunityToolkit.Mvvm.Input;

namespace AttStat.ViewModels
{
    internal partial class TableVM : BaseVM
    {

        private SystemDataContext _context;
        private AddDataWindow _currentDataForm;
        private List<Attestation> _attestations;
        private List<Discipline>? _disciplines;
        private List<Group>? _groups;
        private Group _group;
        private int _facultyId = 0;
        private int _groupId = 0;
        private int _disciplineId = 0;
        private int _orderId;
        private bool _onlyLowRating;
        private double _average;
        private int _count;

        public int FacultyId
        {
            get => _facultyId;
            set
            {
                _facultyId = value;
                try
                {
                    if (value == 0)
                        _groups = [new Group() { Id = 0, Name = "Все" }, .. _context.Groups];
                    else
                        _groups = [new Group() { Id = 0, Name = "Все" }, .. _context.Groups.Where(g => g.SpecializationNavigation.Faculty == value)];
                    Groups = _groups.OrderBy(g => g.Name).ToList();
                    GroupId = 0;
                }
                catch (Exception ex) { new AlertWindow("Ошибка при работе с базой данных", ex.Message).Show(); }
                OnPropertyChanged();
            }
        }
        public int GroupId
        {
            get => _groupId;
            set
            {
                _groupId = value;
                if (value == 0)
                    Disciplines = [new Discipline() { Id = 0, Name = "Все" }, .._context.Disciplines.Where(d => _facultyId == 0 || d.SpecializationNavigation.Faculty == _facultyId)];
                else
                {
                    _group = _context.Groups.Where(g => g.Id == value).First();
                    Disciplines = [new Discipline() { Id = 0, Name = "Все"},.. _context.Disciplines.ToList().Where(d => (d.SpecializationNavigation.Faculty == _group.SpecializationNavigation.Faculty) && d.Course.Split().Contains(_group.Course.ToString()))];
                }
                DisciplineId = 0;
                OnPropertyChanged();
            }
        }
        public int DisciplineId
        {
            get => _disciplineId;
            set
            {
                _disciplineId = value;
                ApplyFilters();
                OrderBy(OrderId);
                OnPropertyChanged();
            }
        }
        public int OrderId
        {
            get => _orderId;
            set
            {
                _orderId = value;
                OrderBy(value);
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
        public List<Discipline>? Disciplines
        {
            get => _disciplines;
            set
            {
                _disciplines = value;
                OnPropertyChanged();
            }
        }
        public List<Attestation>? Attestations
        {
            get => _attestations;
            set
            {
                _attestations = value;
                OnPropertyChanged();
            }
        }
        public List<Faculty>? Faculties { get; set; }
        public List<Order>? Orders { get; set; }
        public bool OnlyLowRating
        {
            get => _onlyLowRating;
            set
            {
                _onlyLowRating = value;
                ApplyFilters();

            }
        }
        public int Count
        {
            get => _count;
            set
            {
                _count = value;
                OnPropertyChanged();
            }
        }
        public double Average
        {
            get => _average;
            set
            {
                _average = value;
                OnPropertyChanged();
            }
        }
        public TableVM()
        {
            _context = new SystemDataContext();
            try
            {
                Faculties = [new Faculty() { Id = 0, Name = "Все" }, .. _context.Faculties];
                Groups = [new Group() { Id = 0, Name = "Все" }, .. _context.Groups];
                Disciplines = [new Discipline() { Id = 0, Name = "Все" }, .. _context.Disciplines];
                Orders = [new Order(0, "Фамилия"), new Order(1, "Дисциплина"), new Order(2, "Рейтинг"), new Order(3, "Группа"), new Order(4, "Курс")];
                LoadData();
            }
            catch(Exception ex) { new AlertWindow("Ошибка при работе с базой данных", ex.Message).Show(); }
        }

        [RelayCommand]
        public void OpenDataForm()
        {
            if (_currentDataForm?.IsClosed ?? true)
            {
                _currentDataForm = new AddDataWindow();
                _currentDataForm.Show();
            }
            else
                _currentDataForm.Show();
        }
        [RelayCommand]
        public async Task LoadData()
        {
            FacultyId = 0;
            await _context.DisposeAsync();
            if (!_context.IsDisposed)
                _context.Dispose();
            _context = new SystemDataContext();
            try
            {
                Attestations = _context.Attestations.ToList();
                Count = Attestations.Count;
                Average = Math.Round((double)Attestations.Select(a => a.Score).Aggregate((s1, s2) => s1 + s2) / Count, 2);
            }
            catch (Exception ex) { new AlertWindow("Ошибка при работе с базой данных", ex.Message).Show(); }
        }
        [RelayCommand]
        void UpdateData()
        {
            if (!_context.IsDisposed)
                _context.Dispose();
            _context = new SystemDataContext();
            ApplyFilters();
        }
        void ApplyFilters()
        {
            try
            {
                Attestations = _context.Attestations
                        .Where(a => (a.Score.Value < 25 || !OnlyLowRating) && (a.StudentNavigation.GroupNavigation.SpecializationNavigation.Faculty == _facultyId || _facultyId == 0)
                        && (a.StudentNavigation.Group == _groupId || _groupId == 0) && (a.Discipline == _disciplineId || _disciplineId == 0)).ToList();
                OrderBy(OrderId);
                Count = Attestations.Count;
                Average = Count > 0 ? Math.Round((double)Attestations.Select(a => a.Score).Aggregate((s1, s2) => s1 + s2) / Count, 2) : 0;
            }
            catch (Exception ex) { new AlertWindow("Ошибка при работе с базой данных", ex.Message).Show(); }
        }
        void OrderBy(int orderId)
        {
            switch (orderId)
            {
                case 0:
                    Attestations = Attestations?.OrderBy(a => a.StudentNavigation.LastName).ToList();
                    break;
                case 1:
                    Attestations = Attestations?.OrderBy(a => a.DisciplineNavigation.Name).ToList();
                    break;
                case 2:
                    Attestations = Attestations?.OrderByDescending(a => a.Score).ToList();
                    break;
                case 3:
                    Attestations = Attestations?.OrderBy(a => a.StudentNavigation.GroupNavigation.Name).ToList();
                    break;
                case 4:
                    Attestations = Attestations?.OrderByDescending(a => a.StudentNavigation.GroupNavigation.Course).ToList();
                    break;
            }
        }
    }
}
