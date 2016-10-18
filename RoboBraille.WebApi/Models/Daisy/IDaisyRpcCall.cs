using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoboBraille.WebApi.Models
{
    public interface IDaisyRpcCall
    {
        byte[] Call(byte[] document, bool isEpub3, string guid);
    }
}
