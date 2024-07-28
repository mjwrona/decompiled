// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.DataspaceComponent2
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class DataspaceComponent2 : DataspaceComponent
  {
    public override List<Dataspace> CreateDataspaces(
      Guid dataspaceidentifier,
      IEnumerable<KeyValuePair<string, int>> categoryDatabaseMapping,
      DataspaceState initialState)
    {
      this.PrepareStoredProcedure("prc_CreateDataspace");
      this.BindGuid("@dataspaceIdentifier", dataspaceidentifier);
      this.BindKeyValuePairStringInt32Table("@categoryDatabaseIds", categoryDatabaseMapping);
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext);
      resultCollection.AddBinder<Dataspace>((ObjectBinder<Dataspace>) new DataspaceColumns());
      return resultCollection.GetCurrent<Dataspace>().Items;
    }

    public override void DeleteDataspace(Guid dataspaceidentifier, string category)
    {
      this.PrepareStoredProcedure("prc_DeleteDataspace");
      this.BindGuid("@dataspaceIdentifier", dataspaceidentifier);
      this.BindString("@category", category, 260, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
      this.ExecuteNonQuery();
    }

    public override List<Dataspace> QueryDataspaces()
    {
      this.PrepareStoredProcedure("prc_QueryDataspace");
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext);
      resultCollection.AddBinder<Dataspace>((ObjectBinder<Dataspace>) new DataspaceColumns());
      return resultCollection.GetCurrent<Dataspace>().Items;
    }

    public override Dataspace QueryDataspace(string dataspaceCategory, Guid dataspaceIdentifier)
    {
      this.PrepareStoredProcedure("prc_QueryDataspace");
      this.BindString("@dataspaceCategory", dataspaceCategory, 260, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
      this.BindGuid("@dataspaceIdentifier", dataspaceIdentifier);
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext);
      resultCollection.AddBinder<Dataspace>((ObjectBinder<Dataspace>) new DataspaceColumns());
      return resultCollection.GetCurrent<Dataspace>().Items.FirstOrDefault<Dataspace>();
    }

    public override Dataspace QueryDataspace(int dataspaceId)
    {
      this.PrepareStoredProcedure("prc_QueryDataspace");
      this.BindInt("@dataspaceId", dataspaceId);
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext);
      resultCollection.AddBinder<Dataspace>((ObjectBinder<Dataspace>) new DataspaceColumns());
      return resultCollection.GetCurrent<Dataspace>().Items.FirstOrDefault<Dataspace>();
    }

    public override void SoftDeleteDataspace(Guid dataspaceidentifier, string category)
    {
      this.PrepareStoredProcedure("prc_SoftDeleteDataspace");
      this.BindGuid("@dataspaceIdentifier", dataspaceidentifier);
      this.BindString("@category", category, 260, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
      this.ExecuteNonQuery();
    }

    public virtual List<Dataspace> UpdateDataspaceDb(
      int newDatabaseId,
      Guid? dataspaceIdentifier,
      string category)
    {
      this.PrepareStoredProcedure("prc_UpdateDataspaceDb");
      this.BindInt("@databaseId", newDatabaseId);
      if (dataspaceIdentifier.HasValue)
        this.BindGuid("@dataspaceIdentifier", dataspaceIdentifier.Value);
      if (category != null)
        this.BindString("@category", category, 260, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
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
      return newDatabaseId.HasValue ? this.UpdateDataspaceDb(newDatabaseId.Value, dataspaceIdentifier, dataspaceCategory) : new List<Dataspace>(0);
    }
  }
}
