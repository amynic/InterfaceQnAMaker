﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using Newtonsoft.Json;
using System.Net;
using System.Net.Http;
using System.IO;


//Admin app to update QnA maker service with new knowledge

namespace InterfaceQnAMaker
{
    class Program
    {
        //get QnA maker credentials - pulled from App.config
        static string knowledgeBaseID = ConfigurationSettings.AppSettings["id"];
        static string qnaMakerSubscriptionKey = ConfigurationSettings.AppSettings["subscriptionkey"];
        public static string uriV2 = "https://westus.api.cognitive.microsoft.com/qnamaker/v2.0/knowledgebases/";

        static void Main(string[] args)
        {

            Run();

        }

        static async Task Run()
        {
            Console.ResetColor();
            var exit = false;

            //Init variables
            string output = string.Empty;
            string query = string.Empty;
            string question = string.Empty;
            string answer = string.Empty;

            try
            {
                //Ask user for task
                Console.WriteLine();
                Console.WriteLine("What do you want to do?");
                Console.WriteLine("=================================================================");
                Console.WriteLine("1. Query QnA Maker");
                Console.WriteLine("2. Add to the QnA Maker knowledge base");
                Console.WriteLine("3. Publish the new knowledge database");
                Console.WriteLine();

                var key = Console.ReadKey(true);

                //Case statement to excute correct code
                switch (key.KeyChar)
                {
                    // Test a query/question against the current database
                    case '1':
                        Console.WriteLine("Please enter a query:");
                        query = Console.ReadLine();
                        output = queryQnA.queryService(query, knowledgeBaseID, qnaMakerSubscriptionKey);
                        Console.WriteLine(output);
                        await Run();
                        break;
                    case '2':

                        Console.WriteLine("Please enter a new question:");
                        question = Console.ReadLine();
                        Console.WriteLine("Please enter a new answer:");
                        answer = Console.ReadLine();

                        output = AddToKnowledge.addQnA(knowledgeBaseID, qnaMakerSubscriptionKey, question, answer);

                        Console.WriteLine(output);
                        await Run();
                        break;

                    case '3':
                        Console.WriteLine("Preparing to publish new knowledge database ...");
                        output = publishQnA(knowledgeBaseID, qnaMakerSubscriptionKey);
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


        
        public static string publishQnA(string id, string key)
        {

            string postUrl = uriV2 + id;
            var payload = "{}";
            string returnString = string.Empty;

            var request = (HttpWebRequest)WebRequest.Create(postUrl);
            request.Headers.Add("Ocp-Apim-Subscription-Key", key);
            request.Method = "PUT";
            request.ContentType = "application/json";

            System.Text.UTF8Encoding encoding = new System.Text.UTF8Encoding();
            byte[] bytes = encoding.GetBytes(payload);
            request.ContentLength = bytes.Length;

            try
            {
                using (Stream requestStream = request.GetRequestStream())
                {
                    // Send the data.
                    requestStream.Write(bytes, 0, bytes.Length);
                }

                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                returnString = response.StatusCode.ToString();
                Console.WriteLine("Publish Complete");
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Ooops, something broke: {0}", ex.Message);
                Console.WriteLine();
            }

            


            return returnString;


        }
    }
}
