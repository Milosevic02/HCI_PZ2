using NetworkService.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using NetworkService;
using System.Windows;
using System.Diagnostics;
using MVVMLight.Messaging;
using NetworkService.Model;
using System.Collections.ObjectModel;
using System.IO;

namespace NetworkService.ViewModel
{
    public class MainWindowViewModel : BindableBase
    {
        private int count = 3; // Inicijalna vrednost broja objekata u sistemu
                                // ######### ZAMENITI stvarnim brojem elemenata
                                //           zavisno od broja entiteta u listi
        public GraphView g;
        public TableView t;

        public ICommand MyButtonClickCommand { get; private set; }
        public static ObservableCollection<Valve> Valves { get; set; }
        public static ObservableCollection<string> ValveNames { get {
                return new ObservableCollection<string>(Valves.Select(v => v.Name));
            } }


        private void MyButtonClick(string viewModelName)
        {

            switch (viewModelName)
            {
                case "GraphViewModel":
                    //ValveNames = new ObservableCollection<string>(Valves.Select(v => v.Name));
                    CurrentView = g;
                    break;
                case "TableViewModel":
                    CurrentView = t;
                    break;
                default:
                    break;
            }

        }

        private UserControl currentView;
        public UserControl CurrentView
        {
            get
            {
                return currentView;
            }

            set
            {
                SetProperty(ref currentView, value);
            }
        }
        public MyICommand<Window> CloseWindowCommand { get; private set; }
        private void CloseWindow(Window MainWindow)
        {
            foreach (var process in Process.GetProcessesByName("MeteringSimulator")) // Zameni sa stvarnim imenom aplikacije bez ekstenzije
            {
                process.Kill();
                process.WaitForExit();
            }
            MainWindow.Close();
        }

        public MainWindowViewModel()
        {
            
            Messenger.Default.Register<int>(this, "Count", UpdateCount);
            Valves = new ObservableCollection<Valve>();
            Valves.Add(new Valve { Id = 50, Name = "Valve_50", Value = 7, Type = new EntityType(Model.Type.CableSensor) });
            Valves.Add(new Valve { Id = 51, Name = "Valve_51", Value = 8, Type = new EntityType(Model.Type.CableSensor) });
            Valves.Add(new Valve { Id = 52, Name = "Valve_52", Value = 9, Type = new EntityType(Model.Type.DigitalManometer) });
            g = new GraphView();
            t = new TableView();
            CurrentView = t;
            MyButtonClickCommand = new MyICommandWithParameter<string>(MyButtonClick);
            CloseWindowCommand = new MyICommand<Window>(CloseWindow);
            createListener(); //Povezivanje sa serverskom aplikacijom

        }

        private void UpdateCount(int c)
        {
            count = c;
        }

        private void createListener()
        {
            var tcp = new TcpListener(IPAddress.Any, 25675);
            tcp.Start();

            var listeningThread = new Thread(() =>
            {
                while (true)
                {
                    var tcpClient = tcp.AcceptTcpClient();
                    ThreadPool.QueueUserWorkItem(param =>
                    {
                        //Prijem poruke
                        NetworkStream stream = tcpClient.GetStream();
                        string incomming;
                        byte[] bytes = new byte[1024];
                        int i = stream.Read(bytes, 0, bytes.Length);
                        //Primljena poruka je sacuvana u incomming stringu
                        incomming = System.Text.Encoding.ASCII.GetString(bytes, 0, i);

                        //Ukoliko je primljena poruka pitanje koliko objekata ima u sistemu -> odgovor
                        if (incomming.Equals("Need object count"))
                        {
                            //Response
                            /* Umesto sto se ovde salje count.ToString(), potrebno je poslati 
                             * duzinu liste koja sadrzi sve objekte pod monitoringom, odnosno
                             * njihov ukupan broj (NE BROJATI OD NULE, VEC POSLATI UKUPAN BROJ)
                             * */
                            Byte[] data = System.Text.Encoding.ASCII.GetBytes(count.ToString());
                            stream.Write(data, 0, data.Length);
                        }
                        else
                        {
                            //U suprotnom, server je poslao promenu stanja nekog objekta u sistemu
                            Console.WriteLine(incomming); //Na primer: "Entitet_1:272"
                            Messenger.Default.Send<string>(incomming, "ValueFromSimulator");

                            //################ IMPLEMENTACIJA ####################
                            // Obraditi poruku kako bi se dobile informacije o izmeni
                            // Azuriranje potrebnih stvari u aplikaciji

                        }
                    }, null);
                }
            });

            listeningThread.IsBackground = true;
            listeningThread.Start();
        }

        public static IEnumerable<string> ReadDataBackwards(string filePath)
        {
            Stack<string> lines = new Stack<string>();

            using (FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            using (StreamReader reader = new StreamReader(fs))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    lines.Push(line);
                }
            }

            while (lines.Count > 0)
            {
                yield return lines.Pop();
            }
        }
    }
}
