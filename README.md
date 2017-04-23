# Hermes

Hermes is a generic websocket notification service giving backend services the ability to submit real-time notifications to user facing web interfaces.

## Use cases

* Asynchronous jobs and status monitoring
* Any kind of actions performed behind the scene that provide value to users (ex: "your request has been approved/denied", "your job has been picked up", etc)

## Features

* REST API to manage notifications (creation, aknowledgement, ...)
* Websocket endpoint to get real-time notifications in web applications
* Uses RabbitMQ to ensure scalablility
* Notifications can either be transient or permanent (and then stored in database for future usage)
* Permanent notifications can be "aknowledged" per user

## Data model

* Application: regroups a collection of channels
* Channel: represents a topic of interest
* Notification: events that can be pushed to channels
* Notification tag: key-value tags that can be added to notifications
* Notification client receipt: user aknowledgement of specific notifications

## How it works

![Hermes flow](https://raw.githubusercontent.com/Mithra/hermes/master/doc/hermes_flow.png)

### Consumer

Users register to the Hermes WS endpoint with the following message:

```json
register 
{
	"ApplicationName": "myapp", 
	"ClientId": "b239d7df-bfeb-4310-a840-43846a87efac", 
	"Channels": ["channel1", "channel2"], 
	"FetchLast": 10, 
	"LastNotificationId": 42
}
```

Users will then receive (at most) the last 10 "unacknowledged" notifications from the mentioned channels after the last received notification (if specified). The "LastNotificationId" field is meant to handle websocket reconnections without reloading a page.

Once registered, users will now receive all newly created notifications from the corresponding channels. Notifications will be received as json objects:
```json
{
	"Type": "NewNotification",
	"Data": {
		"Id": 1,
		"Persistent": true,
		"ChannelId": 1,
		"ChannelName": "channel1",
		"ApplicationId": 1,
		"ApplicationName": "myapp",
		"Time": "2017-04-23T20:44:28.533",
		"Code": "new_notification",
		"Message": "Test new message in channel 1",
		"Tags": {
			"test_key_1": "test_value_1",
			"test_key_2": "test_value_2"
		}
	}
}
```

### Producer

Backend applications can create notifications by calling Hermes API:

```json
{
  "applicationName": "myapp",
  "channelName": "channel1",
  "code": "job_create",
  "message": "New job created",
  "persistent": true,
  "tags": {
    "test_key_1": "test_value_1",
    "test_key_2": "test_value_2"
  }
}
```

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
