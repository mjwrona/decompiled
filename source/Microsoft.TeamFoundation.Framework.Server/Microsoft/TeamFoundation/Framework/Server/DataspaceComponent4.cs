// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.DataspaceComponent4
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Data;
using System.Linq;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class DataspaceComponent4 : DataspaceComponent3
  {
    public override Dataspace CreateSplitDataspace(
      int dataspaceId,
      Guid dataspaceIdentifier,
      string dataspaceCategory,
      int databaseId,
      DataspaceState dataspaceState)
    {
      this.PrepareStoredProcedure("prc_CreateSplitDataspace");
      this.BindInt("@dataspaceId", dataspaceId);
      this.BindGuid("@dataspaceIdentifier", dataspaceIdentifier);
      this.BindString("@dataspaceCategory", dataspaceCategory, 260, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
      this.BindInt("@databaseId", databaseId);
      this.BindByte("@initialState", (byte) dataspaceState);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<Dataspace>((ObjectBinder<Dataspace>) new DataspaceColumns());
        return resultCollection.GetCurrent<Dataspace>().SingleOrDefault<Dataspace>();
      }
    }
  }
}
