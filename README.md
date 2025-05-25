# StealAllTheCats

- Project Endpoints:

  1. POST /api/cats/fetch: Fetch 25 cat images from CaaS API and save them to the database.   
	 After rerunning the operation, no duplicate cats are present.  
     The POST /api/cats/fetch endpoint handle fetching cat images as a background job. 
	 Implement a job using Hangfire. Once the job is started, return a response to the user 
	 with a job identifier that allows them to track the status of the background job (e.g., GET /api/jobs/{id} to check the status).  
  2. GET /api/cats/{id}: Retrieve a cat by its ID.
  3. GET /api/cats: Retrieve cats with paging support (e.g., GET /api/cats?page=1&pageSize=10).  
     GET /api/cats: Retrieve cats with a specific tag with paging support (e.g., GET /api/cats?tag=playful&page=1&pageSize=10)
  4. GET /api/jobs/{id} to check the status

- Project Structure:

  1. Controllers: Contains the API controllers responsible for handling HTTP requests.
  2. Data: Includes data access layer components such as database context.
  3. Dto: Holds Data Transfer Objects (DTOs) that define the shape of data sent to and received from clients.
  4. Models: Contains the domain models or entity classes representing the core data structures of the application, typically mapping to database tables.
  5. Repositories: Implements repository classes responsible for data operations and abstracting database access logic from the rest of the application.
  6. Services: Contains business services.
  7. Logs: Stores log files for capturing application runtime information, errors, and diagnostics.

- Database & Api Configuration:

  1. Use the Installer executable for Microsoft SQL Server 2022 Developer Edition (SQL2022-SSEI-Dev.exe). https://www.microsoft.com/en-us/sql-server/sql-server-downloads
  2. Use Windows Authentication to connect to the 'CatsDB' database on the local SQL Server instance.
  3. Go to appsettings.json and fill your "BaseUrl" and "ApiKey", which you can find here https://developers.thecatapi.com/view-account/ylX4blBYT9FaoVd6OhvR?report=FJkYOq9tW
  
- Database Schema:

  CatEntity:

  1. Id: An auto incremental unique integer that identifies a cat within your database
  2. CatId: Represents the id of the image returned from CaaS API
  3. Width: Represents the width of the image returned from CaaS API
  6. Height: Represents the height of the image returned from CaaS API
  7. Image: This should contain your solution on how to store the image
  8. Created: Timestamp of creation of database record
  
  TagEntity:

  1. Id: An auto incremental unique integer that identifies a tag within your database
  2. Name: Describes the cat's temperament, returned from CaaS API (breeds\temperament).  
	    - One cat may have many tags, and many cats can share a tag  
	    - Field breed/temperament contains comma-separated values. Each one of them is a tag.  
	    - Search images on Cat API with breeds only.  
  3. Created: Timestamp of creation of database record

  CatTagEntity:

  Represents the many-to-many relationship between cats and tags:

  1. CatEntityId: Foreign key referencing CatEntity
  2. TagEntityId: Foreign key referencing TagEntity
  3. Cat: Navigation property to the related CatEntity
  4. Tag: Navigation property to the related TagEntity

- Database Migration:

  1. Open Developer Command Prompt
  2. cd C:\Users\yourUser\source\repos\StealAllTheCats\StealAllTheCats
  3. dotnet ef migrations add InitialCreate
  4. dotnet ef database update

- Third Party API:

  https://developers.thecatapi.com/view-account/ylX4blBYT9FaoVd6OhvR?report=bOoHBz-8t

  Example Response:

  ```json
  {  
	"id":"0XYvRd7oD",  
	"width":1204,"height":1445,  
	"url":"https://cdn2.thecatapi.com/images/0XYvRd7oD.jpg",  
	"breeds":[{  
		"weight":{"imperial":"7  -  10","metric":"3 - 5"},  
		"id":"abys","name":"Abyssinian",  
		"temperament":"Active, Energetic, Independent, Intelligent, Gentle",  
		"origin":"Egypt",  
		"country_codes":"EG",  
		"country_code":"EG",  
		"life_span":"14 - 15",  
		"wikipedia_url":"https://en.wikipedia.org/wiki/Abyssinian_(cat)"  
	}]  
  ```

- Hangfire:

  An easy way to perform background processing in .NET Core applications. https://www.hangfire.io/

- Running the Application:

  1. Clone Github Repository https://github.com/jimk1997/StealAllTheCats.git
  2. Run the app with IIS Express from Visual Studio.
  3. Access the Swagger UI for API documentation and testing at:
  4. https://localhost:44366/swagger/index.html

- Running the Application via Container:

  1. Download Docker Desktop https://www.docker.com/products/docker-desktop/
  2. Restart your PC and enter BIOS/UEFI setup and enable the virtualization option.
  3. Run the following SQL Queries in Sql Server Management Studio https://learn.microsoft.com/en-us/ssms/

	 ```sql
	 CREATE LOGIN stealuser WITH PASSWORD = 'StealCats@123';  
     USE CatsDB;  
     CREATE USER stealuser FOR LOGIN stealuser;  
     ALTER ROLE db_owner ADD MEMBER stealuser;  
     ```
  4. Change the Connection String

	 ```json
	 "DefaultConnection": "Server=host.docker.internal;Database=CatsDB;User Id=stealuser;Password=StealCats@123;TrustServerCertificate=True;"
	 ```
  5. Go to Database Properties via SSMS -> Security -> Select "SQL Server and Windows Authentication mode"
  6. Activate TCP/IP
	 Sql Server Configuration Manager  
	 SQL Server Network Configuration -> Protocols for MSSQLSERVER  
	 Enable TCP/IP  
  7. Run the app with Container(Dockfile) from Visual Studio. 