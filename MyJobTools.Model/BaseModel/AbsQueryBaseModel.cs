using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;

namespace MyJobTools.Model
{
    public abstract class AbsQueryBaseModel
    {
        public abstract string GetWhereFormat();

        public void SetOrderBy(string orderBy)
        {
            _orderBy = orderBy;
        }

        private string _orderBy = null;

        public string GetOrderBy() 
        {
            if (!String.IsNullOrWhiteSpace(_orderBy))
            {
                return String.Format(" order by {0} ", _orderBy);
            }
            else
            {
                return "";
            }
        }
    }
}
