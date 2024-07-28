// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Data.Components.ReleaseDefinitionSqlComponent11
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Data, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A69B0FB7-3028-4162-BA38-794D80D7A49A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Data.dll

using Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.DataAccess;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.DataBinders;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Data.Components
{
  [SuppressMessage("Microsoft.Maintainability", "CA1501:AvoidExcessiveInheritance", Justification = "Necessary to handle AT/DT mismatch")]
  public class ReleaseDefinitionSqlComponent11 : ReleaseDefinitionSqlComponent10
  {
    protected override IList<ReleaseDefinitionArtifactSourceMap> GetReleaseDefinitionArtifactSourceMap(
      ResultCollection resultCollection)
    {
      if (resultCollection == null)
        throw new ArgumentNullException(nameof (resultCollection));
      resultCollection.NextResult();
      return (IList<ReleaseDefinitionArtifactSourceMap>) resultCollection.GetCurrent<ReleaseDefinitionArtifactSourceMap>().Items;
    }

    protected override void BindReleaseTriggerTable(ReleaseDefinition releaseDefinition)
    {
      if (releaseDefinition == null)
        throw new ArgumentNullException(nameof (releaseDefinition));
      this.BindReleaseTriggerTable4("triggers", (IEnumerable<ReleaseTriggerBase>) releaseDefinition.Triggers, (IEnumerable<ArtifactSource>) releaseDefinition.LinkedArtifacts);
    }

    protected override void BindReleaseDefinitionArtifactSourceMap(ResultCollection resultCollection)
    {
      if (resultCollection == null)
        throw new ArgumentNullException(nameof (resultCollection));
      resultCollection.AddBinder<ReleaseDefinitionArtifactSourceMap>((ObjectBinder<ReleaseDefinitionArtifactSourceMap>) new ReleaseDefinitionArtifactSourceBinder((ReleaseManagementSqlResourceComponentBase) this));
    }

    protected override void BindArtifactsFilter(
      IEnumerable<string> artifactSourceIdFilter,
      string artifactTypeId)
    {
      this.BindString(nameof (artifactSourceIdFilter), artifactSourceIdFilter != null ? artifactSourceIdFilter.ElementAtOrDefault<string>(0) : (string) null, 256, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
      this.BindString("artifactTypeFilter", artifactTypeId, 256, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
    }

    [SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "This input has already been validated at above layer")]
    protected override void BindToReleaseDefinitionTable(
      ReleaseDefinition releaseDefinition,
      bool bindIds)
    {
      base.BindToReleaseDefinitionTable(releaseDefinition, bindIds);
      this.BindInt("revision", releaseDefinition.Revision);
    }
  }
}
