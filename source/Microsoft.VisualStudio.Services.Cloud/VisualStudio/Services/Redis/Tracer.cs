// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Redis.Tracer
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Microsoft.VisualStudio.Services.Redis
{
  public abstract class Tracer
  {
    protected readonly string m_cacheArea;
    protected const string c_globalAreaName = "<GLOBAL>";

    protected Tracer(string cacheArea) => this.m_cacheArea = cacheArea;

    public static Tracer Global => ErrorTracer.Global;

    public string Area => this.m_cacheArea;

    public static void RedisError(Exception ex) => TeamFoundationTracingService.TraceExceptionRaw(8110001, TraceLevel.Error, "Redis", "RedisCache", ex);

    public static void RedisError(string message, params object[] args) => TeamFoundationTracingService.TraceRaw(8110001, TraceLevel.Error, "Redis", "RedisCache", message, args);

    public virtual void RedisError(IVssRequestContext requestContext, Exception ex)
    {
    }

    public virtual void RedisError(
      IVssRequestContext requestContext,
      string message,
      params object[] args)
    {
    }

    public virtual void RedisServerInfo(
      IVssRequestContext requestContext,
      string message,
      params object[] args)
    {
    }

    public virtual void RedisSlowCommand(
      IVssRequestContext requestContext,
      string message,
      params object[] args)
    {
    }

    public virtual void RedisCacheHit(IVssRequestContext requestContext, string cacheKey)
    {
    }

    public virtual void RedisCacheMiss(IVssRequestContext requestContext, string cacheKey)
    {
    }

    public virtual void CacheInvalidated(IVssRequestContext requestContext, string cacheKey)
    {
    }

    public virtual void CacheSet(
      IVssRequestContext requestContext,
      IEnumerable<KeyValuePair<string, byte[]>> cacheItems)
    {
    }

    public virtual void CacheExpired(
      IVssRequestContext requestContext,
      long items,
      TimeSpan interval)
    {
    }

    public virtual void RedisCacheEvicted(
      IVssRequestContext requestContext,
      long items,
      TimeSpan interval)
    {
    }

    public virtual void MemoryCacheEvicted(IVssRequestContext requestContext, long items)
    {
    }

    public virtual void CpuUsage(IVssRequestContext requestContext, int seconds)
    {
    }

    public virtual void MemoryUsage(IVssRequestContext requestContext, long bytes)
    {
    }

    public virtual void TransactionFailed(IVssRequestContext requestContext)
    {
    }

    public virtual void EnterRedis(IVssRequestContext requestContext)
    {
    }

    public virtual void ExitRedis(IVssRequestContext requestContext, long duration)
    {
    }

    public virtual void RetryDone(int retryAttempts)
    {
    }

    public virtual IDisposable TimedCall(IVssRequestContext requestContext) => (IDisposable) null;

    public void Trace(
      IVssRequestContext requestContext,
      Action action,
      object arg = null,
      [CallerMemberName] string methodName = null)
    {
      this.Trace(requestContext, action, action.Method.DeclaringType, methodName, arg);
    }

    public T Trace<T>(
      IVssRequestContext requestContext,
      Func<T> action,
      object arg = null,
      [CallerMemberName] string methodName = null)
    {
      T result = default (T);
      this.Trace(requestContext, (Action) (() => result = action()), action.Method.DeclaringType, methodName, arg);
      return result;
    }

    private void Trace(
      IVssRequestContext requestContext,
      Action action,
      Type declaringType,
      string methodName,
      object arg)
    {
      while (declaringType.DeclaringType != (Type) null)
        declaringType = declaringType.DeclaringType;
      try
      {
        this.EnterMethod(requestContext, declaringType.Name, methodName, arg);
        action();
      }
      catch (Exception ex)
      {
        this.MethodException(requestContext, declaringType.Name, methodName, ex);
        throw;
      }
      finally
      {
        this.LeaveMethod(requestContext, declaringType.Name, methodName);
      }
    }

    protected virtual void EnterMethod(
      IVssRequestContext requestContext,
      string className,
      string methodName,
      object arg)
    {
    }

    protected virtual void LeaveMethod(
      IVssRequestContext requestContext,
      string className,
      string methodName)
    {
    }

    protected virtual void MethodException(
      IVssRequestContext requestContext,
      string className,
      string methodName,
      Exception ex)
    {
    }
  }
}
