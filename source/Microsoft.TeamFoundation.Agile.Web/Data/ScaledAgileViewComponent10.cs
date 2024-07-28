// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Agile.Web.Data.ScaledAgileViewComponent10
// Assembly: Microsoft.TeamFoundation.Agile.Web, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDD05019-96D4-4BDC-9BC2-86F688BB0A99
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Agile.Web.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;

namespace Microsoft.TeamFoundation.Agile.Web.Data
{
  internal class ScaledAgileViewComponent10 : ScaledAgileViewComponent9
  {
    internal override void UpdateLastAccessed(Guid projectId, Guid viewId)
    {
      this.PrepareStoredProcedure("prc_UpdateScaledAgileViewLastAccessed");
      this.BindDataspace(projectId);
      this.BindGuid("@viewId", viewId);
      this.ExecuteScalar();
    }

    protected override void AddBindersForGetView(ResultCollection rc)
    {
      rc.AddBinder<FullScaledAgileViewRecord2>((ObjectBinder<FullScaledAgileViewRecord2>) new FullScaledAgileViewRecordBinder3());
      rc.AddBinder<ScaledAgileViewPropertyRecord2>((ObjectBinder<ScaledAgileViewPropertyRecord2>) new ScaledAgileViewPropertyRecordBinder2());
    }

    protected override ShallowScaledAgileViewRecordBinder GetViewDefinitionsRecordBinder() => (ShallowScaledAgileViewRecordBinder) new ShallowScaledAgileViewRecordBinder4();
  }
}
