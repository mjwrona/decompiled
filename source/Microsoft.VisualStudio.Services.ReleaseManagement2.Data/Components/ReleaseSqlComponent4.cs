// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Data.Components.ReleaseSqlComponent4
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Data, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A69B0FB7-3028-4162-BA38-794D80D7A49A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Data.dll

using Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.DataAccess;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.DataBinders;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Data.Components
{
  public class ReleaseSqlComponent4 : ReleaseSqlComponent
  {
    public override Release UpdateDraftRelease(Guid projectId, Release release, string comment)
    {
      if (release == null)
        throw new ArgumentNullException(nameof (release));
      this.PrepareStoredProcedure("Release.prc_Release_UpdateDraft", projectId);
      IEnumerable<PipelineArtifactSource> releaseArtifactSources = this.GetReleaseArtifactSources((IEnumerable<ArtifactSource>) release.LinkedArtifacts);
      this.BindReleaseTable3(nameof (release), release);
      this.BindReleaseEnvironmentTable5("releaseEnvironments", (IEnumerable<ReleaseEnvironment>) release.Environments);
      this.BindReleaseArtifactSourceTable("releaseArtifactSources", releaseArtifactSources);
      return this.GetReleaseObject(projectId);
    }

    public override Release AddRelease(Guid projectId, Release release, string comment)
    {
      if (release == null)
        throw new ArgumentNullException(nameof (release));
      this.PrepareStoredProcedure("Release.prc_Release_Add", projectId);
      IEnumerable<PipelineArtifactSource> releaseArtifactSources = this.GetReleaseArtifactSources((IEnumerable<ArtifactSource>) release.LinkedArtifacts);
      this.BindReleaseTable3(nameof (release), release);
      this.BindReleaseEnvironmentTable5("releaseEnvironments", (IEnumerable<ReleaseEnvironment>) release.Environments);
      this.BindReleaseArtifactSourceTable("releaseArtifactSources", releaseArtifactSources);
      return this.GetReleaseObject(projectId);
    }

    [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "This is an override of a base class method.")]
    protected override ReleaseArtifactDefinitionBinder GetReleaseArtifactDefinitionBinder() => (ReleaseArtifactDefinitionBinder) new ReleaseArtifactDefinitionBinder2();

    [SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "These parameters are optional")]
    public override Release PatchRelease(
      Guid projectId,
      int releaseId,
      Guid modifiedBy,
      IList<ReleaseEnvironment> releaseEnvironments,
      string comment,
      ReleaseStatus? status = null,
      bool? keepForever = null)
    {
      this.PrepareStoredProcedure("Release.prc_UpdateReleaseStatus", projectId);
      this.BindInt(nameof (releaseId), releaseId);
      if (status.HasValue)
        this.BindByte("releaseStatus", (byte) status.Value);
      return this.GetReleaseObject(projectId);
    }
  }
}
