﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace InterfaceQnAMaker
{
    class queryQnA
    {
        /// <summary>
        /// POST: qnamaker/v1.0//knowledgebases/{id}/generateAnswer
        /// Send a question and recieve an answer from the service if it exists
        /// </summary>
        ///     <param name="query">The question asked</param>
        ///     <param name="id">Knowledge Service ID</param>
        ///     <param name="key">Knowledge Service access keys</param>
        /// <remarks> Currently returns JSON object converted to a string to print to the console </remarks>

        public static string queryService(string query, string id, string key)
        {
            string response = string.Empty;
            string answer = string.Empty;
            string score = string.Empty;
            QnAMakerResult extractinfo;

            //Build the URI
            //Uri qnamakerUriBase = new Uri("https://westus.api.cognitive.microsoft.com/qnamaker/v1.0");
            var builder = new UriBuilder($"{Program.uri}/knowledgebases/{id}/generateAnswer");

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
                Console.ForegroundColor = ConsoleColor.Red;
                throw new Exception("Unable to deserialize QnA Maker response string.");
            }

            Console.ForegroundColor = ConsoleColor.Yellow;
            response = "ANSWER: " + "\n" + answer + "\n\n" + "SCORE: " + "\n" + score;
            return response;
        }
    }   
}
