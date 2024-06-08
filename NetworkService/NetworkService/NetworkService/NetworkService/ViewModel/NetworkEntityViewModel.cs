using Game_Client.Helpers;
using NetworkService.Helpers;
using NetworkService.Model;
using NetworkService.Views;
using Notification.Wpf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Xml.Linq;
using static System.Net.Mime.MediaTypeNames;
using Type = NetworkService.Model.Type;

namespace NetworkService.ViewModel
{
    public class NetworkEntityViewModel : BindableBase
    {
        public static int Count { get; set; } = 0;

        private static int _idI = 1;
        private static int _idS = 1;
        private static int _id = 1;
        private string _selecteItemAdd;
        private string _selecteItemFilter;
        private string _selectedTypeFilter;
        private Entity _selectedItemForDelete;
        private string _iDText;
        private bool _isLowerChecked;
        private bool _isEqualsChecked;
        private bool _isHigherChecked;
        private ObservableCollection<Entity> _showedCollection;
        private ObservableCollection<Entity> _selectedItems;
        private List<string> _filterOptions;
        private List<string> _types;
        private Filter _tempFilter;
        private Entity _selectedValve;




        public List<string> FilterOptions { get { return _filterOptions; } set { _filterOptions = value; OnPropertyChanged(nameof(FilterOptions)); } }
        public List<string> Types { get { return _types; } set { _types = value; OnPropertyChanged(nameof(Types)); } }

        public ObservableCollection<Entity> FilterValves { get; set; }
        public Entity SelectedValve
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
        public Dictionary<string, Filter> Filters { get; set; }
        public ObservableCollection<string> FilterNames { get; set; }

        public ObservableCollection<Entity> SelectedItems { get { return _selectedItems; } set { _selectedItems = value; OnPropertyChanged(nameof(SelectedItems)); } }
        public ObservableCollection<Entity> ShowedCollection { get { return _showedCollection; } set { _showedCollection = value; OnPropertyChanged(nameof(ShowedCollection)); DeleteCommand.RaiseCanExecuteChanged(); } }
        public static ObservableCollection<Entity> NetowrkEntities { get; set; } = MainWindowViewModel.entities;
        public Entity SelectedItemForDelete { get { return _selectedItemForDelete; } set { _selectedItemForDelete = value; OnPropertyChanged(nameof(SelectedItemForDelete)); } }
        public Filter TempFilter { get { return _tempFilter; } set { _tempFilter = value; OnPropertyChanged(nameof(TempFilter)); } }
        public string SelectedItemAdd { get { return _selecteItemAdd; } set { _selecteItemAdd = value; OnPropertyChanged(nameof(SelectedItemAdd)); AddCommand.RaiseCanExecuteChanged(); } }
        public string SelectedItemFilter { get { return _selecteItemFilter; } set { _selecteItemFilter = value; OnPropertyChanged(nameof(SelectedItemFilter)); } }
        public string SelectedTypeFilter { get { return _selectedTypeFilter; } set { _selectedTypeFilter = value; OnPropertyChanged(nameof(SelectedTypeFilter)); FilterCommand.RaiseCanExecuteChanged(); SaveCommand.RaiseCanExecuteChanged(); } }
        public string IDText { get { return _iDText; } set { _iDText = value; OnPropertyChanged(nameof(IDText)); FilterCommand.RaiseCanExecuteChanged(); SaveCommand.RaiseCanExecuteChanged(); } }
        public bool IsLowerChecked { get { return _isLowerChecked; } set { if (_isLowerChecked != value) { _isLowerChecked = value; OnPropertyChanged(nameof(IsLowerChecked)); FilterCommand.RaiseCanExecuteChanged(); SaveCommand.RaiseCanExecuteChanged(); } } }
        public bool IsEqualsChecked { get { return _isEqualsChecked; } set { if (_isEqualsChecked != value) { _isEqualsChecked = value; OnPropertyChanged(nameof(IsEqualsChecked)); FilterCommand.RaiseCanExecuteChanged(); SaveCommand.RaiseCanExecuteChanged(); } } }
        public bool IsHigherChecked { get { return _isHigherChecked; } set { if (_isHigherChecked != value) { _isHigherChecked = value; OnPropertyChanged(nameof(IsHigherChecked)); FilterCommand.RaiseCanExecuteChanged(); SaveCommand.RaiseCanExecuteChanged(); } } }


        public MyICommand AddCommand { get; private set; }
        public MyICommand DeleteCommand { get; private set; }
        public MyICommand FilterCommand { get; private set; }
        public MyICommand ResetCommand { get; private set; }
        public MyICommand SaveCommand { get; private set; }
        public MyICommand ComboBoxSelectionChangedCommand { get; private set; }




        private NetworkDisplay _networkDisplay;

        public NetworkEntityViewModel(NetworkDisplay networkDisplay)
        {
            _networkDisplay = networkDisplay;

            


            FilterOptions = new List<string> { "All", "Cable_Sensor", "Digital_Manometer" };

            Types = new List<string> { "Cable_Sensor", "Digital_Manometer" };

            FilterValves = new ObservableCollection<Entity>(MainWindowViewModel.entities);
            Filters = new Dictionary<string, Filter>();
            FilterNames = new ObservableCollection<string>();

            AddCommand = new MyICommand(AddEntity,CanAdd);

            DeleteCommand = new MyICommand(DeleteEntity,CanDelete);

            FilterCommand = new MyICommand(FilterEntity);
            
            ResetCommand = new MyICommand(ResetEntity);

            SaveCommand = new MyICommand(SaveEntity);
            ComboBoxSelectionChangedCommand = new MyICommand(OnComboBoxSelectionChanged);



            SelectedTypeFilter = "All";
        }

        private bool CanAdd()
        {
            return SelectedItemAdd != null;
        }

        private bool CanFilter()
        {
            bool isChecked = IsEqualsChecked || IsLowerChecked || IsHigherChecked;
            bool idChecked = IDText != null & IDText != String.Empty;
            if (idChecked && isChecked)
            {
                return true;
            }
            if (SelectedTypeFilter != "All")
            {
                return true;
            }
            return false;
        }

        private bool CanDelete()
        {
            return SelectedValve != null;
        }

        private void DeleteEntity()
        {
            MessageBoxResult res = MessageBox.Show("Do you want to delete this Valve?", "Delete Valve", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (res == MessageBoxResult.Yes)
            {
                List<Line> line = new List<Line>();

                _networkDisplay._networkDisplayViewModel.RemoveEntityNode(SelectedValve);

                MainWindowViewModel.entities.Remove(SelectedValve);


                foreach (var linija in _networkDisplay._networkDisplayViewModel.LineCollection)
                {
                    if (linija.Destination == GetCanvasIndexForEntityId(SelectedValve.Id) || linija.Source == GetCanvasIndexForEntityId(SelectedValve.Id))
                    {
                        if (!line.Contains(linija))
                            line.Add(linija);
                    }
                }

                foreach (var lineDelete in line)
                {
                    _networkDisplay._networkDisplayViewModel.LineCollection.Remove(lineDelete);
                }


                DeleteEntityFromCanvas(SelectedValve);
                FilterValves.Remove(SelectedValve);
                RestartOtherApplication("C:\\Users\\milos\\Documents\\Faculty\\6. Semestar\\HCI\\Pokusaj2\\NetworkService\\MeteringSimulator\\MeteringSimulator\\bin\\Debug\\MeteringSimulator.exe");
            }


        }



        public int GetCanvasIndexForEntityId(int entityId)
        {
            for (int i = 0; i < _networkDisplay._networkDisplayViewModel.CanvasCollection.Count; i++)
            {
                Entity entity = (_networkDisplay._networkDisplayViewModel.CanvasCollection[i].Resources["data"]) as Entity;

                if ((entity != null) && (entity.Id == entityId))
                {
                    return i;
                }
            }
            return -1;
        }

        public void DeleteEntityFromCanvas(Entity entity)
        {

            int canvasIndex = GetCanvasIndexForEntityId(entity.Id);

            if (canvasIndex != -1)
            {
                _networkDisplay._networkDisplayViewModel.CanvasCollection[canvasIndex].Resources.Remove("taken");
                _networkDisplay._networkDisplayViewModel.CanvasCollection[canvasIndex].Resources.Remove("data");
                _networkDisplay._networkDisplayViewModel.CanvasCollection[canvasIndex].Background = (SolidColorBrush)(new BrushConverter().ConvertFrom("#3E3E3E"));
                _networkDisplay._networkDisplayViewModel.OnFiledsFreeCanvas(canvasIndex);
                NetworkDisplayViewModel.BorderBrushValues[canvasIndex] = "#A09D9D";
                MainWindowViewModel.ShowToastNotification(new ToastNotification("Success", "Successfully delete entity", NotificationType.Success));



            }
        }

        private void AddEntity()
        {
            string type = "";

            try
            {
                type = SelectedItemAdd;
            }
            catch (Exception)
            {
                //GRESKU IZBACITI
            }

            if (type.Equals("Cable_Sensor"))
            {
                string name = $"Cable_Sensor {_idI}";
                List<double> values = new List<double> { 0, 0, 0, 0, 0 };
                Entity newEntity = new Entity(_id, name, Type.Cable_Sensor, values);
                MainWindowViewModel.entities.Add(newEntity);

                if (TempFilter != null)
                {
                    if (TempFilter.FilterEntity(newEntity))
                    {
                        FilterValves.Add(newEntity);
                    }
                }
                else
                {
                    FilterValves.Add(newEntity);
                }
                _networkDisplay._networkDisplayViewModel.AddEntityNode(newEntity);
                Count = MainWindowViewModel.entities.Count;
                _idI++;
                _id++;
            }
            else if (type.Equals("Digital_Manometer"))
            {
                string name = $"Digital_Manometer {_idS}";
                List<double> values = new List<double> { 0, 0, 0, 0, 0 };
                Entity newEntity = new Entity(_id, name, Type.Digital_Manometer, values);
                MainWindowViewModel.entities.Add(newEntity);
                FilterValves.Add(newEntity);

                _networkDisplay._networkDisplayViewModel.AddEntityNode(newEntity);
                Count = MainWindowViewModel.entities.Count;
                _idS++;
                _id++;
            }

            RestartOtherApplication("C:\\Users\\milos\\Documents\\Faculty\\6. Semestar\\HCI\\Pokusaj2\\NetworkService\\MeteringSimulator\\MeteringSimulator\\bin\\Debug\\MeteringSimulator.exe");
            
        }

        private void FilterEntity()
        {
            bool valid = false;
            bool isChecked = IsEqualsChecked || IsLowerChecked || IsHigherChecked;
            bool idChecked = IDText != null & IDText != String.Empty;
            if (idChecked && isChecked)
            {
                valid = true;
            }
            if (SelectedTypeFilter != "All")
            {
                valid = true;
            }
            if (valid)
            {
                Filter filter = CollectFilterInfo();
                TempFilter = filter;
                FilterValves.Clear();
                foreach (Entity v in MainWindowViewModel.entities)
                {
                    if (filter.FilterEntity(v))
                    {
                        FilterValves.Add(v);
                    }
                }
            }
            else
            {
                MainWindowViewModel.ShowToastNotification(new ToastNotification("Error", "You need to select Radio Button and input Id or select Type", NotificationType.Error));

            }

        }

        private Filter CollectFilterInfo()
        {
            string operation = String.Empty;
            Filter filter = new Filter();

            if (IDText != null)
            {
                int id = Int32.Parse(IDText);
                filter.Id = id;
                if (IsHigherChecked)
                {
                    operation = "More";
                }
                else if (IsLowerChecked)
                {
                    operation = "Less";
                }
                else if (IsEqualsChecked)
                {
                    operation = "Equals";
                }
                filter.Operation = operation;
            }
            if (SelectedTypeFilter != null)
            {
                Type type = Model.Type.All;
                if (SelectedTypeFilter[0] == 'D')
                {
                    type = Model.Type.Digital_Manometer;
                }
                else if (SelectedTypeFilter[0] == 'C')
                {
                    type = Model.Type.Cable_Sensor;
                }
                filter.Type = type.ToString();

            }
            return filter;
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

        private void SaveEntity()
        {
            bool valid = false;
            bool isChecked = IsEqualsChecked || IsLowerChecked || IsHigherChecked;
            bool idChecked = IDText != null & IDText != String.Empty;
            if (idChecked && isChecked)
            {
                valid = true;
            }
            if (SelectedTypeFilter != "All")
            {
                valid = true;
            }
            if (valid)
            {
                Filter filter = CollectFilterInfo();
                if (!(Filters.ContainsKey(filter.GetName())))
                {

                    Filters[filter.GetName()] = filter;
                    FilterNames.Add(filter.GetName());
                    SelectedFilterText = filter.GetName();
                }
                else
                {
                    MainWindowViewModel.ShowToastNotification(new ToastNotification("Error", "This Filter Already Exsits!", NotificationType.Error));
                }
            }
            else
            {
                MainWindowViewModel.ShowToastNotification(new ToastNotification("Error", "You need to select Radio Button and input Id or select Type", NotificationType.Error));

            }

        }

        private void ResetFilterForm()
        {
            IDText = null;
            IsEqualsChecked = false;
            IsLowerChecked = false;
            IsHigherChecked = false;
            SelectedFilterText = null;
            SelectedTypeFilter = "All";
            MainWindowViewModel.ShowToastNotification(new ToastNotification("Success", "Successfully reset filter form", NotificationType.Success));

        }

        private void OnComboBoxSelectionChanged()
        {
            if (SelectedFilterText != null)
            {
                Filter filter = Filters[SelectedFilterText];
                IDText = filter.Id.ToString();
                IsHigherChecked = false;
                IsLowerChecked = false;
                IsEqualsChecked = false;
                switch (filter.Operation)
                {
                    case "More":
                        IsHigherChecked = true;
                        break;
                    case "Less":
                        IsLowerChecked = true;
                        break;
                    case "Equals":
                        IsEqualsChecked = true;
                        break;

                }

                if (filter.Type == Type.Digital_Manometer.ToString())
                {
                    SelectedTypeFilter = "Digital_Manometer";
                }
                else if (filter.Type == Type.Cable_Sensor.ToString())
                {
                    SelectedTypeFilter = "Cable_Sensor";
                }
                else
                {
                    SelectedTypeFilter = "All";
                }
            }

        }

        private void ResetEntity()
        {
            ResetFilterForm();
            FilterValves.Clear();
            foreach (Entity v in MainWindowViewModel.entities)
            {
                FilterValves.Add(v);
            }
        }

        
    }
}
