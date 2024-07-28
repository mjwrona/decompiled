// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Boards.WebApi.Common.Converters.WorkItemDeleteFactory
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
  public static class WorkItemDeleteFactory
  {
    public static WorkItemDelete Create(
      WorkItemTrackingRequestContext witRequestContext,
      WorkItemRevision workItem,
      bool returnProjectScopedUrl = true)
    {
      Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItem toWebApiModel = WorkItemFactory.CreateToWebApiModel(witRequestContext, workItem, isDeleted: workItem.IsDeleted, returnProjectScopedUrl: returnProjectScopedUrl);
      WorkItemDelete workItemDelete = new WorkItemDelete();
      workItemDelete.Id = toWebApiModel.Id;
      workItemDelete.Code = new int?(Convert.ToInt32((object) HttpStatusCode.OK));
      object obj;
      workItemDelete.Type = toWebApiModel.Fields.TryGetValue("System.WorkItemType", out obj) ? Convert.ToString(obj) : (string) null;
      workItemDelete.Name = toWebApiModel.Fields.TryGetValue("System.Title", out obj) ? Convert.ToString(obj) : (string) null;
      workItemDelete.Project = toWebApiModel.Fields.TryGetValue("System.TeamProject", out obj) ? Convert.ToString(obj) : (string) null;
      workItemDelete.DeletedDate = toWebApiModel.Fields.TryGetValue("System.ChangedDate", out obj) ? Convert.ToString(obj) : (string) null;
      workItemDelete.DeletedBy = toWebApiModel.Fields.TryGetValue("System.ChangedBy", out obj) ? Convert.ToString(obj) : (string) null;
      workItemDelete.Url = toWebApiModel.Url;
      workItemDelete.Resource = toWebApiModel;
      return workItemDelete;
    }

    internal static WorkItemDelete Create(int workItemId, TeamFoundationServiceException exception)
    {
      bool flag = exception != null;
      WorkItemDelete workItemDelete = new WorkItemDelete();
      workItemDelete.Id = new int?(workItemId);
      workItemDelete.Code = new int?(flag ? Convert.ToInt32((object) HttpStatusCode.NotFound) : Convert.ToInt32((object) HttpStatusCode.OK));
      workItemDelete.Message = flag ? exception.Message : (string) null;
      return workItemDelete;
    }
  }
}
