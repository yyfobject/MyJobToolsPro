using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using MyJobTools.Library.Extension;

namespace MyJobTools.Model
{
    public class PageQueryModel
    {
        public int? Page { get; set; }

        public int? Limit { get; set; }

        /// <summary>
        /// 可用来传递查询结果的总行数，不参与查询条件的逻辑
        /// </summary>
        public int RowsCount = 0;
        
    }
}
