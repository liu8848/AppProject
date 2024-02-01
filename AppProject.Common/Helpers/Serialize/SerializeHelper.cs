using System.Text;
using Newtonsoft.Json;
using JsonConverter = System.Text.Json.Serialization.JsonConverter;

namespace AppProject.Common.Helpers.Serialize;

public class SerializeHelper
{
    /// <summary>
    /// 序列化对象
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public static byte[] Serialize(object item)
    {
        var jsonString = JsonConvert.SerializeObject(item);
        return Encoding.UTF8.GetBytes(jsonString);
    }

    /// <summary>
    /// 反序列化对象
    /// </summary>
    /// <param name="value"></param>
    /// <typeparam name="TEntity"></typeparam>
    /// <returns></returns>
    public static TEntity? Deserialize<TEntity>(byte[]? value)
    {
        if (value == null)
        {
            return default(TEntity);
        }

        var jsonString = Encoding.UTF8.GetString(value);
        return JsonConvert.DeserializeObject<TEntity>(jsonString);
    }


    /// <summary>
    /// Json化对象
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public static string ObjectParseJsonStr(object item)
    {
        return JsonConvert.SerializeObject(item);
    }

    public static TEntity? JsonStrParseObject<TEntity>(string jsonStr)
    {
        return JsonConvert.DeserializeObject<TEntity>(jsonStr);
    }
}