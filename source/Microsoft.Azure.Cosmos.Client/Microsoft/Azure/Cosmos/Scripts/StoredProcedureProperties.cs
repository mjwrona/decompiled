// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Scripts.StoredProcedureProperties
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Microsoft.Azure.Documents;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace Microsoft.Azure.Cosmos.Scripts
{
  public class StoredProcedureProperties
  {
    private string id;
    private string body;

    public StoredProcedureProperties()
    {
    }

    public StoredProcedureProperties(string id, string body)
    {
      this.Id = id;
      this.Body = body;
    }

    [JsonProperty(PropertyName = "body")]
    public string Body
    {
      get => this.body;
      set => this.body = value ?? throw new ArgumentNullException(nameof (Body));
    }

    [JsonProperty(PropertyName = "id")]
    public string Id
    {
      get => this.id;
      set => this.id = value ?? throw new ArgumentNullException(nameof (Id));
    }

    [JsonProperty(PropertyName = "_etag", NullValueHandling = NullValueHandling.Ignore)]
    public string ETag { get; private set; }

    [JsonConverter(typeof (UnixDateTimeConverter))]
    [JsonProperty(PropertyName = "_ts", NullValueHandling = NullValueHandling.Ignore)]
    public DateTime? LastModified { get; private set; }

    [JsonProperty(PropertyName = "_self", NullValueHandling = NullValueHandling.Ignore)]
    public string SelfLink { get; private set; }

    [JsonProperty(PropertyName = "_rid", NullValueHandling = NullValueHandling.Ignore)]
    internal string ResourceId { get; private set; }

    [JsonExtensionData]
    internal IDictionary<string, JToken> AdditionalProperties { get; private set; }
  }
}
