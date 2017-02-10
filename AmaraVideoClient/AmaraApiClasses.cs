using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;

namespace AmaraVideoClient
{
    enum TaskType
    {
        Subtitle,
        Translate
    }

    [DataContract]
    class TaskSummary
    {
        [DataMember(Name = "video_id")]
        public string VideoId { get; set; }
        [DataMember(Name = "language")]
        public string LanguageCode { get; set; }
        [DataMember(Name = "type")]
        public string TaskType { get; set; }
    }

    [DataContract]
    public class SubtitleSummary
    {
        [DataMember(Name = "language_code")] // (string) – bcp-47 code for the language
        public string LanguageCode { get; set; }
        [DataMember(Name = "is_primary_audio_language")] // (boolean) – Is this is the primary spoken language of the video? (optional).
        public bool? IsPrimaryLanguage { get; set; }
        [DataMember(Name = "subtitles_complete")] // (boolean) – Are the subtitles for this language complete? (optional).
        public bool? IsSubComplete { get; set; }
    }

    [DataContract]
    public class VideoSummary
    {
        [DataMember(Name = "video_url")]
        public string VideoUrl { get; set; }
        //[DataMember(Name = "title")]
        //public string Title { get; set; }
        //[DataMember(Name = "description")]
        //public string Description { get; set; }
        //[DataMember(Name = "duration")]
        //public Int32 Duration { get; set; }
        //[DataMember(Name = "primary_audio_language_code")]
        //public string PrimaryAudioLanguageCode { get; set; }
        //[DataMember(Name = "thumbnail")]
        //public string Thumbnail { get; set; }
        //[DataMember(Name = "metadata")]
        //public Dictionary<string, string> Metadata { get; set; }
        //[DataMember(Name = "team")]
        //public string Team { get; set; }
        //[DataMember(Name = "project")]
        //public string Project { get; set; }
    }

    [DataContract]
    class VideoDetailsFilter
    {
        [DataMember(Name = "meta")]
        public Metadata VdMetadata { get; set; }
        [DataMember(Name = "objects")]
        public VideoDetail[] VideoDetails { get; set; }
    }

    [DataContract]
    class Metadata
    {
        [DataMember(Name = "previous")]
        public string Previous { get; set; }
        [DataMember(Name = "next")]
        public string Next { get; set; }
        [DataMember(Name = "offset")]
        public int Offset { get; set; }
        [DataMember(Name = "limit")]
        public int Limit { get; set; }
        [DataMember(Name = "total_count")]
        public int TotalCount { get; set; }
    }

    [DataContract]
    public class VideoDetail
    {
        [DataMember(Name = "id")] // – Amara video id
        public string Id { get; set; }
        [DataMember(Name = "primary_audio_language_code")] // – language code for the audio language
        public string LanguageCode { get; set; }
        [DataMember(Name = "title")] // – Video title
        public string Title { get; set; }
        [DataMember(Name = "description")] // – Video description
        public string Description { get; set; }
        [DataMember(Name = "duration")] // – Video duration in seconds (or null if not known)
        public int? Duration { get; set; }
        [DataMember(Name = "thumbnail")] // – URL to the video thumbnail
        public string Thumbnail { get; set; }
        [DataMember(Name = "created")] // – Video creation date/time
        public string DateCreated { get; set; }
        [DataMember(Name = "team")] // – Slug of the Video’s team (or null)
        public string Team { get; set; }
        [DataMember(Name = "metadata")] // – Dict mapping metadata names to values
        public object Metadata { get; set; }
        [DataMember(Name = "languages")] // – List of languages that have subtitles started (see below)
        public List<Language> Languages { get; set; }
        [DataMember(Name = "video_type")] // – Video type identifier
        public string VideoType { get; set; }
        [DataMember(Name = "all_urls")] // – List of URLs for the video (the first one is the primary video URL)
        public List<string> AllUrls { get; set; }
        [DataMember(Name = "resource_uri")] // – API uri for the video
        public string ResourceUri { get; set; }
    }

    [DataContract]
    public class Language
    {
        [DataMember(Name = "code")] // – Language code
        public string Code { get; set; }
        [DataMember(Name = "name")] // – Human readable label for the language
        public string Name { get; set; }
        [DataMember(Name = "visibile")] // – Are the subtitles publicly viewable?
        public bool PubliclyViewable { get; set; }
        [DataMember(Name = "dir")] // – Language direction (“ltr” or “rtl”)
        public string Direction { get; set; }
        [DataMember(Name = "subtitles_uri")] // – API URI for the subtitles
        public string SubtitleUri { get; set; }
        [DataMember(Name = "resource_uri")] // – API URI for the video language
        public string VideoLanguageUri { get; set; }
    }

    [DataContract]
    class VideoUrl
    {
        [DataMember(Name = "created")] // – creation date/time
        public string CreationDate { get; set; }
        [DataMember(Name = "url")] // – URL string
        public string Url { get; set; }
        [DataMember(Name = "primary")] // – is this the primary URL for the video?
        public bool IsPrimary { get; set; }
        [DataMember(Name = "original")] // – was this the URL that was created with the video?
        public bool IsOriginal { get; set; }
        [DataMember(Name = "resource_uri")] // – API URL for the video URL
        public string ResourceUri { get; set; }
    }


    [DataContract]
    class SubtitleDetail
    {
        [DataMember(Name = "version_number")] // – version number for the subtitles
        public int VersionNumbe { get; set; }
        [DataMember(Name = "subtitles")] // – Subtitle data (str)
        public byte[] SubtitleData { get; set; }
        [DataMember(Name = "sub_format")] // – Format of the subtitles
        public string SubFormat { get; set; }
        [DataMember(Name = "language")] // – Language data
        public string MyProperty1 { get; set; }
        [DataMember(Name = "language.code")] // – BCP-47 language code
        public string MyProperty2 { get; set; }
        [DataMember(Name = "language.name")] // – Human readable name for the language
        public string MyProperty3 { get; set; }
        [DataMember(Name = "language.dir")] // – Language direction (“ltr” or “rtl”)
        public string MyProperty4 { get; set; }
        [DataMember(Name = "title")] // – Video title, translated into the subtitle’s language
        public string MyProperty5 { get; set; }
        [DataMember(Name = "description")] // – Video description, translated into the subtitle’s language
        public string MyProperty6 { get; set; }
        [DataMember(Name = "metadata")] // – Video metadata, translated into the subtitle’s language
        public string MyProperty7 { get; set; }
        [DataMember(Name = "video_title")] // – Video title, translated into the video’s language
        public string MyProperty8 { get; set; }
        [DataMember(Name = "video_description")] // – Video description, translated into the video’s language
        public string MyProperty9 { get; set; }
        [DataMember(Name = "resource_uri")] // – API URI for the subtitles
        public string MyProperty10 { get; set; }
        [DataMember(Name = "site_uri")] // – URI to view the subtitles on site
        public string MyProperty11 { get; set; }
    }

    [DataContract]
    public class VideoLanguageDetail
    {
        [DataMember(Name = "language_code")] // – BCP 47 code for this language
        public string LanguageCode { get; set; }
        [DataMember(Name = "name")] // – Human-readable name for this language
        public string Name { get; set; }
        [DataMember(Name = "is_primary_audio_language")] // – Is this language the primary language spoken in the video?
        public bool IsPrimaryAudioLanguage { get; set; }
        [DataMember(Name = "is_rtl")] // – Is this language RTL?
        public bool IsRtl { get; set; }
        [DataMember(Name = "resource_uri")] // – API URL for the language
        public string ResourceUri { get; set; }
        [DataMember(Name = "created")] // – date/time the language was created
        public string DateCreated { get; set; }
        [DataMember(Name = "title")] // – Video title, translated into this language
        public string VideoTitle { get; set; }
        [DataMember(Name = "description")] // – Video description, translated into this language
        public string VideoDescription { get; set; }
        [DataMember(Name = "metadata")] // – Video metadata, translated into this language
        public Dictionary<string, string> VideoMetadata { get; set; }
        [DataMember(Name = "subtitles_complete")] // – Are the subtitles complete for this language?
        public bool AreSubsComplete { get; set; }
        [DataMember(Name = "subtitle_count")] // – Number of subtitles for this language
        public int SubtitleCount { get; set; }
        [DataMember(Name = "reviewer")] // – Username of the reviewer fro task-based teams
        public string Reviewer { get; set; }
        [DataMember(Name = "approver")] // – Username of the approver for task-based teams
        public string Approver { get; set; }
        [DataMember(Name = "versions")] // – List of subtitle version data
        public List<SubtitleVersion> SubtitleVersionData { get; set; }
    }

    [DataContract]
    public class SubtitleVersion
    {
        [DataMember(Name = "author")] // – Subtitle author’s username
        public AmaraUser Author { get; set; }
        [DataMember(Name = "version_no")] // – number of the version
        public int VersionNumber { get; set; }
        [DataMember(Name = "published")] // – is this version publicly viewable?
        public bool IsPublished { get; set; }
    }

    [DataContract]
    public class AmaraUser
    {
        [DataMember(Name = "username")] // – username
        public string UserName { get; set; }
        [DataMember(Name = "id")] // – id
        public string UserId { get; set; }
        [DataMember(Name = "uri")] // – User API endpoint
        public string UserAPIEndpoint { get; set; }
    }
}
