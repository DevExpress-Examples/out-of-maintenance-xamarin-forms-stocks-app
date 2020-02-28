using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Stocks.Models;
using Stocks.Services;

namespace Stocks.UI.Services {
    class PortfolioConverter : JsonConverter<Portfolio> {
        public override Portfolio ReadJson(JsonReader reader, Type objectType, Portfolio existingValue, bool hasExistingValue, JsonSerializer serializer) {
            if (reader.TokenType != JsonToken.StartObject) {
                throw new JsonReaderException("Cannot read a 'Portfolio' object.");
            }
            JObject jItems = JObject.Load(reader);
            Dictionary<string, PortfolioItem> items = new Dictionary<string, PortfolioItem>(jItems.Count);
            foreach (var pair in jItems) {
                PortfolioItem item = pair.Value.ToObject<PortfolioItem>(serializer);
                items.Add(pair.Key, item);
            }
            return new Portfolio(items);
        }

        public override void WriteJson(JsonWriter writer, Portfolio value, JsonSerializer serializer) {
            JObject jPortfolio = new JObject();
            foreach (string ticker in value.Tickers) {
                JToken jItem = JToken.FromObject(value[ticker], serializer);
                jPortfolio.Add(ticker, jItem);
            }
            //JToken jCashItem = JToken.FromObject(value.CashItem, serializer);
            //jPortfolio.Add(Portfolio.CashKey, jCashItem);
            jPortfolio.WriteTo(writer);
        }
    }

    class PortfolioItemConverter : JsonConverter<PortfolioItem> {
        public override PortfolioItem ReadJson(JsonReader reader, Type objectType, PortfolioItem existingValue, bool hasExistingValue, JsonSerializer serializer) {
            if (reader.TokenType != JsonToken.StartArray) {
                throw new JsonReaderException("Cannot read a 'PortfolioItem' object.");
            }
            JArray jTransactions = JArray.Load(reader);
            List<Transaction> transactions = new List<Transaction>(jTransactions.Count);
            foreach (JToken jTransaction in jTransactions) {
                Transaction transaction = jTransaction.ToObject<Transaction>(serializer);
                transactions.Add(transaction);
            }
            return new PortfolioItem(transactions);
        }

        public override void WriteJson(JsonWriter writer, PortfolioItem value, JsonSerializer serializer) {
            JArray jArray = new JArray();
            foreach (Transaction transaction in value.Transactions) {
                JObject jTransaction = JObject.FromObject(transaction, serializer);
                jArray.Add(jTransaction);
            }
            jArray.WriteTo(writer);
        }
    }

    class TransactionConverter : JsonConverter<Transaction> {
        public override bool CanRead => true;
        public override bool CanWrite => true;

        public override Transaction ReadJson(JsonReader reader, Type objectType, Transaction existingValue, bool hasExistingValue, JsonSerializer serializer) {
            if (reader.TokenType != JsonToken.StartObject) {
                throw new JsonReaderException("Cannot read a 'Transaction' object.");
            }
            JObject item = JObject.Load(reader);
            return new Transaction(
                (String)item[nameof(Transaction.Ticker)],
                (DateTime)item[nameof(Transaction.Date)],
                (int)item[nameof(Transaction.Count)],
                (double)item[nameof(Transaction.Price)]);
        }

        public override void WriteJson(JsonWriter writer, Transaction value, JsonSerializer serializer) {
            JObject item = new JObject();
            item.Add(nameof(Transaction.Ticker), JToken.FromObject(value.Ticker));
            item.Add(nameof(Transaction.Date), JToken.FromObject(value.Date));
            item.Add(nameof(Transaction.Count), JToken.FromObject(value.Count));
            item.Add(nameof(Transaction.Price), JToken.FromObject(value.Price));
            item.WriteTo(writer);
        }
    }

    class PortfolioStatisticsCacheConverter : JsonConverter<PortfolioStatisticsCache> {
        public override bool CanRead => true;
        public override bool CanWrite => true;

        public override PortfolioStatisticsCache ReadJson(JsonReader reader, Type objectType, PortfolioStatisticsCache existingValue, bool hasExistingValue, JsonSerializer serializer) {
            if (reader.TokenType != JsonToken.StartObject) {
                throw new JsonReaderException("Cannot read a 'PortfolioStatisticsCache' object.");
            }
            JObject item = JObject.Load(reader);

            DateTime datestamp = (DateTime)item[nameof(PortfolioStatisticsCache.Datestamp)];
            List<IrrHistoryItem> irrHistory = ((JArray)item[nameof(PortfolioStatisticsCache.IrrHistory)]).ToList<IrrHistoryItem>(serializer);
            List<PortfolioItemStatistics> statistics = ((JArray)item[nameof(PortfolioStatisticsCache.ItemStatistics)]).ToList<PortfolioItemStatistics>(serializer);

            UpdatePartInTotal(statistics);
            
            return new PortfolioStatisticsCache(datestamp, irrHistory, statistics);
        }

        public override void WriteJson(JsonWriter writer, PortfolioStatisticsCache value, JsonSerializer serializer) {
            JObject item = new JObject();
            item.Add(nameof(PortfolioStatisticsCache.Datestamp), JToken.FromObject(value.Datestamp));
            item.Add(nameof(PortfolioStatisticsCache.IrrHistory), JToken.FromObject(value.IrrHistory));
            item.Add(nameof(PortfolioStatisticsCache.ItemStatistics), JToken.FromObject(value.ItemStatistics));
            item.WriteTo(writer);
        }

        void UpdatePartInTotal(IEnumerable<PortfolioItemStatistics> statistics) {
            double totalValue = statistics.Aggregate(0.0, (res, item) => res += Math.Abs(item.ActualValue));
            foreach(PortfolioItemStatistics item in statistics) {
                item.CalculatePart(totalValue);
            }
        }
    }

    class IrrHistoryItemConverter : JsonConverter<IrrHistoryItem> {
        public override bool CanRead => true;
        public override bool CanWrite => true;

        public override IrrHistoryItem ReadJson(JsonReader reader, Type objectType, IrrHistoryItem existingValue, bool hasExistingValue, JsonSerializer serializer) {
            if (reader.TokenType != JsonToken.StartObject) {
                throw new JsonReaderException("Cannot read a 'IrrHistoryItem' object.");
            }
            JObject item = JObject.Load(reader);
            return new IrrHistoryItem(
                (DateTime)item[nameof(IrrHistoryItem.Date)],
                (double)item[nameof(IrrHistoryItem.InternalRateOfReturn)]);
        }

        public override void WriteJson(JsonWriter writer, IrrHistoryItem value, JsonSerializer serializer) {
            JObject item = new JObject();
            item.Add(nameof(IrrHistoryItem.Date), JToken.FromObject(value.Date));
            item.Add(nameof(IrrHistoryItem.InternalRateOfReturn), JToken.FromObject(value.InternalRateOfReturn));
            item.WriteTo(writer);
        }
    }

    class PortfolioItemStatisticsConverter : JsonConverter<PortfolioItemStatistics> {
        public override bool CanRead => true;
        public override bool CanWrite => true;

        public override PortfolioItemStatistics ReadJson(JsonReader reader, Type objectType, PortfolioItemStatistics existingValue, bool hasExistingValue, JsonSerializer serializer) {
            if (reader.TokenType != JsonToken.StartObject) {
                throw new JsonReaderException("Cannot read a 'PortfolioItemStatistics' object.");
            }
            JObject item = JObject.Load(reader);
            return new PortfolioItemStatistics(
                (string)item[nameof(PortfolioItemStatistics.Ticker)],
                (int)item[nameof(PortfolioItemStatistics.Count)],
                (double)item[nameof(PortfolioItemStatistics.OperationPrice)],
                (double)item[nameof(PortfolioItemStatistics.ActualPrice)]);
        }

        public override void WriteJson(JsonWriter writer, PortfolioItemStatistics value, JsonSerializer serializer) {
            JObject item = new JObject();
            item.Add(nameof(PortfolioItemStatistics.Ticker), JToken.FromObject(value.Ticker));
            item.Add(nameof(PortfolioItemStatistics.Count), JToken.FromObject(value.Count));
            item.Add(nameof(PortfolioItemStatistics.OperationPrice), JToken.FromObject(value.OperationPrice));
            item.Add(nameof(PortfolioItemStatistics.ActualPrice), JToken.FromObject(value.ActualPrice));
            item.WriteTo(writer);
        }
    }
}
