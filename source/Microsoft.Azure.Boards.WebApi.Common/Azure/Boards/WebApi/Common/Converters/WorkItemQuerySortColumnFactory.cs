// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Boards.WebApi.Common.Converters.WorkItemQuerySortColumnFactory
// Assembly: Microsoft.Azure.Boards.WebApi.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: FC99C479-6852-4E74-BCA4-2660760F9D83
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Boards.WebApi.Common.dll

using Microsoft.TeamFoundation.WorkItemTracking.Server;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.Azure.Boards.WebApi.Common.Converters
{
  public class WorkItemQuerySortColumnFactory
  {
    public static WorkItemQuerySortColumn Create(
      WorkItemTrackingRequestContext witRequestContext,
      QuerySortField querySortField,
      ISecuredObject securedObject = null,
      Guid? projectId = null,
      bool excludeUrls = false)
    {
      return new WorkItemQuerySortColumn(securedObject)
      {
        Field = WorkItemFieldReferenceFactory.Create(witRequestContext, querySortField.Field, securedObject, projectId, includeUrl: !excludeUrls),
        Descending = querySortField.Descending
      };
    }

    public static IEnumerable<WorkItemQuerySortColumn> Create(
      WorkItemTrackingRequestContext witRequestContext,
      IEnumerable<QuerySortField> sortFields,
      ISecuredObject securedObject = null,
      Guid? projectId = null,
      bool excludeUrls = false)
    {
      IEnumerable<WorkItemQuerySortColumn> source = sortFields.Select<QuerySortField, WorkItemQuerySortColumn>((Func<QuerySortField, WorkItemQuerySortColumn>) (sf => WorkItemQuerySortColumnFactory.Create(witRequestContext, sf, securedObject, projectId, excludeUrls)));
      return source.Any<WorkItemQuerySortColumn>() ? source : (IEnumerable<WorkItemQuerySortColumn>) null;
    }
  }
}
