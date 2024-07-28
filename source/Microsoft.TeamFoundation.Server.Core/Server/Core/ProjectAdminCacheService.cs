// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.Core.ProjectAdminCacheService
// Assembly: Microsoft.TeamFoundation.Server.Core, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9DD3208E-87CF-4F7C-8D96-8880BDAD13B2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.Core.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;

namespace Microsoft.TeamFoundation.Server.Core
{
  internal class ProjectAdminCacheService : VssMemoryCacheService<Guid, string>
  {
    protected override void ServiceStart(IVssRequestContext systemRequestContext)
    {
      base.ServiceStart(systemRequestContext);
      systemRequestContext.GetService<ITeamFoundationSqlNotificationService>().RegisterNotification(systemRequestContext, "Default", ProjectNotifications.TeamProjectDeleted, new SqlNotificationCallback(this.OnTeamProjectDeleted), false);
    }

    protected override void ServiceEnd(IVssRequestContext systemRequestContext)
    {
      systemRequestContext.GetService<ITeamFoundationSqlNotificationService>().UnregisterNotification(systemRequestContext, "Default", ProjectNotifications.TeamProjectDeleted, new SqlNotificationCallback(this.OnTeamProjectDeleted), false);
      base.ServiceEnd(systemRequestContext);
    }

    private void OnTeamProjectDeleted(
      IVssRequestContext requestContext,
      Guid eventClass,
      string eventData)
    {
      Guid result;
      if (Guid.TryParse(eventData, out result))
        this.Remove(requestContext, result);
      else
        this.Clear(requestContext);
    }
  }
}
