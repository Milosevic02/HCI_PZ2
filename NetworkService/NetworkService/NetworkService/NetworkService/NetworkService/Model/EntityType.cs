using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            PicturePath = "Images/" + name.ToString() + ".jpg";
        }

    }
}
