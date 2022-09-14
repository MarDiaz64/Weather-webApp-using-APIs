using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace Restful_L5
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IService1" in both code and config file together.
    [ServiceContract]
    public interface IService1
    {
        [OperationContract]
        [WebGet(UriTemplate = "forecast?zipcode={zipcode}", ResponseFormat = WebMessageFormat.Json)]
        Output PollutionForecast(string zipcode);

        [OperationContract]
        [WebGet(UriTemplate = "current?zipcode={zipcode}", ResponseFormat = WebMessageFormat.Json)]
        Output CurrentPollution(string zipcode);
    }
    public class Output
    {
        public location lo;
        public Day[] days;
        public string PollutanUnits { get; set; }
    }
    public class Day
    {
        public int dt { get; set; }
        public measurement[] values;
    }
    public class location
    {
        public string zipcode { get; set; }
        public string name { get; set; }
        public double lat { get; set; }
        public double lon { get; set; }
    }
    public class measurement
    {
        public string name;
        public double value;
        public int index; //use index ranges to calculate if its higher than it should be
        public string indexStr;
    }
}
