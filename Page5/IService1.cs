using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace SolarAPI
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IService1" in both code and config file together.
    [ServiceContract]
    public interface IService1
    {

        [OperationContract]
        [WebGet(UriTemplate = "Solar?zip={zipcode}", ResponseFormat = WebMessageFormat.Json)]
        Output SolarInfo(string zipcode); 
    }

    public class Output
    {
        public int index { get; set; }
        public Avgs avg_dni { get; set; }
        public Avgs avg_ghi { get; set; }
        public Avgs avg_lat_tilt { get; set; }
    }
    public class Avgs
    {
        public double annual { get; set; }
        public Monthly monthly { get; set; }
        public Seasonal seasonally { get; set; }
    }
    public class Seasonal
    {
        public double winter { get; set; }
        public double spring { get; set; }
        public double summer { get; set; }
        public double fall { get; set; }
    }
}
