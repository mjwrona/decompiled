// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Data.Components.ReleaseSqlComponent38
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Data, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A69B0FB7-3028-4162-BA38-794D80D7A49A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Data.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Data.Components
{
  [SuppressMessage("Microsoft.Maintainability", "CA1501:AvoidExcessiveInheritance", Justification = "Versioning mechanism")]
  public class ReleaseSqlComponent38 : ReleaseSqlComponent37
  {
    public override IEnumerable<Release> ListHardDeleteReleaseCandidates(
      Guid projectId,
      DateTime? maxModifiedTime,
      int maxReleases,
      int continuationToken)
    {
      this.PrepareStoredProcedure("Release.prc_QueryHardDeleteReleaseCandidates", projectId);
      this.BindMaxModifiedTime(nameof (maxModifiedTime), maxModifiedTime);
      if (maxReleases > 0)
        this.BindInt(nameof (maxReleases), maxReleases);
      this.BindInt(nameof (continuationToken), continuationToken);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<Release>((ObjectBinder<Release>) this.GetReleaseBinder(projectId));
        return (IEnumerable<Release>) resultCollection.GetCurrent<Release>().Items;
      }
    }

    public override ReleaseDeployPhase GetReleaseDeployPhase(
      Guid projectId,
      int releaseId,
      int releaseEnvironmentId,
      int releaseDeployPhaseId,
      Guid? planId)
    {
      return !planId.HasValue ? (ReleaseDeployPhase) null : this.GetReleaseEnvironmentData(projectId, releaseId, releaseEnvironmentId, true, false, false).Environment.ReleaseDeployPhases.First<ReleaseDeployPhase>((System.Func<ReleaseDeployPhase, bool>) (p =>
      {
        if (p.Id == releaseDeployPhaseId)
          return true;
        Guid? runPlanId = p.RunPlanId;
        Guid guid = planId.Value;
        if (!runPlanId.HasValue)
          return false;
        return !runPlanId.HasValue || runPlanId.GetValueOrDefault() == guid;
      }));
    }
  }
}
