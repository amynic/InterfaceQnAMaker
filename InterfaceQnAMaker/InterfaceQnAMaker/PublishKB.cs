using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace InterfaceQnAMaker
{
    class PublishKB
    {

        public static string publishQnA(string id, string key)
        {

            string postUrl = Program.uriV2 + id;
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
