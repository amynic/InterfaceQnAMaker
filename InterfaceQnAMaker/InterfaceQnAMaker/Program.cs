using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using Newtonsoft.Json;
using System.Net;

namespace InterfaceQnAMaker
{
    class Program
    {
        //Admin app to update QnA maker service with new knowledge
       

        //get QnA maker credentials
        static string knowledgeBaseID = ConfigurationSettings.AppSettings["id"];
        static string qnaMakerSubscriptionKey = ConfigurationSettings.AppSettings["subscriptionkey"];

        //Menu options

        static void Main(string[] args)
        {

            Run();

        }

        static async Task Run()
        {
            Console.ResetColor();
            var exit = false;
            string output = string.Empty;
            string query = string.Empty;

            try
            {
                Console.WriteLine();
                Console.WriteLine("What do you want to do?");
                Console.WriteLine("=================================================================");
                Console.WriteLine("1. Query QnA Maker");
                Console.WriteLine("2. Add to the QnA Maker knowledge base");
                Console.WriteLine();

                var key = Console.ReadKey(true);

                switch (key.KeyChar)
                {
                    case '1':
                        Console.WriteLine("Please enter a query:");
                        query = Console.ReadLine();
                        output = queryService(query, knowledgeBaseID, qnaMakerSubscriptionKey);
                        Console.WriteLine(output);
                        await Run();
                        break;
                    case '2':
                        

                        await Run();
                        break;
                    
                    default:
                        Console.WriteLine("Press any key to exit..");
                        exit = true;
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Ooops, something broke: {0}", ex.Message);
                Console.WriteLine();
            }

            if (!exit)
            {
                await Run();
            }
        }

        public static string queryService(string query, string id, string key)
        {
            string response = string.Empty;
            string answer = string.Empty;
            string score = string.Empty;
            QnAMakerResult extractinfo;

            //Build the URI
            Uri qnamakerUriBase = new Uri("https://westus.api.cognitive.microsoft.com/qnamaker/v1.0");
            var builder = new UriBuilder($"{qnamakerUriBase}/knowledgebases/{id}/generateAnswer");

            //Add the question as part of the body
            var postBody = $"{{\"question\": \"{query}\"}}";

            //Send the POST request
            using (WebClient client = new WebClient())
            {
                //Set the encoding to UTF8
                client.Encoding = System.Text.Encoding.UTF8;

                //Add the subscription key header
                client.Headers.Add("Ocp-Apim-Subscription-Key", key);
                client.Headers.Add("Content-Type", "application/json");
                response = client.UploadString(builder.Uri, postBody);
            }

            try
            {
                extractinfo = JsonConvert.DeserializeObject<QnAMakerResult>(response);
                answer = extractinfo.Answer.ToString();
                score = extractinfo.Score.ToString();
            }
            catch
            {
                throw new Exception("Unable to deserialize QnA Maker response string.");
            }

            response = "answer: " + answer + " Score: " + score;
            return response;
        }
    }
}
