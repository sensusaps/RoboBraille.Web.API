namespace RoboBraille.WebApi.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RBJobDB : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Jobs",
                c => new
                {
                    Id = c.Guid(nullable: false, identity: true),
                    UserId = c.Guid(nullable: false),
                    FileName = c.String(nullable: false, maxLength: 512),
                    FileExtension = c.String(nullable: false, maxLength: 512),
                    MimeType = c.String(nullable: false),
                    Status = c.Int(nullable: false),
                    SubmitTime = c.DateTime(nullable: false),
                    FinishTime = c.DateTime(nullable: false),
                    InputFileHash = c.Binary(),
                    ResultContent = c.Binary(),
                    DownloadCounter = c.Int(nullable: false),
                    ResultFileExtension = c.String(),
                    ResultMimeType = c.String(),
                    TargetDocumentFormat = c.Int(),
                    FormatOptions = c.Int(),
                    OutputFormat = c.Int(),
                    DaisyOutput = c.Int(),
                    EbookFormat = c.Int(),
                    MSOfficeOutput = c.Int(),
                    VideoUrl = c.String(),
                    SubtitleLangauge = c.String(),
                    SubtitleFormat = c.String(),
                    AmaraVideoId = c.String(),
                    Discriminator = c.String(nullable: false, maxLength: 128),
                })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.ServiceUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);

            CreateTable(
                "dbo.ServiceUsers",
                c => new
                {
                    UserId = c.Guid(nullable: false, identity: true),
                    UserName = c.String(nullable: false, maxLength: 32),
                    ApiKey = c.Binary(nullable: false),
                    FromDate = c.DateTime(nullable: false),
                    ToDate = c.DateTime(nullable: false),
                    EmailAddress = c.String(nullable: false),
                })
                .PrimaryKey(t => t.UserId);

        }

        public override void Down()
        {
            DropForeignKey("dbo.Jobs", "UserId", "dbo.ServiceUsers");
            DropIndex("dbo.Jobs", new[] { "UserId" });
            DropTable("dbo.ServiceUsers");
            DropTable("dbo.Jobs");
        }
    }
}
