// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Data.Components.ReleaseSqlComponent7
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Data, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A69B0FB7-3028-4162-BA38-794D80D7A49A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Data.dll

using Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts;
using Microsoft.VisualStudio.Services.FormInput;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.DataAccess;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Utilities;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Data.Components
{
  [SuppressMessage("Microsoft.Maintainability", "CA1501:AvoidExcessiveInheritance", Justification = "This is our versioning mechanism")]
  public class ReleaseSqlComponent7 : ReleaseSqlComponent6
  {
    protected override void BindReleaseArtifactSourceTable(
      IEnumerable<PipelineArtifactSource> releaseArtifactSources)
    {
      this.BindReleaseArtifactSourceTable3(nameof (releaseArtifactSources), releaseArtifactSources);
    }

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
      this.BindGuid(nameof (modifiedBy), modifiedBy);
      return this.GetReleaseObject(projectId);
    }

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
      this.BindGuid(nameof (modifiedBy), modifiedBy);
      if (status.HasValue)
        this.BindByte("releaseStatus", (byte) status.Value);
      return this.GetReleaseObject(projectId);
    }

    public override void UpdateReleaseArtifactSources(
      Guid projectId,
      int releaseId,
      IEnumerable<PipelineArtifactSource> artifactSources)
    {
      if (artifactSources == null)
        return;
      this.PrepareStoredProcedure("Release.prc_UpdateReleaseArtifactSource", projectId);
      this.BindInt(nameof (releaseId), releaseId);
      foreach (PipelineArtifactSource artifactSource in artifactSources)
      {
        Dictionary<string, InputValue> targetSourceData = new Dictionary<string, InputValue>();
        ArtifactSourceDataUtility.CompressSourceData((IDictionary<string, InputValue>) artifactSource.SourceData, (IDictionary<string, InputValue>) targetSourceData);
        artifactSource.SourceData = targetSourceData;
      }
      this.BindReleaseArtifactSourceTable(artifactSources);
      this.ExecuteNonQuery();
    }
  }
}
