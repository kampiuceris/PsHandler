using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PsHandler.UI
{
    public interface IFilter
    {
        List<string> FilterIncludeAnd { get; }
        List<string> FilterIncludeOr { get; }
        List<string> FilterExcludeAnd { get; }
        List<string> FilterExcludeOr { get; }
        string WindowClass { get; }
    }
}
