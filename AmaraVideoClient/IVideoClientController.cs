using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmaraVideoClient
{
    interface IVideoClientController
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="videoUrl"></param>
        /// <param name="requestedLanguage"></param>
        /// <returns>The video Id</returns>
        SubtitleInfo RequestVideoSubtitle(VideoSummary videoSummary, string requestedLanguageCode, string outputFormat);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="videoId"></param>
        /// <param name="videoLanguage"></param>
        /// <param name="outputFormat"></param>
        /// <returns></returns>
        Subtitle GetVideoSubtitle(string videoId, string videoLanguage,string outputFormat);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="videoId"></param>
        /// <returns>A list of available subtitle languages or null if none exist</returns>
        VideoDetail GetVideoInfo(string videoId);
    }

    public enum VideoSubtitleStatus
    {
        Error = -1,
        Complete = 1,
        NotComplete = 2,
        Submitted = 3,
        Exists = 4,
        SubtitleRequested = 5
    }
}
