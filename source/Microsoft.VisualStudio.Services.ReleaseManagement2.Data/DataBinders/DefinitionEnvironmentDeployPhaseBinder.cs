// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Data.DataBinders.DefinitionEnvironmentDeployPhaseBinder
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Data, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A69B0FB7-3028-4162-BA38-794D80D7A49A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Data.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Utilities;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts;
using Newtonsoft.Json.Linq;
using System.Data;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Data.DataBinders
{
  public class DefinitionEnvironmentDeployPhaseBinder : ObjectBinder<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.DeployPhase>
  {
    private SqlColumnBinder definitionId = new SqlColumnBinder("DefinitionId");
    private SqlColumnBinder definitionEnvironmentId = new SqlColumnBinder("DefinitionEnvironmentId");
    private SqlColumnBinder rank = new SqlColumnBinder("Rank");
    private SqlColumnBinder phaseType = new SqlColumnBinder("PhaseType");
    private SqlColumnBinder name = new SqlColumnBinder("Name");
    private SqlColumnBinder workflow = new SqlColumnBinder("Workflow");
    private SqlColumnBinder deploymentInput = new SqlColumnBinder("DeploymentInput");

    protected override Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.DeployPhase Bind()
    {
      string json = this.deploymentInput.GetString((IDataReader) this.Reader, true);
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.DeployPhase deployPhase = new Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.DeployPhase()
      {
        ReleaseDefinitionId = this.definitionId.GetInt32((IDataReader) this.Reader),
        DefinitionEnvironmentId = this.definitionEnvironmentId.GetInt32((IDataReader) this.Reader),
        Rank = this.rank.GetInt32((IDataReader) this.Reader),
        PhaseType = (DeployPhaseTypes) this.phaseType.GetByte((IDataReader) this.Reader),
        Name = this.name.GetString((IDataReader) this.Reader, false),
        Workflow = this.workflow.GetString((IDataReader) this.Reader, true),
        DeploymentInput = string.IsNullOrEmpty(json) ? (JObject) null : JObject.Parse(json)
      };
      if (string.IsNullOrWhiteSpace(deployPhase.Name))
        deployPhase.Name = ServerModelUtility.GetDefaultPhaseName(deployPhase.PhaseType);
      DefinitionEnvironmentDeployPhaseBinder.NormalizeDeployPhase(deployPhase);
      return deployPhase;
    }

    private static void NormalizeDeployPhase(Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.DeployPhase deployPhase)
    {
      if (deployPhase.PhaseType != DeployPhaseTypes.RunOnServer || deployPhase.DeploymentInput != null)
        return;
      deployPhase.DeploymentInput = JObject.FromObject((object) new ServerDeploymentInput()
      {
        ParallelExecution = (ExecutionInput) new NoneExecutionInput()
      });
    }
  }
}
