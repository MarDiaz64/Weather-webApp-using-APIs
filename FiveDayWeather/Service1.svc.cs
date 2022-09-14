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
using System.Xml;
using System.Xml.Serialization;

namespace _5DayForecast
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Service1" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select Service1.svc or Service1.svc.cs at the Solution Explorer and start debugging.
    public class Service1 : IService1
    {
        public string[] Weather5day(string zipcode)
        {
            WeatherService.ndfdXML client = new WeatherService.ndfdXML();
            XmlDocument doc = new XmlDocument();
            string xml;
            try
            {
                Int32.Parse(zipcode); //should throw error if its not a number
                xml = client.LatLonListZipCode(zipcode);
            }
            catch
            {
                return null;
            }
            doc.LoadXml(xml);

            XmlNodeList laloEl = doc.GetElementsByTagName("latLonList");
            string[] latLon = (laloEl[0].InnerText).Split(',');
            if (latLon[0].Length == 0) {
                return null;
            }
            decimal lat = Convert.ToDecimal(latLon[0]);
            decimal lon = Convert.ToDecimal(latLon[1]);

            //results
            string[] result = new string[5];

            //get one more day because for some reason last variable turns out null in xml response temperatures?? 
            DateTime start = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 20);
            xml = client.NDFDgenByDay(lat, lon, start, "6", "e", "24 hourly");
            doc = new XmlDocument();
            doc.LoadXml(xml);
            XmlNodeList nodes = (doc.GetElementsByTagName("parameters"))[0].ChildNodes;
            XmlNodeList body;
            
            //low temp
            body= nodes[0].ChildNodes;
            for (int i = 0; i < result.Length; i++) {
                result[i] += body[0].InnerText + "in F: ";
            }
            for (int i = 1; i < 6; i++)
            {
                int j = i - 1;
                result[j] += body[i].InnerText;
            }

            //high temp
            body = nodes[1].ChildNodes;
            for (int i = 0; i < result.Length; i++)
            {
                result[i] += "\n"+ body[0].InnerText + "in F: ";
            }
            for (int i = 1; i < 6; i++)
            {
                int j = i - 1;
                result[j] += body[i].InnerText;
            }
            //12-hour % precipitation temp
            body = nodes[2].ChildNodes;
            for (int i = 0; i < result.Length; i++)
            {
                result[i] += "\nProbability of Precipitation in %: ";
            }
            int k= 0;
            for (int i = 1; i < 11; i+=2 )// every other value for one value per day
            {
                result[k] += body[i].InnerText;
                k++;
            }
            //weather
            body = nodes[3].ChildNodes;
            for (int i = 0; i < result.Length; i++)
            {
                result[i] += "\n" + body[0].InnerText + ": "; ;
            }
            for (int i = 1; i < 6; i ++)
            {
                int j = i - 1;
                result[j] += "\tWeather-summary: " + body[i].Attributes["weather-summary"].Value.ToString();
            }
            //icons
            body = nodes[4].ChildNodes;
            for (int i = 0; i < result.Length; i++)
            {
                result[i] += "\n" + body[0].InnerText + ": "; ;
            }
            for (int i = 1; i < 6; i++)
            {
                int j = i - 1;
                result[j] += body[i].InnerText;
            }
            //Hazards
            body = nodes[5].ChildNodes;
            for (int i = 0; i < result.Length; i++)
            {
                result[i] += "\n" + body[0].InnerText + ": "; ;
            }
            if (body[1].ChildNodes.Count > 0)
            {
                body = body[1].ChildNodes;
                for (int i = 0; i < body.Count; i++)
                {
                    for (int j = 0; j < result.Length; j++)
                    {
                        try
                        {

                            result[j] += "\n\tphenomena: " + body[i].Attributes["phenomena"].Value.ToString() +
                                "\n\t\thazardCode: " + body[i].Attributes["hazardCode"].Value.ToString() +
                                "\n\t\tsignificance: " + body[i].Attributes["significance"].Value.ToString() +
                                "\n\t\thazardtype: " + body[i].Attributes["hazardType"].Value.ToString() +
                                "\n\t\ttextURL: " + body[i].ChildNodes[0].InnerText;
                        }
                        catch
                        {
                            result[j] = "";
                        }

                    }

                }
            }
            return result;
           
        }
    }
}
