using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace NetworkService.Model
{
    public enum Type
    {
        CableSensor,
        DigitalManometer
    }
    public class EntityType
    {
        public Type Name {  get; set; }
        public string PicturePath {  get; set; }

        public EntityType(Type name)
        {
            Name = name;
            if(name == Type.CableSensor)
            {
                PicturePath = "C:\\Users\\milos\\Documents\\Faculty\\6. Semestar\\HCI\\HCI_PZ2\\NetworkService\\NetworkService\\NetworkService\\NetworkService\\NetworkService\\Images\\CableSensor.jpg";

            }
            else
            {
                PicturePath = "C:\\Users\\milos\\Documents\\Faculty\\6. Semestar\\HCI\\HCI_PZ2\\NetworkService\\NetworkService\\NetworkService\\NetworkService\\NetworkService\\Images\\DigitalManometer.jpg";
            }
        }

    }
}
