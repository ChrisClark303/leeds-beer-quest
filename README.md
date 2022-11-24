# leeds-beer-quest
Coding challenge to find places to drink beer. In Leeds.

## Project structure

The entire app is broken into two; an angular front-end (Angular is the front-end technology I'm most familiar with) and a dotnet 7 API. The angular code can be found [here](https://github.com/ChrisClark303/leeds-beer-quest/tree/main/LeedsBeerQuest/LeedsBeerQuest.Web/leeds-beer-quest) and the API code is [here](https://github.com/ChrisClark303/leeds-beer-quest/tree/main/LeedsBeerQuest).

The API contains 5 projects:

### LeedsBeerQuest.Api
This holds code that hosts the application logic and is the entry point for the application. No application logic is in here, it only defines code for service setup, controller definitions and DI registrations. The DI being used is the in-built dotnet DI. The controllers defer to a service layer to do the actual work; the only other logic in the controller is input and output formatting. Initially all API routes were implemented using a minimal API - as extra functionality was added I then upgraded one set of routes to a full controller and added tests, but you'll notice that the data management endpoint is still left as a minimal API. 

### LeedsBeerQuest.App
This class library holds models, interface definitions for application logic, and default implementations of those interfaces. The initial cut of the application used an in-memory data store (implemented by MemoryCache) and so was useful for starting to build the UI; that in-memory implementation of the service layer is stored in this project.

### LeedsBeerQuest.Data.Mongo
As a more permanent data store, I migrated to MongoDb. This involved providing Mongo-specific implementations of the various interfaces defined above, and these are located in this project. Arguably this is the most complex piece of the application.

### LeedsBeerQuest.Test
Unit tests for the above. Rather than having a test project per class library, I have instead placed all tests in one project and organised them by namespace/folder. For example, all tests for the App library are located in LeedsBeerQuests.Tests/App. If the number of tests became larger, I'd probably split out into test project per assembly.

### LeedsBeerQuest.Tests.Specflow
I used Specflow to perform end-to-end API testing. This spins up an instance of the LeedsBeerQuest.Api service and allows the tests to interact with it via a HttpClient.  


## Functionality

The API provides three separate pieces of functionality; firstly, it allows consumers to find a list of places to drink beer in ascending distance from a specified location. If a location is not supplied, it uses a default set in config (currently set to the location of Joseph's Well). This returns a set of objects containing name, coordinates, and distance, so they can be plotted on a map. Next, it provides a route to request all details for a given establishment by name. These two routes are used by the UI, which, on-load, requests the nearest establishments and plots them on a map; when a user selected one it then requests the details and prints them out below the map.

The last piece of functionality it provides is a data management endpoint so that the establishments can be imported from the LeedsBeerQuest website. This fetches the CSV, parses the data into models, and then passes these on to a service object to be inserted into the relevant data store. Two data stores are currently supported - an in-memory object cache, and MongoDB. There is a "feature flag" - actually just a bool set in appsettings - that switches between these two. This simply instructs the DI registry to register one set of implementations over another.

## Models
I've used separate Read and Write models, but I've cheated here by only splitting those out there's actually a difference between the two. Essentially, the Location object has to include a Coordinates array when written to Mongo to support the GeoSpatial index. This is not required on the read model so the parser object that builds the model uses the write version, while everything else defaults to the read. 

### MongoDB
I decided on MongoDB for a few reasons - firstly, I wanted something permanent, as the in-memory cache used initially had to be filled everytime the app was restarted (this is why the specflow tests call the DataImport route in test setup). Secondly, I knew it supported geospatial querying, so would be able to provide the nearest establishments and distance calculations out of the box, and thirdly, MongoDB provides free cloud storage!
On the downside, it's definitely the most complex part of the app; the mongo querying syntax is not particular user-friendly, and so I've hidden most of that inside a query builder object. MongoDB has a dotnet driver that supplies a series of static builder objects to generate query objects, but to be honest these are no less idiosyncratic and are almost entirely impossible to test, so I instead generated the queries using BsonDocuments, which can be subject to unit testing. 

## Testing

The app has been (mostly) test-driven, using NUnit and Moq. Ncrunch reports the overall coverage at just under 75%. The only code I have not tested is the service setup code in LeedsBeerQuest.Api (the controller is tested) and the MongoDbConnectionFactory, which instantiates a concrete MongoClient. 

There are integration tests, written in Specflow (again with Nunit and Moq); they test end-to-end functionality, so there are only 4 of these. It's also worth noting that these are not UI tests, so they don't use Selenium or Cypress or similar to actually drive a browser and interact with the app; they are simply API tests, so while they exercise the entire back-end code, the UI is untested. They will, however, test through to MongoDB if the appsetting is set up for that ("Interesting" fact: I discovered that my distance calculation and Mongo's differs by a small amount while running the specflow tests across both providers; I subsequently changed the Assert to factor in a delta value to ensure the tests pass in both scenarios).

## Limitations
