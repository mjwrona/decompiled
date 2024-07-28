// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.MachineGroupDeploymentInput
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AE7F604E-30D7-44A7-BE7B-AB7FB5A67B31
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.dll

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts
{
  public class MachineGroupDeploymentInput : DeploymentInput
  {
    public MachineGroupDeploymentInput() => this.Tags = (IList<string>) new List<string>();

    public MachineGroupDeploymentInput(
      int healthPercent,
      bool skipArtifactsDownload,
      bool enableAccessToken,
      int queueId,
      int timeoutInMinutes,
      IList<Demand> demands,
      IList<string> tags,
      string deploymentHealthOption)
      : this(healthPercent, skipArtifactsDownload, enableAccessToken, queueId, timeoutInMinutes, demands, tags, deploymentHealthOption, (IDictionary<string, string>) null, 1, string.Empty, (ArtifactsDownloadInput) null)
    {
    }

    public MachineGroupDeploymentInput(
      int healthPercent,
      bool skipArtifactsDownload,
      bool enableAccessToken,
      int queueId,
      int timeoutInMinutes,
      IList<Demand> demands,
      IList<string> tags,
      string deploymentHealthOption,
      IDictionary<string, string> overrideInputs,
      int jobCancelTimeoutInMinutes,
      string condition,
      ArtifactsDownloadInput artifactsDownloadInput)
      : base(skipArtifactsDownload, enableAccessToken, queueId, timeoutInMinutes, demands, jobCancelTimeoutInMinutes, condition, artifactsDownloadInput)
    {
      this.HealthPercent = healthPercent;
      this.DeploymentHealthOption = deploymentHealthOption;
      this.Tags = tags ?? (IList<string>) new List<string>();
      this.OverrideInputs = overrideInputs;
    }

    [DataMember]
    public int HealthPercent { get; set; }

    [DataMember]
    public string DeploymentHealthOption { get; set; }

    [DataMember]
    public IList<string> Tags { get; private set; }

    public override bool Equals(BaseDeploymentInput other)
    {
      MachineGroupDeploymentInput otherValue = other as MachineGroupDeploymentInput;
      if (otherValue != null && base.Equals((BaseDeploymentInput) otherValue) && this.HealthPercent == otherValue.HealthPercent)
      {
        int? count1 = this.Tags?.Count;
        int? count2 = otherValue.Tags?.Count;
        if (count1.GetValueOrDefault() == count2.GetValueOrDefault() & count1.HasValue == count2.HasValue && !(this.DeploymentHealthOption != otherValue.DeploymentHealthOption) && !this.Tags.Any<string>((Func<string, bool>) (t1 => !otherValue.Tags.Contains<string>(t1, (IEqualityComparer<string>) StringComparer.CurrentCultureIgnoreCase))))
          return true;
      }
      return false;
    }

    public override BaseDeploymentInput Clone() => (BaseDeploymentInput) new MachineGroupDeploymentInput(this.HealthPercent, this.SkipArtifactsDownload, this.EnableAccessToken, this.QueueId, this.TimeoutInMinutes, this.Demands, this.Tags, this.DeploymentHealthOption, this.OverrideInputs, this.JobCancelTimeoutInMinutes, this.Condition, this.ArtifactsDownloadInput);
  }
}
