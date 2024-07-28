// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Client.SuggestedValueCacheService
// Assembly: Microsoft.TeamFoundation.Client, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03892C75-AE2B-482B-8E0D-B14588A2C857
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Client.dll

using Microsoft.VisualStudio.Services.Security;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.Client
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public abstract class SuggestedValueCacheService : ITfsTeamProjectCollectionObject
  {
    private static int s_expirationMilliseconds = 900000;
    protected TfsTeamProjectCollection m_projectCollection;
    protected IVssHttpClient m_proxy;
    protected Dictionary<Guid, SuggestedValueCacheService.ScopeSuggestedValue> m_cache;
    protected object m_lock;

    [EditorBrowsable(EditorBrowsableState.Never)]
    internal SuggestedValueCacheService()
    {
      this.m_lock = new object();
      this.m_cache = new Dictionary<Guid, SuggestedValueCacheService.ScopeSuggestedValue>();
    }

    void ITfsTeamProjectCollectionObject.Initialize(TfsTeamProjectCollection projectCollection)
    {
      this.m_projectCollection = projectCollection;
      this.OnInitialize();
    }

    protected abstract void OnInitialize();

    public IReadOnlyList<string> GetCachedValues(Guid scope)
    {
      lock (this.m_lock)
      {
        SuggestedValueCacheService.ScopeSuggestedValue scopeSuggestedValue;
        if (this.m_cache.TryGetValue(scope, out scopeSuggestedValue))
          return (IReadOnlyList<string>) scopeSuggestedValue.Values.AsReadOnly();
      }
      return (IReadOnlyList<string>) new List<string>(0);
    }

    public bool IsPopulated(Guid scope)
    {
      lock (this.m_lock)
      {
        SuggestedValueCacheService.ScopeSuggestedValue scopeSuggestedValue;
        if (this.m_cache.TryGetValue(scope, out scopeSuggestedValue))
          return scopeSuggestedValue.IsPopulated;
      }
      return false;
    }

    public bool IsExpired(Guid scope)
    {
      lock (this.m_lock)
      {
        SuggestedValueCacheService.ScopeSuggestedValue scopeSuggestedValue;
        if (this.m_cache.TryGetValue(scope, out scopeSuggestedValue))
          return scopeSuggestedValue.IsExpired;
      }
      return true;
    }

    public Task<IReadOnlyList<string>> RefreshAsync(Guid scope)
    {
      lock (this.m_lock)
      {
        SuggestedValueCacheService.ScopeSuggestedValue scopeValues;
        if (!this.m_cache.TryGetValue(scope, out scopeValues))
        {
          scopeValues = new SuggestedValueCacheService.ScopeSuggestedValue();
          this.m_cache.Add(scope, scopeValues);
        }
        if (scopeValues.Task == null)
          scopeValues.Task = this.GetValueAsync(scope, scopeValues);
        return scopeValues.Task;
      }
    }

    internal void SetExpirationInternal(int milliseconds) => SuggestedValueCacheService.s_expirationMilliseconds = milliseconds;

    internal void SetProxyInternal(IVssHttpClient proxy) => this.m_proxy = proxy;

    protected abstract Task<IReadOnlyList<string>> GetValueAsync(
      Guid scope,
      SuggestedValueCacheService.ScopeSuggestedValue scopeValues);

    protected IReadOnlyList<string> ProcessGetSuggestedValuesAsyncTaskResult(
      Task task,
      SuggestedValueCacheService.ScopeSuggestedValue scopeValues,
      Action action)
    {
      lock (this.m_lock)
      {
        scopeValues.Task = (Task<IReadOnlyList<string>>) null;
        if (task.Exception != null)
        {
          TeamFoundationTrace.TraceException((Exception) task.Exception);
          if (task.Exception != null && task.Exception.InnerException is AccessCheckException)
          {
            scopeValues.TimeStamp = DateTime.Now;
            scopeValues.Values = new List<string>(0);
            scopeValues.IsPopulated = true;
          }
          else
            scopeValues.TimeStamp = DateTime.MinValue;
        }
        else
        {
          scopeValues.TimeStamp = DateTime.Now;
          action();
          scopeValues.IsPopulated = true;
        }
        return (IReadOnlyList<string>) scopeValues.Values.AsReadOnly();
      }
    }

    protected class ScopeSuggestedValue
    {
      internal ScopeSuggestedValue()
      {
        this.TimeStamp = DateTime.MinValue;
        this.Values = new List<string>(0);
      }

      public bool IsPopulated { get; set; }

      public List<string> Values { get; set; }

      public DateTime TimeStamp { get; set; }

      public Task<IReadOnlyList<string>> Task { get; set; }

      public bool IsExpired => DateTime.Now > this.TimeStamp.AddMilliseconds((double) SuggestedValueCacheService.s_expirationMilliseconds);
    }
  }
}
