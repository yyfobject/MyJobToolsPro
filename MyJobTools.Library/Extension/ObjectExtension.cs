using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MyJobTools.Library.Extension
{
    /// <summary>对象函数扩展</summary>
    public static class ObjectExtension
    {
        /// <summary>获取当前字符串MD5Hash后的字符值（加密编码格式默认UTF-8）</summary>
        /// <param name="str">当前字符对象</param>
        /// <param name="encoding">编码格式,default utf-8</param>
        /// <returns></returns>
        //public static string MD5Hash(this string str, string encoding = "UTF-8")
        //{
        //    return Utils.MD5Hash(str, encoding);
        //}

        /// <summary>获取当前字符串SHA1Hash后的字符值（加密编码格式默认UTF-8）</summary>
        /// <param name="str">当前字符对象</param>
        /// <param name="encoding">编码格式,default utf-8</param>
        /// <returns></returns>
        //public static string SHA1Hash(this string str, string encoding = "UTF-8")
        //{
        //    return Utils.SHA1Hash(str, encoding);
        //}

        /// <summary>将指定字符串中的格式项替换为指定数组中相应对象的字符串表示形式。</summary>
        /// <param name="value">复合格式字符串。</param>
        /// <param name="args">一个对象数组，其中包含零个或多个要设置格式的对象。</param>
        /// <returns>format 的副本，其中的格式项已替换为 args 中相应对象的字符串表示形式。</returns>
        public static string StrFormat(this string value, params object[] args)
        {
            return String.Format(value, args);
        }

        /// <summary>确定此字符串实例中是否包含指定的字符串匹配。</summary>
        /// <param name="value">要测试的字符串。</param>
        /// <param name="with">要比较的字符串。</param>
        /// <returns>如果 with 参数在要测试的字符串出现，则为true。</returns>
        public static bool InWith(this string value, string with)
        {
            return value.IndexOf(with, StringComparison.CurrentCultureIgnoreCase) > -1;
        }

        /// <summary>指示指定的字符串是 null、空还是仅由空白字符组成。</summary>
        /// <param name="value">要测试的字符串。</param>
        /// <returns>如果 value 参数为 null 或 System.String.Empty，或者如果 value 仅由空白字符组成，则为 true。</returns>
        public static bool IsNullOrWhiteSpace(this string value)
        {
            return String.IsNullOrWhiteSpace(value);
        }

        /// <summary>获取当前string类型转换成long类型的值</summary>
        /// <param name="value">当前对象值</param>
        /// <returns></returns>
        public static long ToLong(this string value, long? putDefault = null)
        {
            if (string.IsNullOrEmpty(value)) { return putDefault ?? 0; }
            return Convert.ToInt64(value);
        }

        /// <summary>获取当前string类型转换成int类型的值</summary>
        /// <param name="value">当前对象值</param>
        /// <returns></returns>
        public static int ToInt(this string value, int? putDefault = null)
        {
            if (string.IsNullOrEmpty(value)) { return putDefault ?? 0; }
            return Convert.ToInt32(value);
        }

        /// <summary>获取当前对象值转换成double类型的值</summary>
        /// <param name="value">当前对象值</param>
        /// <returns></returns>
        public static double ToDouble(this object value, double? putDefault = null)
        {
            return ((value == null || value == DBNull.Value) ? (putDefault ?? 0) : Convert.ToDouble(value.ToString()));
        }

        /// <summary>获取当前对象值转换成decimal类型的值</summary>
        /// <param name="value">当前对象值</param>
        /// <returns></returns>
        public static decimal ToDecimal(this object value, decimal? putDefault = null)
        {
            try
            {
                return Convert.ToDecimal(ToDouble(value));
            }
            catch (Exception ex)
            {
                Exception exNew = new Exception(value.ToString() + "无法转换成Decimal类型。", ex);
                throw exNew;
            }
        }

        /// <summary>转换成时间类型</summary>
        /// <returns></returns>
        public static DateTime? ToDateTime(this string value)
        {
            if (String.IsNullOrWhiteSpace(value))
            {
                return null;
            }
            try
            {
                return Convert.ToDateTime(value);
            }
            catch (Exception ex)
            {
                Exception exNew = new Exception(value.ToString() + "无法转换成DateTime类型。", ex);
                throw exNew;
            }
        }

        /// <summary>获取当前时间戳</summary>
        /// <returns></returns>
        public static long ToTimeStamp(this DateTime date)
        {
            return Convert.ToInt64((date - TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1))).TotalMilliseconds);
        }

        /// <summary>转换时间戳为日期</summary>
        /// <param name="timestamp">时间戳</param>
        /// <returns></returns>
        public static DateTime ToDateTime(this long timestamp)
        {
            return TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1)).Add(new TimeSpan(timestamp * 10000));
        }

        /// <summary>对象是否为null</summary>
        /// <param name="obj">当前对象</param>
        /// <returns>对象为空true，否则false</returns>
        public static bool IsNull(this object obj)
        {
            return null == obj;
        }

        public static string ToJson<T>(this T obj, string dateFormat = null) where T : class
        {
            if (String.IsNullOrWhiteSpace(dateFormat))
            {
                dateFormat = "yyyy-MM-dd HH:mm:ss";
            }
            IsoDateTimeConverter dtConverter1 = new IsoDateTimeConverter { DateTimeFormat = dateFormat };

            Newtonsoft.Json.JsonSerializerSettings setting = new Newtonsoft.Json.JsonSerializerSettings();
            setting.Converters.Add(dtConverter1);

            setting.ContractResolver = new NullToEmptyStringResolver();

            //空值处理
            setting.NullValueHandling = NullValueHandling.Include;
            setting.DefaultValueHandling = DefaultValueHandling.Include;

            return Newtonsoft.Json.JsonConvert.SerializeObject(obj, setting);
        }
        #region Newtonsoft 序列化使用方法，string=null时指定为 ""
        private class NullToEmptyStringResolver : Newtonsoft.Json.Serialization.DefaultContractResolver
        {
            protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization)
            {
                return type.GetProperties()
                        .Select(p =>
                        {
                            var jp = base.CreateProperty(p, memberSerialization);
                            jp.ValueProvider = new NullToEmptyStringValueProvider(p);
                            return jp;
                        }).ToList();
            }
        }

        private class NullToEmptyStringValueProvider : Newtonsoft.Json.Serialization.IValueProvider
        {
            PropertyInfo _MemberInfo;
            public NullToEmptyStringValueProvider(PropertyInfo memberInfo)
            {
                _MemberInfo = memberInfo;
            }

            public object GetValue(object target)
            {
                object result = _MemberInfo.GetValue(target);
                if (_MemberInfo.PropertyType == typeof(string) && result == null) result = "";
                return result;

            }

            public void SetValue(object target, object value)
            {
                _MemberInfo.SetValue(target, value);
            }
        }
        #endregion Newtonsoft 序列化使用方法，string=null时指定为 ""
        public static string GetNotNullStr(this string obj)
        {
            return obj ?? String.Empty;
        }

        /// <summary>
        /// 将可为空的时间类型转换为匹配格式的字符串，null值返回空字符串
        /// </summary>
        /// <param name="format">格式化字符串</param>
        /// <returns></returns>
        public static string GetNotNullStr(this DateTime? obj, string format = null)
        {
            if (obj.HasValue)
            {
                if (String.IsNullOrWhiteSpace(format))
                {
                    return obj.Value.ToString();
                }
                else
                {
                    return obj.Value.ToString(format);
                }
            }
            return "";
        }
    }
}
