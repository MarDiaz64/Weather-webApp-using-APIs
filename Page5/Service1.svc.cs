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

namespace SolarAPI
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Service1" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select Service1.svc or Service1.svc.cs at the Solution Explorer and start debugging.
    public class Service1 : IService1
    {
        private string APIKEY = "OAXDTL80Ov7G4SDD8vBeaIKy6PFapTnUHlMeqFIS";
        private string APIKEYGEO = "f72d436ef496d500cfa80e4ba6e89daf";
        public Output SolarInfo(string zipcode)
        {
            GeoData g = zipToLatLon(zipcode);
            if (g==null) {
                //return "INVALID ZIPCODE";
                return null;
            }
            string url = @"https://developer.nrel.gov/api/solar/solar_resource/v1.json?api_key="+APIKEY+"&lat="+g.lat+"&lon="+g.lon;

            SunData obj;
            try
            {
                HttpWebRequest request2 = (HttpWebRequest)WebRequest.Create(url);
                WebResponse response2 = request2.GetResponse();
                Stream dataStream2 = response2.GetResponseStream();
                StreamReader sreader2 = new StreamReader(dataStream2);
                string responsereader2 = sreader2.ReadToEnd();
                response2.Close();
                obj = JsonConvert.DeserializeObject<SunData>(responsereader2);
            }
            catch {
                return null; //if there is an error processing request, return null (shouldn't happen but catch it just in case)
            }

            if (obj == null) {//json conversion error catch (just in case)
                return null;
            }
            Output o = new Output();
            //5= high,good for solar pannels, 1=low, bad for solar pannels
            if (obj.outputs.avg_ghi.annual > 6) {
                o.index = 5;
            }
            else if (obj.outputs.avg_ghi.annual > 5) {
                o.index = 4;
            }
            else if (obj.outputs.avg_ghi.annual > 4) {
                o.index = 3;
            }
            else if (obj.outputs.avg_ghi.annual > 3)
            {
                o.index = 2;
            }
            else {
                o.index = 1;
            }
            o.avg_dni = new Avgs();
            o.avg_ghi = new Avgs();
            o.avg_lat_tilt = new Avgs();
            o.avg_dni.annual = obj.outputs.avg_dni.annual;
            o.avg_ghi.annual= obj.outputs.avg_ghi.annual;
            o.avg_lat_tilt.annual= obj.outputs.avg_lat_tilt.annual;
            o.avg_dni.monthly = obj.outputs.avg_dni.monthly;
            o.avg_ghi.monthly = obj.outputs.avg_ghi.monthly;
            o.avg_lat_tilt.monthly = obj.outputs.avg_lat_tilt.monthly;

            //seasonal data:
            o.avg_dni.seasonally = new Seasonal();
            o.avg_ghi.seasonally = new Seasonal();
            o.avg_lat_tilt.seasonally = new Seasonal();
            o.avg_dni.seasonally.winter = Math.Round(((obj.outputs.avg_dni.monthly.dec + obj.outputs.avg_dni.monthly.jan + obj.outputs.avg_dni.monthly.feb) / 3),2);
            o.avg_dni.seasonally.spring = Math.Round(((obj.outputs.avg_dni.monthly.mar + obj.outputs.avg_dni.monthly.apr + obj.outputs.avg_dni.monthly.may) / 3),2);
            o.avg_dni.seasonally.summer = Math.Round(((obj.outputs.avg_dni.monthly.jun + obj.outputs.avg_dni.monthly.jul + obj.outputs.avg_dni.monthly.aug) / 3),2);
            o.avg_dni.seasonally.fall = Math.Round(((obj.outputs.avg_dni.monthly.sep + obj.outputs.avg_dni.monthly.oct + obj.outputs.avg_dni.monthly.nov) / 3), 2);


            o.avg_ghi.seasonally.winter = Math.Round(((obj.outputs.avg_ghi.monthly.dec + obj.outputs.avg_ghi.monthly.jan + obj.outputs.avg_ghi.monthly.feb) / 3), 2);
            o.avg_ghi.seasonally.spring = Math.Round(((obj.outputs.avg_ghi.monthly.mar + obj.outputs.avg_ghi.monthly.apr + obj.outputs.avg_ghi.monthly.may) / 3), 2);
            o.avg_ghi.seasonally.summer = Math.Round(((obj.outputs.avg_ghi.monthly.jun + obj.outputs.avg_ghi.monthly.jul + obj.outputs.avg_ghi.monthly.aug) / 3), 2);
            o.avg_ghi.seasonally.fall = Math.Round(((obj.outputs.avg_ghi.monthly.sep + obj.outputs.avg_ghi.monthly.oct + obj.outputs.avg_ghi.monthly.nov) / 3),2);

            o.avg_lat_tilt.seasonally.winter = Math.Round(((obj.outputs.avg_lat_tilt.monthly.dec + obj.outputs.avg_lat_tilt.monthly.jan + obj.outputs.avg_lat_tilt.monthly.feb) / 3), 2);
            o.avg_lat_tilt.seasonally.spring = Math.Round(((obj.outputs.avg_lat_tilt.monthly.mar + obj.outputs.avg_lat_tilt.monthly.apr + obj.outputs.avg_lat_tilt.monthly.may) / 3), 2);
            o.avg_lat_tilt.seasonally.summer = Math.Round(((obj.outputs.avg_lat_tilt.monthly.jun + obj.outputs.avg_lat_tilt.monthly.jul + obj.outputs.avg_lat_tilt.monthly.aug) / 3),2);
            o.avg_lat_tilt.seasonally.fall = Math.Round(((obj.outputs.avg_lat_tilt.monthly.sep + obj.outputs.avg_lat_tilt.monthly.oct + obj.outputs.avg_lat_tilt.monthly.nov) / 3),2);
            return o;
        }
        private GeoData zipToLatLon(string zipcode)
        {
            GeoData location;
            //get lat and long from zipcode (geocoding API):
            try {
                string url = @"http://api.openweathermap.org/geo/1.0/zip?zip=" + zipcode + ",US&appid=" + APIKEYGEO;
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                WebResponse response = request.GetResponse();
                Stream dataStream = response.GetResponseStream();
                StreamReader sreader = new StreamReader(dataStream);
                string responsereader = sreader.ReadToEnd();
                response.Close();
                location = JsonConvert.DeserializeObject<GeoData>(responsereader);

            }
            catch // if invalid zipcode, return null
            {
                return null;
            }
            return location;
        }
    }
    public class GeoData
    {
        public string zip { get; set; }
        public string name { get; set; }
        public double lat { get; set; }
        public double lon { get; set; }
        public string country { get; set; }
    }
    public class SunData
    {
        public string version { get; set; }
        public string[] warnings { get; set; }
        public string[] errors { get; set; }
        public MetaData metadata { get; set; }
        public Inputs inputs { get; set; }
        public Outputs outputs { get; set; }
    }

    public class MetaData
    {
        public string[] sources { get; set; }
    }
    public class Inputs
    {
        public string apiKey { get; set; }
        public string lat { get; set; }
        public string lon { get; set; }
    }
    public class Outputs
    {
        public Avg avg_dni { get; set; }
        public Avg avg_ghi { get; set; }
        public Avg avg_lat_tilt { get; set; }
    }
    public class Avg
    {
        public double annual { get; set; }
        public Monthly monthly { get; set; }

    }
    public class Monthly
    {
        public double jan { get; set; }
        public double feb { get; set; }
        public double mar { get; set; }
        public double apr { get; set; }
        public double may { get; set; }
        public double jun { get; set; }
        public double jul { get; set; }
        public double aug { get; set; }
        public double sep { get; set; }
        public double oct { get; set; }
        public double nov { get; set; }
        public double dec { get; set; }
    }
}
