using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmaraVideoClient
{
    public class SubtitleInfo
    {
        public string VideoId { get; set; }
        public VideoSubtitleStatus Status { get; set; }

        public SubtitleInfo(string videoId, VideoSubtitleStatus vss)
        {
            this.VideoId = videoId;
            this.Status = vss;
        }
    }
}
