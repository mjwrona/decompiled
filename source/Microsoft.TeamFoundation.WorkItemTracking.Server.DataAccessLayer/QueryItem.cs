// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.QueryItem
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 815CF582-E66F-43C7-9B50-57E1C71BBC84
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer.dll

using Microsoft.TeamFoundation.WorkItemTracking.Server.QueryItems;
using System;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server
{
  public struct QueryItem
  {
    public Guid Id;
    public Guid ParentId;
    public string Name;
    public string QueryText;

    public bool IsFolder => string.IsNullOrEmpty(this.QueryText);

    public static QueryItem Create(Microsoft.TeamFoundation.WorkItemTracking.Server.QueryItems.QueryItem queryItem)
    {
      Query query = queryItem as Query;
      return new QueryItem()
      {
        Id = queryItem.Id,
        ParentId = queryItem.ParentId,
        Name = queryItem.Name,
        QueryText = query?.Wiql
      };
    }
  }
}
