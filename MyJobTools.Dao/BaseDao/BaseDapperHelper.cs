using Dapper;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;
using MyJobTools.Model;

namespace MyJobTools.Dao.BaseDao
{
    /// <summary>
    /// Dapper 辅助类。提供常用基础功能封装。如果不支付则需要自己实现。
    /// </summary>
    public class BaseDapperHelper
    {
        #region 参数
        private IDbConnection dbCnn { get; set; }
        private int? TimeOut { get; set; }

        private IDbTransaction dbTran { get; set; }
        #endregion 参数

        public BaseDapperHelper(string connKey = null)
        {
            dbCnn = BaseDaoConn.GetDbConn(connKey);
        }

        #region 事务
        public void BeginTran()
        {
            dbTran = dbCnn.BeginTransaction();
        }
        public void CommitTran()
        {
            dbTran.Commit();
        }
        public void RollBackTran()
        {
            dbTran.Rollback();
        }
        #endregion 事务

        #region 开启/关闭数据库连接对象
        private void Open()
        {
            dbCnn.Open();
        }
        private void Close()
        {
            if (dbCnn.State == ConnectionState.Open) { dbCnn.Close(); }
        }
        #endregion 开启/关闭数据库连接对象

        public IEnumerable<T> GetList<T>(string sql, object paramObj)
        {
            var list = dbCnn.Query<T>(sql, paramObj, null, true, TimeOut);
            return list;
        }

        public IEnumerable<T> GetListFromTable<T>(string tableName, AbsQueryBaseModel paramObj)
        {
            var sql = String.Format("select * from {0} where {1} {2}", tableName, paramObj.GetWhereFormat(), paramObj.GetOrderBy());
            var list = dbCnn.Query<T>(sql, paramObj, null, true, TimeOut);
            return list;
        }

        /// <summary>
        /// 分页查询
        /// </summary>
        /// <returns></returns>
        public IEnumerable<T> GetPage<T>(string sql, string sqlCount, AbsQueryBaseModel queryModel, PageQueryModel pageModel)
        {
            DynamicParameters pars = new DynamicParameters(queryModel);
            pars.Add("Page", pageModel.GetPage(), DbType.Int32);
            pars.Add("Limit", pageModel.GetLimit(), DbType.Int32);
            //pars.Add("RowsCount", pageModel.RowsCount, DbType.Int32, ParameterDirection.Output);
            var list = dbCnn.Query<T>(sql, pars, null, true, TimeOut);
            pageModel.RowsCount = dbCnn.ExecuteScalar<int>(sqlCount, pars, null, TimeOut);
            return list;
        }


        //public IEnumerable<T> GetPageFromTable<T>(string tableName, AbsQueryBaseModel queryModel, PageQueryModel pageModel)
        //{
        //    var sql = String.Format("select * from (select row_number() over(order by {2}) row_Num,* from {0} where {1} ) T where row_Num>=@StartRow and row_Num<@EndRow ", tableName, queryModel.GetWhereFormat(), queryModel.GetOrderBy());
        //    return GetPage<T>(sql, queryModel, pageModel);
        //}

        public object ExecuteScalar(string sql, object paramObj)
        {
            var result = dbCnn.ExecuteScalar(sql, paramObj, dbTran, TimeOut);
            return result;
        }

        /// <summary>
        /// 执行存储过程，返回select第一行第一列
        /// </summary>
        public object ExecuteScalarStoredProcedure(string sql, object paramObj)
        {
            var result = dbCnn.ExecuteScalar(sql, paramObj, dbTran, TimeOut, CommandType.StoredProcedure);
            return result;
        }

        public int Execute(string sql, object paramObj)
        {
            var result = dbCnn.Execute(sql, paramObj, dbTran, TimeOut);
            return result;
        }

        /// <summary>
        /// 执行存储过程，返回受影响行数
        /// </summary>
        public int ExecuteStoredProcedure(string sql, object paramObj)
        {
            var result = dbCnn.Execute(sql, paramObj, dbTran, TimeOut, CommandType.StoredProcedure);
            return result;
        }
    }
}
