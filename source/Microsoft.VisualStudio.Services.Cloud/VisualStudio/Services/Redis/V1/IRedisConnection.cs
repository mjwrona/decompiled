// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Redis.V1.IRedisConnection
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.CircuitBreaker;
using StackExchange.Redis;
using System;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Redis.V1
{
  public interface IRedisConnection : IDisposable
  {
    bool IsValid(IVssRequestContext requestContext);

    T Call<T>(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.Redis.Tracer tracer,
      Func<IRedisDatabase, T> run,
      Func<bool, Command<T>, T> fallback = null);

    Task<T> CallAsync<T>(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.Redis.Tracer tracer,
      Func<IRedisDatabase, Task<T>> run,
      Func<bool, CommandAsync<T>, Task<T>> fallback = null);

    void Subscribe(
      IVssRequestContext requestContext,
      RedisChannel channel,
      Action<RedisChannel, RedisValue> handler);

    void Unsubscribe(IVssRequestContext requestContext, RedisChannel channel);
  }
}
