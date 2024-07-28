// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.FieldUsageRecordBinder2
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 815CF582-E66F-43C7-9B50-57E1C71BBC84
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer.dll

using Microsoft.TeamFoundation.Framework.Server;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server
{
  internal class FieldUsageRecordBinder2 : FieldUsageRecordBinder
  {
    public FieldUsageRecordBinder2()
    {
      this.FieldIdColumn = new SqlColumnBinder("FieldId");
      this.ObjectIdColumn = new SqlColumnBinder("ObjectId");
      this.DirectObjectIdColumn = new SqlColumnBinder("DirectObjectId");
      this.OftenQueriedColumn = new SqlColumnBinder("IsOftenQueried");
      this.SupportsTextQueryColumn = new SqlColumnBinder("SupportsTextQuery");
      this.CoreColumn = new SqlColumnBinder("IsCore");
    }
  }
}
