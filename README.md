# Hermes

Hermes is a generic websocket notification service giving backend services the ability to submit real-time notifications to user facing web interfaces.

## Features

* REST API to manage notifications (creation, aknowledgement, ...)
* Websocket endpoint to get real-time notifications in web applications
* Uses RabbitMQ to ensure scalablility
* Notifications can either be transient or permanent (and then stored in database for future usage)
* Permanent notifications can be "aknowledged" per user

## Data model

## How it works

## Dependencies

Hermes requires two things:
* SQL database
* RabbitMQ server

## NuGet Dependencies

* [EntityFramework](http://go.microsoft.com/fwlink/?LinkID=320540)
* [FluentValidation](https://github.com/JeremySkinner/fluentvalidation)
* [Nancy](https://github.com/NancyFx/Nancy)
* [Json.NET](https://github.com/JamesNK/Newtonsoft.Json)
* [NLog](https://github.com/NLog/NLog)
* [protobuf-net](https://github.com/mgravell/protobuf-net)
* [RabbitMQ.Client](https://github.com/rabbitmq/rabbitmq-dotnet-client)
* [RestSharp](https://github.com/restsharp/RestSharp)
* [SuperSocket](https://github.com/kerryjiang/SuperSocket)
* [Topshelf](https://github.com/Topshelf/Topshelf)
