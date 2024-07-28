// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Web.Models.Factories.WorkItemDeleteShallowReferenceFactory
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Web, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: CCDEBCB9-DB6B-41A2-8797-78C2455AE321
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Web.dll

using Microsoft.Azure.Boards.WebApi.Common.Converters;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using System;

namespace Microsoft.TeamFoundation.WorkItemTracking.Web.Models.Factories
{
  internal static class WorkItemDeleteShallowReferenceFactory
  {
    public static WorkItemDeleteShallowReference Create(
      WorkItemTrackingRequestContext witRequestContext,
      int id,
      bool returnProjectScopedUrl = true)
    {
      WorkItemDeleteShallowReference shallowReference = new WorkItemDeleteShallowReference();
      shallowReference.Id = new int?(id);
      IVssRequestContext requestContext = witRequestContext.RequestContext;
      int workItemId = id;
      bool flag = returnProjectScopedUrl;
      Guid? project = new Guid?();
      int num = flag ? 1 : 0;
      Guid? remoteHostId = new Guid?();
      Guid? remoteProjectId = new Guid?();
      shallowReference.Url = WitUrlHelper.GetWorkItemUrl(requestContext, workItemId, project: project, generateProjectScopedUrl: num != 0, remoteHostId: remoteHostId, remoteProjectId: remoteProjectId);
      return shallowReference;
    }
  }
}
