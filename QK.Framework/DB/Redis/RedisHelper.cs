using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using QK.Framework.Core.Exceptions;
using QK.Framework.Core.Extensions;
using QK.Framework.Core.Ioc;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QK.Framework.DB.Redis
{
    public class RedisDBContext
    {

        private string redisConnection;
        private int DbIndex = 0;

        /// <summary>
        /// 设置Redis连接参数
        /// </summary>
        /// <param name="host"></param>
        /// <param name="user"></param>
        /// <param name="pwd"></param>
        public void SetInit(string connectionstring,int dbindex=0)
        {
            redisConnection = connectionstring;
            DbIndex = dbindex;
        }

        /// <summary>
        /// 配置文件
        /// </summary>
        public IConfiguration configuration
        {
            get
            {
                return IocContainer.Resolve<IConfiguration>();
            }
        }

        private ConnectionMultiplexer connectionMultiplexer = null;
        private IDatabase _DefaultDb = null;
        private object lockOjb = new object();

        public ConnectionMultiplexer Connection
        {
            get
            {
                if (connectionMultiplexer == null)
                {
                    lock (lockOjb)
                    {
                        if (connectionMultiplexer == null)
                        {

                            if (redisConnection.IsNullOrEmpty())
                                throw new QKException("请先初始化Redis配置");

                            connectionMultiplexer = ConnectionMultiplexer.Connect(redisConnection);                            
                        }
                    }
                }

                return connectionMultiplexer;
            }
        }


        public IDatabase Db
        {
            get
            {
                if (_DefaultDb == null)
                {
                    lock (lockOjb)
                    {
                        if (_DefaultDb == null)
                        {
                            _DefaultDb = Connection.GetDatabase(DbIndex);
                        }
                    }
                }
                return _DefaultDb;
            }
        }

        public async Task<bool> StringSetAsync(string key, object value, DateTime? expiry = null)
        {
            if (key.IsNullOrEmpty())
                return false;
            if (value == null)
                return false;
            return await StringSetAsync(key, JsonConvert.SerializeObject(value), expiry);
        }

        public async Task<bool> StringSetAsync(string key, string value, DateTime? expiry = null)
        {
            if (key.IsNullOrEmpty())
                throw new QKException("Redis Key 不能为空");
            if (value.IsNullOrEmpty())
                throw new QKException("Redis Value 不能为空");

            if (await Db.StringSetAsync(key, value))
            {
                if (expiry.HasValue)
                {
                    return await Db.KeyExpireAsync(key, expiry);
                }
                return true;
            }
            return false;
        }

        public async Task<bool> SetAddAsync(string key, string value, DateTime? expiry = null)
        {
            if (key.IsNullOrEmpty())
                throw new QKException("Redis Key 不能为空");
            if (value.IsNullOrEmpty())
                throw new QKException("Redis Value 不能为空");

            if (await Db.SetAddAsync(key, value))
            {
                if (expiry.HasValue)
                {
                    return await Db.KeyExpireAsync(key, expiry);
                }
                return true;
            }
            return false;
        }

        public async Task<string> StringGetAsync(string key)
        {
            if (key.IsNullOrEmpty())
                throw new QKException("Redis Key 不能为空");

            return await Db.StringGetAsync(key);
        }

        public async Task<T> GetAsync<T>(string key)
        {
            try
            {
                if (key.IsNullOrEmpty())
                    throw new QKException("Redis Key 不能为空");

                var json = await Db.StringGetAsync(key);
                if (json.IsNullOrEmpty)
                    return default(T);

                return JsonConvert.DeserializeObject<T>(json);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<List<string>> GetKeysAsync(string likeKey)
        {
            if (likeKey.IsNullOrEmpty())
                throw new QKException("Redis Key 不能为空");
            const string Script = "return redis.call('KEYS', @key)";
            var prepared = LuaScript.Prepare(Script);
            var result = (string[])await Db.ScriptEvaluateAsync(prepared, new { key = likeKey + "*" });

            return result.ToList();
        }

        public async Task<List<string>> MembersAsync(string key)
        {
            if (key.IsNullOrEmpty())
                throw new QKException("Redis Key 不能为空");

            var list = await Db.SetMembersAsync(key);
            return list.Select(m => m.ToString()).ToList();
        }

        public async Task RemoveAsync(string key)
        {
            if (key.IsNullOrEmpty())
                throw new QKException("Redis Key 不能为空");

            await Db.KeyDeleteAsync(key);
        }
    }
}
