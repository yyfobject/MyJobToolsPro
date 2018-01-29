using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;

namespace MyJobTools.Dao.BaseDao
{
    public class BaseDaoConn : IDisposable
    {
        #region 静态
        public static IDbConnection GetDbConn(string connKey = null)
        {
            var connection_string = GetConnString(connKey);
            var provider_name = GetProviderName(connKey);
            return CreateConnection(connection_string, provider_name);
        }
        public static string GetConnString(string connKey = null)
        {
            if (String.IsNullOrWhiteSpace(connKey))
            {
                return ConfigurationManager.ConnectionStrings[0].ConnectionString;
            }
            else
            {
                return ConfigurationManager.ConnectionStrings[connKey].ConnectionString;
            }
        }

        public static string GetProviderName(string connKey)
        {
            if (String.IsNullOrWhiteSpace(connKey))
            {
                return ConfigurationManager.ConnectionStrings[0].ProviderName;
            }
            else
            {
                return ConfigurationManager.ConnectionStrings[connKey].ProviderName;
            }
        }
        /// <summary>创建新的连接</summary>
        /// <returns></returns>
        private static IDbConnection CreateConnection(string connectionString, string providerName)
        {
            IDbConnection connObj;
            if (providerName.IndexOf("sqlite", StringComparison.CurrentCultureIgnoreCase) > 0)
            {
                connObj = new SQLiteConnection(connectionString);
            }
            else
            {
                var mfactory = DbProviderFactories.GetFactory(providerName);
                connObj = mfactory.CreateConnection();
                connObj.ConnectionString = connectionString;
            }
            return connObj;
        }


        private static IDbConnection GetConn(string connKey = null)
        {
            var connection_string = GetConnString(connKey);
            var provider_name = GetProviderName(connKey);
            return CreateConnection(connection_string, provider_name);
        }
        #endregion 静态


        //数据库连接对象
        private IDbConnection dbConn;
        private DbProviderFactory dbFactory;
        private string connectionString = null;
        private string providerName = null;
        public string ConnKey = "DefualtConn";
        private IDbTransaction dbTran;

        #region 构造

        /// <summary>构造一个数据库操作类的实例</summary>
        /// <param name="connKey">数据库连接配置项key</param>
        public BaseDaoConn(string connKey = null)
        {
            if (!String.IsNullOrWhiteSpace(connKey))
            {
                this.ConnKey = connKey;
            }
            connectionString = GetConnString(connKey);
            providerName = GetProviderName(connKey);
            dbFactory = DbProviderFactories.GetFactory(providerName);

            if (providerName.IndexOf("sqlite", StringComparison.CurrentCultureIgnoreCase) > 0)
            {
                dbConn = new SQLiteConnection(connectionString);
            }
            else
            {
                dbFactory = DbProviderFactories.GetFactory(providerName);
                dbConn = dbFactory.CreateConnection();
                dbConn.ConnectionString = connectionString;
            }

        }
        #endregion
        
        #region Dispose

        /// <summary>资源回收事件</summary>
        public EventHandler OnDispose { get; set; }

        /// <summary>关闭连接回收资源</summary>
        public void Dispose()
        {
            // 关闭连接
            this.Close();
            if (OnDispose != null) { OnDispose(this, new EventArgs()); }
            dbConn.Dispose();
            dbConn = null;
            System.GC.SuppressFinalize(this);
        }

        #endregion

        #region Open/Close

        /// <summary>打开数据库连接</summary>
        /// <returns></returns>
        public void Open()
        {
            //打开连接
            dbConn.Open();
        }

        /// <summary>关闭数据库连接</summary>
        /// <returns></returns>
        public void Close()
        {
            if (dbConn.State == ConnectionState.Open) { dbConn.Close(); }
        }

        public void Close(bool keepConn)
        {

            if (dbConn.State == ConnectionState.Open) { dbConn.Close(); }
        }

        #endregion

        #region CreateCommand (private)

        /// <summary>创建执行命令连接</summary>
        /// <returns></returns>
        private IDbCommand CreateCommand()
        {
            if (dbConn.State == System.Data.ConnectionState.Closed) { this.Open(); }
            IDbCommand cmd = dbConn.CreateCommand();
            if (dbTran != null) { cmd.Transaction = dbTran; }
            cmd.Parameters.Clear();// 清理执行sql的参数
            return cmd;
        }

        #endregion

        #region CreateParameter

        /// <summary>创建sql参数</summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public DbParameter CreateParameter(string name, object value)
        {
            DbParameter param = dbFactory.CreateParameter();
            param.ParameterName = name;
            param.Value = (value == null ? DBNull.Value : value);
            return param;
        }

        /// <summary>创建sql参数</summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <param name="dbtype"></param>
        /// <returns></returns>
        public DbParameter CreateParameter(string name, object value, DbType dbtype)
        {
            DbParameter param = dbFactory.CreateParameter();
            param.ParameterName = name;
            param.Value = (value == null ? DBNull.Value : value);
            param.DbType = dbtype;
            return param;
        }

        /// <summary>创建sql参数</summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <param name="dbtype"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        public DbParameter CreateParameter(string name, object value, DbType dbtype, int size)
        {
            DbParameter param = dbFactory.CreateParameter();
            param.ParameterName = name;
            param.Value = (value == null ? DBNull.Value : value);
            param.DbType = dbtype;
            param.Size = size;
            return param;
        }

        public DbParameter CreateParameter(string name, object value, ParameterDirection direction)
        {
            DbParameter param = dbFactory.CreateParameter();
            param.ParameterName = name;
            param.Direction = direction;
            param.Value = (value == null ? DBNull.Value : value);
            return param;
        }

        #endregion

        #region ExecuteReader

        /// <summary>执行sql语句（select），返回OleDbDataReader</summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public IDataReader ExecuteReader(string sql)
        {
            return ExecuteReader(sql, null);
        }

        /// <summary>是否有参数</summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        private bool HasParam(DbParameter[] parameters)
        {
            return parameters != null && parameters.Length > 0;
        }

        /// <summary>记录异常与参数日志</summary>
        /// <param name="ex"></param>
        /// <param name="parameters"></param>
        private void LogParamsToEx(DbException ex, DbParameter[] parameters)
        {
            if (!HasParam(parameters))
                return;
            for (int i = 0; i < parameters.Length; i++)
            {
                ex.Data.Add(string.Format("参数{0}-{1}", i + 1, parameters[i].ParameterName),
                    Newtonsoft.Json.JsonConvert.SerializeObject(parameters[i].Value));

            }
        }

        /// <summary>执行sql语句（select），返回OleDbDataReader</summary>
        /// <param name="sql"></param>
        /// <param name="arrparm"></param>
        /// <returns></returns>
        public IDataReader ExecuteReader(string sql, DbParameter[] arrparm)
        {
            bool hasParam = HasParam(arrparm);
            try
            {
                IDataReader resultObj;
                this.Open();
                using (IDbCommand cmd = CreateCommand())
                {
                    cmd.CommandText = sql;
                    if (hasParam)
                    {
                        for (int i = 0; i < arrparm.Length; i++)
                        {
                            cmd.Parameters.Add(arrparm[i]);
                        }
                    }
                    resultObj = cmd.ExecuteReader();
                    if (cmd.Transaction == null)
                    {
                        this.Close();
                    }
                }
                return resultObj;
            }
            catch (DbException ex)
            {
                ex.Data.Add("执行的sql", sql);
                LogParamsToEx(ex, arrparm);
                throw;
            }
        }

        #endregion

        #region ExecuteNonQuery

        /// <summary>执行sql语句（insert，update，delete）</summary>
        /// <param name="sql"></param>
        /// <param name="keepConn">是保持数据库连接状态。如保持连接可能增大数据库压力。</param>
        /// <returns>sql执行影响的记录数</returns>
        public int ExecuteNonQuery(string sql, bool keepConn = false)
        {
            return ExecuteNonQuery(sql, null, keepConn);
        }

        /// <summary>执行带参数的sql语句（insert，update，delete）</summary>
        /// <param name="sql"></param>
        /// <param name="arrparm"></param>
        /// <param name="keepConn">是保持数据库连接状态。如保持连接可能增大数据库压力。</param>
        /// <returns>sql执行影响的记录数</returns>
        public int ExecuteNonQuery(string sql, DbParameter[] arrparm, bool keepConn = false)
        {
            bool hasParam = HasParam(arrparm);
            try
            {
                int resultVal = -1;
                using (IDbCommand cmd = CreateCommand())
                {
                    cmd.CommandText = sql;
                    if (hasParam)
                    {
                        for (int i = 0; i < arrparm.Length; i++)
                        {
                            cmd.Parameters.Add(arrparm[i]);
                        }
                    }
                    resultVal = cmd.ExecuteNonQuery();
                    if (cmd.Transaction == null && !keepConn)
                    {
                        this.Close();
                    }
                }
                return resultVal;
            }
            catch (DbException ex)
            {
                ex.Data.Add("执行的sql", sql);
                LogParamsToEx(ex, arrparm);
                throw;
            }
        }
        #endregion

        #region ExecuteScalar

        /// <summary>执行sql语句，并返回查询所返回的结果集中第一行的第一列的值</summary>
        /// <param name="sql"></param>
        /// <param name="keepConn">是保持数据库连接状态。如保持连接可能增大数据库压力。</param>
        /// <returns></returns>
        public object ExecuteScalar(string sql, bool keepConn = false)
        {
            return ExecuteScalar(sql, null, keepConn);
        }

        /// <summary>执行sql语句，并返回查询所返回的结果集中第一行的第一列的值</summary>
        /// <param name="sql"></param>
        /// <param name="arrparm"></param>
        /// <param name="keepConn">是否强制关闭数据库连接。如保持连接可能增大数据库压力。</param>
        /// <returns></returns>
        public object ExecuteScalar(string sql, DbParameter[] arrparm, bool keepConn = false)
        {
            bool hasParam = HasParam(arrparm);
            try
            {
                object resultObj;
                using (IDbCommand cmd = CreateCommand())
                {
                    cmd.CommandText = sql;
                    if (hasParam)
                    {
                        for (int i = 0; i < arrparm.Length; i++)
                        {
                            cmd.Parameters.Add(arrparm[i]);
                        }
                    }
                    resultObj = cmd.ExecuteScalar();
                }
                if (!keepConn)
                {
                    this.Close();
                }
                return resultObj;
            }
            catch (DbException ex)
            {
                ex.Data.Add("执行的sql", sql);
                LogParamsToEx(ex, arrparm);
                throw;
            }
        }

        #endregion

        #region ExecuteProcedure

        /// <summary>执行存储过程，返回DataReader</summary>
        /// <param name="procname">存储过程名称</param>
        /// <param name="arrparm">参数</param>
        /// <param name="keepConn">是保持数据库连接状态。如保持连接可能增大数据库压力。</param>
        /// <returns>存储过程执行结果DbDataReader</returns>
        public IDataReader ExeDataReaderProcedure(string procname, DbParameter[] arrparm, bool keepConn = false)
        {
            bool hasParam = HasParam(arrparm);
            try
            {
                IDataReader resultObj;
                this.Open();
                using (IDbCommand cmd = CreateCommand())
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = procname;
                    if (hasParam)
                    {
                        for (int i = 0; i < arrparm.Length; i++)
                        {
                            cmd.Parameters.Add(arrparm[i]);
                        }
                    }
                    resultObj = cmd.ExecuteReader();
                    if (cmd.Transaction == null && !keepConn)
                    {
                        this.Close();
                    }
                }
                return resultObj;
            }
            catch (DbException ex)
            {
                ex.Data.Add("sp", procname);
                LogParamsToEx(ex, arrparm);
                throw;
            }
        }

        /// <summary>执行存储过程，返回受影响行数</summary>
        /// <param name="procname">存储过程名称</param>
        /// <param name="arrparm">参数</param>
        /// <param name="keepConn">是保持数据库连接状态。如保持连接可能增大数据库压力。</param>
        public int ExecuteNonProcedure(string procname, DbParameter[] arrparm, bool keepConn = false)
        {
            bool hasParam = HasParam(arrparm);
            try
            {
                int resultVal;
                using (IDbCommand cmd = CreateCommand())
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = procname;
                    if (hasParam)
                    {
                        for (int i = 0; i < arrparm.Length; i++)
                        {
                            cmd.Parameters.Add(arrparm[i]);
                        }
                    }
                    resultVal = cmd.ExecuteNonQuery();
                    //foreach (var p in arrparm)
                    //{
                    //    if (p.Direction == ParameterDirection.Output)
                    //    {
                    //        p.Value = cmd.Parameters[p.ParameterName].Value;
                    //    }
                    //}
                    if (cmd.Transaction == null && !keepConn)
                    {
                        this.Close();
                    }
                }
                return resultVal;
            }
            catch (DbException ex)
            {
                ex.Data.Add("sp", procname);
                LogParamsToEx(ex, arrparm);
                throw;
            }
        }

        /// <summary>执行存储过程，返回DataSet</summary>
        /// <param name="procname">存储过程名称</param>
        /// <param name="arrparm">参数</param>
        /// <param name="keepConn">是保持数据库连接状态。如保持连接可能增大数据库压力。</param>
        /// <returns>存储过程执行结果DataSet</returns>
        public DataSet ExeDatasetProcedure(string procname, DbParameter[] arrparm, bool keepConn = false)
        {
            bool hasParam = HasParam(arrparm);
            try
            {
                DataSet resultDs = new DataSet();
                using (IDbCommand cmd = this.CreateCommand())
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = procname;
                    if (hasParam)
                    {
                        for (int i = 0; i < arrparm.Length; i++)
                        {
                            cmd.Parameters.Add(arrparm[i]);
                        }
                    }
                    IDbDataAdapter da = dbFactory.CreateDataAdapter();
                    da.SelectCommand = cmd;
                    da.Fill(resultDs);
                    if (cmd.Transaction == null && !keepConn)
                    {
                        this.Close();
                    }
                }
                return resultDs;
            }
            catch (DbException ex)
            {
                ex.Data.Add("sp", procname);
                LogParamsToEx(ex, arrparm);
                throw;
            }
        }

        #endregion

        #region GetDataSet

        /// <summary>执行sql语句返回DataSet</summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public DataSet GetDataSet(string sql, bool keepConn = false)
        {
            return GetDataSet(sql, null, keepConn);
        }

        /// <summary>执行sql语句返回DataSet</summary>
        /// <param name="sql"></param>
        /// <param name="arrparm"></param>
        /// <returns></returns>
        public DataSet GetDataSet(string sql, DbParameter[] arrparm, bool keepConn = false)
        {
            bool hasParam = HasParam(arrparm);
            try
            {
                DataSet resultDs = new DataSet();
                using (IDbCommand cmd = this.CreateCommand())
                {
                    cmd.CommandText = sql;
                    if (hasParam)
                    {
                        for (int i = 0; i < arrparm.Length; i++)
                        {
                            cmd.Parameters.Add(arrparm[i]);
                        }
                    }
                    IDbDataAdapter da = dbFactory.CreateDataAdapter();
                    da.SelectCommand = cmd;
                    da.Fill(resultDs);
                    if (cmd.Transaction == null && !keepConn)
                    {
                        this.Close();
                    }
                }
                return resultDs;
            }
            catch (DbException ex)
            {
                ex.Data.Add("Exec sql", sql);
                LogParamsToEx(ex, arrparm);
                throw;
            }
        }

        #endregion

        #region Transaction处理

        /// <summary>开始事务处理</summary>
        /// <returns></returns>
        public void TrnStart()
        {
            if (dbConn.State == System.Data.ConnectionState.Closed) { this.Open(); }
            dbTran = dbConn.BeginTransaction();
        }

        /// <summary>提交事务</summary>
        /// <returns></returns>
        public void TrnCommit()
        {
            if (dbConn.State == System.Data.ConnectionState.Closed) { this.Open(); }
            if (dbTran != null) { dbTran.Commit(); }
        }

        /// <summary>撤销事务</summary>
        /// <returns></returns>
        public void TrnRollBack()
        {
            try
            {
                if (dbConn.State == System.Data.ConnectionState.Closed) { this.Open(); }
                if (dbTran != null) { dbTran.Rollback(); }
            }
            catch { }
        }

        #endregion
    }
}
