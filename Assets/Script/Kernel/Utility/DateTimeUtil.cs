using System;
using UnityEngine;

public class DateTimeUtil
{
    // 全世界时间戳都是Utc时间，但是每个地方的时区不同
    // 时间戳是一个绝对概念，但是“天”或者“凌晨”是一个相对概念
    // 每个地方的时间戳是一样的，但是每个地区凌晨所对应时间戳是不一样的
    // 如果要计算某个时区凌晨1点的utc时间
    // 需要先计算utc凌晨1点的时间戳，然后加上时区的offset
    // 就是某时区凌晨1点的utc时间戳
    // 在全球服务器中，所有需要计算"天"概念的时间时
    // 都要把客户端返回的utc时间和时区offset计算出当地凌晨
    // 
    public  static long oneDayTimestamps = 60 * 60 * 24L;
    public  static int AreaTimeOffset = 60 * 60 * 8;

    /// <summary>  
    /// 获取时间戳  秒
    /// linux时间戳用int表示最大可以到2038年1月19日
    /// </summary>  
    /// <returns></returns>  
    public static long GetTimeStamp()
    {
        TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
        return Convert.ToInt64(ts.TotalSeconds);
    }
    /// <summary>
    /// 返回给定时间的utc 时间戳
    /// </summary>
    /// <param name="date"></param>
    /// <returns></returns>
    public static long GetTimeStamp(DateTime date)
    {
        if (date.Kind == DateTimeKind.Local)
        {
            var utcDate = date.ToUniversalTime();
            TimeSpan ts = utcDate - new DateTime(1970, 1, 1);
            return Convert.ToInt64(ts.TotalSeconds);
        }
        else
        {
            TimeSpan ts = date - new DateTime(1970, 1, 1);
            return Convert.ToInt64(ts.TotalSeconds);
        }
    }
    /// <summary>
    /// 返回给定时间所在时区对应的utc时间戳
    /// </summary>
    /// <param name="date"></param>
    /// <returns></returns>
    public static long GetLocalTimeStamp(DateTime date, int timeZoneOffset)
    {
        return GetTimeStamp(date) + timeZoneOffset;
    }
    /// <summary>
    /// 返回给定时间所在时区对应的utc时间戳
    /// </summary>
    /// <param name="date"></param>
    /// <returns></returns>
    public static long GetLocalTimeStamp(DateTime date)
    {
        return GetLocalTimeStamp(date, GetLocalTimeZoneOffset());
    }
    /// <summary>  
    /// 获取时间戳  毫秒
    /// </summary>  
    /// <returns></returns>  
    public static long GetTimeStampMillisecond()
    {
        TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
        return Convert.ToInt64(ts.TotalSeconds * 1000);
    }
    /// <summary>
    /// 将时间戳转换为日期类型，并格式化
    /// </summary>
    /// <param name="longDateTime"></param>
    /// <returns></returns>
    public static string LongDateTimeToDateTimeString(string longDateTime)
    {
        //用来格式化long类型时间的,声明的变量
        long unixDate;
        DateTime start;
        DateTime date;
        //ENd

        unixDate = long.Parse(longDateTime);
        start = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        date = start.AddMilliseconds(unixDate).ToLocalTime();

        return date.ToString("yyyy-MM-dd HH:mm:ss");

    }
    /// <summary>
    /// 将秒转为时间格式，HH:mm:ss
    /// </summary>
    /// <param name="time"></param>
    /// <returns></returns>
    public static string SecondToTime(int time)
    {
        TimeSpan ts = new TimeSpan(0, 0, time);
        return string.Format("{0}:{1}:{2}", ts.Hours, ts.Minutes, ts.Seconds);
    }


    /// <summary>
    /// 转换时间戳到DataTime结构
    /// </summary>
    /// <param name="timeStamp">unix时间戳 单位S</param>
    /// <returns></returns>
    public static DateTime ConvertTimestampToDateTime(long timeStamp)
    {
        DateTime dtStart = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
        TimeSpan toNow = new TimeSpan(timeStamp* 10000000);
        DateTime targetDt = dtStart.Add(toNow);
        return targetDt;
    
    }

    /// <summary>
    /// 0时区 获取和上一天的零点时间时间戳 Utc
    /// </summary>
    /// <returns>时间戳 单位 S</returns>
    /*public static long GetTodayTimeStamp()
    {
        long currentTimestamps = GetTimeStamp();
       
        return currentTimestamps - currentTimestamps % oneDayTimestamps;
    }*/
    /// <summary>
    /// 获得本地0点的Utc时间
    /// </summary>
    /// <returns></returns>
    public static long GetLocalTodayTimeStamp()
    {
        long currentTimestamps = GetTimeStamp();

        return currentTimestamps - (currentTimestamps + GetLocalTimeZoneOffset()) % oneDayTimestamps;
    }
    /// <summary>
    /// 获得本地0点的Utc时间
    /// </summary>
    /// <returns></returns>
   /* public static long GetLocalTodayTimeStamp(int timeZoneOffset)
    {
        return GetTodayTimeStamp() + timeZoneOffset;
    }*/
    public static int GetLocalTimeZoneOffset()
    {
        return (int)TimeZone.CurrentTimeZone.GetUtcOffset(DateTime.Now).TotalSeconds;
    }
    /// <summary>
    /// utc时间-获取给定utc时间戳和当前utc时间戳的差值，单位S
    /// </summary>
    /// <returns></returns>
    public static int GetDValueDestineTime(long timestamp)
    {//如果给定参数时间戳大于utc当前时间戳，需自行判断正负
        return (int)(GetTimeStamp() - timestamp);
    }
    /// <summary>
    /// 距离本周几凌晨0点的时间差 S(本地时间)
    /// </summary>
    /// <param name="dw">周几</param>
    /// <returns></returns>
    public static long GetDValueWeekTime(string unixTimeStamp,DayOfWeek dw, bool isStart = true)
    {
        if (unixTimeStamp == "0")
            return 0;
        DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1)); // 当地时区
        DateTime now = startTime.AddSeconds(double.Parse(unixTimeStamp));
        DateTime next = now.AddDays(1);

        DateTime temp = new DateTime(next.Year, next.Month, next.Day,0,0,0);
        temp = DateTime.SpecifyKind(temp, DateTimeKind.Local);
        int count = 0,wstweek = now.DayOfWeek == DayOfWeek.Sunday ? 7 : (int)now.DayOfWeek,
        estweek = dw == DayOfWeek.Sunday ? 7 : (int)dw;

        if (isStart)
        {
            count = estweek - wstweek;
            temp = temp.AddDays(count);
        }
        else
        {
            count = estweek - wstweek;
            if (count < 0)
            {
                count += 7;
            }
            temp = temp.AddDays(count);
        }
       

        long difftime = GetTimeStamp(temp) - GetTimeStamp(now);
        return difftime > 0 ? difftime : 0;
    }
    /// <summary>
    /// 检查防沉迷时间，晚上22点到早上8点之间返回假
    /// </summary>
    /// <returns></returns>
    public static bool CheckChildPlayTime()
    {
        DateTime nDate = DateTime.Now;
        DateTime sDate = new DateTime(nDate.Year, nDate.Month, nDate.Day, 8, 0, 0);
        DateTime eDate = new DateTime(nDate.Year, nDate.Month, nDate.Day, 22, 0, 0);
        if (nDate > sDate && nDate < eDate)
        {
            return true;
        }
        return false;
    }
}
