// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.AzurePermission
// Assembly: Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 52155B17-64DE-4C30-B15E-F2E70DBED717
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.dll

using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi
{
  [JsonConverter(typeof (AzurePermissionJsonConverter))]
  [KnownType(typeof (AzureKeyVaultPermission))]
  [DataContract]
  public abstract class AzurePermission
  {
    [DataMember]
    public string ResourceProvider { get; set; }

    [DataMember(EmitDefaultValue = true)]
    public bool Provisioned { get; set; }

    internal AzurePermission(string resourceProvider) => this.ResourceProvider = resourceProvider;
  }
}
