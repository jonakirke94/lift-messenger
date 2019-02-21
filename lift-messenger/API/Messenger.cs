using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Amazon.SimpleNotificationService;
using Amazon.SimpleNotificationService.Model;
using lift_messenger.Library;
using lift_messenger.Settings;
using Microsoft.Extensions.Options;

namespace lift_messenger.API
{
    public class Messenger : IMessageProvider
    {
        private readonly AWSSettings _settings;

        public Messenger(IOptions<AWSSettings> settings)
        {
            _settings = settings.Value;
        }

        public string GenerateMessage()
        {
            string[] names = new string[] { "Casper", "Froberg", "Bro" };
            string[] descriptions = new string[] { "Det ser ud til at det regner...", "Regnvejr i dag..", "Så er vi på den igen.." };
            string[] pickupLine = new string[] { "Vil du samle mig op?", "Henter du mig :D?", "Gider du køre forbi mig?" };
            string[] fromName = new string[] { "Jonathan", "Jonnajuice", "Jonathanos" };


            var rnd = new Random();
            var rndNameIndex = rnd.Next(0, 2);
            var rndDescIndex = rnd.Next(0, 2);
            var rndPickupIndex = rnd.Next(0, 2);
            var rndFromIndex = rnd.Next(0, 2);

            return "Hey" + names[rndNameIndex] + "\n" + descriptions[rndDescIndex] + "\n" + pickupLine[rndPickupIndex] + "\n" + fromName[rndFromIndex];
        }

        public async Task SendMessage(string message) {

            AmazonSimpleNotificationServiceClient snsClient = new AmazonSimpleNotificationServiceClient(_settings.Key, _settings.SecretKey, Amazon.RegionEndpoint.EUWest1);
            PublishRequest pubRequest = new PublishRequest
            {
                Message = message,
                PhoneNumber = _settings.Phone
            };
            pubRequest.MessageAttributes["AWS.SNS.SMS.SenderID"] =
                new MessageAttributeValue { StringValue = "NOTICE", DataType = "String" };
            pubRequest.MessageAttributes["AWS.SNS.SMS.MaxPrice"] =
                new MessageAttributeValue { StringValue = "0.10", DataType = "Number" };
            pubRequest.MessageAttributes["AWS.SNS.SMS.SMSType"] =
                new MessageAttributeValue { StringValue = "Promotional", DataType = "String" };

            try
            {     
                PublishResponse pubResponse = await snsClient.PublishAsync(pubRequest);
                Console.WriteLine("Successfully sent message " + pubResponse.MessageId);
            }
            catch (Exception e)
            {
                Console.WriteLine("Caught error in Messenger.cs");
                Console.WriteLine(e);
            }
 
        }
    }
}
