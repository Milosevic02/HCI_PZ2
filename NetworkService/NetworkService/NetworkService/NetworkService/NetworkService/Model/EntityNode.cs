using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkService.Model
{
    public class EntityNode : BindableBase
    {
        private string _type;
        private ObservableCollection<Valve> _entitiesSameType;
        public string Type
        {
            get { return _type; }
            set { _type = value; OnPropertyChanged(nameof(Type)); }
        }
        public ObservableCollection<Valve> EntitiesSameType
        {
            get { return _entitiesSameType; }
            set { _entitiesSameType = value; OnPropertyChanged(nameof(EntitiesSameType)); }
        }

        public EntityNode(Type type)
        {
            Type = type.ToString();
            EntitiesSameType = new ObservableCollection<Valve>();
        }

        public override string ToString()
        {
            string s = Type.ToString() + "\n\n";
            /* foreach(var item in serverViewModels)
             {
                 s += item.ToString() + "\n";
             }*/
            return s;
        }
    }
}