USE [ClothingShopDb]
GO
/****** Object:  Table [dbo].[Clothes]    Script Date: 09.10.2026 22:48:24 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Clothes](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](100) NOT NULL,
	[Price] [decimal](10, 2) NOT NULL,
	[Description] [nvarchar](500) NULL,
	[PurchaseCount] [int] NOT NULL,
	[Type] [nvarchar](50) NOT NULL,
	[IsAvailable] [bit] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[OrderItems]    Script Date: 09.10.2026 22:48:24 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[OrderItems](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[OrderId] [int] NOT NULL,
	[SizeId] [int] NOT NULL,
	[Quantity] [int] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Orders]    Script Date: 09.10.2026 22:48:24 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Orders](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[CreatedAt] [datetime2](7) NOT NULL,
	[Status] [nvarchar](30) NOT NULL,
	[UserId] [int] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Sizes]    Script Date: 09.10.2026 22:48:24 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Sizes](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ClothingId] [int] NOT NULL,
	[Letter] [nvarchar](5) NOT NULL,
	[Number] [int] NOT NULL,
	[Quantity] [int] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Users]    Script Date: 09.10.2026 22:48:24 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Users](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Login] [nvarchar](50) NOT NULL,
	[PasswordHash] [nvarchar](500) NOT NULL,
	[Phone] [nvarchar](20) NOT NULL,
	[Address] [nvarchar](200) NOT NULL,
	[Role] [nvarchar](20) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET IDENTITY_INSERT [dbo].[Clothes] ON 

INSERT [dbo].[Clothes] ([Id], [Name], [Price], [Description], [PurchaseCount], [Type], [IsAvailable]) VALUES (1, N'Футболка оверсайз', CAST(1990.00 AS Decimal(10, 2)), N'Хлопок 100%', 15, N'T-Shirt', 1)
INSERT [dbo].[Clothes] ([Id], [Name], [Price], [Description], [PurchaseCount], [Type], [IsAvailable]) VALUES (2, N'Футболка классик', CAST(1490.00 AS Decimal(10, 2)), N'Хлопок 95%, эластан 5%', 30, N'T-Shirt', 1)
INSERT [dbo].[Clothes] ([Id], [Name], [Price], [Description], [PurchaseCount], [Type], [IsAvailable]) VALUES (3, N'Худи с принтом', CAST(3490.00 AS Decimal(10, 2)), N'Флис', 8, N'Hoodie', 1)
SET IDENTITY_INSERT [dbo].[Clothes] OFF
GO
SET IDENTITY_INSERT [dbo].[Sizes] ON 

INSERT [dbo].[Sizes] ([Id], [ClothingId], [Letter], [Number], [Quantity]) VALUES (1, 1, N'S', 46, 5)
INSERT [dbo].[Sizes] ([Id], [ClothingId], [Letter], [Number], [Quantity]) VALUES (2, 1, N'M', 48, 10)
INSERT [dbo].[Sizes] ([Id], [ClothingId], [Letter], [Number], [Quantity]) VALUES (3, 1, N'L', 50, 7)
INSERT [dbo].[Sizes] ([Id], [ClothingId], [Letter], [Number], [Quantity]) VALUES (4, 2, N'S', 46, 3)
INSERT [dbo].[Sizes] ([Id], [ClothingId], [Letter], [Number], [Quantity]) VALUES (5, 2, N'M', 48, 12)
INSERT [dbo].[Sizes] ([Id], [ClothingId], [Letter], [Number], [Quantity]) VALUES (6, 2, N'L', 50, 6)
INSERT [dbo].[Sizes] ([Id], [ClothingId], [Letter], [Number], [Quantity]) VALUES (7, 2, N'XL', 52, 4)
INSERT [dbo].[Sizes] ([Id], [ClothingId], [Letter], [Number], [Quantity]) VALUES (8, 3, N'M', 48, 5)
INSERT [dbo].[Sizes] ([Id], [ClothingId], [Letter], [Number], [Quantity]) VALUES (9, 3, N'L', 50, 3)
SET IDENTITY_INSERT [dbo].[Sizes] OFF
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [UQ__Users__5E55825BEC863F15]    Script Date: 09.10.2026 22:48:24 ******/
ALTER TABLE [dbo].[Users] ADD UNIQUE NONCLUSTERED 
(
	[Login] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
ALTER TABLE [dbo].[Clothes] ADD  DEFAULT ((0)) FOR [PurchaseCount]
GO
ALTER TABLE [dbo].[Clothes] ADD  DEFAULT ((1)) FOR [IsAvailable]
GO
ALTER TABLE [dbo].[Sizes] ADD  DEFAULT ((0)) FOR [Quantity]
GO
ALTER TABLE [dbo].[Users] ADD  DEFAULT ('User') FOR [Role]
GO
ALTER TABLE [dbo].[OrderItems]  WITH CHECK ADD FOREIGN KEY([OrderId])
REFERENCES [dbo].[Orders] ([Id])
GO
ALTER TABLE [dbo].[OrderItems]  WITH CHECK ADD FOREIGN KEY([SizeId])
REFERENCES [dbo].[Sizes] ([Id])
GO
ALTER TABLE [dbo].[Orders]  WITH CHECK ADD FOREIGN KEY([UserId])
REFERENCES [dbo].[Users] ([Id])
GO
ALTER TABLE [dbo].[Sizes]  WITH CHECK ADD FOREIGN KEY([ClothingId])
REFERENCES [dbo].[Clothes] ([Id])
GO
