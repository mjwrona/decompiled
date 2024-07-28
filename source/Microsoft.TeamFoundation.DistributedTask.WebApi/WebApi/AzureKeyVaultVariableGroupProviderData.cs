// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.WebApi.AzureKeyVaultVariableGroupProviderData
// Assembly: Microsoft.TeamFoundation.DistributedTask.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9201F3B5-DEAF-44A3-860C-DB7B277BB5C6
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.WebApi.dll

using System;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.DistributedTask.WebApi
{
  [DataContract]
  public class AzureKeyVaultVariableGroupProviderData : 
    VariableGroupProviderData,
    IEquatable<AzureKeyVaultVariableGroupProviderData>
  {
    [DataMember(EmitDefaultValue = true)]
    public Guid ServiceEndpointId { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string Vault { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public DateTime LastRefreshedOn { get; set; }

    public bool Equals(
      AzureKeyVaultVariableGroupProviderData azureKeyVaultVariableGroupProviderData)
    {
      return !(azureKeyVaultVariableGroupProviderData.ServiceEndpointId != this.ServiceEndpointId) && !(azureKeyVaultVariableGroupProviderData.Vault != this.Vault);
    }
  }
}
