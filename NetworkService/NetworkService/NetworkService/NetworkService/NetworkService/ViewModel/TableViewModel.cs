using NetworkService.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkService.ViewModel
{
    public class TableViewModel: BindableBase
    {

        private string _typeText;
        private string _idText;
        private string _filterTypeText;
        private Valve _selectedValve;


        public string IdText
        {
            get { return _idText; }
            set
            {
                if (_idText != value)
                {
                    _idText = value;
                    OnPropertyChanged(nameof(IdText));
                }
            }
        }

        public string TypesText { 
            get { return _typeText; }
            set
            {
                if(_typeText != value)
                {
                    _typeText = value;
                    OnPropertyChanged(nameof(TypesText));
                }
            }
        }
        public string FilterTypeText
        {
            get { return _filterTypeText; }
            set
            {
                if (_filterTypeText != value)
                {
                    _filterTypeText = value;
                    OnPropertyChanged(nameof(FilterTypeText));
                }
            }
        }

        private bool _isMoreSelected;
        public bool IsMoreSelected
        {
            get { return _isMoreSelected; }
            set
            {
                if (_isMoreSelected != value)
                {
                    _isMoreSelected = value;
                    OnPropertyChanged(nameof(IsMoreSelected));
                }
            }
        }

        private bool _isLessSelected;
        public bool IsLessSelected
        {
            get { return _isLessSelected; }
            set
            {
                if (_isLessSelected != value)
                {
                    _isLessSelected = value;
                    OnPropertyChanged(nameof(IsLessSelected));
                }
            }
        }

        private bool _isEqualsSelected;
        public bool IsEqualsSelected
        {
            get { return _isEqualsSelected; }
            set
            {
                if (_isEqualsSelected != value)
                {
                    _isEqualsSelected = value;
                    OnPropertyChanged(nameof(IsEqualsSelected));
                }
            }
        }

        public Valve SelectedValve
        {
            get
            {
                return _selectedValve;
            }

            set
            {
                if (_selectedValve != value)
                {
                    _selectedValve = value;
                    OnPropertyChanged(nameof(SelectedValve));
                    DeleteCommand.RaiseCanExecuteChanged();
                }
            }
        }

        public ObservableCollection<string>Types { get; set; }
        public ObservableCollection<Valve>Valves { get; set; }
        public ObservableCollection<Valve> FilterValves { get; set; }

        public List<int> IDs { get; set; }


        public MyICommand AddCommand { get; set; }
        public MyICommand DeleteCommand { get; set; }
        public MyICommand FilterCommand { get; set; }



        public TableViewModel()
        {
            LoadData();
            AddCommand = new MyICommand(OnAdd);
            DeleteCommand = new MyICommand(OnDelete, CanDelete);
        }

        public void LoadData()
        {
            Types = new ObservableCollection<string>() { "Cable Sensor", "Digital Manometer" };

            Valves = new ObservableCollection<Valve>();
            FilterValves = new ObservableCollection<Valve>();

            IDs = new List<int>();
            IDs = Enumerable.Range(1, 100).ToList();

            // For example, you can call the methods of Service classes from the application layer here.
            Valves.Add(new Valve { Id = 50,Name = "Valve_50",Value = 7,Type = new EntityType(Model.Type.CableSensor) });
            Valves.Add(new Valve { Id = 51, Name = "Valve_51", Value = 8, Type = new EntityType(Model.Type.CableSensor) });
            Valves.Add(new Valve { Id = 52, Name = "Valve_52", Value = 9, Type = new EntityType(Model.Type.DigitalManometer) });
            IDs.RemoveAt(49);
            IDs.RemoveAt(50);
            IDs.RemoveAt(51);
            FilterValves = Valves;

        }

        private void OnAdd()
        {
            EntityType type = new EntityType(Model.Type.CableSensor);
            if (TypesText[0] == 'D')
            {
                 type = new EntityType(Model.Type.DigitalManometer);
            }
            
            Valves.Add(new Valve { Id = IDs[0], Name = "Valve_" + IDs[0].ToString(), Value = 7, Type = type });
            IDs.RemoveAt(0);
            TypesText = null;
        }

        private void OnDelete()
        {
            int id = SelectedValve.Id;
            IDs.Insert(id - 1, id);
            Valves.Remove(SelectedValve);

        }

        private bool CanDelete()
        {
            return SelectedValve != null;
        }

        private void OnFilter()
        {

        }

    }
}
