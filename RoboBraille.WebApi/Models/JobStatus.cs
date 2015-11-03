namespace RoboBraille.WebApi.Models
{
    /// <summary>
    /// the job status enum
    /// </summary>
    public enum JobStatus : int
    {
        Error =0,
        Done =1,
        Started =2,
        Queued =3,
        Processing =4,
        Cancelled = 5
    }
}