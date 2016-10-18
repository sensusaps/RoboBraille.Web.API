using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoboBraille.WebApi.Models.MSOfficeConversion
{
    interface IOfficeProcessor
    {
        string ProcessDocument(string sourceFilePath);
    }
}
