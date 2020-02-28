using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Stocks.UI.Services.Mocks {
    public class JsonReader {
        JsonSerializerSettings defaultSerializationSettings;

        public JsonReader() {
            defaultSerializationSettings = new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore };
            defaultSerializationSettings.Converters.Add(new HistoricalClosePriceConverter());
            defaultSerializationSettings.Converters.Add(new ListSymbolConverter());
        }

        public Task<T> GetAsync<T>(Stream stream, CancellationToken token, params JsonConverter[] converters) {
            if (stream == null) {
                throw new ArgumentNullException(nameof(stream));
            }
            if (!stream.CanRead) {
                throw new ArgumentException("Cannot read from the specified file", nameof(stream));
            }
            T result;
            try {
                using (StreamReader reader = new StreamReader(stream)) {
                    string content = reader.ReadToEnd();
                    result = Deserialize<T>(content, converters);
                }
                if (token.IsCancellationRequested) {
                    return Task.FromCanceled<T>(token);
                }
                return Task.FromResult(result);
            } catch (Exception e) {
                return Task.FromException<T>(e);
            }
        }


        T Deserialize<T>(string jsonString, JsonConverter[] converters) {
            var settings = defaultSerializationSettings;
            if (converters != null && converters.Any()) {
                settings = new JsonSerializerSettings() {
                    NullValueHandling = NullValueHandling.Ignore,
                    Converters = new List<JsonConverter>(converters)
                };
            }
            return JsonConvert.DeserializeObject<T>(jsonString, settings);
        }
    }
}
