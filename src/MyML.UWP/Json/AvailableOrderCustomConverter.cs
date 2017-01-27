using MyML.UWP.Models.Mercadolivre;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyML.UWP.Json
{    
    public class AvailableOrderCustomConverter : Newtonsoft.Json.Converters.CustomCreationConverter<Item>
    {
        public override Item Create(Type objectType)
        {
            throw new NotImplementedException();
        }

        public Item Create(Type objectType, JObject jObject)
        {
            var type = (string)jObject.Property("valueType");
            switch (type)
            {
                case "int":
                    return null;
                case "string":
                    return null;
            }

            throw new Exception(String.Format("The given vehicle type {0} is not supported!", type));
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            // Load JObject from stream
            JObject jObject = JObject.Load(reader);

            // Create target object based on JObject
            var target = Create(objectType, jObject);

            // Populate the object properties
            serializer.Populate(jObject.CreateReader(), target);

            return target;
        }
    }

}
