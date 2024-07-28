// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.EnvironmentRetentionPolicy
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AE7F604E-30D7-44A7-BE7B-AB7FB5A67B31
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.dll

using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts
{
  [DataContract]
  public class EnvironmentRetentionPolicy : ReleaseManagementSecuredObject
  {
    [DataMember]
    public int DaysToKeep { get; set; }

    [DataMember]
    public int ReleasesToKeep { get; set; }

    [DataMember]
    public bool RetainBuild { get; set; }

    public override bool Equals(object obj)
    {
      if (obj == null)
        return false;
      if (this == obj)
        return true;
      if (obj.GetType() != this.GetType())
        return false;
      EnvironmentRetentionPolicy environmentRetentionPolicy = (EnvironmentRetentionPolicy) obj;
      return this.DaysToKeep == environmentRetentionPolicy.DaysToKeep && this.ReleasesToKeep == environmentRetentionPolicy.ReleasesToKeep && this.RetainBuild == environmentRetentionPolicy.RetainBuild;
    }

    public override int GetHashCode() => (this.DaysToKeep * 397 ^ this.ReleasesToKeep) * 397 ^ this.RetainBuild.GetHashCode();
  }
}
