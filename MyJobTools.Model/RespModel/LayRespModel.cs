using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using MyJobTools.Library.Extension;

namespace MyJobTools.Model
{
    public class LayRespModel
    {
        public int? code { get; set; }

        public int? count { get; set; }
                
        public string msg { get; set; }

        public Object data { get; set; }
    }
}
