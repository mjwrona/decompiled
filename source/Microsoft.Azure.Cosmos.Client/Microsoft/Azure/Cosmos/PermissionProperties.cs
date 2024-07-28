// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.PermissionProperties
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Microsoft.Azure.Documents.Routing;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace Microsoft.Azure.Cosmos
{
  public class PermissionProperties
  {
    public PermissionProperties(
      string id,
      PermissionMode permissionMode,
      Container container,
      PartitionKey? resourcePartitionKey = null)
    {
      this.Id = id;
      this.PermissionMode = permissionMode;
      this.ResourceUri = UriFactory.CreateDocumentCollectionUri(container.Database.Id, container.Id).ToString();
      if (!resourcePartitionKey.HasValue)
        this.InternalResourcePartitionKey = (PartitionKeyInternal) null;
      else
        this.InternalResourcePartitionKey = resourcePartitionKey?.InternalKey;
    }

    public PermissionProperties(
      string id,
      PermissionMode permissionMode,
      Container container,
      PartitionKey resourcePartitionKey,
      string itemId)
    {
      this.Id = id;
      this.PermissionMode = permissionMode;
      this.ResourceUri = UriFactory.CreateDocumentUri(container.Database.Id, container.Id, itemId).ToString();
      this.InternalResourcePartitionKey = resourcePartitionKey.InternalKey;
    }

    internal PermissionProperties()
    {
    }

    [JsonProperty(PropertyName = "id")]
    public string Id { get; private set; }

    [JsonProperty(PropertyName = "resource")]
    public string ResourceUri { get; private set; }

    [JsonIgnore]
    public PartitionKey? ResourcePartitionKey
    {
      get
      {
        if (this.InternalResourcePartitionKey == null)
          return new PartitionKey?();
        return this.InternalResourcePartitionKey.ToObjectArray().Length != 0 ? new PartitionKey?(new PartitionKey(this.InternalResourcePartitionKey.ToObjectArray()[0])) : new PartitionKey?();
      }
      set
      {
        if (!value.HasValue || value.HasValue && value.Value.IsNone)
          this.InternalResourcePartitionKey = (PartitionKeyInternal) null;
        else
          this.InternalResourcePartitionKey = value.Value.InternalKey;
      }
    }

    [JsonConverter(typeof (StringEnumConverter))]
    [JsonProperty(PropertyName = "permissionMode")]
    public PermissionMode PermissionMode { get; private set; }

    [JsonProperty(PropertyName = "_token", NullValueHandling = NullValueHandling.Ignore)]
    public string Token { get; private set; }

    [JsonProperty(PropertyName = "_etag", NullValueHandling = NullValueHandling.Ignore)]
    public string ETag { get; private set; }

    [JsonConverter(typeof (Microsoft.Azure.Documents.UnixDateTimeConverter))]
    [JsonProperty(PropertyName = "_ts", NullValueHandling = NullValueHandling.Ignore)]
    public DateTime? LastModified { get; private set; }

    [JsonProperty(PropertyName = "_self", NullValueHandling = NullValueHandling.Ignore)]
    public string SelfLink { get; private set; }

    [JsonProperty(PropertyName = "_rid", NullValueHandling = NullValueHandling.Ignore)]
    internal string ResourceId { get; private set; }

    [JsonProperty(PropertyName = "resourcePartitionKey", NullValueHandling = NullValueHandling.Ignore)]
    internal PartitionKeyInternal InternalResourcePartitionKey { get; private set; }

    [JsonExtensionData]
    internal IDictionary<string, JToken> AdditionalProperties { get; private set; }
  }
}
