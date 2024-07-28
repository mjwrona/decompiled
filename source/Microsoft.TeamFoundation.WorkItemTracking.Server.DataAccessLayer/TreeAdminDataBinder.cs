// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.TreeAdminDataBinder
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 815CF582-E66F-43C7-9B50-57E1C71BBC84
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccess;
using System.Data;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server
{
  internal class TreeAdminDataBinder : WorkItemTrackingObjectBinder<TreeAdminData>
  {
    private SqlColumnBinder AreaId = new SqlColumnBinder(nameof (AreaId));
    private SqlColumnBinder fDeleted = new SqlColumnBinder(nameof (fDeleted));
    private SqlColumnBinder TypeId = new SqlColumnBinder(nameof (TypeId));
    private SqlColumnBinder Name = new SqlColumnBinder(nameof (Name));
    private SqlColumnBinder ParentId = new SqlColumnBinder(nameof (ParentId));
    private SqlColumnBinder fAdminOnly = new SqlColumnBinder(nameof (fAdminOnly));
    private SqlColumnBinder StructureType = new SqlColumnBinder(nameof (StructureType));
    private SqlColumnBinder Guid = new SqlColumnBinder("GUID");
    private SqlColumnBinder CacheStamp = new SqlColumnBinder(nameof (CacheStamp));

    public override TreeAdminData Bind(IDataReader reader) => new TreeAdminData()
    {
      AreaId = this.AreaId.GetInt32(reader),
      FDeleted = this.fDeleted.GetBoolean(reader),
      TypeId = this.TypeId.GetInt32(reader),
      Name = this.Name.GetString(reader, true),
      ParentId = this.ParentId.GetInt32(reader),
      FAdminOnly = this.fAdminOnly.GetBoolean(reader),
      StructureType = this.StructureType.GetInt32(reader),
      Guid = this.Guid.GetString(reader, true),
      CacheStamp = this.CacheStamp.GetNullableInt64(reader)
    };
  }
}
