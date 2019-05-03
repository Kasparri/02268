using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace FlightService
{
    class Program
    {
        private static readonly HttpClient client = new HttpClient();

        private const string _camundaEngineAPI = "http://localhost:8080/engine-rest";
        private const string _wso2FlightStreamUrl = "http://localhost:8081/bookingFlightStream";
        private const string _flightId = "3c675a";

        static void Main(string[] args)
        {
            Console.WriteLine("Press any key to get flight information from opensky-network...");
            Console.Read();
            dynamic flightInfo = JsonConvert.DeserializeObject(GetFlightInformation(_flightId).Result);
            Console.WriteLine("Flight info returned");


            Console.WriteLine("Press any key to send flight information to WSO2 Processor");
            Console.Read();
            SendFlightInformationToWSO2(flightInfo[0]).Wait();
            Console.WriteLine("Flight info sent to WSO2 Stream Processor");

            Console.WriteLine("Press any key to send message event to camunda");
            Console.ReadLine();
            NotifyCamundaComplete().Wait();
            Console.WriteLine("Camunda notified...");
            Console.ReadLine();

        }

        private static async Task<string> GetFlightInformation(string flightId)
        {
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders
              .Accept
              .Add(new MediaTypeWithQualityHeaderValue("application/json"));

            return await client.GetStringAsync($"https://opensky-network.org/api/flights/aircraft?icao24={flightId}&begin=1517184000&end=1517270400");
        }

        private static async Task SendFlightInformationToWSO2(dynamic flightData)
        {
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders
              .Accept
              .Add(new MediaTypeWithQualityHeaderValue("application/json"));

            string flightDataSerialized = JsonConvert.SerializeObject(flightData);

            string body = "{";
            body += " \"bookingId\": 2, ";
            body += $" \"flightData\": {flightDataSerialized} ";
            body += "}";

            await client.PostAsync($"{_wso2FlightStreamUrl}", new StringContent(body));

        }

        private static async Task NotifyCamundaComplete()
        {
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders
              .Accept
              .Add(new MediaTypeWithQualityHeaderValue("application/json"));

            string json = "{ \"messageName\": \"FlightInfoMessageEvent\" }";

            await client.PostAsync($"{_camundaEngineAPI}/message", new StringContent(json, Encoding.UTF8, "application/json"));
        }
    }
}
