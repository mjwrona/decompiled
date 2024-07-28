// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Data.Components.ReleaseDefinitionSqlComponent30
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Data, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A69B0FB7-3028-4162-BA38-794D80D7A49A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Data.dll

using Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Framework.Server.AuditLog;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.DataAccess;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.DataBinders;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Utilities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Data.Components
{
  [SuppressMessage("Microsoft.Maintainability", "CA1501:AvoidExcessiveInheritance", Justification = "Versioning mechanism")]
  public class ReleaseDefinitionSqlComponent30 : ReleaseDefinitionSqlComponent29
  {
    public override void BindDefinitionEnvironmentTable(IList<DefinitionEnvironment> environments) => this.BindDefinitionEnvironmentTable10("definitionEnvironments", (IEnumerable<DefinitionEnvironment>) environments);

    protected override DefinitionEnvironmentBinder GetDefinitionEnvironmentBinder() => (DefinitionEnvironmentBinder) new DefinitionEnvironmentBinder3((ReleaseManagementSqlResourceComponentBase) this);

    public override IDictionary<int, string> GetFolderPaths(
      Guid projectId,
      IEnumerable<int> definitionIds)
    {
      this.PrepareStoredProcedure("Release.prc_GetFolderPaths", projectId);
      this.BindInt32Table(nameof (definitionIds), definitionIds);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<ShallowReference>((ObjectBinder<ShallowReference>) new ShallowReferenceBinder());
        return (IDictionary<int, string>) resultCollection.GetCurrent<ShallowReference>().Items.ToDictionary<ShallowReference, int, string>((System.Func<ShallowReference, int>) (e => e.Id), (System.Func<ShallowReference, string>) (e => PathHelper.DBPathToServerPath(e.Name)));
      }
    }

    public override ReleaseDefinition UpdateReleaseDefinition(
      Guid projectId,
      ReleaseDefinition releaseDefinition,
      string oldToken,
      string newToken)
    {
      this.PrepareForAuditingAction(ReleaseAuditConstants.ReleasePipelineModified, projectId: projectId, excludeSqlParameters: true);
      this.PrepareStoredProcedure("Release.prc_ReleaseDefinition_Update", projectId);
      return this.AddOrUpdateReleaseDefinition(releaseDefinition, true, oldToken, newToken);
    }

    [SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "reviewed")]
    [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "This is an override of a base class method.")]
    [SuppressMessage("Microsoft.Design", "CA1062:ValidateArgumentsOfPublicMethods", Justification = "Arguments are already validated at service layer.")]
    protected override ReleaseDefinition AddOrUpdateReleaseDefinition(
      ReleaseDefinition releaseDefinition,
      bool bindIds,
      string oldToken = null,
      string newToken = null)
    {
      IEnumerable<ArtifactSource> linkedArtifactSources = releaseDefinition.LinkedArtifacts.Select<ArtifactSource, ArtifactSource>((System.Func<ArtifactSource, ArtifactSource>) (e => e));
      IEnumerable<DefinitionEnvironmentStep> steps = releaseDefinition.Environments.SelectMany<DefinitionEnvironment, DefinitionEnvironmentStep>((System.Func<DefinitionEnvironment, IEnumerable<DefinitionEnvironmentStep>>) (e => (IEnumerable<DefinitionEnvironmentStep>) e.GetStepsForTests));
      IEnumerable<ForeignKeyReference> parentChildReference = ReleaseDefinitionSqlComponent.GetParentChildReference<DefinitionEnvironment, DefinitionEnvironmentStep>((IEnumerable<DefinitionEnvironment>) releaseDefinition.Environments, (System.Func<DefinitionEnvironment, IList<DefinitionEnvironmentStep>>) (e => e.GetStepsForTests));
      this.BindToReleaseDefinitionTable(releaseDefinition, bindIds);
      this.BindDefinitionEnvironmentTable(releaseDefinition.Environments);
      this.BindDefinitionEnvironmentSteps(steps);
      this.BindForeignKeyReferenceTable("definitionEnvironment_Step", parentChildReference);
      this.BindReleaseDefinitionArtifactSourceTable(linkedArtifactSources);
      this.BindReleaseTriggerTable(releaseDefinition);
      this.BindDeployPhases(releaseDefinition);
      this.BindFolderPath(releaseDefinition.Path);
      if (bindIds)
        this.BindSecurityTokens(oldToken, newToken);
      return this.GetReleaseDefinitionObject(false, false);
    }

    protected override void BindSecurityTokens(string oldToken, string newToken)
    {
      this.BindString("originalSecurityToken", oldToken, -1, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
      this.BindString("newSecurityToken", newToken, -1, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
    }
  }
}
