# Software-Management-Core
End goal for this project is a flexible setup of a CQRS system with eventsourcing, that can be applied to both small and very large projects easily with a variety of technologies.

The main application business logic, including api and datalayer(s) implementations are all in this repository's solution for now, for convenience. This is intended to be split up later to prevent a huge repository download (that happens far too often). Components should stand on their own.

## Setup
This requires Visual Studio 2017 RC or higher with .NET Core 1.1.0 or higher and an EntityFrameworkCore compatible backing for the database (set to SQL for now, others can be easily added). Right now you need to modify the connectionstring in the project directly. This will be made configurable in future.

### Controller
CommandController is the main controller, which receives a list of CommandDtos from the front-end (see the Angular 2 repo). This is an AspNetCore project which picks up the commands which have a Name property which indicates the type of command, and an Entity property which indicates the entity (aggregate root) the command is for. 

### Command Manager
The CommandManager will determine which Command object matches that combination of Name and Entity, which can be configured in the CommandManager with CommandConfigs. This will route the command to the correct service (e.g. ProductService) which can execute the command.

### Services
Each service as well as the commands defines its repository and state interfaces, which have to be injected. This ensures that the services are as agnostic as possible about what kind of datastore backs them, which is an important goal for this project (being able to easily implement and experiment with different types of datastore, both as a learning exercise and as an important practical approach where different datastores can be mixed and matched purposefully). The services (and most helpers like IDateTimeProvider) are also written as Shared Projects, which don't have any dependency of their own and are therefore easily packaged in any platform (so could be used as .NET Framework 4.5 client dlls, Xamarin Forms, targeting different .NET Standard versions, etc.) They are all bundeled into a single SoftwareManagementCore project/dll, but are intended such that at a minimum each 'domain' (projects, products, contacts etc.) can be published as a separate dll.
For more information on Shared Projects as used here, read this blog: https://www.linkedin.com/pulse/i-shared-projects-arwin-van-arum

### Api
The AspNetCore (api) project uses the native dependency injection to inject the implementation of these state and repository interfaces, and currently one such implementation is included in the form of an EntityFrameworkCore implementation (that currently expects a Database on the localhost named SoftwareManagement that it can log into with TrustedAuthentication - the connectionstring will be configurable later). 

### Command Processing to Repository
Currently the CommandController treats the list of commands that comes in as one transaction, and both stores the commands and executes the commands immediately, which results in updated products. This can later be separated if necessary, e.g. save the commands and/or throw them on a bus, and then let other processes have at them. But one of the main goals is to show that even in simple scenarios, this can be a nice and efficient way of working. And of course ideally we will update the Angular 2 data locally and then sync with commands in a webworker thread in the background.
For more information, read this blog: https://www.linkedin.com/pulse/i-repository-arwin-van-arum

### Hooking up to Client
The client will send commands to the CommandsController, as a simple json array. For an example, see the Angular 2 Client project (https://github.com/niwrA/Software-Management-Angular2-Client). Each epic also gets its own readonly controller (see ProjectsController for an example), and then we can start experimenting with different kinds of efficient datastore backings for different purposes, as introducing a new one will only require playing back the commands that are already stored in the database, so that should be fun. Of course it should be extremely easy to implement other clients as well, so other examples will follow (UWP, Xamarin, React, etc.)

### Testing
Testing aims to use proper Unit testing, e.g. the system under test will only be tested for core functionality and implementing the declared interfaces. Testing framework is xUnit (because of early, good Core support) with Moq, and a setup from Microsoft for Controller testing (tests are currently not yet detected for some reason). It has been setup quite cleanly I think. If you contribute, I strongly recommend trying to work test first, and work forward by starting at the command. 
