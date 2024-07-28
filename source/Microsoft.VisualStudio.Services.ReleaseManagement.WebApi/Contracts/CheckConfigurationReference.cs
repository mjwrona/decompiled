// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.CheckConfigurationReference
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AE7F604E-30D7-44A7-BE7B-AB7FB5A67B31
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.dll

using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts
{
  [DataContract]
  public class CheckConfigurationReference : ReleaseManagementSecuredObject
  {
    public CheckConfigurationReference()
    {
    }

    public CheckConfigurationReference(int id, int version)
    {
      this.Id = id;
      this.Version = version;
    }

    [DataMember(EmitDefaultValue = false)]
    public int Id { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public int Version { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string ResourceName { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string ResourceType { get; set; }

    public override bool Equals(object obj) => obj != null && obj is CheckConfigurationReference configurationReference && this.Id == configurationReference.Id && this.Version == configurationReference.Version;

    public override int GetHashCode() => this.ToString().GetHashCode();
  }
}
