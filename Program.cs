using System;
using System.Collections.Generic;

class Program
{
    static void Main()
    {
        Wallet wallet = new Wallet(1000); // Начален баланс
        Market market = new Market();
        bool isRunning = true;

        while (isRunning)
        {
            Console.Clear();
            Console.WriteLine("=== Crypto Simulator ===");
            Console.WriteLine($"Balance: {wallet.Balance:F2} BGN");
            market.UpdatePrices();
            market.ShowPrices();
            Console.WriteLine("1. Buy\n2. Sell\n3. Transaction History\n4. Exit");
            Console.Write(">> ");
            string choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    wallet.Buy(market);
                    break;
                case "2":
                    wallet.Sell(market);
                    break;
                case "3":
                    wallet.ShowHistory();
                    break;
                case "4":
                    isRunning = false;
                    break;
                default:
                    Console.WriteLine("Invalid option.");
                    break;
            }
        }
    }
}

class Wallet
{
    public double Balance { get; private set; }
    private Dictionary<string, double> ownedCrypto = new();
    private List<Transaction> history = new();

    public Wallet(double startBalance)
    {
        Balance = startBalance;
    }

    public void Buy(Market market)
    {
        Console.Write("Enter currency name: ");
        string name = Console.ReadLine();
        if (!market.Prices.ContainsKey(name)) return;

        Console.Write("Amount to spend (BGN): ");
        if (!double.TryParse(Console.ReadLine(), out double amount)) return;
        double price = market.Prices[name];

        if (amount > Balance) return;

        double quantity = amount / price;
        Balance -= amount;

        if (ownedCrypto.ContainsKey(name))
            ownedCrypto[name] += quantity;
        else
            ownedCrypto[name] = quantity;

        history.Add(new Transaction(name, quantity, price, "Bought"));
    }

    public void Sell(Market market)
    {
        Console.Write("Enter currency name: ");
        string name = Console.ReadLine();
        if (!ownedCrypto.ContainsKey(name)) return;

        Console.Write("How much do you want to sell: ");
        if (!double.TryParse(Console.ReadLine(), out double quantity)) return;
        if (quantity > ownedCrypto[name]) return;

        double price = market.Prices[name];
        double earned = quantity * price;
        Balance += earned;
        ownedCrypto[name] -= quantity;

        history.Add(new Transaction(name, quantity, price, "Sold"));
    }

    public void ShowHistory()
    {
        Console.WriteLine("=== Transaction History ===");
        foreach (var t in history)
            Console.WriteLine(t);
        Console.ReadKey();
    }
}

class Transaction
{
    public string CurrencyName;
    public double Quantity;
    public double Price;
    public string Type;

    public Transaction(string name, double qty, double price, string type)
    {
        CurrencyName = name;
        Quantity = qty;
        Price = price;
        Type = type;
    }

    public override string ToString()
    {
        return $"{Type} {Quantity:F4} {CurrencyName} at {Price:F2} BGN";
    }
}

class Market
{
    public Dictionary<string, double> Prices = new();

    private Random rnd = new Random();
    private string[] currencies = { "BTC", "ETH", "DOGE" };

    public void UpdatePrices()
    {
        foreach (var currency in currencies)
        {
            Prices[currency] = rnd.Next(100, 10000) / 10.0;
        }
    }

    public void ShowPrices()
    {
        Console.WriteLine("Current market prices:");
        foreach (var kv in Prices)
        {
            Console.WriteLine($"{kv.Key}: {kv.Value:F2} BGN");
        }
    }
}
