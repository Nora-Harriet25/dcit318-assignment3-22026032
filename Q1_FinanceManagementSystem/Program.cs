using System;
using System.Collections.Generic;

namespace FinanceManagementSystem
{
    // ===== a. Record type representing a financial transaction =====
    // Records are immutable by default - perfect for representing a fixed transaction event
    public record Transaction(int Id, DateTime Date, decimal Amount, string Category);

    // ===== b. Interface defining how a transaction should be processed =====
    public interface ITransactionProcessor
    {
        void Process(Transaction transaction);
    }

    // ===== c. Three concrete implementations of ITransactionProcessor =====
    public class BankTransferProcessor : ITransactionProcessor
    {
        public void Process(Transaction transaction)
        {
            Console.WriteLine($"[Bank Transfer] Processed GHC{transaction.Amount:F2} for {transaction.Category}.");
        }
    }

    public class MobileMoneyProcessor : ITransactionProcessor
    {
        public void Process(Transaction transaction)
        {
            Console.WriteLine($"[Mobile Money] Processed GHC{transaction.Amount:F2} for {transaction.Category}.");
        }
    }

    public class CryptoWalletProcessor : ITransactionProcessor
    {
        public void Process(Transaction transaction)
        {
            Console.WriteLine($"[Crypto Wallet] Processed GHC{transaction.Amount:F2} for {transaction.Category}.");
        }
    }

    // ===== d. Base Account class =====
    public class Account
    {
        public string AccountNumber { get; }
        public decimal Balance { get; protected set; }

        public Account(string accountNumber, decimal initialBalance)
        {
            AccountNumber = accountNumber;
            Balance = initialBalance;
        }

        // virtual so SavingsAccount can override this behavior
        public virtual void ApplyTransaction(Transaction transaction)
        {
            Balance -= transaction.Amount;
        }
    }

    // ===== e. Sealed SavingsAccount - cannot be inherited further =====
    public sealed class SavingsAccount : Account
    {
        public SavingsAccount(string accountNumber, decimal initialBalance)
            : base(accountNumber, initialBalance)
        {
        }

        public override void ApplyTransaction(Transaction transaction)
        {
            if (transaction.Amount > Balance)
            {
                Console.WriteLine($"Insufficient funds for transaction of GHC{transaction.Amount:F2} ({transaction.Category}).");
                return;
            }

            Balance -= transaction.Amount;
            Console.WriteLine($"Transaction applied: -GHC{transaction.Amount:F2} ({transaction.Category}). Updated balance: GHC{Balance:F2}");
        }
    }

    // ===== f. FinanceApp - integrates and simulates the whole system =====
    public class FinanceApp
    {
        private readonly List<Transaction> _transactions = new();

        public void Run()
        {
            // i. Create a savings account with an initial balance
            var savingsAccount = new SavingsAccount("SAV-001", 1000m);
            Console.WriteLine($"Created SavingsAccount {savingsAccount.AccountNumber} with balance GHC{savingsAccount.Balance:F2}\n");

            // ii. Create three sample transactions
            var transaction1 = new Transaction(1, DateTime.Now, 150.00m, "Groceries");
            var transaction2 = new Transaction(2, DateTime.Now, 200.00m, "Utilities");
            var transaction3 = new Transaction(3, DateTime.Now, 500.00m, "Entertainment");

            // iii. Process each transaction with a different processor
            ITransactionProcessor mobileMoneyProcessor = new MobileMoneyProcessor();
            ITransactionProcessor bankTransferProcessor = new BankTransferProcessor();
            ITransactionProcessor cryptoWalletProcessor = new CryptoWalletProcessor();

            mobileMoneyProcessor.Process(transaction1);
            bankTransferProcessor.Process(transaction2);
            cryptoWalletProcessor.Process(transaction3);

            Console.WriteLine();

            // iv. Apply each transaction to the savings account
            savingsAccount.ApplyTransaction(transaction1);
            savingsAccount.ApplyTransaction(transaction2);
            savingsAccount.ApplyTransaction(transaction3);

            // v. Add all transactions to the internal list
            _transactions.Add(transaction1);
            _transactions.Add(transaction2);
            _transactions.Add(transaction3);

            Console.WriteLine($"\nTotal transactions recorded: {_transactions.Count}");
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("=== Finance Management System ===\n");
            var app = new FinanceApp();
            app.Run();
        }
    }
}
