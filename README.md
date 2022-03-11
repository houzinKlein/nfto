# nfto
A console app in C# that receives some subset of transactions, and processes them in such a way that enables the program
to answer questions about NFT ownership.

The application is based on .NET 6 and using SQLite to save the file using the sqlite-net nuget package.
The IDbContext can be implemented to use any other database like MongoDb, DynamoDb or CosmoDb.

** The application is tested on Windows 11. It will probably work on Windows 10.

# Application Layers
- Nfto: The console application
- Nfto.DAL: The data access layer
- Nfto.Tests: The integration tests

# To deploy

1. Download the code

2. Go to the root of the decompressed folder

3. Run the command `dotnet build --configuration Release --output C:\Nfto` where C:\Nfto is your folder. Make sure your prompt has the necessary rights to that folder

4. Create a powershell alias `new-alias program C:\Nfto\Nfto.exe`

5. Run `program --help` to check if it is working. If you see the help, bingo you are done installing the nfto.


# Tests in command line

```powershell

program --read-file transactions.json

program --nft 0xA000000000000000000000000000000000000000

program --nft 0xB000000000000000000000000000000000000000

program --nft 0xC000000000000000000000000000000000000000

program --nft 0xD000000000000000000000000000000000000000

program --read-inline '{ "Type": "Mint", "TokenId":"0xD000000000000000000000000000000000000000", "Address":"0x1000000000000000000000000000000000000000"}'

program --nft 0xD000000000000000000000000000000000000000

program --wallet 0x3000000000000000000000000000000000000000

program --reset

program --wallet 0x3000000000000000000000000000000000000000

```


# Links that helped

- https://stackoverflow.com/questions/23566980/deserialize-json-based-on-fields-in-net-c
- https://github.com/praeclarum/sqlite-net