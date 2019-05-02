using System;
using System.IO;
using System.Net;
using Newtonsoft.Json;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace AirlineApp
{
    class Program
    {
        static void Main(string[] args)
        {
            System.Globalization.CultureInfo customCulture = (System.Globalization.CultureInfo)System.Threading.Thread.CurrentThread.CurrentCulture.Clone();
            customCulture.NumberFormat.NumberDecimalSeparator = ".";

            System.Threading.Thread.CurrentThread.CurrentCulture = customCulture;
            var App = new Program();


            while (true)
            {
                App.PollWeather();
                System.Threading.Thread.Sleep(2000);
            }
        }

        void PollWeather()
        {
            string html = string.Empty;
            string url = @"http://api.openweathermap.org/data/2.5/weather?q=Dubai&APPID=ac5f1c95fa32d2539f1a4b6683e5ec7e&units=metric";

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.AutomaticDecompression = DecompressionMethods.GZip;

            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            using (Stream stream = response.GetResponseStream())
            using (StreamReader reader = new StreamReader(stream))
            {
                html = reader.ReadToEnd();
            }

            var WeatherData = JsonConvert.DeserializeObject<OpenWeather>(html);
            Console.WriteLine("Data from OWM: " + WeatherData.Main.temp);


            Console.WriteLine("Posting data to WSO...");
            string url2 = "http://localhost:8081/WeatherData";

            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(url2);
            client.DefaultRequestHeaders
                  .Accept
                  .Add(new MediaTypeWithQualityHeaderValue("application/json"));

            HttpRequestMessage request2 = new HttpRequestMessage(HttpMethod.Post, "");
            request2.Content = new StringContent("{\"temp\":" + WeatherData.Main.temp + "}",
                                                Encoding.UTF8,
                                                "application/json");

            var response2 = client.SendAsync(request2).Result;
        }

        public class OpenWeather
        {
            [JsonProperty("id")]
            public string Id { get; set; }

            [JsonProperty("name")]
            public string Name { get; set; }

            [JsonProperty("main")]
            public main Main { get; set; }
        }

        public class main
        {
            public float temp { get; set; }
            public int pressure { get; set; }
            public int humidity { get; set; }
            public float temp_min { get; set; }
            public float temp_max { get; set; }
        }
    }
}
