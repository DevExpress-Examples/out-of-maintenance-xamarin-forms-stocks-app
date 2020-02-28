using System;
using System.Collections.Generic;
using System.Linq;

namespace Stocks.Models {
    public interface IPortfolioItem {
        IReadOnlyList<Transaction> Transactions { get; }

        PortfolioItemBalance ActualBalance { get; }
        int ActualCount { get; }
        double ActualPrice { get; }
        double ActualValue { get; }
    }

    public class PortfolioItem: IPortfolioItem {
        List<Transaction> transactions;
        bool isBalanceUpdated;
        PortfolioItemBalance actualBalance;

        public IReadOnlyList<Transaction> Transactions => transactions;

        public PortfolioItemBalance ActualBalance {
            get {
                UpdateActualBalanceIfNeeded();
                return actualBalance;
            }
        }
        public int ActualCount => ActualBalance.Count;
        public double ActualPrice => ActualBalance.Price;
        public double ActualValue => ActualBalance.Price * actualBalance.Count;

        public PortfolioItem() {
            transactions = new List<Transaction>();
        }
        public PortfolioItem(IEnumerable<Transaction> transactions) {
            this.transactions = new List<Transaction>(transactions);
        }

        public void AddTransaction(Transaction transaction) {
            int insertIndex = FindInsertIndex(transaction.Date);
            transactions.Insert(insertIndex, transaction);
            isBalanceUpdated = false;
        }
        public void ReplaceTransactionAt(int index, Transaction transaction) {
            transactions.RemoveAt(index);
            transactions.Insert(index, transaction);
            isBalanceUpdated = false;
        }
        public void ReplaceTransaction(Transaction transaction, Transaction newTransaction) {
            int index = transactions.IndexOf(transaction);
            ReplaceTransactionAt(index, newTransaction);
        }
        public void RemoveTransactionAt(int index) {
            transactions.RemoveAt(index);
            isBalanceUpdated = false;
        }
        public bool RemoveTransaction(Transaction transaction) {
            bool result = transactions.Remove(transaction);
            if (result) 
                isBalanceUpdated = false;
            return result;
        }

        public IEnumerable<Transaction> GetTransactionsBefore(DateTime date) {
            return transactions.Where(t => t.Date < date);
        }

        public PortfolioItemBalance GetBalance(DateTime date, Transaction beforeTransaction) {
            int beforeTransactionIndex = transactions.IndexOf(beforeTransaction);
            return GetBalance(transactions.Where((t, i) => t.Date <= date && (beforeTransactionIndex < 0 ? true : i < beforeTransactionIndex)));
        }

        public PortfolioItemBalance GetBalance(DateTime date) {
            return GetBalance(transactions.Where(t => t.Date < date));
        }

        PortfolioItemBalance GetBalance(IEnumerable<Transaction> calculatedTransaction) {
            int count = 0;
            double meanPrice = 0;
            foreach (Transaction transaction in calculatedTransaction) {
                int oldCount = count;
                count += transaction.Count;

                if (count == 0) {
                    meanPrice = 0;
                }
                else {
                    if (Math.Sign(oldCount) == Math.Sign(count)) {
                        meanPrice = (meanPrice * oldCount + transaction.TotalValue) / count;
                    }
                    else {
                        meanPrice = transaction.Price;
                    }
                }
            }
            return new PortfolioItemBalance(count, meanPrice);
        }

        void UpdateActualBalanceIfNeeded() {
            if (isBalanceUpdated) return;
            actualBalance = GetBalance(DateTime.Today.AddDays(1));
            isBalanceUpdated = true;
        }

        int FindInsertIndex(DateTime date) {
            int index = 0;
            if (transactions.Count == 0) return index;
            while (index < transactions.Count && transactions[index].Date <= date) { index++; }
            return index;
        }
    }
}
