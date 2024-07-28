// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Notifications.Server.IdentityExtensionsCacheService
// Assembly: Microsoft.VisualStudio.Services.Notifications.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8ED1F52D-E567-4EFA-AAED-F2D9262BE9C2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Notifications.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Microsoft.VisualStudio.Services.Notifications.Server
{
  public class IdentityExtensionsCacheService : IIdentityExtensionsCacheService, IVssFrameworkService
  {
    private Dictionary<Guid, (DateTime CreatedTime, bool Value)> _cache = new Dictionary<Guid, (DateTime, bool)>();
    private const int TTL = 300;

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
    }

    public void AddOrUpdate(Guid identityId, bool isTeam, IVssRequestContext requestContext)
    {
      requestContext.Trace(1002095, TraceLevel.Verbose, "Notifications", nameof (IdentityExtensionsCacheService), string.Format("AddOrUpdate called for <{0}> with value <{1}>", (object) identityId, (object) isTeam));
      this._cache[identityId] = (DateTime.Now, isTeam);
    }

    public bool TryGet(Guid identityId, out bool isTeam, IVssRequestContext requestContext)
    {
      isTeam = false;
      (DateTime CreatedTime, bool Value) tuple;
      bool flag = this._cache.TryGetValue(identityId, out tuple);
      if (flag)
      {
        if (DateTime.Now - tuple.CreatedTime < TimeSpan.FromSeconds(300.0))
        {
          isTeam = tuple.Value;
        }
        else
        {
          flag = false;
          this._cache.Remove(identityId);
        }
      }
      string message = flag ? string.Format("Value for <{0}> is found <{1}>", (object) identityId, (object) isTeam) : string.Format("No hit for <{0}>", (object) identityId);
      requestContext.Trace(1002096, TraceLevel.Verbose, "Notifications", nameof (IdentityExtensionsCacheService), message);
      return flag;
    }
  }
}
