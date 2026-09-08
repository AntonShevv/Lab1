using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    public class ResponseData
    {
        public bool Success { get; set; }
        public string? Result { get; set; }
        public string? Error { get; set; }
    }

}
