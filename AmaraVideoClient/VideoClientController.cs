using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmaraVideoClient
{
    public class VideoClientController : IVideoClientController
    {
        public SubtitleInfo RequestVideoSubtitle(VideoSummary videoSummary, string requestedLanguageCode, string outputFormat)
        {
            try
            {
                //step 1) check if subtitle already exists for the request => if yes => return subtitle
                List<VideoDetail> videoDetailList = AmaraApiController.ListVideosWithFilter("video_url", videoSummary.VideoUrl).Result;
                if (videoDetailList.Count > 0)
                {
                    VideoDetail resultVD = videoDetailList[0];
                    //check if video contains requested language
                    Language resultLanguage = (from vdl in resultVD.Languages where vdl.Code == requestedLanguageCode select vdl).FirstOrDefault();
                    //check information from resourceUri and then download from subtitle uri
                    if (resultLanguage != null)
                    {
                        var resourceUri = resultLanguage.VideoLanguageUri;
                        if (resourceUri != null)
                        {
                            //get resurceUri info
                            VideoLanguageDetail vLangDetail = AmaraApiController.GetLanguageDetails(resourceUri).Result;
                            //check if it is completed, then if it is a translation, if it fits continue
                            var subUri = resultLanguage.SubtitleUri;
                            if (subUri != null && vLangDetail.AreSubsComplete && vLangDetail.SubtitleCount > 0)
                            {
                                //all is validated return the subtitle in the desired format
                                return new SubtitleInfo(resultVD.Id, VideoSubtitleStatus.Exists);//GetVideoSubtitle(resultVD.Id, requestedLanguageCode, outputFormat).SubtitleData;
                            }
                            else
                            {
                                return new SubtitleInfo(resultVD.Id, VideoSubtitleStatus.NotComplete);
                            }
                        }
                    }
                    else
                    {
                        //submit request for translation to non-existent language
                        VideoLanguageDetail vLangDetail = AmaraApiController.PostNewSubtitleRequest(resultVD.Id, requestedLanguageCode).Result;
                        return new SubtitleInfo(resultVD.Id, VideoSubtitleStatus.SubtitleRequested);
                    }
                }
                else
                {
                    //step 2) else => submit request to subtitle video towards amara (optional use tweeter/facebook to notify community of a subtitle request)
                    VideoDetail videoDetail = AmaraApiController.PostVideo(videoSummary).Result;
                    //step 3) return videoId and video information relevant to later check for a completed subtitle
                    return new SubtitleInfo(videoDetail.Id, VideoSubtitleStatus.Submitted);
                }
            }
            catch
            {
                return new SubtitleInfo(null, VideoSubtitleStatus.Error);
            }
            return new SubtitleInfo(null, VideoSubtitleStatus.Error);
        }

        public Subtitle GetVideoSubtitle(string videoId, string videoLanguage, string outputFormat)
        {
            //throw new NotImplementedException();
            // return the subtitle for the desired language in the desired format
            return new Subtitle() { SubtitleData = AmaraApiController.GetVideoSubtitle(videoId, videoLanguage, outputFormat) };
        }

        public Subtitle GetVideoSubtitle(string subtitleUri, string outputFormat)
        {
            // return the subtitle for the desired language in the desired format
            return new Subtitle() { SubtitleData = AmaraApiController.GetVideoSubtitle(subtitleUri, outputFormat) };
        }

        public VideoDetail GetVideoInfo(string videoId)
        {
            //step 1) check if subtitle has been made for video
            //step 2) if no return else check if complete
            //step 3) if yes return complete else say it is not complete
            return AmaraApiController.GetVideoInfo(videoId);
        }

        public VideoLanguageDetail GetLanguageDetails(string vlUri)
        {
            return AmaraApiController.GetLanguageDetails(vlUri).Result;
        }
    }
}
