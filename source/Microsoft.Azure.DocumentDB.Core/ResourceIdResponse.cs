// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.ResourceIdResponse
// Assembly: Microsoft.Azure.DocumentDB.Core, Version=2.10.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 20BB0EB7-1465-494C-8F4E-F898448B85D9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DocumentDB.Core.dll

using Newtonsoft.Json;

namespace Microsoft.Azure.Documents
{
  internal sealed class ResourceIdResponse : Resource
  {
    [JsonProperty(PropertyName = "resourceId")]
    public string NewResourceId
    {
      get => this.GetValue<string>("resourceId");
      internal set => this.SetValue("resourceId", (object) value);
    }

    [JsonProperty(PropertyName = "partitionIndex")]
    public int PartitionIndex
    {
      get => this.GetValue<int>("partitionIndex");
      internal set => this.SetValue("partitionIndex", (object) value);
    }

    [JsonProperty(PropertyName = "serviceIndex")]
    public int ServiceIndex
    {
      get => this.GetValue<int>("serviceIndex");
      internal set => this.SetValue("serviceIndex", (object) value);
    }
  }
}
