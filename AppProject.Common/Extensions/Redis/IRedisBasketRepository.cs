using System.Net;
using StackExchange.Redis;

namespace AppProject.Common.Extensions.Redis;

public interface IRedisBasketRepository
{
    Task Clear();
    Task Clear(EndPoint endPoint);

    #region 字符串
    Task<bool> Exist(string key);

    Task<bool> Remove(string key);

    Task<string?> GetValue(string key);

    Task Set(string key, object value, TimeSpan cacheTime);

    Task<TEntity?> Get<TEntity>(string key);
    #endregion

    #region 列表
    Task<RedisValue[]> ListRangeAsync(string key);
    Task<long> ListRightPushAsync(string key, object value, int db = -1);
    Task<long> ListRightPushAsync(string key, IEnumerable<object> values, int db = -1);
    Task<TEntity?> ListLeftPopAsync<TEntity>(string key, int db = -1) where TEntity : class;
    Task<string?> ListLeftPopAsync(string key, int db = -1);
    Task<TEntity?> ListRightPopAsync<TEntity>(string key, int db = -1) where TEntity : class;
    Task<string?> ListRightPopAsync(string key, int db = -1);
    Task<long> ListLengthAsync(string key, int db = -1);
    Task<IEnumerable<string>> ListRangeAsync(string key, int db = -1);
    Task<IEnumerable<TEntity?>> ListRangeAsync<TEntity>(string key, int db = -1) where TEntity : class;
    Task<IEnumerable<string>> ListRangeAsync(string key, int start, int end, int db = -1);
    Task<IEnumerable<TEntity?>> ListRangeAsync<TEntity>(string key, int start, int end, int db = -1);
    Task<long> ListDelRangeAsync(string redisKey, string redisValue, long type = 0, int db = -1);
    Task ListClearAsync(string key, int db = -1);
    #endregion

    #region 哈希表

    Task<bool> HashKeyExists(string redisKey, string hashField, int db=-1);
    Task HashSet(string redisKey, string hashField, object value, int db = -1);
    Task<string?> HashGet(string redisKey, string hashField, int db = -1);
    Task<TEntity?> HashGet<TEntity>(string redisKey, string hashFiled, int db = -1);
    Task HashSet(string redisKey, HashEntry[] hashEntries, int db = -1);
    Task HashPutAll(string redisKey, Dictionary<string, string> dic, int db = -1);
    Task HashPutAll(string redisKey, Dictionary<string, object> dic, int db = -1);
    Task<Dictionary<string, TEntity>> HashGetAll<TEntity>(string redisKey, int db = -1);
    Task<bool> HashDelete(string redisKey, string hashField, int db = -1);
    Task<RedisValue[]> HashKeys(string redisKey, int db = -1);
    Task<RedisValue[]> HashValues(string redisKey, int db = -1);
    Task<long> HashSize(string redisKey, int db = -1);


    #endregion
}