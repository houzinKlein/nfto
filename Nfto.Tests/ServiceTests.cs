using Microsoft.VisualStudio.TestTools.UnitTesting;
using Nfto.Services;
using System;
using System.Threading.Tasks;

namespace Nfto.Tests
{
    [TestClass]
    public class ServiceTests
    {
        INftoService service;

        [TestInitialize]
        public void Initialize()
        {
            string databasePath = $"MyData{Guid.NewGuid().ToString()}.db";

            service = new NftoService(databasePath);
        }

        string jsonOneLine = "'{Type:Mint,TokenId:0x0001,Address:0x5001}'";

        string jsonOneLineArray1 = "'[{Type:Mint,TokenId:0x0002,Address:0x5001},{Type:Burn,TokenId:0x0001}]'";
        
        string jsonOneLineArray2 = "'[{Type:Mint,TokenId:0x0003,Address:0x5001},{Type:Mint,TokenId:0x0004,Address:0x5001}]'";

        [TestMethod]
        public async Task TestWalletWithInlineJson()
        {
            Assert.IsNotNull(service);

            var result = await service.RunReadLineAsync(jsonOneLine);

            Assert.IsTrue(result.Success);

            Assert.IsTrue(result.Model.Length==1);

            var resultArray1 = await service.RunReadLineAsync(jsonOneLineArray1);

            Assert.IsTrue(resultArray1.Success);

            var resultArray2 = await service.RunReadLineAsync(jsonOneLineArray2);

            Assert.IsTrue(resultArray2.Success);

            var checkFileExist = await service.RunNftOwnershipAsync("0x0001");
            Assert.IsFalse(checkFileExist.Success);

            var checkifNftExists = await service.RunNftOwnershipAsync("0x0003");
            Assert.IsTrue(checkifNftExists.Success);
            Assert.IsTrue(checkifNftExists.Model.Address == "0x5001");

            var listTokensResult = await service.RunWalletOwnershipAsync("0x5001");
            Assert.IsTrue(listTokensResult.Success);
            Assert.IsTrue(listTokensResult.Model.Count==3);


            var resetResult = await service.ResetAsync();
            Assert.IsTrue(resetResult.Success);

            var tokensAfterReset = await service.RunWalletOwnershipAsync("0x5001");
            Assert.IsFalse(tokensAfterReset.Success);
            Assert.IsNull(tokensAfterReset.Model);

        }


        string filename = "transactions.json";
        string jsonTesting = "'{Type:Mint,TokenId:0xD000000000000000000000000000000000000000,Address:0x1000000000000000000000000000000000000000}'";

        [TestMethod]
        public async Task TestWithFile()
        {
            Assert.IsNotNull(service);

            var result = await service.RunReadFileAsync(filename);

            Assert.IsTrue(result.Success);

            Assert.IsTrue(result.Model.Length == 5);

            var checkifNftDoesExists = await service.RunNftOwnershipAsync("0xA000000000000000000000000000000000000000");
            Assert.IsFalse(checkifNftDoesExists.Success);

            var checkifNftExists = await service.RunNftOwnershipAsync("0xB000000000000000000000000000000000000000");
            Assert.IsTrue(checkifNftExists.Success);
            Assert.IsTrue(checkifNftExists.Model.Address == "0x3000000000000000000000000000000000000000");


            var checkifNftExists2 = await service.RunNftOwnershipAsync("0xC000000000000000000000000000000000000000");
            Assert.IsTrue(checkifNftExists2.Success);
            Assert.IsTrue(checkifNftExists2.Model.Address == "0x3000000000000000000000000000000000000000");


            var checkifNftDoesExists2 = await service.RunNftOwnershipAsync("0xD000000000000000000000000000000000000000");
            Assert.IsFalse(checkifNftDoesExists2.Success);


            var resultJsonInline = await service.RunReadLineAsync(jsonTesting);
            Assert.IsTrue(resultJsonInline.Success);
            Assert.IsTrue(resultJsonInline.Model.Length==1);
            Assert.IsTrue(resultJsonInline.Model[0].Type == "Mint");

            var checkifNftExists3 = await service.RunNftOwnershipAsync("0xD000000000000000000000000000000000000000");
            Assert.IsTrue(checkifNftExists3.Success);


            var listTokensResult = await service.RunWalletOwnershipAsync("0x3000000000000000000000000000000000000000");
            Assert.IsTrue(listTokensResult.Success);
            Assert.IsTrue(listTokensResult.Model.Count == 2);

            var resetResult = await service.ResetAsync();
            Assert.IsTrue(resetResult.Success);

            var tokensAfterReset = await service.RunWalletOwnershipAsync("0x3000000000000000000000000000000000000000");
            Assert.IsFalse(tokensAfterReset.Success);
            Assert.IsNull(tokensAfterReset.Model);
        }

        [TestCleanup]
        public void CleanResources()
        {
            if(service!= null)
            {
                service.Dispose();
            }
        }


    }
}
