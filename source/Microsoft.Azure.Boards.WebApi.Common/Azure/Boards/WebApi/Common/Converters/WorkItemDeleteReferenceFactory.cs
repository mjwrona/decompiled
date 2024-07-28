// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Boards.WebApi.Common.Converters.WorkItemDeleteReferenceFactory
// Assembly: Microsoft.Azure.Boards.WebApi.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: FC99C479-6852-4E74-BCA4-2660760F9D83
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Boards.WebApi.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using System;
using System.Net;

namespace Microsoft.Azure.Boards.WebApi.Common.Converters
{
  public static class WorkItemDeleteReferenceFactory
  {
    public static WorkItemDeleteReference Create(
      WorkItemTrackingRequestContext witRequestContext,
      WorkItemFieldData workItem,
      bool returnProjectScopedUrl = true)
    {
      WorkItemDeleteReference itemDeleteReference = new WorkItemDeleteReference();
      itemDeleteReference.Id = new int?(workItem.Id);
      itemDeleteReference.Code = new int?(Convert.ToInt32((object) HttpStatusCode.OK));
      itemDeleteReference.Type = workItem.WorkItemType;
      itemDeleteReference.Name = workItem.Title;
      itemDeleteReference.Project = workItem.GetProjectName(witRequestContext);
      itemDeleteReference.DeletedDate = Convert.ToString(workItem.ModifiedDate);
      itemDeleteReference.DeletedBy = workItem.ModifiedBy;
      IVssRequestContext requestContext = witRequestContext.RequestContext;
      int id = workItem.Id;
      int num1 = workItem.IsDeleted ? 1 : 0;
      bool flag = returnProjectScopedUrl;
      Guid? project = new Guid?();
      int num2 = flag ? 1 : 0;
      Guid? remoteHostId = new Guid?();
      Guid? remoteProjectId = new Guid?();
      itemDeleteReference.Url = WitUrlHelper.GetWorkItemUrl(requestContext, id, num1 != 0, project, num2 != 0, remoteHostId, remoteProjectId);
      return itemDeleteReference;
    }
  }
}
