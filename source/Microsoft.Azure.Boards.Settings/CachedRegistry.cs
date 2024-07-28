// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Boards.Settings.CachedRegistry
// Assembly: Microsoft.Azure.Boards.Settings, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2AC3574E-9414-4605-BAB7-1F6B28A75804
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.Azure.Boards.Settings.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.WebAccess;
using System;
using System.Collections.Generic;

namespace Microsoft.Azure.Boards.Settings
{
  public abstract class CachedRegistry : ISettingsHive
  {
    private ILockName m_cacheLockName;
    private Dictionary<string, string> m_cache = new Dictionary<string, string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
    private Dictionary<string, string> m_delta;

    protected CachedRegistry()
    {
    }

    protected CachedRegistry(IVssRequestContext requestContext)
    {
      this.RequestContext = requestContext;
      this.m_cacheLockName = this.RequestContext.ServiceHost.CreateUniqueLockName(nameof (CachedRegistry));
    }

    public IVssRequestContext RequestContext { get; private set; }

    public T ReadSetting<T>(string path, T defaultValue) => ConvertUtility.FromString<T>(this.ReadValue(path), defaultValue);

    public abstract IDictionary<string, object> QuerySettings(string pathPattern);

    public void WriteSetting<T>(string path, T value) => this.WriteValue(path, ConvertUtility.ToString<T>(value));

    public virtual IEnumerable<T> ReadEnumerableSetting<T>(string path, IEnumerable<T> defaultValue)
    {
      string str = this.ReadValue(path);
      return str == null ? defaultValue : str.Split<T>(';');
    }

    public virtual void WriteEnumerableSetting<T>(string path, IEnumerable<T> value)
    {
      if (value == null)
        this.WriteValue(path, (string) null);
      else
        this.WriteValue(path, value.StringJoin<T>(';'));
    }

    public string ReadValue(string path)
    {
      string str;
      using (this.RequestContext.AcquireReaderLock(this.m_cacheLockName))
      {
        if (this.TryGetValue(path, out str))
          return str;
      }
      IDictionary<string, string> data = this.Read(path);
      using (this.RequestContext.AcquireWriterLock(this.m_cacheLockName))
      {
        this.CacheData(data);
        if (!this.TryGetValue(path, out str))
        {
          str = (string) null;
          this.m_cache[path] = str;
        }
      }
      return str;
    }

    private bool TryGetValue(string path, out string value) => this.m_delta != null && this.m_delta.TryGetValue(path, out value) || this.m_cache.TryGetValue(path, out value);

    public void WriteValue(string path, string value)
    {
      using (this.RequestContext.AcquireWriterLock(this.m_cacheLockName))
      {
        if (this.m_delta == null)
          this.m_delta = new Dictionary<string, string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
        this.m_delta[path] = value;
        this.m_cache.Remove(path);
      }
    }

    public void Cache(string pathPattern)
    {
      IDictionary<string, string> data = this.Read(pathPattern);
      using (this.RequestContext.AcquireWriterLock(this.m_cacheLockName))
        this.CacheData(data);
    }

    private void CacheData(IDictionary<string, string> data)
    {
      if (data == null || data.Count <= 0)
        return;
      foreach (KeyValuePair<string, string> keyValuePair in (IEnumerable<KeyValuePair<string, string>>) data)
      {
        if (this.m_delta == null || !this.m_delta.ContainsKey(keyValuePair.Key))
          this.m_cache[keyValuePair.Key] = keyValuePair.Value;
      }
    }

    public void Flush()
    {
      IDictionary<string, string> data = (IDictionary<string, string>) null;
      using (this.RequestContext.AcquireWriterLock(this.m_cacheLockName))
      {
        if (this.m_delta != null)
        {
          data = (IDictionary<string, string>) new Dictionary<string, string>(this.m_delta.Count);
          foreach (KeyValuePair<string, string> keyValuePair in this.m_delta)
          {
            this.m_cache[keyValuePair.Key] = keyValuePair.Value;
            data[keyValuePair.Key] = keyValuePair.Value;
          }
          this.m_delta = (Dictionary<string, string>) null;
        }
      }
      if (data == null)
        return;
      this.Write(data);
    }

    protected abstract IDictionary<string, string> Read(string pathPattern);

    protected abstract void Write(IDictionary<string, string> data);

    protected abstract void Clear();
  }
}
