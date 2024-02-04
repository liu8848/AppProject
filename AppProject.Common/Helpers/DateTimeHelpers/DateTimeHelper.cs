namespace AppProject.Common.Helpers.DateTimeHelpers;

public static class DateTimeHelper
{
    public static string DateToTimeStamp(this DateTime dateTime)
    {
        TimeSpan ts = dateTime - new DateTime(1970, 1, 1, 0, 0, 0, 0);
        return Convert.ToInt64(ts.TotalSeconds).ToString();
    }
}