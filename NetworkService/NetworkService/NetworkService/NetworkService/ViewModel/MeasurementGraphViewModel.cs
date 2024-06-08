using NetworkService.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace NetworkService.ViewModel
{
    public class MeasurementGraphViewModel : BindableBase
    {

        private string _timeLine_Label1;
        private string _timeLine_Label2;
        private string _timeLine_Label3;
        private string _timeLine_Label4;
        private string _timeLine_Label5;

        public ObservableCollection<Entity> AllEntities { get; set; } = MainWindowViewModel.entities;
        private ObservableCollection<double> LastFiveValues { get; set; } = new ObservableCollection<double>();
        public static ObservableCollection<string> LastFiveDateTime { get; set; } = new ObservableCollection<string>();

        public string TimeLine_Label1 { get { return _timeLine_Label1; } set { _timeLine_Label1 = value; OnPropertyChanged(nameof(TimeLine_Label1)); } }
        public string TimeLine_Label2 { get { return _timeLine_Label2; } set { _timeLine_Label2 = value; OnPropertyChanged(nameof(TimeLine_Label2)); } }
        public string TimeLine_Label3 { get { return _timeLine_Label3; } set { _timeLine_Label3 = value; OnPropertyChanged(nameof(TimeLine_Label3)); } }
        public string TimeLine_Label4 { get { return _timeLine_Label4; } set { _timeLine_Label4 = value; OnPropertyChanged(nameof(TimeLine_Label4)); } }
        public string TimeLine_Label5 { get { return _timeLine_Label5; } set { _timeLine_Label5 = value; OnPropertyChanged(nameof(TimeLine_Label5)); } }

        public MeasurementGraphViewModel()
        {
            //Messenger.Default.Register<ObservableCollection<Entity>>(this, "entities_collection", TransferEntities);
            //Messenger.Default.Register<string>(this, "value_name", GetNameValues);
            Height1 = 0;
            Height2 = 0;
            Height3 = 0;
            Height4 = 0;
            Height5 = 0;
        }

        private string selectedEntity;

        public string SelectedEntity
        {
            get { return selectedEntity; }
            set
            {
                if (selectedEntity != value)
                {
                    selectedEntity = value;
                    OnPropertyChanged(SelectedEntity);
                    UpdateGraph();
                }
            }
        }

        private SolidColorBrush _blockColor1h = Brushes.Gray;
        public SolidColorBrush BlockColor1h
        {
            get { return _blockColor1h; }
            set
            {
                _blockColor1h = value;
                OnPropertyChanged(nameof(BlockColor1h));
            }
        }
        private SolidColorBrush _blockColor2h = Brushes.Gray;
        public SolidColorBrush BlockColor2h
        {
            get { return _blockColor2h; }
            set
            {
                _blockColor2h = value;
                OnPropertyChanged(nameof(BlockColor2h));
            }
        }
        private SolidColorBrush _blockColor3h = Brushes.Gray;
        public SolidColorBrush BlockColor3h
        {
            get { return _blockColor3h; }
            set
            {
                _blockColor3h = value;
                OnPropertyChanged(nameof(BlockColor3h));
            }
        }
        private SolidColorBrush _blockColor4h = Brushes.Gray;
        public SolidColorBrush BlockColor4h
        {
            get { return _blockColor4h; }
            set
            {
                _blockColor4h = value;
                OnPropertyChanged(nameof(BlockColor4h));
            }
        }
        private SolidColorBrush _blockColor5h = Brushes.Gray;
        public SolidColorBrush BlockColor5h
        {
            get { return _blockColor5h; }
            set
            {
                _blockColor5h = value;
                OnPropertyChanged(nameof(BlockColor5h));
            }
        }

        private double height1;
        public double Height1
        {
            get { return height1; }
            set
            {
                if (value != height1)
                {
                    height1 = value;
                    OnPropertyChanged(nameof(Height1));

                    // Postavljamo Translate transformaciju Canvas-a da bude negativna visina
                    CanvasTranslateY1 = -height1;
                }
            }
        }

        private double canvasTranslateY1;
        public double CanvasTranslateY1
        {
            get { return canvasTranslateY1; }
            set
            {
                if (value != canvasTranslateY1)
                {
                    canvasTranslateY1 = value;
                    OnPropertyChanged(nameof(CanvasTranslateY1));
                }
            }
        }

        private double height2;
        public double Height2
        {
            get { return height2; }
            set
            {
                if (value != height2)
                {
                    height2 = value;
                    OnPropertyChanged(nameof(Height2));

                    // Postavljamo Translate transformaciju Canvas-a da bude negativna visina
                    CanvasTranslateY2 = -height2;
                }
            }
        }

        private double canvasTranslateY2;
        public double CanvasTranslateY2
        {
            get { return canvasTranslateY2; }
            set
            {
                if (value != canvasTranslateY2)
                {
                    canvasTranslateY2 = value;
                    OnPropertyChanged(nameof(CanvasTranslateY2));
                }
            }
        }

        private double height3;
        public double Height3
        {
            get { return height3; }
            set
            {
                if (value != height3)
                {
                    height3 = value;
                    OnPropertyChanged(nameof(Height3));

                    // Postavljamo Translate transformaciju Canvas-a da bude negativna visina
                    CanvasTranslateY3 = -height3;
                }
            }
        }

        private double canvasTranslateY3;
        public double CanvasTranslateY3
        {
            get { return canvasTranslateY3; }
            set
            {
                if (value != canvasTranslateY3)
                {
                    canvasTranslateY3 = value;
                    OnPropertyChanged(nameof(CanvasTranslateY3));
                }
            }
        }

        private double height4;
        public double Height4
        {
            get { return height4; }
            set
            {
                if (value != height4)
                {
                    height4 = value;
                    OnPropertyChanged(nameof(Height4));
                    CanvasTranslateY4 = -height4;
                }
            }
        }

        private double canvasTranslateY4;
        public double CanvasTranslateY4
        {
            get { return canvasTranslateY4; }
            set
            {
                if (value != canvasTranslateY4)
                {
                    canvasTranslateY4 = value;
                    OnPropertyChanged(nameof(CanvasTranslateY4));
                }
            }
        }

        private double height5;
        public double Height5
        {
            get { return height5; }
            set
            {
                if (value != height5)
                {
                    height5 = value;
                    OnPropertyChanged(nameof(Height5));

                    // Postavljamo Translate transformaciju Canvas-a da bude negativna visina
                    CanvasTranslateY5 = -height5 + 5;
                }
            }
        }

        private double canvasTranslateY5;
        public double CanvasTranslateY5
        {
            get { return canvasTranslateY5; }
            set
            {
                if (value != canvasTranslateY5)
                {
                    canvasTranslateY5 = value;
                    OnPropertyChanged(nameof(CanvasTranslateY5));
                }
            }
        }


        public void UpdateGraph()
        {
            foreach (var entity in AllEntities)
            {
                string id = "";
                if (SelectedEntity != null)
                {
                    id = SelectedEntity.Split(':')[1].TrimStart();
                    id = id.Split(' ')[0].TrimStart();
                }
                if (entity.Id.ToString().Equals(id))
                {
                    UpdateValues(entity.Id);

                    Height1 = LastFiveValues[0] * 20;
                    Height2 = LastFiveValues[1] * 20;
                    Height3 = LastFiveValues[2] * 20;
                    Height4 = LastFiveValues[3] * 20;
                    Height5 = LastFiveValues[4] * 20;

                    BlockColor1h = (LastFiveValues[0] < 6 || LastFiveValues[0] > 15) ? Brushes.Red : Brushes.Gray;
                    BlockColor2h = (LastFiveValues[1] < 6 || LastFiveValues[1] > 15) ? Brushes.Red : Brushes.Gray;
                    BlockColor3h = (LastFiveValues[2] < 6 || LastFiveValues[2] > 15) ? Brushes.Red : Brushes.Gray;
                    BlockColor4h = (LastFiveValues[3] < 6 || LastFiveValues[3] > 15) ? Brushes.Red : Brushes.Gray;
                    BlockColor5h = (LastFiveValues[4] < 6 || LastFiveValues[4] > 15) ? Brushes.Red : Brushes.Gray;
                    


                    if (LastFiveDateTime[0] != DateTime.MinValue.ToString())
                        TimeLine_Label1 = LastFiveDateTime[0];
                    else
                    {
                        TimeLine_Label1 = "";

                    }
                    if (LastFiveDateTime[1] != DateTime.MinValue.ToString())
                        TimeLine_Label2 = LastFiveDateTime[1];
                    else {
                        TimeLine_Label2 = "";

                    }
                    if (LastFiveDateTime[2] != DateTime.MinValue.ToString())
                        TimeLine_Label3 = LastFiveDateTime[2];
                    else
                    {
                        TimeLine_Label3 = "";

                    }
                    if (LastFiveDateTime[3] != DateTime.MinValue.ToString())
                        TimeLine_Label4 = LastFiveDateTime[3];
                    else
                    {
                        TimeLine_Label4 = "";

                    }
                    if (LastFiveDateTime[4] != DateTime.MinValue.ToString())
                        TimeLine_Label5 = LastFiveDateTime[4];
                    else
                    {
                        TimeLine_Label5 = "";

                    }
                }
            }
        }

        public void UpdateValues(int id)
        {
            foreach (var entity in AllEntities)
            {
                if (entity.Id == id)
                {
                    LastFiveValues.Clear();
                    LastFiveValues.Add(0);
                    LastFiveValues.Add(0);
                    LastFiveValues.Add(0);
                    LastFiveValues.Add(0);
                    LastFiveValues.Add(0);

                    LastFiveDateTime.Clear();
                    LastFiveDateTime.Add(DateTime.MinValue.ToString());
                    LastFiveDateTime.Add(DateTime.MinValue.ToString());
                    LastFiveDateTime.Add(DateTime.MinValue.ToString());
                    LastFiveDateTime.Add(DateTime.MinValue.ToString());
                    LastFiveDateTime.Add(DateTime.MinValue.ToString());

                    int i = entity.Value.Count - 1;
                    for (int j = 4; j >= 0; j--)
                    {
                        LastFiveValues[j] = entity.Value[i];
                        i--;
                    }

                    i = entity.TimelineValues.Count - 1;
                    for (int j = 4; j >= 0; j--)
                    {
                        LastFiveDateTime[j] = entity.TimelineValues[i];
                        i--;
                    }
                }
            }
        }
    }
}
