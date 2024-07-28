// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Boards.WebApi.Common.WorkItemLinkFactory
// Assembly: Microsoft.Azure.Boards.WebApi.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: FC99C479-6852-4E74-BCA4-2660760F9D83
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Boards.WebApi.Common.dll

using Microsoft.Azure.Boards.WebApi.Common.Converters;
using Microsoft.TeamFoundation.WorkItemTracking.Server;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using System;
using System.Runtime.Serialization;

namespace Microsoft.Azure.Boards.WebApi.Common
{
  [DataContract]
  public class WorkItemLinkFactory
  {
    public static WorkItemLink Create(
      WorkItemTrackingRequestContext witRequestContext,
      int source,
      int target,
      int linkTypeId,
      bool excludeUrls = false)
    {
      return new WorkItemLink()
      {
        Rel = linkTypeId != 0 ? WorkItemRelationFactory.GetWorkItemLinkType(linkTypeId, witRequestContext) : (string) null,
        Source = source > 0 ? WorkItemReferenceFactory.Create(witRequestContext, source, excludeUrls: excludeUrls) : (WorkItemReference) null,
        Target = WorkItemReferenceFactory.Create(witRequestContext, target, excludeUrls: excludeUrls)
      };
    }

    internal static WorkItemLink Create(
      WorkItemTrackingRequestContext witRequestContext,
      int source,
      int target,
      Guid? sourceProjectId,
      Guid? targetProjectId,
      string sourceToken,
      string targetToken,
      int linkTypeId,
      bool returnProjectScopedUrl = true,
      bool excludeUrls = false)
    {
      return new WorkItemLink(targetToken)
      {
        Rel = linkTypeId != 0 ? WorkItemRelationFactory.GetWorkItemLinkType(linkTypeId, witRequestContext) : (string) null,
        Source = source > 0 ? WorkItemReferenceFactory.Create(witRequestContext, source, sourceProjectId, sourceToken, returnProjectScopedUrl, excludeUrls) : (WorkItemReference) null,
        Target = WorkItemReferenceFactory.Create(witRequestContext, target, targetProjectId, targetToken, returnProjectScopedUrl, excludeUrls)
      };
    }
  }
}
