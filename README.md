# leeds-beer-quest
Coding challenge to find places to drink beer. In Leeds.

## Introduction

This is a dotnet 7 application hosting an aspnetcore webapi, with an angular front-end. In addition, there is a MongoDB data provider for storage. The API and front-end are hosted in Azure, and can be found [here](https://leedsbeerquestapi.azurewebsites.net/swagger/index.html) and [here](https://purple-stone-0a63fe503.2.azurestaticapps.net/) respectively. The MongoDb collection is stored in cloud.mongodb.com. It's stored in my personal account, but there is an application user - specified in the connection string - that can be used to connect and explore the collection. 
There is a basic deployment pipeline setup in [github actions](https://github.com/ChrisClark303/leeds-beer-quest/actions), so that, when changes are PR'd into Main, an automatic deployment is triggered and the latest version is pushed out. This is very basic as it stands - I let the setup process generate the yml for me and I've not made any further changes (apart from to disable the API management aspect).  

## Functionality

At present, the app is very light on functionality - it only really has 3 functions. I am aware that the second stage of the process is to extend the app, so I focused instead on getting the elements of an end-to-end solution in place, and writing a decent level of testing. Hopefully the work put in so far makes the work of extending the application reasonably straightforward. 

The API provides three separate pieces of functionality; firstly, it allows consumers to find a list of places to drink beer in ascending distance from a specified location. If a location is not supplied, it uses a default set in config (currently set to the location of Joseph's Well). This returns a set of objects containing name, coordinates, and distance, so they can be plotted on a map. Next, it provides a route to request all details for a given establishment by name. These two routes are used by the UI, which, on-load, requests the nearest establishments and plots them on a map; when a user selected one it then requests the details and prints them out below the map.

The last piece of functionality it provides is a data management endpoint so that the establishments can be imported from the LeedsBeerQuest website. This fetches the CSV, parses the data into models, and then passes these on to a service object to be inserted into the relevant data store. Two data stores are currently supported - an in-memory object cache, and MongoDB. There is a "feature flag" - actually just a bool set in appsettings - that switches between these two. This simply instructs the DI registry to register one set of implementations over another.

### Usage

There are two ways of using the app - firstly, there is a fully hosted solution available at:

UI - [https://purple-stone-0a63fe503.2.azurestaticapps.net/](https://purple-stone-0a63fe503.2.azurestaticapps.net/)

API - [https://leedsbeerquestapi.azurewebsites.net/swagger/index.html](https://leedsbeerquestapi.azurewebsites.net/swagger/index.html)

This is currently set up to use the MongoDb data store, and therefore should be ready to go (it should be noted, however, that it may be slow on first hit, as the cloud services seem to be aggressively down-scaled if they're not in use for a while - they are all on free or at least very cheap tiers!). 

Secondly, the app can be started by cloning the repo, and building and running the app via the standard methods - during development, I ran the api through visual studio and the angular app by 'ng serve --open' through a terminal in VS Code (note that the angular app currently points to the Azure-hosted API, so the [service class](https://github.com/ChrisClark303/leeds-beer-quest/blob/main/LeedsBeerQuest/LeedsBeerQuest.Web/leeds-beer-quest/src/app/beer-quest.service.ts) will need to be changed to reference a local instance). On first use, particularly if using the in-memory data cache (see below), it may be necessary to import the data from the Data Mill North website. This can be done via the data management endpoint accessible through the API's Swagger (or by hitting PATCH /data-management/import directly). This will download the data, convert it to a series of model objects and import it into the data store. 

Once the data is populated, it is available to be queried: again, you have two options. Firstly, using Swagger, the /beer/nearest-establishments route will return the nearest 20 venues from Joseph's Well; optionally, lat and long coordinates can be supplied via the query string to shift the start location (eg, /beer/nearest-establishments?lat=53.794829&lng=-1.547601 to search around Leeds Train Station). Then, the full details for a particular venue can be retrieved by specifying it by name, eg: /Beer/The%20White%20Rose. 

Alternatively, there is a [UI](https://purple-stone-0a63fe503.2.azurestaticapps.net) available. On load, the UI will request the nearest establishments from the API (at present, it uses the default location). These are then added as markers on the map; when one of these markers is selected, the full details of that establishment is selected and the details are then presented to the user.

Note that the ability to import data is not available in the UI, as it is considered an Admin utility.

## Project structure

The application is composed of two main components: an angular front-end (Angular is the front-end technology I'm most familiar with) and a dotnet 7 API. The angular code can be found [here](https://github.com/ChrisClark303/leeds-beer-quest/tree/main/LeedsBeerQuest/LeedsBeerQuest.Web/leeds-beer-quest) and the API code is [here](https://github.com/ChrisClark303/leeds-beer-quest/tree/main/LeedsBeerQuest).

The API contains 5 projects:

### LeedsBeerQuest.Api
This holds code that hosts the application logic and is the entry point for the application. No application logic is in here, it only defines code for service setup, controller definitions and DI registrations. The DI being used is the in-built dotnet DI. The controllers defer to a service layer to do the actual work; the only other logic in the controller is input and output formatting, and basic exception handling. Initially all API routes were implemented using a minimal API - as extra functionality was added I then upgraded one set of routes to a full controller and added tests, but you'll notice that the data management endpoint is still left as a minimal API. 

### LeedsBeerQuest.App
This class library holds models, interface definitions for application logic, and default implementations of those interfaces. The initial cut of the application used an in-memory data store (implemented by MemoryCache) and so was useful for starting to build the UI; that in-memory implementation of the service layer is stored in this assembly.

#### Models
I've used separate Read and Write models, but I've cheated here by only splitting those out there's actually a difference between the two. Essentially, the Location object has to include a Coordinates array when written to Mongo to support the GeoSpatial index. This is not required on the read model so the parser object that builds the model for the datastore uses the write version, while everything else defaults to the read. 

### LeedsBeerQuest.Data.Mongo
As a more permanent data store, I migrated to MongoDb. This involved providing Mongo-specific implementations of the various interfaces described above, which are defined in this project. Arguably this is the most complex piece of the application.

### LeedsBeerQuest.Test
Unit tests for the above. Rather than having a test project per class library, I have instead placed all tests in one project and organised them by namespace/folder. For example, all tests for the App library are located in LeedsBeerQuests.Tests/App. If the number of tests became larger, I'd probably split out into test project per assembly.

### LeedsBeerQuest.Tests.Specflow
I used Specflow to perform end-to-end API testing. This spins up an instance of the LeedsBeerQuest.Api service and allows the tests to interact with it via a HttpClient. Currently this is setup to be completely end-to-end - the data stores are not stubbed and so the tests will interact with MongoDb. The one exception here is that the CSV download from Data Mill North is stubbed.

### MongoDB
I decided on MongoDB for a few reasons - firstly, I wanted something permanent, as the in-memory cache used initially had to be filled everytime the app was restarted (this is why the specflow tests call the DataImport route in test setup). Secondly, I knew it supported geospatial querying, so would be able to provide the nearest establishments and distance calculations out of the box, and thirdly, MongoDB provides free cloud storage!
On the downside, it's definitely the most complex part of the app; the mongo querying syntax is not particular user-friendly, and so I've hidden most of that inside a query builder object. MongoDB has a dotnet driver that supplies a series of static builder objects to generate query objects, but to be honest these are no less idiosyncratic and are almost entirely impossible to test, so I instead generated the queries using BsonDocuments, which can be subject to unit testing. 

## Testing

The app has been (mostly) test-driven, using NUnit and Moq. Ncrunch reports the overall coverage at around 77%. The only code I have not tested is the service setup code in LeedsBeerQuest.Api (the controller is tested) and the MongoDbConnectionFactory, which instantiates a concrete MongoClient. 

There are integration tests, written in Specflow (again with Nunit and Moq); they test end-to-end functionality, so there are only 4 of these. It's also worth noting that these are not UI tests, so they don't use Selenium or Cypress or similar to actually drive a browser and interact with the app; they are simply API tests, so while they exercise the entire back-end code, the UI is untested. They will, however, test through to MongoDB if the appsetting is set up for that ("Interesting" fact: I discovered that my distance calculation and Mongo's differs by a small amount while running the specflow tests across both providers; I subsequently changed the Assert to factor in a delta value to ensure the tests pass in both scenarios).

I used the tests extensively during refactoring to ensure that I hadn't broken any functionality. In addition, I added the MongoDb implementation after already putting the MemoryCache in place and writing specflow tests. This meant that I could be sure I was implementating the second provider correctly by re-running those tests as I went.

## Limitations

There are a number of aspects that could be improved; firstly, there is no separate test/stage/prod environments setup in Azure, or the deployment pipeline. This means that there is no way to test code apart from on the local machine or in prod. Ideally I would want a test environment and the ability to deploy to it from a feature branch at will. However, I figured this was beyond the scope of this piece of work, but I would like to highlight that I wouldn't normally want to work like that! 
On a related note, it isn't possible to inject config - such as the MongoDB connection string or the API url used in the UI - on a per-environment basis via the CI/CD pipeline. This would limit the usefulness of having separate test/stage/prod environments.
In addition, the connection string is stored in plain text, and should really use a secret specified in the deployment pipeline to inject this in at deployment time (also note that, to ensure that this application would work anywhere for review purposes, I have allowed access from any IP address in Mongo - normally I would white-list a specific set of IPs to ensure that, even if someone is able to find the username and password, they cannot use it indiscriminately).

The UI is very rudimentary, the styling is taken from the defaults provided by Angular when first creating an application using ng new. There has been no mobile optimization, and so I am certain it will not adapt well is this regard.

Exception handling is unsophisticated, as is logging. Exceptions are left to bubble up to the controller, as the entry point to the app. At this point there are handled and logged, and a 500 is returned to the caller. In larger applications, greater visibility is warranted and therefore logging would be more comprehensive. 

I used the establishment name as a kind of natural identifier when fetching the details for an individual establishment, but ideally an ID should be generated and used in that route, due to the risk of non-URL-safe characters appearing in the name. In practice, it seems that both Swagger and Angular deal with this acceptably, even for names with accents and apostrophes etc, but there does seem to be an issue with "Golf" Cafe Bar, due to the quotes around Golf (in actual fact, there is a bug in the parser that builds the models here, too, which prevents the name from being correctly generate).

As already mentioned, functionality is very light - there are no options for filtering, or even selecting the next page. In addition, closed venues are not filtered out from search results, so anyone using the app could well be disappointed!

