using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Nfto.Models;

namespace Nfto.Services
{
    class MyMessageConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return typeof(MessageBase).IsAssignableFrom(objectType);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            JObject obj = JObject.Load(reader);
            string discriminator = (string)obj["Type"];

            MessageBase item;
            switch (discriminator.ToLower())
            {
                case "mint":
                    item = new MintMessage();
                    break;
                case "burn":
                    item = new BurnMessage();
                    break;
                case "transfer":
                    item = new TransferMessage();
                    break;
                default:
                    throw new NotImplementedException();
            }

            serializer.Populate(obj.CreateReader(), item);

            return item;

        }


        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {

        }
    }
}
