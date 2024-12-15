using LiveChartsCore.SkiaSharpView;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView.VisualElements;
using AttStat.Db.Models;
using System.Diagnostics;
using CommunityToolkit.Mvvm.Input;

namespace AttStat.ViewModels
{
    internal partial class ChartsVM : BaseVM
    {
        private SystemDataContext _context;
        private IEnumerable<ISeries> _series;
        private List<Axis> _xaxes;
        private int _specializationId;
        private int _disciplineId;
        private int _course;
        private List<Specialization> _specializations;
        private List<int> _courses;
        public int SpecializationId
        {
            get => _specializationId;
            set
            {
                _specializationId = value;
                Courses = _context.Groups.Where(g => g.Specialization == value).Select(g => g.Course).Distinct().Order().ToList();
                Course = Courses.FirstOrDefault();
                OnPropertyChanged();
            }
        }
        public int Course
        {
            get => _course;
            set
            {
                _course = value;
                UpdateChart();
                OnPropertyChanged();
            }
        }
        public List<Specialization> Specializations
        {
            get => _specializations;
            set
            {
                _specializations = value;
                OnPropertyChanged();
            }
        }
        public List<int> Courses
        {
            get => _courses;
            set
            {
                _courses = value;
                OnPropertyChanged();
            }
        }
        public IEnumerable<ISeries> Series
        {
            get => _series;
            set
            {
                _series = value;
                OnPropertyChanged();
            }
        }

        public List<Axis> XAxes
        {
            get => _xaxes;
            set
            {
                _xaxes = value;
                OnPropertyChanged();
            }
        }
        public List<Axis> YAxes { get; set; }
        public LabelVisual Title { get; set; } =
            new LabelVisual
            {
                Text = "Средний рейтинг групп специальности",
                TextSize = 25,
                Padding = new LiveChartsCore.Drawing.Padding(15)
            };
        public ChartsVM()
        {
            _context = new SystemDataContext();
            try
            {
                Specializations = _context.Specializations.ToList();
                SpecializationId = Specializations.FirstOrDefault()?.Id ?? 0;
            }
            catch (Exception ex) { new AlertWindow("Ошибка при работе с базой данных", ex.Message).Show(); }
            YAxes = new List<Axis>() { new Axis() { MaxLimit = 100 } };
        }
        [RelayCommand]
        void UpdateChart()
        {
            _context.Dispose();
            _context = new SystemDataContext();
            try
            {
                var stat = _context.Attestations.Where(a => a.Score != null && a.StudentNavigation.GroupNavigation.Specialization == _specializationId && a.StudentNavigation.GroupNavigation.Course == _course).ToArray().GroupBy(a => a.StudentNavigation.GroupNavigation.Name).Select(g =>
                {
                    Debug.WriteLine(g.Count());
                    return KeyValuePair.Create(g.Key, Math.Round(g.Select(i => i.Score.Value).Aggregate((s1, s2) => s1 + s2) / (double)g.Count(), 2));
                }).ToDictionary();
                XAxes = new List<Axis>() { new Axis() { Labels = stat.Keys.ToArray() } };
                Series = new List<ISeries> { new ColumnSeries<double>() { Values = stat.Values.ToArray() } };
            }
            catch (Exception ex) { new AlertWindow("Ошибка при работе с базой данных", ex.Message).Show(); }
        }
    }
}
