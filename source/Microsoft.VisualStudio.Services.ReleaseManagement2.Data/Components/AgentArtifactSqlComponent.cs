// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Data.Components.AgentArtifactSqlComponent
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Data, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A69B0FB7-3028-4162-BA38-794D80D7A49A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Data.dll

using Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.DataBinders;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Data.Components
{
  public class AgentArtifactSqlComponent : ReleaseManagementSqlResourceComponentBase
  {
    [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "This signature is required by VSSF. Don't change.")]
    public static readonly ComponentFactory ComponentFactory = new ComponentFactory(new IComponentCreator[4]
    {
      (IComponentCreator) new ComponentCreator<AgentArtifactSqlComponent>(1),
      (IComponentCreator) new ComponentCreator<AgentArtifactSqlComponent2>(2),
      (IComponentCreator) new ComponentCreator<AgentArtifactSqlComponent3>(3),
      (IComponentCreator) new ComponentCreator<AgentArtifactSqlComponent4>(4)
    }, "ReleaseManagementAgentArtifact", "ReleaseManagement");

    public virtual IEnumerable<AgentArtifactDefinition> ListAgentArtifacts(
      Guid projectId,
      int releaseId)
    {
      this.PrepareStoredProcedure("Release.prc_AgentReleaseArtifacts_List", projectId);
      this.BindInt(nameof (releaseId), releaseId);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<AgentArtifactDefinition>((ObjectBinder<AgentArtifactDefinition>) this.GetAgentReleaseArtifactBinder());
        return (IEnumerable<AgentArtifactDefinition>) resultCollection.GetCurrent<AgentArtifactDefinition>().Items;
      }
    }

    [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "Override of base class method.")]
    protected virtual AgentReleaseArtifactBinder GetAgentReleaseArtifactBinder() => new AgentReleaseArtifactBinder();
  }
}
