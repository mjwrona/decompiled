// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Boards.WebApi.Common.Converters.WorkItemTypeReferenceFactory
// Assembly: Microsoft.Azure.Boards.WebApi.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: FC99C479-6852-4E74-BCA4-2660760F9D83
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Boards.WebApi.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using Microsoft.VisualStudio.Services.WebApi;

namespace Microsoft.Azure.Boards.WebApi.Common.Converters
{
  public static class WorkItemTypeReferenceFactory
  {
    public static WorkItemTypeReference Create(
      IVssRequestContext requestContext,
      Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.WorkItemType workItemType)
    {
      WorkItemTypeReference itemTypeReference = new WorkItemTypeReference((ISecuredObject) workItemType);
      itemTypeReference.Name = workItemType.Name;
      itemTypeReference.Url = WitUrlHelper.GetWorkItemTypeUrl(requestContext, workItemType.ProjectId, workItemType.ReferenceName);
      return itemTypeReference;
    }
  }
}
