using System;
using System.Threading.Tasks;

namespace RoboBraille.WebApi.Models
{
    /// <summary>
    /// Interface for creating a Repository class for each job
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IRoboBrailleJob<in T> where T : Job
    {
        /// <summary>
        /// Starts the job in a new thread
        /// </summary>
        /// <param name="t">The class that extends the Job base class</param>
        /// <returns>The Guid of the current job</returns>
        Task<Guid> SubmitWorkItem(T t);
        
        /// <summary>
        /// Returns the status of the current job
        /// 0 = error
        /// 1 = success
        /// 2 = processing
        /// </summary>
        /// <param name="guid">The Guid of the job</param>
        /// <returns>An integer representing the status.</returns>
        int GetWorkStatus(Guid guid);
        FileResult GetResultContents(Guid guid);
    }
}
