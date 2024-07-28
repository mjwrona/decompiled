// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Notifications.Server.ProcessTreeContext
// Assembly: Microsoft.VisualStudio.Services.Notifications.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8ED1F52D-E567-4EFA-AAED-F2D9262BE9C2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Notifications.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Notifications.Server
{
  public class ProcessTreeContext
  {
    private Dictionary<string, Guid> m_displayNameMap;
    private Dictionary<Guid, string> m_teamFoundationIdMap;

    public ProcessTreeContext(
      IVssRequestContext requestContext,
      Subscription subscription,
      bool isAfterReadSubscription)
    {
      this.IVssRequestContext = requestContext;
      this.IsAfterReadSubscription = isAfterReadSubscription;
      this.Subscription = subscription;
    }

    public IVssRequestContext IVssRequestContext { get; internal set; }

    public Subscription Subscription { get; internal set; }

    public bool IsAfterReadSubscription { get; internal set; }

    public Dictionary<string, Guid> DisplayNameMap
    {
      get
      {
        if (this.m_displayNameMap == null)
          this.m_displayNameMap = new Dictionary<string, Guid>((IEqualityComparer<string>) VssStringComparer.DomainName);
        return this.m_displayNameMap;
      }
    }

    public Dictionary<Guid, string> TeamFoundationIdMap
    {
      get
      {
        if (this.m_teamFoundationIdMap == null)
          this.m_teamFoundationIdMap = new Dictionary<Guid, string>();
        return this.m_teamFoundationIdMap;
      }
    }
  }
}
