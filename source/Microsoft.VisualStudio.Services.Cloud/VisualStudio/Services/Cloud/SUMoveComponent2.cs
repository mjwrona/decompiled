// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.SUMoveComponent2
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Data;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Cloud
{
  public class SUMoveComponent2 : SUMoveComponent
  {
    public override ScaleUnitDataTransfer CreateScaleUnitDataTransfer(
      ScaleUnitDataTransfer suDataTransfer)
    {
      this.PrepareStoredProcedure("SUMove.prc_CreateScaleUnitDataTransfer");
      this.BindString("@sourceStorageAccount", suDataTransfer.SourceStorageAccount, 100, BindStringBehavior.NullToEmptyString, SqlDbType.VarChar);
      this.BindString("@targetStorageAccount", suDataTransfer.TargetStorageAccount, 100, BindStringBehavior.NullToEmptyString, SqlDbType.VarChar);
      this.BindString("@primaryId", suDataTransfer.PrimaryId, 100, BindStringBehavior.NullToEmptyString, SqlDbType.VarChar);
      this.BindString("@subsetId", suDataTransfer.SubsetId, 250, BindStringBehavior.NullToEmptyString, SqlDbType.VarChar);
      this.BindString("@itemType", suDataTransfer.ItemType, 10, BindStringBehavior.NullToEmptyString, SqlDbType.VarChar);
      this.BindNullableGuid("@jobThread", suDataTransfer.JobThread);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<ScaleUnitDataTransfer>((ObjectBinder<ScaleUnitDataTransfer>) new ScaleUnitDataTransferColumns());
        return resultCollection.GetCurrent<ScaleUnitDataTransfer>().Items.FirstOrDefault<ScaleUnitDataTransfer>();
      }
    }
  }
}
