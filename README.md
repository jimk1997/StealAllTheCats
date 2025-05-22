# StealAllTheCats

- Database Connection
  This connection string uses Windows Authentication to connect to the `master` database on the local SQL Server instance.
  
- Database Schema

  CatEntity:

	Id: An auto incremental unique integer that identifies a cat within your database
	CatId: Represents the id of the image returned from CaaS API
	Width: Represents the width of the image returned from CaaS API
	Height: Represents the height of the image returned from CaaS API
	Image: This should contain your solution on how to store the image
	Created: Timestamp of creation of database record
  
  TagEntity:

	Id: An auto incremental unique integer that identifies a tag within your database
	Name: Describes the cat’s temperament, returned from CaaS API (breeds\temperament). Note:
	    - One cat may have many tags, and many cats can share a tag
	    - Field breed/temperament contains comma-separated values. Each one of them is a tag.
	    - Search images on Cat API with breeds only.
    Created: Timestamp of creation of database record

  CatTagEntity
  Represents the many-to-many relationship between cats and tags:

  CatEntityId: Foreign key referencing CatEntity
  TagEntityId: Foreign key referencing TagEntity
  Cat: Navigation property to the related CatEntity
  Tag: Navigation property to the related TagEntity

- Database Migration
  dotnet ef migrations add InitialCreate
  dotnet ef database update