// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.AzureManagementGroup
// Assembly: Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 52155B17-64DE-4C30-B15E-F2E70DBED717
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.dll

using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi
{
  [DataContract]
  public class AzureManagementGroup
  {
    [DataMember]
    [JsonProperty(PropertyName = "Name")]
    public string Name { get; set; }

    [DataMember]
    [JsonProperty(PropertyName = "Id")]
    public string Id { get; set; }

    [DataMember]
    [JsonProperty(PropertyName = "displayName")]
    public string DisplayName { get; set; }

    [DataMember]
    public string TenantId { get; set; }
  }
}
