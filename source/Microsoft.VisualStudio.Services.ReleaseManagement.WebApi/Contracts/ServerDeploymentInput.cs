// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.ServerDeploymentInput
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AE7F604E-30D7-44A7-BE7B-AB7FB5A67B31
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.dll

using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts
{
  [DataContract]
  public class ServerDeploymentInput : BaseDeploymentInput
  {
    private ExecutionInput parallelExecution;

    [JsonConstructor]
    public ServerDeploymentInput()
    {
    }

    internal override void SetSecuredObject(string token, int requiredPermissions)
    {
      base.SetSecuredObject(token, requiredPermissions);
      this.ParallelExecution?.SetSecuredObject(token, requiredPermissions);
    }

    private ServerDeploymentInput(
      ExecutionInput parallelExecutionInput,
      int timeoutInMinutes,
      string condition)
      : base(timeoutInMinutes)
    {
      if (parallelExecutionInput != null)
        this.parallelExecution = parallelExecutionInput.Clone() as ExecutionInput;
      this.Condition = condition;
    }

    [DataMember]
    public ExecutionInput ParallelExecution
    {
      get
      {
        if (this.parallelExecution == null)
          this.parallelExecution = (ExecutionInput) new NoneExecutionInput();
        return this.parallelExecution;
      }
      set => this.parallelExecution = value;
    }

    public override bool Equals(BaseDeploymentInput other) => other is ServerDeploymentInput serverDeploymentInput && this.ParallelExecution.Equals(serverDeploymentInput.ParallelExecution) && !(this.Condition != serverDeploymentInput.Condition);

    public override BaseDeploymentInput Clone() => (BaseDeploymentInput) new ServerDeploymentInput(this.parallelExecution, this.TimeoutInMinutes, this.Condition);
  }
}
