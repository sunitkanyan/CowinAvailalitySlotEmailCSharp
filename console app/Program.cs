using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Text;
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
            Console.WriteLine("start...");
          //  double countermy;
            //countermy = 1;
            //while (true)
            //{
              //  Console.WriteLine(string.Format("running again...countermy{0}",countermy.ToString()));
                InvokeAPI();
               // countermy++;
                //Thread.Sleep(60000); // run every 30 min

            //}
            Console.WriteLine("end");
        }

        private static void InvokeAPI()
        {
            try
            {
                Console.WriteLine("running.............");
                //cahneg pin and date in url
                // make sure date should be a day next alteast
                string day = DateTime.Today.Day <= 10 ? string.Format("{0}{1}", 0, DateTime.Today.Day) : (DateTime.Today.Day).ToString();
                string month = DateTime.Today.Month <= 10 ? string.Format("{0}{1}", 0, DateTime.Today.Month) : (DateTime.Today.Month).ToString();

                var client = new RestClient(string.Format("https://cdn-api.co-vin.in/api/v2/appointment/sessions/calendarByPin?pincode=122001&date={0}-{1}-2021",day, month));
                client.Timeout = -1;
                var request = new RestRequest(Method.GET);
                client.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/90.0.4430.93 Safari/537.36 Edg/90.0.818.51";
                request.AddParameter("text/plain", "", ParameterType.RequestBody);
                IRestResponse response = client.Execute(request);
               // Console.WriteLine(response.Content);
                VaccinedSlots vaccinedSlots = JsonConvert.DeserializeObject<VaccinedSlots>(response.Content);
                if (vaccinedSlots != null)
                {
                    if (vaccinedSlots.Centers.Count > 0)
                    {
                  //      var ce = vaccinedSlots.Centers;
                   //var ce =  vaccinedSlots.Centers.FindAll(l => l.Address.Contains("Patel Nagar")); //your nearby hospitl name ste name
                   //     if (ce != null)
                   //           {
                   //         foreach (Center c in ce)
                   //         {
                   //             foreach (Session ss in c.Sessions)
                   //             {
                   //                 if (ss.MinAgeLimit == 18)
                   //                 {
                   //                     if (ss.AvailableCapacity > 0)
                   //                     {//suite email
                   //                      //
                   //                         Console.WriteLine("\n slot mill gya");
                   //                         Email();
                   //                         goto exitnow;
                   //                     }
                   //                 }
                   //             }
                   //         }
                   //     }

                        List<SlotAvaiableInCenter> listCenter = new();
                        var ceAll = vaccinedSlots.Centers; //your nearby hospitl name ste name
                        if (ceAll != null)
                        {
                            foreach (Center c in ceAll)
                            {
                                foreach (Session ss in c.Sessions)
                                {
                                    //if (ss.MinAgeLimit == 18)
                                    //{
                                       if (ss.AvailableCapacity > 0)
                                        {//suite email
                                         //
                                            listCenter.Add(new SlotAvaiableInCenter { Address = c.Address, Center = c.Name, district_name = c.DistrictName });
                                            Console.WriteLine("\n slot mill gya");
                                         }
                                   // }
                                }
                            }
                            if (listCenter.Count > 0)
                            {
                                Email(listCenter);
                            }
                        }
                    }
                }
               
            }
            catch (Exception ex)
            {

            }
            
            exitnow: Console.WriteLine("...runing");
        }

        private static void Email(List<SlotAvaiableInCenter> centerList)
        {
            try
            {
                StringBuilder strEmailBody = new StringBuilder();
                strEmailBody.Append("<table>");
                foreach (SlotAvaiableInCenter cen in centerList)
                {
                    strEmailBody.Append(string.Format("<tr><td>{0}</td><td>{1}</td><td>{2}</td><td>{3}</td></tr>", cen.Center, cen.Address, cen.district_name,cen.Date));
                }
                strEmailBody.Append("</table>");
                using (MailMessage mail = new MailMessage())
                {
                    mail.From = new MailAddress("testemail@gmail.com");
                    mail.To.Add("testemail123@gmail.com");
                    mail.Subject = "list of center have slots - book immediately";
                    mail.Body = strEmailBody.ToString();
                    mail.IsBodyHtml = true;

                    using (SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587))
                    {
                        smtp.UseDefaultCredentials = false;
                        smtp.Credentials = new NetworkCredential("testemail@gmail.com", "ac12345677");
                        smtp.EnableSsl = true;
                        smtp.Send(mail);
                    }
                }

                Console.WriteLine("\nemail sent \n");
            }
            catch (Exception ex)
            {

                Console.WriteLine("\nemail error \n");
            }
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


    public  class SlotAvaiableInCenter
    {
         public string Center { get; set; }
        public string Address { get; set; }
        public string district_name { get; set; }

        public string Date { get; set; }
    }



}
