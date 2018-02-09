using MyJobTools.Dao.BaseDao;
using MyJobTools.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyJobTools.Dao
{
    public class UserDao
    {
        public List<UserModel> GetPage(UserQueryModel queryModel, PageQueryModel pageModel)
        {
            BaseDapperHelper dbHelper = new BaseDapperHelper("DefualtConn");
            //return dbHelper.GetPageFromTable<UserModel>("User", queryModel, pageModel).ToList();
            string sql = String.Format("select * from User where 1=1 {0} {1} limit @Limit offset @Page;", queryModel.GetWhereFormat(), queryModel.GetOrderBy());
            string sqlCount = String.Format("select count(1) RowsCount from User where 1=1 {0} ", queryModel.GetWhereFormat());
            return dbHelper.GetPage<UserModel>(sql, sqlCount, queryModel, pageModel).ToList();
        }

        public int Insert(UserModel item)
        {
            BaseDapperHelper dbHelper = new BaseDapperHelper("DefualtConn");
            return dbHelper.Execute("INSERT INTO User (UserCode, UserName, Address,Birthday)  VALUES(@UserCode, @UserName, @Address,@Birthday);  ", item);
        }
    }
}
