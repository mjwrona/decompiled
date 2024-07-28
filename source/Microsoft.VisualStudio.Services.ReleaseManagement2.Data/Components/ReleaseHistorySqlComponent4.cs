// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Data.Components.ReleaseHistorySqlComponent4
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Data, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A69B0FB7-3028-4162-BA38-794D80D7A49A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Data.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Data.Components
{
  [SuppressMessage("Microsoft.Maintainability", "CA1501:AvoidExcessiveInheritance", Justification = "Versioning mechanism")]
  public class ReleaseHistorySqlComponent4 : ReleaseHistorySqlComponent3
  {
    public override IEnumerable<ReleaseRevision> GetHistory(
      Guid projectId,
      int releaseId,
      int environmentId,
      int attempt)
    {
      this.PrepareStoredProcedure("Release.prc_GetReleaseHistory", projectId);
      this.BindInt(nameof (releaseId), releaseId);
      this.BindInt(nameof (environmentId), environmentId);
      this.BindInt(nameof (attempt), attempt);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<ReleaseRevision>((ObjectBinder<ReleaseRevision>) this.GetReleaseHistoryBinder());
        return (IEnumerable<ReleaseRevision>) resultCollection.GetCurrent<ReleaseRevision>().Items;
      }
    }
  }
}
