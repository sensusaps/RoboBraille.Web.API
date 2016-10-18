using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net;

namespace AmaraVideoClient
{
    class AmaraApiController
    {
        private static string userName = "paul.sensus";//"senus";//
        private static string apiKey = "fc4a6c01c6c1d3162fb2fb3df2493fef53c43293";//"7402fdebc988196d6aa6f2fc3a0fe1bf84c547fe";//
        private static string baseAddress = "https://amara.org/api/";//"https://staging.amara.org/api/";
        static async Task ListVideos()
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(baseAddress);
                client.DefaultRequestHeaders.Add("X-api-username", userName);
                client.DefaultRequestHeaders.Add("X-api-key", apiKey);
                HttpResponseMessage response = await client.GetAsync("videos/");
                if (response.IsSuccessStatusCode)
                {
                    List<VideoDetail> listVideo = await response.Content.ReadAsAsync<List<VideoDetail>>();

                    foreach (VideoDetail video in listVideo)
                        Console.WriteLine("Video: " + video);
                }
            }
        }
        public static async Task<List<VideoDetail>> ListVideosWithFilter(string filterName, string filterValue)
        {
            return ListVideosWithFilter(filterName, filterValue, null).Result;
        }
        public static async Task<List<VideoDetail>> ListVideosWithFilter(string filterName, string filterValue, string orderBy)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    List<VideoDetail> listVideo = new List<VideoDetail>();
                    client.BaseAddress = new Uri(baseAddress);
                    client.DefaultRequestHeaders.Add("X-api-username", userName);
                    client.DefaultRequestHeaders.Add("X-api-key", apiKey);
                    string paramUri = "videos/?" + filterName + "=" + filterValue;
                    if (orderBy != null)
                    {
                        paramUri += "?order_by=" + orderBy;
                    }
                    HttpResponseMessage response = await client.GetAsync(paramUri);
                    if (response.IsSuccessStatusCode)
                    {
                        VideoDetailsFilter vdf = await response.Content.ReadAsAsync<VideoDetailsFilter>();
                        if (vdf.VideoDetails.Length > 0)
                        {
                            listVideo = vdf.VideoDetails.ToList();
                            foreach (VideoDetail video in listVideo)
                                Console.WriteLine("Video: " + video);
                        }
                        else
                        {
                            Console.WriteLine("List empty, video does not exist in database.");
                        }
                    }
                    else Console.WriteLine("Video failure: " + response.StatusCode);
                    return listVideo;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return null;
            }
        }

        public static async Task<VideoDetail> PostVideo(VideoSummary videoPost)
        {
            using (var client = new HttpClient())
            {
                VideoDetail video = null;
                client.BaseAddress = new Uri(baseAddress);
                client.DefaultRequestHeaders.Add("X-api-username", userName);
                client.DefaultRequestHeaders.Add("X-api-key", apiKey);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                HttpResponseMessage response = await client.PostAsJsonAsync("videos/", videoPost);
                if (response.IsSuccessStatusCode)
                {
                    video = await response.Content.ReadAsAsync<VideoDetail>();
                    Console.WriteLine("Post Video Success: " + video);
                }
                else Console.WriteLine("Post Video failure: " + response.StatusCode);
                return video;
            }
        }

        /// <summary>
        /// This method may not work or be useful.
        /// </summary>
        /// <param name="videoId"></param>
        /// <param name="languageCode"></param>
        /// <returns></returns>
        public static async Task<VideoLanguageDetail> PostNewSubtitleRequest(string videoId, string languageCode)
        {
            using (var client = new HttpClient())
            {
                VideoLanguageDetail vld = null;
                client.BaseAddress = new Uri(baseAddress);
                client.DefaultRequestHeaders.Add("X-api-username", userName);
                client.DefaultRequestHeaders.Add("X-api-key", apiKey);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                SubtitleSummary ss = new SubtitleSummary()
                {
                    LanguageCode = languageCode
                };
                HttpResponseMessage response = await client.PostAsJsonAsync("videos/" + videoId + "/languages/", ss);
                if (response.IsSuccessStatusCode)
                {
                    vld = await response.Content.ReadAsAsync<VideoLanguageDetail>();
                    Console.WriteLine("Post Video Success: " + vld);
                }
                else Console.WriteLine("Post Video failure: " + response.StatusCode);
                return vld;
            }
        }
        public static async Task<VideoLanguageDetail> PostNewSubtitleRequest2(string videoId, string languageCode)
        {
            using (var client = new HttpClient())
            {
                VideoLanguageDetail vld = null;
                client.BaseAddress = new Uri(baseAddress);
                client.DefaultRequestHeaders.Add("X-api-username", userName);
                client.DefaultRequestHeaders.Add("X-api-key", apiKey);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                HttpResponseMessage response = await client.PostAsJsonAsync("videos/"+videoId+"/languages/"+languageCode+"/subtitles/", "");
                if (response.IsSuccessStatusCode)
                {
                    vld = await response.Content.ReadAsAsync<VideoLanguageDetail>();
                    Console.WriteLine("Post Video Success: " + vld);
                }
                else Console.WriteLine("Post Video failure: " + response.StatusCode);
                return vld;
            }
        }

        public async Task DeleteVideo(string videoId)
        {
            //TODO It may not be possible.

        }

        static async Task UpdateVideo(VideoDetail v)
        {

        }

        public static async Task<VideoDetail> GetVideoInfo(string videoId)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    VideoDetail videoResponse = null;
                    client.BaseAddress = new Uri(baseAddress);
                    client.DefaultRequestHeaders.Add("X-api-username", userName);
                    client.DefaultRequestHeaders.Add("X-api-key", apiKey);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    HttpResponseMessage response = await client.GetAsync("videos/" + videoId + "/");
                    if (response.IsSuccessStatusCode)
                    {
                        videoResponse = await response.Content.ReadAsAsync<VideoDetail>();
                        Console.WriteLine("Video: " + videoResponse);
                    }
                    else Console.WriteLine("Video failure: " + response.StatusCode);
                    return videoResponse;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Get Info failure: " + e);
                return null;
            }
        }

        public static List<Language> ListVideoLanguages(string videoId)
        {
            return GetVideoInfo(videoId).Result.Languages;
        }

        public static async Task CreateTaskToTeam(string videoId, string languageCode, TaskType tt, string teamSlug)
        {
            using (var client = new HttpClient())
            {
                TaskSummary ts = new TaskSummary()
                {
                    VideoId = videoId,
                    LanguageCode = languageCode,
                    TaskType = Enum.GetName(tt.GetType(), tt)
                };
                client.BaseAddress = new Uri(baseAddress);
                client.DefaultRequestHeaders.Add("X-api-username", userName);
                client.DefaultRequestHeaders.Add("X-api-key", apiKey);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                HttpResponseMessage response = await client.PostAsJsonAsync("teams/"+teamSlug+"/tasks/", ts);
                if (response.IsSuccessStatusCode)
                {
                    Console.WriteLine("Create task success ");
                }
                else Console.WriteLine("Create task failure: " + response.StatusCode);

            }
        }

        public static async Task<VideoLanguageDetail> GetLanguageDetails(string resourceUrl)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    VideoLanguageDetail vLangDetail = null;
                    client.BaseAddress = new Uri(baseAddress);
                    client.DefaultRequestHeaders.Add("X-api-username", userName);
                    client.DefaultRequestHeaders.Add("X-api-key", apiKey);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    HttpResponseMessage response = await client.GetAsync(resourceUrl);
                    if (response.IsSuccessStatusCode)
                    {
                        vLangDetail = await response.Content.ReadAsAsync<VideoLanguageDetail>();
                        Console.WriteLine("Subtitle detail: " + vLangDetail);
                    }
                    else Console.WriteLine("Failed " + response.StatusCode);
                    return vLangDetail;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("GetLanguageDetails failure: " + e);
                return null;
            }
        }

        static async Task GetLanguageDetails(string videoId, string languageCode)
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="videoId"></param>
        /// <param name="langaugeCode"></param>
        /// <param name="subFormat"></param>
        /// <returns></returns>
        public static byte[] GetVideoSubtitle(string videoId, string langaugeCode, string subFormat)
        {
            //TODO Validate the input going into the url
            using (var client = new WebClient())
            {
                client.BaseAddress = baseAddress;
                client.Headers.Add("X-api-username", userName);
                client.Headers.Add("X-api-key", apiKey);
                string downloadUrl = "videos/" + videoId + "/languages/" + langaugeCode + "/subtitles/?format=" + subFormat;
                return client.DownloadData(downloadUrl);
            }
        }

        public static byte[] GetVideoSubtitle(string resourceUri, string subFormat)
        {
            //TODO Validate the input going into the url
            using (var client = new WebClient())
            {
                client.BaseAddress = baseAddress;
                client.Headers.Add("X-api-username", userName);
                client.Headers.Add("X-api-key", apiKey);
                return client.DownloadData(resourceUri + "?format=" + subFormat);
            }
        }
    }
}
