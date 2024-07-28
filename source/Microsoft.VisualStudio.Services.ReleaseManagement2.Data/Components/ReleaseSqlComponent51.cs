// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Data.Components.ReleaseSqlComponent51
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Data, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A69B0FB7-3028-4162-BA38-794D80D7A49A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Data.dll

using Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts;
using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Utilities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Data.Components
{
  [SuppressMessage("Microsoft.Maintainability", "CA1501:AvoidExcessiveInheritance", Justification = "Versioning mechanism")]
  public class ReleaseSqlComponent51 : ReleaseSqlComponent50
  {
    public override Release UpdateDraftRelease(Guid projectId, Release release, string comment)
    {
      if (release == null)
        throw new ArgumentNullException(nameof (release));
      this.PrepareStoredProcedure("Release.prc_Release_UpdateDraft", projectId);
      IEnumerable<PipelineArtifactSource> releaseArtifactSources = this.GetReleaseArtifactSources((IEnumerable<ArtifactSource>) release.LinkedArtifacts);
      this.BindInt("releaseId", release.Id);
      this.BindString("releaseName", release.Name, 256, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
      this.BindString("description", release.Description, 4000, BindStringBehavior.NullToEmptyString, SqlDbType.NVarChar);
      this.BindString("definitionJson", ReleaseDefinitionSnapshotUtility.ToJson(release.DefinitionSnapshot), -1, BindStringBehavior.NullToEmptyString, SqlDbType.NVarChar);
      this.BindString("variables", Microsoft.VisualStudio.Services.ReleaseManagement.Data.Utilities.ServerModelUtility.ToString((object) Microsoft.VisualStudio.Services.ReleaseManagement.Data.DataAccess.VariablesUtility.ReplaceSecretVariablesWithNull(release.Variables)), -1, BindStringBehavior.NullToEmptyString, SqlDbType.NVarChar);
      this.BindString("variableGroups", Microsoft.VisualStudio.Services.ReleaseManagement.Data.Utilities.ServerModelUtility.ToString((object) VariableGroupUtility.ClearSecrets(release.VariableGroups)), -1, BindStringBehavior.NullToEmptyString, SqlDbType.NVarChar);
      this.BindGuid("modifiedBy", release.ModifiedBy);
      this.BindString(nameof (comment), comment, 2048, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
      this.BindInt("definitionSnapshotRevision", release.DefinitionSnapshotRevision);
      this.BindReleaseEnvironments((IEnumerable<ReleaseEnvironment>) release.Environments);
      this.BindReleaseArtifactSourceTable(releaseArtifactSources);
      return this.GetReleaseObject(projectId);
    }
  }
}
