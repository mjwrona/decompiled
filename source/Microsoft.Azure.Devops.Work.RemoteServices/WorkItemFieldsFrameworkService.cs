// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Devops.Work.RemoteServices.WorkItemFieldsFrameworkService
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
  public class WorkItemFieldsFrameworkService : IWorkItemFieldsRemotableService, IVssFrameworkService
  {
    public List<WorkItemField2> GetFields(
      IVssRequestContext requestContext,
      GetFieldsExpand? expand = null,
      string projectName = null)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      return requestContext.TraceBlock<List<WorkItemField2>>(919800, 919801, 919802, "FrameworkServices", "WorkItemFrameworkService", nameof (GetFields), (Func<List<WorkItemField2>>) (() =>
      {
        WorkItemTrackingHttpClient client = requestContext.GetClient<WorkItemTrackingHttpClient>();
        return projectName == null ? client.GetWorkItemFieldsAsync(expand).Result : client.GetWorkItemFieldsAsync(projectName, expand).Result;
      }));
    }

    public List<WorkItemField2> GetFields(
      IVssRequestContext requestContext,
      GetFieldsExpand? expand = null,
      Guid? projectId = null)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      return requestContext.TraceBlock<List<WorkItemField2>>(919800, 919801, 919802, "FrameworkServices", "WorkItemFrameworkService", nameof (GetFields), (Func<List<WorkItemField2>>) (() =>
      {
        WorkItemTrackingHttpClient client = requestContext.GetClient<WorkItemTrackingHttpClient>();
        if (projectId.HasValue)
        {
          Guid? nullable = projectId;
          Guid empty = Guid.Empty;
          if ((nullable.HasValue ? (nullable.HasValue ? (nullable.GetValueOrDefault() == empty ? 1 : 0) : 1) : 0) == 0)
            return client.GetWorkItemFieldsAsync(projectId.Value, expand).Result;
        }
        return client.GetWorkItemFieldsAsync(expand).Result;
      }));
    }

    public WorkItemField2 GetField(
      IVssRequestContext requestContext,
      string fieldNameOrRefName,
      string projectName = null)
    {
      return requestContext.TraceBlock<WorkItemField2>(919800, 919801, 919802, "FrameworkServices", "WorkItemFrameworkService", nameof (GetField), (Func<WorkItemField2>) (() =>
      {
        WorkItemTrackingHttpClient client = requestContext.GetClient<WorkItemTrackingHttpClient>();
        return projectName == null ? client.GetWorkItemFieldAsync(fieldNameOrRefName).Result : client.GetWorkItemFieldAsync(projectName, fieldNameOrRefName).Result;
      }));
    }

    public WorkItemField2 GetField(
      IVssRequestContext requestContext,
      string fieldNameOrRefName,
      Guid? projectId = null)
    {
      return requestContext.TraceBlock<WorkItemField2>(919800, 919801, 919802, "FrameworkServices", "WorkItemFrameworkService", nameof (GetField), (Func<WorkItemField2>) (() =>
      {
        WorkItemTrackingHttpClient client = requestContext.GetClient<WorkItemTrackingHttpClient>();
        if (projectId.HasValue)
        {
          Guid? nullable = projectId;
          Guid empty = Guid.Empty;
          if ((nullable.HasValue ? (nullable.HasValue ? (nullable.GetValueOrDefault() == empty ? 1 : 0) : 1) : 0) == 0)
            return client.GetWorkItemFieldAsync(projectId.Value, fieldNameOrRefName).Result;
        }
        return client.GetWorkItemFieldAsync(fieldNameOrRefName).Result;
      }));
    }

    public bool IsUpdatable(IVssRequestContext requestContext, string fieldReferenceName) => throw new NotImplementedException();

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
    }
  }
}
