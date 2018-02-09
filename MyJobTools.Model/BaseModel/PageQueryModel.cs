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

        /// <summary>
        /// 获取dapper使用的参数对象
        /// </summary>
        /// <returns></returns>
        public int GetStartRow()
        {
            var _page = Page ?? 1;
            var _pageSize = Limit ?? 20;
            return (_page - 1) * _pageSize + 1;
        }
        public int GetEndRow()
        {
            var _page = Page ?? 1;
            var _pageSize = Limit ?? 20;
            return _page * _pageSize;
        }
        public int GetSkip()
        {
            var _page = Page ?? 1;
            var _pageSize = Limit ?? 20;
            return (_page - 1) * _pageSize;
        }

        public int GetLimit()
        {
            return Limit ?? 20;
        }

        public int GetPage()
        {
            return Page ?? 1;
        }
    }
}
