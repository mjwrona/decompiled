// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.TeamFoundationActivityLogService
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Common;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public class TeamFoundationActivityLogService : IVssFrameworkService
  {
    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    public ActivityLogEntry GetActivitylogEntry(IVssRequestContext requestContext, int commandId)
    {
      this.CheckReadPermission(requestContext);
      ActivityLogEntry activitylogEntry = (ActivityLogEntry) null;
      using (CommandComponent component = requestContext.CreateComponent<CommandComponent>())
      {
        ResultCollection activityLogEntry = component.GetActivityLogEntry(commandId);
        ObjectBinder<ActivityLogEntry> current1 = activityLogEntry.GetCurrent<ActivityLogEntry>();
        if (current1.Items.Count > 0)
        {
          activitylogEntry = current1.Items[0];
          activityLogEntry.NextResult();
          ObjectBinder<ActivityLogParameter> current2 = activityLogEntry.GetCurrent<ActivityLogParameter>();
          if (current2.Items.Count > 0)
            activitylogEntry.Parameters.AddRange((IEnumerable<ActivityLogParameter>) current2.Items);
        }
      }
      return activitylogEntry;
    }

    public IEnumerable<int> QueryActivityLogIds(
      IVssRequestContext requestContext,
      string identitiy,
      int limit,
      List<KeyValuePair<ActivityLogColumns, SortOrder>> sortColumns)
    {
      this.CheckReadPermission(requestContext);
      using (CommandComponent component = requestContext.CreateComponent<CommandComponent>())
      {
        ObjectBinder<int> current = component.QueryActivityLogIds(identitiy, limit, (IEnumerable<KeyValuePair<ActivityLogColumns, SortOrder>>) sortColumns).GetCurrent<int>();
        if (current.Items.Count > 0)
          return (IEnumerable<int>) current.Items;
      }
      return (IEnumerable<int>) new List<int>();
    }

    public IEnumerable<ActivityLogEntry> QueryActivityLogEntries(
      IVssRequestContext requestContext,
      int[] ids)
    {
      this.CheckReadPermission(requestContext);
      using (CommandComponent component = requestContext.CreateComponent<CommandComponent>())
      {
        ObjectBinder<ActivityLogEntry> current = component.QueryActivitylogEntries(ids).GetCurrent<ActivityLogEntry>();
        if (current.Items.Count > 0)
          return (IEnumerable<ActivityLogEntry>) current.Items;
      }
      return (IEnumerable<ActivityLogEntry>) new List<ActivityLogEntry>();
    }

    private void CheckReadPermission(IVssRequestContext requestContext)
    {
      if (requestContext.IsSystemContext)
        return;
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
      TeamFoundationSecurityService service = vssRequestContext.GetService<TeamFoundationSecurityService>();
      IVssSecurityNamespace securityNamespace = service.GetSecurityNamespace(vssRequestContext, FrameworkSecurity.FrameworkNamespaceId);
      if (service.GetSecurityNamespace(vssRequestContext, FrameworkSecurity.DiagnosticNamespaceId).HasPermission(vssRequestContext, FrameworkSecurity.DiagnosticNamespaceToken, 4, false))
        return;
      securityNamespace.CheckPermission(vssRequestContext, FrameworkSecurity.FrameworkNamespaceToken, 2, false);
    }
  }
}
