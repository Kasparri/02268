using System;
using System.IO;
using System.Net;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace AirlineApp
{
    class Program
    {
        static void Main(string[] args)
        {
            var App = new Program();

            App.StartProcess();

            while (true)
            {
                App.PollCamunda();
                System.Threading.Thread.Sleep(4000);
            }


        }

        void StartProcess()
        {
            Console.WriteLine("Sending start message...");
            //POST SEND MESSAGE
            string url2 = "http://localhost:8080/engine-rest/message";



            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(url2);
            client.DefaultRequestHeaders
                  .Accept
                  .Add(new MediaTypeWithQualityHeaderValue("application/json"));//ACCEPT header

            HttpRequestMessage request2 = new HttpRequestMessage(HttpMethod.Post, "");
            request2.Content = new StringContent("{\"messageName\":\"StartProcess\",\"businessKey\":\"1\"}",
                                                Encoding.UTF8,
                                                "application/json");//CONTENT-TYPE header

            var response = client.SendAsync(request2).Result;
        }

        void PollCamunda()
        {
            string html = string.Empty;
            string url = @"http://localhost:8080/engine-rest/task?processDefinitionKey=Process_0twgpq8";

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.AutomaticDecompression = DecompressionMethods.GZip;

            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            using (Stream stream = response.GetResponseStream())
            using (StreamReader reader = new StreamReader(stream))
            {
                html = reader.ReadToEnd();
            }

            var yo = JsonConvert.DeserializeObject<List<AirlineTask>>(html);

            if (yo.Any())
            {
                Console.WriteLine("Press enter to complete task \"" + yo.FirstOrDefault().Name + "\"");
                Console.ReadLine();
                var id = yo.FirstOrDefault().Id;
                Console.WriteLine("Completing " + yo.FirstOrDefault().Name + "...");
                string url2 = "http://localhost:8080/engine-rest/task/" + id + "/complete";



                HttpClient client = new HttpClient();
                client.BaseAddress = new Uri(url2);
                client.DefaultRequestHeaders
                      .Accept
                      .Add(new MediaTypeWithQualityHeaderValue("application/json"));

                HttpRequestMessage request2 = new HttpRequestMessage(HttpMethod.Post, "");
                request2.Content = new StringContent("{\"variables\":{\"credCheck\":{\"value\":\"Rexjected\"}}}",
                                                    Encoding.UTF8,
                                                    "application/json");

                var response = client.SendAsync(request2).Result;
            }
            else
            {
                string url2 = "http://localhost:8080/engine-rest/message";



                HttpClient client = new HttpClient();
                client.BaseAddress = new Uri(url2);
                client.DefaultRequestHeaders
                      .Accept
                      .Add(new MediaTypeWithQualityHeaderValue("application/json"));

                HttpRequestMessage request2 = new HttpRequestMessage(HttpMethod.Post, "");
                request2.Content = new StringContent("{\"messageName\":\"Payment\",\"businessKey\":\"1\"}",
                                                    Encoding.UTF8,
                                                    "application/json");

                var response = client.SendAsync(request2).Result;
            }

        }

        public class AirlineTask
        {
            [JsonProperty("id")]
            public string Id { get; set; }

            [JsonProperty("name")]
            public string Name { get; set; }
        }
    }
}
