using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Stocks.Models {
    public static class JsonConverterUtils {
        public static JToken GetProperty(this JObject item, string propertyName) {
            return item[ToCamelCase(propertyName)];
        }

        public static void SetProperty(this JObject item, string propertyName, object value) {
            item.Add(ToCamelCase(propertyName), JToken.FromObject(value));
        }

        static string ToCamelCase(string propertyName) {
            if (string.IsNullOrEmpty(propertyName)) 
                return string.Empty;
            char[] chars = propertyName.ToCharArray();
            chars[0] = char.ToLower(chars[0]);
            return new string(chars);
        }

        public static List<T> ToList<T>(this JArray jArray, JsonSerializer serializer) {
            List<T> list = new List<T>(jArray.Count);
            foreach (JToken jItem in jArray) {
                list.Add(jItem.ToObject<T>(serializer));
            }
            return list;
        }
    }
}
