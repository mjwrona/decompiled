// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Agile.Web.Data.ScaledAgileViewComponent6
// Assembly: Microsoft.TeamFoundation.Agile.Web, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDD05019-96D4-4BDC-9BC2-86F688BB0A99
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Agile.Web.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Data;

namespace Microsoft.TeamFoundation.Agile.Web.Data
{
  internal class ScaledAgileViewComponent6 : ScaledAgileViewComponent5
  {
    internal override IEnumerable<ShallowScaledAgileViewRecord> GetViewDefinitions(
      Guid projectId,
      Guid ownerId = default (Guid))
    {
      this.PrepareStoredProcedure("prc_GetScaledAgileViews");
      this.BindDataspace(projectId);
      if (ownerId != new Guid())
        this.BindGuid("@ownerId", ownerId);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<ShallowScaledAgileViewRecord>((ObjectBinder<ShallowScaledAgileViewRecord>) this.GetViewDefinitionsRecordBinder());
        return (IEnumerable<ShallowScaledAgileViewRecord>) resultCollection.GetCurrent<ShallowScaledAgileViewRecord>().Items;
      }
    }
  }
}
