USE [HermesMaster]
GO
/****** Object:  Table [dbo].[application]    Script Date: 23/04/2017 00:00:54 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[application](
	[application_id] [int] IDENTITY(1,1) NOT NULL,
	[application_name] [nvarchar](128) NOT NULL,
 CONSTRAINT [PK_dbo.application] PRIMARY KEY CLUSTERED 
(
	[application_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[channel]    Script Date: 23/04/2017 00:00:54 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[channel](
	[channel_id] [int] IDENTITY(1,1) NOT NULL,
	[channel_name] [nvarchar](128) NOT NULL,
	[application_id] [int] NOT NULL,
 CONSTRAINT [PK_dbo.channel] PRIMARY KEY CLUSTERED 
(
	[channel_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[notification]    Script Date: 23/04/2017 00:00:54 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[notification](
	[notification_id] [bigint] IDENTITY(1,1) NOT NULL,
	[notification_time] [datetime] NOT NULL,
	[notification_message] [nvarchar](256) NOT NULL,
	[channel_id] [int] NOT NULL,
 CONSTRAINT [PK_dbo.notification] PRIMARY KEY CLUSTERED 
(
	[notification_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[notification_client_receipt]    Script Date: 23/04/2017 00:00:54 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[notification_client_receipt](
	[notification_id] [bigint] NOT NULL,
	[client_id] [uniqueidentifier] NOT NULL,
 CONSTRAINT [PK_dbo.notification_client_receipt] PRIMARY KEY CLUSTERED 
(
	[notification_id] ASC,
	[client_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
ALTER TABLE [dbo].[channel]  WITH CHECK ADD  CONSTRAINT [FK_dbo.channel_dbo.application_application_id] FOREIGN KEY([application_id])
REFERENCES [dbo].[application] ([application_id])
GO
ALTER TABLE [dbo].[channel] CHECK CONSTRAINT [FK_dbo.channel_dbo.application_application_id]
GO
ALTER TABLE [dbo].[notification]  WITH CHECK ADD  CONSTRAINT [FK_dbo.notification_dbo.channel_channel_id] FOREIGN KEY([channel_id])
REFERENCES [dbo].[channel] ([channel_id])
GO
ALTER TABLE [dbo].[notification] CHECK CONSTRAINT [FK_dbo.notification_dbo.channel_channel_id]
GO
ALTER TABLE [dbo].[notification_client_receipt]  WITH CHECK ADD  CONSTRAINT [FK_dbo.notification_client_receipt_dbo.notification_notification_id] FOREIGN KEY([notification_id])
REFERENCES [dbo].[notification] ([notification_id])
GO
ALTER TABLE [dbo].[notification_client_receipt] CHECK CONSTRAINT [FK_dbo.notification_client_receipt_dbo.notification_notification_id]
GO
