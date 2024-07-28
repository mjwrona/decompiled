// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Devops.Work.RemoteServices.WorkItemTypeFrameworkService
// Assembly: Microsoft.Azure.Devops.Work.RemoteServices, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C97796CA-4166-42B2-B96F-9A166B07FF72
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.Azure.Devops.Work.RemoteServices.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;

namespace Microsoft.Azure.Devops.Work.RemoteServices
{
  internal class WorkItemTypeFrameworkService : IWorkItemTypeRemotableService, IVssFrameworkService
  {
    void IVssFrameworkService.ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    void IVssFrameworkService.ServiceStart(IVssRequestContext systemRequestContext)
    {
    }

    public WorkItemType GetWorkItemType(
      IVssRequestContext requestContext,
      Guid projectId,
      string witNameOrReferenceName)
    {
      ArgumentUtility.CheckForEmptyGuid(projectId, nameof (projectId));
      ArgumentUtility.CheckStringForNullOrEmpty(witNameOrReferenceName, nameof (witNameOrReferenceName));
      return requestContext.TraceBlock<WorkItemType>(919650, 919651, 919652, "FrameworkServices", nameof (WorkItemTypeFrameworkService), nameof (GetWorkItemType), (Func<WorkItemType>) (() => requestContext.GetClient<WorkItemTrackingHttpClient>().GetWorkItemTypeAsync(projectId, witNameOrReferenceName).Result));
    }

    public WorkItemType GetWorkItemType(
      IVssRequestContext requestContext,
      string projectName,
      string witNameOrReferenceName)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(projectName, nameof (projectName));
      ArgumentUtility.CheckStringForNullOrEmpty(witNameOrReferenceName, nameof (witNameOrReferenceName));
      return requestContext.TraceBlock<WorkItemType>(919650, 919651, 919652, "FrameworkServices", nameof (WorkItemTypeFrameworkService), nameof (GetWorkItemType), (Func<WorkItemType>) (() => requestContext.GetClient<WorkItemTrackingHttpClient>().GetWorkItemTypeAsync(projectName, witNameOrReferenceName).Result));
    }

    public IEnumerable<WorkItemType> GetWorkItemTypes(
      IVssRequestContext requestContext,
      Guid projectId)
    {
      ArgumentUtility.CheckForEmptyGuid(projectId, nameof (projectId));
      return (IEnumerable<WorkItemType>) requestContext.TraceBlock<List<WorkItemType>>(919650, 919651, 919652, "FrameworkServices", nameof (WorkItemTypeFrameworkService), "GetWorkItemType", (Func<List<WorkItemType>>) (() => requestContext.GetClient<WorkItemTrackingHttpClient>().GetWorkItemTypesAsync(projectId).Result));
    }

    public IEnumerable<WorkItemType> GetWorkItemTypes(
      IVssRequestContext requestContext,
      string projectName)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(projectName, nameof (projectName));
      return (IEnumerable<WorkItemType>) requestContext.TraceBlock<List<WorkItemType>>(919650, 919651, 919652, "FrameworkServices", nameof (WorkItemTypeFrameworkService), "GetWorkItemType", (Func<List<WorkItemType>>) (() => requestContext.GetClient<WorkItemTrackingHttpClient>().GetWorkItemTypesAsync(projectName).Result));
    }
  }
}
