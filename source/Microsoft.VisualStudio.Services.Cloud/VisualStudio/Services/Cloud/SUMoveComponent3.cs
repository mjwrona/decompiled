// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.SUMoveComponent3
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Collections.Generic;
using System.Data;

namespace Microsoft.VisualStudio.Services.Cloud
{
  public class SUMoveComponent3 : SUMoveComponent2
  {
    public virtual List<ScaleUnitDataTransfer> GetAllTableRepresentativeEntries()
    {
      this.PrepareStoredProcedure("SUMove.prc_GetAllTableRepresentativeEntries");
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<ScaleUnitDataTransfer>((ObjectBinder<ScaleUnitDataTransfer>) new ScaleUnitDataTransferColumns());
        return resultCollection.GetCurrent<ScaleUnitDataTransfer>().Items;
      }
    }
  }
}
