// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.ServicingComponent7
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Collections.Generic;
using System.Data;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class ServicingComponent7 : ServicingComponent6
  {
    public List<ServicingJobDetail> QueryServicingJobDetails(
      IEnumerable<KeyValuePair<Guid, Guid>> hostIdJobIdCollection)
    {
      this.PrepareStoredProcedure("prc_QueryServicingJobDetails");
      this.BindKeyValuePairGuidGuidTable("@hostIdJobIdTable", hostIdJobIdCollection);
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), "prc_QueryServicingJobDetails", this.RequestContext);
      resultCollection.AddBinder<ServicingJobDetail>((ObjectBinder<ServicingJobDetail>) new ServicingJobDetailColumns());
      return resultCollection.GetCurrent<ServicingJobDetail>().Items;
    }
  }
}
