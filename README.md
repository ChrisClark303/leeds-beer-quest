# leeds-beer-quest
Coding challenge to find places to drink beer. In Leeds.

##Project structure

The entire app is broken into two; an angular front-end (Angular is the front-end technology I'm most familiar with) and a dotnet 7 API. The angular code can be found [here](https://github.com/ChrisClark303/leeds-beer-quest/tree/main/LeedsBeerQuest/LeedsBeerQuest.Web/leeds-beer-quest) and the API code is [here](https://github.com/ChrisClark303/leeds-beer-quest/tree/main/LeedsBeerQuest).

The API contains 4 sets of projects:

###LeedsBeerQuest.Api
This holds code that hosts the application logic and is the entry point for the application. No application logic is in here, the only code here is for service setup, controller definitions and DI registrations. The DI being used is the in-built dotnet DI. The controllers defer to a service layer to do the actual work; the only other logic in the controller is input and output formatting. Initially all API routes were implemented using a minimal API - as extra functionality was added I then upgraded one set of routes to a full controller and added tests, but you'll notice that the data management endpoint is still left as a minimal API. 

###LeedsBeerQuest.App
This holds models, interface definitions for application logic, and default implementations of those interfaces. The initial cut of the application used an in-memory data store (implemented by MemoryCache) and so was useful for starting to build the UI; that in-memory implementation of the service layer is stored in this project.
