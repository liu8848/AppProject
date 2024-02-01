using System.Net;
using AppProject.Common.Helpers.Serialize;
using Newtonsoft.Json;
using StackExchange.Redis;

namespace AppProject.Common.Extensions.Redis;

public class RedisBasketRepository:IRedisBasketRepository
{
    private readonly ConnectionMultiplexer _redis;
    private readonly IDatabase _database;

    public RedisBasketRepository(ConnectionMultiplexer redis)
    {
        _redis = redis;
        _database = redis.GetDatabase();
    }

    /// <summary>
    /// 默认获取第一个服务器
    /// </summary>
    /// <returns></returns>
    private IServer GetServer()
    {
        var endPoints = _redis.GetEndPoints();
        return _redis.GetServer(endPoints.First());
    }
    /// <summary>
    /// 获取指定redis服务器
    /// </summary>
    /// <param name="endPoint"></param>
    /// <returns></returns>
    private IServer GetServer(EndPoint endPoint)
    {
        return _redis.GetServer(endPoint);
    }

    /// <summary>
    /// 移除集群中缓存的所有key
    /// </summary>
    public async Task Clear()
    {
        foreach (var endPoint in _redis.GetEndPoints())
        {
            await Clear(endPoint);
        }
    }
    /// <summary>
    /// 清楚指定服务器中的key
    /// </summary>
    /// <param name="endPoint"></param>
    public async Task Clear(EndPoint endPoint)
    {
        var server = GetServer(endPoint);
        foreach (var key in server.Keys())
        {
            await _database.KeyDeleteAsync(key);
        }
    }

    /// <summary>
    /// 判断key是否存在
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public async Task<bool> Exist(string key)
    {
        return await _database.KeyExistsAsync(key);
    }

    /// <summary>
    /// 移除指定key
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public async Task<bool> Remove(string key)
    {
        return await _database.KeyDeleteAsync(key);
    }

    #region 字符串
    /// <summary>
    /// 获取指定key内容
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public async Task<string?> GetValue(string key)
    {
        return await _database.StringGetAsync(key);
    }

    /// <summary>
    /// 存储key-value
    /// </summary>
    /// <param name="key"></param>
    /// <param name="value"></param>
    /// <param name="cacheTime">过期时间</param>
    public async Task Set(string key, object? value, TimeSpan cacheTime)
    {
        if (value is not null)
        {
            if (value is string cacheValue)
            {
                //字符串无需序列化
                await _database.StringSetAsync(key, cacheValue, cacheTime);
            }
            else
            {
                //序列化,将object值生成RedisValue
                await _database.StringSetAsync(key,
                    SerializeHelper.Serialize(value),
                    cacheTime);
            }
        }
    }

    /// <summary>
    /// 获取对应key的内容
    /// </summary>
    /// <param name="key"></param>
    /// <typeparam name="TEntity"></typeparam>
    /// <returns></returns>
    public async Task<TEntity?> Get<TEntity>(string key)
    {
        var value = await _database.StringGetAsync(key);
        if (value.HasValue)
        {
            return SerializeHelper.Deserialize<TEntity>(value);
        }
        return default(TEntity);
    }
    #endregion

    #region 列表
    /// <summary>
    /// 根据key获取redis列表
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public async Task<RedisValue[]> ListRangeAsync(string key)
    {
        return await _database.ListRangeAsync(key);
    }
    /// <summary>
    /// 在列表尾部插入元素，若键不存在，先创建再插入
    /// </summary>
    /// <param name="key"></param>
    /// <param name="value"></param>
    /// <param name="db"></param>
    /// <returns></returns>
    public async Task<long> ListRightPushAsync(string key, object value, int db = -1)
    {
        if (value is not string valueStr)
        {
            valueStr = SerializeHelper.ObjectParseJsonStr(value);
        }
        return await _database.ListRightPushAsync(key, valueStr);
    }
    /// <summary>
    /// 在列表尾部插入数组集合
    /// </summary>
    /// <param name="key"></param>
    /// <param name="values"></param>
    /// <param name="db"></param>
    /// <returns></returns>
    public async Task<long> ListRightPushAsync(string key, IEnumerable<object> values, int db = -1)
    {
        var redisValues = new List<RedisValue>();
        foreach (var item in values)
        {
            if (item is not string itemStr)
            {
                itemStr = SerializeHelper.ObjectParseJsonStr(item);
            }
            redisValues.Add(itemStr);
        }
        return await _database.ListRightPushAsync(key, redisValues.ToArray());
    }
    public async Task<long> ListRightPushAsync(string key, IEnumerable<byte[]> values, int db = -1)
    {
        var redisValues = new List<RedisValue>();
        foreach (var item in values)
        {
            redisValues.Add(item);
        }
        return await _database.ListRightPushAsync(key, redisValues.ToArray());
    }
    

    /// <summary>
    /// 移除并返回列表第一个元素     反序列化
    /// </summary>
    /// <param name="key"></param>
    /// <param name="db"></param>
    /// <typeparam name="TEntity"></typeparam>
    /// <returns></returns>
    public async Task<TEntity?> ListLeftPopAsync<TEntity>(string key, int db = -1) where TEntity:class
    {
        var value = await _database.ListLeftPopAsync(key);
        return value.HasValue ? SerializeHelper.JsonStrParseObject<TEntity>(value) : default(TEntity);
    }
    /// <summary>
    /// 移除并返回列表第一个元素    
    /// </summary>
    /// <param name="key"></param>
    /// <param name="db"></param>
    /// <returns></returns>
    public async Task<string?> ListLeftPopAsync(string key, int db = -1)
    {
        return await _database.ListLeftPopAsync(key);
    }

    /// <summary>
    /// 移除并返回列表最后一个元素 反序列化
    /// </summary>
    /// <param name="key"></param>
    /// <param name="db"></param>
    /// <typeparam name="TEntity"></typeparam>
    /// <returns></returns>
    public async Task<TEntity?> ListRightPopAsync<TEntity>(string key, int db = -1) where TEntity : class
    {
        var value = await _database.ListRightPopAsync(key);
        return value.HasValue ? SerializeHelper.JsonStrParseObject<TEntity>(value) : default(TEntity);
    } 
    /// <summary>
    /// 移除并返回列表最后一个元素    
    /// </summary>
    /// <param name="key"></param>
    /// <param name="db"></param>
    /// <returns></returns>
    public async Task<string?> ListRightPopAsync(string key, int db = -1)
    {
        return await _database.ListRightPopAsync(key);
    }
    /// <summary>
    /// 获取列表长度
    /// </summary>
    /// <param name="key"></param>
    /// <param name="db"></param>
    /// <returns></returns>
    public async Task<long> ListLengthAsync(string key, int db = -1)
    {
        return await _database.ListLengthAsync(key);
    }
    /// <summary>
    /// 返回列表对应元素
    /// </summary>
    /// <param name="key"></param>
    /// <param name="db"></param>
    /// <returns></returns>
    public async Task<IEnumerable<string>> ListRangeAsync(string key, int db = -1)
    {
        var result = await _database.ListRangeAsync(key);
        return result.Select(o => o.ToString());
    }
    /// <summary>
    /// 返回列表对应元素   反序列化
    /// </summary>
    /// <param name="key"></param>
    /// <param name="db"></param>
    /// <typeparam name="TEntity"></typeparam>
    /// <returns></returns>
    public async Task<IEnumerable<TEntity?>> ListRangeAsync<TEntity>(string key, int db = -1) where TEntity : class
    {
        var result = await _database.ListRangeAsync(key);
        
        return result.Select(o => SerializeHelper.JsonStrParseObject<TEntity>(o));
    }
    /// <summary>
    /// 返回列表指定索引数据
    /// </summary>
    /// <param name="key"></param>
    /// <param name="start"></param>
    /// <param name="end"></param>
    /// <param name="db"></param>
    /// <returns></returns>
    public async Task<IEnumerable<string>> ListRangeAsync(string key, int start, int end, int db = -1)
    {
        var result = await _database.ListRangeAsync(key,start,end);
        return result.Select(o => o.ToString());
    }

    /// <summary>
    /// 返回列表指定索引数据     反序列化
    /// </summary>
    /// <param name="key"></param>
    /// <param name="start"></param>
    /// <param name="end"></param>
    /// <param name="db"></param>
    /// <typeparam name="TEntity"></typeparam>
    /// <returns></returns>
    public async Task<IEnumerable<TEntity?>> ListRangeAsync<TEntity>(string key, int start, int end, int db = -1)
    {
        var result = await _database.ListRangeAsync(key,start,end);
        return result.Select(o => SerializeHelper.JsonStrParseObject<TEntity>(o));
    }
    
    /// <summary>
    /// 删除List中的元素 并返回删除的个数
    /// </summary>
    /// <param name="redisKey">key</param>
    /// <param name="redisValue">元素</param>
    /// <param name="type">大于零 : 从表头开始向表尾搜索，小于零 : 从表尾开始向表头搜索，等于零：移除表中所有与 VALUE 相等的值</param>
    /// <param name="db"></param>
    /// <returns></returns>
    public async Task<long> ListDelRangeAsync(string redisKey, string redisValue, long type = 0, int db = -1)
    {
        return await _database.ListRemoveAsync(redisKey, redisValue, type);
    }

    /// <summary>
    /// 清空List
    /// </summary>
    /// <param name="key"></param>
    /// <param name="db"></param>
    public async Task ListClearAsync(string key, int db = -1)
    {
        await _database.ListTrimAsync(key, 1, 0);
    }

    #endregion

    #region 哈希表

    /// <summary>
    /// 哈希字段是否存在
    /// </summary>
    /// <param name="redisKey"></param>
    /// <param name="hashField"></param>
    /// <param name="db"></param>
    /// <returns></returns>
    public async Task<bool> HashKeyExists(string redisKey, string hashField, int db = -1)
    {
        return await _database.HashExistsAsync(redisKey, hashField);
    }
    /// <summary>
    /// 设置哈希表字段的字符串值
    /// </summary>
    /// <param name="redisKey"></param>
    /// <param name="hashField"></param>
    /// <param name="value"></param>
    /// <param name="db"></param>
    public async Task HashSet(string redisKey, string hashField, object value, int db = -1)
    {
        if (!await HashKeyExists(redisKey, hashField, db))
        {
            if (value is not string valueStr)
            {
                valueStr = SerializeHelper.ObjectParseJsonStr(value);
            }
            await _database.HashSetAsync(redisKey, hashField, valueStr);
        }
    }
    /// <summary>
    /// 获取指定键的哈希字段值
    /// </summary>
    /// <param name="redisKey"></param>
    /// <param name="hashField"></param>
    /// <param name="db"></param>
    /// <returns></returns>
    public async Task<string?> HashGet(string redisKey, string hashField, int db = -1)
    {
        return await _database.HashGetAsync(redisKey, hashField);
    }
    /// <summary>
    /// 获取指定键的哈希字段值   反序列化
    /// </summary>
    /// <param name="redisKey"></param>
    /// <param name="hashFiled"></param>
    /// <param name="db"></param>
    /// <typeparam name="TEntity"></typeparam>
    /// <returns></returns>
    public async Task<TEntity?> HashGet<TEntity>(string redisKey, string hashFiled, int db = -1)
    {
        var result = await _database.HashGetAsync(redisKey,hashFiled);
        return result.HasValue ? SerializeHelper.JsonStrParseObject<TEntity>(result) : default(TEntity);
    }
    /// <summary>
    /// 设置哈希集合
    /// </summary>
    /// <param name="redisKey"></param>
    /// <param name="hashEntries"></param>
    /// <param name="db"></param>
    public async Task HashSet(string redisKey, HashEntry[] hashEntries, int db = -1)
    {
        await _database.HashSetAsync(redisKey, hashEntries);
    }
    /// <summary>
    /// 存入Dictionary
    /// </summary>
    /// <param name="redisKey"></param>
    /// <param name="dic"></param>
    /// <param name="db"></param>
    public async Task HashPutAll(string redisKey, Dictionary<string, string> dic, int db = -1)
    {
        var entries = new List<HashEntry>();
        foreach (var (key,value) in dic)
        {
            entries.Add(new HashEntry(key,value));
        }

        await HashSet(redisKey, entries.ToArray());
    }
    public async Task HashPutAll(string redisKey, Dictionary<string, object> dic, int db = -1)
    {
        var entries = new List<HashEntry>();
        foreach (var (key,value) in dic)
        {
            if (value is not string valueStr)
            {
                valueStr = SerializeHelper.ObjectParseJsonStr(value);
            }
            entries.Add(new HashEntry(key,valueStr));
        }

        await HashSet(redisKey, entries.ToArray());
    }
    /// <summary>
    /// 获取指定键所有哈希字段和值
    /// </summary>
    /// <param name="redisKey"></param>
    /// <param name="db"></param>
    /// <typeparam name="TEntity"></typeparam>
    /// <returns></returns>
    public async Task<Dictionary<string, string>> HashGetAll(string redisKey, int db = -1)
    {
        var results = await _database.HashGetAllAsync(redisKey);
        var dic = new Dictionary<string,string>();
        foreach (var entry in results)
        {
            dic.Add(entry.Name,entry.Value);
        }

        return dic;
    }
    public async Task<Dictionary<string, TEntity>> HashGetAll<TEntity>(string redisKey, int db = -1)
    {
        var results = await _database.HashGetAllAsync(redisKey);
        var dic = new Dictionary<string,TEntity>();
        foreach (var entry in results)
        {
            dic.Add(entry.Name,SerializeHelper.JsonStrParseObject<TEntity>(entry.Value));
        }

        return dic;
    }
    /// <summary>
    /// 删除指定键的指定哈希字段
    /// </summary>
    /// <param name="redisKey"></param>
    /// <param name="hashField"></param>
    /// <param name="db"></param>
    /// <returns></returns>
    public async Task<bool> HashDelete(string redisKey, string hashField, int db = -1)
    {
        return await _database.HashDeleteAsync(redisKey, hashField);
    }
    /// <summary>
    /// 获取哈希中的所有键
    /// </summary>
    /// <param name="redisKey"></param>
    /// <param name="db"></param>
    /// <returns></returns>
    public async Task<RedisValue[]> HashKeys(string redisKey, int db = -1)
    {
        return await _database.HashKeysAsync(redisKey);
    }
    /// <summary>
    /// 获取哈希中的所有值
    /// </summary>
    /// <param name="redisKey"></param>
    /// <param name="db"></param>
    /// <returns></returns>
    public async Task<RedisValue[]> HashValues(string redisKey, int db = -1)
    {
        return await _database.HashValuesAsync(redisKey);
    }
    /// <summary>
    /// 获取哈希表的长度
    /// </summary>
    /// <param name="redisKey"></param>
    /// <param name="db"></param>
    /// <returns></returns>
    public async Task<long> HashSize(string redisKey, int db = -1)
    {
        return await _database.HashLengthAsync(redisKey);
    }

    #endregion
}