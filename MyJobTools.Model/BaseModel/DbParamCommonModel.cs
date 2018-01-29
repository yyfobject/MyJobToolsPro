using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;

namespace TMSModule
{
    public class DbParamCommonModel
    {
        public string Key { get; set; }
        public object Val { get; set; }

        /// <summary>
        /// 参数类型
        /// </summary>
        public ParameterDirection ParamType { get; set; }

        public DbParamCommonModel() { ParamType = ParameterDirection.Input; }

        public static DbParamCommonModel Create(string paramKey, object paramVal, ParameterDirection paramType = ParameterDirection.Input)
        { return new DbParamCommonModel() { Key = paramKey, Val = paramVal, ParamType = paramType }; }

        public DbParameter GetDbParamObj(CommonDbContext dbContext)
        {
            //ParameterDirection
            //var Direction = (ParameterDirection)Enum.Parse(typeof(ParameterDirection), Convert.ToInt32(ParamType).ToString());
            return dbContext.CreateParameter(this.Key, this.Val, ParamType);
        }
    }

    //public enum DbParamTypeEnum
    //{
    //    // 摘要:
    //    //     参数是输入参数。
    //    Input = 1,
    //    //
    //    // 摘要:
    //    //     参数是输出参数。
    //    Output = 2,
    //    //
    //    // 摘要:
    //    //     参数既能输入，也能输出。
    //    InputOutput = 3,
    //    //
    //    // 摘要:
    //    //     参数表示诸如存储过程、内置函数或用户定义函数之类的操作的返回值。
    //    ReturnValue = 6,
    //}
}
