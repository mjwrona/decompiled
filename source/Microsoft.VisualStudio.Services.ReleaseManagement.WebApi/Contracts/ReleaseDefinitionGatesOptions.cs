// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.ReleaseDefinitionGatesOptions
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AE7F604E-30D7-44A7-BE7B-AB7FB5A67B31
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.dll

using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts
{
  [DataContract]
  public class ReleaseDefinitionGatesOptions : ReleaseManagementSecuredObject
  {
    [DataMember]
    public bool IsEnabled { get; set; }

    [DataMember]
    public int Timeout { get; set; }

    [DataMember]
    public int SamplingInterval { get; set; }

    [DataMember]
    public int StabilizationTime { get; set; }

    [DataMember]
    public int MinimumSuccessDuration { get; set; }

    public ReleaseDefinitionGatesOptions()
    {
      this.IsEnabled = false;
      this.Timeout = 1440;
      this.SamplingInterval = 15;
      this.StabilizationTime = 5;
      this.MinimumSuccessDuration = 0;
    }

    public override int GetHashCode() => base.GetHashCode();

    public override bool Equals(object obj) => obj is ReleaseDefinitionGatesOptions definitionGatesOptions && this.IsEnabled == definitionGatesOptions.IsEnabled && this.StabilizationTime == definitionGatesOptions.StabilizationTime && this.Timeout == definitionGatesOptions.Timeout && this.SamplingInterval == definitionGatesOptions.SamplingInterval && this.MinimumSuccessDuration == definitionGatesOptions.MinimumSuccessDuration;
  }
}
