using NetworkService.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using MVVMLight.Messaging;
using System.Windows;
using System.Diagnostics;
using System.IO;

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
        private string _selectedFilterText;
        public string SelectedFilterText
        {
            get { return _selectedFilterText; }
            set
            {
                if (_selectedFilterText != value)
                {
                    _selectedFilterText = value;
                    OnPropertyChanged(nameof(SelectedFilterText));
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


        private Filter _tempFilter;
        public Filter TempFilter
        {
            get
            {
                return _tempFilter;
            }
            set
            {
                if (_tempFilter != value)
                {
                    _tempFilter = value;
                    OnPropertyChanged(nameof(TempFilter));
                }
            }
        }
        public ObservableCollection<string>Types { get; set; }
        public ObservableCollection<Valve> FilterValves { get; set; }
        public Dictionary<string,Filter>Filters { get; set; }
        public ObservableCollection<string> FilterNames { get; set; }



        public List<int> IDs { get; set; }


        public MyICommand AddCommand { get; set; }
        public MyICommand DeleteCommand { get; set; }
        public MyICommand FilterCommand { get; set; }
        public MyICommand ResetCommand { get; set; }
        public MyICommand SaveCommand { get; set; }
        public MyICommand ComboBoxSelectionChangedCommand { get; private set; }

        

        public TableViewModel()
        {
            LoadData();
            AddCommand = new MyICommand(OnAdd);
            DeleteCommand = new MyICommand(OnDelete, CanDelete);
            FilterCommand = new MyICommand(OnFilter);
            ResetCommand = new MyICommand(OnReset);
            SaveCommand = new MyICommand(OnSave);
            Messenger.Default.Register<string>(this, "ValueFromSimulator", WriteValueToEntity);
        }

        public void LoadData()
        {
            Types = new ObservableCollection<string>() { "Cable Sensor", "Digital Manometer" };
            Filter f1 = new Filter();
            Filters = new Dictionary<string, Filter>();
            FilterNames = new ObservableCollection<string>();

            ComboBoxSelectionChangedCommand = new MyICommand(OnComboBoxSelectionChanged);

            IDs = new List<int>();
            IDs = Enumerable.Range(1, 100).ToList();
            IDs.RemoveAt(49);
            IDs.RemoveAt(50);
            IDs.RemoveAt(51);
            FilterValves = new ObservableCollection<Valve>(MainWindowViewModel.Valves);


        }

        private void WriteValueToEntity(string received)
        {
            string[] pom = received.Split('_',':');
            int row = Int32.Parse(pom[1]);
            int value = Int32.Parse(pom[2]);
            bool valid = value >= 5 && value <= 16;
            int id = MainWindowViewModel.Valves[row].Id;
            string retVal = id.ToString() + ";" + pom[2] + ";" + valid.ToString() + ";" + DateTime.Now;
            string path = "SimulatorValue.txt";
            if(valid)
                MainWindowViewModel.Valves[row].Value = value;
                
            if (!File.Exists(path))
            {
                MessageBox.Show("File does not exsits ");
            }
            else
            {
                using (StreamWriter sw = new StreamWriter(path,true))
                {
                    sw.WriteLine(retVal);
                }
            }
        }

        private void OnAdd()
        {
            EntityType type = new EntityType(Model.Type.CableSensor);
            if (TypesText[0] == 'D')
            {
                 type = new EntityType(Model.Type.DigitalManometer);
            }
            Valve valve = new Valve { Id = IDs[0], Name = "Valve_" + IDs[0].ToString(), Value = 7, Type = type };
            MainWindowViewModel.Valves.Add(valve);
            if (TempFilter != null)
            {
                if (TempFilter.FilterEntity(valve))
                {
                    FilterValves.Add(valve);    
                }
            }
            else
            {
                FilterValves.Add(valve);
            }
            IDs.RemoveAt(0);
            TypesText = null;
            Messenger.Default.Send<int>(MainWindowViewModel.Valves.Count(), "Count");
            RestartOtherApplication("C:\\Users\\milos\\Documents\\Faculty\\6. Semestar\\HCI\\HCI_PZ2\\NetworkService\\NetworkService\\MeteringSimulator\\MeteringSimulator\\bin\\Debug\\MeteringSimulator.exe");


        }

        private void OnDelete()
        {
            int id = SelectedValve.Id;
            IDs.Insert(id - 1, id);
            MainWindowViewModel.Valves.Remove(SelectedValve);
            FilterValves.Remove(SelectedValve);
            Messenger.Default.Send<int>(MainWindowViewModel.Valves.Count(), "Count");
            RestartOtherApplication("C:\\Users\\milos\\Documents\\Faculty\\6. Semestar\\HCI\\HCI_PZ2\\NetworkService\\NetworkService\\MeteringSimulator\\MeteringSimulator\\bin\\Debug\\MeteringSimulator.exe");


        }

        private bool CanDelete()
        {
            return SelectedValve != null;
        }

        private void OnFilter()
        {
            Filter filter = CollectFilterInfo();
            TempFilter = filter;
            FilterValves.Clear();
            foreach(Valve v in MainWindowViewModel.Valves)
            {
                if(filter.FilterEntity(v))
                {
                    FilterValves.Add(v);
                }
            }
        }
        private Filter CollectFilterInfo()
        {
            string operation = String.Empty;
            Filter filter = new Filter();

            if (IdText != null)
            {
                int id = Int32.Parse(IdText);
                filter.Id = id;
                if (IsMoreSelected)
                {
                    operation = "More";
                }
                else if (IsLessSelected)
                {
                    operation = "Less";
                }
                else if (IsEqualsSelected)
                {
                    operation = "Equals";
                }
                filter.Operation = operation;
            }
            if (FilterTypeText != null)
            {
                EntityType type = new EntityType(Model.Type.CableSensor);
                if (FilterTypeText[0] == 'D')
                {
                    type = new EntityType(Model.Type.DigitalManometer);
                }
                filter.Type = type;

            }
            return filter;
        }

        private void ResetFilterForm()
        {
            IdText = null;
            IsEqualsSelected = false;
            IsLessSelected = false;
            IsMoreSelected = false;
            FilterTypeText = null;
            SelectedFilterText = null;
        }
        private void OnReset()
        {
            ResetFilterForm();
            FilterValves.Clear();
            foreach(Valve v in MainWindowViewModel.Valves)
            {
                FilterValves.Add(v);
            }
        }

        private void OnSave()
        {
            
            Filter filter = CollectFilterInfo();
            if (!(Filters.ContainsKey(filter.GetName()))){
                
                Filters[filter.GetName()] = filter;
                FilterNames.Add(filter.GetName());
                SelectedFilterText = filter.GetName();
            }
        }

        private void OnComboBoxSelectionChanged()
        {
            if(SelectedFilterText != null)
            {
                Filter filter = Filters[SelectedFilterText];
                IdText = filter.Id.ToString();
                IsMoreSelected = false;
                IsLessSelected = false;
                IsEqualsSelected = false;
                switch (filter.Operation)
                {
                    case "More":
                        IsMoreSelected = true;
                        break;
                    case "Less":
                        IsLessSelected = true;
                        break;
                    case "Equals":
                        IsEqualsSelected = true;
                        break;

                }

                if (filter.Type.Name == Model.Type.DigitalManometer)
                {
                    FilterTypeText = "Digital Manometer";
                }
                else
                {
                    FilterTypeText = "Cable Sensor";
                }
            }
            
        }
        public void RestartOtherApplication(string otherAppExecutablePath)
        {
            // Pronađi sve instance druge aplikacije i ugasi ih
            foreach (var process in Process.GetProcessesByName("MeteringSimulator")) // Zameni sa stvarnim imenom aplikacije bez ekstenzije
            {
                process.Kill();
                process.WaitForExit();
            }

            // Pokreni novu instancu aplikacije
            Process.Start(otherAppExecutablePath);
        }



    }
}
