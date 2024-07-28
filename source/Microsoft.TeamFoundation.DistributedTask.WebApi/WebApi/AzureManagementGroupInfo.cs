// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.WebApi.AzureManagementGroupInfo
// Assembly: Microsoft.TeamFoundation.DistributedTask.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9201F3B5-DEAF-44A3-860C-DB7B277BB5C6
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.WebApi.dll

using Newtonsoft.Json;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.DistributedTask.WebApi
{
  public class AzureManagementGroupInfo
  {
    public IDictionary<string, string> Properties;

    public AzureManagementGroupInfo() => this.Properties = (IDictionary<string, string>) new Dictionary<string, string>();

    [JsonProperty(PropertyName = "id")]
    public string Id { get; set; }

    [JsonProperty(PropertyName = "type")]
    public string Type { get; set; }

    [JsonProperty(PropertyName = "name")]
    public string Name { get; set; }
  }
}
