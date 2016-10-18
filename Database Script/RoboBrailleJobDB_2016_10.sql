USE [master]
GO
/****** Object:  Database [RoboBrailleJobDB]    Script Date: 10/17/2016 6:59:12 PM ******/
CREATE DATABASE [RoboBrailleJobDB]
 CONTAINMENT = NONE
 ON  PRIMARY 
( NAME = N'RoboBrailleJobDB', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL11.CVPSERVER\MSSQL\DATA\RoboBrailleJobDB.mdf' , SIZE = 420864KB , MAXSIZE = UNLIMITED, FILEGROWTH = 1024KB )
 LOG ON 
( NAME = N'RoboBrailleJobDB_log', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL11.CVPSERVER\MSSQL\DATA\RoboBrailleJobDB_log.ldf' , SIZE = 241216KB , MAXSIZE = 2048GB , FILEGROWTH = 10%)
GO
ALTER DATABASE [RoboBrailleJobDB] SET COMPATIBILITY_LEVEL = 110
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [RoboBrailleJobDB].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [RoboBrailleJobDB] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [RoboBrailleJobDB] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [RoboBrailleJobDB] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [RoboBrailleJobDB] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [RoboBrailleJobDB] SET ARITHABORT OFF 
GO
ALTER DATABASE [RoboBrailleJobDB] SET AUTO_CLOSE OFF 
GO
ALTER DATABASE [RoboBrailleJobDB] SET AUTO_CREATE_STATISTICS ON 
GO
ALTER DATABASE [RoboBrailleJobDB] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [RoboBrailleJobDB] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [RoboBrailleJobDB] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [RoboBrailleJobDB] SET CURSOR_DEFAULT  GLOBAL 
GO
ALTER DATABASE [RoboBrailleJobDB] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [RoboBrailleJobDB] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [RoboBrailleJobDB] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [RoboBrailleJobDB] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [RoboBrailleJobDB] SET  DISABLE_BROKER 
GO
ALTER DATABASE [RoboBrailleJobDB] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [RoboBrailleJobDB] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [RoboBrailleJobDB] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [RoboBrailleJobDB] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO
ALTER DATABASE [RoboBrailleJobDB] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [RoboBrailleJobDB] SET READ_COMMITTED_SNAPSHOT OFF 
GO
ALTER DATABASE [RoboBrailleJobDB] SET HONOR_BROKER_PRIORITY OFF 
GO
ALTER DATABASE [RoboBrailleJobDB] SET RECOVERY FULL 
GO
ALTER DATABASE [RoboBrailleJobDB] SET  MULTI_USER 
GO
ALTER DATABASE [RoboBrailleJobDB] SET PAGE_VERIFY CHECKSUM  
GO
ALTER DATABASE [RoboBrailleJobDB] SET DB_CHAINING OFF 
GO
ALTER DATABASE [RoboBrailleJobDB] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO
ALTER DATABASE [RoboBrailleJobDB] SET TARGET_RECOVERY_TIME = 0 SECONDS 
GO
EXEC sys.sp_db_vardecimal_storage_format N'RoboBrailleJobDB', N'ON'
GO
USE [RoboBrailleJobDB]
GO
/****** Object:  StoredProcedure [dbo].[RoboBrailleCleanUp]    Script Date: 10/17/2016 6:59:12 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO

CREATE PROCEDURE [dbo].[RoboBrailleCleanUp] AS
DELETE FROM dbo.Jobs 
WHERE (SELECT DATEADD(MONTH,1,Jobs.FinishTime)) < CURRENT_TIMESTAMP;

GO
/****** Object:  Table [dbo].[__MigrationHistory]    Script Date: 10/17/2016 6:59:12 PM ******/
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
/****** Object:  Table [dbo].[Jobs]    Script Date: 10/17/2016 6:59:12 PM ******/
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
	[InputFileHash] [varbinary](max) NULL,
	[ResultContent] [varbinary](max) NULL,
	[DownloadCounter] [int] NOT NULL,
	[ResultFileExtension] [nvarchar](max) NULL,
	[ResultMimeType] [nvarchar](max) NULL,
	[TargetDocumentFormat] [int] NULL,
	[FormatOptions] [int] NULL,
	[OutputFormat] [int] NULL,
	[DaisyOutput] [int] NULL,
	[EbookFormat] [int] NULL,
	[MSOfficeOutput] [int] NULL,
	[VideoUrl] [nvarchar](max) NULL,
	[SubtitleLangauge] [nvarchar](max) NULL,
	[SubtitleFormat] [nvarchar](max) NULL,
	[AmaraVideoId] [nvarchar](max) NULL,
	[Discriminator] [nvarchar](128) NOT NULL,
 CONSTRAINT [PK_dbo.Jobs] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[ServiceUsers]    Script Date: 10/17/2016 6:59:12 PM ******/
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
INSERT [dbo].[__MigrationHistory] ([MigrationId], [ContextKey], [Model], [ProductVersion]) VALUES (N'201607291157420_RBJobDb', N'RoboBraille.WebApi.Migrations.Configuration', 0x1F8B0800000000000400E55CDB72E336127DDFAAFD07169F922DC7F225D99DB8A4A4645D269A5896D79267264F531009C9A821018504BD76A5F265FBB09FB4BFB0CD3BC09B408AB6A4DAF28B0C340E80EE4603040EF0DF7FFFA7FBF3B36D694FD87109A33DFDFCF44CD730359849E8BAA77B7CF5DD3BFDE79FFEFA97EEC8B49FB58FB1DCA52F0725A9DBD31F39DF5C753AAEF1886DE49EDAC47098CB56FCD460760799AC737176F663E7FCBC830142072C4DEBDE7B94131B07FFC0BF03460DBCE11EB2A6CCC4961BA543CE3C40D56E918DDD0D32704FBF674B76ED206259F8F4135EF637E4342CA46B7D8B2068D01C5B2B5D4394328E3834F7EAC1C573EE30BA9E6F2001598B970D06B915B25C1C75E32A1557EDD1D985DFA34E5A3086323C9733BB26E0F965A4A24EB6782345EB890A41892350367FF17B1D28B2A77F604B5DCBD67335B01C5FA642C5A750F044CB679F249E010EE4FF9D6803CFE29E837B147BDC41D68976E72D2D62FC8A5F16EC2BA63DEA5916586CE942AEC17B3A773C2C361A9A0DB2520224DD396C831DFE728F57515726A6AE75E4729D6CC1A4985026ECE97B8FC0EF5B680B5A5A387109412973CE1CFC1E53EC208ECD3BC43976A80F8103A5E62ACF5405AEE76CADAE1A624C2CECFF8A41C0956170EADA143DDF60BAE68F3DFD87F30B5D1B93676CC62911F20325309671A4DE26358F9E39A66EE0866F5DFD1422845F6745CDF0F3356A9E83F13D37A917E289EFF9716A5D306F6913BE20A90587E04AE1FFB50D4289FBD80AD4846E3CEE1BF817E43EC668D78422E7454DC3D5F0F7D8850000711DBC87B70F3F64FFA21643E680C144829DB88209E59717B55511B655D5D9555D4EA5CE163D3C53DD2D7A22EB20821504245DBBC75690E93E924DEAE05FC2CCB1C3EC7B6685F34490F665CE3CC7F09BC932190BE4AC3197ABEF76D209A7721A9A63E7891838ACB5C9742400B4392D359886E2385F772A529A1FDA9D8EB6CC2597AF12CBC11AA08A2681A06E8804E7F583E2CE0172C15A8119D9E0917DD374B0EBBEC144563AEE61C4BAF5C77D3CBC73E33E0E088DC67DDF30401B04BA011344F4EDD178495A02D6EA32F51AB9585E0BE8D5BE13A86DC80CCF867139668E8DB8B49A98793C9A7EC3CC11F5EC2DC655D7AE6712D65C9D51E9BDEA2F54CA6C13B8AAA4B8A079B142DBD157D4C7C61A4BCBEF5567914B95FA5AAB3A9B22FED8586151E1D7D49668D34E654F8688B82F8DBB1297DEABE583468456960C2FA5B763F7D135635F1B6B2B2EBD576D8D96D088826122A5B7A3AD5F16D31BCEEE86E3C61A13115E5B6B5B068ADF94055BE067BE536F12883D77673A9FAD56F0EDD03C8AA5007BF5E7B81D0501209BD58E57CF0CA785655B1665CFEEF09198B8F9A2C9CF09104E639CBDBA44D08807C7DAC3C699B704C55AF806D135F2D6FBD8BA8B5A9089F06F567FDF460E0A0C907ED6B7B4ADA33C4283FD01623476E739B2379098C2BCF5E0F46CF9B022DE799DB8630BADD3A39B864718215CAB9D7AA02676AC1730B3B813297BCA14DB4BECC4CB10C761F0CDFD11591EFC7796732B4978C8284E64CFAB65A1730EC766227E512DFE4F0F7B82F465B534387BF0B5ED7B7354E2FBEA1203440D0C0A4EABF821EFD5A1BD2B7CA0F87B792777E85F5FFFF65BE187F87E1D63361E7F99CE3F31C754F58EB0C4E819F4ACEA247E91FBC558D5497CF1CFD31B552FF1C561CDAAEA229178BFCC438AE4FD456422FFF7EDF283F9C744FC1FDBC5FD956A22FF6EBBFC2D4B173349B91F15BA3119A76A3A57B0F387BBF7A9BC82913F5CFC9ACA2B5879E21FA75064C5D3675C54C1E2C3D9E0735A40C1E69F6FE6420105A37FB89EBCBF484B2898BD7FB398A505140C3FBA7BB84E0BBC6B10AAA41DAA36262C0170BF9169BAB9548D2F9FD0936A6CF96423D540D14746D9A0AC338BB4681B1171BFC6B9159708DB0289C1E376AA1A345A96AA1AF50EAF54278BDBFEE0BACC0194AC2A6D73B561540170BF365D20EB2B48FBBB66AA761A6DBCE5E5A7E9ACCC504A1A95B6C2DAD0A800B85F8DDA6C4954558941953BA931BBFDD2862665CCFD2A7363AE5475C985D5DA96D0F1C86D4B754270F8AADE84D0775D669040DBE9D76578FC295733A2A656C681C87CBB4E41BB6403FA848FF09EFEB75C8B0BA09263D5144AE242C890E772DB00724687D8C21C6B7D2324180E906B2033BF39014A30E5947BBCC28E4F1640162C597DF61DA13CCF6F20D4201B6495343C235F8311E1B72941CFE60CF106539FC950A2FADDAA4DD0332ADAA6916E47F09AFCBE8BCF718212894F0A237288380A2850BEF7E7463BB4341AF06E21FD31849F632E9DE0A79B3D65FB27054505E72A82907C2F0325F45D6A4AC41910B2B37C82AC012A8654D2DEB89739EB550C22A1B0DCCFEC5890BB52B0B1969832C9EB7642027294D0ED943095BB53B4D940E41498CB518A360F69CB83EFE6F589BC7688D131DC023E6FD2DAA426CE1CB4C6995CA81A5A3A268ECB7D7F5C227FEA1998764EACC4714B3C2BAE54F0CDBCCD623F8B85FDDF11FDD0F57FCF56DF546ED47D5B30E423AC3174D5275704BDC6B203E54B693E931C59C82960020F98E5D9B48C4D5C553A8E35224259FC294749F9BD224E9A5A0F49A03066E1842C75CC94A228C2A5A9EA48F126AE8813A7D54011D8B41292905E476329A15656579AAE8E96E1D44A8E2567A9636688B42266264B1D33C79E1551739975DB5AE185850275F18B3D329B9747ED7632A123B742C805AC3A11AD3643EDB0635B31854DC42B9628AC01FA6D9260D11D2DD64B1980922712D770884D28E2ACD00FF767D1986277D026CCB0E8A4F82667291A2D21161E87950412D841DB49DE961471E41C352B89D4B7E3B0534C26FC3F3252C29F3C0E0B2524C9833691B4112C2956CC5033504A0B3D0E0B25C4CC83B690B4B12CC248196A164AA9A8C76121890C7A2856CA6955A6AC1E8F665366EA41AB5620D01E876E459AECA168B6789F2273D822ED5664F214E76891217C1CD6CA51730FC56439E5E649C4C7A1E184277C289A2D2C9D1280458C34B5D6765B86D09BD974CBE4D6472E5A0E64F3D45165FAAD8829E7A87969CA0A3F0EEF1488BF87E29F399D8AE4E437D56A9C9F1E5E281D6D48674AED98493C6FAB6FA7CAD2AF7B54915E1FCFE2D43DAA88EF834B23344AABB1C3955CF69636B792D41ADB9D2C8F13A7D5F8BE912E7B4B1F3852CE1B3B77EE7C332B92D49E9C7366CE33BBD1D9E2F6E79972878DA188AE81AA9E209C3AE0C02F2EC7F6A92F703AFFDD1A582438BF8805A68892157679C84DD12FCE7CDEAAF4B4D3E13CB3D4715DD32A389B95AF2FBCFDBB461E25BF7B9804AF42AC887F8AB3F5E1889AF75CE4872BF2F5E5E80E136AE2E79EFE4750FE4A9B7CFE12429C683307AC7EA59D697FEEFC36127D428EF1889CFCF3446D3C7DD41A7AF6DD9718F81B1B3D7FBBE36B45846EBBE95780907BA2C8042FE1ED3C51D418AAF08922D0D432783D24A72BF5677732CF12ED0659F214511333543C4454EA21CD1F1BDA09B2EA858B4CD755E00A1F7C688053F408420398821BF50D500A6E9A374029BEDFDB00287B2B7427FB975DF46C0554D6D94E9045D7317702CC7CA6544C0CE717EFDABA725DBAE47FCBC7A9DE664DB175366FF0C09BFCF85479B86FF4C054E3E94D7E60AA314CD1035375D613654ED8802A2DF148F7C06D9648A2F589DA8DA8D1D5DFF2AF458B3E0E2A749E785AB2E3227C2A95109CC3AF4818264B06860D9DB9882A5C457DDEC67C2EAAA39251FC1ADCE882F6E6767DB6D0A3B37CEAD76142E77704C07F84679DC16D5DB24E21FC9D408A0DC9731299095DB1D881332D8A45B2CB22CC11C44CD477603642062FB8993EB297D89CD070E1045DC6F6D292B697FC8150557F40F796DBDC8DD6A86D74019A49FCB03FA3D71EB1D2EBDEE382205D02E18FB06802F66DC9FD8978FD9220859702558022F525816181ED8D0560EE8CCED1136ED23670D51BBC46C64BBCB1530EB2DD10B2DABB4382D60EB2DD08232D0FFF820F9BF6F34FFF0362F7F40EDD5C0000, N'6.1.3-40302')
INSERT [dbo].[ServiceUsers] ([UserId], [UserName], [ApiKey], [FromDate], [ToDate], [EmailAddress]) VALUES (N'c7521f26-fbb5-e511-82b8-606c66415a40', N'TestUserNotExpiring', 0x7468656B65797468656B65797468656B65797468656B65797468656B65797468656B6579, CAST(0x0000A41300000000 AS DateTime), CAST(0x0000A41300000000 AS DateTime), N'notexpired@sensus.dk')
INSERT [dbo].[ServiceUsers] ([UserId], [UserName], [ApiKey], [FromDate], [ToDate], [EmailAddress]) VALUES (N'c8521f26-fbb5-e511-82b8-606c66415a40', N'TestUser2', 0x62666665383766342D393664342D346130622D613663392D653437313639393762373738, CAST(0x0000A41300000000 AS DateTime), CAST(0x0000B25800000000 AS DateTime), N'source2@sensus.dk')
INSERT [dbo].[ServiceUsers] ([UserId], [UserName], [ApiKey], [FromDate], [ToDate], [EmailAddress]) VALUES (N'3c25f563-58e4-e511-82c7-606c66415a40', N'Test', 0x36663634653964312D396439362D343763322D623134652D346139343438643161393366, CAST(0x0000A5C200BE6CA6 AS DateTime), CAST(0x0000A72F00BE6CA6 AS DateTime), N'test@test.dk')
INSERT [dbo].[ServiceUsers] ([UserId], [UserName], [ApiKey], [FromDate], [ToDate], [EmailAddress]) VALUES (N'58c08ca9-59e4-e511-82c7-606c66415a40', N'testCreate', 0x62626164323132372D663533652D343762382D626666352D316533373434366163623766, CAST(0x0000A5C200C0DBF4 AS DateTime), CAST(0x0000D73E00000000 AS DateTime), N'create@create.dk')
INSERT [dbo].[ServiceUsers] ([UserId], [UserName], [ApiKey], [FromDate], [ToDate], [EmailAddress]) VALUES (N'65d6666d-7efd-e511-82cf-606c66415a40', N'test2', 0x33303532636636312D653831622D343163632D613531302D663332376464636231623739, CAST(0x0000A5E200BFDEA6 AS DateTime), CAST(0x0000A5E200BFDEA6 AS DateTime), N'test@sesnsus.dk')
INSERT [dbo].[ServiceUsers] ([UserId], [UserName], [ApiKey], [FromDate], [ToDate], [EmailAddress]) VALUES (N'5926f78f-7ffd-e511-82cf-606c66415a40', N'test3', 0x39326162376534382D373063322D346537662D616563352D323564336634646339323861, CAST(0x0000A5E200C21A2B AS DateTime), CAST(0x0000A5E200C21A2B AS DateTime), N't3@ses.dd')
INSERT [dbo].[ServiceUsers] ([UserId], [UserName], [ApiKey], [FromDate], [ToDate], [EmailAddress]) VALUES (N'769983ab-7ffd-e511-82cf-606c66415a40', N'test3', 0x32303935613933332D323736632D343732382D383537332D626237643338643361643766, CAST(0x0000A5E200C25179 AS DateTime), CAST(0x0000A5E200C25179 AS DateTime), N't3@ses.dd')
INSERT [dbo].[ServiceUsers] ([UserId], [UserName], [ApiKey], [FromDate], [ToDate], [EmailAddress]) VALUES (N'8128e9c0-7ffd-e511-82cf-606c66415a40', N'test4', 0x34613635333335312D386233302D343765392D383730382D353861353332636161663030, CAST(0x0000A5E200C27B8B AS DateTime), CAST(0x0000A5E200C27B8B AS DateTime), N'test@sss.dk')
INSERT [dbo].[ServiceUsers] ([UserId], [UserName], [ApiKey], [FromDate], [ToDate], [EmailAddress]) VALUES (N'37097095-80fd-e511-82cf-606c66415a40', N'test5', 0x38623330343964612D663034342D343834302D393363352D613965303530346237363236, CAST(0x0000A5E200C41BAD AS DateTime), CAST(0x0000A5E200C41BAD AS DateTime), N'test5@sss.dd')
INSERT [dbo].[ServiceUsers] ([UserId], [UserName], [ApiKey], [FromDate], [ToDate], [EmailAddress]) VALUES (N'02484442-81fd-e511-82cf-606c66415a40', N'test6', 0x30636466376137362D323638652D343333352D383734642D393037353836626539363839, CAST(0x0000A5E200C57016 AS DateTime), CAST(0x0000A5E200C57016 AS DateTime), N'ttt@ss.dk')
INSERT [dbo].[ServiceUsers] ([UserId], [UserName], [ApiKey], [FromDate], [ToDate], [EmailAddress]) VALUES (N'9608b3bf-81fd-e511-82cf-606c66415a40', N'test7', 0x64633466353935612D646138322D346233372D613930322D303231326466363739656261, CAST(0x0000A5E200C66697 AS DateTime), CAST(0x0000A5E200C66697 AS DateTime), N't7@sdsd.dk')
INSERT [dbo].[ServiceUsers] ([UserId], [UserName], [ApiKey], [FromDate], [ToDate], [EmailAddress]) VALUES (N'5686c5b1-82fd-e511-82cf-606c66415a40', N'test8', 0x33356466656463612D323936302D343637662D616335352D333739323237333839316536, CAST(0x0000A5E200C84271 AS DateTime), CAST(0x0000A5E200C84271 AS DateTime), N'sdasda@asdas.gh')
INSERT [dbo].[ServiceUsers] ([UserId], [UserName], [ApiKey], [FromDate], [ToDate], [EmailAddress]) VALUES (N'844aff65-83fd-e511-82cf-606c66415a40', N't2', 0x37643662303336342D396533382D346632362D613539342D356266353830656664346237, CAST(0x0000A5E200C9A50A AS DateTime), CAST(0x0000A5E200C9A50A AS DateTime), N't3@sda.dfd')
INSERT [dbo].[ServiceUsers] ([UserId], [UserName], [ApiKey], [FromDate], [ToDate], [EmailAddress]) VALUES (N'e9461c99-83fd-e511-82cf-606c66415a40', N'test9', 0x37323162643063382D623434662D346632612D393964322D666331646462386131643363, CAST(0x0000A5E200CA099A AS DateTime), CAST(0x0000A5E200CA099A AS DateTime), N'test9@senus.dk')
INSERT [dbo].[ServiceUsers] ([UserId], [UserName], [ApiKey], [FromDate], [ToDate], [EmailAddress]) VALUES (N'd2b97532-e8c5-e411-8270-f0def103cfd0', N'TestUser', 0x37623736616534312D646566332D653431312D383033302D306338626664323333366364, CAST(0x0000A41300000000 AS DateTime), CAST(0x0000AB3500000000 AS DateTime), N'source@sensus.dk')
/****** Object:  Index [IX_UserId]    Script Date: 10/17/2016 6:59:12 PM ******/
CREATE NONCLUSTERED INDEX [IX_UserId] ON [dbo].[Jobs]
(
	[UserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
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
USE [master]
GO
ALTER DATABASE [RoboBrailleJobDB] SET  READ_WRITE 
GO
