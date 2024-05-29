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

        public string TypeText { 
            get { return _typeText; }
            set
            {
                if(_typeText != value)
                {
                    _typeText = value;
                    OnPropertyChanged(nameof(TypeText));
                }
            }
        }

        public ObservableCollection<string>Types { get; set; }
        public ObservableCollection<Valve>Valves { get; set; }
        public List<int> IDs { get; set; }


        public MyICommand AddCommand { get; set; }

        public TableViewModel()
        {
            LoadData();
            AddCommand = new MyICommand(OnAdd);
        }

        public void LoadData()
        {
            Types = new ObservableCollection<string>() { "Cable Sensor", "Digital Manometer" };

            Valves = new ObservableCollection<Valve>();

            IDs = new List<int>();
            IDs = Enumerable.Range(1, 97).ToList();

            // For example, you can call the methods of Service classes from the application layer here.
            Valves.Add(new Valve { Id = 100,Name = "Valve_1",Value = 7,Type = new EntityType(Model.Type.CableSensor) });
            Valves.Add(new Valve { Id = 99, Name = "Valve_2", Value = 8, Type = new EntityType(Model.Type.CableSensor) });
            Valves.Add(new Valve { Id = 98, Name = "Valve_3", Value = 9, Type = new EntityType(Model.Type.DigitalManometer) });
            
        }

        private void OnAdd()
        {
            EntityType type = new EntityType(Model.Type.CableSensor);
            if (TypeText[0] == 'D')
            {
                 type = new EntityType(Model.Type.DigitalManometer);
            }
            
            Valves.Add(new Valve { Id = IDs[0], Name = "Valve_" + IDs[0].ToString(), Value = 7, Type = type });
            IDs.RemoveAt(0);
            TypeText = null;
        }
    }
}
