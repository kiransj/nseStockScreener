using System;
using Helper;

namespace StockDatabase
{
    internal class DateHelper
    {
        private static Logger log = Logger.GetLoggerInstance();
        private readonly static DateTime FirstDay = new DateTime(2000, 1, 1);
        static public int DateToDay(DateTime date)
        {
            if(date.Year < 2000) {
                log.Error($"Date {date} is less than 2000");
                throw new Exception($"Date {date} is less than 2000");
            }
            return (date - FirstDay).Days;
        }

        static public DateTime DayToDate(int day)
        {
            return FirstDay.AddDays(day);
        }
    }

    internal class ConversionHelper 
    {
        public static long convertToLong(string str)
        {
            if(Int64.TryParse(str, out var result))
            {
                return result;
            }
            return 0;
        }

        public static Double convertToDouble(string str)
        {
            if(Double.TryParse(str, out var result))
            {
                return result;
            }
            return 0.0;
        }
    }
}