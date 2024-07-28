// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Boards.WebApi.Common.Extensions.EnumExtensions
// Assembly: Microsoft.Azure.Boards.WebApi.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: FC99C479-6852-4E74-BCA4-2660760F9D83
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Boards.WebApi.Common.dll

using Microsoft.TeamFoundation.WorkItemTracking.Common;
using System;

namespace Microsoft.Azure.Boards.WebApi.Common.Extensions
{
  public static class EnumExtensions
  {
    public static Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.QueryType ToQueryType(
      this Microsoft.TeamFoundation.WorkItemTracking.Server.QueryType queryType)
    {
      if (EnumExtensions.IsOneHopQuery(queryType))
        return Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.QueryType.OneHop;
      return queryType == Microsoft.TeamFoundation.WorkItemTracking.Server.QueryType.WorkItems ? Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.QueryType.Flat : Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.QueryType.Tree;
    }

    public static Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.QueryResultType ToQueryResultType(
      this Microsoft.TeamFoundation.WorkItemTracking.Server.QueryResultType queryResultType)
    {
      if (queryResultType == Microsoft.TeamFoundation.WorkItemTracking.Server.QueryResultType.WorkItem)
        return Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.QueryResultType.WorkItem;
      if (queryResultType == Microsoft.TeamFoundation.WorkItemTracking.Server.QueryResultType.WorkItemLink)
        ;
      return Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.QueryResultType.WorkItemLink;
    }

    public static InternalFieldType ToInternalFieldType(this Microsoft.TeamFoundation.WorkItemTracking.ProcessDefinitions.WebApi.Models.FieldType fieldType) => (InternalFieldType) Enum.Parse(typeof (InternalFieldType), fieldType.ToString());

    public static InternalFieldType ToInternalFieldType(this Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.FieldType fieldType) => (InternalFieldType) Enum.Parse(typeof (InternalFieldType), fieldType.ToString());

    public static bool IsOneHopQuery(Microsoft.TeamFoundation.WorkItemTracking.Server.QueryType queryType)
    {
      switch (queryType)
      {
        case Microsoft.TeamFoundation.WorkItemTracking.Server.QueryType.LinksOneHopMustContain:
        case Microsoft.TeamFoundation.WorkItemTracking.Server.QueryType.LinksOneHopMayContain:
        case Microsoft.TeamFoundation.WorkItemTracking.Server.QueryType.LinksOneHopDoesNotContain:
          return true;
        default:
          return false;
      }
    }
  }
}
