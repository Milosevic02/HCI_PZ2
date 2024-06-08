using Notifications.Wpf.Annotations;
using System;
using System.Collections.Generic;
using System.IO.Packaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkService.Model
{
    public class Filter
    {
        public int Id { get; set; } = 0;
        public string Operation { get; set; } = string.Empty;
        public string Type { get; set; } = null;

        public Filter() { }

        public bool FilterEntity(Entity valve)
        {
            bool retVal = true;
            if (Operation != String.Empty)
            {
                switch (Operation)
                {
                    case "More":
                        retVal = valve.Id > Id;
                        break;
                    case "Less":
                        retVal = valve.Id < Id;
                        break;
                    case "Equals":
                        retVal = valve.Id == Id;
                        break;
                    default:
                        retVal = false;
                        break;

                }
            }

            if (Type != null && retVal && Type != "All")
            {
                if (!(valve.Type.ToString() == Type))
                {
                    retVal = false;
                }

            }

            return retVal;
        }

        public string GetName()
        {
            string retVal = "";
            if (Operation != String.Empty)
            {
                string id = Id.ToString();
                switch (Operation)
                {
                    case "More":
                        retVal += "More then " + id + " ";
                        break;
                    case "Less":
                        retVal += "Less then " + id + " ";
                        break;
                    case "Equals":
                        retVal += "Equals with  " + id + " ";
                        break;
                    default:
                        retVal = "Error";
                        break;
                }
            }

            if (Type != null)
            {
                if (Type == Model.Type.Digital_Manometer.ToString())
                {
                    retVal += "Type:DM";
                }
                else if(Type == Model.Type.Cable_Sensor.ToString())
                {
                    retVal += "Type:CS";
                }


            }

            return retVal;

        }
    }
}
