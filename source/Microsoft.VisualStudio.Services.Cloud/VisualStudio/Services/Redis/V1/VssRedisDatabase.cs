// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Redis.V1.VssRedisDatabase
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Redis.V1
{
  internal class VssRedisDatabase : IRedisDatabase, IRedisDatabaseInternal
  {
    private readonly IDatabase m_redisDb;
    private readonly IResourceManager m_resourceManager;
    private readonly int m_maxMessageSize;
    private readonly bool m_enforceMessageSize;
    private readonly double m_radixBackoff;
    private const string c_luaScriptExtension = ".lua";
    private const string c_area = "Redis";
    private const string c_layer = "VssRedisDatabase";

    public VssRedisDatabase(IDatabase redisDb, int maxMessageSize = 0, double radixBackoff = 1.0)
      : this(redisDb, maxMessageSize, radixBackoff, (IResourceManager) ResourceManager.Instance)
    {
    }

    internal VssRedisDatabase(
      IDatabase redisDb,
      int maxMessageSize,
      double radixBackoff,
      IResourceManager resourceManager)
    {
      this.m_redisDb = redisDb;
      this.m_enforceMessageSize = maxMessageSize > 0;
      this.m_maxMessageSize = Math.Abs(maxMessageSize);
      this.m_radixBackoff = Math.Max(radixBackoff, 1.0);
      this.m_resourceManager = resourceManager;
    }

    public IDatabase RedisDatabase => this.m_redisDb;

    public RedisResult ScriptEvaluate(
      IVssRequestContext requestContext,
      string scriptName,
      RedisKey[] keys,
      RedisValue[] values)
    {
      this.ValidateMessageSize(requestContext, (IList<RedisKey>) keys, (IList<RedisValue>) values, callerName: nameof (ScriptEvaluate));
      string str = this.m_resourceManager.LoadLuaScript(requestContext, this.EnsureScriptExtension(scriptName));
      if (string.IsNullOrEmpty(str))
        throw new FileNotFoundException("Lua script with name " + scriptName + " was not found");
      return this.RedisDatabase.ScriptEvaluate(str, keys, values, (CommandFlags) 0);
    }

    public Task<RedisResult> ScriptEvaluateAsync(
      IVssRequestContext requestContext,
      string scriptName,
      RedisKey[] keys,
      RedisValue[] values)
    {
      this.ValidateMessageSize(requestContext, (IList<RedisKey>) keys, (IList<RedisValue>) values, callerName: nameof (ScriptEvaluateAsync));
      string str = this.m_resourceManager.LoadLuaScript(requestContext, this.EnsureScriptExtension(scriptName));
      if (string.IsNullOrEmpty(str))
        throw new FileNotFoundException("Lua script with name " + scriptName + " was not found");
      return ((IDatabaseAsync) this.RedisDatabase).ScriptEvaluateAsync(str, keys, values, (CommandFlags) 0);
    }

    public RedisValue[] GetVolatileValue(
      IVssRequestContext requestContext,
      RedisKey[] items,
      bool allowBatching)
    {
      if (items == null || items.Length == 0)
        return Array.Empty<RedisValue>();
      if (((items.Length <= 1 ? 0 : (this.m_maxMessageSize > 0 ? 1 : 0)) & (allowBatching ? 1 : 0)) != 0)
      {
        string str = this.m_resourceManager.LoadLuaScript(requestContext, "GetVolatileValue.lua");
        RedisValue[] destinationArray1 = (RedisValue[]) null;
        int destinationIndex = 0;
        int totalBatches = 0;
        int totalSize = 0;
        int totalRequests = 0;
        TimeSpan zero = TimeSpan.Zero;
        RedisKey[] destinationArray2;
        foreach (IEnumerable<RedisKey> source in this.Batch<RedisKey>(requestContext, (IEnumerable<RedisKey>) items, (Func<RedisKey, int>) (x => x.GetSize()), allowBatching))
        {
          for (RedisKey[] redisKeyArray = source.ToArray<RedisKey>(); redisKeyArray.Length != 0; redisKeyArray = destinationArray2)
          {
            this.ValidateMessageSize(requestContext, (IList<RedisKey>) redisKeyArray, (IList<RedisValue>) null, callerName: nameof (GetVolatileValue));
            zero += this.Backoff(requestContext, totalRequests++);
            RedisValue[] redisValueArray = RedisResult.op_Explicit(this.m_redisDb.ScriptEvaluate(str, redisKeyArray, new RedisValue[1]
            {
              RedisValue.op_Implicit(this.m_maxMessageSize)
            }, (CommandFlags) 0));
            ++totalBatches;
            totalSize += this.ValidateMessageSize(requestContext, (IList<RedisKey>) null, (IList<RedisValue>) redisValueArray, new RedisKey?(((IEnumerable<RedisKey>) redisKeyArray).FirstOrDefault<RedisKey>()), false, nameof (GetVolatileValue));
            if (redisValueArray.Length == items.Length)
            {
              destinationArray1 = redisValueArray;
              break;
            }
            destinationArray1 = destinationArray1 ?? new RedisValue[items.Length];
            Array.Copy((Array) redisValueArray, 0, (Array) destinationArray1, destinationIndex, redisValueArray.Length);
            destinationIndex += redisValueArray.Length;
            destinationArray2 = new RedisKey[redisKeyArray.Length - redisValueArray.Length];
            Array.Copy((Array) redisKeyArray, redisValueArray.Length, (Array) destinationArray2, 0, redisKeyArray.Length - redisValueArray.Length);
          }
        }
        this.TraceBatchCompleted(requestContext, totalBatches, items.Length, items.Length, totalSize, totalRequests, zero, nameof (GetVolatileValue));
        return destinationArray1;
      }
      this.ValidateMessageSize(requestContext, (IList<RedisKey>) items, (IList<RedisValue>) null, callerName: nameof (GetVolatileValue));
      RedisValue[] values = this.m_redisDb.StringGet(items, (CommandFlags) 0);
      int totalSize1 = this.ValidateMessageSize(requestContext, (IList<RedisKey>) null, (IList<RedisValue>) values, items != null ? new RedisKey?(((IEnumerable<RedisKey>) items).FirstOrDefault<RedisKey>()) : new RedisKey?(), false, nameof (GetVolatileValue));
      this.TraceBatchCompleted(requestContext, 1, items.Length, items.Length, totalSize1, 1, TimeSpan.Zero, nameof (GetVolatileValue));
      return values;
    }

    public Task<RedisValue[]> GetListValueAsync(IVssRequestContext requestContext, RedisKey key) => ((IDatabaseAsync) this.RedisDatabase).ListRangeAsync(key, 0L, -1L, (CommandFlags) 0);

    public bool SetVolatileValue(
      IVssRequestContext requestContext,
      IEnumerable<SetVolatileValueParameter> items,
      bool allowBatching)
    {
      string str = this.m_resourceManager.LoadLuaScript(requestContext, "SetVolatileValue.lua");
      int totalBatches = 0;
      int totalKeys = 0;
      int totalValues = 0;
      int totalSize = 0;
      int totalRequests = 0;
      TimeSpan zero = TimeSpan.Zero;
      Func<SetVolatileValueParameter, int> getSize = (Func<SetVolatileValueParameter, int>) (x => x.Key.GetSize() + x.Value.GetSize() + 8);
      foreach (IList<SetVolatileValueParameter> volatileValueParameterList in this.Batch<SetVolatileValueParameter>(requestContext, items, getSize, allowBatching))
      {
        RedisKey[] redisKeyArray1 = new RedisKey[volatileValueParameterList.Count];
        RedisValue[] redisValueArray1 = new RedisValue[volatileValueParameterList.Count * 2];
        IEnumerator<SetVolatileValueParameter> enumerator = volatileValueParameterList.GetEnumerator();
        int num1 = 0;
        while (enumerator.MoveNext())
        {
          SetVolatileValueParameter current = enumerator.Current;
          TimeSpan? expiry = current.Expiry;
          if (expiry.HasValue)
          {
            current = enumerator.Current;
            expiry = current.Expiry;
            if (expiry.Value < TimeSpan.Zero)
              throw new ArgumentException("Expiry must be non-negative value");
          }
          RedisKey[] redisKeyArray2 = redisKeyArray1;
          int index1 = num1;
          current = enumerator.Current;
          RedisKey key = current.Key;
          redisKeyArray2[index1] = key;
          int index2 = num1 * 2;
          current = enumerator.Current;
          RedisValue redisValue1 = current.Value;
          if (((RedisValue) ref redisValue1).IsNull)
          {
            redisValueArray1[index2] = RedisValue.EmptyString;
            redisValueArray1[index2 + 1] = RedisValue.op_Implicit(-2L);
          }
          else
          {
            RedisValue[] redisValueArray2 = redisValueArray1;
            int index3 = index2;
            current = enumerator.Current;
            RedisValue redisValue2 = current.Value;
            redisValueArray2[index3] = redisValue2;
            RedisValue[] redisValueArray3 = redisValueArray1;
            int index4 = index2 + 1;
            current = enumerator.Current;
            expiry = current.Expiry;
            long num2;
            if (!expiry.HasValue)
            {
              num2 = -1L;
            }
            else
            {
              current = enumerator.Current;
              expiry = current.Expiry;
              num2 = (long) expiry.Value.TotalMilliseconds;
            }
            RedisValue redisValue3 = RedisValue.op_Implicit(num2);
            redisValueArray3[index4] = redisValue3;
          }
          ++num1;
        }
        ++totalBatches;
        totalKeys += redisKeyArray1.Length;
        totalValues += redisValueArray1.Length;
        totalSize += this.ValidateMessageSize(requestContext, (IList<RedisKey>) redisKeyArray1, (IList<RedisValue>) redisValueArray1, callerName: nameof (SetVolatileValue));
        zero += this.Backoff(requestContext, totalRequests++);
        this.m_redisDb.ScriptEvaluate(str, ((IEnumerable<RedisKey>) redisKeyArray1).ToArray<RedisKey>(), ((IEnumerable<RedisValue>) redisValueArray1).ToArray<RedisValue>(), (CommandFlags) 0);
      }
      this.TraceBatchCompleted(requestContext, totalBatches, totalKeys, totalValues, totalSize, totalRequests, zero, nameof (SetVolatileValue));
      return true;
    }

    public IEnumerable<long> IncrementVolatileValue(
      IVssRequestContext requestContext,
      IEnumerable<IncrementVolatileValueParameter> items,
      bool allowBatching)
    {
      string str = this.m_resourceManager.LoadLuaScript(requestContext, "IncrementVolatileValue.lua");
      List<long> longList = new List<long>();
      int totalBatches = 0;
      int totalKeys = 0;
      int totalValues = 0;
      int totalSize = 0;
      int totalRequests = 0;
      TimeSpan zero = TimeSpan.Zero;
      Func<IncrementVolatileValueParameter, int> getSize = (Func<IncrementVolatileValueParameter, int>) (x => x.Key.GetSize() + 8 + 8);
      foreach (IList<IncrementVolatileValueParameter> volatileValueParameterList in this.Batch<IncrementVolatileValueParameter>(requestContext, items, getSize, allowBatching))
      {
        RedisKey[] keys = new RedisKey[volatileValueParameterList.Count];
        RedisValue[] values = new RedisValue[volatileValueParameterList.Count * 2];
        IEnumerator<IncrementVolatileValueParameter> enumerator = volatileValueParameterList.GetEnumerator();
        int num1 = 0;
        while (enumerator.MoveNext())
        {
          IncrementVolatileValueParameter current = enumerator.Current;
          TimeSpan? expiry = current.Expiry;
          if (expiry.HasValue)
          {
            current = enumerator.Current;
            expiry = current.Expiry;
            if (expiry.Value < TimeSpan.Zero)
              throw new ArgumentException("Expiry must be non-negative value");
          }
          RedisKey[] redisKeyArray = keys;
          int index1 = num1;
          current = enumerator.Current;
          RedisKey key = current.Key;
          redisKeyArray[index1] = key;
          int num2 = num1 * 2;
          RedisValue[] redisValueArray1 = values;
          int index2 = num2;
          current = enumerator.Current;
          RedisValue redisValue1 = RedisValue.op_Implicit(current.Value);
          redisValueArray1[index2] = redisValue1;
          RedisValue[] redisValueArray2 = values;
          int index3 = num2 + 1;
          current = enumerator.Current;
          expiry = current.Expiry;
          long num3;
          if (!expiry.HasValue)
          {
            num3 = -1L;
          }
          else
          {
            current = enumerator.Current;
            expiry = current.Expiry;
            num3 = (long) expiry.Value.TotalMilliseconds;
          }
          RedisValue redisValue2 = RedisValue.op_Implicit(num3);
          redisValueArray2[index3] = redisValue2;
          ++num1;
        }
        ++totalBatches;
        totalKeys += keys.Length;
        totalValues += values.Length;
        totalSize += this.ValidateMessageSize(requestContext, (IList<RedisKey>) keys, (IList<RedisValue>) values, callerName: nameof (IncrementVolatileValue));
        zero += this.Backoff(requestContext, totalRequests++);
        RedisValue[] source = RedisResult.op_Explicit(this.m_redisDb.ScriptEvaluate(str, keys, values, (CommandFlags) 0));
        longList.AddRange(((IEnumerable<RedisValue>) source).Select<RedisValue, long>((Func<RedisValue, long>) (x => RedisValue.op_Explicit(x))));
      }
      this.TraceBatchCompleted(requestContext, totalBatches, totalKeys, totalValues, totalSize, totalRequests, zero, nameof (IncrementVolatileValue));
      return (IEnumerable<long>) longList;
    }

    public long DeleteVolatileValue(
      IVssRequestContext requestContext,
      RedisKey[] items,
      bool allowBatching)
    {
      long num1 = 0;
      int totalBatches = 0;
      int totalKeys = 0;
      int totalSize = 0;
      int totalRequests = 0;
      TimeSpan zero = TimeSpan.Zero;
      foreach (IList<RedisKey> redisKeyList in this.Batch<RedisKey>(requestContext, (IEnumerable<RedisKey>) items, (Func<RedisKey, int>) (x => x.GetSize()), allowBatching))
      {
        int num2 = this.ValidateMessageSize(requestContext, redisKeyList, (IList<RedisValue>) null, callerName: nameof (DeleteVolatileValue));
        zero += this.Backoff(requestContext, totalRequests++);
        num1 += this.m_redisDb.KeyDelete(redisKeyList.ToArray<RedisKey>(), (CommandFlags) 0);
        ++totalBatches;
        totalKeys += redisKeyList.Count;
        totalSize += num2;
      }
      this.TraceBatchCompleted(requestContext, totalBatches, totalKeys, 0, totalSize, totalRequests, zero, nameof (DeleteVolatileValue));
      return num1;
    }

    public async Task<long> DeleteVolatileValueAsync(
      IVssRequestContext requestContext,
      RedisKey[] items,
      bool allowBatching)
    {
      long result = 0;
      int totalBatches = 0;
      int totalKeys = 0;
      int totalSize = 0;
      int totalRequests = 0;
      TimeSpan totalBackoff = TimeSpan.Zero;
      foreach (IList<RedisKey> batch in this.Batch<RedisKey>(requestContext, (IEnumerable<RedisKey>) items, (Func<RedisKey, int>) (x => x.GetSize()), allowBatching))
      {
        int batchSize = this.ValidateMessageSize(requestContext, batch, (IList<RedisValue>) null, callerName: nameof (DeleteVolatileValueAsync));
        TimeSpan timeSpan = totalBackoff;
        totalBackoff = timeSpan + await this.BackoffAsync(requestContext, totalRequests++);
        timeSpan = new TimeSpan();
        long num = result;
        result = num + await ((IDatabaseAsync) this.m_redisDb).KeyDeleteAsync(batch.ToArray<RedisKey>(), (CommandFlags) 0);
        ++totalBatches;
        totalKeys += batch.Count;
        totalSize += batchSize;
      }
      this.TraceBatchCompleted(requestContext, totalBatches, totalKeys, 0, totalSize, totalRequests, totalBackoff, nameof (DeleteVolatileValueAsync));
      return result;
    }

    public TimeSpan?[] GetTimeToLiveVolatileValue(
      IVssRequestContext requestContext,
      RedisKey[] items)
    {
      if (items == null || items.Length == 0)
        return new TimeSpan?[0];
      string str = this.m_resourceManager.LoadLuaScript(requestContext, "GetVolatileTtl.lua");
      this.ValidateMessageSize(requestContext, (IList<RedisKey>) items, (IList<RedisValue>) null, callerName: nameof (GetTimeToLiveVolatileValue));
      RedisValue[] array = RedisResult.op_Explicit(this.m_redisDb.ScriptEvaluate(str, items, (RedisValue[]) null, (CommandFlags) 0));
      Func<long, TimeSpan?> longToTimespan = (Func<long, TimeSpan?>) (x => x >= 0L ? new TimeSpan?(TimeSpan.FromMilliseconds((double) x)) : new TimeSpan?());
      Converter<RedisValue, TimeSpan?> converter = (Converter<RedisValue, TimeSpan?>) (item => longToTimespan(RedisValue.op_Explicit(item)));
      return Array.ConvertAll<RedisValue, TimeSpan?>(array, converter);
    }

    public IEnumerable<long> GetWindowedValue(
      IVssRequestContext requestContext,
      IEnumerable<RedisKey> items,
      TimeSpan windowDuration)
    {
      int length = items.Count<RedisKey>();
      if (items == null || length <= 0)
        return (IEnumerable<long>) Array.Empty<long>();
      string str = this.m_resourceManager.LoadLuaScript(requestContext, "GetWindowedValue.lua");
      RedisKey[] keys = new RedisKey[length];
      RedisValue[] values = new RedisValue[1];
      IEnumerator<RedisKey> enumerator = items.GetEnumerator();
      int index = 0;
      while (enumerator.MoveNext())
      {
        keys[index] = enumerator.Current;
        ++index;
      }
      values[0] = RedisValue.op_Implicit((long) windowDuration.TotalMilliseconds);
      this.ValidateMessageSize(requestContext, (IList<RedisKey>) keys, (IList<RedisValue>) values, callerName: nameof (GetWindowedValue));
      return ((IEnumerable<RedisValue>) RedisResult.op_Explicit(this.m_redisDb.ScriptEvaluate(str, keys, values, (CommandFlags) 0))).Select<RedisValue, long>((Func<RedisValue, long>) (x => RedisValue.op_Explicit(x)));
    }

    public IEnumerable<long> IncrementWindowedValue(
      IVssRequestContext requestContext,
      IEnumerable<IncrementWindowedValueParameter> items,
      TimeSpan windowDuration,
      bool allowBatching)
    {
      string str = this.m_resourceManager.LoadLuaScript(requestContext, "IncrementWindowedValue.lua");
      List<long> longList = new List<long>();
      int totalBatches = 0;
      int totalKeys = 0;
      int totalValues = 0;
      int totalSize = 0;
      int totalRequests = 0;
      TimeSpan zero = TimeSpan.Zero;
      Func<IncrementWindowedValueParameter, int> getSize = (Func<IncrementWindowedValueParameter, int>) (x => x.Key.GetSize() + 8 + 8);
      foreach (IList<IncrementWindowedValueParameter> windowedValueParameterList in this.Batch<IncrementWindowedValueParameter>(requestContext, items, getSize, allowBatching))
      {
        RedisKey[] keys = new RedisKey[windowedValueParameterList.Count];
        RedisValue[] values = new RedisValue[windowedValueParameterList.Count * 2 + 1];
        IEnumerator<IncrementWindowedValueParameter> enumerator = windowedValueParameterList.GetEnumerator();
        int num1 = 0;
        while (enumerator.MoveNext())
        {
          RedisKey[] redisKeyArray = keys;
          int index1 = num1;
          IncrementWindowedValueParameter current = enumerator.Current;
          RedisKey key = current.Key;
          redisKeyArray[index1] = key;
          int num2 = num1 * 2;
          RedisValue[] redisValueArray1 = values;
          int index2 = num2;
          current = enumerator.Current;
          RedisValue redisValue1 = RedisValue.op_Implicit(current.Value);
          redisValueArray1[index2] = redisValue1;
          RedisValue[] redisValueArray2 = values;
          int index3 = num2 + 1;
          current = enumerator.Current;
          RedisValue redisValue2 = RedisValue.op_Implicit(current.Maximum);
          redisValueArray2[index3] = redisValue2;
          ++num1;
        }
        values[windowedValueParameterList.Count * 2] = RedisValue.op_Implicit((long) windowDuration.TotalMilliseconds);
        ++totalBatches;
        totalKeys += keys.Length;
        totalValues += values.Length;
        totalSize += this.ValidateMessageSize(requestContext, (IList<RedisKey>) keys, (IList<RedisValue>) values, callerName: nameof (IncrementWindowedValue));
        zero += this.Backoff(requestContext, totalRequests++);
        RedisValue[] source = RedisResult.op_Explicit(this.m_redisDb.ScriptEvaluate(str, keys, values, (CommandFlags) 0));
        longList.AddRange(((IEnumerable<RedisValue>) source).Select<RedisValue, long>((Func<RedisValue, long>) (x => RedisValue.op_Explicit(x))));
      }
      this.TraceBatchCompleted(requestContext, totalBatches, totalKeys, totalValues, totalSize, totalRequests, zero, nameof (IncrementWindowedValue));
      return (IEnumerable<long>) longList;
    }

    public TimeSpan?[] GetTimeToLiveWindowedValue(
      IVssRequestContext requestContext,
      RedisKey[] items)
    {
      if (items == null || items.Length == 0)
        return new TimeSpan?[0];
      string str = this.m_resourceManager.LoadLuaScript(requestContext, "GetWindowedTtl.lua");
      this.ValidateMessageSize(requestContext, (IList<RedisKey>) items, (IList<RedisValue>) null, callerName: nameof (GetTimeToLiveWindowedValue));
      RedisValue[] array = RedisResult.op_Explicit(this.m_redisDb.ScriptEvaluate(str, items, (RedisValue[]) null, (CommandFlags) 0));
      Func<long, TimeSpan?> longToTimespan = (Func<long, TimeSpan?>) (x => x >= 0L ? new TimeSpan?(TimeSpan.FromMilliseconds((double) x)) : new TimeSpan?());
      Converter<RedisValue, TimeSpan?> converter = (Converter<RedisValue, TimeSpan?>) (item => longToTimespan(RedisValue.op_Explicit(item)));
      return Array.ConvertAll<RedisValue, TimeSpan?>(array, converter);
    }

    private string EnsureScriptExtension(string scriptName)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(scriptName, nameof (scriptName));
      return scriptName.EndsWith(".lua") ? scriptName : scriptName + ".lua";
    }

    private int ValidateMessageSize(
      IVssRequestContext requestContext,
      IList<RedisKey> keys,
      IList<RedisValue> values,
      RedisKey? sampleKey = null,
      bool throwOnError = true,
      [CallerMemberName] string callerName = null)
    {
      int num = 0;
      if (keys != null)
      {
        for (int index = 0; index < keys.Count; ++index)
          num += keys[index].GetSize();
      }
      if (values != null)
      {
        for (int index = 0; index < values.Count; ++index)
          num += values[index].GetSize();
      }
      if (this.m_maxMessageSize != 0 && num > this.m_maxMessageSize)
      {
        sampleKey = sampleKey ?? (keys != null ? new RedisKey?(keys.FirstOrDefault<RedisKey>()) : new RedisKey?());
        requestContext.Trace(8110005, TraceLevel.Error, "Redis", nameof (VssRedisDatabase), string.Format("Oversize message: caller={0}, totalKeys={1}, totalValues={2}, overallSize={3}, threshold={4}, sampleKey={5}, stack={6}", (object) callerName, (object) keys?.Count, (object) values?.Count, (object) num, (object) this.m_maxMessageSize, (object) sampleKey, (object) Environment.StackTrace));
        if (this.m_enforceMessageSize & throwOnError)
          throw new RedisOversizeMessageException(string.Format("Message size exceeds the threshold (size={0}, threshold={1})", (object) num, (object) this.m_maxMessageSize));
      }
      return num;
    }

    protected virtual TimeSpan Backoff(IVssRequestContext requestContext, int requestCount)
    {
      if (requestCount == 0)
        return TimeSpan.Zero;
      TimeSpan exponentialBackoff = BackoffTimerHelper.GetExponentialBackoff(requestCount, TimeSpan.FromMilliseconds(0.0), TimeSpan.FromMilliseconds(50.0), TimeSpan.FromMilliseconds(1.0), this.m_radixBackoff);
      requestContext.ThrowIfCanceled();
      Thread.Sleep(exponentialBackoff);
      return exponentialBackoff;
    }

    protected virtual async Task<TimeSpan> BackoffAsync(
      IVssRequestContext requestContext,
      int requestCount)
    {
      if (requestCount == 0)
        return TimeSpan.Zero;
      TimeSpan backoff = BackoffTimerHelper.GetExponentialBackoff(requestCount, TimeSpan.FromMilliseconds(0.0), TimeSpan.FromMilliseconds(50.0), TimeSpan.FromMilliseconds(1.0), this.m_radixBackoff);
      await Task.Delay(backoff, requestContext.CancellationToken);
      return backoff;
    }

    private IEnumerable<IList<T>> Batch<T>(
      IVssRequestContext requestContext,
      IEnumerable<T> items,
      Func<T, int> getSize,
      bool allowBatching)
    {
      items = items ?? Enumerable.Empty<T>();
      if (this.m_maxMessageSize > 0 & allowBatching)
      {
        List<T> batch = new List<T>();
        int num = 0;
        foreach (T obj in items)
        {
          T item = obj;
          int itemSize = getSize(item);
          if (num + itemSize > this.m_maxMessageSize && batch.Count > 0)
          {
            yield return (IList<T>) batch;
            batch.Clear();
            num = 0;
          }
          batch.Add(item);
          num += itemSize;
          item = default (T);
        }
        if (batch.Count > 0)
          yield return (IList<T>) batch;
        batch = (List<T>) null;
      }
      else
      {
        if (!(items is IList<T> objList))
          objList = (IList<T>) items.ToList<T>();
        if (objList.Count > 0)
          yield return objList;
      }
    }

    protected virtual void TraceBatchCompleted(
      IVssRequestContext requestContext,
      int totalBatches,
      int totalKeys,
      int totalValues,
      int totalSize,
      int totalRequests,
      TimeSpan totalBackoff,
      [CallerMemberName] string methodName = null)
    {
      if (totalBatches <= 1)
        return;
      requestContext.TraceAlways(8140006, TraceLevel.Info, "Redis", nameof (VssRedisDatabase), "{0} completed with batches={1}, keys={2}, values={3}, size={4}, requests={5}, backoff={6}ms", (object) methodName, (object) totalBatches, (object) totalKeys, (object) totalValues, (object) totalSize, (object) totalRequests, (object) totalBackoff.TotalMilliseconds);
    }
  }
}
