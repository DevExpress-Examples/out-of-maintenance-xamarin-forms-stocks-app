using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Stocks.Models;

namespace Stocks.UI.Services {
    public class HistoricalClosePriceConverter : JsonConverter<HistoricalClosePrice> {
        public override bool CanRead => true;
        public override bool CanWrite => false;

        public override HistoricalClosePrice ReadJson(JsonReader reader, Type objectType, HistoricalClosePrice existingValue, bool hasExistingValue, JsonSerializer serializer) {
            if (reader.TokenType != JsonToken.StartObject) {
                throw new JsonReaderException("Cannot read a 'HistoricalPrice' object.");
            }
            JObject item = JObject.Load(reader);
            return new HistoricalClosePrice(
                (DateTime)item.GetProperty(nameof(HistoricalClosePrice.Timestamp)),
                (double)item.GetProperty(nameof(HistoricalClosePrice.Close))
            );
        }

        public override void WriteJson(JsonWriter writer, HistoricalClosePrice value, JsonSerializer serializer) {
        }
    }
}
