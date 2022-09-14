using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace Restful_L5
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Service1" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select Service1.svc or Service1.svc.cs at the Solution Explorer and start debugging.
    public class Service1 : IService1
    {
        private string APIKEY = "f72d436ef496d500cfa80e4ba6e89daf";
        private string cache;
        private GeoData cachedState;

        // WRAPING THE POLLUTION WEATHERMAP API, with additional usage of the GEOCODING API
        // with additional information added like location detales like:
        // Human readable index:ex:"good" for overall aqi, and individual values according to the aqi chart
        public Output PollutionForecast(string zipcode) {//US ZIPCODES ONLY!
            GeoData location = ZipToLatLon(zipcode);
            if (location == null)
            {
                return null;
            }
            return ProcessRequest(location, 'f',"","");
        }
        public Output CurrentPollution(string zipcode) {
            GeoData location = ZipToLatLon(zipcode);
            if (location == null)
            {
                return null;
            }
            return ProcessRequest(location, 'c',"","");
        }
        private Output ProcessRequest(GeoData location, char type, string start, string end) {
            //get pollution forecast:
            string url;
            if (type == 'f') {
                url = @"http://api.openweathermap.org/data/2.5/air_pollution/forecast?lat=" + location.lat + "&lon=" + location.lon + "&appid=" + APIKEY;
            }
            //get current pollution:
            else{
                url = @"http://api.openweathermap.org/data/2.5/air_pollution?lat=" + location.lat + "&lon=" + location.lon + "&appid=" + APIKEY;
            }
            try
            {
                HttpWebRequest request2 = (HttpWebRequest)WebRequest.Create(url);
                WebResponse response2 = request2.GetResponse();
                Stream dataStream2 = response2.GetResponseStream();
                StreamReader sreader2 = new StreamReader(dataStream2);
                string responsereader2 = sreader2.ReadToEnd();
                response2.Close();
                pollutionData obj = JsonConvert.DeserializeObject<pollutionData>(responsereader2);
                Output temp = ProcessData(obj, location);
                return temp;
            }
            catch {
                return null; //if some error with the request, return null just to be safe
            }

        }

        private GeoData ZipToLatLon(string zipcode) {
            GeoData location;
            //cache the data for faster repeat searches
            if (cache==null || !(zipcode.Equals(cache)))
            {
                //get lat and long from zipcode (geocoding API):
                try
                {
                    string url = @"http://api.openweathermap.org/geo/1.0/zip?zip=" + zipcode + ",US&appid=" + APIKEY;
                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                    WebResponse response = request.GetResponse();
                    Stream dataStream = response.GetResponseStream();
                    StreamReader sreader = new StreamReader(dataStream);
                    string responsereader = sreader.ReadToEnd();
                    response.Close();
                    location = JsonConvert.DeserializeObject<GeoData>(responsereader);
                    cachedState = location;
                    cache = zipcode;
                }
                catch
                {
                    return null;
                }
            }
            else
            {
                location = cachedState;
            }
            return location;
        }
        //helper method too process the data
        private Output ProcessData(pollutionData inputp, GeoData input)
        {
            Output output = new Output();
            output.lo = new location();
            output.lo.zipcode = input.zip;
            output.lo.name = input.name;
            output.lo.lat = input.lat;
            output.lo.lon = input.lon;
            output.PollutanUnits = "μg/m3";
            output.days = new Day[inputp.list.Count];
            for (int i = 0; i < inputp.list.Count; i++) {
                output.days[i] = new Day();

                //general information
                measurement main = new measurement();
                main.name = "General";
                main.value = inputp.list[i].main.aqi;
                main.index = inputp.list[i].main.aqi;
                main.indexStr = HumanReadableIndex(inputp.list[i].main.aqi);
                //individual ones
                measurement co = new measurement();
                co.name = "co";
                co.value = inputp.list[i].components.co;
                co.index = GetIndividualIndex(co.name, co.value);
                co.indexStr = HumanReadableIndex(co.index);

                measurement no = new measurement();
                no.name = "no";
                no.value = inputp.list[i].components.no;
                no.index = GetIndividualIndex(no.name, no.value);
                no.indexStr = HumanReadableIndex(no.index);

                measurement no2 = new measurement();
                no2.name = "no2";
                no2.value = inputp.list[i].components.no2;
                no2.index = GetIndividualIndex(no2.name, no2.value);
                no2.indexStr = HumanReadableIndex(no2.index);

                measurement o3 = new measurement();
                o3.name = "o3";
                o3.value = inputp.list[i].components.o3;
                o3.index = GetIndividualIndex(o3.name, o3.value);
                o3.indexStr = HumanReadableIndex(o3.index);

                measurement so2 = new measurement();
                so2.name = "so2";
                so2.value = inputp.list[i].components.so2;
                so2.index = GetIndividualIndex(so2.name, so2.value);
                so2.indexStr = HumanReadableIndex(so2.index);

                measurement pm2_5 = new measurement();
                pm2_5.name = "pm2_5";
                pm2_5.value = inputp.list[i].components.pm2_5;
                pm2_5.index = GetIndividualIndex(pm2_5.name, pm2_5.value);
                pm2_5.indexStr = HumanReadableIndex(pm2_5.index);

                measurement pm10 = new measurement();
                pm10.name = "pm10";
                pm10.value = inputp.list[i].components.pm10;
                pm10.index = GetIndividualIndex(pm10.name, pm10.value);
                pm10.indexStr = HumanReadableIndex(pm10.index);

                measurement nh3 = new measurement();
                nh3.name = "nh3";
                nh3.value = inputp.list[i].components.nh3;
                nh3.index = GetIndividualIndex(nh3.name, nh3.value);
                nh3.indexStr = HumanReadableIndex(nh3.index);

                output.days[i].dt = inputp.list[i].dt;
                output.days[i].values = new measurement[] { main, co, no, no2, o3, so2, pm2_5, pm10 };
                
            }
            return output;
        }
        private string HumanReadableIndex(int index) {
            switch (index) {
                case 1: return "Good";
                case 2: return "Fair";
                case 3: return "Moderate";
                case 4: return "Poor";
                case 5: return "Very Poor";
            }
            return "";
        }
        /*             NO2     PM10    O3     PM25
            Good	1	0-50	0-25	0-60	0-15
            Fair	2	50-100	25-50	60-120	15-30
            Moder	3	100-200	50-90	120-180	30-55
            Poor	4	200-400	90-180	180-240	55-110
            Very	5	>400	>180	>240	>110*/
        private int GetIndividualIndex(string s, double value) {
            switch (s) {
                case "no2": {
                        if (value < 50)
                        {
                            return 1;
                        }
                        else if (value < 100)
                        {
                            return 2;
                        }
                        else if (value < 200)
                        {
                            return 3;
                        }
                        else if (value < 400)
                        {
                            return 4;
                        }
                        return 5;
                    }
                case "pm10": {
                        if (value < 25)
                        {
                            return 1;
                        }
                        else if (value < 50)
                        {
                            return 2;
                        }
                        else if (value < 90)
                        {
                            return 3;
                        }
                        else if (value < 180)
                        {
                            return 4;
                        }
                        return 5;
                    }
                case "o3": {
                        if (value < 60)
                        {
                            return 1;
                        }
                        else if (value < 120)
                        {
                            return 2;
                        }
                        else if (value < 180)
                        {
                            return 3;
                        }
                        else if (value < 240)
                        {
                            return 4;
                        }
                        return 5;
                    }
                case "pm2_5": {
                        if (value < 15)
                        {
                            return 1;
                        }
                        else if (value < 30)
                        {
                            return 2;
                        }
                        else if (value < 55)
                        {
                            return 3;
                        }
                        else if (value < 110)
                        {
                            return 4;
                        }
                        return 5;
                    }
                default: return 0;
            }
        }

    }

    //~~~~~~~~~~~~~~~~~~~deserialize calls~~~~~~~~~~~~~~~~~~~~~~~//
    public class GeoData
    {
        public string zip { get; set; }
        public string name { get; set; }
        public double lat { get; set; }
        public double lon { get; set; }
        public string country { get; set; }
    }

    public class Main
    {
        public int aqi { get; set; }
    }

    public class Components
    {
        public double co { get; set; }
        public double no { get; set; }
        public double no2 { get; set; }
        public double o3 { get; set; }
        public double so2 { get; set; }
        public double pm2_5 { get; set; }
        public double pm10 { get; set; }
        public double nh3 { get; set; }
    }

    public class ListObj
    {
        public int dt { get; set; }
        public Main main { get; set; }
        public Components components { get; set; }
    }
    public class Coord {
        public double lat { get; set; }
        public double lon { get; set; }
    }

    public class pollutionData
        {
        public Coord coord { get; set; }
        public List<ListObj> list { get; set; } 
    }
}