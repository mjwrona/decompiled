// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Devops.Work.RemoteServices.WorkItemTypeCategoryFrameworkService
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
  internal class WorkItemTypeCategoryFrameworkService : 
    IWorkItemTypeCategoryRemotableService,
    IVssFrameworkService
  {
    void IVssFrameworkService.ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    void IVssFrameworkService.ServiceStart(IVssRequestContext systemRequestContext)
    {
    }

    public WorkItemTypeCategory GetWorkItemTypeCategory(
      IVssRequestContext requestContext,
      Guid projectId,
      string categoryNameOrReferenceName)
    {
      ArgumentUtility.CheckForEmptyGuid(projectId, nameof (projectId));
      ArgumentUtility.CheckStringForNullOrEmpty(categoryNameOrReferenceName, nameof (categoryNameOrReferenceName));
      return requestContext.TraceBlock<WorkItemTypeCategory>(919650, 919651, 919652, "FrameworkServices", nameof (WorkItemTypeCategoryFrameworkService), nameof (GetWorkItemTypeCategory), (Func<WorkItemTypeCategory>) (() => requestContext.GetClient<WorkItemTrackingHttpClient>().GetWorkItemTypeCategoryAsync(projectId, categoryNameOrReferenceName).Result));
    }

    public WorkItemTypeCategory GetWorkItemTypeCategory(
      IVssRequestContext requestContext,
      string projectName,
      string categoryNameOrReferenceName)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(projectName, nameof (projectName));
      ArgumentUtility.CheckStringForNullOrEmpty(categoryNameOrReferenceName, nameof (categoryNameOrReferenceName));
      return requestContext.TraceBlock<WorkItemTypeCategory>(919650, 919651, 919652, "FrameworkServices", nameof (WorkItemTypeCategoryFrameworkService), nameof (GetWorkItemTypeCategory), (Func<WorkItemTypeCategory>) (() => requestContext.GetClient<WorkItemTrackingHttpClient>().GetWorkItemTypeCategoryAsync(projectName, categoryNameOrReferenceName).Result));
    }

    public IEnumerable<WorkItemTypeCategory> GetWorkItemTypeCategories(
      IVssRequestContext requestContext,
      string projectName)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(projectName, nameof (projectName));
      return (IEnumerable<WorkItemTypeCategory>) requestContext.TraceBlock<List<WorkItemTypeCategory>>(919650, 919651, 919652, "FrameworkServices", nameof (WorkItemTypeCategoryFrameworkService), "GetWorkItemTypeCategory", (Func<List<WorkItemTypeCategory>>) (() => requestContext.GetClient<WorkItemTrackingHttpClient>().GetWorkItemTypeCategoriesAsync(projectName).Result));
    }

    public IEnumerable<WorkItemTypeCategory> GetWorkItemTypeCategories(
      IVssRequestContext requestContext,
      Guid projectId)
    {
      ArgumentUtility.CheckForEmptyGuid(projectId, nameof (projectId));
      return (IEnumerable<WorkItemTypeCategory>) requestContext.TraceBlock<List<WorkItemTypeCategory>>(919650, 919651, 919652, "FrameworkServices", nameof (WorkItemTypeCategoryFrameworkService), "GetWorkItemTypeCategory", (Func<List<WorkItemTypeCategory>>) (() => requestContext.GetClient<WorkItemTrackingHttpClient>().GetWorkItemTypeCategoriesAsync(projectId).Result));
    }
  }
}
