using AmaraVideoClient;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using RoboBraille.WebApi.Models;

namespace RoboBraille.WebApi.Models.RoboVideo
{
    public class VideoJobProcessor
    {
        private static VideoClientController vcc = new VideoClientController();
        private static string distFile =null;
        public string CreateVideoUrl(VideoJob vj)
        {
            string tempVideoName = vj.FileName +"." + vj.FileExtension;
            distFile = ConfigurationManager.AppSettings.Get("DistDirectory") + tempVideoName;
            File.WriteAllBytes(distFile, vj.FileContent);
            //string linkUrl = @"http://2.109.50.18:5150/dist/" + tempVideoName;
            string linkUrl = @"http://localhost:35015/dist/" + tempVideoName;
            return linkUrl;
        }

        public VideoLanguageDetail GetVideoInfo(string videoId, string languageCode)
        {
            VideoDetail vd = vcc.GetVideoInfo(videoId);
            if (vd.Languages.Count > 0)
            {
                string vlUri = (from l in vd.Languages where l.Code == languageCode select l).FirstOrDefault().VideoLanguageUri;
                if (!string.IsNullOrWhiteSpace(vlUri))
                {
                    return vcc.GetLanguageDetails(vlUri);
                    //Console.WriteLine("Video language Detail: " + vlDetails);
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return null;
            }
        }

        public byte[] DownloadSubtitle(VideoJob vj)
        {
            return vcc.GetVideoSubtitle(vj.AmaraVideoId, vj.SubtitleLangauge, vj.SubtitleFormat).SubtitleData;
        }

        public SubtitleInfo PostVideo(VideoJob vj)
        {
            VideoSummary vs = null;
            if (vj.VideoUrl.Contains("youtube") || vj.VideoUrl.Contains("vimeo") || vj.VideoUrl.Contains("dailymotion"))
            {
                vs = new VideoSummary()
                {
                    VideoUrl = vj.VideoUrl//"http://www.youtube.com/watch?v=cJs7obmEABE"//"https://www.youtube.com/watch?v=RdKAVE0frIM&ab_channel=BBC"
                };
            }
            else
            {
                if (vj.VideoUrl.Contains("localhost"))
                {
                    vj.VideoUrl = "http://2.109.50.18:5150/dist/test.webm";
                }
                vs = new VideoSummary() {
                    VideoUrl = vj.VideoUrl,
                    //Title = vj.FileName,
                    //Description = "a video uploaded by RoboBraille",
                    //Duration = 80,
                    //PrimaryAudioLanguageCode = vj.SubtitleLangauge,
                    //Thumbnail = "http://www.robobraille.org/sites/default/files/webrobo_logo.jpg",
                    //Metadata = new Dictionary<string, string>() { {"location","dk"} },
                    //Team = "RoboBraille",
                    //Project = "P4All"
                };
            }
            SubtitleInfo si = vcc.RequestVideoSubtitle(vs, vj.SubtitleLangauge, vj.SubtitleFormat);
            if (File.Exists(distFile))
            {
                File.Delete(distFile);
            }
            return si;
        }

    }
}