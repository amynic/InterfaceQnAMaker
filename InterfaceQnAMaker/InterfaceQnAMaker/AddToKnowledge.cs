using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace InterfaceQnAMaker
{
    class AddToKnowledge
    {
        //If the sample query does not return the correct knowledge, add it to the service

        public static string addQnA(string id, string key, string question, string answer)
        {
            string response = string.Empty;
            string query = "{\"add\": {\"qnaPairs\": [{\"answer\": \"" + answer + "\", \"question\": \"" + question + "\"}]}}";

            var client = new HttpClient();

            // Build URI
            var uri = Program.uriV2 + id;


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
