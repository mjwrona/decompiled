// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.ServicingComponent12
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class ServicingComponent12 : ServicingComponent11
  {
    public override ServicingJobInfo GetServicingJobInfo(Guid hostId, Guid jobId)
    {
      this.PrepareStoredProcedure("prc_GetServicingJobInfo");
      this.BindGuid("@hostId", hostId);
      this.BindGuid("@jobId", jobId);
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), "prc_GetServicingJobInfo", this.RequestContext);
      resultCollection.AddBinder<ServicingJobInfo>((ObjectBinder<ServicingJobInfo>) new ServicingJobInfoColumns());
      return resultCollection.GetCurrent<ServicingJobInfo>().FirstOrDefault<ServicingJobInfo>();
    }

    public override List<ServicingJobInfo> GetServicingJobsInfo(Guid jobId)
    {
      this.PrepareStoredProcedure("prc_GetServicingJobInfo");
      this.BindGuid("@jobId", jobId);
      this.BindNullableGuid("@hostId", new Guid?());
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), "prc_GetServicingJobInfo", this.RequestContext);
      resultCollection.AddBinder<ServicingJobInfo>((ObjectBinder<ServicingJobInfo>) new ServicingJobInfoColumns());
      return resultCollection.GetCurrent<ServicingJobInfo>().Items;
    }

    public virtual ServicingOperation[] GetServicingOperations() => throw new NotImplementedException();
  }
}
