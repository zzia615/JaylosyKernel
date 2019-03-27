using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace Jaylosy.Kernel
{
    /// <summary>
    /// 扩展函数
    /// </summary>
    public static class ExtensiveFunc
    {
        public static void AppendAnd(this StringBuilder strBuilder,string data)
        {
            if(strBuilder.Length>0)
                strBuilder.Append(" and ");
            strBuilder.Append(data);
        }

        public static void AppendOr(this StringBuilder strBuilder, string data)
        {
            if (strBuilder.Length > 0)
                strBuilder.Append(" or ");
            strBuilder.Append(data);
        }

        #region 数据转换
        /// <summary>
        /// 转换为布尔值
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static bool AsBoolean(this object data, bool defaultValue = false)
        {
            bool temp = defaultValue;
            if (data == null)
                return temp;
            try
            {
                temp = Convert.ToBoolean(data);
            }
            catch
            {

            }
            return temp;
        }
        /// <summary>
        /// 转换为字符串
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static string AsString(this object data, string defaultValue = "")
        {
            string temp = defaultValue;
            if (data == null)
                return temp;
            try
            {
                temp = data.ToString();
            }
            catch
            {
            }
            return temp;
        }
        /// <summary>
        /// 转换有符号的32位整数
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static int AsInt(this object data, int defaultValue = 0)
        {
            int temp = defaultValue;
            if (data == null)
                return temp;
            try
            {
                temp = Convert.ToInt32(data);
            }
            catch
            {


            }
            return temp;
        }


        /// <summary>
        /// 转换双精度浮点数
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static double AsDouble(this object data, double defaultValue = 0)
        {
            double temp = defaultValue;
            if (data == null)
                return temp;
            try
            {
                temp = Convert.ToDouble(data);
            }
            catch
            {
            }
            return temp;
        }
        /// <summary>
        /// 转换为有符号64位整数
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static long AsLong(this object data, long defaultValue = 0)
        {
            long temp = defaultValue;
            if (data == null)
                return temp;
            try
            {
                temp = Convert.ToInt64(data);
            }
            catch
            {


            }
            return temp;
        }
        /// <summary>
        /// 转换为等效十进制
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static decimal AsDecimal(this object data, decimal defaultValue = 0)
        {
            decimal temp = defaultValue;
            if (data == null)
                return temp;
            try
            {
                temp = Convert.ToDecimal(data);
            }
            catch
            {


            }
            return temp;
        }
        /// <summary>
        /// 转换成DateTime对象
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static DateTime AsDateTime(this object data)
        {
            DateTime temp = DateTime.Parse("1900-01-01");
            if (data == null)
                return temp;
            try
            {
                temp = Convert.ToDateTime(data);
            }
            catch
            {


            }
            return temp;
        }

        /// <summary>
        /// 转换可为空的Datetime
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static DateTime? AsDateTimeNullable(this object data)
        {
            DateTime? temp = null;
            if (data == null)
                return temp;
            try
            {
                temp = Convert.ToDateTime(data);
            }
            catch
            {


            }
            return temp;
        }
        /// <summary>
        /// 转换成DateTime对象
        /// </summary>
        /// <param name="data"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static DateTime AsDateTime(this object data, DateTime defaultValue)
        {
            DateTime temp = defaultValue;
            if (data == null)
                return temp;
            try
            {
                temp = Convert.ToDateTime(data);
            }
            catch
            {


            }
            return temp;
        }
        #endregion

        #region 校验数字等类型
        /// <summary>
        /// 校验是否为数字
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool IsNumeric(this string value)
        {
            return Regex.IsMatch(value, @"^[+-]?\d*[.]?\d*$");
        }
        /// <summary>
        /// 校验是否是Int
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool IsInt(this string value)
        {
            return Regex.IsMatch(value, @"^[+-]?\d*$");
        }
        /// <summary>
        /// 校验是否是无符号
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool IsUnsign(this string value)
        {
            return Regex.IsMatch(value, @"^\d*[.]?\d*$");
        }
        /// <summary>
        /// 校验是否电话号码
        /// </summary>
        /// <param name="strInput"></param>
        /// <returns></returns>
        public static bool isTel(this string strInput)
        {
            return Regex.IsMatch(strInput, @"\d{3}-\d{8}|\d{4}-\d{7}");
        } 
        #endregion


        #region DataTable转实体类
        /// <summary>
        /// 获取实体类
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="table"></param>
        /// <returns></returns>
        public static List<T> GetEntity<T>(this DataTable table) where T:class,new()
        {
            //如果没有记录，则返回空集合
            if (table == null || table.Rows.Count == 0)
            {
                return new List<T>();
            }

            //转换
            List<T> list = new List<T>();
            foreach (DataRow row in table.Rows)
            {
                T t = new T();
                PropertyInfo[] properties = typeof(T).GetProperties();
                foreach (var property in properties)
                {
                    //校验DataTable的列是否存在。
                    if(table.Columns.Contains(property.Name))
                    {
                        object obj = row[property.Name];
                        property.SetValue(t, ConvertObj(obj), null);
                    }
                }
                list.Add(t);

            }
            return list;
        }

        private static object ConvertObj(object obj)
        {
            if (obj == DBNull.Value)
            {
                return null;
            }
            if (obj.GetType() == typeof(System.Byte))
            {
                try
                {
                    return (byte)obj;
                }
                catch
                {

                    return null;
                }
            }
            if (obj.GetType() == typeof(System.Int32)||(obj.GetType() == typeof(System.Int16)))
            {
                try
                {
                    return (int)obj;
                }
                catch
                {

                    return null;
                }
            }
            else if(obj.GetType() == typeof(System.Int64))
            {
                try
                {
                    return (long)obj;
                }
                catch
                {

                    return null;
                }
            }
            else if(obj.GetType() == typeof(System.Double))
            {
                try
                {
                    return (double)obj;
                }
                catch
                {

                    return null;
                }
            }
            else if (obj.GetType() == typeof(System.Decimal))
            {
                try
                {
                    return (decimal)obj;
                }
                catch
                {

                    return null;
                }
            }
            else if (obj.GetType() == typeof(System.Single))
            {
                try
                {
                    return (float)obj;
                }
                catch
                {

                    return null;
                }
            }
            else if (obj.GetType() == typeof(System.DateTime))
            {
                try
                {
                    return (DateTime)obj;
                }
                catch
                {

                    return null;
                }
            }
            else if (obj.GetType() == typeof(System.String))
            {
                try
                {
                    return obj.ToString();
                }
                catch
                {

                    return "";
                }
            }
            else
            {
                return obj;
            }
        }


        #endregion
    }
}
