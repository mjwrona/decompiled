// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Server.VersionedItemComponent29
// Assembly: Microsoft.TeamFoundation.VersionControl.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C5D9EE74-8805-4E00-959F-39760D2358B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.VersionControl.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Data;
using System.Linq;

namespace Microsoft.TeamFoundation.VersionControl.Server
{
  internal class VersionedItemComponent29 : VersionedItemComponent28
  {
    public override TfvcFileStats QueryTfvcFileStats(ItemSpec scopePath)
    {
      this.PrepareStoredProcedure("Tfvc.prc_GetFileStats", 3600);
      this.BindServerItem("@scopeServerItem", this.BestEffortConvertPathPairToPathWithProjectGuid(scopePath.ItemPathPair), false);
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.MaxItemsPerRequest, this.ProcedureName, this.m_resultExHandler, this.RequestContext);
      resultCollection.AddBinder<TfvcFileStats>((ObjectBinder<TfvcFileStats>) new QueryTfvcFileStatsColumns());
      return resultCollection.GetCurrent<TfvcFileStats>().Items.Single<TfvcFileStats>();
    }
  }
}
