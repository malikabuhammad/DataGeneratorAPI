
# Mock Data Insertion API

A .NET 9 API designed for inserting mock data into predefined table entities.

---

## Requirements

To run this solution, ensure you have the following installed and configured:

1. **.NET SDK 9.0**  
   Download and install it from the official [.NET website](https://dotnet.microsoft.com/).

2. **App Settings Configuration**  
   Create an `appsettings.json` file in the root directory of your project or use `appsettings.Development.json` for local development. Include the necessary configuration, such as database connection strings and other settings.  

   Example `appsettings.json` structure:
   ```json
   {
       "ConnectionStrings": {
           "DefaultConnection": "YourDatabaseConnectionStringHere"
       },
       "AppSettings": {
           "SomeKey": "SomeValue"
       }
   }
