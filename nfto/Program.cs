using Newtonsoft.Json;
using Nfto.Models;
using Nfto.Services;
using System;
using System.Text;
using System.Text.RegularExpressions;

namespace Nfto
{
    internal class Program
    {

        static async Task Main(string[] args)
        {

            //Get the command type while ignoring null spaces
            string commandType = string.Empty;
            int i = 0;

            while (i < args.Length && string.IsNullOrEmpty(commandType))
            {
                if (!string.IsNullOrWhiteSpace(args[i]))
                {
                    commandType = args[i];
                }

                i++;
            }


            if (string.IsNullOrEmpty(commandType))
            {
                WrongParametersDisplay();
                return;
            }

            //Get the parameters
            StringBuilder parameters = new StringBuilder();
            for (int k = i; k < args.Length; k++)
            {
                parameters.Append(args[k]);
            }

            string databasePath= Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "MyData.db");
            
            INftoService service = new NftoService(databasePath);


            switch (commandType)
            {
                case "--read-inline":
                    var resultLine = await service.RunReadLineAsync(parameters.ToString());
                    break;

                case "--read-file":
                    var resultFile = await service.RunReadFileAsync(parameters.ToString());
                    break;

                case "--nft":
                    var resultOwnership = await service.RunNftOwnershipAsync(parameters.ToString());
                    break;

                case "--wallet":
                    var resultWallet = await service.RunWalletOwnershipAsync(parameters.ToString());
                    break;

                case "--reset":
                    var resultReset = await service.ResetAsync();
                    break;

                case "--help":
                    DisplayHelp();
                    break;

                default:
                    WrongParametersDisplay();
                    break;
            }

#if DEBUG
            Console.ReadKey();
#endif

        }

        private static void WrongParametersDisplay()
        {
            Console.WriteLine("Error wrong parameters. ");
            Console.WriteLine("Type 'program --help' to get help");
        }

        private static void DisplayHelp()
        {
            Console.WriteLine("****************************** HELP");
            Console.WriteLine("");
            Console.WriteLine("");
            Console.WriteLine("Read Inline(--read-inline <json>)");
            Console.WriteLine("Reads either a single json element, or an array of json elements representing transactions as an argument.");
            Console.WriteLine("program --read-inline '{\"Type\": \"Burn\", \"TokenId\": \"0x...\"}'");
            Console.WriteLine("program --read-inline '[{\"Type\": \"Mint\", \"TokenId\": \"0x...\", \"Address\": \"0x...\"}, {\"Type\": \"Burn\", \"TokenId\": \"0x...\"}]'");
            Console.WriteLine("");
            Console.WriteLine("");
            Console.WriteLine("Read File (--read-file <file>)");
            Console.WriteLine("Reads either a single json element, or an array of json elements representing transactions from the file in the specified location.");
            Console.WriteLine("program --read-file transactions.json");
            Console.WriteLine("");
            Console.WriteLine("");
            Console.WriteLine("NFT Ownership (--nft <id>)");
            Console.WriteLine("Returns ownership information for the nft with the given id");
            Console.WriteLine("program --nft 0x...");
            Console.WriteLine("");
            Console.WriteLine("");
            Console.WriteLine("Wallet Ownership (--wallet <address>)");
            Console.WriteLine("Lists all NFTs currently owned by the wallet of the given address");
            Console.WriteLine("program --wallet 0x...");
            Console.WriteLine("");
            Console.WriteLine("");
            Console.WriteLine("Reset (--reset)");
            Console.WriteLine("Deletes all data previously processed by the Nfto");
        }
    }





}