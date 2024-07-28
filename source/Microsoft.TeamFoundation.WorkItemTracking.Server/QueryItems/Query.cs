// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.QueryItems.Query
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using System;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.QueryItems
{
  public class Query : QueryItem
  {
    internal Query(QueryItemEntry queryEntry)
      : base(queryEntry)
    {
      this.Wiql = queryEntry.Wiql;
      int? queryType = queryEntry.QueryType;
      this.QueryType = queryType.HasValue ? new Microsoft.TeamFoundation.WorkItemTracking.Server.QueryType?((Microsoft.TeamFoundation.WorkItemTracking.Server.QueryType) queryType.GetValueOrDefault()) : new Microsoft.TeamFoundation.WorkItemTracking.Server.QueryType?();
      this.LastExecutedById = queryEntry.LastExecutedById;
      this.LastExecutedDate = queryEntry.LastExecutedDate;
    }

    internal Query()
    {
    }

    public string Wiql { get; internal set; }

    public Microsoft.TeamFoundation.WorkItemTracking.Server.QueryType? QueryType { get; internal set; }

    public Guid? LastExecutedById { get; internal set; }

    public DateTime? LastExecutedDate { get; internal set; }

    public override object Clone()
    {
      Query query = new Query();
      query.DeepCopyFrom((QueryItem) this);
      return (object) query;
    }

    internal override void DeepCopyFrom(QueryItem queryItem)
    {
      base.DeepCopyFrom(queryItem);
      if (!(queryItem is Query query))
        return;
      this.Wiql = query.Wiql;
      this.QueryType = query.QueryType;
      this.LastExecutedById = query.LastExecutedById;
      this.LastExecutedDate = query.LastExecutedDate;
    }
  }
}
