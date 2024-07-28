// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server.ContribtuionLookupService
// Assembly: Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3FE9DD3E-5758-4D1B-8056-ECAF8A4B7A77
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ExtensionManagement.WebApi;
using Microsoft.VisualStudio.Services.Gallery.Types.Server;
using System;
using System.Collections.Concurrent;

namespace Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server
{
  internal class ContribtuionLookupService : IVssFrameworkService
  {
    private ConcurrentDictionary<string, Contribution> contributionHashMap;

    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
      systemRequestContext.CheckDeploymentRequestContext();
      this.contributionHashMap = new ConcurrentDictionary<string, Contribution>();
      systemRequestContext.GetService<ITeamFoundationSqlNotificationService>().CreateRegistration(systemRequestContext, "Default", GallerySdkSqlNotificationClasses.ForceCacheRefresh, new SqlNotificationCallback(this.OnForceFlush), false, false);
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext) => systemRequestContext.GetService<ITeamFoundationSqlNotificationService>().UnregisterNotification(systemRequestContext, "Default", GallerySdkSqlNotificationClasses.ForceCacheRefresh, new SqlNotificationCallback(this.OnForceFlush), false);

    public Contribution GetContribution(IVssRequestContext requestContext, Contribution c)
    {
      string key = c.Id + "::" + c.GetHashCode().ToString();
      Contribution contribution;
      if (this.contributionHashMap.TryGetValue(key, out contribution))
        return contribution;
      this.contributionHashMap.TryAdd(key, c);
      return c;
    }

    private void OnForceFlush(IVssRequestContext requestContext, Guid eventClass, string eventData) => this.contributionHashMap = new ConcurrentDictionary<string, Contribution>();
  }
}
