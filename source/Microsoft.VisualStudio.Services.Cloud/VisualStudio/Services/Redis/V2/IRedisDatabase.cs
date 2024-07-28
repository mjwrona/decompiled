// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Redis.V2.IRedisDatabase
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.TeamFoundation.Framework.Server;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Redis.V2
{
  public interface IRedisDatabase
  {
    long DeleteVolatileValue(
      IVssRequestContext requestContext,
      RedisKey[] items,
      bool allowBatching = false);

    Task<long> DeleteVolatileValueAsync(
      IVssRequestContext requestContext,
      RedisKey[] items,
      bool allowBatching = false);

    TimeSpan?[] GetTimeToLiveVolatileValue(IVssRequestContext requestContext, RedisKey[] items);

    TimeSpan?[] GetTimeToLiveWindowedValue(IVssRequestContext requestContext, RedisKey[] items);

    IEnumerable<long> GetWindowedValue(
      IVssRequestContext requestContext,
      IEnumerable<RedisKey> items,
      TimeSpan windowDuration);

    RedisValue[] GetVolatileValue(
      IVssRequestContext requestContext,
      RedisKey[] items,
      bool allowBatching = false);

    Task<RedisValue[]> GetListValueAsync(IVssRequestContext requestContext, RedisKey key);

    IEnumerable<long> IncrementVolatileValue(
      IVssRequestContext requestContext,
      IEnumerable<IncrementVolatileValueParameter> items,
      bool allowBatching = false);

    IEnumerable<long> IncrementWindowedValue(
      IVssRequestContext requestContext,
      IEnumerable<IncrementWindowedValueParameter> items,
      TimeSpan windowDuration,
      bool allowBatching = false);

    bool SetVolatileValue(
      IVssRequestContext requestContext,
      IEnumerable<SetVolatileValueParameter> items,
      bool allowBatching = false);

    RedisResult ScriptEvaluate(
      IVssRequestContext requestContext,
      string scriptName,
      RedisKey[] keys = null,
      RedisValue[] values = null);

    Task<RedisResult> ScriptEvaluateAsync(
      IVssRequestContext requestContext,
      string scriptName,
      RedisKey[] keys = null,
      RedisValue[] values = null);
  }
}
