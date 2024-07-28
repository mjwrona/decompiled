// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Data.Components.ReleaseDefinitionSqlComponent14
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Data, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A69B0FB7-3028-4162-BA38-794D80D7A49A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Data.dll

using Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.DataAccess;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.DataBinders;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Data.Components
{
  [SuppressMessage("Microsoft.Maintainability", "CA1501:AvoidExcessiveInheritance", Justification = "Necessary to handle AT/DT mismatch")]
  public class ReleaseDefinitionSqlComponent14 : ReleaseDefinitionSqlComponent13
  {
    [SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "This input has already been validated at above layer")]
    protected override void BindToReleaseDefinitionTable(
      ReleaseDefinition releaseDefinition,
      bool bindIds)
    {
      if (bindIds)
      {
        this.BindInt("id", releaseDefinition.Id);
        this.BindInt("revision", releaseDefinition.Revision);
      }
      this.BindString("Name", string.IsNullOrEmpty(releaseDefinition.Name) ? string.Empty : releaseDefinition.Name, 256, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
      this.BindGuid("CreatedBy", releaseDefinition.CreatedBy);
      this.BindNullableDateTime("CreatedOn", releaseDefinition.CreatedOn);
      this.BindGuid("ModifiedBy", releaseDefinition.ModifiedBy);
      this.BindNullableDateTime("ModifiedOn", releaseDefinition.ModifiedOn);
      this.BindString("Variables", Microsoft.VisualStudio.Services.ReleaseManagement.Data.Utilities.ServerModelUtility.ToString((object) Microsoft.VisualStudio.Services.ReleaseManagement.Data.DataAccess.VariablesUtility.ReplaceSecretVariablesWithNull(releaseDefinition.Variables)), -1, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
      this.BindString("ReleaseNameFormat", string.IsNullOrEmpty(releaseDefinition.ReleaseNameFormat) ? string.Empty : releaseDefinition.ReleaseNameFormat, -1, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
    }

    protected override void BindReleaseDefinitionArtifactSourceTable(
      IEnumerable<ArtifactSource> linkedArtifactSources)
    {
      this.BindReleaseDefinitionArtifactSourceTable3("definitionArtifactSources", linkedArtifactSources);
    }

    protected override ReleaseDefinitionArtifactSourceBinder GetReleaseDefinitionArtifactSourceBinder => (ReleaseDefinitionArtifactSourceBinder) new ReleaseDefinitionArtifactSourceBinder3((ReleaseManagementSqlResourceComponentBase) this);

    protected override ArtifactSourceBinder GetArtifactSourceBinder() => (ArtifactSourceBinder) new ArtifactSourceBinder2();
  }
}
