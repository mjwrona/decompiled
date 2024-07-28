// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Data.Components.ReleaseSqlComponent6
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Data, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A69B0FB7-3028-4162-BA38-794D80D7A49A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Data.dll

using Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.DataAccess;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Data.Components
{
  public class ReleaseSqlComponent6 : ReleaseSqlComponent5
  {
    public override Release UpdateReleaseAndStepsStatus(
      Guid projectId,
      Release release,
      ReleaseStatus releaseStatus,
      ReleaseEnvironmentStepStatus statusFrom,
      ReleaseEnvironmentStepStatus statusTo,
      Guid modifiedBy)
    {
      if (release == null)
        throw new ArgumentNullException(nameof (release));
      this.PrepareStoredProcedure("Release.prc_UpdateReleaseAndStepsStatus", projectId);
      this.BindInt("releaseId", release.Id);
      this.BindByte(nameof (releaseStatus), (byte) releaseStatus);
      this.BindByte(nameof (statusFrom), (byte) statusFrom);
      this.BindByte(nameof (statusTo), (byte) statusTo);
      return this.GetReleaseObject(projectId);
    }

    public override Release AddRelease(Guid projectId, Release release, string comment)
    {
      if (release == null)
        throw new ArgumentNullException(nameof (release));
      if (release.Status == ReleaseStatus.Draft)
        release.Name = release.PartiallyExpandedReleaseNameFormat;
      this.EnsureReleaseNameIsNotEmpty(release);
      this.PrepareStoredProcedure("Release.prc_Release_Add", projectId);
      IEnumerable<PipelineArtifactSource> releaseArtifactSources = this.GetReleaseArtifactSources((IEnumerable<ArtifactSource>) release.LinkedArtifacts);
      this.BindReleaseTable(nameof (release), release);
      this.BindReleaseEnvironmentTable5("releaseEnvironments", (IEnumerable<ReleaseEnvironment>) release.Environments);
      this.BindReleaseArtifactSourceTable(releaseArtifactSources);
      return this.GetReleaseObject(projectId);
    }

    public override Release UpdateDraftRelease(Guid projectId, Release release, string comment)
    {
      if (release == null)
        throw new ArgumentNullException(nameof (release));
      this.PrepareStoredProcedure("Release.prc_Release_UpdateDraft", projectId);
      IEnumerable<PipelineArtifactSource> releaseArtifactSources = this.GetReleaseArtifactSources((IEnumerable<ArtifactSource>) release.LinkedArtifacts);
      this.BindReleaseTable(nameof (release), release);
      this.BindReleaseEnvironmentTable5("releaseEnvironments", (IEnumerable<ReleaseEnvironment>) release.Environments);
      this.BindReleaseArtifactSourceTable(releaseArtifactSources);
      return this.GetReleaseObject(projectId);
    }

    protected override void EnsureReleaseNameIsNotEmpty(Release release)
    {
      if (release == null)
        throw new ArgumentNullException(nameof (release));
      if (!string.IsNullOrEmpty(release.Name))
        return;
      release.Name = "Release-$$REV1$$";
    }
  }
}
