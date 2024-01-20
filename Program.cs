using System;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;
using Hazzik.Qif;
using Hazzik.Qif.Transactions;

namespace Investor360_CSV_to_QIF

{
    class Program
    {
        private const string InputFile = "AccountActivity_Export_IRA_1_1_23.csv";
        private const string OutputFile = "AccountActivity_Export_IRA_1_1_23.qmtf";

        static void Main(string[] args)
        {

            var myPath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "/Downloads/";

            StreamReader reader = new StreamReader(myPath + InputFile);
            var qif = new QifDocument();
            var colHeaders = reader.ReadLine();
            do
            {
               var i =  Add360Record(reader.ReadLine());
             //  var i =  AddEdgeWalletRecord(reader.ReadLine());
               qif.InvestmentTransactions.Add(i);
            }
            while (reader.Peek()!= -1);
            reader.Close();

            var writer = new StreamWriter(myPath + OutputFile);
            qif.Save(writer);
            writer.Flush();
            writer.Close();
        }

        static InvestmentTransaction Add360Record(string record)
        {

            var CSVParser = new Regex(",(?=(?:[^\"]*\"[^\"]*\")*(?![^\"]*\"))");

            var fields = CSVParser.Split(record);
            fields[3] = fields[3].Replace("\"", ""); // export tool quotes fields with commas

            var i = new InvestmentTransaction
            {
                Date = DateTime.Parse(fields[0]),      // D
                Action = fields[5] switch              // N
                {
                    "Management Fee" => "Cash",
                    "Contribution to Asset" => "Cash",
                    "Dividend Received" => "Div",
                    "Interest Income" => "Cash", //"IntInc",
                    "Long Term Capital Gain" => "CGLong",
                    "Short Term Capital Gain" => "CGShort",
                    "Reinvestment" => "Buy",//"ReinvDiv",
                    _ => fields[5]
                }
                ,
                Security = fields[3].Split('(',')')[^2], // Y - could have multiple (), want last set
                TextFirstLine = fields[3],                             // P
                Memo = fields[4],                                      // M
                Quantity = Math.Abs(Decimal.Parse(fields[6])),         // Q
                Price = decimal.Parse(fields[7]),                      // I
                AmountTransferred = Math.Abs(decimal.Parse(fields[8])),// $
                TransactionAmount = decimal.Parse(fields[8]),          // T
                Commission = 0                                         // O

            };
            if (fields[3].Split('(',')')[1] != "Debit") // so that debits map in properly
                i.TransactionAmount = Math.Abs(i.TransactionAmount);
            return i;
        }

        static InvestmentTransaction AddEdgeWalletRecord(string record)
        {

            var CSVParser = new Regex(",(?=(?:[^\"]*\"[^\"]*\")*(?![^\"]*\"))");

            var fields = CSVParser.Split(record);
            for (int xx=0; xx < fields.Length; xx++)
                  fields[xx] = fields[xx].Replace("\"", "");

            var i = new InvestmentTransaction
            {
                Date = DateTime.Parse(fields[1] + " " + fields[2]),    // D
                Action = decimal.Parse(fields[4]) > 0 ? "Buy" : "Sell",// N
                Security = fields[0] + "USDT",                         // Y
                TextFirstLine = "TXID:" + fields[10] + " OUR RX ADDR:" + fields[11],       // P
                Memo = (fields[7] + " "  + fields[8]+ " "  + fields[3]).Trim(),                                      // M
                Quantity = Math.Abs(Decimal.Parse(fields[4])),         // Q
                Price = fields[7].Contains("mining",StringComparison.CurrentCultureIgnoreCase) ? 0 : decimal.Parse(fields[6])/ Math.Abs(Decimal.Parse(fields[4])),
                // Price = decimal.Parse(fields[6]),                      // I
                //AmountTransferred = Math.Abs(decimal.Parse(fields[6])),// $
                TransactionAmount = fields[7].Contains("mining",StringComparison.CurrentCultureIgnoreCase) ? 0 :decimal.Parse(fields[6]),          // T
                Commission = decimal.Parse(fields[9]) // O

            };
           // if (fields[3].Split('(',')')[1] != "Debit") // so that debits map in properly
           //     i.TransactionAmount = Math.Abs(i.TransactionAmount);
           return i;
        }
    }
}
