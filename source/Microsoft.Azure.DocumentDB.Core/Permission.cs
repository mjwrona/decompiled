// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.Permission
// Assembly: Microsoft.Azure.DocumentDB.Core, Version=2.10.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 20BB0EB7-1465-494C-8F4E-F898448B85D9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DocumentDB.Core.dll

using Microsoft.Azure.Documents.Routing;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Microsoft.Azure.Documents
{
  public class Permission : Resource
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
