using System;
using System.Collections.Generic;
using System.Linq;

namespace Stocks.Models {
    public struct Transaction {
        public string Ticker { get; }
        public DateTime Date { get; }
        public int Count { get; }
        public double Price { get; }
        public double TotalValue => Price * Count;

        public Transaction(string ticker, DateTime date, int count, double price) {
            Ticker = ticker;
            Date = date;
            Count = count;
            Price = price;
        }
    }

    public class PortfolioItemBalance {
        public int Count { get; }
        public double Price { get; }

        public PortfolioItemBalance(int count, double price) {
            Count = count;
            Price = price;
        }
    }

    public class CashFlow {
        public DateTime Date { get; }
        public double Value { get; }

        public CashFlow(DateTime date, double value) {
            Date = date;
            Value = value;
        }
    }

    public class Portfolio {
        public const string CashKey = "$Cash";
        Dictionary<string, PortfolioItem> items;
        
        public IPortfolioItem this[string ticker] { get => items[ticker]; }
        public IEnumerable<string> Tickers => items.Keys;
        public IEnumerable<Transaction> Transactions => items.Values.SelectMany(i => i.Transactions);
        public double PortfolioValue => items.Values
            .Select(i => i.ActualValue)
            .Aggregate(0.0, (left, right) => left + right);


        public Portfolio() {
            this.items = new Dictionary<string, PortfolioItem>();
        }
        public Portfolio(IDictionary<string, PortfolioItem> items) {
            this.items = new Dictionary<string, PortfolioItem>(items);
        }
        public void AddTransaction(Transaction transaction) {
            PortfolioItem item = null;
            if (!items.TryGetValue(transaction.Ticker, out item)) {
                item = new PortfolioItem();
                items.Add(transaction.Ticker, item);
            }
            item.AddTransaction(transaction);
        }

        public void ReplaceTransactionAt(string ticker, int index, Transaction newTransaction) {
            Transaction transactionToReplace = items[ticker].Transactions[index];
            ReplaceTransaction(transactionToReplace, newTransaction);
        }
        public void ReplaceTransaction(Transaction transactionToReplace, Transaction newTransaction) {
            RemoveTransaction(transactionToReplace);
            AddTransaction(newTransaction);
        }

        public void RemoveAllTransactions(string ticker) {
            items.Remove(ticker);
        }

        public void RemoveTransactionAt(string ticker, int index) {
            Transaction transactionToRemove = items[ticker].Transactions[index];
            RemoveTransaction(transactionToRemove);
        }

        public void RemoveTransaction(Transaction transactionToRemove) {
            items[transactionToRemove.Ticker].RemoveTransaction(transactionToRemove);
        }

        public IList<string> GetOpenedPositions(DateTime date) {
            DateTime next = date.AddDays(1);
            return items.Where(p => p.Value.GetBalance(next).Count != 0)
                .Select(p => p.Key)
                .ToList();
        }

        public PortfolioItemBalance GetBalance(string ticker, DateTime date, Transaction before) {
            PortfolioItem item;
            if (items.TryGetValue(ticker, out item)) 
                return item.GetBalance(date, before);
            return null;
        }

        public IList<CashFlow> GetCashFlows(DateTime date, IDictionary<string, double> actualPrices) {
            List<CashFlow> cashFlows = items.Values
                .SelectMany(i => i.GetTransactionsBefore(date))
                .GroupBy(t => t.Date)
                .Select(g => new CashFlow(
                    date: g.Key,
                    value: g.Select(t => t.Price * t.Count)
                            .Aggregate(0.0, (left, right) => left + right)
                ))
                .Where(f => Math.Abs(f.Value) >= 0.00000001)
                .ToList();

            var actualCash = items
                .Select(p => {
                    double actualPrice = 0.0;
                    actualPrices.TryGetValue(p.Key, out actualPrice);
                    PortfolioItemBalance balance = p.Value.GetBalance(date);
                    return actualPrice * balance.Count;
                })
                .Aggregate(0.0, (left, right) => left + right);

            cashFlows.Add(new CashFlow(date, -actualCash));

            return cashFlows;
        }

        public IReadOnlyDictionary<string, PortfolioItemBalance> GetActualOpenedPositionsBalance() {
            Dictionary<string, PortfolioItemBalance> result = new Dictionary<string, PortfolioItemBalance>();
            foreach(var pair in items) {
                if (pair.Value.ActualCount != 0) {
                    result.Add(pair.Key, pair.Value.ActualBalance);
                }
            }
            return result;
        }

        public IReadOnlyList<KeyValuePair<string, PortfolioItemBalance>> GetShortPositionBalances(DateTime date) {
            List<KeyValuePair<string, PortfolioItemBalance>> shortPositions =
                items.Select(i => new KeyValuePair<string, PortfolioItemBalance>(i.Key, i.Value.GetBalance(date)))
                        .Where(i => i.Value.Count < 0)
                        .ToList();
            return shortPositions;
        }

        public double GetCashValue(DateTime date) {
            return 0;
        }
    }

    class ByDateTransactionComparer : IComparer<Transaction> {
        public int Compare(Transaction x, Transaction y) => x.Date.CompareTo(y.Date);
    }
}
