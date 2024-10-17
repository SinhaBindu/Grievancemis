//using Microsoft.AspNet.Identity.EntityFramework;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
//using Microsoft.Owin.Security.DataHandler;
using System.Web.Script.Serialization;

namespace Grievancemis.Manager
{
    public static class Extension
    {
        public static bool IsGuidNullorEmpty(this Guid guidId)
        {
            return guidId == Guid.Empty;
        }

        public static bool IsGuidNullorEmpty(this Guid? guidId)
        {
            if (!guidId.HasValue)
                return true;
            Guid? nullable = guidId;
            Guid empty = Guid.Empty;
            if (!nullable.HasValue)
                return false;
            return !nullable.HasValue || nullable.GetValueOrDefault() == empty;
        }

        public static string ToDateTimeDDMMYYYY(this DateTime dateTime)
        {
            return !(dateTime == DateTime.MinValue) ? dateTime.ToString("dd-MMM-yyyy") : string.Empty;
        }

        public static string ToDateTimeDDMMYYYY(this DateTime? dateTime)
        {
            return dateTime.DateTimeNullOrEmpty() ? string.Empty : dateTime.Value.ToString("dd-MMM-yyyy");
        }

        public static string ToDateTimeyyyyMMdd(this DateTime dateTime)
        {
            return !(dateTime == DateTime.MinValue) ? dateTime.ToString("yyyy-MM-dd") : string.Empty;
        }

        public static string ToDateTimeyyyyMMdd(this DateTime? dateTime)
        {
            return dateTime.DateTimeNullOrEmpty() ? string.Empty : dateTime.Value.ToString("yyyy-MM-dd");
        }

        public static bool DateTimeNullOrEmpty(this DateTime? dateTime)
        {
            if (dateTime.HasValue)
            {
                DateTime? nullable = dateTime;
                DateTime dateTime1 = new DateTime();
                if ((nullable.HasValue ? (nullable.HasValue ? (nullable.GetValueOrDefault() == dateTime1 ? 1 : 0) : 1) : 0) == 0)
                    return false;
            }
            return true;
        }

        public static bool DateTimeNullOrEmpty(this DateTime dateTime)
        {
            return dateTime == new DateTime();
        }

        public static string ToTimehhmmtt(this TimeSpan timeSpan)
        {
            return DateTime.Today.Add(timeSpan).ToString("hh:mm tt");
        }
        //public static string ToTimestamp(this DateTime value)
        //{
        //    return value.ToString("yyyyMMddHHmmssffff");
        //}
        public static string ToJSON(this object obj)
        {
            return new JavaScriptSerializer().Serialize(obj);
        }
        public static string ToJSON(this DataTable dataTable)
        {
            DataTable dt = new DataTable();
            dt = dataTable;

            System.Web.Script.Serialization.JavaScriptSerializer serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
            List<Dictionary<string, object>> rows = new List<Dictionary<string, object>>();
            Dictionary<string, object> row;
            foreach (DataRow dr in dt.Rows)
            {
                row = new Dictionary<string, object>();
                foreach (DataColumn col in dt.Columns)
                {
                    row.Add(col.ColumnName, dr[col]);
                }
                rows.Add(row);
            }
            return serializer.Serialize(rows);
        }

        public static string ToJSON(this object obj, int recursionDepth)
        {
            return new JavaScriptSerializer()
            {
                RecursionLimit = recursionDepth
            }.Serialize(obj);
        }


        public static string DataTableToJson(this DataTable dataTable)
        {
            if (dataTable == null)
            {
                return string.Empty;
            }
            return HttpUtility.HtmlDecode(Newtonsoft.Json.JsonConvert.SerializeObject(dataTable));
        }
        public static dynamic ParseToJson(this object obj)
        {
            return JsonConvert.DeserializeObject(obj?.ToString());
        }


        public static SelectList ToSelectList<TEnum>(this TEnum enumObj)
     where TEnum : struct, IComparable, IFormattable, IConvertible
        {
            var values = from TEnum e in Enum.GetValues(typeof(TEnum))
                         select new { Id = e, Name = e.ToString() };
            return new SelectList(values, "Id", "Name", enumObj);
        }

        public static List<SelectListItem> ToSelectList(this DataTable table, string valueField, string textField)
        {
            List<SelectListItem> list = new List<SelectListItem>();

            foreach (DataRow row in table.Rows)
            {
                list.Add(new SelectListItem()
                {
                    Text = row[textField].ToString(),
                    Value = row[valueField].ToString()
                });
            }

            //return new SelectListItem(list, "Value", "Text");
            return list;
        }



    }


}