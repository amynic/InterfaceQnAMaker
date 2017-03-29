using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using Newtonsoft.Json;
using System.Net;
using System.Net.Http;
using System.IO;

using InterfaceQnAMaker;

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
            string question = string.Empty;
            string answer = string.Empty;

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

                        Console.WriteLine("Please enter a new question:");
                        question = Console.ReadLine();
                        Console.WriteLine("Please enter a new answer:");
                        answer = Console.ReadLine();

                        output = addQnA(knowledgeBaseID, qnaMakerSubscriptionKey, question, answer);

                        Console.WriteLine(output);
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

        public static string addQnA(string id, string key, string question, string answer)
        {
            string response = string.Empty;
            string query = "{\"add\": {\"qnaPairs\": [{\"answer\": \"" + answer + "\", \"question\": \"" + question + "\"}]}}";

            var client = new HttpClient();

            // Build URI
            var uri = "https://westus.api.cognitive.microsoft.com/qnamaker/v2.0/knowledgebases/" + id;


            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(uri);
            request.Headers.Add("Ocp-Apim-Subscription-Key", key);
            request.Method = "PATCH";
            request.ContentType = "application/json";
           

            System.Text.UTF8Encoding encoding = new System.Text.UTF8Encoding();
            byte[] bytes = encoding.GetBytes(query);
            request.ContentLength = bytes.Length;

            using (Stream requestStream = request.GetRequestStream())
            {
                // Send the data.
                requestStream.Write(bytes, 0, bytes.Length);
            }
            var responseRaw = (HttpWebResponse)request.GetResponse();
            response = new StreamReader(responseRaw.GetResponseStream()).ReadToEnd();



            return response;
        }
    }
}
