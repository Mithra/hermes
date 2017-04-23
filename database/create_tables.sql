USE [HermesMaster]
GO


CREATE TABLE [application]
( 
	[application_id]     int  NOT NULL  IDENTITY ( 1,1 ) ,
	[application_name]   nvarchar(128) COLLATE SQL_Latin1_General_CP1_CI_AS  NOT NULL 
)
go

ALTER TABLE [application]
	ADD CONSTRAINT [PK_application] PRIMARY KEY  CLUSTERED ([application_id] ASC)
go

CREATE TABLE [channel]
( 
	[channel_id]         bigint  NOT NULL  IDENTITY ( 1,1 ) ,
	[channel_name]       nvarchar(128) COLLATE SQL_Latin1_General_CP1_CI_AS  NOT NULL ,
	[application_id]     int  NOT NULL 
)
go

ALTER TABLE [channel]
	ADD CONSTRAINT [PK_channel] PRIMARY KEY  CLUSTERED ([channel_id] ASC)
go

CREATE TABLE [notification]
( 
	[notification_id]    bigint  NOT NULL  IDENTITY ( 1,1 ) ,
	[notification_code]  varchar(128)  NOT NULL ,
	[notification_time]  datetime  NOT NULL ,
	[notification_message] nvarchar(256) COLLATE SQL_Latin1_General_CP1_CI_AS  NOT NULL ,
	[channel_id]         bigint  NOT NULL 
)
go

ALTER TABLE [notification]
	ADD CONSTRAINT [PK_notification] PRIMARY KEY  CLUSTERED ([notification_id] ASC)
go

CREATE TABLE [notification_client_receipt]
( 
	[notification_id]    bigint  NOT NULL ,
	[client_id]          uniqueidentifier  NOT NULL 
)
go

ALTER TABLE [notification_client_receipt]
	ADD CONSTRAINT [PK_notification_client_receipt] PRIMARY KEY  CLUSTERED ([notification_id] ASC,[client_id] ASC)
go

CREATE TABLE [notification_tag]
( 
	[notification_tag_id] bigint  NOT NULL  IDENTITY ,
	[notification_id]    bigint  NOT NULL ,
	[notification_tag_key] varchar(128)  NOT NULL ,
	[notification_tag_value] varchar(128)  NOT NULL 
)
go

ALTER TABLE [notification_tag]
	ADD CONSTRAINT [PK_notification_tag] PRIMARY KEY  CLUSTERED ([notification_tag_id] ASC)
go

ALTER TABLE [notification_tag]
	ADD CONSTRAINT [UX_notification_tag] UNIQUE ([notification_id]  ASC,[notification_tag_key]  ASC)
go


ALTER TABLE [channel] WITH CHECK 
	ADD CONSTRAINT [FK_channel__application] FOREIGN KEY ([application_id]) REFERENCES [application]([application_id])
go

ALTER TABLE [channel]
	  WITH CHECK CHECK CONSTRAINT [FK_channel__application]
go


ALTER TABLE [notification] WITH CHECK 
	ADD CONSTRAINT [FK_notification__channel] FOREIGN KEY ([channel_id]) REFERENCES [channel]([channel_id])
go

ALTER TABLE [notification]
	  WITH CHECK CHECK CONSTRAINT [FK_notification__channel]
go


ALTER TABLE [notification_client_receipt] WITH CHECK 
	ADD CONSTRAINT [FK_notification_client_receipt__notification] FOREIGN KEY ([notification_id]) REFERENCES [notification]([notification_id])
go

ALTER TABLE [notification_client_receipt]
	  WITH CHECK CHECK CONSTRAINT [FK_notification_client_receipt__notification]
go


ALTER TABLE [notification_tag]
	ADD CONSTRAINT [FK_notification_key__notification] FOREIGN KEY ([notification_id]) REFERENCES [notification]([notification_id])
go
