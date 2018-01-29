//using Dapper;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;
using System.Data.SqlClient;

namespace MyJobTools.Dao.BaseDao
{
    public class SqlConnBase
    {

        /// <summary>
        /// 使用 SqlBulkCopy 批量向数据库指定表中新增数据。
        /// </summary>
        /// <param name="dt">插入数据库的数据。列名要与数据库中目标表的列名一致。</param>
        /// <param name="tableName">要插入数据的数据库中的表名</param>
        public void InsertByBulkCopy(DataTable dt, string tableName)
        {
            using (SqlConnection sqlConn = new SqlConnection(BaseDaoConn.GetConnString()))
            {
                SqlBulkCopy bulkCopy = new SqlBulkCopy(sqlConn);
                bulkCopy.DestinationTableName = tableName;
                bulkCopy.BatchSize = dt.Rows.Count;
                try
                {
                    sqlConn.Open();
                    if (dt != null && dt.Rows.Count != 0)
                        bulkCopy.WriteToServer(dt);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    sqlConn.Close();
                    if (bulkCopy != null)
                        bulkCopy.Close();
                    sqlConn.Dispose();
                }
            }
        }

        /// <summary>
        /// 使用表值参数批量插入数据。因为需要在数据库建立表侄参数对象，最好只在有需要特殊处理的地方使用。
        /// </summary>
        /// <param name="dt">插入数据库的数据。结构要与数据库中表值参数一致。</param>
        /// <param name="tableName">要插入数据的数据库中的表名</param>
        public void InserByTableValued(DataTable dt, string tableName)
        {
            using (SqlConnection sqlConn = new SqlConnection(BaseDaoConn.GetConnString()))
            {
                string TSqlStatement = "insert into " + tableName + " (Id,UserName,Pwd)" +
                 " SELECT nc.Id, nc.UserName,nc.Pwd" + " FROM @TableParam1 AS nc";
                SqlCommand cmd = new SqlCommand(TSqlStatement, sqlConn);
                SqlParameter catParam = cmd.Parameters.AddWithValue("@TableParam1", dt);
                catParam.SqlDbType = SqlDbType.Structured;
                //表值参数的名字叫BulkUdt，在上面的建立测试环境的SQL中有。  
                catParam.TypeName = "dbo.BulkUdt";//这里是在数据库预先建立的表值参数名称

                /*****************
   CREATE TYPE BulkUdt AS TABLE  
  (Id int,  
   UserName nvarchar(32),  
   Pwd varchar(16)) 
                 ****************/

                try
                {
                    sqlConn.Open();
                    if (dt != null && dt.Rows.Count != 0)
                    {
                        cmd.ExecuteNonQuery();
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    sqlConn.Close();
                    sqlConn.Dispose();
                }
            }
        }


    }
}
