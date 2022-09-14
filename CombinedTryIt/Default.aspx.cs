using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace CombinedTryIt
{
    public partial class Default : System.Web.UI.Page
    {
        HttpCookie cookie;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Input.Text =="") {
                HttpCookie cookie = Request.Cookies["myCookie"];
                if (cookie != null)
                {
                    Input.Text = cookie["input"];
                    if (cookie["ERROR"] != "e")
                    {
                        Day1.Text = cookie["DAY1"].Replace("%0a","\n");
                        Day2.Text = cookie["DAY2"].Replace("%0a", "\n");
                        Day3.Text = cookie["DAY3"].Replace("%0a", "\n");
                        Day4.Text = cookie["DAY4"].Replace("%0a", "\n");
                        Day5.Text = cookie["DAY5"].Replace("%0a", "\n");
                        ImgDay1.ImageUrl = cookie["IMGDAY1"];
                        ImgDay2.ImageUrl = cookie["IMGDAY2"];
                        ImgDay3.ImageUrl = cookie["IMGDAY3"];
                        ImgDay4.ImageUrl = cookie["IMGDAY4"];
                        ImgDay5.ImageUrl = cookie["IMGDAY5"];
                        ghi.Text = cookie["GHI"];
                        dni.Text = cookie["DNI"];
                        tilt.Text = cookie["TILT"];
                        AQ.Text = cookie["AQ"];
                        Location.Text = cookie["Location"];

                    }
                }

            }

        }
        /*Daily Maximum Temperaturein F: 62
Daily Minimum Temperaturein F: 42
Probability of Precipitation in %: 3
Weather Type, Coverage, and Intensity: Weather - summary: Mostly Cloudy
Conditions Icons: http://forecast.weather.gov/images/wtf/bkn.jpg
Watches, Warnings, and Advisories: */
        protected Forecast[] parseForecast(string[] forcast) {
            Forecast[] data = new Forecast[5];
            data[0] = new Forecast();
            data[1] = new Forecast();
            data[2] = new Forecast();
            data[3] = new Forecast();
            data[4] = new Forecast();
            for (int i=0; i<5; i++) {
                string[] split = forcast[i].Split(new string[] { "\n" }, StringSplitOptions.None);
                data[i].tempMax = Int32.Parse(Regex.Matches(split[0], @"[+-]?\d+(\.\d+)?")[0].Value);
                data[i].tempMin = Int32.Parse(Regex.Matches(split[1], @"[+-]?\d+(\.\d+)?")[0].Value);
                data[i].precip = Int32.Parse(Regex.Matches(split[2], @"[+-]?\d+(\.\d+)?")[0].Value);
                string temp = split[3].Substring(split[3].IndexOf(":")+1);
                data[i].summary = temp.Substring(temp.IndexOf(":")+1).Trim();
                data[i].image = split[4].Substring(split[4].IndexOf(":")+1).Trim();
                data[i].hazards = split[5].Substring(split[5].IndexOf(":")+1).Trim();
            }
            return data;
        }
        protected void fiveDayForecast(string zip)
        {
            WeatherService.Service1Client client = new WeatherService.Service1Client();
            string[] output = client.Weather5day(zip);
            if (output == null)
            {
                cookie["ERROR"] = "e";
                Input.BorderColor = System.Drawing.Color.Red;
                Input.BorderWidth = 2;
                Err.Attributes.CssStyle["display"] = "block";
            }
            else
            {
                Forecast[] temp = parseForecast(output);
                //Forecasts:
                Day1.Text = temp[0].summary + "\n" + temp[0].tempMin + "/" + temp[0].tempMax + "F\nPrecipitation:" + temp[0].precip + "%";
                Day2.Text = temp[1].summary + "\n" + temp[1].tempMin + "/" + temp[1].tempMax + "F\nPrecipitation:" + temp[1].precip + "%";
                Day3.Text = temp[2].summary + "\n" + temp[2].tempMin + "/" + temp[2].tempMax + "F\nPrecipitation:" + temp[2].precip + "%";
                Day4.Text = temp[3].summary + "\n" + temp[3].tempMin + "/" + temp[3].tempMax + "F\nPrecipitation:" + temp[3].precip + "%";
                Day5.Text = temp[4].summary + "\n" + temp[4].tempMin + "/" + temp[4].tempMax + "F\nPrecipitation:" + temp[4].precip + "%";

                //Images For Forecasts:
                ImgDay1.ImageUrl = temp[0].image;
                ImgDay2.ImageUrl = temp[1].image;
                ImgDay3.ImageUrl = temp[2].image;
                ImgDay4.ImageUrl = temp[3].image;
                ImgDay5.ImageUrl = temp[4].image;

                //cookie stuff:
                cookie["DAY1"] = Day1.Text;
                cookie["DAY2"] = Day2.Text;
                cookie["DAY3"] = Day3.Text;
                cookie["DAY4"] = Day4.Text;
                cookie["DAY5"] = Day5.Text;
                cookie["IMGDAY1"] = ImgDay1.ImageUrl;
                cookie["IMGDAY2"] = ImgDay2.ImageUrl;
                cookie["IMGDAY3"] = ImgDay3.ImageUrl;
                cookie["IMGDAY4"] = ImgDay4.ImageUrl;
                cookie["IMGDAY5"] = ImgDay5.ImageUrl;
            }
        }
        protected void currentPollution(string zip) {
            string url = @"http://localhost:57214/Service1.svc/current?zipcode=" + zip;
            HttpWebRequest request2 = (HttpWebRequest)WebRequest.Create(url);
            WebResponse response2 = request2.GetResponse();
            Stream dataStream2 = response2.GetResponseStream();
            StreamReader sreader2 = new StreamReader(dataStream2);
            var responsereader2 = sreader2.ReadToEnd();
            response2.Close();
            string temp = responsereader2;
            if (temp.Equals(""))
            {
                cookie["ERROR"] = "e";
                Input.BorderColor = System.Drawing.Color.Red;
                Input.BorderWidth = 2;
                Err.Attributes.CssStyle["display"] = "block";
            }
            else
            {
                PolOutput temp1 = JsonConvert.DeserializeObject<PolOutput>(responsereader2);
                //add more display info her
                AQ.Text = temp1.days[0].values[0].indexStr;
                Location.Text = temp1.lo.name + ", " + temp1.lo.zipcode + ",U.S.";
                cookie["AQ"] = AQ.Text;
                cookie["Location"] = Location.Text;
            }
        }
        protected void solarInformation(string zip) {
            string url = @"http://localhost:50089/Service1.svc/Solar?zip=" + zip;
            HttpWebRequest request2 = (HttpWebRequest)WebRequest.Create(url);
            WebResponse response2 = request2.GetResponse();
            Stream dataStream2 = response2.GetResponseStream();
            StreamReader sreader2 = new StreamReader(dataStream2);
            string responsereader2 = sreader2.ReadToEnd();
            response2.Close();
            string temp = responsereader2;
            if (temp.Equals(""))
            {
                cookie["ERROR"] = "e";
                Input.BorderColor = System.Drawing.Color.Red;
                Input.BorderWidth = 2;
                Err.Attributes.CssStyle["display"] = "block";
            }
            else
            {
                SunOutput temp1 = JsonConvert.DeserializeObject<SunOutput>(responsereader2);
                ghi.Text = temp1.avg_ghi.annual.ToString();
                dni.Text = temp1.avg_dni.annual.ToString();
                tilt.Text = temp1.avg_lat_tilt.annual.ToString();
                cookie["GHI"] = ghi.Text;
                cookie["DNI"] = dni.Text;
                cookie["TILT"] = tilt.Text;
            }
        }

        protected void ImageButton1_Click(object sender, ImageClickEventArgs e)
        {
            //create cookie:
            cookie = new HttpCookie("myCookie");
            cookie["input"] = Input.Text;
            cookie["ERROR"] = "a";
            cookie.Expires = DateTime.Now.AddMinutes(1);
            //rest of the actions
            Input.BorderColor = System.Drawing.Color.Black;
            Input.BorderWidth = 1;
            Err.Attributes.CssStyle["display"] = "none";
            fiveDayForecast(Input.Text);
            solarInformation(Input.Text);
            currentPollution(Input.Text);

            //finish cookie
            Response.Cookies.Add(cookie);
        }
    }
    //collect forecast information:
    public class Forecast {
        public int tempMax { get; set; }
        public int tempMin { get; set; }
        public int precip { get; set; }
        public string summary { get; set; }
        public string image { get; set; }
        public string hazards { get; set; }
    }
    // Deserialize Solar information:
    public class SunOutput
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
    //deserialize pollution information
    public class PolOutput
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
        public int index; 
        public string indexStr;
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