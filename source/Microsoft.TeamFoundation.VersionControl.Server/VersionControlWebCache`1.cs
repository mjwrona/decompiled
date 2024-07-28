// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Server.VersionControlWebCache`1
// Assembly: Microsoft.TeamFoundation.VersionControl.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C5D9EE74-8805-4E00-959F-39760D2358B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.VersionControl.Server.dll

using Microsoft.VisualStudio.Services.Common;
using System;
using System.Web;
using System.Web.Caching;

namespace Microsoft.TeamFoundation.VersionControl.Server
{
  internal abstract class VersionControlWebCache<T> : IVersionControlWebCache<T>
  {
    protected TeamFoundationVersionControlService m_versionControlService;

    protected VersionControlWebCache(
      TeamFoundationVersionControlService versionControlService)
    {
      ArgumentUtility.CheckForNull<TeamFoundationVersionControlService>(versionControlService, nameof (versionControlService));
      this.m_versionControlService = versionControlService;
    }

    protected virtual string FullyQualifyKey(string providedKey) => this.m_versionControlService.ServiceHost.InstanceId.ToString() + ", " + providedKey;

    public T Get(string key) => (T) HttpRuntime.Cache.Get(this.FullyQualifyKey(key));

    public void Insert(
      string key,
      T value,
      DateTime absoluteExpiration,
      TimeSpan slidingExpiration)
    {
      HttpRuntime.Cache.Insert(this.FullyQualifyKey(key), (object) value, (CacheDependency) null, absoluteExpiration, slidingExpiration);
    }

    public T Remove(string key) => (T) HttpRuntime.Cache.Remove(this.FullyQualifyKey(key));
  }
}
