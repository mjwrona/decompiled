// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.Permission
// Assembly: Microsoft.Azure.Cosmos.Direct, Version=3.29.4.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: FFE3C00D-4333-4294-8947-B1C93A852E2F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Direct.dll

using Microsoft.Azure.Documents.Routing;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Microsoft.Azure.Documents
{
  internal class Permission : Resource
  {
    [JsonProperty(PropertyName = "resource")]
    public string ResourceLink
    {
      get => this.GetValue<string>("resource");
      set => this.SetValue("resource", (object) value);
    }

    [JsonProperty(PropertyName = "resourcePartitionKey")]
    public PartitionKey ResourcePartitionKey
    {
      get
      {
        PartitionKeyInternal partitionKeyInternal = this.GetValue<PartitionKeyInternal>("resourcePartitionKey");
        return partitionKeyInternal != null ? new PartitionKey(partitionKeyInternal.ToObjectArray()[0]) : (PartitionKey) null;
      }
      set
      {
        if (value == null)
          return;
        this.SetValue("resourcePartitionKey", (object) value.InternalKey);
      }
    }

    [JsonConverter(typeof (StringEnumConverter))]
    [JsonProperty(PropertyName = "permissionMode")]
    public PermissionMode PermissionMode
    {
      get => this.GetValue<PermissionMode>("permissionMode", PermissionMode.All);
      set => this.SetValue("permissionMode", (object) value.ToString());
    }

    [JsonProperty(PropertyName = "_token")]
    public string Token
    {
      get => this.GetValue<string>("_token");
      private set => this.SetValue("_token", (object) value);
    }
  }
}
