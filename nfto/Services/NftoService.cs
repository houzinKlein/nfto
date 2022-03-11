using Newtonsoft.Json;
using Nfto.DAL.Repositories;
using Nfto.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Nfto.DAL.Models;

namespace Nfto.Services
{

    public class NftoService : INftoService
    {
        #region Constants
        private const string WRONG_PARAMETER_MESSAGE = "wrong parameters";
        private const string ERROR_PROCESSING_MESSAGE = "error processing";
        private const string ERROR_FILE_MESSAGE = "error file content";

        private string _databasePath;

        #endregion

        #region Fields -- Constructors 

        JsonSerializerSettings settings;

        public NftoService(string databasePath)
        {
            settings = new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Objects,

            };

            settings.Converters.Add(new MyMessageConverter());

            _databasePath = databasePath;
        }

        #endregion

        #region INftoService

        /// <summary>
        /// Read data from inline and processe them
        /// </summary>
        /// <param name="jsonParameter"></param>
        /// <returns></returns>
        public async Task<Result<MessageBase[]>> RunReadLineAsync(string jsonParameter)
        {
            string cleanedJson = "";
            try
            {
                //
                if (jsonParameter.Length < 2)
                {
                    return Result<MessageBase[]>.BuildErrorResult(WRONG_PARAMETER_MESSAGE);
                }

                cleanedJson = CleanJson(jsonParameter);

                MessageBase[] messages = null;

                //check if the json is a single or an array
                if (cleanedJson[0] == '{')
                {
                    messages = new MessageBase[] { GetMessage(cleanedJson) };
                }
                else if (cleanedJson[0] == '[')
                {
                    messages = GetMessages(cleanedJson);
                }

                int nb = 0;
                if (messages != null && messages.Length > 0)
                {
                    nb = await ProcessMessagesAsync(messages);
                }

                return Result<MessageBase[]>.BuildSuccessResult(messages);
            }
            catch (JsonSerializationException ex)
            {
                Debug.WriteLine($"error {ex.Message} jsonCleaned {cleanedJson}");
                Console.WriteLine($"Error. Check your input format");

                return Result<MessageBase[]>.BuildErrorResult(ERROR_PROCESSING_MESSAGE);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"error {ex.Message}");
                Console.WriteLine(ERROR_PROCESSING_MESSAGE);

                return Result<MessageBase[]>.BuildErrorResult(ex.Message);
            }

        }

        /// <summary>
        /// Read transactions from a file
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public async Task<Result<MessageBase[]>> RunReadFileAsync(string filePath)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(filePath))
                {
                    return Result<MessageBase[]>.BuildErrorResult(WRONG_PARAMETER_MESSAGE);
                }

                //Read the whole content of the file
                string fileContent = File.ReadAllText(filePath).Trim();

                if (fileContent.Length == 0)
                {
                    return Result<MessageBase[]>.BuildErrorResult("Empty file");
                }

                MessageBase[] messages = JsonConvert.DeserializeObject<MessageBase[]>(fileContent, settings);

                if (messages != null && messages.Length > 0)
                {
                    await ProcessMessagesAsync(messages);
                }

                return Result<MessageBase[]>.BuildSuccessResult(messages);

            }
            catch (JsonSerializationException ex)
            {
                Debug.WriteLine($"error {ex.Message}");
                Console.WriteLine($"Error. Check your input format");

                return Result<MessageBase[]>.BuildErrorResult(ERROR_FILE_MESSAGE);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"error {ex.Message}");

                Console.WriteLine(ERROR_PROCESSING_MESSAGE);

                return Result<MessageBase[]>.BuildErrorResult(ERROR_PROCESSING_MESSAGE);
            }

        }

        /// <summary>
        /// Get the ownership
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<Result<Nft>> RunNftOwnershipAsync(string tokenId)
        {
            try
            {

                if (string.IsNullOrWhiteSpace(tokenId))
                {
                    return Result<Nft>.BuildErrorResult(WRONG_PARAMETER_MESSAGE);
                }

                IDbContext dbContext = new SqliteDbContext(_databasePath);

                var nft = await dbContext.GetNftAsync(tokenId);

                if (nft == null)
                {
                    Console.WriteLine($"{tokenId} is not owned by any wallet");
                    return Result<Nft>.BuildErrorResult($"{tokenId} is not owned by any wallet");
                }

                Console.WriteLine($"{tokenId} is owned by {nft.Address}");

                return Result<Nft>.BuildSuccessResult(nft);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"error {ex.Message}");
                Console.WriteLine(ERROR_PROCESSING_MESSAGE);

                return Result<Nft>.BuildErrorResult(ERROR_PROCESSING_MESSAGE);
            }
        }


        /// <summary>
        /// Get all the token belonging to the address
        /// </summary>
        /// <param name="address"></param>
        /// <returns></returns>
        public async Task<Result<List<Nft>>> RunWalletOwnershipAsync(string address)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(address))
                {
                    return Result < List<Nft>>.BuildErrorResult(WRONG_PARAMETER_MESSAGE);
                }

                IDbContext dbContext = new SqliteDbContext(_databasePath);

                var list = await dbContext.GetAllTokenForAddressAsync(address);

                if (list == null || list.Count == 0)
                {
                    Console.WriteLine($"{address} holds no Tokens");
                    return Result < List<Nft>>.BuildErrorResult($"{address} holds no Tokens");
                }
                else
                {
                    Console.WriteLine($"{address} holds {list.Count} Tokens: ");

                    list.ForEach(n => Console.WriteLine(n.TokenId));

                    return Result < List<Nft>>.BuildSuccessResult(list);

                }


            }
            catch (Exception ex)
            {
                Debug.WriteLine($"error {ex.Message}");
                Console.WriteLine(ERROR_PROCESSING_MESSAGE);

                return Result < List<Nft>>.BuildErrorResult(ERROR_PROCESSING_MESSAGE);
            }
        }

        /// <summary>
        /// Reset the database
        /// </summary>
        /// <returns></returns>
        public async Task<Result> ResetAsync()
        {
            try
            {
                IDbContext dbContext = new SqliteDbContext(_databasePath);

                var ok = await dbContext.ResetDatabaseAsync();

                Console.WriteLine("Program was reset");

                return Result.BuildSuccessResult();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"error {ex.Message}");
                Console.WriteLine(ERROR_PROCESSING_MESSAGE);

                return Result.BuildErrorResult(ERROR_PROCESSING_MESSAGE);
            }
        }


        /// <summary>
        /// Dispose the DbContext and delete the database file
        /// </summary>
        public void Dispose()
        {
            try
            {
                //Delete the Sqlite file
                File.Delete(_databasePath);
            }
            catch
            {

            }
        }

        #endregion

        #region Private helpers methods

        /// <summary>
        /// Process the messages one by one
        /// </summary>
        /// <param name="messages"></param>
        private async Task<int> ProcessMessagesAsync(MessageBase[] messages)
        {

            IDbContext dbContext = new SqliteDbContext(_databasePath);

            int nbProcessed = 0;
            foreach (var message in messages)
            {
                bool success = false;

                if (message is MintMessage)
                {
                    var mintMessage = (MintMessage)message;

                    success = await dbContext.SaveMintAsync(new Nft(mintMessage.TokenId, mintMessage.Address));
                }
                else if (message is BurnMessage)
                {
                    var burnMessage = (BurnMessage)message;

                    success = await dbContext.DeleteTokenAsync(burnMessage.TokenId);

                }
                else if (message is TransferMessage)
                {
                    var transferMessage = (TransferMessage)message;

                    success = await dbContext.TransferTokenAsync(transferMessage.TokenId, transferMessage.From, transferMessage.To);

                }
                if (success) nbProcessed++;

                Debug.WriteLine($"Processing {message} Success={success}");

            }

            Console.WriteLine($"Read {messages.Length} transaction(s)");

            //Returning the number of processed transactions and not all the transactions
            return nbProcessed;
        }


        /// <summary>
        /// Clean the JSon passed as argument
        /// </summary>
        /// <param name="badJson"></param>
        /// <returns></returns>
        private string CleanJson(string badJson)
        {
            return badJson
                    .Replace("\n","")
                    .Replace("\t", "")
                    .Replace("{", "{\"")
                    .Replace("}", "\"}")
                    .Replace(":", "\":\"")
                    .Replace(",", "\",\"")
                    .Replace("}\",\"{", "},{")
                    .Replace("\" ","\"")
                    .Replace("'", "");
        }

        private MessageBase[] GetMessages(string jsonArray)
        {
            var items = JsonConvert.DeserializeObject<MessageBase[]>(jsonArray, settings);

            return items;
        }

        private MessageBase GetMessage(string jsonSingle)
        {
            var item = JsonConvert.DeserializeObject<MessageBase>(jsonSingle, settings);
            return item;
        }

        #endregion

    }
}
