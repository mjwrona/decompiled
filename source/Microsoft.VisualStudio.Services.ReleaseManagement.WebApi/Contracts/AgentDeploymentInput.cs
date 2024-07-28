// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.AgentDeploymentInput
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AE7F604E-30D7-44A7-BE7B-AB7FB5A67B31
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.dll

using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts
{
  [DataContract]
  public class AgentDeploymentInput : DeploymentInput
  {
    private ExecutionInput parallelExecution;

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

    [DataMember(EmitDefaultValue = false)]
    public int ImageId { get; set; }

    [DataMember]
    public AgentSpecification AgentSpecification { get; set; }

    protected AgentDeploymentInput(
      bool skipArtifactsDownload,
      bool enableAccessToken,
      int queueId,
      int timeoutInMinutes,
      IList<Demand> demands,
      ExecutionInput parallelExecution)
      : this(skipArtifactsDownload, enableAccessToken, queueId, timeoutInMinutes, demands, parallelExecution, (IDictionary<string, string>) null, 1, string.Empty, (ArtifactsDownloadInput) null)
    {
    }

    protected AgentDeploymentInput(
      bool skipArtifactsDownload,
      bool enableAccessToken,
      int queueId,
      int timeoutInMinutes,
      IList<Demand> demands,
      ExecutionInput parallelExecution,
      IDictionary<string, string> overrideInputs,
      int jobCancelTimeoutInMinutes,
      string condition,
      ArtifactsDownloadInput artifactsDownloadInput)
      : base(skipArtifactsDownload, enableAccessToken, queueId, timeoutInMinutes, demands, jobCancelTimeoutInMinutes, condition, artifactsDownloadInput)
    {
      if (parallelExecution != null)
        this.parallelExecution = (ExecutionInput) parallelExecution.Clone();
      this.OverrideInputs = overrideInputs;
    }

    public AgentDeploymentInput()
    {
    }

    public override bool Equals(BaseDeploymentInput other) => other is AgentDeploymentInput other1 && base.Equals((BaseDeploymentInput) other1) && this.ParallelExecution.Equals(other1.ParallelExecution);

    public override BaseDeploymentInput Clone() => (BaseDeploymentInput) new AgentDeploymentInput(this.SkipArtifactsDownload, this.EnableAccessToken, this.QueueId, this.TimeoutInMinutes, this.Demands, this.ParallelExecution, this.OverrideInputs, this.JobCancelTimeoutInMinutes, this.Condition, this.ArtifactsDownloadInput);

    public bool IsDefaultParallelExecutionInput() => this.ParallelExecution.Equals((ExecutionInput) new NoneExecutionInput()) || this.ParallelExecution.Equals((ExecutionInput) new MultiConfigInput(string.Empty));

    internal override void SetSecuredObject(string token, int requiredPermissions)
    {
      base.SetSecuredObject(token, requiredPermissions);
      this.ParallelExecution?.SetSecuredObject(token, requiredPermissions);
      this.AgentSpecification?.SetSecuredObject(token, requiredPermissions);
    }
  }
}
