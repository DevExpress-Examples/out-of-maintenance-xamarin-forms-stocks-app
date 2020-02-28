using System;
using Stocks.Models;

namespace Stocks.ViewModels {
    public enum TransactionType {
        Buy,
        Sell
    }

    public class TransactionViewModel {
        public Transaction Transaction { get; }
        public string Ticker => Transaction.Ticker;
        public DateTime Date => Transaction.Date;
        public TransactionType Type => Transaction.Count >= 0 ? TransactionType.Buy : TransactionType.Sell;
        public double Price => Transaction.Price;
        public int Count => Math.Abs(Transaction.Count);

        public string DateString => Transaction.Date.ToShortDateString();
        public string PriceWithCountText => $"{Price:n2} x {Count}";
        public string TotalText => $"{Count * Price:n2}";

        public TransactionViewModel(Transaction transaction) {
            Transaction = transaction;
        }
    }
}
