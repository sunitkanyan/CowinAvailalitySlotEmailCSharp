using System;
using System.Collections.Generic;

using System.Globalization;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Threading;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using RestSharp;

namespace console_app
{
    class Program
    {
        static void Main(string[] args)
        {

            while (true)
            {

                InvokeAPI();
                Thread.Sleep(60000*60); // run every 1 hour


            }
            Console.WriteLine("Hello World!");
        }

        private static void InvokeAPI()
        {
            try
            {

                var client = new RestClient("https://cdn-api.co-vin.in/api/v2/appointment/sessions/calendarByPin?pincode=122001&date=07-05-2021");
                client.Timeout = -1;
                var request = new RestRequest(Method.GET);
                client.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/90.0.4430.93 Safari/537.36 Edg/90.0.818.51";
                request.AddParameter("text/plain", "", ParameterType.RequestBody);
                IRestResponse response = client.Execute(request);
                Console.WriteLine(response.Content);
                VaccinedSlots vaccinedSlots = JsonConvert.DeserializeObject<VaccinedSlots>(response.Content);
                if (vaccinedSlots != null)
                {
                    if (vaccinedSlots.Centers.Count > 0)
                    {
                      var ce=  vaccinedSlots.Centers.Find(c => c.Address.Contains("patel")); //your nearby hospitl name
                        if(ce != null)
                              {
                            foreach (Session ss in ce.Sessions)
                            {
                                if (ss.AvailableCapacity > 0)
                                {//suite email
                                 //
                                    Email();
                                    goto  exitnow;
                                }
                            }
                        }
                    }
                }
               
            }
            catch (Exception ex)
            {

            }
            exitnow: Console.WriteLine("slot found");
        }

        private static void Email()
        {
            try
            {
                MailMessage message = new MailMessage();
                SmtpClient smtp = new SmtpClient();
                message.From = new MailAddress("*****@gmail.com");
                message.To.Add(new MailAddress("*****@gmail.com"));
                message.Subject = "book apt right way";
                message.IsBodyHtml = true; //to make message body as html  
                message.Body = "book now please";
                smtp.Port = 587;
                smtp.Host = "smtp.gmail.com"; //for gmail host  
                smtp.EnableSsl = true;
                smtp.UseDefaultCredentials = true;

                smtp.Credentials = new NetworkCredential("*********@gmail.com", "********");
                smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                smtp.Send(message);
            }
            catch (Exception ex) { }
        }
    }
  

    public partial class VaccinedSlots
        {
            [JsonProperty("centers")]
            public List<Center> Centers { get; set; }
        }

        public partial class Center
        {
            [JsonProperty("center_id")]
            public long CenterId { get; set; }

            [JsonProperty("name")]
            public string Name { get; set; }

            [JsonProperty("address")]
            public string Address { get; set; }

            [JsonProperty("state_name")]
            public string StateName { get; set; }

            [JsonProperty("district_name")]
            public string DistrictName { get; set; }

            [JsonProperty("block_name")]
            public string BlockName { get; set; }

            [JsonProperty("pincode")]
            public long Pincode { get; set; }

            [JsonProperty("lat")]
            public long Lat { get; set; }

            [JsonProperty("long")]
            public long Long { get; set; }

            [JsonProperty("from")]
            public DateTimeOffset From { get; set; }

            [JsonProperty("to")]
            public DateTimeOffset To { get; set; }

            [JsonProperty("fee_type")]
            public string FeeType { get; set; }

            [JsonProperty("sessions")]
            public List<Session> Sessions { get; set; }

            [JsonProperty("vaccine_fees", NullValueHandling = NullValueHandling.Ignore)]
            public VaccineFee[] VaccineFees { get; set; }
        }

        public partial class Session
        {
            [JsonProperty("session_id")]
            public Guid SessionId { get; set; }

            [JsonProperty("date")]
            public string Date { get; set; }

            [JsonProperty("available_capacity")]
            public long AvailableCapacity { get; set; }

            [JsonProperty("min_age_limit")]
            public long MinAgeLimit { get; set; }

            [JsonProperty("vaccine")]
            public string Vaccine { get; set; }

            [JsonProperty("slots")]
            public string[] Slots { get; set; }
        }

        public partial class VaccineFee
        {
            [JsonProperty("vaccine")]
            public string Vaccine { get; set; }

            [JsonProperty("fee")]
            //[JsonConverter(typeof(ParseStringConverter))]
            public long Fee { get; set; }
        }
    

}
