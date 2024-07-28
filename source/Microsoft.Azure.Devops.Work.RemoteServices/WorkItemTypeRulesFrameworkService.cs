// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Devops.Work.RemoteServices.WorkItemTypeRulesFrameworkService
// Assembly: Microsoft.Azure.Devops.Work.RemoteServices, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C97796CA-4166-42B2-B96F-9A166B07FF72
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.Azure.Devops.Work.RemoteServices.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Process.WebApi;
using Microsoft.TeamFoundation.WorkItemTracking.Process.WebApi.Models;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;

namespace Microsoft.Azure.Devops.Work.RemoteServices
{
  public class WorkItemTypeRulesFrameworkService : 
    IWorkItemTypeRulesRemotableService,
    IVssFrameworkService
  {
    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
    }

    public IEnumerable<ProcessRule> GetProcessRulesForWorkItemType(
      IVssRequestContext requestContext,
      Guid processId,
      string workItemTypeReferenceName)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForEmptyGuid(processId, nameof (processId));
      ArgumentUtility.CheckStringForNullOrEmpty(workItemTypeReferenceName, nameof (workItemTypeReferenceName));
      return (IEnumerable<ProcessRule>) requestContext.TraceBlock<List<ProcessRule>>(919750, 919751, 919752, "FrameworkServices", "WorkItemTypeFrameworkService", nameof (GetProcessRulesForWorkItemType), (Func<List<ProcessRule>>) (() => requestContext.GetClient<WorkItemTrackingProcessHttpClient>().GetProcessWorkItemTypeRulesAsync(processId, workItemTypeReferenceName).Result));
    }
  }
}
