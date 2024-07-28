// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.DalSqlResourceComponent22
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 815CF582-E66F-43C7-9B50-57E1C71BBC84
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Data;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server
{
  internal class DalSqlResourceComponent22 : DalSqlResourceComponent21
  {
    public override void GetWorkItemLinkChanges(
      Guid? projectId,
      IEnumerable<string> types,
      IEnumerable<string> linkTypes,
      long rowVersion,
      bool bypassPermissions,
      int batchSize,
      DateTime? createdDateWatermark,
      DateTime? removedDateWatermark,
      int uncommittedChangesLookbackWindowInSeconds,
      out ResultCollection rc)
    {
      int dataspaceId = !projectId.HasValue || !(projectId.Value != Guid.Empty) ? 0 : this.GetDataspaceId(projectId.Value, "WorkItem");
      this.PrepareStoredProcedure(nameof (GetWorkItemLinkChanges));
      this.BindNullableInt("@dataspaceId", dataspaceId, 0);
      this.BindLong("@rowVersion", rowVersion);
      this.BindNullableDateTime("@createdDateWatermark", createdDateWatermark);
      this.BindNullableDateTime("@removedDateWatermark", removedDateWatermark);
      this.BindInt("@batchSize", batchSize);
      this.BindBoolean("@bypassPermissions", bypassPermissions);
      this.BindStringTable("@types", types);
      this.BindStringTable("@linkTypes", linkTypes);
      IDataReader reader = this.ExecuteReader();
      PermissionCheckHelper helper = bypassPermissions ? (PermissionCheckHelper) null : new PermissionCheckHelper(this.RequestContext);
      rc = new ResultCollection(reader, this.ProcedureName, this.RequestContext);
      rc.AddBinder<WorkItemLinkChange>((ObjectBinder<WorkItemLinkChange>) this.GetWorkItemLinkChangeBinder(helper));
    }

    public override AdminMetadata GetMinimumAdminMetaData(
      int clientVersion,
      long treeCacheStamp,
      long fieldsCacheStamp,
      long fieldUsagesCacheStamp)
    {
      this.PrepareStoredProcedure("GetMinimumAdminData");
      this.BindLong("@clientVersion", (long) clientVersion);
      this.BindLong("@treeCacheStamp", treeCacheStamp);
      this.BindLong("@fieldsCacheStamp", fieldsCacheStamp);
      this.BindLong("@fieldUsagesCacheStamp", fieldsCacheStamp);
      AdminMetadata minimumAdminMetaData = new AdminMetadata();
      using (ResultCollection resultCollection = new ResultCollection(this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<TreeAdminData>((ObjectBinder<TreeAdminData>) new TreeAdminDataBinder());
        resultCollection.AddBinder<FieldAdminData>((ObjectBinder<FieldAdminData>) new FieldAdminDataBinder());
        resultCollection.AddBinder<FieldUsagesAdminData>((ObjectBinder<FieldUsagesAdminData>) new FieldUsagesAdminDataBinder());
        minimumAdminMetaData.TreeAdminData = (IReadOnlyCollection<TreeAdminData>) resultCollection.GetCurrent<TreeAdminData>().Items;
        if (resultCollection.TryNextResult())
          minimumAdminMetaData.FieldAdminData = (IReadOnlyCollection<FieldAdminData>) resultCollection.GetCurrent<FieldAdminData>().Items;
        if (resultCollection.TryNextResult())
          minimumAdminMetaData.FieldUsagesAdminData = (IReadOnlyCollection<FieldUsagesAdminData>) resultCollection.GetCurrent<FieldUsagesAdminData>().Items;
      }
      return minimumAdminMetaData;
    }
  }
}
