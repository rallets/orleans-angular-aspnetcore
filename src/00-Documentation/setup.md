# Test project using Asp.NET Core, Angular and Microsoft Orleans

## Develop mode

Install Asp Net Core 2.2 SDK (> 2.2.301)
Install [Azure Storage Explorer (> 1.6.2)](https://azure.microsoft.com/en-us/features/storage-explorer/)
Install node.js (> 10.15.2)
Install npm (> 6.8.0)
Install angular cli (> 7.3.4)

## Code

Based on [Orleans tutorial](https://dotnet.github.io/orleans/Documentation/tutorials_and_samples/tutorial_1.html)

## Run it yourself

1. Install dotnet core 2.1 (or higher) and nodejs 8 (or higher).
2. Create a secrets.json file, put it in the right place (in Windows, it's `%AppData%\Roaming\Microsoft\UserSecrets\orleansangularaspnetcore-secrets\secrets.json`).
   A storage account first has to be created in [Microsoft Azure](https://portal.azure.com). On Windows, you can alternatively install the Azure Storage Emulator and put "UseDevelopmentStorage=true" for the storage connection strings.
   The minimum secrets file contains [Work in progress]:

   ```json
   {
     "ConnectionStrings": {
       "DataConnectionString": "DefaultEndpointsProtocol=https;AccountName=[AZURE STORAGE ACCOUNT];AccountKey=[STORAGE KEY];EndpointSuffix=core.windows.net",
       "ReduxConnectionString": "DefaultEndpointsProtocol=https;AccountName=[AZURE STORAGE ACCOUNT];AccountKey=[STORAGE KEY];EndpointSuffix=core.windows.net",
       "SmtpConnectionString": "Host=[SMTP HOST];UserName=[SMTP USERNAME];Password=[SMTP PASSWORD]"
     },
   }
   ```

3. (optional) Run the Azure Storage Emulator
4. in src/06-Frontends/Silo, run `dotnet run`
5. in src/06-Frontends/WebApi, run `dotnet run`
6. in src/06-Frontends/WebClient/dashboard, run ng-serve.bat (or run `npm install`, then `ng serve`)
7. navigate to `http://localhost:4200/`

## Disclaimer

This code should be considered experimental. It works, however the project may have rough edges and has not been thoroughly tested.
I welcome feedback!

-- Mauro
