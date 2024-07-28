// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Boards.WebApi.Common.Converters.WorkItemTypeCategoryFactory
// Assembly: Microsoft.Azure.Boards.WebApi.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: FC99C479-6852-4E74-BCA4-2660760F9D83
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Boards.WebApi.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Linq;

namespace Microsoft.Azure.Boards.WebApi.Common.Converters
{
  public static class WorkItemTypeCategoryFactory
  {
    public static Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItemTypeCategory Create(
      IVssRequestContext requestContext,
      Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.WorkItemTypeCategory workItemTypeCategory)
    {
      IWorkItemTypeService workItemTypeService = requestContext.GetService<IWorkItemTypeService>();
      Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItemTypeCategory itemTypeCategory = new Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItemTypeCategory((ISecuredObject) workItemTypeCategory);
      itemTypeCategory.Name = workItemTypeCategory.Name;
      itemTypeCategory.ReferenceName = workItemTypeCategory.ReferenceName;
      itemTypeCategory.DefaultWorkItemType = WorkItemTypeReferenceFactory.Create(requestContext, workItemTypeService.GetWorkItemTypeByReferenceName(requestContext, workItemTypeCategory.ProjectId, workItemTypeCategory.DefaultWorkItemTypeName));
      itemTypeCategory.WorkItemTypes = workItemTypeCategory.WorkItemTypeNames.Select<string, WorkItemTypeReference>((Func<string, WorkItemTypeReference>) (win => WorkItemTypeReferenceFactory.Create(requestContext, workItemTypeService.GetWorkItemTypeByReferenceName(requestContext, workItemTypeCategory.ProjectId, win))));
      itemTypeCategory.Url = WitUrlHelper.GetWorkItemTypeCategoryUrl(requestContext, workItemTypeCategory.ProjectId, workItemTypeCategory.ReferenceName);
      return itemTypeCategory;
    }
  }
}
