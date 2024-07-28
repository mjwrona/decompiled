// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Data.Components.ReleaseSqlComponent5
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Data, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A69B0FB7-3028-4162-BA38-794D80D7A49A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Data.dll

using Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.DataAccess;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.DataBinders;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Data.Components
{
  public class ReleaseSqlComponent5 : ReleaseSqlComponent4
  {
    protected virtual void BindReleaseArtifactSourceTable(
      IEnumerable<PipelineArtifactSource> releaseArtifactSources)
    {
      this.BindReleaseArtifactSourceTable2(nameof (releaseArtifactSources), releaseArtifactSources);
    }

    public override KeyValuePair<int, string> GetReleaseDefinitionFolderPathAndId(
      Guid projectId,
      int releaseId)
    {
      this.PrepareStoredProcedure("Release.prc_GetDefinitionIdFromReleaseId", projectId);
      this.BindInt(nameof (releaseId), releaseId);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<int>((ObjectBinder<int>) new ReleaseDefinitionIdBinder());
        return new KeyValuePair<int, string>(resultCollection.GetCurrent<int>().Items.FirstOrDefault<int>(), (string) null);
      }
    }

    public override Release UpdateDraftRelease(Guid projectId, Release release, string comment)
    {
      if (release == null)
        throw new ArgumentNullException(nameof (release));
      this.PrepareStoredProcedure("Release.prc_Release_UpdateDraft", projectId);
      IEnumerable<PipelineArtifactSource> releaseArtifactSources = this.GetReleaseArtifactSources((IEnumerable<ArtifactSource>) release.LinkedArtifacts);
      this.BindReleaseTable3(nameof (release), release);
      this.BindReleaseEnvironmentTable5("releaseEnvironments", (IEnumerable<ReleaseEnvironment>) release.Environments);
      this.BindReleaseArtifactSourceTable(releaseArtifactSources);
      return this.GetReleaseObject(projectId);
    }

    public override Release AddRelease(Guid projectId, Release release, string comment)
    {
      if (release == null)
        throw new ArgumentNullException(nameof (release));
      this.EnsureReleaseNameIsNotEmpty(release);
      this.PrepareStoredProcedure("Release.prc_Release_Add", projectId);
      IEnumerable<PipelineArtifactSource> releaseArtifactSources = this.GetReleaseArtifactSources((IEnumerable<ArtifactSource>) release.LinkedArtifacts);
      this.BindReleaseTable3(nameof (release), release);
      this.BindReleaseEnvironmentTable5("releaseEnvironments", (IEnumerable<ReleaseEnvironment>) release.Environments);
      this.BindReleaseArtifactSourceTable(releaseArtifactSources);
      return this.GetReleaseObject(projectId);
    }

    [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "This is an override of a base class method.")]
    protected override ReleaseArtifactSourceBinder GetReleaseArtifactSourceBinder() => (ReleaseArtifactSourceBinder) new ReleaseArtifactSourceBinder2((ReleaseManagementSqlResourceComponentBase) this);

    [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "This is an override of a base class method.")]
    protected override ReleaseArtifactDefinitionBinder GetReleaseArtifactDefinitionBinder() => (ReleaseArtifactDefinitionBinder) new ReleaseArtifactDefinitionBinder2();

    [SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "These parameters are optional")]
    public override IEnumerable<Release> ListReleases(
      Guid projectId,
      string namePattern,
      int releaseDefinitionId,
      int definitionEnvironmentId,
      ReleaseStatus statusFilter,
      ReleaseEnvironmentStatus environmentStatusFilter,
      IEnumerable<Guid> createdByIds,
      DateTime? minCreatedTime,
      DateTime? maxCreatedTime,
      DateTime? maxModifiedTime,
      int maxReleases,
      ReleaseQueryOrder releaseQueryOrder,
      int releaseContinuationToken,
      bool includeEnvironments,
      bool includeArtifacts,
      bool includeApprovals,
      bool includeManualInterventions,
      string artifactTypeId,
      string sourceId,
      string artifactVersionId,
      string sourceBranchFilter,
      bool isDeleted,
      bool includeDeletedReleaseDefinitions,
      bool includeVariables = false,
      bool includeTags = false,
      IEnumerable<string> tagFilter = null,
      IEnumerable<int> releaseIds = null,
      string path = null)
    {
      this.PrepareStoredProcedure("Release.prc_Release_List", projectId);
      this.BindString(nameof (namePattern), namePattern, 256, BindStringBehavior.NullToEmptyString, SqlDbType.NVarChar);
      this.BindInt(nameof (releaseDefinitionId), releaseDefinitionId);
      this.BindByte(nameof (statusFilter), (byte) statusFilter);
      this.BindNullableDateTime(nameof (minCreatedTime), minCreatedTime);
      this.BindNullableDateTime(nameof (maxCreatedTime), maxCreatedTime);
      this.BindInt(nameof (maxReleases), maxReleases);
      this.BindByte("queryOrder", (byte) releaseQueryOrder);
      this.BindInt(nameof (releaseContinuationToken), releaseContinuationToken);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<Release>((ObjectBinder<Release>) this.GetReleaseBinder(projectId));
        resultCollection.AddBinder<ReleaseEnvironment>((ObjectBinder<ReleaseEnvironment>) this.GetReleaseEnvironmentBinder());
        resultCollection.AddBinder<ReleaseEnvironmentStep>((ObjectBinder<ReleaseEnvironmentStep>) this.GetReleaseEnvironmentStepBinder());
        resultCollection.AddBinder<PipelineArtifactSource>((ObjectBinder<PipelineArtifactSource>) this.GetReleaseArtifactSourceBinder());
        resultCollection.AddBinder<ReleaseArtifactDefinition>((ObjectBinder<ReleaseArtifactDefinition>) this.GetReleaseArtifactDefinitionBinder());
        List<Release> items1 = resultCollection.GetCurrent<Release>().Items;
        resultCollection.NextResult();
        List<ReleaseEnvironment> items2 = resultCollection.GetCurrent<ReleaseEnvironment>().Items;
        resultCollection.NextResult();
        List<ReleaseEnvironmentStep> items3 = resultCollection.GetCurrent<ReleaseEnvironmentStep>().Items;
        resultCollection.NextResult();
        List<PipelineArtifactSource> items4 = resultCollection.GetCurrent<PipelineArtifactSource>().Items;
        resultCollection.NextResult();
        List<ReleaseArtifactDefinition> items5 = resultCollection.GetCurrent<ReleaseArtifactDefinition>().Items;
        foreach (Release release in items1)
        {
          int releaseId = release.Id;
          ReleaseSqlComponent.StitchReleaseObject(release, items2.Where<ReleaseEnvironment>((System.Func<ReleaseEnvironment, bool>) (env => env.ReleaseId == releaseId)), items3.Where<ReleaseEnvironmentStep>((System.Func<ReleaseEnvironmentStep, bool>) (step => step.ReleaseId == releaseId)), items4.Where<PipelineArtifactSource>((System.Func<PipelineArtifactSource, bool>) (source => source.ReleaseId == releaseId)), (IEnumerable<ReleaseArtifactDefinition>) items5, (IEnumerable<ReleaseDeployPhase>) null);
        }
        return (IEnumerable<Release>) items1;
      }
    }

    protected virtual void EnsureReleaseNameIsNotEmpty(Release release)
    {
      if (release == null)
        throw new ArgumentNullException(nameof (release));
      if (!string.IsNullOrEmpty(release.Name))
        return;
      release.Name = string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.ReleaseNameFormatForBuildTriggeredRelease, (object) DateTime.Now.ToString("yyyyMMddHHmmss", (IFormatProvider) CultureInfo.InvariantCulture));
    }
  }
}
