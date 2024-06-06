using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkService.Model
{
    public class Filter
    {
        public int Id { get; set; } = 0;
        public string Operation { get; set; } = string.Empty;
        public EntityType Type { get; set; } = null;

        public Filter() { }

        public bool FilterEntity(Valve valve)
        {
            bool retVal = true;
            if(Operation != String.Empty)
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

            if(Type != null && retVal)
            {
                if(!(valve.Type.Name == Type.Name))
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
                if(Type.Name == Model.Type.DigitalManometer)
                {
                    retVal += "Type:DM";
                }
                else
                {
                    retVal += "Type:CS";
                }


            }

            return retVal;

        }

    }
}
