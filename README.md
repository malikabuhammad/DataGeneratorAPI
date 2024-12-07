
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
   ```

3. **Database Setup**  
   - The API currently requires predefined table entities for data insertion. Ensure the database tables are set up before running the API.  


## Features

- **Insert Mock Data**  
   The current version supports inserting mock data into predefined table entities.  

---

## Future Updates

Im planning to enhance the API with the following features:

### **1. Dynamic Table Entity Support**
- The API will evolve to handle dynamic table entities, eliminating the need for predefined schemas.
- **Approach**: 
  - Implement JSON-based column storage for flexible data structures.
  - Introduce a single dynamic table with key-value pairs or JSON objects.
    
### **2. Integration with Frontend UI**
- Develop a lightweight frontend interface to interact with the API for inserting and managing data entities.


