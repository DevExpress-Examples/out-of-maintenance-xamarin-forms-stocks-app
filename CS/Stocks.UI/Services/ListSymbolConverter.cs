using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Stocks.Models;

namespace Stocks.UI.Services {
    class ListSymbolConverter : JsonConverter<Symbol> {
        public override bool CanRead => true;
        public override bool CanWrite => false;

        public override Symbol ReadJson(JsonReader reader, Type objectType, Symbol existingValue, bool hasExistingValue, JsonSerializer serializer) {
            if (reader.TokenType != JsonToken.StartObject) {
                throw new JsonReaderException("Cannot read a 'Quote' object.");
            }
            JObject item = JObject.Load(reader);
            return new Symbol() {
                Ticker = (string)item.GetProperty(nameof(Symbol.Ticker)),
                CompanyName = (string)item.GetProperty(nameof(Symbol.CompanyName)),

                LatestPrice = (double)item.GetProperty(nameof(Symbol.LatestPrice)),

                Change = (double)item.GetProperty(nameof(Symbol.Change)),
                ChangePercent = (double)item.GetProperty(nameof(Symbol.ChangePercent)),
                LatestUpdate = TimeZoneUtils.FromIEXToLocal((DateTime)item.GetProperty(nameof(Symbol.LatestUpdate)))
            };
        }

        public override void WriteJson(JsonWriter writer, Symbol value, JsonSerializer serializer) {
        }
    }
}
