// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Data.DataBinders.DefinitionEnvironmentBinder1
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Data, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A69B0FB7-3028-4162-BA38-794D80D7A49A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Data.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Components;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Utilities;
using Newtonsoft.Json.Linq;
using System.Data;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Data.DataBinders
{
  public sealed class DefinitionEnvironmentBinder1 : DefinitionEnvironmentBinder
  {
    private SqlColumnBinder queueId = new SqlColumnBinder("QueueId");
    private SqlColumnBinder demands = new SqlColumnBinder("Demands");
    private SqlColumnBinder workflow = new SqlColumnBinder("Workflow");
    private SqlColumnBinder runoptions = new SqlColumnBinder("RunOptions");

    public DefinitionEnvironmentBinder1(
      ReleaseManagementSqlResourceComponentBase sqlComponent)
      : base(sqlComponent)
    {
    }

    protected override DefinitionEnvironment Bind()
    {
      Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.EnvironmentOptions environmentOptions = ServerModelUtility.GetWebApiEnvironmentOptions(this.runoptions.GetString((IDataReader) this.Reader, (string) null));
      string str = this.workflow.GetString((IDataReader) this.Reader, false);
      string demands = this.demands.GetString((IDataReader) this.Reader, true);
      int int32 = this.queueId.GetInt32((IDataReader) this.Reader, 0, 0);
      DefinitionEnvironment definitionEnvironment = base.Bind();
      bool artifactsDownload = environmentOptions.SkipArtifactsDownload;
      int timeoutInMinutes1 = environmentOptions.TimeoutInMinutes;
      bool enableAccessToken = environmentOptions.EnableAccessToken;
      int queueId = int32;
      int num1 = artifactsDownload ? 1 : 0;
      int timeoutInMinutes2 = timeoutInMinutes1;
      int num2 = enableAccessToken ? 1 : 0;
      JObject deploymentInputJobject = EnvironmentCompatExtensions.CreateCompatDeploymentInputJObject(demands, queueId, num1 != 0, timeoutInMinutes2, num2 != 0);
      DeployPhase deployPhase = new DeployPhase()
      {
        ReleaseDefinitionId = definitionEnvironment.ReleaseDefinitionId,
        DefinitionEnvironmentId = definitionEnvironment.Id,
        PhaseType = Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations.DeployPhaseTypes.AgentBasedDeployment,
        Name = string.Empty,
        Rank = 1,
        Workflow = str,
        DeploymentInput = deploymentInputJobject
      };
      definitionEnvironment.DeployPhases.Add(deployPhase);
      definitionEnvironment.EnvironmentOptions = new Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.EnvironmentOptions()
      {
        EmailNotificationType = environmentOptions.EmailNotificationType,
        EmailRecipients = environmentOptions.EmailRecipients
      };
      return definitionEnvironment;
    }
  }
}
