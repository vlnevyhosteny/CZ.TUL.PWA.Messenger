# CZ.TUL.PWA.Messenger

Semestral work for subject PWA in TUL. App for chatting in ASP.NET Core with communication with SignalR and Angular 7 client.

# Technology

- C#
- ASP.NET Core
- Typescript, jQuery
- Entity Framework Core
- MySQL
- Angular
- SignalR
- Swagger
- JWT Auth

# Prerequisites

- Visual Studio 2017 or Visual Studio for Mac
- .NET Core 2.1 (https://www.microsoft.com/net/core)
- MySQL

# How to run on local

## Server
- Open the CZ.TUL.PWA.Messenger.sln solution in Visual Studio
- Build the solution 
- Set the data provider of your choice in the appsettings.json file and modify the default connection string accordingly if needed.
- Run (F5 or Ctrl+F5)
- Database and seed data will be created automatically the first time you run the application.

## Client
- Go to CZ.TUL.PWA.Messenger.Client/CZ-TUL-PWA-Messenger-Client/
- Run ng serve
