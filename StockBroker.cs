using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Stock
{
    public class StockBroker
    {
        public string BrokerName { get; set; }
        public List<Stock> stocks = new List<Stock>();
        public static ReaderWriterLockSlim myLock = new ReaderWriterLockSlim();
        readonly string destPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Lab1_output.txt");
        public string titles = "Broker".PadRight(10) + "Stock".PadRight(15) + "Value".PadRight(10) + "Changes".PadRight(10) + "Date and Time";

        /// <summary>
        /// The stockbroker object
        /// </summary>
        /// <param name="brokerName">The stockbroker's name</param>
        public StockBroker(string brokerName)
        {
            BrokerName = brokerName;
        }
        /// <summary>
        /// Adds stock objects to the stock list
        /// </summary>
        /// <param name="stock">Stock object</param>
        public void AddStock(Stock stock)
        {
            stocks.Add(stock);
            stock.StockEvent += async (sender, e) => await EventHandler(sender, e);
        }
        public async Task EventHandler(object sender, EventArgs e)
        {
            try
            {
                myLock.EnterWriteLock();
                Stock newStock = (Stock)sender;

                // Output to the console window
                Console.WriteLine($"{BrokerName,-16} {newStock.StockName,-15} {newStock.CurrentValue,-10} {newStock.NumChanges,-10} {DateTime.Now}");

                // Output to the file
                using (StreamWriter outputFile = new StreamWriter(destPath, true))
                {
                    await outputFile.WriteLineAsync($"{BrokerName,-16} {newStock.StockName,-15} {newStock.CurrentValue,-10} {newStock.NumChanges,-10} {DateTime.Now}");
                }
            }
            finally
            {
                myLock.ExitWriteLock();
            }
        }

    }
}
