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
        public string CreateVideoUrl(VideoJob vj) {
            try
            {
                string tempVideoName = vj.Id + vj.FileExtension;
                string distPath = System.Web.HttpContext.Current.Server.MapPath("~") + "\\Dist\\" + tempVideoName;
                File.WriteAllBytes(distPath, vj.FileContent);
                string linkUrl = @"http://2.109.50.18:5150/Dist/" + tempVideoName;
                return linkUrl;
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public void GetVideoInfo(string videoId, string languageCode)
        {
            VideoDetail vd = vcc.GetVideoInfo(videoId);
            if (vd.Languages.Count > 0)
            {
                string vlUri = (from l in vd.Languages where l.Code == languageCode select l).FirstOrDefault().VideoLanguageUri;
                if (!string.IsNullOrWhiteSpace(vlUri))
                {
                    var vlDetails = vcc.GetLanguageDetails(vlUri);
                    //Console.WriteLine("Video language Detail: " + vlDetails);
                }
            }
        }

        public byte[] DownloadSubtitle(VideoJob vj)
        {
            return vcc.GetVideoSubtitle(vj.AmaraVideoId, vj.SubtitleLangauge, vj.SubtitleFormat).SubtitleData;
        }

        public SubtitleInfo PostVideo(VideoJob vj)
        {
            VideoSummary vs = new VideoSummary()
            {
                VideoUrl = vj.VideoUrl//"http://www.youtube.com/watch?v=cJs7obmEABE"//"https://www.youtube.com/watch?v=RdKAVE0frIM&ab_channel=BBC"
            };
            SubtitleInfo si = vcc.RequestVideoSubtitle(vs, vj.SubtitleLangauge, vj.SubtitleFormat);
            //switch (si.Status)
            //{
            //    case VideoSubtitleStatus.Complete:
            //        break;
            //    case VideoSubtitleStatus.SubtitleRequested:
            //        //Console.WriteLine("Video exists, but subtitle does not, request for subtitle is made.");
            //        break;
            //    case VideoSubtitleStatus.Error:
            //        //Console.WriteLine("Error happened!");
            //        break;
            //    case VideoSubtitleStatus.Exists:
            //        byte[] res = vcc.GetVideoSubtitle(si.VideoId, vj.SubtitleLangauge, vj.SubtitleFormat).SubtitleData;
            //        //Console.WriteLine("Result: " + Encoding.Default.GetString(res));
            //        break;
            //    case VideoSubtitleStatus.NotComplete: break;
            //    case VideoSubtitleStatus.Submitted:
            //        //Console.WriteLine("Video with id {1} has been submitted for manual subtitling. You'll get notified when it's ready by pigeon!", si.VideoId);
            //        break;
            //    default: break;
            //}
            return si;
        }

    }
} 