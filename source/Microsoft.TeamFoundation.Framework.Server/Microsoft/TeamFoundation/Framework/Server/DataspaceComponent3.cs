// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.DataspaceComponent3
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Collections.Generic;
using System.Data;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class DataspaceComponent3 : DataspaceComponent2
  {
    public override List<Dataspace> CreateDataspaces(
      Guid dataspaceidentifier,
      IEnumerable<KeyValuePair<string, int>> categoryDatabaseMapping,
      DataspaceState initialState)
    {
      this.PrepareStoredProcedure("prc_CreateDataspace");
      this.BindGuid("@dataspaceIdentifier", dataspaceidentifier);
      this.BindKeyValuePairStringInt32Table("@categoryDatabaseIds", categoryDatabaseMapping);
      this.BindByte("@initialState", (byte) initialState);
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext);
      resultCollection.AddBinder<Dataspace>((ObjectBinder<Dataspace>) new DataspaceColumns());
      return resultCollection.GetCurrent<Dataspace>().Items;
    }

    public override List<Dataspace> UpdateDataspaces(
      string dataspaceCategory,
      Guid? dataspaceIdentifier,
      int? newDatabaseId,
      DataspaceState? newDataspaceState)
    {
      this.PrepareStoredProcedure("prc_UpdateDataspaces");
      if (dataspaceCategory != null)
        this.BindString("@dataspaceCategory", dataspaceCategory, 260, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
      if (dataspaceIdentifier.HasValue)
        this.BindGuid("@dataspaceIdentifier", dataspaceIdentifier.Value);
      if (newDatabaseId.HasValue)
        this.BindInt("@databaseId", newDatabaseId.Value);
      if (newDataspaceState.HasValue)
        this.BindByte("@dataspaceState", (byte) newDataspaceState.Value);
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext);
      resultCollection.AddBinder<Dataspace>((ObjectBinder<Dataspace>) new DataspaceColumns());
      return resultCollection.GetCurrent<Dataspace>().Items;
    }
  }
}
