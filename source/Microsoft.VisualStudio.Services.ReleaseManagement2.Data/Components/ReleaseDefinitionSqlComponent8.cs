// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Data.Components.ReleaseDefinitionSqlComponent8
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Data, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A69B0FB7-3028-4162-BA38-794D80D7A49A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Data.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.DataAccess;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Utilities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Data.Components
{
  [SuppressMessage("Microsoft.Maintainability", "CA1501:AvoidExcessiveInheritance", Justification = "Necessary to handle AT/DT mismatch")]
  public class ReleaseDefinitionSqlComponent8 : ReleaseDefinitionSqlComponent7
  {
    public override void BindDefinitionEnvironmentTable(IList<DefinitionEnvironment> environments) => this.BindDefinitionEnvironmentTable5("definitionEnvironments", (IEnumerable<DefinitionEnvironment>) environments);

    [SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "This input has already been validated at above layer")]
    protected override void BindToReleaseDefinitionTable(
      ReleaseDefinition releaseDefinition,
      bool bindIds)
    {
      this.BindInt("id", releaseDefinition.Id);
      this.BindString("Name", string.IsNullOrEmpty(releaseDefinition.Name) ? string.Empty : releaseDefinition.Name, 256, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
      this.BindGuid("CreatedBy", releaseDefinition.CreatedBy);
      this.BindNullableDateTime("CreatedOn", releaseDefinition.CreatedOn);
      this.BindGuid("ModifiedBy", releaseDefinition.ModifiedBy);
      this.BindNullableDateTime("ModifiedOn", releaseDefinition.ModifiedOn);
      this.BindString("Variables", ServerModelUtility.ToString((object) VariablesUtility.ReplaceSecretVariablesWithNull(releaseDefinition.Variables)), -1, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
      this.BindString("ReleaseNameFormat", string.IsNullOrEmpty(releaseDefinition.ReleaseNameFormat) ? string.Empty : releaseDefinition.ReleaseNameFormat, -1, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
    }

    [SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "These are optional parameters")]
    public override void SoftDeleteReleaseDefinition(
      Guid projectId,
      Guid requestorId,
      int releaseDefinitionId = 0,
      string comment = null,
      bool forceDelete = false)
    {
      this.PrepareStoredProcedure("Release.prc_SoftDeleteReleaseDefinition", projectId);
      this.BindInt(nameof (releaseDefinitionId), releaseDefinitionId);
      this.BindGuid("modifiedBy", requestorId);
      this.ExecuteNonQuery();
    }

    public override void DeleteReleaseDefinition(
      Guid projectId,
      int releaseDefinitionId,
      Guid requestorId)
    {
      this.PrepareStoredProcedure("Release.prc_DeleteReleaseDefinition", projectId);
      this.BindInt(nameof (releaseDefinitionId), releaseDefinitionId);
      this.BindGuid("modifiedBy", requestorId);
      this.ExecuteNonQuery();
    }

    protected virtual void BindReleaseDefinitionArtifactSourceMap(ResultCollection resultCollection)
    {
    }

    protected virtual void BindArtifactsFilter(
      IEnumerable<string> artifactSourceIdFilter,
      string artifactTypeId)
    {
      this.BindInt("artifactIdFilter", 0);
    }

    protected virtual void BindIsDeletedFilter(bool isDeletedFilter) => this.BindBoolean("excludeDeleted", !isDeletedFilter);

    protected virtual void AddTriggersBinder(ResultCollection resultCollection)
    {
    }

    protected virtual IList<ReleaseDefinitionArtifactSourceMap> GetReleaseDefinitionArtifactSourceMap(
      ResultCollection resultCollection)
    {
      return (IList<ReleaseDefinitionArtifactSourceMap>) new List<ReleaseDefinitionArtifactSourceMap>();
    }

    protected virtual IEnumerable<ReleaseTriggerBase> GetReleaseTriggers(
      ResultCollection resultCollection)
    {
      return (IEnumerable<ReleaseTriggerBase>) new List<ReleaseTriggerBase>();
    }

    protected virtual void BindIncludeTriggers(bool includeTriggers)
    {
    }

    protected virtual void BindMaxModifiedTime(DateTime? modifiedTime)
    {
    }

    protected virtual void BindMinReleaseDefinitionId(
      string parameterName,
      int minReleaseDefinitionId)
    {
    }

    protected virtual void BindMaxReleaseDefinitionsCount(
      string parameterName,
      int maxReleaseDefinitionsCount)
    {
    }
  }
}
