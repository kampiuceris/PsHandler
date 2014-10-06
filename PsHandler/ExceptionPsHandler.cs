using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PsHandler
{
    public class ExceptionPsHandler
    {
        public Exception Exception { get; set; }
        public string Header { get; set; }

        public ExceptionPsHandler(Exception e, string header)
        {
            Exception = e;
            Header = header;
        }
    }
}
