// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Data.Components.ReleaseSqlComponent49
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Data, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A69B0FB7-3028-4162-BA38-794D80D7A49A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Data.dll

using Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts;
using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Framework.Server.AuditLog;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.DataBinders;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Utilities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Data.Components
{
  [SuppressMessage("Microsoft.Maintainability", "CA1501:AvoidExcessiveInheritance", Justification = "Versioning mechanism for AT/DT")]
  public class ReleaseSqlComponent49 : ReleaseSqlComponent48
  {
    public override Release AddRelease(Guid projectId, Release release, string comment)
    {
      if (release == null)
        throw new ArgumentNullException(nameof (release));
      this.EnsureReleaseNameIsNotEmpty(release);
      this.PrepareForAuditingAction(ReleaseAuditConstants.ReleaseCreated, projectId: projectId, excludeSqlParameters: true);
      this.PrepareStoredProcedure("Release.prc_Release_Add", projectId);
      IEnumerable<PipelineArtifactSource> releaseArtifactSources = this.GetReleaseArtifactSources((IEnumerable<ArtifactSource>) release.LinkedArtifacts);
      this.BindString("releaseName", release.Name, 256, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
      this.BindString("description", release.Description, 4000, BindStringBehavior.NullToEmptyString, SqlDbType.NVarChar);
      this.BindByte("releaseStatus", (byte) release.Status);
      this.BindInt("releaseDefinitionId", release.ReleaseDefinitionId);
      this.BindInt("releaseDefinitionRevision", release.ReleaseDefinitionRevision);
      this.BindString("definitionJson", ReleaseDefinitionSnapshotUtility.ToJson(release.DefinitionSnapshot), -1, BindStringBehavior.NullToEmptyString, SqlDbType.NVarChar);
      this.BindString("variables", Microsoft.VisualStudio.Services.ReleaseManagement.Data.Utilities.ServerModelUtility.ToString((object) Microsoft.VisualStudio.Services.ReleaseManagement.Data.DataAccess.VariablesUtility.ReplaceSecretVariablesWithNull(release.Variables)), -1, BindStringBehavior.NullToEmptyString, SqlDbType.NVarChar);
      this.BindString("variableGroups", Microsoft.VisualStudio.Services.ReleaseManagement.Data.Utilities.ServerModelUtility.ToString((object) VariableGroupUtility.ClearSecrets(release.VariableGroups)), -1, BindStringBehavior.NullToEmptyString, SqlDbType.NVarChar);
      this.BindGuid("createdBy", release.CreatedBy);
      this.BindNullableGuid("createdFor", release.CreatedFor ?? Guid.Empty);
      this.BindGuid("modifiedBy", release.ModifiedBy);
      this.BindByte("reason", (byte) release.Reason);
      this.BindString("releaseNameFormat", release.ReleaseNameFormat, 256, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
      this.BindString(nameof (comment), comment, 2048, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
      this.BindReleaseEnvironments((IEnumerable<ReleaseEnvironment>) release.Environments);
      this.BindReleaseArtifactSourceTable(releaseArtifactSources);
      return this.GetReleaseObject(projectId);
    }

    [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "Override of base class method.")]
    protected override ReleaseBinder GetReleaseBinder(Guid projectId) => (ReleaseBinder) new ReleaseBinder4(this.RequestContext, (ReleaseManagementSqlResourceComponentBase) this);
  }
}
