// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.TestManagement.QueryAdapter
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.TestManagement, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2E4165D5-898A-42D9-B816-9FABF135E4DA
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.TestManagement.dll

using Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Models;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Server.WebAccess.TestManagement
{
  public abstract class QueryAdapter
  {
    public abstract IEnumerable<QueryField> GetFields();

    public abstract IEnumerable<QueryField> GetQueryableFields();

    public abstract QueryField TryGetFieldByDisplayName(string displayName);

    public abstract QueryResultModel ExecuteQuery(QueryModel query);

    public abstract QueryColumnInfoModel GetDefaultColumns();

    public abstract FilterModel GetDefaultFilter();

    public abstract IEnumerable<QueryResultDataRowModel> FetchDataRowsFromIds(
      IEnumerable<string> Ids,
      IEnumerable<QueryDisplayColumn> columns);

    public abstract List<string> GetQueryResultItemIds(QueryModel query);

    public abstract FilterModel GetTestRunTitleFilter();

    public abstract QueryColumnInfoModel GetTitleColumn();
  }
}
