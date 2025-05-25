# StealAllTheCats

- Project Endpoints:

  1. POST /api/cats/fetch: Fetch 25 cat images from CaaS API and save them to the database. After rerunning the operation, no duplicate cats are present.
     The POST /api/cats/fetch endpoint handle fetching cat images as a background job. Implement a job, using Hangfire.
     Once the job is started, return a response to the user with a job identifier that allows them to track the status of the background job (e.g., GET /api/jobs/{id} to check the status).
  2. GET /api/cats/{id}: Retrieve a cat by its ID.
  3. GET /api/cats: Retrieve cats with paging support (e.g., GET /api/cats?page=1&pageSize=10).
     GET /api/cats: Retrieve cats with a specific tag with paging support (e.g., GET /api/cats?tag=playful&page=1&pageSize=10”)
  4. GET /api/jobs/{id} to check the status

- Project Structure:

  Controllers: Contains the API controllers responsible for handling HTTP requests.
  Data: Includes data access layer components such as database context.
  Dto: Holds Data Transfer Objects (DTOs) that define the shape of data sent to and received from clients.
  Models: Contains the domain models or entity classes representing the core data structures of the application, typically mapping to database tables.
  Repositories: Implements repository classes responsible for data operations and abstracting database access logic from the rest of the application.
  Services: Contains business services.
  Logs: Stores log files for capturing application runtime information, errors, and diagnostics.

- Database & Api Configuration:

  Use the Installer executable for Microsoft SQL Server 2022 Developer Edition (SQL2022-SSEI-Dev.exe). https://www.microsoft.com/en-us/sql-server/sql-server-downloads
  Uses Windows Authentication to connect to the 'CatsDB' database on the local SQL Server instance.
  Go to appsettings.json and fill your "BaseUrl" and "ApiKey", which you can find here https://developers.thecatapi.com/view-account/ylX4blBYT9FaoVd6OhvR?report=FJkYOq9tW
  
- Database Schema:

  CatEntity:

	Id: An auto incremental unique integer that identifies a cat within your database
	CatId: Represents the id of the image returned from CaaS API
	Width: Represents the width of the image returned from CaaS API
	Height: Represents the height of the image returned from CaaS API
	Image: This should contain your solution on how to store the image
	Created: Timestamp of creation of database record
  
  TagEntity:

	Id: An auto incremental unique integer that identifies a tag within your database
	Name: Describes the cat s temperament, returned from CaaS API (breeds\temperament). Note:
	    - One cat may have many tags, and many cats can share a tag
	    - Field breed/temperament contains comma-separated values. Each one of them is a tag.
	    - Search images on Cat API with breeds only.
    Created: Timestamp of creation of database record

  CatTagEntity:

  Represents the many-to-many relationship between cats and tags:

  CatEntityId: Foreign key referencing CatEntity
  TagEntityId: Foreign key referencing TagEntity
  Cat: Navigation property to the related CatEntity
  Tag: Navigation property to the related TagEntity

- Database Migration:

  Open Developer Command Prompt
  cd C:\Users\yourUser\source\repos\StealAllTheCats\StealAllTheCats
  dotnet ef migrations add InitialCreate
  dotnet ef database update

- Third Party API:
  https://developers.thecatapi.com/view-account/ylX4blBYT9FaoVd6OhvR?report=bOoHBz-8t

  Example Response:

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
	}

- Hangfire:
  https://www.hangfire.io/
  An easy way to perform background processing in .NET Core applications. 

- Running the Application:

  Clone Github Repository https://github.com/jimk1997/StealAllTheCats.git
  Run the app with IIS Express from Visual Studio.
  Access the Swagger UI for API documentation and testing at:
  https://localhost:44366/swagger/index.html