// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.FieldUsagesAdminDataBinder
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 815CF582-E66F-43C7-9B50-57E1C71BBC84
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccess;
using System.Data;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server
{
  internal class FieldUsagesAdminDataBinder : WorkItemTrackingObjectBinder<FieldUsagesAdminData>
  {
    private SqlColumnBinder FldUsageId = new SqlColumnBinder(nameof (FldUsageId));
    private SqlColumnBinder FDeleted = new SqlColumnBinder("fDeleted");
    private SqlColumnBinder ObjectId = new SqlColumnBinder(nameof (ObjectId));
    private SqlColumnBinder FldId = new SqlColumnBinder(nameof (FldId));
    private SqlColumnBinder DirectObjectId = new SqlColumnBinder(nameof (DirectObjectId));
    private SqlColumnBinder FOftenQueried = new SqlColumnBinder("fOftenQueried");
    private SqlColumnBinder FCore = new SqlColumnBinder("fCore");
    private SqlColumnBinder Cachestamp = new SqlColumnBinder("CacheStamp");
    private SqlColumnBinder FOftenQueriedAsText = new SqlColumnBinder("fOftenQueriedAsText");
    private SqlColumnBinder FSupportsTextQuery = new SqlColumnBinder("fSupportsTextQuery");

    public override FieldUsagesAdminData Bind(IDataReader reader) => new FieldUsagesAdminData()
    {
      FldUsageId = this.FldUsageId.GetInt32(reader),
      FDeleted = this.FDeleted.GetBoolean(reader),
      ObjectId = this.ObjectId.GetInt32(reader),
      FldId = this.FldId.GetInt32(reader),
      DirectObjectId = this.DirectObjectId.GetInt32(reader),
      FOftenQueried = this.FOftenQueried.GetBoolean(reader),
      FCore = this.FCore.GetBoolean(reader),
      FOftenQueriedAsText = this.FOftenQueriedAsText.GetBoolean(reader),
      FSupportsTextQuery = this.FSupportsTextQuery.GetBoolean(reader),
      CacheStamp = this.Cachestamp.GetNullableInt64(reader)
    };
  }
}
