// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Boards.WebApi.Common.Converters.WorkItemRelationTypeFactory
// Assembly: Microsoft.Azure.Boards.WebApi.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: FC99C479-6852-4E74-BCA4-2660760F9D83
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Boards.WebApi.Common.dll

using Microsoft.TeamFoundation.WorkItemTracking.Common.Metadata;
using Microsoft.TeamFoundation.WorkItemTracking.Server;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using System.Collections.Generic;

namespace Microsoft.Azure.Boards.WebApi.Common.Converters
{
  public static class WorkItemRelationTypeFactory
  {
    public static WorkItemRelationType Create(
      WorkItemTrackingRequestContext witRequestContext,
      WorkItemLinkTypeEnd linkTypeEnd)
    {
      Dictionary<string, object> dictionary = new Dictionary<string, object>()
      {
        {
          "usage",
          (object) WorkItemRelationTypeFactory.WorkItemRelationTypeUsage.WorkItemLink
        },
        {
          "editable",
          (object) linkTypeEnd.LinkType.CanEdit
        },
        {
          "enabled",
          (object) linkTypeEnd.LinkType.Enabled
        },
        {
          "acyclic",
          (object) !linkTypeEnd.LinkType.IsCircular
        },
        {
          "directional",
          (object) linkTypeEnd.LinkType.IsDirectional
        },
        {
          "singleTarget",
          (object) !linkTypeEnd.LinkType.IsOneToMany
        },
        {
          "topology",
          (object) linkTypeEnd.LinkType.Topology
        },
        {
          "remote",
          (object) linkTypeEnd.LinkType.IsRemote
        }
      };
      if (linkTypeEnd.LinkType.IsDirectional)
      {
        dictionary["isForward"] = (object) linkTypeEnd.IsForwardEnd;
        dictionary["oppositeEndReferenceName"] = (object) linkTypeEnd.OppositeEnd.ReferenceName;
      }
      WorkItemRelationType itemRelationType = new WorkItemRelationType();
      itemRelationType.ReferenceName = WorkItemRelationTypeFactory.GetRelationReferenceName(linkTypeEnd);
      itemRelationType.Name = linkTypeEnd.Name;
      itemRelationType.Attributes = (IDictionary<string, object>) dictionary;
      itemRelationType.Url = WitUrlHelper.GetWorkItemRelationTypeUrl(witRequestContext, linkTypeEnd.ReferenceName);
      return itemRelationType;
    }

    public static WorkItemRelationType Create(
      WorkItemTrackingRequestContext witRequestContext,
      string resourceLinkReferenceName,
      string resourceLinkName)
    {
      WorkItemRelationType itemRelationType = new WorkItemRelationType();
      itemRelationType.ReferenceName = resourceLinkReferenceName;
      itemRelationType.Name = resourceLinkName;
      itemRelationType.Attributes = (IDictionary<string, object>) new Dictionary<string, object>()
      {
        {
          "usage",
          (object) WorkItemRelationTypeFactory.WorkItemRelationTypeUsage.ResourceLink
        },
        {
          "editable",
          (object) false
        },
        {
          "enabled",
          (object) true
        }
      };
      itemRelationType.Url = WitUrlHelper.GetWorkItemRelationTypeUrl(witRequestContext, resourceLinkReferenceName);
      return itemRelationType;
    }

    public static string GetRelationReferenceName(WorkItemLinkTypeEnd linkTypeEnd) => linkTypeEnd.ReferenceName;

    public enum WorkItemRelationTypeUsage
    {
      WorkItemLink,
      ResourceLink,
    }
  }
}
