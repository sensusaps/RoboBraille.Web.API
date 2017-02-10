using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoboBraille.WebApi.Models
{
    public interface IRoboBrailleJobRepository<in T> where T : Job
    {
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

        RoboBrailleDataContext GetDataContext();
    }
}
