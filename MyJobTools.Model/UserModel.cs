using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyJobTools.Model
{
    public class UserModel
    {
        public string UserCode { get; set; }

        public string UserName { get; set; }

        public string Address { get; set; }

        public DateTime? Birthday { get; set; }
    }

    public class UserQueryModel : AbsQueryBaseModel
    {
        public string UserCode { get; set; }
        public string UserName_Like { get; set; }

        public override string GetWhereFormat()
        {
            StringBuilder where = new StringBuilder();
            if (!String.IsNullOrWhiteSpace(UserCode))
            {
                where.Append(" and UserCode=@UserCode ");
            }
            if (!String.IsNullOrWhiteSpace(UserName_Like))
            {
                where.Append(" and UserName like '%'+@UserName_Like+'%' ");
            }
            return where.ToString();
        }
    }
}
