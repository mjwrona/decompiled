// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Data.Components.ReleaseSqlComponent32
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
  public class ReleaseSqlComponent32 : ReleaseSqlComponent31
  {
    public override KeyValuePair<int, string> GetReleaseDefinitionFolderPathAndId(
      Guid projectId,
      int releaseId)
    {
      this.PrepareStoredProcedure("Release.prc_GetDefinitionIdFromReleaseId", projectId);
      this.BindInt(nameof (releaseId), releaseId);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<KeyValuePair<int, string>>((ObjectBinder<KeyValuePair<int, string>>) new FolderPathAndDefinitionIdBinder());
        return resultCollection.GetCurrent<KeyValuePair<int, string>>().Items.FirstOrDefault<KeyValuePair<int, string>>();
      }
    }

    protected override void BindReleaseArtifactSourceTable(
      IEnumerable<PipelineArtifactSource> releaseArtifactSources)
    {
      this.BindReleaseArtifactSourceTable8(nameof (releaseArtifactSources), releaseArtifactSources);
    }

    protected override ReleaseArtifactSourceBinder GetReleaseArtifactSourceBinder() => (ReleaseArtifactSourceBinder) new ReleaseArtifactSourceBinder6((ReleaseManagementSqlResourceComponentBase) this);

    protected override ReleaseEnvironmentBinder GetReleaseEnvironmentBinder() => (ReleaseEnvironmentBinder) new ReleaseEnvironmentBinder3((ReleaseManagementSqlResourceComponentBase) this);

    protected override void BindReleaseEnvironments(
      IEnumerable<ReleaseEnvironment> releaseEnvironments)
    {
      this.BindReleaseEnvironmentTable11(nameof (releaseEnvironments), releaseEnvironments);
    }
  }
}
