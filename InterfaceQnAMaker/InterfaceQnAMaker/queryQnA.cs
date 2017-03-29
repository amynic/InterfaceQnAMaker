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
        // Query the QnA service to check for a response on a sample query

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
