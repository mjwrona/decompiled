// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.ServicingComponent6
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Collections.Generic;
using System.Data;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class ServicingComponent6 : ServicingComponent5
  {
    public override List<string> QueryServicingOperationNames()
    {
      this.PrepareStoredProcedure("prc_QueryServicingOperationNames");
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), "prc_QueryServicingOperationNames", this.RequestContext);
      resultCollection.AddBinder<string>((ObjectBinder<string>) new ServicingOperationNameColumns());
      return resultCollection.GetCurrent<string>().Items;
    }

    public override List<ServicingJobDetail> QueryServicingJobDetails(
      Guid hostId,
      string operationClass)
    {
      this.PrepareStoredProcedure("prc_QueryServicingJobDetailsByHostId");
      this.BindGuid("@hostId", hostId);
      this.BindString("@operationClass", operationClass, 64, true, SqlDbType.VarChar);
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), "prc_QueryServicingJobDetailsByHostId", this.RequestContext);
      resultCollection.AddBinder<ServicingJobDetail>((ObjectBinder<ServicingJobDetail>) new ServicingJobDetailColumns());
      return resultCollection.GetCurrent<ServicingJobDetail>().Items;
    }
  }
}
