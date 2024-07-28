// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.EnvironmentExecutionPolicy
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AE7F604E-30D7-44A7-BE7B-AB7FB5A67B31
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.dll

using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts
{
  [DataContract]
  public class EnvironmentExecutionPolicy : ReleaseManagementSecuredObject
  {
    [DataMember]
    public int ConcurrencyCount { get; set; }

    [DataMember]
    public int QueueDepthCount { get; set; }

    public EnvironmentExecutionPolicy()
    {
      this.ConcurrencyCount = 0;
      this.QueueDepthCount = 0;
    }

    public override bool Equals(object obj)
    {
      if (obj == null)
        return false;
      if (this == obj)
        return true;
      if (obj.GetType() != this.GetType())
        return false;
      EnvironmentExecutionPolicy environmentExecutionPolicy = (EnvironmentExecutionPolicy) obj;
      return this.ConcurrencyCount == environmentExecutionPolicy.ConcurrencyCount && this.QueueDepthCount == environmentExecutionPolicy.QueueDepthCount;
    }

    public override int GetHashCode() => this.ConcurrencyCount * 397 ^ this.QueueDepthCount;
  }
}
