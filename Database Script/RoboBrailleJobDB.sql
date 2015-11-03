USE [RoboBrailleJobDB]
GO
/****** Object:  Table [dbo].[__MigrationHistory]    Script Date: 11/3/2015 1:11:49 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[__MigrationHistory](
	[MigrationId] [nvarchar](150) NOT NULL,
	[ContextKey] [nvarchar](300) NOT NULL,
	[Model] [varbinary](max) NOT NULL,
	[ProductVersion] [nvarchar](32) NOT NULL,
 CONSTRAINT [PK_dbo.__MigrationHistory] PRIMARY KEY CLUSTERED 
(
	[MigrationId] ASC,
	[ContextKey] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Jobs]    Script Date: 11/3/2015 1:11:49 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Jobs](
	[Id] [uniqueidentifier] NOT NULL,
	[UserId] [uniqueidentifier] NOT NULL,
	[FileName] [nvarchar](512) NOT NULL,
	[FileExtension] [nvarchar](512) NOT NULL,
	[MimeType] [nvarchar](max) NOT NULL,
	[Status] [int] NOT NULL,
	[SubmitTime] [datetime] NOT NULL,
	[FinishTime] [datetime] NOT NULL,
	[ResultContent] [varbinary](max) NULL,
	[SourceDocumnetFormat] [int] NULL,
	[TargetDocumentFormat] [int] NULL,
	[SpeedOptions] [int] NULL,
	[FormatOptions] [int] NULL,
	[BrailleFormat] [int] NULL,
	[Contraction] [int] NULL,
	[OutputFormat] [int] NULL,
	[DaisyOutput] [int] NULL,
	[EbookFormat] [int] NULL,
	[size] [int] NULL,
	[MSOfficeOutput] [int] NULL,
	[Discriminator] [nvarchar](128) NOT NULL,
 CONSTRAINT [PK_dbo.Jobs] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[ServiceUsers]    Script Date: 11/3/2015 1:11:49 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[ServiceUsers](
	[UserId] [uniqueidentifier] NOT NULL,
	[UserName] [nvarchar](32) NOT NULL,
	[ApiKey] [varbinary](max) NOT NULL,
	[FromDate] [datetime] NOT NULL,
	[ToDate] [datetime] NOT NULL,
	[EmailAddress] [nvarchar](max) NOT NULL,
 CONSTRAINT [PK_dbo.ServiceUsers] PRIMARY KEY CLUSTERED 
(
	[UserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
INSERT [dbo].[__MigrationHistory] ([MigrationId], [ContextKey], [Model], [ProductVersion]) VALUES (N'201511031159177_RBJobDB', N'RoboBraille.WebApi.Migrations.Configuration', 0x1F8B0800000000000400D55C5B73E3B6157EEF4CFF03874F4967D7B2E46CB2EB9192916569E3C4B25C4B7BC9D30E44423276790B09BA523BFD657DE84FEA5FE801AF00AF204D5BF2F8C5220E3E00E786EB39FFFBCF7F87BFEC4C4379C0AE476C6BA4F64F4E55055B9AAD136B3B527DBA79FD56FDE5E7BFFE6538D5CD9DF231A63B637450D3F246EA3DA5CE79AFE769F7D844DE894934D7F6EC0D3DD16CB38774BB37383D7DD7EBF77B182054C05294E19D6F5162E2E007FC9CD896861DEA23636EEBD8F0A2EF50B20C50951B6462CF411A1EA977F6DABE7011310C7CF209AFC70E39092BA9CAD820083AB4C4C646559065D91451E8EEF9070F2FA96B5BDBA5031F90B1DA3B18E836C8F070348CF3945C7644A70336A25E5A3186D27C8FDA6643C0FE59C4A25EB67A2B46AB090B8189536036DDB351078C1CA9BFD96B55C9B6733E315C4653C1E213A8F84AC917BF4A34031488FDBD5226BE417D178F2CEC531719AF945B7F6D10ED77BC5FD9DFB035B27CC30089AD3D28D5E848A5AE8FF94E43B78156F8009F6E5DDBC12EDDDFE14D34942B5D557A62BD5EB662528DAB138EF4BD4FE0FF1BE80B5A1B3851098E294B6ABBF83DB6B08B28D66F11A5D8B518040E989A6B3CD314A89E5BDB5C35C48C1898FD1783802A8371AACA1CEDAEB1B5A5F723F54D7FA02A33B2C37AFC2542FE6011B0651CB1B74DCBD31DC59617A8E173373F070FC1DAAC6819FE7D8A9697207CDF4BDA057FC2343FFEDA14CC5F9B84AE482AC14B50A5F077638158C4BBEF04EA0E7B60A1E07841BC3446BB201672F7721CCEC0DFA007B20D4CA6C00254E50E1B41A1774F9C94A35FC2C2996B9B77B6113AA6E0DB97A5EDBB1A1BA59D295821778BA9D8FCB0977AB84ABFB7C4EE03D170D86A1BFFC70174E9075BF8BDD8B134F57D520EA95BFF57E3BCCE9EC479803480156D14BBA94D82F2322B7CB445AEEC4E60A62668E458D75DEC79CFE0394BED1E2CD66B6EF7B179E7EC3E7608ADEC7EAC69C00D02C30087172D765BAF814AC03A5D175D200F8B938F5A3DC704DCB9B435DFB4309DD9AE89A8307D8504714153CD0C8412A083D517A02F7CEAF8942D17C2C2A9E59B35ADC8CBCED789DD5E5851EDC34AC7C1585F38811D087C0B7A179436773B01A34B41A5242D2D83886FADA590D63FA81CA2660B143853D250166C15C57631FC2A9943158A1B424796556A729D8AF912116FDF5AC871ED838A38E844C81B815DC2F76EB835BDB0ED6FADB915D73E28B7A66BE844817209DFBBE1D6AFABF935B56F2F67AD39C6231C946B1EF92716D8758BA078197CED8E592B7B8577F451DC4A209E9A5DE22873C3992F179B0D6C9B5A0F860338A8E4E37E1438986C51378AB0D0DC0E56AC599403AB43B09B245AEBE12C91E9C0C714E6B9C7E39BE2596A7C3074E5CD0CB44D4F965B9EB086709D0EEA83A563D7D8C326306CFFCAA2B0ED16757D8ECD3576E399C1756DD8A17D44860FBF4E73CB1481F8D2B67042DBAFA685C1B994AD7723F24135F9DF7DEC73D467D5D460AEC1DE8CED75A31A3F54D798204BC3C0E0B4893779530DE55DA103E2EEAA0B35E0110FAB09BAADC9CA164877B282BDA7A6212B56A09595A74B376592CC9252B249697FACA54D487FAA26DD72A46FAB49BF3AA99EBEAB26753895EED798E3DA7452DA1A91399CC4FA3522D379DA1A917D1D7C4B696B64F6D519A4B43542FBEA707DA8119AFEF521A5AD919AA3A752EBD7886DC7EE1A63DA1AB9012D67123582D3BCB4C3833AC13934A5AD111CD0729D386BE1E08A0F781EE5E8C617177FFC51787274587FB798CDBECC979F6C57979DFEC21AD31D4C24B29E9255B95BCD649D2523FF3CBF96F5978C1CF649B23E33221FCB3A4E46CF3616B2CE93D14F961F651D282367BB17592FCAE86FEC74812BEB5283615CCD66D27E9555F8EDF6BDB46F0DE807BF4BFB57460FEA895D0B19F15A42D2DDB2AA978BC967699F1B68D4F5F2B3B4E30D067371F57E20ED7E598DF1F56A21ED835985E9ED878B52472CE5AAF833D52E566229DE61FDD20DD30843D627CD904765BD0BA3C5AEACB708A83D69E35F1AF63F78F2D732F469675ED7F48691A7C483B6FAD2E5D29D033CACC6CC9D33D9F9E8137A90D5964F2692559531D2CA9CB8946832A7F25D0847803CAC783CB2D36D693BC2647B4F79FA473054B890E890AB1CEE6159BB09AAC9A9BE075ED590557E83DD84CA2EC302E281ACB16C5DA4E353D9355840DD97559E807AB096DEC206E44476011692CB2EBB0CFC808D53E9455740DE975E7305E403E92557407E26BDCCF23D6E6ACA2EB19A6CA43AF4683CE2A15728DC3160DD5A5AA3713F650D357A98226BAAB778236BA837E3C94599994A4955B85DEC42A81CE06165BA42C637A0669795B2729A3AFEFAECD37C512628298E0A37905D7094033C2C474D7B4D64598981958F62237733D9051313B8C3B210F565198806B2EE029DC97A0BF483EC948EDEC84EE7E847D9A91CFD243B8BA3B7B213387A273B79A37E93999BF2DBCAECCC2DA5BFD95BD52E9458C43CAC260BC7CE3557103BE90DBD70B7D2E0C2446A4B31F63C5B2301B7D31BD0F041A7D8CCD4D295B257DD99FBD539709738C04F42F723F56FB91E1740250F45B9378EFCEB6E11B22FF60D2017D625060DC5CA580B637426C8D3909E7F90054CD0C52F7778835DF6FC1919B0E361012CC4A259FC5B97581A719051D2F10CBD22FFC69BF52941CF965C62075BEC6D7609EB1FD76C829E61511D47863D4E6B4473676F03D8BE116A243AC959E425A228085260DA9FB376E86964F05E61045108BFC434D5544F55D207096577FC055539E52A8210742F03C58D5DE84AF40A9A2BCEBE90CE0AA0C2A492FEC6A3CC49AFC288B8CAE238B3B6200E25E32B04512665C35E18C3177D18F64A82FD8673E438E039B9E0BFE88BB20C23FF26AF97CD63E1CC10A3A779052171496F9396A8EDA22DCE94B2179E3A9E11D7A34C1FD7884D3D13DDCC9195286E8966C58D72BA999759AC673131FB3FAC70E5B1FF179BEF2A1F937C5F60F211D60C86CA1E7407A3C6A202E56B292C181319C82D08A69BD8866F5AE9EFACEE95D78E7D0D8F50E67FCA51D210391E27FDDA0C890B79CBC27145F298691C1B0F977E95478A1F1AF138F1B706285C409A80C47D6FC2B134264D6457FA5D1E2D1396C603668AF298C35E46B173F357CE9C9AD85BE38890E3B6BCE29011411F0A29E45B280E1BE15B28A6286C0138AB9360D1192D564B637AF8062E89A7B9C42416A2766E623EACCEC4612DC7AD2442E48AA01C4249037721C6AD081E432C925483243CE865C89D0BA5396AC967EEDE78A04C913CA670EFC4230A05F278E2613A0F2896C869121FE4F432742989D7396A4D120EC705C6F20572324A23945E86849218A1A3969070D8CEC3080572124AA3A25E868484B8A4A396521876C4D70FBFC8C9458CBF7A39B249C3A08E453885ACE5A2B55E066FF998AC63E16CF1EE39730520ECA1336572F212C2D15E86B4727160C722B21C73F3116B2F83C35C50DAD1F2960F9C7B56AEC6E5E9A1A5D491A67096DC8D98F873F6E672AAACFDB4479469229C2C4ED323CA38B30D8F137F6BB00F4FD2D6085BF0E46B83631E3B8F137F6BB00615D2D6088B50A1E499953B77AF9125495A4FEE3732F718C3E84EA13EB361EE922124511560D503D1D905C372EF516C9E308293E59FC6C420C159684C304716D9608F8677D2EAE094BD7F13B2221E4F86C29EE7E946C19D8C185AFBFC29017D8BFCE96312E4B7DA107641579B02AB614E0F310557BEBDDC35E795A5E3DD48FD5750FF5CB9FAFC258478A52C5C90FAB972AAFCFBD16905AD07E46AF7C8CD67F6EB226B6067E8D9A48031F07726DA7DFFC8447FC46A9EFB259FDD4F072DA1DD64F76B0D5598DD0F38B50EF2A0E57815A6FAAA1968458AA90CE364E0AA724AB5802B4AB1D402A630AB520B9CC24C432D700A320BB540294A22D402A620B74E0B94829C332D50F81C2C2DAA1727F268C31371E95DE1ECFA83B75D6505295DC63E67EAC8E799276B67A8B3E65388981AB2DC2BB64AFFD8DA658BE91F5BC314A57F6C32479629618B677FC29BA803BCD3131E3C357F74D8EA995FF5FEF4A99EF8BD8C677DF9475425A708DCF2BFE4B15EB833023359DB20D850998B9EBD553DE3AB7BC557D446E5EBB8A778E757D0DFDC4946CD53BFECDBC0A779D597DFE582FE7059FE416D3DB24D21D8E99685354173129A2B6B63C70A9CE9514C929DD03145E033D1D885D908164C059980A6E61AEB575638E5C390B1B936842313660855ED074F17C53E0FA3A5621743806E12E6F617D6854F8C34FBC4ACC0499740300B8B2660264BCA26E2ED3E410A03B4648022F6258E61854DC700306F612DD1036ED33750D56BBC45DA3E3EAC2807A91784C8F6E125415B17995E8491D6879FA0C3BAB9FBF9FF755CC735EC620000, N'6.1.3-40302')
INSERT [dbo].[ServiceUsers] ([UserId], [UserName], [ApiKey], [FromDate], [ToDate], [EmailAddress]) VALUES (N'd2b97532-e8c5-e411-8270-f0def103cfd0', N'TestUser', 0x41AE767BF3DE11E480300C8BFD2336CD, CAST(0x0000A41300000000 AS DateTime), CAST(0x0000AB3500000000 AS DateTime), N'source@sensus.dk')
ALTER TABLE [dbo].[Jobs] ADD  DEFAULT (newsequentialid()) FOR [Id]
GO
ALTER TABLE [dbo].[ServiceUsers] ADD  DEFAULT (newsequentialid()) FOR [UserId]
GO
ALTER TABLE [dbo].[Jobs]  WITH CHECK ADD  CONSTRAINT [FK_dbo.Jobs_dbo.ServiceUsers_UserId] FOREIGN KEY([UserId])
REFERENCES [dbo].[ServiceUsers] ([UserId])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Jobs] CHECK CONSTRAINT [FK_dbo.Jobs_dbo.ServiceUsers_UserId]
GO
