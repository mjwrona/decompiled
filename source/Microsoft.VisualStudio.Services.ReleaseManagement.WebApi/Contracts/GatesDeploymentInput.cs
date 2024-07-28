// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.GatesDeploymentInput
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AE7F604E-30D7-44A7-BE7B-AB7FB5A67B31
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.dll

using Newtonsoft.Json;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts
{
  [DataContract]
  public class GatesDeploymentInput : BaseDeploymentInput
  {
    [DataMember]
    public int SamplingInterval { get; set; }

    [DataMember]
    public int StabilizationTime { get; set; }

    [DataMember]
    public int MinimumSuccessDuration { get; set; }

    [JsonConstructor]
    public GatesDeploymentInput()
    {
    }

    private GatesDeploymentInput(
      int timeoutInMinutes,
      int jobCancelTimeoutInMinutes,
      string condition,
      IDictionary<string, string> overrideInputs,
      int samplingInterval,
      int stabilizationTime,
      int minimumSuccessDuration)
      : base(timeoutInMinutes, jobCancelTimeoutInMinutes, condition, overrideInputs)
    {
      this.SamplingInterval = samplingInterval;
      this.StabilizationTime = stabilizationTime;
      this.MinimumSuccessDuration = minimumSuccessDuration;
    }

    public override bool Equals(BaseDeploymentInput other) => other is GatesDeploymentInput other1 && base.Equals((BaseDeploymentInput) other1) && this.SamplingInterval == other1.SamplingInterval && this.StabilizationTime == other1.StabilizationTime && this.MinimumSuccessDuration == other1.MinimumSuccessDuration;

    public override BaseDeploymentInput Clone() => (BaseDeploymentInput) new GatesDeploymentInput(this.TimeoutInMinutes, this.JobCancelTimeoutInMinutes, this.Condition, this.OverrideInputs, this.SamplingInterval, this.StabilizationTime, this.MinimumSuccessDuration);
  }
}
