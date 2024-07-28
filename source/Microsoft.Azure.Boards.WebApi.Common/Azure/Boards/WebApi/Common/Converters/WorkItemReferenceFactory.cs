// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Boards.WebApi.Common.Converters.WorkItemReferenceFactory
// Assembly: Microsoft.Azure.Boards.WebApi.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: FC99C479-6852-4E74-BCA4-2660760F9D83
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Boards.WebApi.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using System;

namespace Microsoft.Azure.Boards.WebApi.Common.Converters
{
  public static class WorkItemReferenceFactory
  {
    public static WorkItemReference Create(
      WorkItemTrackingRequestContext witRequestContext,
      int id,
      bool returnProjectScopedUrl = true,
      bool excludeUrls = false)
    {
      WorkItemReference workItemReference = new WorkItemReference();
      workItemReference.Id = id;
      string str;
      if (!excludeUrls)
      {
        IVssRequestContext requestContext = witRequestContext.RequestContext;
        int workItemId = id;
        bool flag = returnProjectScopedUrl;
        Guid? project = new Guid?();
        int num = flag ? 1 : 0;
        Guid? remoteHostId = new Guid?();
        Guid? remoteProjectId = new Guid?();
        str = WitUrlHelper.GetWorkItemUrl(requestContext, workItemId, project: project, generateProjectScopedUrl: num != 0, remoteHostId: remoteHostId, remoteProjectId: remoteProjectId);
      }
      else
        str = (string) null;
      workItemReference.Url = str;
      return workItemReference;
    }

    public static WorkItemReference Create(
      WorkItemTrackingRequestContext witRequestContext,
      int id,
      Guid? projectId,
      string token,
      bool returnProjectScopedUrl = true,
      bool excludeUrls = false)
    {
      return new WorkItemReference(token)
      {
        Id = id,
        Url = excludeUrls ? (string) null : WitUrlHelper.GetWorkItemUrl(witRequestContext.RequestContext, id, project: projectId, generateProjectScopedUrl: returnProjectScopedUrl)
      };
    }
  }
}
