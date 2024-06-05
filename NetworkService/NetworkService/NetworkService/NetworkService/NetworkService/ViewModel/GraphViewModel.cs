using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MVVMLight.Messaging;
using NetworkService.Model;

namespace NetworkService.ViewModel
{

    public class DataPoint
    {
        public int Value { get; set;}
        public string Time { get; set;}   
    }

    public class GraphViewModel: BindableBase
    {

       

        private string _selectedValveText;
        public string SelectedValveText
        {
            get { return _selectedValveText; }
            set
            {
                if (_selectedValveText != value)
                {
                    _selectedValveText = value;
                    OnPropertyChanged(nameof(SelectedValveText));
                }
            }
        }
        public ObservableCollection<DataPoint> DataPoints { get; set; }

        
        
       

        public GraphViewModel() {
            
        }

        private void ReadData()
        {
            DataPoints.Clear();
            string path = "SimulatorValue.txt";

            // Pisanje podataka u datoteku
            foreach (string line in MainWindowViewModel.ReadDataBackwards(path))
            {
                string[] timeEntitySplit = line.Split('\t');
                string[] times = timeEntitySplit[0].Split(' ');
                string[] timeHourMinuteSecond = times[1].Split(':');
                string hourMinute = timeHourMinuteSecond[0] + ":" + timeHourMinuteSecond[1];


                //string[] valueEntity = timeEntitySplit[1];
                int value = Int32.Parse(timeEntitySplit[1]);

                string entityID = timeEntitySplit[2].Split('_')[1];

                if (Int32.Parse(entityID) == Int32.Parse(SelectedValveText))
                {
                    DataPoints.Add(new DataPoint() { Time = hourMinute, Value = value });
                    if (DataPoints.Count() == 5)
                    {
                        break;
                    }
                }
            }

            //DrawGraph();

        }





    }
}
