// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Data.Components.ReleaseSqlComponent34
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Data, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A69B0FB7-3028-4162-BA38-794D80D7A49A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Data.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Data.Components
{
  [SuppressMessage("Microsoft.Maintainability", "CA1501:AvoidExcessiveInheritance", Justification = "Versioning mechanism")]
  public class ReleaseSqlComponent34 : ReleaseSqlComponent33
  {
    [SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "Defaults are required.")]
    [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "Defaults are required.")]
    [SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = "Versioning mechanism")]
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
      if (projectId != Guid.Empty && this.IsQueryingByDefinitionId(namePattern, releaseDefinitionId, definitionEnvironmentId, statusFilter, environmentStatusFilter, createdByIds, minCreatedTime, maxCreatedTime, maxModifiedTime, maxReleases, releaseQueryOrder, releaseContinuationToken, artifactTypeId, sourceId, artifactVersionId, sourceBranchFilter, tagFilter, releaseIds, isDeleted, includeDeletedReleaseDefinitions, includeEnvironments, includeArtifacts, includeApprovals, includeManualInterventions, includeVariables, includeTags))
        this.PrepareQueryReleasesByDefinitionIdStoredProcedure(projectId, releaseDefinitionId, statusFilter, maxReleases, releaseContinuationToken, includeEnvironments, includeArtifacts, includeApprovals, includeManualInterventions, sourceBranchFilter, isDeleted, includeVariables, includeTags);
      else
        this.PrepareQueryReleasesStoredProcedure(projectId, namePattern, releaseDefinitionId, definitionEnvironmentId, statusFilter, environmentStatusFilter, createdByIds, minCreatedTime, maxCreatedTime, maxModifiedTime, maxReleases, releaseQueryOrder, releaseContinuationToken, includeEnvironments, includeArtifacts, includeApprovals, includeManualInterventions, artifactTypeId, sourceId, artifactVersionId, sourceBranchFilter, isDeleted, includeDeletedReleaseDefinitions, includeVariables, includeTags, tagFilter, releaseIds, path);
      return this.GetReleaseObjects(projectId, includeEnvironments, includeManualInterventions, includeApprovals, includeArtifacts, includeTags);
    }

    private void PrepareQueryReleasesByDefinitionIdStoredProcedure(
      Guid projectId,
      int releaseDefinitionId,
      ReleaseStatus statusFilter,
      int maxReleases,
      int releaseContinuationToken,
      bool includeEnvironments,
      bool includeArtifacts,
      bool includeApprovals,
      bool includeManualInterventions,
      string sourceBranchFilter,
      bool isDeleted,
      bool includeVariables,
      bool includeTags)
    {
      this.PrepareStoredProcedure("Release.prc_QueryReleasesByDefinitionId", projectId);
      this.BindInt(nameof (releaseDefinitionId), releaseDefinitionId);
      this.BindByte(nameof (statusFilter), (byte) statusFilter);
      this.BindString(nameof (sourceBranchFilter), sourceBranchFilter, 256, BindStringBehavior.NullToEmptyString, SqlDbType.NVarChar);
      this.BindIsDeleted(nameof (isDeleted), isDeleted);
      this.BindInt(nameof (maxReleases), maxReleases);
      this.BindInt(nameof (releaseContinuationToken), releaseContinuationToken);
      this.BindBoolean(nameof (includeEnvironments), includeEnvironments);
      this.BindBoolean(nameof (includeArtifacts), includeArtifacts);
      this.BindBoolean(nameof (includeApprovals), includeApprovals);
      this.BindBoolean(nameof (includeManualInterventions), includeManualInterventions);
      this.BindIncludeVariables(includeVariables);
      this.BindBoolean(nameof (includeTags), includeTags);
    }

    protected virtual bool IsQueryingByDefinitionId(
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
      string artifactTypeId,
      string sourceId,
      string artifactVersionId,
      string sourceBranchFilter,
      IEnumerable<string> tagFilter,
      IEnumerable<int> releaseIds,
      bool isDeleted,
      bool includeDeletedReleaseDefinitions,
      bool includeEnvironments,
      bool includeArtifacts,
      bool includeApprovals,
      bool includeManualInterventions,
      bool includeVariables,
      bool includeTags)
    {
      return releaseDefinitionId > 0 && releaseQueryOrder == ReleaseQueryOrder.IdDescending && string.IsNullOrWhiteSpace(namePattern) && definitionEnvironmentId == 0 && environmentStatusFilter == ReleaseEnvironmentStatus.Undefined && createdByIds.IsNullOrEmpty<Guid>() && !minCreatedTime.HasValue && !maxCreatedTime.HasValue && !maxModifiedTime.HasValue && string.IsNullOrWhiteSpace(artifactTypeId) && string.IsNullOrWhiteSpace(sourceId) && string.IsNullOrWhiteSpace(artifactVersionId) && !includeDeletedReleaseDefinitions && tagFilter.IsNullOrEmpty<string>() && releaseIds.IsNullOrEmpty<int>();
    }

    protected virtual void PrepareQueryReleasesStoredProcedure(
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
      bool includeVariables,
      bool includeTags,
      IEnumerable<string> tagFilter,
      IEnumerable<int> releaseIds,
      string path)
    {
      if (projectId.Equals(Guid.Empty))
        this.PrepareStoredProcedure("Release.prc_QueryReleases");
      else
        this.PrepareStoredProcedure("Release.prc_QueryReleases", projectId);
      this.BindString(nameof (namePattern), namePattern, 256, BindStringBehavior.NullToEmptyString, SqlDbType.NVarChar);
      this.BindInt(nameof (releaseDefinitionId), releaseDefinitionId);
      this.BindInt(nameof (definitionEnvironmentId), definitionEnvironmentId);
      this.BindByte(nameof (statusFilter), (byte) statusFilter);
      this.BindEnvironmentStatusFilter(environmentStatusFilter);
      this.BindGuidTable("createdByIdTable", createdByIds);
      this.BindNullableDateTime(nameof (minCreatedTime), minCreatedTime);
      this.BindNullableDateTime(nameof (maxCreatedTime), maxCreatedTime);
      this.BindMaxModifiedTime(nameof (maxModifiedTime), maxModifiedTime);
      this.BindInt(nameof (maxReleases), maxReleases);
      this.BindByte("queryOrder", (byte) releaseQueryOrder);
      this.BindInt(nameof (releaseContinuationToken), releaseContinuationToken);
      this.BindBoolean(nameof (includeEnvironments), includeEnvironments);
      this.BindBoolean(nameof (includeArtifacts), includeArtifacts);
      this.BindBoolean(nameof (includeApprovals), includeApprovals);
      this.BindBoolean(nameof (includeManualInterventions), includeManualInterventions);
      this.BindIncludeVariables(includeVariables);
      this.BindBoolean(nameof (includeTags), includeTags);
      this.BindBoolean(nameof (includeDeletedReleaseDefinitions), includeDeletedReleaseDefinitions);
      this.BindString(nameof (artifactTypeId), artifactTypeId, 256, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
      this.BindString("SourceId", sourceId, 256, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
      this.BindString(nameof (artifactVersionId), artifactVersionId, 256, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
      this.BindString(nameof (sourceBranchFilter), sourceBranchFilter, 256, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
      this.BindIsDeleted(nameof (isDeleted), isDeleted);
      this.BindStringTable("@tagFilter", tagFilter);
      this.BindInt32Table("@releaseIdFilter", releaseIds);
      this.BindFolderPath(path);
    }
  }
}
